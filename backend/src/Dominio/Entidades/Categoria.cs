namespace Dominio.Entidades;

/// <summary>
/// Categoría que agrupa las subastas.
/// </summary>
public class Categoria : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    // Navegación
    public ICollection<Subasta> Subastas { get; set; } = [];
}
