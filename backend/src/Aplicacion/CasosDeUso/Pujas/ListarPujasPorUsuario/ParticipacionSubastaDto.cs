using Dominio.Enumeraciones;

namespace Aplicacion.CasosDeUso.Pujas.ListarPujasPorUsuario;


// DTO con la información de una subasta y el detalle de la participación del usuario.


public class ParticipacionSubastaDto
{
    public int SubastaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string ImagenUrl { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string VendedorAlias { get; set; } = string.Empty;
    public EstadoSubasta Estado { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal PrecioActual { get; set; }
    public decimal MiMayorPuja { get; set; }
    public DateTime FechaUltimaPuja { get; set; }
    public int CantidadMisPujas { get; set; }
    public int CantidadPujasTotales { get; set; }
    public bool EsGanador { get; set; }
    public bool EstaLiderando { get; set; }
    public bool FueSuperado { get; set; }
    public string? GanadorAlias { get; set; }
    public decimal? MontoFinal { get; set; }
}
