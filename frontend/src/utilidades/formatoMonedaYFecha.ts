/**
 * Utilidades de formateo para moneda, tiempo restante y fechas.
 * Nombres basados en sustantivos y conceptos (sin verbos).
 */

export const formatoMoneda = (monto: number): string =>
  "$" + Math.round(monto).toLocaleString("es-AR", { maximumFractionDigits: 0 });

export const formatoTiempoRestante = (milisegundos: number): string => {
  if (milisegundos <= 0) return "Finalizada";
  const segundosTotales = Math.floor(milisegundos / 1000);
  const dias = Math.floor(segundosTotales / 86400);
  const horas = Math.floor((segundosTotales % 86400) / 3600);
  const minutos = Math.floor((segundosTotales % 3600) / 60);
  const segundos = segundosTotales % 60;

  if (dias > 0) return `${dias}d ${horas}h`;
  if (horas > 0) return `${horas}h ${String(minutos).padStart(2, "0")}m`;
  return `${String(minutos).padStart(2, "0")}:${String(segundos).padStart(2, "0")}`;
};

export const formatoHora = (marcaTiempo: string | number): string =>
  new Date(marcaTiempo).toLocaleTimeString("es-AR", {
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
  });

export const formatoFecha = (marcaTiempo: string | number): string =>
  new Date(marcaTiempo).toLocaleString("es-AR", {
    day: "2-digit",
    month: "short",
    hour: "2-digit",
    minute: "2-digit",
  });
