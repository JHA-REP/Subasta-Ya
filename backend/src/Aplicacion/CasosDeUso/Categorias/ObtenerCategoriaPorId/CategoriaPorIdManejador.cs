using Aplicacion.CasosDeUso.Categorias.ListarCategorias;
using Aplicacion.Comun.Mapeos;
using Aplicacion.Interfaces;

using Dominio.Entidades;
using Dominio.Excepciones;


namespace Aplicacion.CasosDeUso.Categorias.ObtenerCategoriaPorId;

public class CategoriaPorIdManejador : IConsultaManejador<CategoriaPorIdConsulta, CategoriaDto>
{
    private readonly IRepositorio<Categoria> _repositorioCategorias;

    public CategoriaPorIdManejador(IRepositorio<Categoria> repositorioCategorias)
    {
        _repositorioCategorias = repositorioCategorias;
    }

    public async Task<CategoriaDto> EjecucionAsync(CategoriaPorIdConsulta consulta)
    {
        var categoria = await _repositorioCategorias.PorIdAsync(consulta.Id)
            ?? throw new ExcepcionNoEncontrado(nameof(Categoria), consulta.Id);

        return categoria.MapeoDto();
    }
}
