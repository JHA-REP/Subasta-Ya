namespace SubastaYa.Worker;

/// <summary>
/// Worker de procesamiento de subastas.
/// Placeholder — la lógica completa se implementará en etapas posteriores:
/// - Cierre automático de subastas vencidas
/// - Liberación de fondos retenidos
/// - Notificaciones a ganadores
/// </summary>
public class ProcesadorSubastas : BackgroundService
{
    private readonly ILogger<ProcesadorSubastas> _registro;

    public ProcesadorSubastas(ILogger<ProcesadorSubastas> registro)
    {
        _registro = registro;
    }

    protected override async Task ExecuteAsync(CancellationToken tokenCancelacion)
    {
        _registro.LogInformation("ProcesadorSubastas iniciado. Esperando implementación de lógica de negocio.");

        while (!tokenCancelacion.IsCancellationRequested)
        {
            // TODO: Implementar en etapas posteriores:
            // 1. Consulta de subastas vencidas sin procesar
            // 2. Cierre de subastas y determinación de ganador
            // 3. Procesamiento de escrow (retención/liberación de fondos)
            // 4. Envío de notificaciones vía SignalR

            await Task.Delay(TimeSpan.FromMinutes(1), tokenCancelacion);
        }
    }
}
