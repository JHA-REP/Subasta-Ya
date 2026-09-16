using SubastaYa.Aplicacion.Comun.Mapeos;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Usuarios.ListarUsuarios;

public class ListadoUsuariosManejador : IConsultaManejador<ListadoUsuariosConsulta, IEnumerable<UsuarioDto>>
{
    private readonly IRepositorio<Usuario> _repositorioUsuarios;

    public ListadoUsuariosManejador(IRepositorio<Usuario> repositorioUsuarios)
    {
        _repositorioUsuarios = repositorioUsuarios;
    }

    public async Task<IEnumerable<UsuarioDto>> EjecucionAsync(ListadoUsuariosConsulta consulta)
    {
        var usuarios = await _repositorioUsuarios.TodosAsync();
        return usuarios.Select(u => u.MapeoDto());
    }
}
