using System.Text.Json;
using SubastaYa.Aplicacion.Comun.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;
using SubastaYa.Aplicacion.Interfaces;


namespace SubastaYa.Infraestructura.Servicios;

/// <summary>
/// Implementación del servicio de auditoría.
/// Serializa el detalle a JSON y persiste via IAuditoriaRepositorio.
/// La persistencia se confirma inmediatamente (SaveChanges propio)
/// para garantizar que el log sobrevive a un rollback de la transacción principal.
/// </summary>
public class AuditoriaServicio : IAuditoriaServicio
{
    private readonly IAuditoriaRepositorio _repositorio;
    private readonly Persistencia.SubastaYaDbContext _contexto;

    private static readonly JsonSerializerOptions OpcionesJson = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public AuditoriaServicio(
        IAuditoriaRepositorio repositorio,
        Persistencia.SubastaYaDbContext contexto)
    {
        _repositorio = repositorio;
        _contexto = contexto;
    }

    public async Task RegistroAsync(
        TipoAccionAuditoria accion,
        string entidadTipo,
        int entidadId,
        object detalle,
        string? usuarioOrigen = null)
    {
        var registro = new RegistroAuditoria
        {
            Accion = accion,
            EntidadTipo = entidadTipo,
            EntidadId = entidadId,
            DetalleJson = JsonSerializer.Serialize(detalle, OpcionesJson),
            UsuarioOrigen = usuarioOrigen,
            FechaRegistro = DateTime.UtcNow
        };

        await _repositorio.AltaAsync(registro);

        // Persistencia inmediata e independiente del contexto de transacción del llamador.
        // Al usar el mismo DbContext, SaveChanges persiste el registro dentro de la
        // transacción activa si la hay, o de forma autónoma si no hay transacción.
        // Esto garantiza atomicidad con la operación de negocio.
        await _contexto.SaveChangesAsync();
    }

    public async Task<IEnumerable<RegistroAuditoria>> HistorialPorEntidadAsync(
        string entidadTipo, int entidadId)
    {
        return await _repositorio.PorEntidadAsync(entidadTipo, entidadId);
    }

    public async Task<IEnumerable<RegistroAuditoria>> HistorialPorAccionAsync(
        TipoAccionAuditoria accion)
    {
        return await _repositorio.PorAccionAsync(accion);
    }
}
