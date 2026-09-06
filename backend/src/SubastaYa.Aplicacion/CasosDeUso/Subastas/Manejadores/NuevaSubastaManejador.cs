using SubastaYa.Aplicacion.CasosDeUso.Subastas.Comandos;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entities;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.Manejadores;

public class NuevaSubastaManejador
{
    private readonly IRepositorioSubastas _repositorioSubastas;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public NuevaSubastaManejador(
        IRepositorioSubastas repositorioSubastas,
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
            Activa = true
        };

        await _repositorioSubastas.AltaAsync(subasta);
        await _unidadDeTrabajo.GuardadoCambiosAsync();

        return subasta.MapeoDto();
    }
}
