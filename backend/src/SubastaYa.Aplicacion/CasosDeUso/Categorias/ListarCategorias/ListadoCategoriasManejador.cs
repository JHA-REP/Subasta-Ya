using SubastaYa.Aplicacion.Comun.Mapeos;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;


namespace SubastaYa.Aplicacion.CasosDeUso.Categorias.ListarCategorias;

public class ListadoCategoriasManejador : IConsultaManejador<ListadoCategoriasConsulta, IEnumerable<CategoriaDto>>
{
    private readonly IRepositorio<Categoria> _repositorioCategorias;

    public ListadoCategoriasManejador(IRepositorio<Categoria> repositorioCategorias)
    {
        _repositorioCategorias = repositorioCategorias;
    }

    public async Task<IEnumerable<CategoriaDto>> EjecucionAsync(ListadoCategoriasConsulta consulta)
    {
        var categorias = await _repositorioCategorias.TodosAsync();
        return categorias.Select(c => c.MapeoDto());
    }
}
