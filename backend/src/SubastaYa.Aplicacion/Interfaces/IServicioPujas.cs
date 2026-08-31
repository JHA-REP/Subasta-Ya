using SubastaYa.Aplicacion.DTOs;

namespace SubastaYa.Aplicacion.Interfaces;

/// <summary>
/// Contrato del servicio de pujas.
/// </summary>
public interface IServicioPujas
{
    Task<IEnumerable<PujaDto>> ListadoPorSubastaAsync(int subastaId);
    Task<PujaDto> NuevaAsync(NuevaPujaDto dto);
}
