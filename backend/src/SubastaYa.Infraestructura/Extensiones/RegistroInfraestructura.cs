using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubastaYa.Aplicacion.Comun.Interfaces;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Infraestructura.Persistencia;
using SubastaYa.Infraestructura.Repositorios;
using SubastaYa.Infraestructura.Servicios;

namespace SubastaYa.Infraestructura.Extensiones;

/// <summary>
/// Extension para registrar todos los servicios de infraestructura en el contenedor DI.
/// </summary>
public static class RegistroInfraestructura
{
    public static IServiceCollection ConInfraestructura(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        // EF Core — SQL Server
        servicios.AddDbContext<SubastaYaDbContext>(opciones =>
            opciones.UseSqlServer(
                configuracion.GetConnectionString("SubastaYaDb"),
                sql => sql.MigrationsAssembly(typeof(SubastaYaDbContext).Assembly.FullName)));

        // Repositorios genérico y de auditoría
        servicios.AddScoped(typeof(IRepositorio<>), typeof(RepositorioGenerico<>));
        servicios.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
        servicios.AddScoped<IAuditoriaRepositorio, AuditoriaRepositorio>();

        // Servicios de aplicación
        servicios.AddScoped<IAuditoriaServicio, AuditoriaServicio>();

        return servicios;
    }
}
