using Microsoft.AspNetCore.Mvc;
using SubastaYa.Aplicacion.CasosDeUso.Pujas.ListarPujasPorSubasta;
using SubastaYa.Aplicacion.CasosDeUso.Pujas.RegistrarPuja;

namespace SubastaYa.Api.Controladores;

//  endpoint de pujas con manejadores cqrs
[ApiController]
[Route("api/subastas/{subastaId:int}/pujas")]
public class PujasControlador : ControllerBase
{
    // listado de pujas por subasta
    [HttpGet]
    public async Task<IActionResult> ListadoPorSubasta(int subastaId, [FromServices] ListadoPujasPorSubastaManejador manejador)
    {
        var resultado = await manejador.EjecucionAsync(new ListadoPujasPorSubastaConsulta(subastaId));
        return Ok(resultado);
    }

    // creacion de puja con reglas de negocio y transaccion
    [HttpPost]
    public async Task<IActionResult> Creacion(int subastaId, [FromBody] PujaRegistroComando comando, [FromServices] PujaRegistroManejador manejador)
    {
        comando.SubastaId = subastaId;
        var resultado = await manejador.EjecucionAsync(comando);
        return CreatedAtAction(nameof(ListadoPorSubasta), new { subastaId = resultado.SubastaId }, resultado);
    }
}
