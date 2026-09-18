using SubastaYa.Aplicacion.CasosDeUso.Usuarios.ListarUsuarios;
using SubastaYa.Dominio.Entidades;

namespace SubastaYa.Aplicacion.Comun.Mapeos;

public static class UsuarioMapeos
{
    public static UsuarioDto MapeoDto(this Usuario entidad)
    {
        return new UsuarioDto
        {
            Id = entidad.Id,
            Alias = entidad.Alias,
            Email = entidad.Email,
            Rol = entidad.Rol,
            FechaRegistro = entidad.FechaRegistro
        };
    }
}
