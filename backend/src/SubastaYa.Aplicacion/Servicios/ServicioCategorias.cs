using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.Servicios;

/// <summary>
/// Servicio de aplicación para categorías.
/// </summary>
public class ServicioCategorias : IServicioCategorias
{
    private readonly IRepositorio<Categoria> _repositorioCategorias;

    public ServicioCategorias(IRepositorio<Categoria> repositorioCategorias)
    {
        _repositorioCategorias = repositorioCategorias;
    }

    public async Task<IEnumerable<CategoriaDto>> ListadoAsync()
    {
        var categorias = await _repositorioCategorias.TodosAsync();

        return categorias.Select(c => new CategoriaDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Descripcion = c.Descripcion,
            CantidadSubastas = c.Subastas?.Count ?? 0
        });
    }

    public async Task<CategoriaDto> DetallePorIdAsync(int id)
    {
        var categoria = await _repositorioCategorias.PorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado(nameof(Categoria), id);

        return new CategoriaDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            CantidadSubastas = categoria.Subastas?.Count ?? 0
        };
    }
}
