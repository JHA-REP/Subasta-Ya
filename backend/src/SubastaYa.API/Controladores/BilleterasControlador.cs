using Microsoft.AspNetCore.Mvc;
using SubastaYa.Aplicacion.CasosDeUso.Billeteras.AcreditarSaldo;

using SubastaYa.Aplicacion.CasosDeUso.Billeteras.ObtenerBilletera;

namespace SubastaYa.Api.Controladores;

//  endpoint de billeteras con manejadores cqrs
[ApiController]
[Route("api/usuarios/{usuarioId:int}/billetera")]
public class BilleterasControlador : ControllerBase
{
    // detalle de billetera por usuario
    [HttpGet]
    public async Task<IActionResult> DetallePorUsuario(int usuarioId, [FromServices] BilleteraPorUsuarioManejador manejador)
    {
        var resultado = await manejador.EjecucionAsync(new BilleteraPorUsuarioConsulta(usuarioId));
        if (resultado == null) return NotFound();
        return Ok(resultado);
    }

    // acreditacion simulada de saldo
    [HttpPost("acreditaciones")]
    public async Task<IActionResult> AcreditacionSimulada(int usuarioId, [FromBody] AcreditacionSaldoComando comando, [FromServices] AcreditacionSaldoManejador manejador)
    {
        comando.UsuarioId = usuarioId;
        var resultado = await manejador.EjecucionAsync(comando);
        return CreatedAtAction(nameof(DetallePorUsuario), new { usuarioId }, resultado);
    }

    // listado de movimientos contables de la billetera
    [HttpGet("movimientos")]
    public async Task<IActionResult> ListarMovimientos(int usuarioId, [FromServices] BilleteraPorUsuarioManejador manejador)
    {
        var billetera = await manejador.EjecucionAsync(new BilleteraPorUsuarioConsulta(usuarioId));
        if (billetera == null) return NotFound();
        return Ok(billetera.Movimientos);
    }
}
