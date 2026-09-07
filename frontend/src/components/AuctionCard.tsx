import { Link } from "@tanstack/react-router";
import { Gavel, Crown } from "lucide-react";
import { Countdown } from "@/components/Countdown";
import { Badge } from "@/components/ui/badge";
import type { Subasta } from "@/tipos/subastaTipos";
import { useUsuario } from "@/contextos/contextoUsuario";
import { formatoMoneda } from "@/utilidades/formatoMonedaYFecha";

interface PropiedadesTarjetaSubasta {
  subasta: Subasta;
  now: number;
}

export function AuctionCard({ subasta, now }: PropiedadesTarjetaSubasta) {
  const { usuarioActual } = useUsuario();

  const esActiva = subasta.estado === "Activa";
  const esFinalizada = subasta.estado === "Finalizada" || subasta.estado === "Desierta";
  const precioActual = subasta.montoMayorPuja ?? subasta.precioBase;
  const lider = usuarioActual && subasta.ganadorId === usuarioActual.id;

  return (
    <Link
      to="/subasta/$id"
      params={{ id: String(subasta.id) }}
      className="group surface-card animate-rise flex flex-col overflow-hidden rounded-xl transition-all duration-300 hover:-translate-y-1 hover:glow-ring"
    >
      <div className="relative aspect-[4/3] overflow-hidden bg-muted">
        <img
          src={subasta.imagenUrl}
          alt={subasta.titulo}
          loading="lazy"
          className="size-full object-cover transition-transform duration-500 group-hover:scale-105"
        />
        <div className="absolute inset-x-0 bottom-0 flex items-center justify-between gap-2 bg-gradient-to-t from-background/95 to-transparent p-3">
          <Badge variant="secondary" className="backdrop-blur">
            {subasta.categoria || "General"}
          </Badge>
          <Countdown endAt={subasta.fechaFin} startAt={subasta.fechaInicio} now={now} />
        </div>
        {esFinalizada && (
          <div className="absolute inset-0 flex items-center justify-center bg-background/70 text-sm font-semibold uppercase tracking-widest text-muted-foreground">
            {subasta.estado === "Desierta" ? "Desierta" : "Finalizada"}
          </div>
        )}
      </div>

      <div className="flex flex-1 flex-col gap-3 p-4">
        <h3 className="line-clamp-2 text-base font-semibold leading-snug">{subasta.titulo}</h3>

        <div className="mt-auto flex items-end justify-between gap-3">
          <div>
            <p className="text-xs text-muted-foreground">
              {subasta.montoMayorPuja ? "Oferta más alta" : "Precio base"}
            </p>
            <p className="font-display text-xl font-bold text-primary">
              {formatoMoneda(precioActual)}
            </p>
          </div>
          <div className="text-right text-xs text-muted-foreground">
            <span className="inline-flex items-center gap-1">
              <Gavel className="size-3.5" aria-hidden />
              {subasta.cantidadPujas} {subasta.cantidadPujas === 1 ? "oferta" : "ofertas"}
            </span>
            {lider && esActiva && (
              <span className="mt-1 flex items-center justify-end gap-1 font-medium text-success">
                <Crown className="size-3.5" aria-hidden /> Liderás
              </span>
            )}
          </div>
        </div>
      </div>
    </Link>
  );
}
