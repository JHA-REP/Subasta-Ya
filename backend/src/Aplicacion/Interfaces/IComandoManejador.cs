namespace Aplicacion.Interfaces;

/// <summary>
/// Contrato genérico para manejadores de comandos (operaciones de escritura).
/// </summary>
public interface IComandoManejador<TComando, TResultado>
{
    Task<TResultado> EjecucionAsync(TComando comando);
}
