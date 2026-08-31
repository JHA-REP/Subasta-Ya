using Microsoft.AspNetCore.Mvc;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;

namespace SubastaYa.API.Controladores;

/// <summary>
/// Controlador REST para subastas.
/// Rutas con sustantivos plurales, sin verbos.
/// </summary>
[ApiController]
[Route("api/subastas")]
public class SubastasControlador : ControllerBase
{
    private readonly IServicioSubastas _servicioSubastas;

    public SubastasControlador(IServicioSubastas servicioSubastas)
    {
        _servicioSubastas = servicioSubastas;
    }

    /// <summary>
    /// Listado de todas las subastas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubastaDto>>> Listado()
    {
        var subastas = await _servicioSubastas.ListadoAsync();
        return Ok(subastas);
    }

    /// <summary>
    /// Detalle de una subasta por su identificador.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubastaDto>> DetallePorId(int id)
    {
        var subasta = await _servicioSubastas.DetallePorIdAsync(id);
        return Ok(subasta);
    }

    /// <summary>
    /// Alta de una nueva subasta.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SubastaDto>> Nueva([FromBody] NuevaSubastaDto dto)
    {
        var subasta = await _servicioSubastas.NuevaAsync(dto);
        return CreatedAtAction(nameof(DetallePorId), new { id = subasta.Id }, subasta);
    }
}
