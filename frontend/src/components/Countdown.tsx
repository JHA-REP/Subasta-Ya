import { Timer } from "lucide-react";
import { cn } from "@/lib/utils";
import { formatoTiempoRestante } from "@/utilidades/formatoMonedaYFecha";

interface PropiedadesContador {
  endAt: number | string;
  now: number;
  startAt?: number | string;
  size?: "sm" | "lg";
  className?: string;
}

/**
 * Cuenta regresiva con zona critica:
 * - Ultimo minuto: ambar
 * - Ultimos 10s: rojo pulsante
 */
export function Countdown({
  endAt,
  now,
  startAt,
  size = "sm",
  className,
}: PropiedadesContador) {
  const tiempoFinal = typeof endAt === "string" ? new Date(endAt).getTime() : endAt;
  const tiempoInicio = startAt !== undefined
    ? typeof startAt === "string"
      ? new Date(startAt).getTime()
      : startAt
    : undefined;

  const noIniciada = tiempoInicio !== undefined && now < tiempoInicio;
  const objetivo = noIniciada ? tiempoInicio! : tiempoFinal;
  const restante = objetivo - now;
  const finalizada = !noIniciada && restante <= 0;
  const critica = !noIniciada && !finalizada && restante <= 10_000;
  const advertencia = !noIniciada && !finalizada && restante <= 60_000;

  return (
    <span
      className={cn(
        "inline-flex items-center gap-1.5 rounded-full border px-2.5 py-1 font-mono tabular-nums",
        size === "sm" ? "text-xs" : "text-2xl px-4 py-2 font-semibold",
        finalizada && "border-border bg-muted text-muted-foreground",
        noIniciada && "border-accent/40 bg-accent/10 text-accent",
        !finalizada && !noIniciada && !advertencia && "border-border bg-secondary text-foreground",
        advertencia && !critica && "border-warning/50 bg-warning/15 text-warning",
        critica && "border-destructive/60 bg-destructive/20 text-destructive animate-pulse-critical",
        className,
      )}
    >
      <Timer className={size === "sm" ? "size-3.5" : "size-5"} aria-hidden />
      {noIniciada
        ? `Inicia en ${formatoTiempoRestante(restante)}`
        : formatoTiempoRestante(restante)}
    </span>
  );
}
