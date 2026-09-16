using Microsoft.AspNetCore.Mvc;
using SubastaYa.Aplicacion.CasosDeUso.Usuarios.ListarUsuarios;
using SubastaYa.Aplicacion.CasosDeUso.Usuarios.ObtenerUsuarioPorId;

namespace SubastaYa.API.Controladores;

// endpoint de usuarios con manejadores cqrs
[ApiController]
[Route("api/usuarios")]
public class UsuariosControlador : ControllerBase
{
    // listado de usuarios
    [HttpGet]
    public async Task<IActionResult> Listado([FromServices] ListadoUsuariosManejador manejador)
    {
        var resultado = await manejador.EjecucionAsync(new ListadoUsuariosConsulta());
        return Ok(resultado);
    }

    // detalle por id
    [HttpGet("{id:int}")]
    public async Task<IActionResult> DetallePorId(int id, [FromServices] UsuarioPorIdManejador manejador)
    {
        var resultado = await manejador.EjecucionAsync(new UsuarioPorIdConsulta(id));
        return Ok(resultado);
    }
}
