using SubastaYa.Aplicacion.DTOs;

namespace SubastaYa.Aplicacion.Interfaces;

/// <summary>
/// Abstracción para notificaciones en tiempo real sobre eventos de subastas.
/// La implementación concreta (SignalR) reside en Infraestructura.
/// Todos los eventos se emiten DESPUÉS de confirmar la transacción de base de datos.
/// </summary>
public interface INotificadorSubastas
{
    /// <summary>
    /// Evento de subasta finalizada con ganador determinado.
    /// Se emite al grupo de la subasta y al canal global.
    /// </summary>
    Task EventoSubastaFinalizada(SubastaFinalizadaDto datos);

    /// <summary>
    /// Evento de subasta declarada desierta (sin pujas al vencer).
    /// </summary>
    Task EventoSubastaDesierta(int subastaId);

    /// <summary>
    /// Evento de nueva puja aceptada.
    /// Incluye monto actual, líder anonimizado e historial actualizado.
    /// </summary>
    Task EventoPujaRecibida(PujaDto datos);

    /// <summary>
    /// Estado actual del temporizador de una subasta.
    /// Se emite periódicamente (cada 10 s) por el BackgroundService TemporizadorSubastas.
    /// </summary>
    Task EstadoTemporizador(InformacionTemporizadorDto datos);

    /// <summary>
    /// Evento de extensión Anti-Sniping: una puja en los últimos 60 s extendió el tiempo.
    /// El frontend debe actualizar su contador con la nueva fecha de finalización.
    /// </summary>
    Task EventoExtensionAntiSniping(EventoExtensionAntiSnipingDto datos);
}
