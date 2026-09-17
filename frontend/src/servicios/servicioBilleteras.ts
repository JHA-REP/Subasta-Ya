import { clienteHttp } from "./clienteHttp";
import type { Billetera, AcreditacionEntrada, MovimientoContableDto } from "@/tipos/subastaTipos";

/**
 * Servicio de comunicaciones para la entidad Billetera.
 * Nombres basados en sustantivos y conceptos.
 */
export const servicioBilleteras = {
  detallePorUsuario: (usuarioId: number) =>
    clienteHttp.consulta<Billetera>(`/usuarios/${usuarioId}/billetera`),

  acreditacion: (usuarioId: number, datos: AcreditacionEntrada) =>
    clienteHttp.envio<Billetera>(`/usuarios/${usuarioId}/billetera/acreditaciones`, { ...datos, usuarioId }),

  movimientosPorUsuario: (usuarioId: number) =>
    clienteHttp.consulta<MovimientoContableDto[]>(`/usuarios/${usuarioId}/billetera/movimientos`)
};
