using SubastaYa.Aplicacion.CasosDeUso.Categorias.ListarCategorias;
using SubastaYa.Aplicacion.Comun.Mapeos;
using SubastaYa.Aplicacion.Interfaces;

using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Categorias.ObtenerCategoriaPorId;

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
