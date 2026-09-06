using SubastaYa.Aplicacion.CasosDeUso.Subastas.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.Manejadores;

public class ListadoSubastasManejador
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;

    public ListadoSubastasManejador(IRepositorio<Subasta> repositorioSubastas)
    {
        _repositorioSubastas = repositorioSubastas;
    }

    public async Task<IEnumerable<SubastaDto>> EjecucionAsync(ListadoSubastasConsulta consulta)
    {
        var subastas = await _repositorioSubastas.TodosAsync();
        return subastas.Select(s => s.MapeoDto());
    }
}
