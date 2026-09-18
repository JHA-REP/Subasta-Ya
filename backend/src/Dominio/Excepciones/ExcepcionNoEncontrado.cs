namespace Dominio.Excepciones;

/// <summary>
/// Excepción lanzada cuando una entidad no se encuentra.
/// </summary>
public class ExcepcionNoEncontrado : ExcepcionDominio
{
    public string NombreEntidad { get; }
    public object Clave { get; }

    public ExcepcionNoEncontrado(string nombreEntidad, object clave)
        : base($"La entidad \"{nombreEntidad}\" con clave ({clave}) no fue encontrada.")
    {
        NombreEntidad = nombreEntidad;
        Clave = clave;
    }
}
