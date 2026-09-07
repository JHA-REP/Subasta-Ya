using SubastaYa.Aplicacion.DTOs;

namespace SubastaYa.Aplicacion.Interfaces;

/// <summary>
/// Abstracción para notificaciones en tiempo real sobre eventos de subastas.
/// La implementación concreta (SignalR, WebSocket, etc.) reside en Infraestructura.
/// </summary>
public interface INotificadorSubastas
{
    /// <summary>
    /// Evento de subasta finalizada con ganador determinado.
    /// </summary>
    Task EventoSubastaFinalizada(SubastaFinalizadaDto datos);

    /// <summary>
    /// Evento de subasta declarada desierta (sin pujas).
    /// </summary>
    Task EventoSubastaDesierta(int subastaId);

    /// <summary>
    /// Evento de nueva puja recibida en una subasta.
    /// </summary>
    Task EventoPujaRecibida(PujaDto datos);
}
