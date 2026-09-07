using Microsoft.AspNetCore.SignalR;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;

namespace SubastaYa.Infraestructura.TiempoReal;

/// <summary>
/// Implementación de INotificadorSubastas mediante SignalR.
/// Envía mensajes a los grupos de subastas y al canal global según el evento.
/// No contiene lógica de negocio — solo despacha mensajes.
/// </summary>
public class NotificadorSubastasSignalR : INotificadorSubastas
{
    private readonly IHubContext<SubastaHub> _contextoHub;

    public NotificadorSubastasSignalR(IHubContext<SubastaHub> contextoHub)
    {
        _contextoHub = contextoHub;
    }

    public async Task EventoSubastaFinalizada(SubastaFinalizadaDto datos)
    {
        var nombreGrupo = $"subasta-{datos.SubastaId}";

        // Notificar al grupo específico y al canal global (listado general)
        await _contextoHub.Clients.Group(nombreGrupo)
            .SendAsync("eventoSubastaFinalizada", datos);
        await _contextoHub.Clients.All
            .SendAsync("eventoSubastaFinalizada", datos);
    }

    public async Task EventoSubastaDesierta(int subastaId)
    {
        var nombreGrupo = $"subasta-{subastaId}";
        var carga = new { subastaId };

        await _contextoHub.Clients.Group(nombreGrupo)
            .SendAsync("eventoSubastaDesierta", carga);
        await _contextoHub.Clients.All
            .SendAsync("eventoSubastaDesierta", carga);
    }

    public async Task EventoPujaRecibida(PujaDto datos)
    {
        var nombreGrupo = $"subasta-{datos.SubastaId}";

        // Solo al grupo de la subasta — el canal global no recibe todas las pujas
        await _contextoHub.Clients.Group(nombreGrupo)
            .SendAsync("eventoPujaRecibida", datos);
    }

    public async Task EstadoTemporizador(InformacionTemporizadorDto datos)
    {
        var nombreGrupo = $"subasta-{datos.SubastaId}";

        // Solo al grupo — cada sala recibe su propio temporizador
        await _contextoHub.Clients.Group(nombreGrupo)
            .SendAsync("estadoTemporizador", datos);
    }

    public async Task EventoExtensionAntiSniping(EventoExtensionAntiSnipingDto datos)
    {
        var nombreGrupo = $"subasta-{datos.SubastaId}";

        // Al grupo de la subasta Y a todos (listado general puede mostrar badge de extensión)
        await _contextoHub.Clients.Group(nombreGrupo)
            .SendAsync("eventoExtensionAntiSniping", datos);
        await _contextoHub.Clients.All
            .SendAsync("eventoExtensionAntiSniping", datos);
    }
}
