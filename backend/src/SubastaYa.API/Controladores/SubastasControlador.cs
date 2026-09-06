using Microsoft.AspNetCore.Mvc;
using SubastaYa.Aplicacion.CasosDeUso.Subastas.Comandos;
using SubastaYa.Aplicacion.CasosDeUso.Subastas.Consultas;
using SubastaYa.Aplicacion.CasosDeUso.Subastas.Manejadores;

namespace SubastaYa.Api.Controladores;

// endpoint de subastas con manejadores cqrs
[ApiController]
[Route("api/subastas")]
public class SubastasControlador : ControllerBase
{
    // listado de subastas
    [HttpGet]
    public async Task<IActionResult> Listado([FromServices] ListadoSubastasManejador manejador)
    {
        var resultado = await manejador.EjecucionAsync(new ListadoSubastasConsulta());
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
