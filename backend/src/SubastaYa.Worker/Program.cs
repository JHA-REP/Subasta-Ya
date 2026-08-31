using SubastaYa.Infraestructura.Extensiones;

namespace SubastaYa.Worker;

/// <summary>
/// Punto de entrada del Worker Service.
/// Preparado para implementar el procesamiento de subastas vencidas en etapas posteriores.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var constructor = Host.CreateApplicationBuilder(args);

        // Infraestructura (EF Core, Repositorios, Servicios)
        constructor.Services.ConInfraestructura(constructor.Configuration);

        // Worker de procesamiento de subastas
        constructor.Services.AddHostedService<ProcesadorSubastas>();

        var host = constructor.Build();
        host.Run();
    }
}
