namespace SubastaYa.Dominio.Enumeraciones;

/// <summary>
/// Tipos de acción registrados en el audit log inmutable.
/// Cada valor representa un evento de negocio auditable.
/// </summary>
public enum TipoAccionAuditoria
{
    /// <summary>Cambio de estado de una subasta (Activa → Finalizada, Activa → Desierta, etc.).</summary>
    CambioEstadoSubasta = 1,

    /// <summary>Finalización ejecutada automáticamente por el Worker.</summary>
    FinalizacionWorker = 2,

    /// <summary>Extensión de tiempo por regla Anti-Sniping.</summary>
    ExtensionAntiSniping = 3,

    /// <summary>Puja rechazada por conflicto de concurrencia (Optimistic Locking).</summary>
    PujaRechazadaConcurrencia = 4,

    /// <summary>Validación de negocio rechazada (monto insuficiente, saldo, etc.).</summary>
    ValidacionRechazada = 5,

    /// <summary>Acreditación manual de saldo en billetera.</summary>
    AcreditacionManualSaldo = 6,

    /// <summary>Puja aceptada y registrada.</summary>
    PujaAceptada = 7,

    /// <summary>Liquidación financiera completada (débito ganador + crédito vendedor).</summary>
    LiquidacionCompletada = 8,

    /// <summary>Declaración de subasta desierta (sin pujas al vencer).</summary>
    DeclaracionDesierta = 9
}
