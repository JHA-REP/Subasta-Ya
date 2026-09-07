using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Dominio.Entidades;

namespace SubastaYa.Infraestructura.Configuraciones;

/// <summary>
/// Configuración Fluent API de la tabla de auditoría inmutable.
/// Sin DeleteBehavior.Cascade — los registros son permanentes.
/// </summary>
public class AuditoriaConfiguracion : IEntityTypeConfiguration<RegistroAuditoria>
{
    public void Configure(EntityTypeBuilder<RegistroAuditoria> constructor)
    {
        constructor.ToTable("AuditoriaRegistros");

        constructor.HasKey(r => r.Id);

        constructor.Property(r => r.Accion)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        constructor.Property(r => r.EntidadTipo)
            .IsRequired()
            .HasMaxLength(100);

        constructor.Property(r => r.EntidadId)
            .IsRequired();

        constructor.Property(r => r.DetalleJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        constructor.Property(r => r.UsuarioOrigen)
            .HasMaxLength(200);

        constructor.Property(r => r.FechaRegistro)
            .IsRequired();

        // Índice para consultas por entidad (más frecuentes)
        constructor.HasIndex(r => new { r.EntidadTipo, r.EntidadId })
            .HasDatabaseName("IX_AuditoriaRegistros_Entidad");

        // Índice para consultas cronológicas y por acción
        constructor.HasIndex(r => r.FechaRegistro)
            .HasDatabaseName("IX_AuditoriaRegistros_Fecha");

        constructor.HasIndex(r => r.Accion)
            .HasDatabaseName("IX_AuditoriaRegistros_Accion");
    }
}
