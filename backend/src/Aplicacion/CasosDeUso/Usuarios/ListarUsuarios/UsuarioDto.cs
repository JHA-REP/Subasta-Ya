using Dominio.Enumeraciones;

namespace Aplicacion.CasosDeUso.Usuarios.ListarUsuarios;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Alias { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public class NuevoUsuarioDto
{
    public string Alias { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
}
