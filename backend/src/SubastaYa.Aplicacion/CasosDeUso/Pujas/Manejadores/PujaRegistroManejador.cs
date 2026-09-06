using SubastaYa.Aplicacion.CasosDeUso.Pujas.Comandos;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;
using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Interfaces;
using SubastaYa.Dominio.Reglas;

namespace SubastaYa.Aplicacion.CasosDeUso.Pujas.Manejadores;

public class PujaRegistroManejador
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IRepositorio<Puja> _repositorioPujas;
    private readonly IRepositorio<Billetera> _repositorioBilleteras;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public PujaRegistroManejador(
        IRepositorio<Subasta> repositorioSubastas,
        IRepositorio<Puja> repositorioPujas,
        IRepositorio<Billetera> repositorioBilleteras,
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

        await _unidadDeTrabajo.InicioTransaccionAsync();

        var subasta = await _repositorioSubastas.PorIdAsync(comando.SubastaId);
        if (subasta == null)
            throw new ExcepcionValidacion("La subasta especificada no existe.");

        var billeteras = await _repositorioBilleteras.FiltradasAsync(b => b.UsuarioId == comando.PostorId);
        var billetera = billeteras.FirstOrDefault();
        if (billetera == null)
            throw new ExcepcionValidacion("La billetera del postor no existe.");

        var pujas = await _repositorioPujas.FiltradasAsync(p => p.SubastaId == comando.SubastaId);
        var pujaLiderAnterior = pujas.OrderByDescending(p => p.Monto).FirstOrDefault();

        // Validaciones
        PujaValidacionReglas.ValidacionEstadoSubasta(subasta.Activa ? EstadoSubasta.Activa : EstadoSubasta.Finalizada);
        PujaValidacionReglas.ValidacionVentanaTemporal(subasta.FechaInicio, subasta.FechaFin, fechaHoraActual);
        PujaValidacionReglas.ValidacionPostorDiferenteDeVendedor(comando.PostorId, subasta.VendedorId);
        PujaValidacionReglas.ValidacionMontoOferta(comando.Monto, subasta.PrecioInicial, subasta.IncrementoMinimo, pujaLiderAnterior?.Monto);
        PujaValidacionReglas.ValidacionSaldoDisponible(billetera.SaldoDisponible + billetera.SaldoRetenido, billetera.SaldoRetenido, comando.Monto);

        // Liberar líder anterior
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

        // Retener saldo nuevo postor
        billetera.RetencionSaldo(comando.Monto);
        billetera.Movimientos.Add(new MovimientoContable
        {
            Monto = comando.Monto,
            Tipo = TipoMovimiento.Retencion,
            FechaMovimiento = fechaHoraActual,
            Concepto = $"Retención por puja en subasta {comando.SubastaId}"
        });
        _repositorioBilleteras.Modificacion(billetera);

        // Actualizar subasta
        subasta.ActualizacionPrecioActual(comando.Monto);

        // Regla Anti-Sniping
        subasta.ExtensionTiempoAntiSniping(fechaHoraActual);
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

        await _unidadDeTrabajo.ConfirmacionAsync();
        await _unidadDeTrabajo.ConfirmacionTransaccionAsync();

        return puja.MapeoDto();
    }
}
