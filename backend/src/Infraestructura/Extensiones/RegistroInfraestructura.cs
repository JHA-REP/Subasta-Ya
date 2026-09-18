using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Aplicacion.Comun.Interfaces;
using Aplicacion.Interfaces;
using Infraestructura.Persistencia;
using Infraestructura.Repositorios;
using Infraestructura.Servicios;

namespace Infraestructura.Extensiones;

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
