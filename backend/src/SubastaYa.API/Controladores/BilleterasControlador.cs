using Microsoft.AspNetCore.Mvc;
using SubastaYa.Aplicacion.CasosDeUso.Billeteras.AcreditarSaldo;

using SubastaYa.Aplicacion.CasosDeUso.Billeteras.ObtenerBilletera;

namespace SubastaYa.Api.Controladores;

//  endpoint de billeteras con manejadores cqrs
[ApiController]
[Route("api/billeteras")]
public class BilleterasControlador : ControllerBase
{
    // detalle de billetera por usuario
    [HttpGet("usuario/{usuarioId:int}")]
    public async Task<IActionResult> DetallePorUsuario(int usuarioId, [FromServices] BilleteraPorUsuarioManejador manejador)
    {
        var resultado = await manejador.EjecucionAsync(new BilleteraPorUsuarioConsulta(usuarioId));
        if (resultado == null) return NotFound();
        return Ok(resultado);
    }

    // acreditacion simulada de saldo
    [HttpPost("usuario/{usuarioId:int}/acreditaciones")]
    public async Task<IActionResult> AcreditacionSimulada(int usuarioId, [FromBody] AcreditacionSaldoComando comando, [FromServices] AcreditacionSaldoManejador manejador)
    {
        comando.UsuarioId = usuarioId;
        var resultado = await manejador.EjecucionAsync(comando);
        return Ok(resultado);
    }
}
