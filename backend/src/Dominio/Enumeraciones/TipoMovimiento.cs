namespace Dominio.Enumeraciones;

/// <summary>
/// Tipos de movimiento contable en la billetera.
/// </summary>
public enum TipoMovimiento
{
    Carga = 0,
    Retencion = 1,
    Liberacion = 2,
    Debito = 3,
    Credito = 4 //<-- Acreditacion de fondos al vendedor por venta finalizada
}
