using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Aplicacion.DTOs;

/// <summary>
/// Estado del temporizador de una subasta activa.
/// Se emite periódicamente por SignalR para mantener el contador del frontend sincronizado.
/// </summary>
public class InformacionTemporizadorDto
{
    /// <summary>Identificador de la subasta.</summary>
    public int SubastaId { get; set; }

    /// <summary>Fecha y hora UTC de finalización actual de la subasta.</summary>
    public DateTime FechaFinUtc { get; set; }

    /// <summary>Segundos restantes hasta la finalización (puede ser negativo si ya venció).</summary>
    public long SegundosRestantes { get; set; }

    /// <summary>
    /// Indica que quedan menos de 60 segundos.
    /// El frontend usa este flag para activar el estado visual de urgencia.
    /// </summary>
    public bool Critico { get; set; }

    /// <summary>Estado actual de la subasta.</summary>
    public EstadoSubasta EstadoSubasta { get; set; }

    /// <summary>Precio actual de la subasta al momento de la emisión.</summary>
    public decimal PrecioActual { get; set; }
}
