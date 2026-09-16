namespace SubastaYa.Aplicacion.Interfaces;

/// <summary>
/// Contrato genérico para manejadores de consultas (operaciones de lectura).
/// </summary>
public interface IConsultaManejador<TConsulta, TResultado>
{
    Task<TResultado> EjecucionAsync(TConsulta consulta);
}
