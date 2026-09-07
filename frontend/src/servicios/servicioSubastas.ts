import { clienteHttp } from "./clienteHttp";
import type { Subasta, NuevaSubastaEntrada } from "@/tipos/subastaTipos";

/**
 * Servicio de comunicaciones para la entidad Subasta.
 * Nombres basados en sustantivos y conceptos.
 */
export const servicioSubastas = {
  listado: (): Promise<Subasta[]> =>
    clienteHttp.consulta<Subasta[]>("/subastas"),

  detalle: (id: number): Promise<Subasta> =>
    clienteHttp.consulta<Subasta>(`/subastas/${id}`),

  creacion: (datos: NuevaSubastaEntrada): Promise<Subasta> =>
    clienteHttp.envio<Subasta>("/subastas", datos),
};
