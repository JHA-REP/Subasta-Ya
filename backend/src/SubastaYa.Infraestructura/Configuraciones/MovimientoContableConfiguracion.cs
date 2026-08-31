using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Infraestructura.Configuraciones;

/// <summary>
/// Configuración Fluent API para la entidad MovimientoContable.
/// </summary>
public class MovimientoContableConfiguracion : IEntityTypeConfiguration<MovimientoContable>
{
    public void Configure(EntityTypeBuilder<MovimientoContable> constructor)
    {
        constructor.ToTable("MovimientosContables");

        constructor.HasKey(m => m.Id);

        constructor.Property(m => m.Tipo)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        constructor.Property(m => m.Monto)
            .IsRequired()
            .HasPrecision(18, 2);

        constructor.Property(m => m.Concepto)
            .IsRequired()
            .HasMaxLength(500);

        constructor.Property(m => m.FechaMovimiento)
            .IsRequired();
    }
}
