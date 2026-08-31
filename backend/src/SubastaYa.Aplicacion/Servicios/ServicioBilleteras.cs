using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;
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
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public ServicioBilleteras(
        IRepositorio<Billetera> repositorioBilleteras,
        IRepositorio<MovimientoContable> repositorioMovimientos,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repositorioBilleteras = repositorioBilleteras;
        _repositorioMovimientos = repositorioMovimientos;
        _unidadDeTrabajo = unidadDeTrabajo;
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

        return movimientos.OrderByDescending(m => m.FechaMovimiento).Select(m => new MovimientoContableDto
        {
            Id = m.Id,
            BilleteraId = m.BilleteraId,
            Tipo = m.Tipo,
            Monto = m.Monto,
            Concepto = m.Concepto,
            FechaMovimiento = m.FechaMovimiento
        });
    }
    //
    public async Task<BilleteraDto> AcreditacionSaldoSimuladaAsync(int usuarioId, decimal monto)
    {
        if (monto <= 0)
            throw new ExcepcionValidacion("El monto a acreditar debe ser mayor a cero.");
        var billeteras = await _repositorioBilleteras.FiltradasAsync(b => b.UsuarioId == usuarioId);
        var billetera = billeteras.FirstOrDefault()
            ?? throw new ExcepcionNoEncontrado(nameof(Billetera), usuarioId);
        // acreditacion de saldo
        billetera.Saldo += monto;
        _repositorioBilleteras.Modificacion(billetera);
        // registro en el ledger
        await _repositorioMovimientos.AltaAsync(new MovimientoContable
        {
            BilleteraId = billetera.Id,
            Tipo = TipoMovimiento.Carga,
            Monto = monto,
            Concepto = "Carga simulada de saldo",
            FechaMovimiento = DateTime.UtcNow
        });
        await _unidadDeTrabajo.ConfirmacionAsync();
        return new BilleteraDto
        {
            Id = billetera.Id,
            UsuarioId = billetera.UsuarioId,
            UsuarioAlias = billetera.Usuario?.Alias ?? string.Empty,
            Saldo = billetera.Saldo,
            SaldoRetenido = billetera.SaldoRetenido
        };
    }
}
