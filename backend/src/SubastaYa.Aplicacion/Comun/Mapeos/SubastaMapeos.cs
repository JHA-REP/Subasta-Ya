using SubastaYa.Aplicacion.Comun.DTOs;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Aplicacion.Comun.Mapeos;

public static class SubastaMapeos
{
    public static SubastaDto MapeoDto(this Subasta entidad)
    {
        var liderActual = entidad.Pujas?.OrderByDescending(p => p.Monto).FirstOrDefault();
        bool esFinalizada = entidad.Estado == EstadoSubasta.Finalizada || entidad.Estado == EstadoSubasta.Desierta;

        return new SubastaDto
        {
            Id = entidad.Id,
            Titulo = entidad.Titulo,
            Descripcion = entidad.Descripcion,
            PrecioBase = entidad.PrecioInicial,
            IncrementoMinimo = entidad.IncrementoMinimo,
            ImagenUrl = entidad.ImagenUrl,
            CategoriaId = entidad.CategoriaId,
            Categoria = entidad.Categoria?.Nombre ?? string.Empty,
            Vendedor = entidad.Vendedor?.Alias ?? string.Empty,
            FechaInicio = entidad.FechaInicio,
            FechaFin = entidad.FechaFin,
            VendedorId = entidad.VendedorId,
            Estado = entidad.Estado,
            CantidadPujas = entidad.Pujas?.Count ?? 0,
            MontoMayorPuja = entidad.PrecioActual != entidad.PrecioInicial ? entidad.PrecioActual : null,
            GanadorId = esFinalizada ? entidad.GanadorId : liderActual?.PostorId,
            GanadorAlias = esFinalizada ? entidad.Ganador?.Alias : liderActual?.Postor?.Alias ?? string.Empty,
            MontoFinal = entidad.MontoFinal
        };
    }
}
