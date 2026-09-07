using SubastaYa.Aplicacion.CasosDeUso.Subastas.Comandos;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;
using SubastaYa.Dominio.Interfaces;
using SubastaYa.Dominio.Reglas;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.Manejadores;

/// <summary>
/// Manejador de finalización automática de subastas vencidas.
/// Contiene toda la lógica de negocio para:
/// - Determinación de ganador o subasta desierta.
/// - Liquidación financiera (débito ganador, crédito vendedor).
/// - Registro de movimientos contables.
/// - Notificación en tiempo real via INotificadorSubastas.
/// </summary>
public class SubastaFinalizacionManejador
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IRepositorio<Puja> _repositorioPujas;
    private readonly IRepositorio<Billetera> _repositorioBilleteras;
    private readonly IRepositorio<Usuario> _repositorioUsuarios;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly INotificadorSubastas _notificador;

    public SubastaFinalizacionManejador(
        IRepositorio<Subasta> repositorioSubastas,
        IRepositorio<Puja> repositorioPujas,
        IRepositorio<Billetera> repositorioBilleteras,
        IRepositorio<Usuario> repositorioUsuarios,
        IUnidadDeTrabajo unidadDeTrabajo,
        INotificadorSubastas notificador)
    {
        _repositorioSubastas = repositorioSubastas;
        _repositorioPujas = repositorioPujas;
        _repositorioBilleteras = repositorioBilleteras;
        _repositorioUsuarios = repositorioUsuarios;
        _unidadDeTrabajo = unidadDeTrabajo;
        _notificador = notificador;
    }

    /// <summary>
    /// Ejecución del procesamiento de subastas vencidas.
    /// Retorna el listado de liquidaciones realizadas en el ciclo.
    /// </summary>
    public async Task<IEnumerable<ResultadoLiquidacionDto>> EjecucionAsync(SubastaFinalizacionComando comando)
    {
        var resultados = new List<ResultadoLiquidacionDto>();
        var fechaCorte = comando.FechaCorte ?? DateTime.UtcNow;

        // Consulta de subastas activas cuya fecha de finalización fue alcanzada
        var subastasVencidas = await _repositorioSubastas.FiltradasAsync(
            s => s.Estado == EstadoSubasta.Activa && s.FechaFin <= fechaCorte);

        foreach (var subasta in subastasVencidas)
        {
            // Validaciones de dominio
            SubastaFinalizacionReglas.ValidacionEstadoParaFinalizacion(subasta.Estado);
            SubastaFinalizacionReglas.ValidacionFechaVencimiento(subasta.FechaFin, fechaCorte);

            var pujas = await _repositorioPujas.FiltradasAsync(p => p.SubastaId == subasta.Id);
            var listaPujas = pujas.OrderByDescending(p => p.Monto).ToList();

            if (listaPujas.Count > 0)
            {
                var resultado = await LiquidacionConGanadorAsync(subasta, listaPujas, fechaCorte);
                resultados.Add(resultado);
            }
            else
            {
                var resultado = await DeclaracionDesertaAsync(subasta, fechaCorte);
                resultados.Add(resultado);
            }
        }

        return resultados;
    }

    /// <summary>
    /// Liquidación de subasta con ganador: débito, crédito, movimientos y notificación.
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

            // 2. Débito al ganador: confirmar retención (saldo retenido → 0)
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

            // 3. Crédito al vendedor: acreditación del monto liquidado
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

            // Persistencia transaccional
            await _unidadDeTrabajo.ConfirmacionAsync();
            await _unidadDeTrabajo.ConfirmacionTransaccionAsync();

            // 4. Notificación en tiempo real (fuera de la transacción)
            var ganador = await _repositorioUsuarios.PorIdAsync(pujaGanadora.PostorId);

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
    /// Declaración de subasta desierta: cambio de estado y notificación.
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

            // Notificación en tiempo real (fuera de la transacción)
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
