using Dominio.Enumeraciones;
using Dominio.Excepciones;

namespace Dominio.Reglas;

/// <summary>
/// Reglas puras de validación de dominio para la finalización de subastas.
/// </summary>
public static class SubastaFinalizacionReglas
{
    /// <summary>
    /// Validación de que la subasta tenga estado Activa para poder finalizarse.
    /// </summary>
    public static void ValidacionEstadoParaFinalizacion(EstadoSubasta estado)
    {
        if (estado != EstadoSubasta.Activa)
        {
            throw new ExcepcionValidacion("Solo se pueden finalizar subastas con estado Activa.");
        }
    }

    /// <summary>
    /// Validación de que la fecha de finalización haya sido alcanzada.
    /// </summary>
    public static void ValidacionFechaVencimiento(DateTime fechaFin, DateTime fechaActual)
    {
        if (fechaActual < fechaFin)
        {
            throw new ExcepcionValidacion("La subasta aún no ha alcanzado su fecha de finalización.");
        }
    }
}
