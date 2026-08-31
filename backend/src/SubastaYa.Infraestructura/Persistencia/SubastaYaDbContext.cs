using Microsoft.EntityFrameworkCore;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Infraestructura.Configuraciones;
using SubastaYa.Infraestructura.Datos;

namespace SubastaYa.Infraestructura.Persistencia;

/// <summary>
/// Contexto de base de datos principal de SubastaYa.
/// </summary>
public class SubastaYaDbContext : DbContext
{
    public SubastaYaDbContext(DbContextOptions<SubastaYaDbContext> opciones) : base(opciones)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Billetera> Billeteras => Set<Billetera>();
    public DbSet<MovimientoContable> MovimientosContables => Set<MovimientoContable>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Subasta> Subastas => Set<Subasta>();
    public DbSet<Puja> Pujas => Set<Puja>();

    protected override void OnModelCreating(ModelBuilder constructor)
    {
        base.OnModelCreating(constructor);

        // Aplicar configuraciones de Fluent API
        constructor.ApplyConfiguration(new UsuarioConfiguracion());
        constructor.ApplyConfiguration(new BilleteraConfiguracion());
        constructor.ApplyConfiguration(new MovimientoContableConfiguracion());
        constructor.ApplyConfiguration(new CategoriaConfiguracion());
        constructor.ApplyConfiguration(new SubastaConfiguracion());
        constructor.ApplyConfiguration(new PujaConfiguracion());

        // Datos semilla
        DatosSemilla.Aplicacion(constructor);
    }
}
