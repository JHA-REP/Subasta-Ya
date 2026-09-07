using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;
using SubastaYa.Infraestructura.Extensiones;

namespace SubastaYa.Worker;

/// <summary>
/// Punto de entrada del Worker Service (ejecución independiente).
/// Usa la misma BD que la API (cadena de conexión SubastaYaDb).
/// Notificador SignalR no disponible en este proceso — usa stub de logging.
/// Auditoría se escribe a la misma BD via IAuditoriaServicio registrado en Infraestructura.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var constructor = Host.CreateApplicationBuilder(args);

        // Infraestructura completa (incluye IAuditoriaRepositorio e IAuditoriaServicio)
        constructor.Services.ConInfraestructura(constructor.Configuration);

        // Notificador stub (sin SignalR en este proceso)
        constructor.Services.AddScoped<INotificadorSubastas, NotificadorSubastasRegistro>();

        // Worker de procesamiento de subastas
        constructor.Services.AddHostedService<ProcesadorSubastas>();

        var host = constructor.Build();
        host.Run();
    }
}

/// <summary>
/// Implementación de INotificadorSubastas que solo registra eventos en el log.
/// Se usa cuando el Worker corre como proceso independiente (sin SignalR).
/// </summary>
internal class NotificadorSubastasRegistro : INotificadorSubastas
{
    private readonly ILogger<NotificadorSubastasRegistro> _registro;

    public NotificadorSubastasRegistro(ILogger<NotificadorSubastasRegistro> registro)
    {
        _registro = registro;
    }

    public Task EventoSubastaFinalizada(SubastaFinalizadaDto datos)
    {
        _registro.LogInformation(
            "[Notificación] Subasta #{SubastaId} finalizada. Ganador: {Ganador}. Monto: {Monto}.",
            datos.SubastaId, datos.GanadorAlias, datos.MontoFinal);
        return Task.CompletedTask;
    }

    public Task EventoSubastaDesierta(int subastaId)
    {
        _registro.LogInformation(
            "[Notificación] Subasta #{SubastaId} declarada desierta.", subastaId);
        return Task.CompletedTask;
    }

    public Task EventoPujaRecibida(PujaDto datos)
    {
        _registro.LogInformation(
            "[Notificación] Puja en subasta #{SubastaId}. Monto: {Monto}.",
            datos.SubastaId, datos.Monto);
        return Task.CompletedTask;
    }

    public Task EstadoTemporizador(InformacionTemporizadorDto datos)
    {
        // En el Worker standalone no se emite temporizador (no hay hub)
        return Task.CompletedTask;
    }

    public Task EventoExtensionAntiSniping(EventoExtensionAntiSnipingDto datos)
    {
        _registro.LogInformation(
            "[Notificación] Extensión Anti-Sniping en subasta #{SubastaId}. Nueva fecha: {FechaFin}.",
            datos.SubastaId, datos.FechaFinNuevaUtc);
        return Task.CompletedTask;
    }
}
