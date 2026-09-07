namespace SubastaYa.Aplicacion.DTOs;

public class PujaDto
{
    public int Id { get; set; }
    public int SubastaId { get; set; }
    public int PostorId { get; set; }
    public string PostorAlias { get; set; } = string.Empty;

    /// <summary>
    /// Alias anonimizado para difusión pública por WebSocket (primeros 3 chars + ***).
    /// Ejemplo: "mar***" para "maria_compradora".
    /// </summary>
    public string LiderAnonimizado { get; set; } = string.Empty;

    public decimal Monto { get; set; }
    public DateTime FechaPuja { get; set; }
}

public class NuevaPujaDto
{
    public int SubastaId { get; set; }
    public int PostorId { get; set; }
    public decimal Monto { get; set; }
}
