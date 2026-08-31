namespace SubastaYa.Dominio.Excepciones;

/// <summary>
/// Excepción lanzada cuando falla una validación de negocio.
/// </summary>
public class ExcepcionValidacion : ExcepcionDominio
{
    public IDictionary<string, string[]> Errores { get; }

    public ExcepcionValidacion(IDictionary<string, string[]> errores)
        : base("Se produjeron uno o más errores de validación.")
    {
        Errores = errores;
    }

    public ExcepcionValidacion(string campo, string mensaje)
        : base(mensaje)
    {
        Errores = new Dictionary<string, string[]>
        {
            { campo, new[] { mensaje } }
        };
    }
}
