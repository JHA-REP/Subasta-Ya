using SubastaYa.Aplicacion.Comun.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;
using SubastaYa.Dominio.Interfaces;
using SubastaYa.Dominio.Reglas;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.FinalizarSubasta;

/// <summary>
/// Manejador de finalización automática de subastas vencidas.
/// Contiene toda la lógica de negocio para:
/// - Determinación de ganador o subasta desierta.
/// - Liquidación financiera (débito ganador, crédito vendedor).
/// - Registro de movimientos contables y de auditoría.
/// - Notificación en tiempo real via INotificadorSubastas.
/// Invocado exclusivamente por el ProcesadorSubastas (BackgroundService).
/// </summary>
public class SubastaFinalizacionManejador
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IRepositorio<Puja> _repositorioPujas;
    private readonly IRepositorio<Billetera> _repositorioBilleteras;
    private readonly IRepositorio<Usuario> _repositorioUsuarios;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly INotificadorSubastas _notificador;
    private readonly IAuditoriaServicio _auditoria;

    public SubastaFinalizacionManejador(
        IRepositorio<Subasta> repositorioSubastas,
        IRepositorio<Puja> repositorioPujas,
        IRepositorio<Billetera> repositorioBilleteras,
        IRepositorio<Usuario> repositorioUsuarios,
        IUnidadDeTrabajo unidadDeTrabajo,
        INotificadorSubastas notificador,
        IAuditoriaServicio auditoria)
    {
        _repositorioSubastas = repositorioSubastas;
        _repositorioPujas = repositorioPujas;
        _repositorioBilleteras = repositorioBilleteras;
        _repositorioUsuarios = repositorioUsuarios;
        _unidadDeTrabajo = unidadDeTrabajo;
        _notificador = notificador;
        _auditoria = auditoria;
    }

    /// <summary>
    /// Procesamiento de todas las subastas activas cuya fecha de finalización fue alcanzada.
    /// Retorna el listado de liquidaciones realizadas en el ciclo.
    /// </summary>
    public async Task<IEnumerable<ResultadoLiquidacionDto>> EjecucionAsync(SubastaFinalizacionComando comando)
    {
        var resultados = new List<ResultadoLiquidacionDto>();
        var fechaCorte = comando.FechaCorte ?? DateTime.UtcNow;

        // Solo subastas activas vencidas
        var subastasVencidas = await _repositorioSubastas.FiltradasAsync(
            s => s.Estado == EstadoSubasta.Activa && s.FechaFin <= fechaCorte);

        foreach (var subasta in subastasVencidas)
        {
            SubastaFinalizacionReglas.ValidacionEstadoParaFinalizacion(subasta.Estado);
            SubastaFinalizacionReglas.ValidacionFechaVencimiento(subasta.FechaFin, fechaCorte);

            var pujas = await _repositorioPujas.FiltradasAsync(p => p.SubastaId == subasta.Id);
            var listaPujas = pujas.OrderByDescending(p => p.Monto).ToList();

            ResultadoLiquidacionDto resultado;
            if (listaPujas.Count > 0)
                resultado = await LiquidacionConGanadorAsync(subasta, listaPujas, fechaCorte);
            else
                resultado = await DeclaracionDesertaAsync(subasta, fechaCorte);

            resultados.Add(resultado);
        }

        return resultados;
    }

    /// <summary>
    /// Liquidación de subasta con ganador: débito, crédito, movimientos, auditoría y notificación.
    /// Protegida por Optimistic Locking (RowVersion) para evitar doble liquidación.
    /// </summary>
    private async Task<ResultadoLiquidacionDto> LiquidacionConGanadorAsync(
        Subasta subasta, List<Puja> pujasOrdenadas, DateTime fechaCorte)
    {
        await _unidadDeTrabajo.InicioTransaccionAsync();

        try
        {
            var pujaGanadora = pujasOrdenadas.First();
            var contadorMovimientos = 0;

            // 1. Cambio de estado de la subasta
            subasta.ResultadoFinalizacion(pujaGanadora.PostorId, pujaGanadora.Monto);
            _repositorioSubastas.Modificacion(subasta);

            // 2. Débito al ganador
            var billeterasGanador = await _repositorioBilleteras
                .FiltradasAsync(b => b.UsuarioId == pujaGanadora.PostorId);
            var billeteraGanador = billeterasGanador.First();

            billeteraGanador.ConfirmacionRetencion(pujaGanadora.Monto);
            billeteraGanador.Movimientos.Add(new MovimientoContable
            {
                Tipo = TipoMovimiento.Debito,
                Monto = pujaGanadora.Monto,
                Concepto = $"Débito por adjudicación de subasta #{subasta.Id} — {subasta.Titulo}",
                FechaMovimiento = fechaCorte
            });
            _repositorioBilleteras.Modificacion(billeteraGanador);
            contadorMovimientos++;

            // 3. Crédito al vendedor
            var billeterasVendedor = await _repositorioBilleteras
                .FiltradasAsync(b => b.UsuarioId == subasta.VendedorId);
            var billeteraVendedor = billeterasVendedor.First();

            billeteraVendedor.AcreditacionSaldo(pujaGanadora.Monto);
            billeteraVendedor.Movimientos.Add(new MovimientoContable
            {
                Tipo = TipoMovimiento.Credito,
                Monto = pujaGanadora.Monto,
                Concepto = $"Crédito por venta en subasta #{subasta.Id} — {subasta.Titulo}",
                FechaMovimiento = fechaCorte
            });
            _repositorioBilleteras.Modificacion(billeteraVendedor);
            contadorMovimientos++;

            // 4. Retenciones de perdedores:
            // Nota de diseño: En PujaRegistroManejador, la liberación de saldo al postor superado
            // ya se ejecuta de forma inmediata al registrarse cada nueva puja mayor.
            // Por lo tanto, al finalizar la subasta únicamente el ganador mantiene saldo retenido por esta subasta.

            // Persistencia transaccional (Optimistic Locking via RowVersion protege contra doble ejecución)
            await _unidadDeTrabajo.ConfirmacionAsync();
            await _unidadDeTrabajo.ConfirmacionTransaccionAsync();

            // === Post-transacción: auditoría y notificaciones ===

            var ganador = await _repositorioUsuarios.PorIdAsync(pujaGanadora.PostorId);

            // Auditoría: cambio de estado
            await _auditoria.RegistroAsync(
                TipoAccionAuditoria.CambioEstadoSubasta,
                nameof(Subasta),
                subasta.Id,
                new { estadoAnterior = nameof(EstadoSubasta.Activa), estadoNuevo = nameof(EstadoSubasta.Finalizada) },
                "Worker");

            // Auditoría: liquidación
            await _auditoria.RegistroAsync(
                TipoAccionAuditoria.LiquidacionCompletada,
                nameof(Subasta),
                subasta.Id,
                new
                {
                    ganadorId = pujaGanadora.PostorId,
                    ganadorAlias = ganador?.Alias,
                    montoFinal = pujaGanadora.Monto,
                    movimientosGenerados = contadorMovimientos,
                    fechaCorte
                },
                "Worker");

            // Auditoría: finalización Worker
            await _auditoria.RegistroAsync(
                TipoAccionAuditoria.FinalizacionWorker,
                nameof(Subasta),
                subasta.Id,
                new { cicloFecha = fechaCorte },
                "Worker");

            // Notificación en tiempo real
            await _notificador.EventoSubastaFinalizada(new SubastaFinalizadaDto
            {
                SubastaId = subasta.Id,
                Titulo = subasta.Titulo,
                EstadoResultante = EstadoSubasta.Finalizada,
                GanadorId = pujaGanadora.PostorId,
                GanadorAlias = ganador?.Alias ?? "Desconocido",
                MontoFinal = pujaGanadora.Monto,
                FechaFinalizacion = fechaCorte
            });

            return new ResultadoLiquidacionDto
            {
                SubastaId = subasta.Id,
                Titulo = subasta.Titulo,
                EstadoResultante = EstadoSubasta.Finalizada,
                MontoLiquidado = pujaGanadora.Monto,
                MovimientosGenerados = contadorMovimientos
            };
        }
        catch
        {
            await _unidadDeTrabajo.ReversionTransaccionAsync();
            throw;
        }
    }

    /// <summary>
    /// Declaración de subasta desierta: cambio de estado, auditoría y notificación.
    /// </summary>
    private async Task<ResultadoLiquidacionDto> DeclaracionDesertaAsync(
        Subasta subasta, DateTime fechaCorte)
    {
        await _unidadDeTrabajo.InicioTransaccionAsync();

        try
        {
            subasta.ResultadoDesierta();
            _repositorioSubastas.Modificacion(subasta);

            await _unidadDeTrabajo.ConfirmacionAsync();
            await _unidadDeTrabajo.ConfirmacionTransaccionAsync();

            // Auditoría
            await _auditoria.RegistroAsync(
                TipoAccionAuditoria.DeclaracionDesierta,
                nameof(Subasta),
                subasta.Id,
                new { titulo = subasta.Titulo, fechaCorte },
                "Worker");

            await _auditoria.RegistroAsync(
                TipoAccionAuditoria.CambioEstadoSubasta,
                nameof(Subasta),
                subasta.Id,
                new { estadoAnterior = nameof(EstadoSubasta.Activa), estadoNuevo = nameof(EstadoSubasta.Desierta) },
                "Worker");

            await _auditoria.RegistroAsync(
                TipoAccionAuditoria.FinalizacionWorker,
                nameof(Subasta),
                subasta.Id,
                new { cicloFecha = fechaCorte, resultado = "Desierta" },
                "Worker");

            // Notificación en tiempo real
            await _notificador.EventoSubastaDesierta(subasta.Id);

            return new ResultadoLiquidacionDto
            {
                SubastaId = subasta.Id,
                Titulo = subasta.Titulo,
                EstadoResultante = EstadoSubasta.Desierta,
                MontoLiquidado = null,
                MovimientosGenerados = 0
            };
        }
        catch
        {
            await _unidadDeTrabajo.ReversionTransaccionAsync();
            throw;
        }
    }
}
