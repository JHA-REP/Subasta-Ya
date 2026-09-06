using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Dominio.Entidades;

namespace SubastaYa.Infraestructura.Configuraciones;

/// <summary>
/// Configuración Fluent API para la entidad Subasta.
/// Incluye Optimistic Locking con RowVersion.
/// </summary>
public class SubastaConfiguracion : IEntityTypeConfiguration<Subasta>
{
    public void Configure(EntityTypeBuilder<Subasta> constructor)
    {
        constructor.ToTable("Subastas");

        constructor.HasKey(s => s.Id);

        constructor.Property(s => s.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        constructor.Property(s => s.Descripcion)
            .HasMaxLength(2000);

        constructor.Property(s => s.PrecioInicial)
            .IsRequired()
            .HasPrecision(18, 2);

        constructor.Property(s => s.Estado)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        constructor.Property(s => s.FechaInicio)
            .IsRequired();

        constructor.Property(s => s.FechaFin)
            .IsRequired();

        // Optimistic Locking
        constructor.Property(s => s.RowVersion)
            .IsRowVersion();

        // Relación: Subasta 1 — N Pujas
        constructor.HasMany(s => s.Pujas)
            .WithOne(p => p.Subasta)
            .HasForeignKey(p => p.SubastaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
