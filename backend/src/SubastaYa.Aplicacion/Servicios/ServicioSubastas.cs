using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;
using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.Servicios;

/// <summary>
/// Servicio de aplicación para subastas.
/// </summary>
public class ServicioSubastas : IServicioSubastas
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public ServicioSubastas(
        IRepositorio<Subasta> repositorioSubastas,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repositorioSubastas = repositorioSubastas;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<IEnumerable<SubastaDto>> ListadoAsync()
    {
        var subastas = await _repositorioSubastas.TodosAsync();

        return subastas.Select(s => new SubastaDto
        {
            Id = s.Id,
            Titulo = s.Titulo,
            Descripcion = s.Descripcion,
            PrecioBase = s.PrecioBase,
            Categoria = s.Categoria?.Nombre ?? string.Empty,
            CategoriaId = s.CategoriaId,
            Vendedor = s.Vendedor?.Alias ?? string.Empty,
            VendedorId = s.VendedorId,
            Estado = s.Estado,
            FechaInicio = s.FechaInicio,
            FechaFin = s.FechaFin,
            CantidadPujas = s.Pujas?.Count ?? 0,
            MontoMayorPuja = s.Pujas?.Any() == true
                ? s.Pujas.Max(p => p.Monto)
                : null
        });
    }

    public async Task<SubastaDto> DetallePorIdAsync(int id)
    {
        var subasta = await _repositorioSubastas.PorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado(nameof(Subasta), id);

        return new SubastaDto
        {
            Id = subasta.Id,
            Titulo = subasta.Titulo,
            Descripcion = subasta.Descripcion,
            PrecioBase = subasta.PrecioBase,
            Categoria = subasta.Categoria?.Nombre ?? string.Empty,
            CategoriaId = subasta.CategoriaId,
            Vendedor = subasta.Vendedor?.Alias ?? string.Empty,
            VendedorId = subasta.VendedorId,
            Estado = subasta.Estado,
            FechaInicio = subasta.FechaInicio,
            FechaFin = subasta.FechaFin,
            CantidadPujas = subasta.Pujas?.Count ?? 0,
            MontoMayorPuja = subasta.Pujas?.Any() == true
                ? subasta.Pujas.Max(p => p.Monto)
                : null
        };
    }

    public async Task<SubastaDto> NuevaAsync(NuevaSubastaDto dto)
    {
        var subasta = new Subasta
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            PrecioBase = dto.PrecioBase,
            CategoriaId = dto.CategoriaId,
            VendedorId = dto.VendedorId,
            Estado = EstadoSubasta.Pendiente,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin
        };

        await _repositorioSubastas.AltaAsync(subasta);
        await _unidadDeTrabajo.ConfirmacionAsync();

        return new SubastaDto
        {
            Id = subasta.Id,
            Titulo = subasta.Titulo,
            Descripcion = subasta.Descripcion,
            PrecioBase = subasta.PrecioBase,
            CategoriaId = subasta.CategoriaId,
            VendedorId = subasta.VendedorId,
            Estado = subasta.Estado,
            FechaInicio = subasta.FechaInicio,
            FechaFin = subasta.FechaFin,
            CantidadPujas = 0
        };
    }
}
