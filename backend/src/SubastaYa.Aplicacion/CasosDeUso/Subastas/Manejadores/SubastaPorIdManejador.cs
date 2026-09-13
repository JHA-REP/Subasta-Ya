using SubastaYa.Aplicacion.CasosDeUso.Subastas.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.Manejadores;

public class SubastaPorIdManejador
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IRepositorio<Categoria> _repositorioCategorias;
    private readonly IRepositorio<Usuario> _repositorioUsuarios;

    public SubastaPorIdManejador(
        IRepositorio<Subasta> repositorioSubastas,
        IRepositorio<Categoria> repositorioCategorias,
        IRepositorio<Usuario> repositorioUsuarios)
    {
        _repositorioSubastas = repositorioSubastas;
        _repositorioCategorias = repositorioCategorias;
        _repositorioUsuarios = repositorioUsuarios;
    }

    public async Task<SubastaDto?> EjecucionAsync(SubastaPorIdConsulta consulta)
    {
        var subasta = await _repositorioSubastas.PorIdAsync(consulta.Id);
        if (subasta == null) return null;

        // carga manual de relaciones no soportadas por el repositorio generico
        subasta.Categoria = await _repositorioCategorias.PorIdAsync(subasta.CategoriaId);
        subasta.Vendedor = await _repositorioUsuarios.PorIdAsync(subasta.VendedorId);

        return subasta.MapeoDto();
    }
}
