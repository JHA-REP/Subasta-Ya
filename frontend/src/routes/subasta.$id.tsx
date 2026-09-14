import { createFileRoute, Link } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { ArrowLeft, Crown, Gavel, TrendingUp, Users, Zap } from "lucide-react";
import { Countdown } from "@/components/Countdown";
import { Badge } from "@/components/ui/badge";
import { Dialog, DialogContent, DialogTrigger } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useNow } from "@/hooks/use-now";
import { cn } from "@/lib/utils";
import { servicioSubastas } from "@/servicios/servicioSubastas";
import { servicioPujas } from "@/servicios/servicioPujas";
import { servicioTiempoReal } from "@/servicios/servicioTiempoReal";
import { useUsuario } from "@/contextos/contextoUsuario";
import {
  formatoMoneda,
  formatoFecha,
  formatoHora,
} from "@/utilidades/formatoMonedaYFecha";
import type { Subasta, Puja } from "@/tipos/subastaTipos";

export const Route = createFileRoute("/subasta/$id")({
  ssr: false,
  head: () => ({
    meta: [
      { title: "Sala de subasta en vivo | SubastaYa" },
      {
        name: "description",
        content:
          "Pujá en tiempo real: temporizador, historial de ofertas y consola de puja con anti-sniping.",
      },
      { property: "og:title", content: "Sala de subasta en vivo | SubastaYa" },
      {
        property: "og:description",
        content: "Temporizador crítico, historial de ofertas y estado Liderando / Superado.",
      },
    ],
  }),
  component: SalaEnVivo,
});

