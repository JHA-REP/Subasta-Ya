import { clienteHttp } from "./clienteHttp";
import type { Puja, NuevaPujaEntrada } from "@/tipos/subastaTipos";

/**
 * Servicio de comunicaciones para la entidad Puja.
 * Nombres basados en sustantivos y conceptos.
 */
export const servicioPujas = {
  listadoPorSubasta: (subastaId: number): Promise<Puja[]> =>
    clienteHttp.consulta<Puja[]>(`/pujas/subasta/${subastaId}`),

  creacion: (datos: NuevaPujaEntrada): Promise<Puja> =>
    clienteHttp.envio<Puja>("/pujas", datos),
};
