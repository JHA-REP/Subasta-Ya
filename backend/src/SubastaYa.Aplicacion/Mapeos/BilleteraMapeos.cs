using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Dominio.Entidades;

namespace SubastaYa.Aplicacion.Mapeos;

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
            SaldoRetenido = entidad.SaldoRetenido
        };
    }
}
