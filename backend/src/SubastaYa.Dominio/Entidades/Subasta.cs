using SubastaYa.Dominio.Excepciones;
using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Dominio.Entidades;

public class Subasta : EntidadBase
{
    public string Titulo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal PrecioInicial { get; set; }
    public decimal PrecioActual { get; set; }
    public decimal IncrementoMinimo { get; set; }
    public string ImagenUrl { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int VendedorId { get; set; }
    public int CategoriaId { get; set; }
    public EstadoSubasta Estado { get; set; } = EstadoSubasta.Activa;
    public bool Activa => Estado == EstadoSubasta.Activa;
    public byte[] RowVersion { get; set; } = null!;

    /// <summary>
    /// Identificador del postor ganador (null si no finalizó o es desierta).
    /// </summary>
    public int? GanadorId { get; set; }

    /// <summary>
    /// Monto final de adjudicación (null si no finalizó o es desierta).
    /// </summary>
    public decimal? MontoFinal { get; set; }

    // Navegación
    public Categoria? Categoria { get; set; }
    public Usuario? Vendedor { get; set; }
    public Usuario? Ganador { get; set; }
    public ICollection<Puja> Pujas { get; set; } = [];

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

    /// <summary>
    /// Resultado de finalización con ganador: cambia estado y registra adjudicación.
    /// </summary>
    public void ResultadoFinalizacion(int ganadorId, decimal montoFinal)
    {
        Estado = EstadoSubasta.Finalizada;
        GanadorId = ganadorId;
        MontoFinal = montoFinal;
    }

    /// <summary>
    /// Resultado desierta: cambia estado cuando no hubo pujas.
    /// </summary>
    public void ResultadoDesierta()
    {
        Estado = EstadoSubasta.Desierta;
    }
}
