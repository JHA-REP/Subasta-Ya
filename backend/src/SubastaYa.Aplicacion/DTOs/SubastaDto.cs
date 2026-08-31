using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Aplicacion.DTOs;

public class SubastaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal PrecioBase { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string Vendedor { get; set; } = string.Empty;
    public int VendedorId { get; set; }
    public EstadoSubasta Estado { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int CantidadPujas { get; set; }
    public decimal? MontoMayorPuja { get; set; }
}

public class NuevaSubastaDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal PrecioBase { get; set; }
    public int CategoriaId { get; set; }
    public int VendedorId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
}
