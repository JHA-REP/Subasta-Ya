using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Infraestructura.Extensiones;

namespace SubastaYa.Worker;

/// <summary>
/// Punto de entrada del Worker Service (ejecución independiente).
/// Cuando se ejecuta standalone, utiliza un notificador de registro (logging)
/// en lugar de SignalR, ya que el Hub reside en el proceso de la API.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var constructor = Host.CreateApplicationBuilder(args);

        // Infraestructura (EF Core, Repositorios, Servicios, Manejadores)
        constructor.Services.ConInfraestructura(constructor.Configuration);

        // Notificador de subastas: implementación de registro (sin SignalR)
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

    public Task EventoSubastaFinalizada(SubastaYa.Aplicacion.DTOs.SubastaFinalizadaDto datos)
    {
        _registro.LogInformation(
            "[Notificación] Subasta #{SubastaId} finalizada. Ganador: {Ganador}. Monto: {Monto}.",
            datos.SubastaId, datos.GanadorAlias, datos.MontoFinal);
        return Task.CompletedTask;
    }

    public Task EventoSubastaDesierta(int subastaId)
    {
        _registro.LogInformation(
            "[Notificación] Subasta #{SubastaId} declarada desierta (sin pujas).", subastaId);
        return Task.CompletedTask;
    }

    public Task EventoPujaRecibida(SubastaYa.Aplicacion.DTOs.PujaDto datos)
    {
        _registro.LogInformation(
            "[Notificación] Nueva puja en subasta #{SubastaId}. Monto: {Monto}.",
            datos.SubastaId, datos.Monto);
        return Task.CompletedTask;
    }
}
