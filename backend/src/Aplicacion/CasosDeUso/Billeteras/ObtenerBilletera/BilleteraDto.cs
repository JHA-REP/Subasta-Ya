using Dominio.Enumeraciones;

namespace Aplicacion.CasosDeUso.Billeteras.ObtenerBilletera;

public class BilleteraDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioAlias { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
    public decimal SaldoRetenido { get; set; }
    public decimal SaldoDisponible => Saldo - SaldoRetenido;
    public List<MovimientoContableDto> Movimientos { get; set; } = new();
}


