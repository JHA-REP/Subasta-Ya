using SubastaYa.Dominio.Excepciones;

namespace SubastaYa.Dominio.Entities;

public class Subasta
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal PrecioInicial { get; set; }
    public decimal PrecioActual { get; set; }
    public decimal IncrementoMinimo { get; set; }
    public string ImagenUrl { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int VendedorId { get; set; }
    public bool Activa { get; set; } = true;
    public byte[] RowVersion { get; set; } = null!;

    public void ExtensionTiempoAntiSniping(DateTime fechaHoraActual)
    {
        var tiempoRestante = FechaFin - fechaHoraActual;
        if (tiempoRestante <= TimeSpan.FromSeconds(60))
        {
            FechaFin = FechaFin.AddMinutes(2);
        }
    }

    public void ActualizacionPrecioActual(decimal nuevoMonto)
    {
        PrecioActual = nuevoMonto;
    }
}
