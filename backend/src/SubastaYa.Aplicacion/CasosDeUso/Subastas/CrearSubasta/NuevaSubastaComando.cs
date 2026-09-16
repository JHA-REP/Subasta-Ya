using System.ComponentModel.DataAnnotations;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.CrearSubasta;

public class NuevaSubastaComando
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
    public string Titulo { get; set; } = null!;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; } = null!;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio inicial debe ser mayor a cero.")]
    public decimal PrecioInicial { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El incremento mínimo debe ser mayor a cero.")]
    public decimal IncrementoMinimo { get; set; }

    [Required(ErrorMessage = "La URL de la imagen es obligatoria.")]
    public string ImagenUrl { get; set; } = null!;

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; }

    [Required]
    public int VendedorId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe especificar una categoría válida.")]
    public int CategoriaId { get; set; } = 1;
}
