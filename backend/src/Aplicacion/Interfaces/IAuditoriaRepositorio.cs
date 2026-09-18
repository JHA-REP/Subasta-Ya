using Dominio.Entidades;
using Dominio.Enumeraciones;

namespace Aplicacion.Interfaces;

/// <summary>
/// Repositorio de auditoría inmutable.
/// Solo soporta escritura (alta) y consultas de lectura.
/// No expone Update ni Delete para preservar la inmutabilidad del log.
/// </summary>
public interface IAuditoriaRepositorio
{
    /// <summary>
    /// Alta de un nuevo registro de auditoría.
    /// Operación append-only — jamás debe modificarse el registro creado.
    /// </summary>
    Task AltaAsync(RegistroAuditoria registro);

    /// <summary>
    /// Historial de auditoría de una entidad específica, ordenado por fecha ascendente.
    /// </summary>
    Task<IEnumerable<RegistroAuditoria>> PorEntidadAsync(string entidadTipo, int entidadId);

    /// <summary>
    /// Historial de auditoría filtrado por tipo de acción.
    /// </summary>
    Task<IEnumerable<RegistroAuditoria>> PorAccionAsync(TipoAccionAuditoria accion);
}
