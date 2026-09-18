using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Dominio.Entidades;

namespace SubastaYa.Infraestructura.Configuraciones;

/// <summary>
/// Configuración Fluent API para la entidad Categoria.
/// </summary>
public class CategoriaConfiguracion : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> constructor)
    {
        constructor.ToTable("Categorias");

        constructor.HasKey(c => c.Id);

        constructor.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        constructor.HasIndex(c => c.Nombre)
            .IsUnique();

        constructor.Property(c => c.Descripcion)
            .HasMaxLength(500);

        // Relación: Categoria 1 → N Subastas
        constructor.HasMany(c => c.Subastas)
            .WithOne(s => s.Categoria)
            .HasForeignKey(s => s.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
