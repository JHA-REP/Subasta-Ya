using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.FinalizarSubasta;


/// Datos del resultado de finalización de una subasta.
/// Se utiliza para notificaciones en tiempo real y respuestas del manejador.

public class SubastaFinalizadaDto
{
    public int SubastaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public EstadoSubasta EstadoResultante { get; set; }
    public int? GanadorId { get; set; }
    public string? GanadorAlias { get; set; }
    public decimal? MontoFinal { get; set; }
    public DateTime FechaFinalizacion { get; set; }
}
