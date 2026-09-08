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

    public async Task<ResultadoPaginadoDto<SubastaDto>> EjecucionAsync(ListadoSubastasConsulta consulta)
    {
        var todasLasSubastas = await _repositorioSubastas.TodosAsync();

        // aplicar filtros
        var subastasFiltradas = todasLasSubastas.AsQueryable();

        if (consulta.Estado.HasValue)
            subastasFiltradas = subastasFiltradas.Where(s => s.Estado == consulta.Estado.Value);

        if (consulta.CategoriaId.HasValue)
            subastasFiltradas = subastasFiltradas.Where(s => s.CategoriaId == consulta.CategoriaId.Value);

        if (consulta.PrecioMin.HasValue)
            subastasFiltradas = subastasFiltradas.Where(s => s.PrecioActual >= consulta.PrecioMin.Value);

        if (consulta.PrecioMax.HasValue)
            subastasFiltradas = subastasFiltradas.Where(s => s.PrecioActual <= consulta.PrecioMax.Value);

        // contar total antes de paginar
        var totalItems = subastasFiltradas.Count();

        // paginar
        var items = subastasFiltradas
            .Skip((consulta.Pagina - 1) * consulta.TamanoPagina)
            .Take(consulta.TamanoPagina)
            .Select(s => s.MapeoDto());

        return new ResultadoPaginadoDto<SubastaDto>
        {
            Items = items,
            TotalItems = totalItems,
            Pagina = consulta.Pagina,
            TamanoPagina = consulta.TamanoPagina,
            TotalPaginas = (int)Math.Ceiling((double)totalItems / consulta.TamanoPagina)
        };
    }
}
