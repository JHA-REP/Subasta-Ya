using SubastaYa.Aplicacion.CasosDeUso.Pujas.ListarPujasPorSubasta;
using SubastaYa.Aplicacion.Comun.DTOs;
using SubastaYa.Aplicacion.Comun.Interfaces;
using SubastaYa.Aplicacion.Comun.Mapeos;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;
using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Interfaces;
using SubastaYa.Dominio.Reglas;

namespace SubastaYa.Aplicacion.CasosDeUso.Pujas.RegistrarPuja;

/// <summary>
/// Manejador de registro de puja.
/// Responsabilidades:
/// - Validar reglas de negocio.
/// - Ejecutar transacción ACID (retención/liberación de saldos).
/// - Auditar el evento.
/// - Emitir notificaciones en tiempo real (puja + extensión Anti-Sniping si aplica).
/// No contiene lógica de dominio propia — delega a reglas y entidades.
/// </summary>
public class PujaRegistroManejador : IComandoManejador<PujaRegistroComando, PujaDto>
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IRepositorio<Puja> _repositorioPujas;
    private readonly IRepositorio<Billetera> _repositorioBilleteras;
    private readonly IRepositorio<Usuario> _repositorioUsuarios;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly INotificadorSubastas _notificador;
    private readonly IAuditoriaServicio _auditoria;

    public PujaRegistroManejador(
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


    public async Task<PujaDto> EjecucionAsync(PujaRegistroComando comando)
    {
        var fechaHoraActual = DateTime.UtcNow;

        await _unidadDeTrabajo.InicioTransaccionAsync();

        try
        {
            var subasta = await _repositorioSubastas.PorIdAsync(comando.SubastaId);
            if (subasta == null)
                throw new ExcepcionNoEncontrado(nameof(Subasta), comando.SubastaId);

            var billeteras = await _repositorioBilleteras.FiltradasAsync(b => b.UsuarioId == comando.PostorId);
            var billetera = billeteras.FirstOrDefault();
            if (billetera == null)
                throw new ExcepcionValidacion("La billetera del postor no existe.");

            var pujas = await _repositorioPujas.FiltradasAsync(p => p.SubastaId == comando.SubastaId);
            var pujaLiderAnterior = pujas.OrderByDescending(p => p.Monto).FirstOrDefault();

            // Validaciones de dominio
            PujaValidacionReglas.ValidacionEstadoSubasta(subasta.Activa ? EstadoSubasta.Activa : EstadoSubasta.Finalizada);
            PujaValidacionReglas.ValidacionVentanaTemporal(subasta.FechaInicio, subasta.FechaFin, fechaHoraActual);
            PujaValidacionReglas.ValidacionPostorDiferenteDeVendedor(comando.PostorId, subasta.VendedorId);
            PujaValidacionReglas.ValidacionMontoOferta(comando.Monto, subasta.PrecioInicial, subasta.IncrementoMinimo, pujaLiderAnterior?.Monto);
            PujaValidacionReglas.ValidacionSaldoDisponible(billetera.SaldoDisponible + billetera.SaldoRetenido, billetera.SaldoRetenido, comando.Monto);

            // Liberar saldo al líder anterior
            if (pujaLiderAnterior != null)
            {
                var billeterasLider = await _repositorioBilleteras.FiltradasAsync(b => b.UsuarioId == pujaLiderAnterior.PostorId);
                var billeteraLiderAnterior = billeterasLider.FirstOrDefault();
                if (billeteraLiderAnterior != null)
                {
                    billeteraLiderAnterior.LiberacionSaldo(pujaLiderAnterior.Monto);
                    billeteraLiderAnterior.Movimientos.Add(new MovimientoContable
                    {
                        Monto = pujaLiderAnterior.Monto,
                        Tipo = TipoMovimiento.Liberacion,
                        FechaMovimiento = fechaHoraActual,
                        Concepto = $"Liberación de puja superada en subasta {comando.SubastaId}"
                    });
                    _repositorioBilleteras.Modificacion(billeteraLiderAnterior);
                }
            }

            // Retención al nuevo postor
            billetera.RetencionSaldo(comando.Monto);
            billetera.Movimientos.Add(new MovimientoContable
            {
                Monto = comando.Monto,
                Tipo = TipoMovimiento.Retencion,
                FechaMovimiento = fechaHoraActual,
                Concepto = $"Retención por puja en subasta {comando.SubastaId}"
            });
            _repositorioBilleteras.Modificacion(billetera);

            // Actualizar precio actual de la subasta
            subasta.ActualizacionPrecioActual(comando.Monto);

            // Regla Anti-Sniping — capturar fecha anterior para comparación
            var fechaFinAnterior = subasta.FechaFin;
            subasta.ExtensionTiempoAntiSniping(fechaHoraActual);
            var huboExtension = subasta.FechaFin != fechaFinAnterior;

            _repositorioSubastas.Modificacion(subasta);

            // Crear la puja
            var puja = new Puja
            {
                SubastaId = comando.SubastaId,
                PostorId = comando.PostorId,
                Monto = comando.Monto,
                FechaPuja = fechaHoraActual
            };

            await _repositorioPujas.AltaAsync(puja);

            // Confirmar transacción ACID
            await _unidadDeTrabajo.ConfirmacionAsync();
            await _unidadDeTrabajo.ConfirmacionTransaccionAsync();

            // === Post-transacción: auditoría y notificaciones ===

            // Obtener alias del postor para anonimización
            var postor = await _repositorioUsuarios.PorIdAsync(comando.PostorId);
            var aliasPostor = postor?.Alias ?? string.Empty;

            // Auditoría: puja aceptada
            await _auditoria.RegistroAsync(
                TipoAccionAuditoria.PujaAceptada,
                nameof(Puja),
                puja.Id,
                new
                {
                    subastaId = comando.SubastaId,
                    postorId = comando.PostorId,
                    monto = comando.Monto,
                    fechaHora = fechaHoraActual
                },
                aliasPostor);

            // Auditoría: extensión Anti-Sniping
            if (huboExtension)
            {
                await _auditoria.RegistroAsync(
                    TipoAccionAuditoria.ExtensionAntiSniping,
                    nameof(Subasta),
                    subasta.Id,
                    new
                    {
                        fechaFinAnterior,
                        fechaFinNueva = subasta.FechaFin,
                        extensionSegundos = (int)(subasta.FechaFin - fechaFinAnterior).TotalSeconds,
                        postorId = comando.PostorId
                    },
                    "Sistema");
            }

            // Notificación en tiempo real: nueva puja
            var dto = puja.MapeoDto(aliasPostor);
            await _notificador.EventoPujaRecibida(dto);

            // Notificación en tiempo real: extensión Anti-Sniping
            if (huboExtension)
            {
                await _notificador.EventoExtensionAntiSniping(new EventoExtensionAntiSnipingDto
                {
                    SubastaId = subasta.Id,
                    FechaFinAnteriorUtc = fechaFinAnterior,
                    FechaFinNuevaUtc = subasta.FechaFin,
                    ExtensionSegundos = (int)(subasta.FechaFin - fechaFinAnterior).TotalSeconds,
                    PostorAnonimizado = PujaMapeos.AliasAnonimizado(aliasPostor)
                });
            }

            return dto;
        }
        catch (ExcepcionConcurrencia exConcurrencia)
        {
            // Revertir PRIMERO antes de cualquier otra operacion con el DbContext
            await _unidadDeTrabajo.ReversionTransaccionAsync();
            // auditoria post-rollback (en contexto limpio, dentro de su propio try-catch)
            try
            {
                await _auditoria.RegistroAsync(
                    TipoAccionAuditoria.PujaRechazadaConcurrencia,
                    nameof(Puja),
                    0,
                    new
                    {
                        subastaId = comando.SubastaId,
                        postorId = comando.PostorId,
                        monto = comando.Monto,
                        mensajeError = exConcurrencia.Message
                    },
                    "Sistema");
            }
            catch { /* si falla la auditoría no debe tapar el 409 */ }
            throw;
        }
        catch
        {
            await _unidadDeTrabajo.ReversionTransaccionAsync();
            throw;
        }
    }
}
