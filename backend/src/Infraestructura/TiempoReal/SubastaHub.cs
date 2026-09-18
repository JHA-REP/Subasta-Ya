using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Infraestructura.TiempoReal;

/// <summary>
/// Hub de SignalR para la sala de subastas en tiempo real.
/// Delgado: gestiona suscripción/desuscripción a grupos y ciclo de vida de conexiones.
/// Sin lógica de negocio.
/// </summary>
public class SubastaHub : Hub
{
    private readonly ILogger<SubastaHub> _registro;

    public SubastaHub(ILogger<SubastaHub> registro)
    {
        _registro = registro;
    }

    /// <summary>
    /// Suscripción del cliente al canal de una subasta específica.
    /// Tras suscribirse, el cliente recibirá todos los eventos de esa subasta.
    /// </summary>
    public async Task SuscripcionSubasta(int subastaId)
    {
        var nombreGrupo = $"subasta-{subastaId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, nombreGrupo);

        _registro.LogInformation(
            "Cliente {ConexionId} suscripto al grupo {Grupo}.",
            Context.ConnectionId, nombreGrupo);
    }

    /// <summary>
    /// Desuscripción del cliente del canal de una subasta.
    /// </summary>
    public async Task DesuscripcionSubasta(int subastaId)
    {
        var nombreGrupo = $"subasta-{subastaId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, nombreGrupo);

        _registro.LogInformation(
            "Cliente {ConexionId} desuscripto del grupo {Grupo}.",
            Context.ConnectionId, nombreGrupo);
    }

    /// <summary>
    /// Evento de desconexión.
    /// SignalR limpia los grupos automáticamente al desconectar.
    /// Se registra en el log para diagnóstico.
    /// Si el cliente se reconecta, debe volver a llamar a SuscripcionSubasta.
    /// </summary>
    public override Task OnDisconnectedAsync(Exception? excepcion)
    {
        if (excepcion is not null)
        {
            _registro.LogWarning(
                "Cliente {ConexionId} desconectado con error: {Mensaje}.",
                Context.ConnectionId, excepcion.Message);
        }
        else
        {
            _registro.LogInformation(
                "Cliente {ConexionId} desconectado normalmente.",
                Context.ConnectionId);
        }

        return base.OnDisconnectedAsync(excepcion);
    }
}
