using Microsoft.AspNetCore.Mvc;

using SubastaYa.Aplicacion.CasosDeUso.Categorias.ListarCategorias;
using SubastaYa.Aplicacion.CasosDeUso.Categorias.ObtenerCategoriaPorId;

namespace SubastaYa.API.Controladores;

// endpoint de categorias con manejadores cqrs
[ApiController]
[Route("api/categorias")]
public class CategoriasControlador : ControllerBase
{
    // listado de categorias
    [HttpGet]
    public async Task<IActionResult> Listado([FromServices] ListadoCategoriasManejador manejador)
    {
        var resultado = await manejador.EjecucionAsync(new ListadoCategoriasConsulta());
        return Ok(resultado);
    }

    // detalle por id
    [HttpGet("{id:int}")]
    public async Task<IActionResult> DetallePorId(int id, [FromServices] CategoriaPorIdManejador manejador)
    {
        var resultado = await manejador.EjecucionAsync(new CategoriaPorIdConsulta(id));
        if (resultado == null)
        {
            return NotFound();
        }
        return Ok(resultado);
    }
}
