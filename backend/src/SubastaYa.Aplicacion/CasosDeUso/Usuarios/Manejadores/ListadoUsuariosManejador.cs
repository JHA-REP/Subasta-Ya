using SubastaYa.Aplicacion.CasosDeUso.Usuarios.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Usuarios.Manejadores;

public class ListadoUsuariosManejador
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
