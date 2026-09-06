using SubastaYa.Aplicacion.CasosDeUso.Subastas.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Aplicacion.Mapeos;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.Manejadores;

public class SubastaPorIdManejador
{
    private readonly IRepositorioSubastas _repositorioSubastas;

    public SubastaPorIdManejador(IRepositorioSubastas repositorioSubastas)
    {
        _repositorioSubastas = repositorioSubastas;
    }

    public async Task<SubastaDto?> EjecucionAsync(SubastaPorIdConsulta consulta)
    {
        var subasta = await _repositorioSubastas.ObtenerPorIdAsync(consulta.Id);
        return subasta?.MapeoDto();
    }
}
