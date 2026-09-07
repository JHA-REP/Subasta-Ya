using Microsoft.AspNetCore.SignalR;

namespace SubastaYa.Infraestructura.TiempoReal;

/// <summary>
/// Hub de SignalR para notificaciones en tiempo real de subastas.
/// Delgado: solo gestiona suscripción/desuscripción a grupos.
/// No contiene lógica de negocio.
/// </summary>
public class SubastaHub : Hub
{
    /// <summary>
    /// Suscripción del cliente al grupo de una subasta específica.
    /// </summary>
    public async Task SuscripcionSubasta(int subastaId)
    {
        var nombreGrupo = $"subasta-{subastaId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, nombreGrupo);
    }

    /// <summary>
    /// Desuscripción del cliente del grupo de una subasta.
    /// </summary>
    public async Task DesuscripcionSubasta(int subastaId)
    {
        var nombreGrupo = $"subasta-{subastaId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, nombreGrupo);
    }
}
