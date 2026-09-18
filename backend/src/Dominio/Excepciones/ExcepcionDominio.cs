namespace Dominio.Excepciones;

/// <summary>
/// Excepción base del dominio.
/// </summary>
public class ExcepcionDominio : Exception
{
    public ExcepcionDominio() { }
    public ExcepcionDominio(string mensaje) : base(mensaje) { }
    public ExcepcionDominio(string mensaje, Exception interna) : base(mensaje, interna) { }
}
