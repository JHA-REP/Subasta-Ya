namespace Dominio.Excepciones;

/// <summary>
/// Excepción lanzada cuando se detecta un conflicto de concurrencia (Optimistic Locking).
/// </summary>
public class ExcepcionConcurrencia : ExcepcionDominio
{
    public ExcepcionConcurrencia()
        : base("El registro fue modificado por otro proceso. Reintente la operación.") { }

    public ExcepcionConcurrencia(string mensaje) : base(mensaje) { }

    public ExcepcionConcurrencia(string mensaje, Exception interna) : base(mensaje, interna) { }
}
