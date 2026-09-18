using SubastaYa.Aplicacion.CasosDeUso.Billeteras.ObtenerBilletera;
using SubastaYa.Dominio.Entidades;

namespace SubastaYa.Aplicacion.Comun.Mapeos;

public static class BilleteraMapeos
{
    public static BilleteraDto MapeoDto(this Billetera entidad)
    {
        return new BilleteraDto
        {
            Id = entidad.Id,
            UsuarioId = entidad.UsuarioId,
            UsuarioAlias = entidad.Usuario?.Alias ?? string.Empty,
            Saldo = entidad.SaldoDisponible + entidad.SaldoRetenido,
            SaldoRetenido = entidad.SaldoRetenido,
            Movimientos = entidad.Movimientos.Select(m => new MovimientoContableDto
            {
                Id = m.Id,
                BilleteraId = m.BilleteraId,
                Tipo = m.Tipo,
                Monto = m.Monto,
                Concepto = m.Concepto,
                FechaMovimiento = m.FechaMovimiento
            }).ToList()
        };
    }
}
