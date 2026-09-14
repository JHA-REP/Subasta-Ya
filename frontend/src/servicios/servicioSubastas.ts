import { clienteHttp } from "./clienteHttp";
import type { Subasta, NuevaSubastaEntrada } from "@/tipos/subastaTipos";

export interface FiltrosSubasta {
  estado?: string;
  categoriaId?: number;
  precioMin?: number;
  precioMax?: number;
  pagina?: number;
  tamanoPagina?: number;
  terminoBusqueda?: string;
  criterioOrden?: string;
}

export interface PaginadoResponse<T> {
  items: T[];
  totalItems: number;
  pagina: number;
  tamanoPagina: number;
  totalPaginas: number;
}

/**
 * Servicio de comunicaciones para la entidad Subasta.
 * Nombres basados en sustantivos y conceptos.
 */
export const servicioSubastas = {
  listado: async (filtros: FiltrosSubasta = {}): Promise<PaginadoResponse<Subasta>> => {
    const query = new URLSearchParams();
    
    if (filtros.estado && filtros.estado !== "todas") query.append("estado", filtros.estado);
    if (filtros.categoriaId) query.append("categoriaId", String(filtros.categoriaId));
    if (filtros.precioMin !== undefined) query.append("precioMin", String(filtros.precioMin));
    if (filtros.precioMax !== undefined) query.append("precioMax", String(filtros.precioMax));
    if (filtros.pagina) query.append("pagina", String(filtros.pagina));
    if (filtros.tamanoPagina) query.append("tamanoPagina", String(filtros.tamanoPagina));
    if (filtros.terminoBusqueda) query.append("terminoBusqueda", filtros.terminoBusqueda);
    if (filtros.criterioOrden) query.append("criterioOrden", filtros.criterioOrden);

    const queryString = query.toString() ? `?${query.toString()}` : "";
    return clienteHttp.consulta<PaginadoResponse<Subasta>>(`/subastas${queryString}`);
  },

  detalle: (id: number): Promise<Subasta> =>
    clienteHttp.consulta<Subasta>(`/subastas/${id}`),

  creacion: (datos: NuevaSubastaEntrada): Promise<Subasta> =>
    clienteHttp.envio<Subasta>("/subastas", datos),
};
