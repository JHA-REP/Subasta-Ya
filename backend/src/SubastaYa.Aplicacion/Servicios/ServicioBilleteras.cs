using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.Servicios;

/// <summary>
/// Servicio de aplicación para billeteras y movimientos contables.
/// </summary>
public class ServicioBilleteras : IServicioBilleteras
{
    private readonly IRepositorio<Billetera> _repositorioBilleteras;
    private readonly IRepositorio<MovimientoContable> _repositorioMovimientos;

    public ServicioBilleteras(
        IRepositorio<Billetera> repositorioBilleteras,
        IRepositorio<MovimientoContable> repositorioMovimientos)
    {
        _repositorioBilleteras = repositorioBilleteras;
        _repositorioMovimientos = repositorioMovimientos;
    }

    public async Task<BilleteraDto> DetallePorUsuarioIdAsync(int usuarioId)
    {
        var billeteras = await _repositorioBilleteras.FiltradasAsync(b => b.UsuarioId == usuarioId);
        var billetera = billeteras.FirstOrDefault()
            ?? throw new ExcepcionNoEncontrado(nameof(Billetera), usuarioId);

        return new BilleteraDto
        {
            Id = billetera.Id,
            UsuarioId = billetera.UsuarioId,
            UsuarioAlias = billetera.Usuario?.Alias ?? string.Empty,
            Saldo = billetera.Saldo,
            SaldoRetenido = billetera.SaldoRetenido
        };
    }

    public async Task<IEnumerable<MovimientoContableDto>> MovimientosPorBilleteraIdAsync(int billeteraId)
    {
        var movimientos = await _repositorioMovimientos.FiltradasAsync(m => m.BilleteraId == billeteraId);

        return movimientos.Select(m => new MovimientoContableDto
        {
            Id = m.Id,
            BilleteraId = m.BilleteraId,
            Tipo = m.Tipo,
            Monto = m.Monto,
            Concepto = m.Concepto,
            FechaMovimiento = m.FechaMovimiento
        });
    }
}
