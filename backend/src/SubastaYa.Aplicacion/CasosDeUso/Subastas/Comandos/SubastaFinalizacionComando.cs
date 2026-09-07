namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.Comandos;

/// <summary>
/// Comando para la finalización automática de subastas vencidas.
/// </summary>
public class SubastaFinalizacionComando
{
    /// <summary>
    /// Fecha de corte para determinar subastas vencidas.
    /// Si no se especifica, se utiliza DateTime.UtcNow.
    /// </summary>
    public DateTime? FechaCorte { get; set; }
}
