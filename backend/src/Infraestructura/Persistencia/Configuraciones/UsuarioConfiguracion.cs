using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dominio.Entidades;
using Dominio.Enumeraciones;

namespace Infraestructura.Configuraciones;

/// <summary>
/// Configuración Fluent API para la entidad Usuario.
/// </summary>
public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> constructor)
    {
        constructor.ToTable("Usuarios");

        constructor.HasKey(u => u.Id);

        constructor.Property(u => u.Alias)
            .IsRequired()
            .HasMaxLength(50);

        constructor.HasIndex(u => u.Alias)
            .IsUnique();

        constructor.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        constructor.HasIndex(u => u.Email)
            .IsUnique();

        constructor.Property(u => u.ClaveHash)
            .IsRequired()
            .HasMaxLength(256);

        constructor.Property(u => u.Rol)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        constructor.Property(u => u.FechaRegistro)
            .IsRequired();

        constructor.Property(u => u.Version)
            .IsRowVersion();

        // Relación: Usuario 1 → 0..1 Billetera
        constructor.HasOne(u => u.Billetera)
            .WithOne(b => b.Usuario)
            .HasForeignKey<Billetera>(b => b.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación: Usuario 1 → N Subastas (como vendedor)
        constructor.HasMany(u => u.SubastasComoVendedor)
            .WithOne(s => s.Vendedor)
            .HasForeignKey(s => s.VendedorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: Usuario 1 → N Pujas (como postor)
        constructor.HasMany(u => u.Pujas)
            .WithOne(p => p.Postor)
            .HasForeignKey(p => p.PostorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
