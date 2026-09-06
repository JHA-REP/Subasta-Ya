using SubastaYa.Dominio.Excepciones;

namespace SubastaYa.Dominio.Entities;

public class Billetera
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public decimal SaldoDisponible { get; private set; }
    public decimal SaldoRetenido { get; private set; }

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
}
