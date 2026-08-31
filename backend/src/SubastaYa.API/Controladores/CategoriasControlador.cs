using Microsoft.AspNetCore.Mvc;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;

namespace SubastaYa.API.Controladores;

/// <summary>
/// Controlador REST para categorías.
/// </summary>
[ApiController]
[Route("api/categorias")]
public class CategoriasControlador : ControllerBase
{
    private readonly IServicioCategorias _servicioCategorias;

    public CategoriasControlador(IServicioCategorias servicioCategorias)
    {
        _servicioCategorias = servicioCategorias;
    }

    /// <summary>
    /// Listado de todas las categorías.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> Listado()
    {
        var categorias = await _servicioCategorias.ListadoAsync();
        return Ok(categorias);
    }

    /// <summary>
    /// Detalle de una categoría por su identificador.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoriaDto>> DetallePorId(int id)
    {
        var categoria = await _servicioCategorias.DetallePorIdAsync(id);
        return Ok(categoria);
    }
}
