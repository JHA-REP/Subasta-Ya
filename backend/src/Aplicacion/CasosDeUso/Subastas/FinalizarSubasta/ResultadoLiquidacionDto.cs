using Dominio.Enumeraciones;

namespace Aplicacion.CasosDeUso.Subastas.FinalizarSubasta;

/// <summary>
/// Resumen de la liquidación de una subasta procesada por el Worker.
/// </summary>
public class ResultadoLiquidacionDto
{
    public int SubastaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public EstadoSubasta EstadoResultante { get; set; }
    public decimal? MontoLiquidado { get; set; }
    public int MovimientosGenerados { get; set; }
}
