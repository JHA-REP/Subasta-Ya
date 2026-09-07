using SubastaYa.Aplicacion.CasosDeUso.Categorias.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Categorias.Manejadores;

public class ListadoCategoriasManejador
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
