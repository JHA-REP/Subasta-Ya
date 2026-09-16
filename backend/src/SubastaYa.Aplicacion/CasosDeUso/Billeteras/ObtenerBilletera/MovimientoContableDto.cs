using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Aplicacion.CasosDeUso.Billeteras.ObtenerBilletera;

public class MovimientoContableDto
{
    public int Id { get; set; }
    public int BilleteraId { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public decimal Monto { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public DateTime FechaMovimiento { get; set; }
}

