using Aplicacion.Comun.Mapeos;
using Aplicacion.Interfaces;
using Dominio.Entidades;



namespace Aplicacion.CasosDeUso.Billeteras.ObtenerBilletera;

public class BilleteraPorUsuarioManejador : IConsultaManejador<BilleteraPorUsuarioConsulta, BilleteraDto?>
{
    private readonly IRepositorio<Billetera> _repositorioBilleteras;
    private readonly IRepositorio<Usuario> _repositorioUsuarios;
    private readonly IRepositorio<MovimientoContable> _repositorioMovimientos;

    public BilleteraPorUsuarioManejador(
        IRepositorio<Billetera> repositorioBilleteras,
        IRepositorio<Usuario> repositorioUsuarios,
        IRepositorio<MovimientoContable> repositorioMovimientos)
    {
        _repositorioBilleteras = repositorioBilleteras;
        _repositorioUsuarios = repositorioUsuarios;
        _repositorioMovimientos = repositorioMovimientos;
    }

    public async Task<BilleteraDto?> EjecucionAsync(BilleteraPorUsuarioConsulta consulta)
    {
        var billeteras = await _repositorioBilleteras.FiltradasAsync(b => b.UsuarioId == consulta.UsuarioId);
        var billetera = billeteras.FirstOrDefault();
        if (billetera == null) return null;

        // carga manual de relacion 
        billetera.Usuario = await _repositorioUsuarios.PorIdAsync(billetera.UsuarioId);

        // carga de historial de movimientos contables
        var movimientos = await _repositorioMovimientos.FiltradasAsync(m => m.BilleteraId == billetera.Id);
        billetera.Movimientos = movimientos.OrderByDescending(m => m.FechaMovimiento).ToList();

        return billetera.MapeoDto();
    }
}
