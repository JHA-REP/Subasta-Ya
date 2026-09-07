using SubastaYa.Aplicacion.CasosDeUso.Usuarios.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Usuarios.Manejadores;

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
