using System.ComponentModel.DataAnnotations;

//uso dataannotations para validacion de entrada

namespace SubastaYa.Aplicacion.CasosDeUso.Billeteras.Comandos;

public class AcreditacionSaldoComando
{
    [Required]
    public int UsuarioId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto a acreditar debe ser mayor a cero.")]
    public decimal Monto { get; set; }
}
