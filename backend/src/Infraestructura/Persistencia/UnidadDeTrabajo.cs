using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Dominio.Excepciones;

using Aplicacion.Interfaces;
using Infraestructura.Persistencia;

namespace Infraestructura.Repositorios;

/// <summary>
/// Implementación de Unidad de Trabajo con soporte para transacciones ACID
/// y manejo de conflictos de concurrencia (Optimistic Locking).
/// </summary>
public class UnidadDeTrabajo : IUnidadDeTrabajo
{
    private readonly SubastaYaDbContext _contexto;
    private IDbContextTransaction? _transaccion;

    public UnidadDeTrabajo(SubastaYaDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<int> ConfirmacionAsync()
    {
        try
        {
            return await _contexto.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ExcepcionConcurrencia(
                "Se detectó un conflicto de concurrencia. El registro fue modificado por otro proceso.",
                ex);
        }
    }

    public async Task InicioTransaccionAsync()
    {
        _transaccion = await _contexto.Database.BeginTransactionAsync();
    }

    public async Task ConfirmacionTransaccionAsync()
    {
        if (_transaccion is not null)
        {
            await _transaccion.CommitAsync();
            await _transaccion.DisposeAsync();
            _transaccion = null;
        }
    }

    public async Task ReversionTransaccionAsync()
    {
        if (_transaccion is not null)
        {
            await _transaccion.RollbackAsync();
            await _transaccion.DisposeAsync();
            _transaccion = null;
        }
    }

    public void Dispose()
    {
        _transaccion?.Dispose();
        _contexto.Dispose();
        GC.SuppressFinalize(this);
    }
}
