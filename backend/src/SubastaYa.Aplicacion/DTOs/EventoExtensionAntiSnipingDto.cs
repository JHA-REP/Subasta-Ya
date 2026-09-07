namespace SubastaYa.Aplicacion.DTOs;

/// <summary>
/// Evento de extensión de tiempo por regla Anti-Sniping.
/// Se emite por SignalR cuando una puja en los últimos 60 s extiende la subasta 2 minutos.
/// </summary>
public class EventoExtensionAntiSnipingDto
{
    /// <summary>Identificador de la subasta.</summary>
    public int SubastaId { get; set; }

    /// <summary>Fecha y hora UTC anterior a la extensión.</summary>
    public DateTime FechaFinAnteriorUtc { get; set; }

    /// <summary>Nueva fecha y hora UTC de finalización.</summary>
    public DateTime FechaFinNuevaUtc { get; set; }

    /// <summary>Segundos que se extendió la subasta (normalmente 120).</summary>
    public int ExtensionSegundos { get; set; }

    /// <summary>Alias anonimizado del postor que activó la extensión.</summary>
    public string PostorAnonimizado { get; set; } = string.Empty;
}
