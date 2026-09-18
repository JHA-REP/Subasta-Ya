using SubastaYa.Dominio.Excepciones;

namespace SubastaYa.Dominio.Entidades;

public class Billetera : EntidadBase
{
    public int UsuarioId { get; set; }
    public decimal SaldoDisponible { get; set; }
    public decimal SaldoRetenido { get; set; }
    public byte[] RowVersion { get; set; } = [];

    // Navegación
    public Usuario? Usuario { get; set; }
    public List<MovimientoContable> Movimientos { get; set; } = new();

    public void AcreditacionSaldo(decimal monto)
    {
        if (monto <= 0)
            throw new ExcepcionValidacion("El monto a acreditar debe ser mayor a cero.");

        SaldoDisponible += monto;
    }

    public void RetencionSaldo(decimal monto)
    {
        if (monto <= 0)
            throw new ExcepcionValidacion("El monto a retener debe ser mayor a cero.");

        if (SaldoDisponible < monto)
            throw new ExcepcionValidacion("Saldo disponible insuficiente para realizar la puja.");

        SaldoDisponible -= monto;
        SaldoRetenido += monto;
    }

    public void LiberacionSaldo(decimal monto)
    {
        if (monto <= 0)
            throw new ExcepcionValidacion("El monto a liberar debe ser mayor a cero.");

        if (SaldoRetenido < monto)
            throw new ExcepcionValidacion("No hay saldo retenido suficiente para liberar.");

        SaldoRetenido -= monto;
        SaldoDisponible += monto;
    }

    /// <summary>
    /// Confirmación de retención: débito efectivo del saldo retenido (no retorna a disponible).
    /// Se utiliza al liquidar una subasta ganada.
    /// </summary>
    public void ConfirmacionRetencion(decimal monto)
    {
        if (monto <= 0)
            throw new ExcepcionValidacion("El monto a confirmar debe ser mayor a cero.");

        if (SaldoRetenido < monto)
            throw new ExcepcionValidacion("No hay saldo retenido suficiente para confirmar el débito.");

        SaldoRetenido -= monto;
    }
}
