using Microsoft.AspNetCore.SignalR;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;

namespace SubastaYa.Infraestructura.TiempoReal;

/// <summary>
/// Implementación de INotificadorSubastas mediante SignalR.
/// Envía mensajes a los grupos de subastas correspondientes.
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
        await _contextoHub.Clients.Group(nombreGrupo)
            .SendAsync("eventoSubastaFinalizada", datos);

        // También notificar a todos los clientes conectados (listado general)
        await _contextoHub.Clients.All
            .SendAsync("eventoSubastaFinalizada", datos);
    }

    public async Task EventoSubastaDesierta(int subastaId)
    {
        var nombreGrupo = $"subasta-{subastaId}";
        await _contextoHub.Clients.Group(nombreGrupo)
            .SendAsync("eventoSubastaDesierta", new { subastaId });

        await _contextoHub.Clients.All
            .SendAsync("eventoSubastaDesierta", new { subastaId });
    }

    public async Task EventoPujaRecibida(PujaDto datos)
    {
        var nombreGrupo = $"subasta-{datos.SubastaId}";
        await _contextoHub.Clients.Group(nombreGrupo)
            .SendAsync("eventoPujaRecibida", datos);
    }
}
