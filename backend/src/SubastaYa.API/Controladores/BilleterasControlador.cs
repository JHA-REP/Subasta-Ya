using Microsoft.AspNetCore.Mvc;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;

namespace SubastaYa.API.Controladores;

/// <summary>
/// Controlador REST para billeteras y movimientos contables.
/// </summary>
[ApiController]
[Route("api/billeteras")]
public class BilleterasControlador : ControllerBase
{
    private readonly IServicioBilleteras _servicioBilleteras;

    public BilleterasControlador(IServicioBilleteras servicioBilleteras)
    {
        _servicioBilleteras = servicioBilleteras;
    }

    /// <summary>
    /// Detalle de la billetera de un usuario.
    /// </summary>
    [HttpGet("usuario/{usuarioId:int}")]
    public async Task<ActionResult<BilleteraDto>> DetallePorUsuario(int usuarioId)
    {
        var billetera = await _servicioBilleteras.DetallePorUsuarioIdAsync(usuarioId);
        return Ok(billetera);
    }

    /// <summary>
    /// Listado de movimientos contables de una billetera.
    /// </summary>
    [HttpGet("{billeteraId:int}/movimientos")]
    public async Task<ActionResult<IEnumerable<MovimientoContableDto>>> MovimientosPorBilletera(int billeteraId)
    {
        var movimientos = await _servicioBilleteras.MovimientosPorBilleteraIdAsync(billeteraId);
        return Ok(movimientos);
    }
}
