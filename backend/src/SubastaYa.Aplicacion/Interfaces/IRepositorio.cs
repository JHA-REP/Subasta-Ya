using System.Linq.Expressions;
using SubastaYa.Dominio.Entidades;

namespace SubastaYa.Aplicacion.Interfaces;

/// <summary>
/// Contrato genérico de repositorio para operaciones de persistencia.
/// Nombres nominales (sin verbos).
/// </summary>
public interface IRepositorio<T> where T : EntidadBase
{
    /// <summary>
    /// Listado completo de entidades.
    /// </summary>
    Task<IEnumerable<T>> TodosAsync();

    /// <summary>
    /// Entidad por su identificador.
    /// </summary>
    Task<T?> PorIdAsync(int id);

    /// <summary>
    /// Listado de entidades que cumplen un predicado.
    /// </summary>
    Task<IEnumerable<T>> FiltradasAsync(Expression<Func<T, bool>> predicado);

 
    /// Consulta paginada con filtrado, ordenamiento y carga de navegación ejecutada directamente en el origen de datos.

    Task<(IEnumerable<T> Items, int TotalItems)> ObtenerPaginadoAsync(
        Func<IQueryable<T>, IQueryable<T>>? consulta,
        int pagina,
        int tamanoPagina,
        params string[] incluirPropiedades);

    /// <summary>
    /// Alta de una nueva entidad.
    /// </summary>
    Task AltaAsync(T entidad);

    /// <summary>
    /// Modificación de una entidad existente.
    /// </summary>
    void Modificacion(T entidad);

    /// <summary>
    /// Baja de una entidad.
    /// </summary>
    void Baja(T entidad);
}
