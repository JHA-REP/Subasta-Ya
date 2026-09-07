/**
 * Servicio de comunicaciones en tiempo real mediante SignalR (WebSockets).
 * Conecta con el hub /hubs/subastas y distribuye los eventos recibidos.
 * Nombres basados en sustantivos y conceptos (sin verbos).
 */

import * as signalR from "@microsoft/signalr";
import { configuracionEntorno } from "@/configuracion/entorno";
import type {
  Puja,
  InformacionTemporizador,
  EventoExtensionAntiSniping,
  SubastaFinalizada,
} from "@/tipos/subastaTipos";

let conexionHub: signalR.HubConnection | null = null;

const oyentesPuja = new Set<(puja: Puja) => void>();
const oyentesTemporizador = new Set<(info: InformacionTemporizador) => void>();
const oyentesExtension = new Set<(info: EventoExtensionAntiSniping) => void>();
const oyentesFinalizada = new Set<(info: SubastaFinalizada) => void>();
const oyentesDesierta = new Set<(info: { subastaId: number }) => void>();

const inicializacionConexion = async (): Promise<signalR.HubConnection> => {
  if (conexionHub && conexionHub.state === signalR.HubConnectionState.Connected) {
    return conexionHub;
  }

  if (!conexionHub) {
    conexionHub = new signalR.HubConnectionBuilder()
      .withUrl(configuracionEntorno.urlHubSubastas, {
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling,
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .configureLogging(signalR.LogLevel.Information)
      .build();

    conexionHub.on("eventoPujaRecibida", (datos: Puja) => {
      oyentesPuja.forEach((oyente) => oyente(datos));
    });

    conexionHub.on("estadoTemporizador", (datos: InformacionTemporizador) => {
      oyentesTemporizador.forEach((oyente) => oyente(datos));
    });

    conexionHub.on("eventoExtensionAntiSniping", (datos: EventoExtensionAntiSniping) => {
      oyentesExtension.forEach((oyente) => oyente(datos));
    });

    conexionHub.on("eventoSubastaFinalizada", (datos: SubastaFinalizada) => {
      oyentesFinalizada.forEach((oyente) => oyente(datos));
    });

    conexionHub.on("eventoSubastaDesierta", (datos: { subastaId: number }) => {
      oyentesDesierta.forEach((oyente) => oyente(datos));
    });
  }

  if (conexionHub.state === signalR.HubConnectionState.Disconnected) {
    try {
      await conexionHub.start();
    } catch {
      /* Reintento automatico gestionado por SignalR */
    }
  }

  return conexionHub;
};

export const servicioTiempoReal = {
  conexion: inicializacionConexion,

  suscripcionSubasta: async (subastaId: number): Promise<void> => {
    try {
      const conexion = await inicializacionConexion();
      if (conexion.state === signalR.HubConnectionState.Connected) {
        await conexion.invoke("SuscripcionSubasta", subastaId);
      }
    } catch {
      /* Manejo silencioso en reintento */
    }
  },

  desuscripcionSubasta: async (subastaId: number): Promise<void> => {
    try {
      if (conexionHub && conexionHub.state === signalR.HubConnectionState.Connected) {
        await conexionHub.invoke("DesuscripcionSubasta", subastaId);
      }
    } catch {
      /* Manejo silencioso */
    }
  },

  registroPuja: (oyente: (puja: Puja) => void): (() => void) => {
    oyentesPuja.add(oyente);
    return () => oyentesPuja.delete(oyente);
  },

  registroTemporizador: (oyente: (info: InformacionTemporizador) => void): (() => void) => {
    oyentesTemporizador.add(oyente);
    return () => oyentesTemporizador.delete(oyente);
  },

  registroExtension: (oyente: (info: EventoExtensionAntiSniping) => void): (() => void) => {
    oyentesExtension.add(oyente);
    return () => oyentesExtension.delete(oyente);
  },

  registroFinalizada: (oyente: (info: SubastaFinalizada) => void): (() => void) => {
    oyentesFinalizada.add(oyente);
    return () => oyentesFinalizada.delete(oyente);
  },

  registroDesierta: (oyente: (info: { subastaId: number }) => void): (() => void) => {
    oyentesDesierta.add(oyente);
    return () => oyentesDesierta.delete(oyente);
  },
};
