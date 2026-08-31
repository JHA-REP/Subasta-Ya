using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Dominio.Entidades;

/// <summary>
/// Representa un usuario del sistema (comprador, vendedor o administrador).
/// </summary>
public class Usuario : EntidadBase
{
    public string Alias { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ClaveHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public DateTime FechaRegistro { get; set; }

    /// <summary>
    /// Token de concurrencia optimista.
    /// </summary>
    public byte[] Version { get; set; } = [];

    // Navegación
    public Billetera? Billetera { get; set; }
    public ICollection<Subasta> SubastasComoVendedor { get; set; } = [];
    public ICollection<Puja> Pujas { get; set; } = [];
}
