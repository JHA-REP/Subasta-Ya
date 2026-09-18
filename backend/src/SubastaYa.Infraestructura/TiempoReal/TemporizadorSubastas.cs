using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SubastaYa.Aplicacion.Comun.Interfaces;
using SubastaYa.Aplicacion.Comun.DTOs;
using SubastaYa.Dominio.Enumeraciones;
using SubastaYa.Aplicacion.Interfaces;


namespace SubastaYa.Infraestructura.TiempoReal;

/// <summary>
/// BackgroundService de sincronización del temporizador.
/// Cada 10 segundos consulta subastas activas y emite el estado del temporizador
/// via SignalR para mantener el contador del cliente sincronizado con el servidor.
/// 
/// Responsabilidad única: emitir estadoTemporizador.
/// La detección de subastas vencidas es responsabilidad del ProcesadorSubastas.
/// </summary>
public class TemporizadorSubastas : BackgroundService
{
    private readonly IServiceScopeFactory _fabricaAlcance;
    private readonly ILogger<TemporizadorSubastas> _registro;
    private readonly TimeSpan _intervalo = TimeSpan.FromSeconds(10);

    public TemporizadorSubastas(
        IServiceScopeFactory fabricaAlcance,
        ILogger<TemporizadorSubastas> registro)
    {
        _fabricaAlcance = fabricaAlcance;
        _registro = registro;
    }

    protected override async Task ExecuteAsync(CancellationToken tokenCancelacion)
    {
        _registro.LogInformation(
            "TemporizadorSubastas iniciado. Intervalo: {Intervalo}s.", _intervalo.TotalSeconds);

        while (!tokenCancelacion.IsCancellationRequested)
        {
            try
            {
                await CicloDifusionAsync(tokenCancelacion);
            }
            catch (OperationCanceledException) when (tokenCancelacion.IsCancellationRequested)
            {
                break;
            }
            catch (Exception excepcion)
            {
                _registro.LogError(excepcion,
                    "Error en TemporizadorSubastas. Reintentando en {Intervalo}s.",
                    _intervalo.TotalSeconds);
            }

            await Task.Delay(_intervalo, tokenCancelacion);
        }

        _registro.LogInformation("TemporizadorSubastas detenido.");
    }

    /// <summary>
    /// Un ciclo de difusión: consulta subastas activas y emite estadoTemporizador a cada grupo.
    /// </summary>
    private async Task CicloDifusionAsync(CancellationToken tokenCancelacion)
    {
        using var alcance = _fabricaAlcance.CreateScope();
        var repositorioSubastas = alcance.ServiceProvider
            .GetRequiredService<IRepositorio<SubastaYa.Dominio.Entidades.Subasta>>();
        var notificador = alcance.ServiceProvider
            .GetRequiredService<INotificadorSubastas>();

        var ahora = DateTime.UtcNow;

        var subastasActivas = await repositorioSubastas.FiltradasAsync(
            s => s.Estado == EstadoSubasta.Activa);

        foreach (var subasta in subastasActivas)
        {
            var segundosRestantes = (long)(subasta.FechaFin - ahora).TotalSeconds;

            var informacion = new InformacionTemporizadorDto
            {
                SubastaId = subasta.Id,
                FechaFinUtc = subasta.FechaFin,
                SegundosRestantes = segundosRestantes,
                Critico = segundosRestantes is >= 0 and < 60,
                EstadoSubasta = subasta.Estado,
                PrecioActual = subasta.PrecioActual
            };

            await notificador.EstadoTemporizador(informacion);
        }

        if (subastasActivas.Any())
        {
            _registro.LogDebug(
                "TemporizadorSubastas: {Cantidad} subasta(s) activa(s) notificadas.",
                subastasActivas.Count());
        }
    }
}
