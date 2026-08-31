using Microsoft.AspNetCore.Mvc;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;

namespace SubastaYa.API.Controladores;

/// <summary>
/// Controlador REST para usuarios.
/// </summary>
[ApiController]
[Route("api/usuarios")]
public class UsuariosControlador : ControllerBase
{
    private readonly IServicioUsuarios _servicioUsuarios;

    public UsuariosControlador(IServicioUsuarios servicioUsuarios)
    {
        _servicioUsuarios = servicioUsuarios;
    }

    /// <summary>
    /// Listado de todos los usuarios.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> Listado()
    {
        var usuarios = await _servicioUsuarios.ListadoAsync();
        return Ok(usuarios);
    }

    /// <summary>
    /// Detalle de un usuario por su identificador.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> DetallePorId(int id)
    {
        var usuario = await _servicioUsuarios.DetallePorIdAsync(id);
        return Ok(usuario);
    }
}
