using SubastaYa.Aplicacion.CasosDeUso.Billeteras.Comandos;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entities;
using SubastaYa.Dominio.Excepciones;

namespace SubastaYa.Aplicacion.CasosDeUso.Billeteras.Manejadores;

public class AcreditacionSaldoManejador
{
    private readonly IRepositorioBilleteras _repositorioBilleteras;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public AcreditacionSaldoManejador(
        IRepositorioBilleteras repositorioBilleteras,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repositorioBilleteras = repositorioBilleteras;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<BilleteraDto> EjecucionAsync(AcreditacionSaldoComando comando)
    {
        var billetera = await _repositorioBilleteras.ObtenerPorUsuarioIdAsync(comando.UsuarioId);
        if (billetera == null)
            throw new ExcepcionValidacion("La billetera del usuario no existe.");

        billetera.AcreditacionSaldo(comando.Monto);

        billetera.Movimientos.Add(new MovimientoContable
        {
            Monto = comando.Monto,
            Tipo = TipoMovimiento.Credito,
            FechaHora = DateTime.UtcNow
        });

        _repositorioBilleteras.Modificacion(billetera);
        await _unidadDeTrabajo.GuardadoCambiosAsync();

        return billetera.MapeoDto();
    }
}
