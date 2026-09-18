using SubastaYa.Dominio.Enumeraciones;
using SubastaYa.Dominio.Excepciones;

namespace SubastaYa.Dominio.Reglas;

/// reglas puras de validación de dominio para el proceso de ofertas/pujas.

public static class PujaValidacionReglas
{
    public static void ValidacionEstadoSubasta(EstadoSubasta estadoActual)
    {
        if (estadoActual != EstadoSubasta.Activa)
        {
            throw new ExcepcionValidacion("La subasta no se encuentra activa para recibir pujas.");
        }
    }

    public static void ValidacionVentanaTemporal(DateTime fechaInicio, DateTime fechaFin, DateTime fechaActual)
    {
        if (fechaActual < fechaInicio || fechaActual > fechaFin)
        {
            throw new ExcepcionValidacion("La subasta está fuera de su ventana temporal de recepción de ofertas.");
        }
    }

    public static void ValidacionPostorDiferenteDeVendedor(int postorId, int vendedorId)
    {
        if (postorId == vendedorId)
        {
            throw new ExcepcionValidacion("El vendedor no puede realizar ofertas en su propia subasta.");
        }
    }

    public static void ValidacionMontoOferta(decimal montoOferta, decimal precioBase, decimal incrementoMinimo, decimal? montoUltimaPuja)
    {
        var montoMinimoRequerido = montoUltimaPuja.HasValue
            ? montoUltimaPuja.Value + incrementoMinimo
            : precioBase;

        if (montoOferta < montoMinimoRequerido)
        {
            throw new ExcepcionValidacion($"El monto ofertado (${montoOferta}) debe ser igual o superior al mínimo requerido de ${montoMinimoRequerido}.");
        }
    }

    public static void ValidacionSaldoDisponible(decimal saldoDisponible, decimal montoOferta)
    {
        if (saldoDisponible < montoOferta)
        {
            throw new ExcepcionValidacion($"Saldo disponible insuficiente (${saldoDisponible}). Se requiere un saldo disponible de ${montoOferta}.");
        }
    }

}
