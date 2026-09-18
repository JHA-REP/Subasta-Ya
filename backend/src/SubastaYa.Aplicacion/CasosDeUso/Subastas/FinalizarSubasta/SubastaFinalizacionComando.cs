namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.FinalizarSubasta;


/// Comando para la finalización automática de subastas vencidas.

public class SubastaFinalizacionComando
{
   
    /// Fecha de corte para determinar subastas vencidas.
    
    
    public DateTime? FechaCorte { get; set; }
}
