using Aplicacion.Comun.DTOs;
using Aplicacion.Comun.Mapeos;
using Aplicacion.Interfaces;
using Dominio.Entidades;


namespace Aplicacion.CasosDeUso.Subastas.ListarSubastas;

public class ListadoSubastasManejador : IConsultaManejador<ListadoSubastasConsulta, ResultadoPaginadoDto<SubastaDto>>
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;

    public ListadoSubastasManejador(IRepositorio<Subasta> repositorioSubastas)
    {
        _repositorioSubastas = repositorioSubastas;
    }

    public async Task<ResultadoPaginadoDto<SubastaDto>> EjecucionAsync(ListadoSubastasConsulta consulta)
    {
        var (items, totalItems) = await _repositorioSubastas.ObtenerPaginadoAsync(
            query =>
            {
                if (consulta.Estado.HasValue)
                    query = query.Where(s => s.Estado == consulta.Estado.Value);

                if (consulta.CategoriaId.HasValue)
                    query = query.Where(s => s.CategoriaId == consulta.CategoriaId.Value);

                if (consulta.PrecioMin.HasValue)
                    query = query.Where(s => s.PrecioActual >= consulta.PrecioMin.Value);

                if (consulta.PrecioMax.HasValue)
                    query = query.Where(s => s.PrecioActual <= consulta.PrecioMax.Value);

                if (consulta.VendedorId.HasValue)
                    query = query.Where(s => s.VendedorId == consulta.VendedorId.Value);

                if (!string.IsNullOrWhiteSpace(consulta.TerminoBusqueda))
                {
                    var termino = consulta.TerminoBusqueda.Trim().ToLower();
                    query = query.Where(s => s.Titulo.ToLower().Contains(termino) || s.Descripcion.ToLower().Contains(termino));
                }

                return consulta.CriterioOrden switch
                {
                    "puja-desc" => query.OrderByDescending(s => s.PrecioActual).ThenBy(s => s.FechaFin),
                    "puja-asc" => query.OrderBy(s => s.PrecioActual).ThenBy(s => s.FechaFin),
                    "ofertas" => query.OrderByDescending(s => s.Pujas.Count).ThenBy(s => s.FechaFin),
                    "destacadas" => query.OrderByDescending(s => s.Pujas.Count).ThenBy(s => s.FechaFin),
                    _ => query.OrderBy(s => s.FechaFin) // "tiempo" por defecto
                };
            },
            consulta.Pagina,
            consulta.TamanoPagina,
            nameof(Subasta.Categoria),
            nameof(Subasta.Vendedor),
            nameof(Subasta.Pujas));

        return new ResultadoPaginadoDto<SubastaDto>
        {
            Items = items.Select(s => s.MapeoDto()),
            TotalItems = totalItems,
            Pagina = consulta.Pagina,
            TamanoPagina = consulta.TamanoPagina,
            TotalPaginas = (int)Math.Ceiling((double)totalItems / consulta.TamanoPagina)
        };
    }
}
