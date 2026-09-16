using SubastaYa.Aplicacion.Comun.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Billeteras.ObtenerBilletera;

public class BilleteraPorUsuarioManejador
{
    private readonly IRepositorio<Billetera> _repositorioBilleteras;
    private readonly IRepositorio<Usuario> _repositorioUsuarios;

    public BilleteraPorUsuarioManejador(
        IRepositorio<Billetera> repositorioBilleteras,
        IRepositorio<Usuario> repositorioUsuarios)
    {
        _repositorioBilleteras = repositorioBilleteras;
        _repositorioUsuarios = repositorioUsuarios;
    }

    public async Task<BilleteraDto?> EjecucionAsync(BilleteraPorUsuarioConsulta consulta)
    {
        var billeteras = await _repositorioBilleteras.FiltradasAsync(b => b.UsuarioId == consulta.UsuarioId);
        var billetera = billeteras.FirstOrDefault();
        if (billetera == null) return null;

        // carga manual de relacion 
        billetera.Usuario = await _repositorioUsuarios.PorIdAsync(billetera.UsuarioId);

        return billetera.MapeoDto();
    }
}
