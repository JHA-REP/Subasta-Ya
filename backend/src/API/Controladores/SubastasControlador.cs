using Microsoft.AspNetCore.Mvc;
using Aplicacion.CasosDeUso.Subastas.CrearSubasta;
using Aplicacion.CasosDeUso.Subastas.ListarSubastas;
using Aplicacion.CasosDeUso.Subastas.ObtenerSubastaPorId;
using Dominio.Entidades;
using Dominio.Enumeraciones;


namespace Api.Controladores;

// endpoint de subastas con manejadores cqrs
[ApiController]
[Route("api/subastas")]
public class SubastasControlador : ControllerBase
{
    // listado de subastas
    [HttpGet]
     public async Task<IActionResult> Listado(
     [FromServices] ListadoSubastasManejador manejador,
     [FromQuery] EstadoSubasta? estado = null,
     [FromQuery] int? categoriaId = null,
     [FromQuery] decimal? precioMin = null,
     [FromQuery] decimal? precioMax = null,
     [FromQuery] int pagina = 1,
     [FromQuery] int tamanoPagina = 10,
     [FromQuery] string? terminoBusqueda = null,
     [FromQuery] string? criterioOrden = null,
     [FromQuery] int? vendedorId = null)
    {
        var consulta = new ListadoSubastasConsulta
        {
            Estado = estado,
            CategoriaId = categoriaId,
            PrecioMin = precioMin,  
            PrecioMax = precioMax,
            Pagina = pagina,
            TamanoPagina = tamanoPagina,
            TerminoBusqueda = terminoBusqueda,
            CriterioOrden = criterioOrden,
            VendedorId = vendedorId
        };
        var resultado = await manejador.EjecucionAsync(consulta);
        return Ok(resultado);
    }

    // detalle por id
    [HttpGet("{id:int}")]
    public async Task<IActionResult> DetallePorId(int id, [FromServices] SubastaPorIdManejador manejador)
    {
        var resultado = await manejador.EjecucionAsync(new SubastaPorIdConsulta(id));
        if (resultado == null) return NotFound();
        return Ok(resultado);
    }

    // creacion de subasta
    [HttpPost]
    public async Task<IActionResult> Creacion([FromBody] NuevaSubastaComando comando, [FromServices] NuevaSubastaManejador manejador)
    {
        var resultado = await manejador.EjecucionAsync(comando);
        return CreatedAtAction(nameof(DetallePorId), new { id = resultado.Id }, resultado);
    }
}
