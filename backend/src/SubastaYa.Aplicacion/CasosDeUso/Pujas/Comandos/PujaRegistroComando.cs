using System.ComponentModel.DataAnnotations;

namespace SubastaYa.Aplicacion.CasosDeUso.Pujas.Comandos;

public class PujaRegistroComando
{
    [Required]
    public int SubastaId { get; set; }

    [Required]
    public int PostorId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto de la puja debe ser mayor a cero.")]
    public decimal Monto { get; set; }
}
