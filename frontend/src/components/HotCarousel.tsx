import { useEffect, useMemo, useState } from "react";
import { Link } from "@tanstack/react-router";
import { ChevronLeft, ChevronRight, Flame, Gavel } from "lucide-react";
import { Countdown } from "@/components/Countdown";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import type { Subasta } from "@/tipos/subastaTipos";
import { useUsuario } from "@/contextos/contextoUsuario";
import { formatoMoneda } from "@/utilidades/formatoMonedaYFecha";

const tiempoRotacionMs = 5000;

interface PropiedadesCarrusel {
  subastas: Subasta[];
  now: number;
}

/** Carrusel con las 5 subastas mas pujadas que estan por vencer. */
export function HotCarousel({ subastas, now }: PropiedadesCarrusel) {
  const { usuarioActual } = useUsuario();
  const [indice, setIndice] = useState(0);

  const destacadas = subastas;

  useEffect(() => {
    if (destacadas.length < 2) return;
    const temporizador = setInterval(
      () => setIndice((i) => (i + 1) % destacadas.length),
      tiempoRotacionMs,
    );
    return () => clearInterval(temporizador);
  }, [destacadas.length]);

  useEffect(() => {
    if (indice >= destacadas.length) setIndice(0);
  }, [destacadas.length, indice]);

  if (!destacadas.length) return null;

  return (
    <section aria-label="Subastas más pujadas próximas a vencer" className="mx-auto max-w-7xl px-4 pt-8">
      <div className="mb-4 flex items-center gap-2">
        <Flame className="size-5 text-primary" aria-hidden />
        <h2 className="text-xl font-bold md:text-2xl">Las más pujadas, por vencer</h2>
      </div>

      <div className="surface-card relative overflow-hidden rounded-2xl">
        <div
          className="flex transition-transform duration-500 ease-out"
          style={{ transform: `translateX(-${indice * 100}%)` }}
        >
          {destacadas.map((a) => {
            const precioActual = a.montoMayorPuja ?? a.precioBase;
            const lider = usuarioActual && a.ganadorId === usuarioActual.id;

            return (
              <Link
                key={a.id}
                to="/subasta/$id"
                params={{ id: String(a.id) }}
                className="group grid w-full shrink-0 gap-0 md:grid-cols-2"
              >
                <div className="relative aspect-[16/9] overflow-hidden bg-muted md:aspect-auto md:min-h-72">
                  <img
                    src={a.imagenUrl}
                    alt={a.titulo}
                    loading="lazy"
                    className="absolute inset-0 size-full object-cover transition-transform duration-500 group-hover:scale-105"
                  />
                  <Badge variant="secondary" className="absolute left-4 top-4 backdrop-blur">
                    {a.categoria || "General"}
                  </Badge>
                </div>

                <div className="flex flex-col justify-center gap-4 p-6 md:p-10">
                  <div className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
                    <Gavel className="size-4 text-primary" aria-hidden />
                    {a.cantidadPujas} ofertas
                  </div>
                  <h3 className="text-2xl font-bold leading-tight md:text-3xl">{a.titulo}</h3>
                  <p className="line-clamp-2 text-sm text-muted-foreground">{a.descripcion}</p>

                  <div className="flex flex-wrap items-end gap-x-8 gap-y-3">
                    <div>
                      <p className="text-xs uppercase tracking-wide text-muted-foreground">
                        Oferta actual
                      </p>
                      <p className="font-display text-3xl font-bold text-primary">
                        {formatoMoneda(precioActual)}
                      </p>
                    </div>
                    <div>
                      <p className="mb-1 text-xs uppercase tracking-wide text-muted-foreground">
                        Cierra en
                      </p>
                      <Countdown endAt={a.fechaFin} startAt={a.fechaInicio} now={now} />
                    </div>
                  </div>

                  {lider && (
                    <p className="text-sm font-semibold text-success">Vas ganando esta subasta</p>
                  )}
                </div>
              </Link>
            );
          })}
        </div>

        {destacadas.length > 1 && (
          <>
            <Button
              variant="secondary"
              size="icon"
              aria-label="Anterior"
              className="absolute left-3 top-1/2 -translate-y-1/2 rounded-full shadow"
              onClick={(e) => {
                e.preventDefault();
                setIndice((i) => (i - 1 + destacadas.length) % destacadas.length);
              }}
            >
              <ChevronLeft className="size-5" />
            </Button>
            <Button
              variant="secondary"
              size="icon"
              aria-label="Siguiente"
              className="absolute right-3 top-1/2 -translate-y-1/2 rounded-full shadow"
              onClick={(e) => {
                e.preventDefault();
                setIndice((i) => (i + 1) % destacadas.length);
              }}
            >
              <ChevronRight className="size-5" />
            </Button>

            <div className="absolute bottom-3 left-1/2 flex -translate-x-1/2 gap-1.5">
              {destacadas.map((a, i) => (
                <button
                  key={a.id}
                  type="button"
                  aria-label={`Ir al producto ${i + 1}`}
                  onClick={(e) => {
                    e.preventDefault();
                    setIndice(i);
                  }}
                  className={`h-2 rounded-full transition-all ${
                    i === indice ? "w-6 bg-primary" : "w-2 bg-muted-foreground/40"
                  }`}
                />
              ))}
            </div>
          </>
        )}
      </div>
    </section>
  );
}
