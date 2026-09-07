import { clienteHttp } from "./clienteHttp";
import type { Usuario } from "@/tipos/subastaTipos";

/**
 * Servicio de comunicaciones para la entidad Usuario.
 * Nombres basados en sustantivos y conceptos.
 */
export const servicioUsuarios = {
  listado: (): Promise<Usuario[]> =>
    clienteHttp.consulta<Usuario[]>("/usuarios"),

  detalle: (id: number): Promise<Usuario> =>
    clienteHttp.consulta<Usuario>(`/usuarios/${id}`),
};
