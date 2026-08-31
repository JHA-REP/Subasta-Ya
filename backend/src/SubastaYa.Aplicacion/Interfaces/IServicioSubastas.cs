using SubastaYa.Aplicacion.DTOs;

namespace SubastaYa.Aplicacion.Interfaces;

/// <summary>
/// Contrato del servicio de subastas.
/// </summary>
public interface IServicioSubastas
{
    Task<IEnumerable<SubastaDto>> ListadoAsync();
    Task<SubastaDto> DetallePorIdAsync(int id);
    Task<SubastaDto> NuevaAsync(NuevaSubastaDto dto);
}
