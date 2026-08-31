using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Dominio.Entidades;

/// <summary>
/// Subasta publicada por un vendedor.
/// Protegida con Optimistic Locking (RowVersion) para controlar concurrencia en pujas.
/// </summary>
public class Subasta : EntidadBase
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal PrecioBase { get; set; }
    public int CategoriaId { get; set; }
    public int VendedorId { get; set; }
    public EstadoSubasta Estado { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    /// <summary>
    /// Token de concurrencia optimista — crítico para pujas concurrentes.
    /// </summary>
    public byte[] Version { get; set; } = [];

    // Navegación
    public Categoria? Categoria { get; set; }
    public Usuario? Vendedor { get; set; }
    public ICollection<Puja> Pujas { get; set; } = [];
}
