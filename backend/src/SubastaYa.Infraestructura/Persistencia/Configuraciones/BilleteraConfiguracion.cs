using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Dominio.Entidades;

namespace SubastaYa.Infraestructura.Configuraciones;

/// <summary>
/// Configuración Fluent API para la entidad Billetera.
/// Incluye Optimistic Locking con RowVersion.
/// </summary>
public class BilleteraConfiguracion : IEntityTypeConfiguration<Billetera>
{
    public void Configure(EntityTypeBuilder<Billetera> constructor)
    {
        constructor.ToTable("Billeteras");

        constructor.HasKey(b => b.Id);

        constructor.Property(b => b.SaldoDisponible)
            .IsRequired()
            .HasPrecision(18, 2);

        constructor.Property(b => b.SaldoRetenido)
            .IsRequired()
            .HasPrecision(18, 2);

        // Optimistic Locking
        constructor.Property(b => b.RowVersion)
            .IsRowVersion();

        constructor.HasIndex(b => b.UsuarioId)
            .IsUnique();

        // Relación: Billetera 1 — N MovimientosContables
        constructor.HasMany(b => b.Movimientos)
            .WithOne(m => m.Billetera)
            .HasForeignKey(m => m.BilleteraId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
