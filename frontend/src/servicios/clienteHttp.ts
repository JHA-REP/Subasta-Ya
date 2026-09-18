/**
 * Cliente HTTP centralizado para peticiones a la API de SubastaYa.
 * Nombres basados en conceptos y sustantivos (sin verbos).
 */

import { configuracionEntorno } from "@/configuracion/entorno";

export class ErrorComunicacion extends Error {
  codigoEstado: number;
  detalle: unknown;

  constructor(mensaje: string, codigoEstado: number, detalle?: unknown) {
    super(mensaje);
    this.name = "ErrorComunicacion";
    this.codigoEstado = codigoEstado;
    this.detalle = detalle;
  }
}

const cabecerasPorDefecto = {
  "Content-Type": "application/json",
  Accept: "application/json",
};

export const clienteHttp = {
  consulta: async <T>(ruta: string): Promise<T> => {
    const urlCompleta = `${configuracionEntorno.urlApi}${ruta}`;
    const respuesta = await fetch(urlCompleta, {
      method: "GET",
      headers: cabecerasPorDefecto,
    });

    if (!respuesta.ok) {
      let textoError = `Error HTTP ${respuesta.status}: ${respuesta.statusText}`;
      try {
        const errorJson = await respuesta.json();
        if (errorJson?.mensaje) textoError = errorJson.mensaje;
        else if (errorJson?.title) textoError = errorJson.title;
        else if (typeof errorJson === "string") textoError = errorJson;
      } catch {
        /* ignora si no es JSON */
      }
      throw new ErrorComunicacion(textoError, respuesta.status);
    }

    return (await respuesta.json()) as T;
  },

  envio: async <T>(ruta: string, cuerpo?: unknown): Promise<T> => {
    const urlCompleta = `${configuracionEntorno.urlApi}${ruta}`;
    const respuesta = await fetch(urlCompleta, {
      method: "POST",
      headers: cabecerasPorDefecto,
      body: cuerpo !== undefined ? JSON.stringify(cuerpo) : undefined,
    });

    if (!respuesta.ok) {
      let textoError = `Error HTTP ${respuesta.status}: ${respuesta.statusText}`;
      try {
        const errorJson = await respuesta.json();
        if (errorJson?.mensaje) textoError = errorJson.mensaje;
        else if (errorJson?.title) textoError = errorJson.title;
        else if (typeof errorJson === "string") textoError = errorJson;
      } catch {
        /* ignora si no es JSON */
      }
      throw new ErrorComunicacion(textoError, respuesta.status);
    }

    return (await respuesta.json()) as T;
  },
};
