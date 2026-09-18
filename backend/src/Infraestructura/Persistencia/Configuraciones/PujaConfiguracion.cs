using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dominio.Entidades;

namespace Infraestructura.Configuraciones;

/// <summary>
/// Configuración Fluent API para la entidad Puja.
/// </summary>
public class PujaConfiguracion : IEntityTypeConfiguration<Puja>
{
    public void Configure(EntityTypeBuilder<Puja> constructor)
    {
        constructor.ToTable("Pujas");

        constructor.HasKey(p => p.Id);

        constructor.Property(p => p.Monto)
            .IsRequired()
            .HasPrecision(18, 2);

        constructor.Property(p => p.FechaPuja)
            .IsRequired();
    }
}
