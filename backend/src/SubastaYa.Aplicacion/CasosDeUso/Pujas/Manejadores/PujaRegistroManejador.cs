using SubastaYa.Aplicacion.CasosDeUso.Pujas.Comandos;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entities;
using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Reglas;

namespace SubastaYa.Aplicacion.CasosDeUso.Pujas.Manejadores;

//aca tendriamos lo que teniamos antes en serviciopujas

public class PujaRegistroManejador
{
    private readonly IRepositorioSubastas _repositorioSubastas;
    private readonly IRepositorioPujas _repositorioPujas;
    private readonly IRepositorioBilleteras _repositorioBilleteras;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public PujaRegistroManejador(
        IRepositorioSubastas repositorioSubastas,
        IRepositorioPujas repositorioPujas,
        IRepositorioBilleteras repositorioBilleteras,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repositorioSubastas = repositorioSubastas;
        _repositorioPujas = repositorioPujas;
        _repositorioBilleteras = repositorioBilleteras;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<PujaDto> EjecucionAsync(PujaRegistroComando comando)
    {
        var fechaHoraActual = DateTime.UtcNow;

        using var transaccion = await _unidadDeTrabajo.InicioTransaccionAsync();

        var subasta = await _repositorioSubastas.ObtenerPorIdAsync(comando.SubastaId);
        if (subasta == null)
            throw new ExcepcionValidacion("La subasta especificada no existe.");

        var billetera = await _repositorioBilleteras.ObtenerPorUsuarioIdAsync(comando.PostorId);
        if (billetera == null)
            throw new ExcepcionValidacion("La billetera del postor no existe.");

        // validaciones
        PujaValidacionReglas.ValidacionEstadoSubasta(subasta);
        PujaValidacionReglas.ValidacionVentanaTemporal(subasta, fechaHoraActual);
        PujaValidacionReglas.ValidacionPostorDiferenteDeVendedor(subasta, comando.PostorId);
        PujaValidacionReglas.ValidacionMontoOferta(subasta, comando.Monto);
        PujaValidacionReglas.ValidacionSaldoDisponible(billetera, comando.Monto);

        // liberar lider anterior
        var pujas = await _repositorioPujas.ObtenerPorSubastaIdAsync(comando.SubastaId);
        var pujaLiderAnterior = pujas.OrderByDescending(p => p.Monto).FirstOrDefault();

        if (pujaLiderAnterior != null)
        {
            var billeteraLiderAnterior = await _repositorioBilleteras.ObtenerPorUsuarioIdAsync(pujaLiderAnterior.PostorId);
            if (billeteraLiderAnterior != null)
            {
                billeteraLiderAnterior.LiberacionSaldo(pujaLiderAnterior.Monto);
                billeteraLiderAnterior.Movimientos.Add(new MovimientoContable
                {
                    Monto = pujaLiderAnterior.Monto,
                    Tipo = TipoMovimiento.Liberacion,
                    FechaHora = fechaHoraActual,
                    ReferenciaSubastaId = comando.SubastaId
                });
                _repositorioBilleteras.Modificacion(billeteraLiderAnterior);
            }
        }

        // retener saldo nuevo postor
        billetera.RetencionSaldo(comando.Monto);
        billetera.Movimientos.Add(new MovimientoContable
        {
            Monto = comando.Monto,
            Tipo = TipoMovimiento.Retencion,
            FechaHora = fechaHoraActual,
            ReferenciaSubastaId = comando.SubastaId
        });
        _repositorioBilleteras.Modificacion(billetera);

        // actualizar subasta
        subasta.ActualizacionPrecioActual(comando.Monto);

        // regla Anti-Sniping
        subasta.ExtensionTiempoAntiSniping(fechaHoraActual);
        _repositorioSubastas.Modificacion(subasta);

        // crear la puja
        var puja = new Puja
        {
            SubastaId = comando.SubastaId,
            PostorId = comando.PostorId,
            Monto = comando.Monto,
            FechaHora = fechaHoraActual
        };

        await _repositorioPujas.AltaAsync(puja);

        await _unidadDeTrabajo.GuardadoCambiosAsync();
        await transaccion.CommitAsync();

        return puja.MapeoDto();
    }
}
