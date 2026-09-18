using Aplicacion.CasosDeUso.Usuarios.ListarUsuarios;
using Aplicacion.Comun.Mapeos;
using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Excepciones;


namespace Aplicacion.CasosDeUso.Usuarios.ObtenerUsuarioPorId;

public class UsuarioPorIdManejador : IConsultaManejador<UsuarioPorIdConsulta, UsuarioDto>
{
    private readonly IRepositorio<Usuario> _repositorioUsuarios;

    public UsuarioPorIdManejador(IRepositorio<Usuario> repositorioUsuarios)
    {
        _repositorioUsuarios = repositorioUsuarios;
    }

    public async Task<UsuarioDto> EjecucionAsync(UsuarioPorIdConsulta consulta)
    {
        var usuario = await _repositorioUsuarios.PorIdAsync(consulta.Id)
            ?? throw new ExcepcionNoEncontrado(nameof(Usuario), consulta.Id);

        return usuario.MapeoDto();
    }
}
