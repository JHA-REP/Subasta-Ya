using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Dominio.Entidades;

/// <summary>
/// Registro de auditoría inmutable.
/// Representa un evento de negocio relevante ocurrido en el sistema.
/// NUNCA se modifica ni elimina — solo se agrega (append-only).
/// </summary>
public class RegistroAuditoria : EntidadBase
{
    /// <summary>Tipo de acción auditada.</summary>
    public TipoAccionAuditoria Accion { get; set; }

    /// <summary>Nombre del tipo de entidad afectada (ej: "Subasta", "Billetera", "Puja").</summary>
    public string EntidadTipo { get; set; } = string.Empty;

    /// <summary>Identificador de la entidad afectada.</summary>
    public int EntidadId { get; set; }

    /// <summary>Detalle serializado en JSON con contexto adicional del evento.</summary>
    public string DetalleJson { get; set; } = "{}";

    /// <summary>
    /// Identificador del actor que originó el evento.
    /// Null si fue originado por un proceso automático (Worker, sistema).
    /// </summary>
    public string? UsuarioOrigen { get; set; }

    /// <summary>Fecha y hora UTC del registro. Inmutable desde la creación.</summary>
    public DateTime FechaRegistro { get; set; }
}
