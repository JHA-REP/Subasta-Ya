using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Dominio.Entities;

namespace SubastaYa.Aplicacion.Mapeos;

public static class SubastaMapeos
{
    public static SubastaDto MapeoDto(this Subasta entidad)
    {
        return new SubastaDto
        {
            Id = entidad.Id,
            Titulo = entidad.Titulo,
            Descripcion = entidad.Descripcion,
            PrecioInicial = entidad.PrecioInicial,
            PrecioActual = entidad.PrecioActual,
            IncrementoMinimo = entidad.IncrementoMinimo,
            ImagenUrl = entidad.ImagenUrl,
            FechaInicio = entidad.FechaInicio,
            FechaFin = entidad.FechaFin,
            VendedorId = entidad.VendedorId,
            Activa = entidad.Activa
        };
    }
}
