namespace SubastaYa.Dominio.Interfaces;

/// <summary>
/// Contrato de Unidad de Trabajo para coordinar transacciones.
/// Preparado para transacciones ACID.
/// </summary>
public interface IUnidadDeTrabajo : IDisposable
{
    /// <summary>
    /// Confirmación de todos los cambios pendientes en la base de datos.
    /// </summary>
    Task<int> ConfirmacionAsync();

    /// <summary>
    /// Inicio de una transacción explícita (preparación para ACID).
    /// </summary>
    Task InicioTransaccionAsync();

    /// <summary>
    /// Confirmación de la transacción activa.
    /// </summary>
    Task ConfirmacionTransaccionAsync();

    /// <summary>
    /// Reversión de la transacción activa.
    /// </summary>
    Task ReversionTransaccionAsync();
}
