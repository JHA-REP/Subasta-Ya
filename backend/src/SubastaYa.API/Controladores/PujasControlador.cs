using Microsoft.AspNetCore.Mvc;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;

namespace SubastaYa.API.Controladores;

/// <summary>
/// Controlador REST para pujas dentro de una subasta.
/// Ruta anidada: /api/subastas/{subastaId}/pujas
/// </summary>
[ApiController]
[Route("api/subastas/{subastaId:int}/pujas")]
public class PujasControlador : ControllerBase
{
    private readonly IServicioPujas _servicioPujas;

    public PujasControlador(IServicioPujas servicioPujas)
    {
        _servicioPujas = servicioPujas;
    }

    /// <summary>
    /// Listado de pujas de una subasta.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PujaDto>>> ListadoPorSubasta(int subastaId)
    {
        var pujas = await _servicioPujas.ListadoPorSubastaAsync(subastaId);
        return Ok(pujas);
    }

    /// <summary>
    /// Alta de una nueva puja en una subasta.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PujaDto>> Nueva(int subastaId, [FromBody] NuevaPujaDto dto)
    {
        dto.SubastaId = subastaId;
        var puja = await _servicioPujas.NuevaAsync(dto);
        return CreatedAtAction(nameof(ListadoPorSubasta), new { subastaId }, puja);
    }
}
