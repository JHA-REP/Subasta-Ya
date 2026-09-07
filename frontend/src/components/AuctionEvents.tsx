import { useEffect } from "react";
import { toast } from "sonner";
import { servicioTiempoReal } from "@/servicios/servicioTiempoReal";
import { useUsuario } from "@/contextos/contextoUsuario";

/**
 * Escucha global de eventos de tiempo real mediante SignalR para mostrar alertas sonner.
 * Reemplaza los temporizadores y simulaciones locales con eventos reales del backend.
 */
export function AuctionEvents() {
  const { usuarioActual, eventoActualizacionBilletera } = useUsuario();

  useEffect(() => {
    // Inicia conexion a SignalR
    servicioTiempoReal.conexion();

    const cancelacionExtension = servicioTiempoReal.registroExtension((datos) => {
      toast.warning("Subasta extendida", {
        description: `Regla anti-sniping: se sumaron ${datos.segundosAdicionales} segundos a la subasta #${datos.subastaId}.`,
      });
    });

    const cancelacionFinalizada = servicioTiempoReal.registroFinalizada((datos) => {
      const ganadorEsUsuario = usuarioActual && datos.ganadorId === usuarioActual.id;
      if (ganadorEsUsuario) {
        toast.success("¡Felicitaciones, ganaste la subasta!", {
          description: `La subasta #${datos.subastaId} fue adjudicada a tu favor.`,
        });
      } else {
        toast.info("Subasta finalizada", {
          description: `La subasta #${datos.subastaId} ha finalizado.`,
        });
      }
      eventoActualizacionBilletera();
    });

    const cancelacionDesierta = servicioTiempoReal.registroDesierta((datos) => {
      toast.info("Subasta desierta", {
        description: `La subasta #${datos.subastaId} finalizó sin ofertas.`,
      });
    });

    const cancelacionPuja = servicioTiempoReal.registroPuja((datos) => {
      // Si la puja la hizo el usuario actual, actualizar la billetera para reflejar retencion
      if (usuarioActual && datos.postorId === usuarioActual.id) {
        eventoActualizacionBilletera();
      }
    });

    return () => {
      cancelacionExtension();
      cancelacionFinalizada();
      cancelacionDesierta();
      cancelacionPuja();
    };
  }, [usuarioActual, eventoActualizacionBilletera]);

  return null;
}
