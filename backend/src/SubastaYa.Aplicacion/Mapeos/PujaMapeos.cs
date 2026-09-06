using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Dominio.Entidades;

namespace SubastaYa.Aplicacion.Mapeos;

public static class PujaMapeos
{
    public static PujaDto MapeoDto(this Puja entidad)
    {
        return new PujaDto
        {
            Id = entidad.Id,
            SubastaId = entidad.SubastaId,
            PostorId = entidad.PostorId,
            Monto = entidad.Monto,
            FechaPuja = entidad.FechaPuja
        };
    }
}
