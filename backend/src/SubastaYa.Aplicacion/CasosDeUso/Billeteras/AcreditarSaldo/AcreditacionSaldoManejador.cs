using SubastaYa.Aplicacion.CasosDeUso.Billeteras.ObtenerBilletera;
using SubastaYa.Aplicacion.Comun.Mapeos;
using SubastaYa.Aplicacion.Interfaces;

using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;
using SubastaYa.Dominio.Excepciones;



namespace SubastaYa.Aplicacion.CasosDeUso.Billeteras.AcreditarSaldo;

public class AcreditacionSaldoManejador : IComandoManejador<AcreditacionSaldoComando, BilleteraDto>
{
    private readonly IRepositorio<Billetera> _repositorioBilleteras;
    private readonly IRepositorio<Usuario> _repositorioUsuarios;
    private readonly IRepositorio<MovimientoContable> _repositorioMovimientos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public AcreditacionSaldoManejador(
        IRepositorio<Billetera> repositorioBilleteras,
        IRepositorio<Usuario> repositorioUsuarios,
        IRepositorio<MovimientoContable> repositorioMovimientos,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repositorioBilleteras = repositorioBilleteras;
        _repositorioUsuarios = repositorioUsuarios;
        _repositorioMovimientos = repositorioMovimientos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<BilleteraDto> EjecucionAsync(AcreditacionSaldoComando comando)
    {
        var billeteras = await _repositorioBilleteras.FiltradasAsync(b => b.UsuarioId == comando.UsuarioId);
        var billetera = billeteras.FirstOrDefault();
        if (billetera == null)
            throw new ExcepcionValidacion("La billetera del usuario no existe.");

        billetera.AcreditacionSaldo(comando.Monto);

        var movimiento = new MovimientoContable
        {
            BilleteraId = billetera.Id,
            Monto = comando.Monto,
            Tipo = TipoMovimiento.Carga,
            FechaMovimiento = DateTime.UtcNow,
            Concepto = "Carga de saldo en cuenta"
        };

        await _repositorioMovimientos.AltaAsync(movimiento);
        _repositorioBilleteras.Modificacion(billetera);
        await _unidadDeTrabajo.ConfirmacionAsync();

        // carga manual de la relacion usuario para que el DTO tenga el alias
        billetera.Usuario = await _repositorioUsuarios.PorIdAsync(billetera.UsuarioId);

        var movimientos = await _repositorioMovimientos.FiltradasAsync(m => m.BilleteraId == billetera.Id);
        billetera.Movimientos = movimientos.OrderByDescending(m => m.FechaMovimiento).ToList();

        return billetera.MapeoDto();
    }
}
