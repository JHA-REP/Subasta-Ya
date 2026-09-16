using SubastaYa.Aplicacion.Comun.DTOs;
using SubastaYa.Aplicacion.Comun.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.ObtenerSubastaPorId;

public class SubastaPorIdManejador
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IRepositorio<Categoria> _repositorioCategorias;
    private readonly IRepositorio<Usuario> _repositorioUsuarios;

    private readonly IRepositorio<Puja> _repositorioPujas;

    public SubastaPorIdManejador(
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

    public async Task<SubastaDto?> EjecucionAsync(SubastaPorIdConsulta consulta)
    {
        var subasta = await _repositorioSubastas.PorIdAsync(consulta.Id);
        if (subasta == null) return null;

        // carga manual de relaciones no soportadas por el repositorio generico
        subasta.Categoria = await _repositorioCategorias.PorIdAsync(subasta.CategoriaId);
        subasta.Vendedor = await _repositorioUsuarios.PorIdAsync(subasta.VendedorId);
        var pujas = await _repositorioPujas.FiltradasAsync(p => p.SubastaId == subasta.Id);
        subasta.Pujas = pujas.ToList();


        return subasta.MapeoDto();
    }
}
