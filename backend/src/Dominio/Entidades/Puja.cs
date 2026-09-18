namespace Dominio.Entidades;

/// <summary>
/// Oferta realizada por un postor en una subasta.
/// </summary>
public class Puja : EntidadBase
{
    public int SubastaId { get; set; }
    public int PostorId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPuja { get; set; }

    // Navegación
    public Subasta? Subasta { get; set; }
    public Usuario? Postor { get; set; }
}
