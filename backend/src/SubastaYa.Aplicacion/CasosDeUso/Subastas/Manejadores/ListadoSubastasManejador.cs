using SubastaYa.Aplicacion.CasosDeUso.Subastas.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.Manejadores;

public class ListadoSubastasManejador
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IRepositorio<Categoria> _repositorioCategorias;
    private readonly IRepositorio<Usuario> _repositorioUsuarios;
    private readonly IRepositorio<Puja> _repositorioPujas;
    public ListadoSubastasManejador(
        IRepositorio<Subasta> repositorioSubastas,
        IRepositorio<Categoria> repositorioCategorias,
        IRepositorio<Usuario> repositorioUsuarios,
        IRepositorio<Puja> repositorioPujas)
    {
        _repositorioSubastas = repositorioSubastas;
        _repositorioCategorias = repositorioCategorias;
        _repositorioUsuarios = repositorioUsuarios;
        _repositorioPujas = repositorioPujas;
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

        // paginar y materializar la lista
        var listaFiltrada = subastasFiltradas
            .Skip((consulta.Pagina - 1) * consulta.TamanoPagina)
            .Take(consulta.TamanoPagina)
            .ToList();

        // carga manual de relaciones - una sola query por tipo, no N queries
        var categorias = await _repositorioCategorias.TodosAsync();
        var usuarios = await _repositorioUsuarios.TodosAsync();
        var subastasIds = listaFiltrada.Select(s => s.Id).ToList();
        var pujas = await _repositorioPujas.FiltradasAsync(p => subastasIds.Contains(p.SubastaId));

        foreach (var s in listaFiltrada)
        {
            s.Categoria = categorias.FirstOrDefault(c => c.Id == s.CategoriaId);
            s.Vendedor = usuarios.FirstOrDefault(u => u.Id == s.VendedorId);
            s.Pujas = pujas.Where(p => p.SubastaId == s.Id).ToList();
        }

        return new ResultadoPaginadoDto<SubastaDto>
        {
            Items = listaFiltrada.Select(s => s.MapeoDto()),
            TotalItems = totalItems,
            Pagina = consulta.Pagina,
            TamanoPagina = consulta.TamanoPagina,
            TotalPaginas = (int)Math.Ceiling((double)totalItems / consulta.TamanoPagina)
        };
    }
}
