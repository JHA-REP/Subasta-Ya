using SubastaYa.Aplicacion.CasosDeUso.Usuarios.ListarUsuarios;
using SubastaYa.Aplicacion.Comun.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Usuarios.ObtenerUsuarioPorId;

public class UsuarioPorIdManejador
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
