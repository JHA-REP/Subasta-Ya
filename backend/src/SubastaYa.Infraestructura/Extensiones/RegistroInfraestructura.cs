using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubastaYa.Dominio.Interfaces;
using SubastaYa.Infraestructura.Persistencia;
using SubastaYa.Infraestructura.Repositorios;

namespace SubastaYa.Infraestructura.Extensiones;

/// <summary>
/// Extension para registrar los servicios de infraestructura en el contenedor DI.
/// </summary>
public static class RegistroInfraestructura
{
    public static IServiceCollection ConInfraestructura(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        // EF Core - SQL Server
        servicios.AddDbContext<SubastaYaDbContext>(opciones =>
            opciones.UseSqlServer(
                configuracion.GetConnectionString("SubastaYaDb"),
                sql => sql.MigrationsAssembly(typeof(SubastaYaDbContext).Assembly.FullName)));

        // Repositorios
        servicios.AddScoped(typeof(IRepositorio<>), typeof(RepositorioGenerico<>));
        servicios.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();

        return servicios;
    }
}
