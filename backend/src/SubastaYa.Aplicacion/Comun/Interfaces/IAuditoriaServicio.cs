using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Aplicacion.Comun.Interfaces;

/// <summary>
/// Servicio de auditoría inmutable.
/// Abstracción para el registro y consulta del audit log.
/// La implementación concreta reside en Infraestructura.
/// </summary>
public interface IAuditoriaServicio
{
    /// <summary>
    /// Registro de un evento de auditoría.
    /// El detalle adicional se serializa internamente a JSON.
    /// </summary>
    Task RegistroAsync(
        TipoAccionAuditoria accion,
        string entidadTipo,
        int entidadId,
        object detalle,
        string? usuarioOrigen = null);

    /// <summary>
    /// Historial de auditoría de una entidad específica.
    /// </summary>
    Task<IEnumerable<RegistroAuditoria>> HistorialPorEntidadAsync(string entidadTipo, int entidadId);

    /// <summary>
    /// Historial de auditoría filtrado por tipo de acción.
    /// </summary>
    Task<IEnumerable<RegistroAuditoria>> HistorialPorAccionAsync(TipoAccionAuditoria accion);
}
