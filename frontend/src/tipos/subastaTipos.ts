/**
 * Definiciones de tipos e interfaces del dominio SubastaYa.
 * Representacion fidedigna de los DTOs y modelos del backend en castellano.
 */

export type EstadoSubasta =
  | "Pendiente"
  | "Activa"
  | "Finalizada"
  | "Cancelada"
  | "Desierta";

export type RolUsuario = "Administrador" | "Vendedor" | "Comprador";

export type TipoMovimiento =
  | "Carga"
  | "Retencion"
  | "Liberacion"
  | "Debito"
  | "Credito";

export interface Subasta {
  id: number;
  titulo: string;
  descripcion: string;
  imagenUrl: string;
  precioBase: number;
  incrementoMinimo: number;
  categoria: string;
  categoriaId: number;
  vendedor: string;
  vendedorId: number;
  estado: EstadoSubasta;
  fechaInicio: string;
  fechaFin: string;
  cantidadPujas: number;
  montoMayorPuja: number | null;
  ganadorId: number | null;
  ganadorAlias: string | null;
  montoFinal: number | null;
}

export interface NuevaSubastaEntrada {
  titulo: string;
  descripcion: string;
  precioInicial: number;
  incrementoMinimo: number;
  imagenUrl: string;
  fechaInicio: string;
  fechaFin: string;
  vendedorId: number;
  categoriaId: number;
}

export interface Puja {
  id: number;
  subastaId: number;
  postorId: number;
  postorAlias: string;
  liderAnonimizado: string;
  monto: number;
  fechaPuja: string;
}

export interface NuevaPujaEntrada {
  subastaId: number;
  postorId: number;
  monto: number;
}

export interface MovimientoContableDto {
  id: number;
  billeteraId: number;
  tipo: TipoMovimiento;
  monto: number;
  concepto: string;
  fechaMovimiento: string;
}

export interface Billetera {
  id: number;
  usuarioId: number;
  usuarioAlias: string;
  saldo: number;
  saldoRetenido: number;
  saldoDisponible: number;
  movimientos?: MovimientoContableDto[];
}

export interface AcreditacionEntrada {
  usuarioId?: number;
  monto: number;
}

export interface Usuario {
  id: number;
  alias: string;
  email: string;
  rol: RolUsuario;
  fechaRegistro: string;
}

export interface Categoria {
  id: number;
  nombre: string;
  descripcion: string;
  cantidadSubastas: number;
}

export interface InformacionTemporizador {
  subastaId: number;
  segundosRestantes: number;
  critico: boolean;
  fechaFinUtc: string;
}

export interface EventoExtensionAntiSniping {
  subastaId: number;
  nuevaFechaFin: string;
  segundosAdicionales: number;
  motivo: string;
}

export interface SubastaFinalizada {
  subastaId: number;
  ganadorId?: number | null;
  montoFinal?: number | null;
  motivo: string;
}

export interface MovimientoSesion {
  id: string;
  fecha: number;
  tipo: TipoMovimiento;
  detalle: string;
  monto: number;
}
