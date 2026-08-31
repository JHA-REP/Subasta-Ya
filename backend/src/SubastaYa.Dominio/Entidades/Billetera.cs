namespace SubastaYa.Dominio.Entidades;

/// <summary>
/// Billetera virtual asociada a un usuario. Controla saldo disponible y retenido.
/// Protegida con Optimistic Locking (RowVersion).
/// </summary>
public class Billetera : EntidadBase
{
    public int UsuarioId { get; set; }
    public decimal Saldo { get; set; }
    public decimal SaldoRetenido { get; set; }

    /// <summary>
    /// Token de concurrencia optimista — crítico para operaciones financieras.
    /// </summary>
    public byte[] Version { get; set; } = [];

    // Navegación
    public Usuario? Usuario { get; set; }
    public ICollection<MovimientoContable> Movimientos { get; set; } = [];
}
