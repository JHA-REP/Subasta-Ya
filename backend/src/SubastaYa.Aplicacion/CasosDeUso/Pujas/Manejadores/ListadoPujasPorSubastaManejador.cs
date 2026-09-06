using SubastaYa.Aplicacion.CasosDeUso.Pujas.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Pujas.Manejadores;

public class ListadoPujasPorSubastaManejador
{
    private readonly IRepositorio<Puja> _repositorioPujas;

    public ListadoPujasPorSubastaManejador(IRepositorio<Puja> repositorioPujas)
    {
        _repositorioPujas = repositorioPujas;
    }

    public async Task<IEnumerable<PujaDto>> EjecucionAsync(ListadoPujasPorSubastaConsulta consulta)
    {
        var pujas = await _repositorioPujas.FiltradasAsync(p => p.SubastaId == consulta.SubastaId);
        return pujas.Select(p => p.MapeoDto());
    }
}
