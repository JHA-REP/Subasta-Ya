using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Dominio.Entidades;

namespace SubastaYa.Aplicacion.Mapeos;

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
