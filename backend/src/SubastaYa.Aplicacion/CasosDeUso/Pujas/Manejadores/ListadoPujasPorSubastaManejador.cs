using SubastaYa.Aplicacion.CasosDeUso.Pujas.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Aplicacion.Mapeos;

namespace SubastaYa.Aplicacion.CasosDeUso.Pujas.Manejadores;

public class ListadoPujasPorSubastaManejador
{
    private readonly IRepositorioPujas _repositorioPujas;

    public ListadoPujasPorSubastaManejador(IRepositorioPujas repositorioPujas)
    {
        _repositorioPujas = repositorioPujas;
    }

    public async Task<IEnumerable<PujaDto>> EjecucionAsync(ListadoPujasPorSubastaConsulta consulta)
    {
        var pujas = await _repositorioPujas.ObtenerPorSubastaIdAsync(consulta.SubastaId);
        return pujas.Select(p => p.MapeoDto());
    }
}