function SalaEnVivo() {
  const { id } = Route.useParams();
  const subastaId = Number(id);
  const now = useNow();
  const { usuarioActual, billeteraActual, eventoActualizacionBilletera } = useUsuario();

  const [subasta, setSubasta] = useState<Subasta | null>(null);
  const [historialPujas, setHistorialPujas] = useState<Puja[]>([]);
  const [montoPersonalizado, setMontoPersonalizado] = useState("");
  const [destello, setDestello] = useState(false);
  const [cargando, setCargando] = useState(true);
  const [enviandoPuja, setEnviandoPuja] = useState(false);

  // Carga inicial y conexion a SignalR
  useEffect(() => {
    let montado = true;

    const cargaDatos = async () => {
      try {
        const [detalleSubasta, pujas] = await Promise.all([
          servicioSubastas.detalle(subastaId),
          servicioPujas.listadoPorSubasta(subastaId),
        ]);
        if (!montado) return;
        setSubasta(detalleSubasta);
        setHistorialPujas(pujas);
      } catch {
        /* Error al cargar */
      } finally {
        if (montado) setCargando(false);
      }
    };

    cargaDatos();

    // Suscripcion a la sala de SignalR
    servicioTiempoReal.suscripcionSubasta(subastaId);

    // Escucha de eventos de tiempo real
    const cancelacionPuja = servicioTiempoReal.registroPuja((puja) => {
      if (puja.subastaId === subastaId) {
        setHistorialPujas((actuales) => [
          puja,
          ...actuales.filter((p) => p.id !== puja.id),
        ]);
        setSubasta((actual) =>
          actual
            ? {
                ...actual,
                montoMayorPuja: puja.monto,
                cantidadPujas: actual.cantidadPujas + 1,
                ganadorId: puja.postorId,
                ganadorAlias: puja.postorAlias,
              }
            : null,
        );
        setDestello(true);
        setTimeout(() => setDestello(false), 700);
      }
    });

    const cancelacionTemporizador = servicioTiempoReal.registroTemporizador((info) => {
      if (info.subastaId === subastaId) {
        setSubasta((actual) =>
          actual ? { ...actual, fechaFin: info.fechaFin } : null,
        );
      }
    });

    const cancelacionExtension = servicioTiempoReal.registroExtension((ext) => {
      if (ext.subastaId === subastaId) {
        setSubasta((actual) =>
          actual ? { ...actual, fechaFin: ext.nuevaFechaFin } : null,
        );
      }
    });

    const cancelacionFinalizada = servicioTiempoReal.registroFinalizada((fin) => {
      if (fin.subastaId === subastaId) {
        setSubasta((actual) =>
          actual
            ? {
                ...actual,
                estado: "Finalizada",
                ganadorId: fin.ganadorId ?? actual.ganadorId,
                montoFinal: fin.montoFinal ?? actual.montoFinal,
              }
            : null,
        );
      }
    });

    const cancelacionDesierta = servicioTiempoReal.registroDesierta((des) => {
      if (des.subastaId === subastaId) {
        setSubasta((actual) =>
          actual ? { ...actual, estado: "Desierta" } : null,
        );
      }
    });

    return () => {
      montado = false;
      servicioTiempoReal.desuscripcionSubasta(subastaId);
      cancelacionPuja();
      cancelacionTemporizador();
      cancelacionExtension();
      cancelacionFinalizada();
      cancelacionDesierta();
    };
  }, [subastaId]);

  if (cargando) {
    return (
      <main className="mx-auto max-w-3xl px-4 py-24 text-center">
        <p className="text-muted-foreground">Cargando sala de subasta…</p>
      </main>
    );
  }

  if (!subasta) {
    return (
      <main className="mx-auto max-w-3xl px-4 py-24 text-center">
        <h1 className="text-2xl font-bold">Subasta no encontrada</h1>
        <Link to="/" className="mt-4 inline-block text-primary underline">
          Volver al catálogo
        </Link>
      </main>
    );
  }

  const esActiva = subasta.estado === "Activa";
  const esProxima = subasta.estado === "Pendiente";
  const esFinalizada = subasta.estado === "Finalizada" || subasta.estado === "Desierta";

  const pujaMayor = historialPujas[0];
  
  const precioActual = pujaMayor?.monto ?? subasta.montoMayorPuja ?? subasta.precioBase;
  const proximaPujaMinima = pujaMayor
    ? pujaMayor.monto + subasta.incrementoMinimo
    : subasta.montoMayorPuja
      ? subasta.montoMayorPuja + subasta.incrementoMinimo
      : subasta.precioBase;
  const lider = usuarioActual && (subasta.ganadorId === usuarioActual.id || pujaMayor?.postorId === usuarioActual.id);
  const superado =
    !lider &&
    usuarioActual &&
    historialPujas.some((p) => p.postorId === usuarioActual.id);

  const tiempoFinMs = new Date(subasta.fechaFin).getTime();
  const tiempoRestante = tiempoFinMs - now;
  const esCritico = esActiva && tiempoRestante <= 60_000;
  const saldoDisponible = billeteraActual?.saldoDisponible ?? 0;

  const eventoEnvioPuja = async (monto: number) => {
    if (!usuarioActual) {
      toast.error("Seleccioná un usuario activo en el encabezado.");
      return;
    }

    if (usuarioActual.id === subasta.vendedorId) {
      toast.error("Regla de negocio", {
        description: "El vendedor no puede ofertar en su propia subasta.",
      });
      return;
    }

    if (lider) {
      toast.info("Ya sos el postor líder", {
        description: "Tu oferta actual ya lidera la subasta.",
      });
      return;
    }

    if (monto < proximaPujaMinima) {
      toast.error("Oferta insuficiente", {
        description: `La oferta mínima permitida es ${formatoMoneda(proximaPujaMinima)}.`,
      });
      return;
    }

    if (monto > saldoDisponible) {
      toast.error("Fondos insuficientes", {
        description: `Tenés ${formatoMoneda(saldoDisponible)} disponibles en tu billetera. Cargá saldo desde la pestaña Billetera.`,
      });
      return;
    }

    setEnviandoPuja(true);
    try {
      await servicioPujas.creacion({
        subastaId,
        postorId: usuarioActual.id,
        monto,
      });

      setMontoPersonalizado("");
      toast.success("¡Puja confirmada!", {
        description: `Ofertaste ${formatoMoneda(monto)} con éxito.`,
      });
      await eventoActualizacionBilletera();
    } catch (error: unknown) {
      const mensaje = error instanceof Error ? error.message : "Error al procesar la puja.";
      toast.error("No se pudo registrar la puja", {
        description: mensaje,
      });
    } finally {
      setEnviandoPuja(false);
    }
  };

  const cantidadPostoresUnicos = new Set(historialPujas.map((p) => p.postorId)).size;

  return (
    <main className="mx-auto max-w-7xl px-4 py-8">
      <Link
        to="/"
        className="inline-flex items-center gap-1.5 text-sm text-muted-foreground hover:text-foreground"
      >
        <ArrowLeft className="size-4" aria-hidden /> Volver al catálogo
      </Link>

      <div className="mt-4 grid gap-6 lg:grid-cols-[1.35fr_1fr]">
        {/* Producto */}
        <section className="space-y-5">
          <div
            className={cn(
              "surface-card overflow-hidden rounded-xl",
              esCritico && tiempoRestante <= 10_000 && "animate-pulse-critical",
            )}
          >
            <div className="relative aspect-video bg-muted">
              <Dialog>
                <DialogTrigger asChild>
                  <img
                    src={subasta.imagenUrl}
                    alt={subasta.titulo}
                    className="size-full object-cover cursor-pointer transition-opacity hover:opacity-90"
                    title="Hacé clic para ver la imagen completa"
                  />
                </DialogTrigger>
                <DialogContent className="max-w-5xl border-none bg-transparent p-0 shadow-none">
                  <img
                    src={subasta.imagenUrl}
                    alt={subasta.titulo}
                    className="max-h-[85vh] w-full rounded-md object-contain"
                  />
                </DialogContent>
              </Dialog>
              <div className="absolute left-3 top-3 flex gap-2">
                <Badge variant="secondary">{subasta.categoria || "General"}</Badge>
                {esActiva && (
                  <Badge className="gap-1">
                    <span className="size-1.5 animate-pulse rounded-full bg-current" />
                    EN VIVO
                  </Badge>
                )}
              </div>
            </div>
            <div className="p-5">
              <h1 className="text-2xl font-bold md:text-3xl">{subasta.titulo}</h1>
              <p className="mt-1 text-sm text-muted-foreground">
                Vendedor: {subasta.vendedor || `Vendedor #${subasta.vendedorId}`} • Cierre previsto {formatoFecha(subasta.fechaFin)}
              </p>
              <p className="mt-4 whitespace-pre-line text-sm leading-relaxed text-muted-foreground">
                {subasta.descripcion}
              </p>
              <dl className="mt-5 grid grid-cols-2 gap-3 sm:grid-cols-4">
                <Metric label="Precio base" value={formatoMoneda(subasta.precioBase)} />
                <Metric label="Incremento mín." value={formatoMoneda(subasta.incrementoMinimo)} />
                <Metric label="Ofertas" value={String(subasta.cantidadPujas)} icon={Users} />
                <Metric
                  label="Postores"
                  value={String(cantidadPostoresUnicos)}
                  icon={TrendingUp}
                />
              </dl>
            </div>
          </div>

          {/* Historial de ofertas */}
          <div className="surface-card rounded-xl p-5">
            <h2 className="flex items-center gap-2 text-lg font-semibold">
              <Gavel className="size-4 text-primary" aria-hidden /> Historial de ofertas
            </h2>
            <ul className="mt-4 max-h-96 space-y-2 overflow-y-auto pr-1">
              {historialPujas.length === 0 && (
                <li className="rounded-lg border border-dashed border-border p-6 text-center text-sm text-muted-foreground">
                  Todavía no hay ofertas. Podés abrir la puja al precio base.
                </li>
              )}
              {historialPujas.map((b, i) => {
                const esMia = usuarioActual && b.postorId === usuarioActual.id;
                return (
                  <li
                    key={b.id}
                    className={cn(
                      "flex items-center justify-between gap-3 rounded-lg border px-3 py-2.5",
                      i === 0
                        ? "border-primary/50 bg-primary/10 animate-bid-in"
                        : "border-border bg-secondary/40",
                    )}
                  >
                    <div className="flex items-center gap-3">
                      <span className="flex size-8 items-center justify-center rounded-full bg-secondary font-mono text-xs">
                        {(b.postorAlias || "PO").slice(0, 2).toUpperCase()}
                      </span>
                      <div>
                        <p className="text-sm font-medium">
                          {esMia ? "Vos" : b.postorAlias || b.liderAnonimizado}
                          {i === 0 && (
                            <span className="ml-2 text-xs font-normal text-primary">líder</span>
                          )}
                        </p>
                        <p className="font-mono text-xs text-muted-foreground">{formatoHora(b.fechaPuja)}</p>
                      </div>
                    </div>
                    <p className="font-mono text-sm font-semibold">{formatoMoneda(b.monto)}</p>
                  </li>
                );
              })}
            </ul>
          </div>
        </section>

        {/* Consola de puja */}
        <aside className="space-y-5 lg:sticky lg:top-24 lg:h-fit">
          <div className="surface-card rounded-xl p-5 text-center">
            <p className="text-xs uppercase tracking-widest text-muted-foreground">
              {esProxima
                ? "Comienza en"
                : esFinalizada
                  ? "Subasta cerrada"
                  : "Tiempo restante"}
            </p>
            <div className="mt-3 flex justify-center">
              <Countdown
                endAt={subasta.fechaFin}
                startAt={subasta.fechaInicio}
                now={now}
                size="lg"
                className="w-full justify-center"
              />
            </div>
            {esActiva && tiempoRestante <= 10_000 && (
              <p className="mt-3 flex items-center justify-center gap-1.5 text-xs font-medium text-warning">
                <Zap className="size-3.5" aria-hidden /> Zona anti-sniping: cada puja suma tiempo
              </p>
            )}
          </div>

          <div
            className={cn(
              "surface-card rounded-xl p-5",
              destello && "glow-ring transition-shadow duration-500",
            )}
          >
            <p className="text-xs uppercase tracking-widest text-muted-foreground">
              Oferta más alta
            </p>
            <p className="font-display text-4xl font-bold text-ember">
              {formatoMoneda(precioActual)}
            </p>
            <p className="mt-1 text-sm text-muted-foreground">
              {pujaMayor
                ? `por ${usuarioActual && pujaMayor.postorId === usuarioActual.id ? "vos" : pujaMayor.postorAlias || pujaMayor.liderAnonimizado}`
                : "Sin ofertas aún"}
            </p>

            {esActiva && (lider || superado) && (
              <div
                className={cn(
                  "mt-4 flex items-center gap-2 rounded-lg border px-3 py-2 text-sm font-medium",
                  lider
                    ? "border-success/50 bg-success/15 text-success"
                    : "border-destructive/50 bg-destructive/15 text-destructive animate-shake",
                )}
              >
                {lider ? <Crown className="size-4" aria-hidden /> : <Zap className="size-4" aria-hidden />}
                {lider ? "Estás liderando la subasta" : "Te superaron — volvé a ofertar"}
              </div>
            )}

            {esActiva ? (
              <div className="mt-5 space-y-3">
                <Button
                  size="lg"
                  className="w-full"
                  disabled={enviandoPuja || Boolean(lider)}
                  onClick={() => eventoEnvioPuja(proximaPujaMinima)}
                >
                  {lider ? "Sos el postor líder" : `Pujar ${formatoMoneda(proximaPujaMinima)}`}
                </Button>
                <div className="flex gap-2">
                  <Input
                    type="number"
                    inputMode="numeric"
                    min={proximaPujaMinima}
                    value={montoPersonalizado}
                    onChange={(e) => setMontoPersonalizado(e.target.value)}
                    placeholder={`Monto personalizado (mín. ${proximaPujaMinima})`}
                  />
                  <Button
                    variant="secondary"
                    disabled={!montoPersonalizado || enviandoPuja || Boolean(lider)}
                    onClick={() => eventoEnvioPuja(Number(montoPersonalizado))}
                  >
                    Ofertar
                  </Button>
                </div>
                <p className="text-xs text-muted-foreground">
                  Saldo disponible:{" "}
                  <span className="font-mono text-success">{formatoMoneda(saldoDisponible)}</span> • Sugerencia ={" "}
                  puja actual + {formatoMoneda(subasta.incrementoMinimo)}
                </p>
              </div>
            ) : esProxima ? (
              <p className="mt-5 rounded-lg border border-accent/40 bg-accent/10 p-3 text-sm text-accent">
                Esta subasta todavía no comenzó. Volvé cuando arranque el reloj.
              </p>
            ) : (
              <p className="mt-5 rounded-lg border border-border bg-secondary p-3 text-sm">
                {subasta.ganadorId
                  ? `Adjudicada a ${usuarioActual && subasta.ganadorId === usuarioActual.id ? "vos" : subasta.ganadorAlias || `Usuario #${subasta.ganadorId}`} por ${formatoMoneda(subasta.montoFinal ?? precioActual)}.`
                  : "Finalizó sin ofertas (Desierta)."}
              </p>
            )}
          </div>
        </aside>
      </div>
    </main>
  );
}

function Metric({
  label,
  value,
  icon: Icon,
}: {
  label: string;
  value: string;
  icon?: React.ComponentType<{ className?: string }>;
}) {
  return (
    <div className="rounded-lg border border-border bg-secondary/40 p-3">
      <dt className="flex items-center gap-1 text-[11px] uppercase tracking-wider text-muted-foreground">
        {Icon && <Icon className="size-3" />} {label}
      </dt>
      <dd className="mt-1 font-mono text-sm font-semibold">{value}</dd>
    </div>
  );
}
