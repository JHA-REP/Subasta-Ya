using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Dominio.Entities;

namespace SubastaYa.Aplicacion.Mapeos;

public static class BilleteraMapeos
{
    public static BilleteraDto MapeoDto(this Billetera entidad)
    {
        return new BilleteraDto
        {
            Id = entidad.Id,
            UsuarioId = entidad.UsuarioId,
            SaldoDisponible = entidad.SaldoDisponible,
            SaldoRetenido = entidad.SaldoRetenido,
            Movimientos = entidad.Movimientos.Select(m => new MovimientoContableDto
            {
                Id = m.Id,
                BilleteraId = m.BilleteraId,
                Monto = m.Monto,
                Tipo = m.Tipo,
                FechaHora = m.FechaHora,
                ReferenciaSubastaId = m.ReferenciaSubastaId
            }).ToList()
        };
    }
}
