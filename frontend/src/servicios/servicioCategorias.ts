import { clienteHttp } from "./clienteHttp";
import type { Categoria } from "@/tipos/subastaTipos";

/**
 * Servicio de comunicaciones para la entidad Categoria.
 * Nombres basados en sustantivos y conceptos.
 */
export const servicioCategorias = {
  listado: (): Promise<Categoria[]> =>
    clienteHttp.consulta<Categoria[]>("/categorias"),

  detalle: (id: number): Promise<Categoria> =>
    clienteHttp.consulta<Categoria>(`/categorias/${id}`),
};
