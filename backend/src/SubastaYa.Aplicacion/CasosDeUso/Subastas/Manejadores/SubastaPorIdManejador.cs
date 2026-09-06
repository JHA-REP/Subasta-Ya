using SubastaYa.Aplicacion.CasosDeUso.Subastas.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.Manejadores;

public class SubastaPorIdManejador
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;

    public SubastaPorIdManejador(IRepositorio<Subasta> repositorioSubastas)
    {
        _repositorioSubastas = repositorioSubastas;
    }

    public async Task<SubastaDto?> EjecucionAsync(SubastaPorIdConsulta consulta)
    {
        var subasta = await _repositorioSubastas.PorIdAsync(consulta.Id);
        return subasta?.MapeoDto();
    }
}
