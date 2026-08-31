using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.Servicios;

/// <summary>
/// Servicio de aplicación para usuarios.
/// </summary>
public class ServicioUsuarios : IServicioUsuarios
{
    private readonly IRepositorio<Usuario> _repositorioUsuarios;

    public ServicioUsuarios(IRepositorio<Usuario> repositorioUsuarios)
    {
        _repositorioUsuarios = repositorioUsuarios;
    }

    public async Task<IEnumerable<UsuarioDto>> ListadoAsync()
    {
        var usuarios = await _repositorioUsuarios.TodosAsync();

        return usuarios.Select(u => new UsuarioDto
        {
            Id = u.Id,
            Alias = u.Alias,
            Email = u.Email,
            Rol = u.Rol,
            FechaRegistro = u.FechaRegistro
        });
    }

    public async Task<UsuarioDto> DetallePorIdAsync(int id)
    {
        var usuario = await _repositorioUsuarios.PorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado(nameof(Usuario), id);

        return new UsuarioDto
        {
            Id = usuario.Id,
            Alias = usuario.Alias,
            Email = usuario.Email,
            Rol = usuario.Rol,
            FechaRegistro = usuario.FechaRegistro
        };
    }
}
