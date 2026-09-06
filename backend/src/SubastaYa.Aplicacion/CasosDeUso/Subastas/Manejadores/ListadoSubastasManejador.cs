using SubastaYa.Aplicacion.CasosDeUso.Subastas.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Aplicacion.Mapeos;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.Manejadores;

public class ListadoSubastasManejador
{
    private readonly IRepositorioSubastas _repositorioSubastas;

    public ListadoSubastasManejador(IRepositorioSubastas repositorioSubastas)
    {
        _repositorioSubastas = repositorioSubastas;
    }

    public async Task<IEnumerable<SubastaDto>> EjecucionAsync(ListadoSubastasConsulta consulta)
    {
        var subastas = await _repositorioSubastas.ObtenerTodasAsync();
        return subastas.Select(s => s.MapeoDto());
    }
}
