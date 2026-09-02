using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;
using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Interfaces;
using SubastaYa.Dominio.Reglas;

namespace SubastaYa.Aplicacion.Servicios;

/// <summary>
/// Servicio de aplicación para pujas.
/// La lógica completa de validación y escrow se implementará en etapas posteriores.
/// </summary>
public class ServicioPujas : IServicioPujas
{
    private readonly IRepositorio<Puja> _repositorioPujas;
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IRepositorio<Billetera> _repositorioBilleteras;
    private readonly IRepositorio<MovimientoContable> _repositorioMovimientos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public ServicioPujas(
        IRepositorio<Puja> repositorioPujas,
        IRepositorio<Subasta> repositorioSubastas,
        IRepositorio<Billetera> repositorioBilleteras,
        IRepositorio<MovimientoContable> repositorioMovimientos,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repositorioPujas = repositorioPujas;
        _repositorioSubastas = repositorioSubastas;
        _repositorioBilleteras = repositorioBilleteras;
        _repositorioMovimientos = repositorioMovimientos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<IEnumerable<PujaDto>> ListadoPorSubastaAsync(int subastaId)
    {
        var pujas = await _repositorioPujas.FiltradasAsync(p => p.SubastaId == subastaId);

        return pujas.Select(p => new PujaDto
        {
            Id = p.Id,
            SubastaId = p.SubastaId,
            PostorId = p.PostorId,
            PostorAlias = p.Postor?.Alias ?? string.Empty,
            Monto = p.Monto,
            FechaPuja = p.FechaPuja
        });
    }

    public async Task<PujaDto> NuevaAsync(NuevaPujaDto dto)
    {
        var fechaHoraActual = DateTime.UtcNow;
       
        var subasta = await _repositorioSubastas.PorIdAsync(dto.SubastaId)
            ?? throw new ExcepcionNoEncontrado(nameof(Subasta), dto.SubastaId);

        // evaluacion de reglas de negocio de subasta

        PujaValidacionReglas.ValidacionEstadoSubasta(subasta.Estado);
        PujaValidacionReglas.ValidacionVentanaTemporal(subasta.FechaInicio, subasta.FechaFin, fechaHoraActual);
        PujaValidacionReglas.ValidacionPostorDiferenteDeVendedor(dto.PostorId, subasta.VendedorId);

        //evaluacion de ofertas anteriores y monto mínimo

        var pujasExistentes = await _repositorioPujas.FiltradasAsync(p => p.SubastaId == dto.SubastaId);
        var pujaLiderAnterior = pujasExistentes.OrderByDescending(p => p.Monto).FirstOrDefault();
        PujaValidacionReglas.ValidacionMontoOferta(dto.Monto, subasta.PrecioBase, subasta.IncrementoMinimo, pujaLiderAnterior?.Monto);

        //evaluación de saldo disponible en la billetera del postor

        var billeterasNuevoPostor = await _repositorioBilleteras.FiltradasAsync(b => b.UsuarioId == dto.PostorId);
        var billeteraNuevoPostor = billeterasNuevoPostor.FirstOrDefault()
                   ?? throw new ExcepcionNoEncontrado(nameof(Billetera), dto.PostorId);
        PujaValidacionReglas.ValidacionSaldoDisponible(billeteraNuevoPostor.Saldo, billeteraNuevoPostor.SaldoRetenido, dto.Monto);

        
    
       // inicio de transaccion 
        await _unidadDeTrabajo.InicioTransaccionAsync();

        try
        {
            // liberacion de retencion del líder anterior (si existia)
            if (pujaLiderAnterior is not null)
            {
                var billeterasLiderAnterior = await _repositorioBilleteras.FiltradasAsync(b => b.UsuarioId == pujaLiderAnterior.PostorId);
                var billeteraLiderAnterior = billeterasLiderAnterior.FirstOrDefault();

                if (billeteraLiderAnterior is not null)
                {
                    billeteraLiderAnterior.SaldoRetenido -= pujaLiderAnterior.Monto;
                    _repositorioBilleteras.Modificacion(billeteraLiderAnterior);
                     
                    await _repositorioMovimientos.AltaAsync(new MovimientoContable
                    {
                        BilleteraId = billeteraLiderAnterior.Id,
                        Tipo = TipoMovimiento.Liberacion,
                        Monto = pujaLiderAnterior.Monto,
                        Concepto = $"Liberación de retención por puja superada en subasta #{subasta.Id}",
                        FechaMovimiento = fechaHoraActual
                    });
                }
            }

        /// retencion de fondos en billetera y registro en ledger

        billeteraNuevoPostor.SaldoRetenido += dto.Monto;
        _repositorioBilleteras.Modificacion(billeteraNuevoPostor);

        await _repositorioMovimientos.AltaAsync(new MovimientoContable
        {
            BilleteraId = billeteraNuevoPostor.Id,
            Tipo = TipoMovimiento.Retencion,
            Monto = -dto.Monto,
            Concepto = $"Retención por oferta líder en subasta #{subasta.Id}",
            FechaMovimiento = fechaHoraActual
        });

        var puja = new Puja
        {
            SubastaId = dto.SubastaId,
            PostorId = dto.PostorId,
            Monto = dto.Monto,
            FechaPuja = fechaHoraActual
        };

        await _repositorioPujas.AltaAsync(puja);

        //anti-sniping 

        var tiempoRestante = subasta.FechaFin - fechaHoraActual;
        if (tiempoRestante <= TimeSpan.FromSeconds(60))
        {
            subasta.FechaFin = subasta.FechaFin.AddMinutes(2);
        }

        // activacion concurrencia optimista sobre la subasta
        _repositorioSubastas.Modificacion(subasta);

        await _unidadDeTrabajo.ConfirmacionAsync();
        await _unidadDeTrabajo.ConfirmacionTransaccionAsync();

        return new PujaDto
        {
            Id = puja.Id,
            SubastaId = puja.SubastaId,
            PostorId = puja.PostorId,
            Monto = puja.Monto,
            FechaPuja = puja.FechaPuja
        };
            }
        catch
        {
            await _unidadDeTrabajo.ReversionTransaccionAsync();
            throw;
        }
    }
}
