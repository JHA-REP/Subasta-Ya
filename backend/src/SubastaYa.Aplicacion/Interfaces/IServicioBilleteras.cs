using SubastaYa.Aplicacion.DTOs;

namespace SubastaYa.Aplicacion.Interfaces;

/// <summary>
/// Contrato del servicio de billeteras.
/// </summary>
public interface IServicioBilleteras
{
    Task<BilleteraDto> DetallePorUsuarioIdAsync(int usuarioId);
    Task<IEnumerable<MovimientoContableDto>> MovimientosPorBilleteraIdAsync(int billeteraId);
}
