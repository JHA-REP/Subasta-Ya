using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Aplicacion.Servicios;
using SubastaYa.Dominio.Interfaces;
using SubastaYa.Infraestructura.Persistencia;
using SubastaYa.Infraestructura.Repositorios;

namespace SubastaYa.Infraestructura.Extensiones;

/// <summary>
/// Extensión para registrar todos los servicios de infraestructura y aplicación en el contenedor DI.
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

        // Repositorios
        servicios.AddScoped(typeof(IRepositorio<>), typeof(RepositorioGenerico<>));
        servicios.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();

        // Servicios de aplicación
        servicios.AddScoped<IServicioSubastas, ServicioSubastas>();
        servicios.AddScoped<IServicioUsuarios, ServicioUsuarios>();
        servicios.AddScoped<IServicioCategorias, ServicioCategorias>();
        servicios.AddScoped<IServicioPujas, ServicioPujas>();
        servicios.AddScoped<IServicioBilleteras, ServicioBilleteras>();

        return servicios;
    }
}
