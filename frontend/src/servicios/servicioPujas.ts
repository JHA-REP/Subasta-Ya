import { clienteHttp } from "./clienteHttp";
import type { Puja, NuevaPujaEntrada } from "@/tipos/subastaTipos";

/**
 * Servicio de comunicaciones para la entidad Puja.

 */
export const servicioPujas = {
  listadoPorSubasta: (subastaId: number): Promise<Puja[]> =>
    clienteHttp.consulta<Puja[]>(`/subastas/${subastaId}/pujas`),


  creacion: (subastaId: number, datos: NuevaPujaEntrada): Promise<Puja> =>
    clienteHttp.envio<Puja>(`/subastas/${subastaId}/pujas`, datos),
};
