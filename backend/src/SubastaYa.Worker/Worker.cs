using SubastaYa.Aplicacion.CasosDeUso.Subastas.FinalizarSubasta;

namespace SubastaYa.Worker;

/// <summary>
/// Worker de procesamiento de subastas.
/// Ejecuta periódicamente el manejador de finalización para detectar
/// subastas vencidas y procesarlas (liquidación o declaración desierta).
/// No contiene lógica de negocio — delega al manejador de aplicación.
/// </summary>
public class ProcesadorSubastas : BackgroundService
{
    private readonly IServiceScopeFactory _fabricaAlcance;
    private readonly ILogger<ProcesadorSubastas> _registro;
    private readonly TimeSpan _intervalo = TimeSpan.FromSeconds(30);

    public ProcesadorSubastas(
        IServiceScopeFactory fabricaAlcance,
        ILogger<ProcesadorSubastas> registro)
    {
        _fabricaAlcance = fabricaAlcance;
        _registro = registro;
    }

    protected override async Task ExecuteAsync(CancellationToken tokenCancelacion)
    {
        _registro.LogInformation("ProcesadorSubastas iniciado. Intervalo de ejecución: {Intervalo}s.",
            _intervalo.TotalSeconds);

        while (!tokenCancelacion.IsCancellationRequested)
        {
            try
            {
                await CicloProcesamientoAsync(tokenCancelacion);
            }
            catch (OperationCanceledException) when (tokenCancelacion.IsCancellationRequested)
            {
                // Cancelación solicitada — salida limpia
                break;
            }
            catch (Exception excepcion)
            {
                _registro.LogError(excepcion,
                    "Error inesperado en el ciclo del ProcesadorSubastas. Reintentando en {Intervalo}s.",
                    _intervalo.TotalSeconds);
            }

            await Task.Delay(_intervalo, tokenCancelacion);
        }

        _registro.LogInformation("ProcesadorSubastas detenido.");
    }

    /// <summary>
    /// Ciclo individual de procesamiento: crea un scope, resuelve el manejador y ejecuta.
    /// </summary>
    private async Task CicloProcesamientoAsync(CancellationToken tokenCancelacion)
    {
        using var alcance = _fabricaAlcance.CreateScope();
        var manejador = alcance.ServiceProvider.GetRequiredService<SubastaFinalizacionManejador>();

        var comando = new SubastaFinalizacionComando
        {
            FechaCorte = DateTime.UtcNow
        };

        var resultados = await manejador.EjecucionAsync(comando);
        var listaResultados = resultados.ToList();

        if (listaResultados.Count > 0)
        {
            foreach (var resultado in listaResultados)
            {
                _registro.LogInformation(
                    "Subasta #{SubastaId} ({Titulo}) → {Estado}. Monto liquidado: {Monto}. Movimientos: {Movimientos}.",
                    resultado.SubastaId,
                    resultado.Titulo,
                    resultado.EstadoResultante,
                    resultado.MontoLiquidado?.ToString("C") ?? "N/A",
                    resultado.MovimientosGenerados);
            }

            _registro.LogInformation("Ciclo completado: {Cantidad} subasta(s) procesada(s).",
                listaResultados.Count);
        }
    }
}
