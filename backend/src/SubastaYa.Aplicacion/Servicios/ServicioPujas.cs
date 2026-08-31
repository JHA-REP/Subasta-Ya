using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.Servicios;

/// <summary>
/// Servicio de aplicación para pujas.
/// La lógica completa de validación y escrow se implementará en etapas posteriores.
/// </summary>
public class ServicioPujas : IServicioPujas
{
    private readonly IRepositorio<Puja> _repositorioPujas;
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public ServicioPujas(
        IRepositorio<Puja> repositorioPujas,
        IRepositorio<Subasta> repositorioSubastas,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repositorioPujas = repositorioPujas;
        _repositorioSubastas = repositorioSubastas;
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
        // Validación básica — la lógica completa (escrow, anti-sniping) se implementará en etapas posteriores.
        var subasta = await _repositorioSubastas.PorIdAsync(dto.SubastaId)
            ?? throw new ExcepcionNoEncontrado(nameof(Subasta), dto.SubastaId);

        var puja = new Puja
        {
            SubastaId = dto.SubastaId,
            PostorId = dto.PostorId,
            Monto = dto.Monto,
            FechaPuja = DateTime.UtcNow
        };

        await _repositorioPujas.AltaAsync(puja);
        await _unidadDeTrabajo.ConfirmacionAsync();

        return new PujaDto
        {
            Id = puja.Id,
            SubastaId = puja.SubastaId,
            PostorId = puja.PostorId,
            Monto = puja.Monto,
            FechaPuja = puja.FechaPuja
        };
    }
}
