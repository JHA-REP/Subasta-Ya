using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Dominio.Entidades;

/// <summary>
/// Registro contable de un movimiento en una billetera.
/// </summary>
public class MovimientoContable : EntidadBase
{
    public int BilleteraId { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public decimal Monto { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public DateTime FechaMovimiento { get; set; }

    // Navegación
    public Billetera? Billetera { get; set; }
}
