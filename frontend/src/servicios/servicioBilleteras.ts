import { clienteHttp } from "./clienteHttp";
import type { Billetera, AcreditacionEntrada } from "@/tipos/subastaTipos";

/**
 * Servicio de comunicaciones para la entidad Billetera.
 * Nombres basados en sustantivos y conceptos.
 */
export const servicioBilleteras = {
  detallePorUsuario: (usuarioId: number): Promise<Billetera> =>
    clienteHttp.consulta<Billetera>(`/billeteras/usuario/${usuarioId}`),

  acreditacion: (usuarioId: number, datos: AcreditacionEntrada): Promise<Billetera> =>
    clienteHttp.envio<Billetera>(`/billeteras/usuario/${usuarioId}/acreditaciones`, {
      ...datos,
      usuarioId,
    }),
};
