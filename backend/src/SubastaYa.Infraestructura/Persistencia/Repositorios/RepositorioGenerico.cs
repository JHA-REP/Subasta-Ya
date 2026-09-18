using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SubastaYa.Dominio.Entidades;

using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Infraestructura.Persistencia;

namespace SubastaYa.Infraestructura.Repositorios;

/// <summary>
/// Implementación genérica del repositorio usando EF Core.
/// </summary>
public class RepositorioGenerico<T> : IRepositorio<T> where T : EntidadBase
{
    protected readonly SubastaYaDbContext _contexto;
    protected readonly DbSet<T> _conjunto;

    public RepositorioGenerico(SubastaYaDbContext contexto)
    {
        _contexto = contexto;
        _conjunto = contexto.Set<T>();
    }

    public async Task<IEnumerable<T>> TodosAsync()
    {
        return await _conjunto.ToListAsync();
    }

    public async Task<T?> PorIdAsync(int id)
    {
        return await _conjunto.FindAsync(id);
    }

    public async Task<IEnumerable<T>> FiltradasAsync(Expression<Func<T, bool>> predicado)
    {
        return await _conjunto.Where(predicado).ToListAsync();
    }

    public async Task<(IEnumerable<T> Items, int TotalItems)> ObtenerPaginadoAsync(
        Func<IQueryable<T>, IQueryable<T>>? consulta,
        int pagina,
        int tamanoPagina,
        params string[] incluirPropiedades)
    {
        IQueryable<T> query = _conjunto.AsNoTracking();

        foreach (var include in incluirPropiedades)
        {
            query = query.Include(include);
        }

        if (consulta != null)
        {
            query = consulta(query);
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return (items, totalItems);
    }

    public async Task AltaAsync(T entidad)
    {
        await _conjunto.AddAsync(entidad);
    }

    public void Modificacion(T entidad)
    {
        _conjunto.Update(entidad);
    }

    public void Baja(T entidad)
    {
        _conjunto.Remove(entidad);
    }
}
