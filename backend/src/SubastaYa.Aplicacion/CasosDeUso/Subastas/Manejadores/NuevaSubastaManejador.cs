using SubastaYa.Aplicacion.CasosDeUso.Subastas.Comandos;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.Manejadores;

public class NuevaSubastaManejador
{
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public NuevaSubastaManejador(
        IRepositorio<Subasta> repositorioSubastas,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repositorioSubastas = repositorioSubastas;
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
            Estado = SubastaYa.Dominio.Enumeraciones.EstadoSubasta.Activa
        };

        await _repositorioSubastas.AltaAsync(subasta);
        await _unidadDeTrabajo.ConfirmacionAsync();

        return subasta.MapeoDto();
    }
}

