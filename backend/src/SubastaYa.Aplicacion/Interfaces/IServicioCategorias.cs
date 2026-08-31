using SubastaYa.Aplicacion.DTOs;

namespace SubastaYa.Aplicacion.Interfaces;

/// <summary>
/// Contrato del servicio de categorías.
/// </summary>
public interface IServicioCategorias
{
    Task<IEnumerable<CategoriaDto>> ListadoAsync();
    Task<CategoriaDto> DetallePorIdAsync(int id);
}
