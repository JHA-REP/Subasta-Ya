using Microsoft.EntityFrameworkCore;
using Dominio.Entidades;
using Dominio.Enumeraciones;

using Aplicacion.Interfaces;
using Infraestructura.Persistencia;

namespace Infraestructura.Repositorios;

/// <summary>
/// Repositorio inmutable de auditoría.
/// Solo soporta alta y lecturas. Sin Update, sin Delete.
/// </summary>
public class AuditoriaRepositorio : IAuditoriaRepositorio
{
    private readonly SubastaYaDbContext _contexto;

    public AuditoriaRepositorio(SubastaYaDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task AltaAsync(RegistroAuditoria registro)
    {
        await _contexto.AuditoriaRegistros.AddAsync(registro);
    }

    public async Task<IEnumerable<RegistroAuditoria>> PorEntidadAsync(string entidadTipo, int entidadId)
    {
        return await _contexto.AuditoriaRegistros
            .Where(r => r.EntidadTipo == entidadTipo && r.EntidadId == entidadId)
            .OrderBy(r => r.FechaRegistro)
            .ToListAsync();
    }

    public async Task<IEnumerable<RegistroAuditoria>> PorAccionAsync(TipoAccionAuditoria accion)
    {
        return await _contexto.AuditoriaRegistros
            .Where(r => r.Accion == accion)
            .OrderBy(r => r.FechaRegistro)
            .ToListAsync();
    }
}
