using SubastaYa.Aplicacion.Comun.DTOs;
using SubastaYa.Aplicacion.Comun.Mapeos;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;


namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.CrearSubasta;

public class NuevaSubastaManejador : IComandoManejador<NuevaSubastaComando, SubastaDto>
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IRepositorio<Categoria> _repositorioCategorias;
    private readonly IRepositorio<Usuario> _repositorioUsuarios;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public NuevaSubastaManejador(
        IRepositorio<Subasta> repositorioSubastas,
        IRepositorio<Categoria> repositorioCategorias,
        IRepositorio<Usuario> repositorioUsuarios,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repositorioSubastas = repositorioSubastas;
        _repositorioCategorias = repositorioCategorias;
        _repositorioUsuarios = repositorioUsuarios;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<SubastaDto> EjecucionAsync(NuevaSubastaComando comando)
    {
        var subasta = new Subasta
        {
            Titulo = comando.Titulo,
            Descripcion = comando.Descripcion,
            PrecioInicial = comando.PrecioInicial,
            PrecioActual = comando.PrecioInicial,
            IncrementoMinimo = comando.IncrementoMinimo,
            ImagenUrl = comando.ImagenUrl,
            FechaInicio = comando.FechaInicio,
            FechaFin = comando.FechaFin,
            VendedorId = comando.VendedorId,
            CategoriaId = comando.CategoriaId > 0 ? comando.CategoriaId : 1,
            Estado = SubastaYa.Dominio.Enumeraciones.EstadoSubasta.Activa
        };

        await _repositorioSubastas.AltaAsync(subasta);
        await _unidadDeTrabajo.ConfirmacionAsync();

        // carga manual de relaciones para que el DTO devuelva categoria y vendedor
        subasta.Categoria = await _repositorioCategorias.PorIdAsync(subasta.CategoriaId);
        subasta.Vendedor = await _repositorioUsuarios.PorIdAsync(subasta.VendedorId);

        return subasta.MapeoDto();
    }
}

