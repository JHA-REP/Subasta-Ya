import { createFileRoute, Link } from "@tanstack/react-router";
import { useEffect, useState, useCallback, useMemo } from "react";
import {
  AlertCircle,
  ArrowUpRight,
  Award,
  CheckCircle2,
  Flame,
  Gavel,
  Plus,
  RotateCw,
  TrendingUp,
  Trophy,
  Wallet,
} from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Countdown } from "@/components/Countdown";
import { useNow } from "@/hooks/use-now";
import { useUsuario } from "@/contextos/contextoUsuario";
import { servicioSubastas } from "@/servicios/servicioSubastas";
import { servicioPujas } from "@/servicios/servicioPujas";
import { formatoMoneda, formatoFecha } from "@/utilidades/formatoMonedaYFecha";
import type { Subasta, ParticipacionSubasta } from "@/tipos/subastaTipos";
import { cn } from "@/lib/utils";

export const Route = createFileRoute("/actividad")({
  ssr: false,
  head: () => ({
    meta: [
      { title: "Mis actividades: compras y publicaciones | SubastaYa" },
      {
        name: "description",
        content:
          "Seguí las subastas donde participaste y las métricas de recaudación de tus publicaciones.",
      },
      { property: "og:title", content: "Mis actividades | SubastaYa" },
      {
        property: "og:description",
        content: "Estado de tus pujas, adjudicaciones y ventas en SubastaYa.",
      },
    ],
  }),
  component: Actividad,
});

function Actividad() {
  const now = useNow();
  const { usuarioActual } = useUsuario();
  const [participaciones, setParticipaciones] = useState<ParticipacionSubasta[]>([]);
  const [publicaciones, setPublicaciones] = useState<Subasta[]>([]);
  const [cargando, setCargando] = useState(true);

  const cargarDatos = useCallback(async () => {
    if (!usuarioActual) {
      setParticipaciones([]);
      setPublicaciones([]);
      setCargando(false);
      return;
    }

    setCargando(true);
    try {
      const [resParticipaciones, resPublicaciones] = await Promise.all([
        servicioPujas.participacionesPorUsuario(usuarioActual.id).catch(() => []),
        servicioSubastas
          .listado({ vendedorId: usuarioActual.id, tamanoPagina: 100 })
          .then((res) => (res.items || []).filter((p) => p.vendedorId === usuarioActual.id))
          .catch(() => []),
      ]);

      setParticipaciones(resParticipaciones);
      setPublicaciones(resPublicaciones);
    } finally {
      setCargando(false);
    }
  }, [usuarioActual]);

  useEffect(() => {
    cargarDatos();
  }, [cargarDatos]);

  // Cálculos de métricas para Mis Publicaciones
  const metricas = useMemo(() => {
    const finalizadas = publicaciones.filter((p) => p.estado === "Finalizada");
    const activas = publicaciones.filter((p) => p.estado === "Activa");
    const desiertas = publicaciones.filter((p) => p.estado === "Desierta");

    const totalRecaudado = finalizadas.reduce(
      (acc, p) => acc + (p.montoFinal ?? p.montoMayorPuja ?? 0),
      0,
    );

    const recaudacionEnCurso = activas
      .filter((p) => p.cantidadPujas > 0)
      .reduce((acc, p) => acc + (p.montoMayorPuja ?? p.precioBase), 0);

    return {
      totalRecaudado,
      recaudacionEnCurso,
      adjudicadasCantidad: finalizadas.length,
      activasCantidad: activas.length,
      desiertasCantidad: desiertas.length,
      totalPublicaciones: publicaciones.length,
    };
  }, [publicaciones]);

  // Conteo de compras/pujas
  const conteoParticipaciones = useMemo(() => {
    const ganadas = participaciones.filter((p) => p.esGanador).length;
    const liderando = participaciones.filter((p) => p.estaLiderando).length;
    const superadas = participaciones.filter((p) => p.fueSuperado).length;
    return { ganadas, liderando, superadas };
  }, [participaciones]);

  return (
    <main className="mx-auto max-w-6xl px-4 py-10">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold md:text-4xl">Mis actividades</h1>
          <p className="mt-2 text-muted-foreground">
            Panel de control para el usuario{" "}
            <span className="font-semibold text-foreground">
              {usuarioActual ? `${usuarioActual.alias} (#${usuarioActual.id})` : "no seleccionado"}
            </span>
            .
          </p>
        </div>
        <Button
          variant="outline"
          size="sm"
          onClick={cargarDatos}
          disabled={cargando}
          className="w-fit gap-2"
        >
          <RotateCw className={cn("size-3.5", cargando && "animate-spin")} />
          Actualizar datos
        </Button>
      </div>

      <Tabs defaultValue="pujas" className="mt-8">
        <TabsList className="grid w-full grid-cols-2 max-w-md">
          <TabsTrigger value="pujas" className="gap-2">
            <span>Mis compras / pujas</span>
            <Badge variant="secondary" className="px-1.5 py-0 text-xs">
              {participaciones.length}
            </Badge>
          </TabsTrigger>
          <TabsTrigger value="publicaciones" className="gap-2">
            <span>Mis publicaciones</span>
            <Badge variant="secondary" className="px-1.5 py-0 text-xs">
              {publicaciones.length}
            </Badge>
          </TabsTrigger>
        </TabsList>

        {/* ─── PESTAÑA: MIS COMPRAS / PUJAS ─── */}
        <TabsContent value="pujas" className="mt-6 space-y-4">
          {/* Barra de resumen rápido de pujas */}
          {participaciones.length > 0 && (
            <div className="flex flex-wrap items-center gap-3 text-xs text-muted-foreground">
              <span className="font-medium text-foreground">Resumen:</span>
              {conteoParticipaciones.ganadas > 0 && (
                <span className="inline-flex items-center gap-1 rounded-md bg-emerald-500/10 px-2 py-1 text-emerald-600 dark:text-emerald-400 font-medium">
                  <Trophy className="size-3" />
                  {conteoParticipaciones.ganadas} ganada{conteoParticipaciones.ganadas > 1 ? "s" : ""}
                </span>
              )}
              {conteoParticipaciones.liderando > 0 && (
                <span className="inline-flex items-center gap-1 rounded-md bg-amber-500/10 px-2 py-1 text-amber-600 dark:text-amber-400 font-medium">
                  <Flame className="size-3" />
                  {conteoParticipaciones.liderando} liderando
                </span>
              )}
              {conteoParticipaciones.superadas > 0 && (
                <span className="inline-flex items-center gap-1 rounded-md bg-rose-500/10 px-2 py-1 text-rose-600 dark:text-rose-400 font-medium">
                  <AlertCircle className="size-3" />
                  {conteoParticipaciones.superadas} superada{conteoParticipaciones.superadas > 1 ? "s" : ""}
                </span>
              )}
            </div>
          )}

          {cargando ? (
            <div className="surface-card rounded-xl p-12 text-center text-muted-foreground">
              <RotateCw className="mx-auto size-6 animate-spin mb-2" />
              Cargando historial de participaciones…
            </div>
          ) : participaciones.length === 0 ? (
            <div className="surface-card rounded-xl p-12 text-center">
              <Gavel className="mx-auto size-12 text-muted-foreground/40 mb-3" />
              <h2 className="text-lg font-semibold">Sin pujas registradas</h2>
              <p className="mt-1 text-sm text-muted-foreground max-w-sm mx-auto">
                Aún no participaste con ofertas en ninguna subasta. Explorá el catálogo y realizá tu primera puja.
              </p>
              <Button asChild className="mt-5" size="sm">
                <Link to="/">Explorar subastas activas</Link>
              </Button>
            </div>
          ) : (
            <div className="space-y-3">
              {participaciones.map((p) => {
                const esActiva = p.estado === "Activa";

                return (
                  <div
                    key={p.subastaId}
                    className={cn(
                      "surface-card flex flex-col gap-4 rounded-xl p-4 transition-colors md:flex-row md:items-center md:justify-between border",
                      p.esGanador && "border-emerald-500/40 bg-emerald-500/5",
                      p.fueSuperado && esActiva && "border-rose-500/40",
                      p.estaLiderando && esActiva && "border-amber-500/40",
                    )}
                  >
                    <div className="flex items-center gap-4 flex-1 min-w-0">
                      <img
                        src={p.imagenUrl}
                        alt={p.titulo}
                        loading="lazy"
                        className="size-16 rounded-lg object-cover flex-shrink-0"
                      />
                      <div className="min-w-0 flex-1">
                        <div className="flex items-center gap-2">
                          <Link
                            to="/subasta/$id"
                            params={{ id: String(p.subastaId) }}
                            className="font-semibold hover:underline truncate"
                          >
                            {p.titulo}
                          </Link>
                        </div>
                        <p className="text-xs text-muted-foreground mt-0.5">
                          {p.categoria || "General"} · Vendedor: {p.vendedorAlias || "Anónimo"} · {p.cantidadMisPujas} oferta{p.cantidadMisPujas > 1 ? "s" : ""} tuya{p.cantidadMisPujas > 1 ? "s" : ""}
                        </p>
                        <p className="text-xs text-muted-foreground mt-0.5">
                          Última puja: {formatoFecha(p.fechaUltimaPuja)}
                        </p>
                      </div>
                    </div>

                    <div className="flex flex-wrap items-center gap-4 md:gap-6 justify-between md:justify-end">
                      {/* Estado temporal / Cuenta regresiva */}
                      <div className="text-left md:text-right">
                        <p className="text-xs text-muted-foreground">
                          {esActiva ? "Tiempo restante" : "Estado"}
                        </p>
                        {esActiva ? (
                          <Countdown endAt={p.fechaFin} startAt={p.fechaInicio} now={now} />
                        ) : (
                          <span className="text-xs font-medium text-muted-foreground">
                            Cerrada el {formatoFecha(p.fechaFin)}
                          </span>
                        )}
                      </div>

                      {/* Montos: Mi oferta vs Precio Actual */}
                      <div className="text-left md:text-right">
                        <p className="text-xs text-muted-foreground">Tu mayor puja</p>
                        <p className="font-mono text-sm font-semibold text-foreground">
                          {formatoMoneda(p.miMayorPuja)}
                        </p>
                        <p className="text-[11px] text-muted-foreground font-mono">
                          Líder: {formatoMoneda(p.precioActual)}
                        </p>
                      </div>

                      {/* Badge de situación */}
                      <div className="flex items-center gap-2">
                        {p.esGanador ? (
                          <Badge className="bg-emerald-500/15 text-emerald-600 dark:text-emerald-400 border-emerald-500/30 gap-1.5 py-1 px-2.5">
                            <Trophy className="size-3.5" />
                            Ganada ({formatoMoneda(p.montoFinal ?? p.precioActual)})
                          </Badge>
                        ) : p.estaLiderando && esActiva ? (
                          <Badge className="bg-amber-500/15 text-amber-600 dark:text-amber-400 border-amber-500/30 gap-1 py-1 px-2.5">
                            <Flame className="size-3.5" />
                            Liderando
                          </Badge>
                        ) : p.fueSuperado && esActiva ? (
                          <div className="flex items-center gap-2">
                            <Badge className="bg-rose-500/15 text-rose-600 dark:text-rose-400 border-rose-500/30 gap-1 py-1 px-2.5">
                              <AlertCircle className="size-3.5" />
                              Superado
                            </Badge>
                            <Button asChild size="sm" variant="outline" className="h-7 text-xs gap-1">
                              <Link to="/subasta/$id" params={{ id: String(p.subastaId) }}>
                                Pujar de nuevo
                                <ArrowUpRight className="size-3" />
                              </Link>
                            </Button>
                          </div>
                        ) : (
                          <Badge variant="secondary" className="py-1 px-2.5 text-muted-foreground">
                            {p.estado === "Desierta"
                              ? "Desierta"
                              : p.ganadorAlias
                                ? `Adjudicada a ${p.ganadorAlias}`
                                : "Finalizada"}
                          </Badge>
                        )}
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </TabsContent>

        {/* ─── PESTAÑA: MIS PUBLICACIONES ─── */}
        <TabsContent value="publicaciones" className="mt-6 space-y-6">
          {/* Tarjetas de Métricas de Recaudación */}
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            <TarjetaMetrica
              icono={Wallet}
              etiqueta="Total recaudado"
              valor={formatoMoneda(metricas.totalRecaudado)}
              detalle={`${metricas.adjudicadasCantidad} subastas liquidadas`}
              tono="success"
            />
            <TarjetaMetrica
              icono={TrendingUp}
              etiqueta="Recaudación en curso"
              valor={formatoMoneda(metricas.recaudacionEnCurso)}
              detalle="Ofertas líderes en subastas activas"
              tono="primary"
            />
            <TarjetaMetrica
              icono={Award}
              etiqueta="Adjudicadas"
              valor={String(metricas.adjudicadasCantidad)}
              detalle={
                metricas.desiertasCantidad > 0
                  ? `${metricas.desiertasCantidad} desierta${metricas.desiertasCantidad > 1 ? "s" : ""}`
                  : "100% de éxito en cerradas"
              }
              tono="default"
            />
            <TarjetaMetrica
              icono={Gavel}
              etiqueta="Publicaciones activas"
              valor={String(metricas.activasCantidad)}
              detalle={`De ${metricas.totalPublicaciones} publicaciones totales`}
              tono="warning"
            />
          </div>

          {/* Listado de publicaciones del vendedor */}
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-semibold">Listado de publicaciones creadas</h2>
              <Button asChild size="sm" className="gap-1.5">
                <Link to="/publicar">
                  <Plus className="size-4" />
                  Nueva publicación
                </Link>
              </Button>
            </div>

            {cargando ? (
              <div className="surface-card rounded-xl p-12 text-center text-muted-foreground">
                <RotateCw className="mx-auto size-6 animate-spin mb-2" />
                Cargando publicaciones…
              </div>
            ) : publicaciones.length === 0 ? (
              <div className="surface-card rounded-xl p-12 text-center">
                <Gavel className="mx-auto size-12 text-muted-foreground/40 mb-3" />
                <h3 className="text-lg font-semibold">Aún no publicaste subastas</h3>
                <p className="mt-1 text-sm text-muted-foreground max-w-sm mx-auto">
                  Como vendedor podés publicar productos, definir precio inicial y seguir en tiempo real la recaudación.
                </p>
                <Button asChild className="mt-5" size="sm">
                  <Link to="/publicar">Crear mi primera subasta</Link>
                </Button>
              </div>
            ) : (
              <div className="space-y-3">
                {publicaciones.map((a) => {
                  const esFinalizada = a.estado === "Finalizada";
                  const esActiva = a.estado === "Activa";
                  const precioActual = a.montoMayorPuja ?? a.precioBase;

                  return (
                    <div
                      key={a.id}
                      className={cn(
                        "surface-card flex flex-col gap-4 rounded-xl p-4 transition-colors md:flex-row md:items-center md:justify-between border",
                        esFinalizada && "border-emerald-500/30 bg-emerald-500/5",
                        esActiva && a.cantidadPujas > 0 && "border-primary/40",
                      )}
                    >
                      <div className="flex items-center gap-4 flex-1 min-w-0">
                        <img
                          src={a.imagenUrl}
                          alt={a.titulo}
                          loading="lazy"
                          className="size-16 rounded-lg object-cover flex-shrink-0"
                        />
                        <div className="min-w-0 flex-1">
                          <Link
                            to="/subasta/$id"
                            params={{ id: String(a.id) }}
                            className="font-semibold hover:underline truncate block"
                          >
                            {a.titulo}
                          </Link>
                          <p className="text-xs text-muted-foreground mt-0.5">
                            {a.categoria || "General"} · Base: {formatoMoneda(a.precioBase)} · Incremento: {formatoMoneda(a.incrementoMinimo)}
                          </p>
                          <p className="text-xs text-muted-foreground mt-0.5">
                            {a.cantidadPujas} oferta{a.cantidadPujas !== 1 ? "s" : ""} recibida{a.cantidadPujas !== 1 ? "s" : ""}
                          </p>
                        </div>
                      </div>

                      <div className="flex flex-wrap items-center gap-4 md:gap-6 justify-between md:justify-end">
                        {/* Estado temporal */}
                        <div className="text-left md:text-right">
                          <p className="text-xs text-muted-foreground">
                            {esActiva ? "Cierre" : "Estado"}
                          </p>
                          {esActiva ? (
                            <Countdown endAt={a.fechaFin} startAt={a.fechaInicio} now={now} />
                          ) : (
                            <span className="text-xs font-medium text-muted-foreground">
                              {a.estado}
                            </span>
                          )}
                        </div>

                        {/* Recaudación */}
                        <div className="text-left md:text-right">
                          <p className="text-xs text-muted-foreground">
                            {esFinalizada ? "Monto liquidado" : "Recaudación actual"}
                          </p>
                          <p
                            className={cn(
                              "font-mono text-base font-bold",
                              esFinalizada
                                ? "text-emerald-600 dark:text-emerald-400"
                                : a.cantidadPujas > 0
                                  ? "text-primary"
                                  : "text-muted-foreground",
                            )}
                          >
                            {formatoMoneda(esFinalizada ? a.montoFinal ?? precioActual : precioActual)}
                          </p>
                        </div>

                        {/* Estado de adjudicación */}
                        <div>
                          {esFinalizada ? (
                            <Badge className="bg-emerald-500/15 text-emerald-600 dark:text-emerald-400 border-emerald-500/30 gap-1 py-1 px-2.5">
                              <CheckCircle2 className="size-3.5" />
                              {a.ganadorAlias ? `Adjudicada a ${a.ganadorAlias}` : "Adjudicada"}
                            </Badge>
                          ) : a.estado === "Desierta" ? (
                            <Badge variant="outline" className="text-muted-foreground border-dashed">
                              Desierta (sin ofertas)
                            </Badge>
                          ) : a.estado === "Pendiente" ? (
                            <Badge variant="outline">Programada</Badge>
                          ) : a.cantidadPujas > 0 ? (
                            <Badge className="bg-primary/15 text-primary border-primary/30 gap-1 py-1 px-2.5">
                              <Flame className="size-3.5" />
                              {a.cantidadPujas} puja{a.cantidadPujas > 1 ? "s" : ""} en curso
                            </Badge>
                          ) : (
                            <Badge variant="secondary" className="text-xs">
                              Abierta (sin ofertas)
                            </Badge>
                          )}
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        </TabsContent>
      </Tabs>
    </main>
  );
}

function TarjetaMetrica({
  icono: Icono,
  etiqueta,
  valor,
  detalle,
  tono,
}: {
  icono: React.ComponentType<{ className?: string }>;
  etiqueta: string;
  valor: string;
  detalle?: string;
  tono: "default" | "warning" | "success" | "primary";
}) {
  return (
    <div className="surface-card rounded-xl p-5 border">
      <p className="flex items-center gap-2 text-xs uppercase tracking-wider text-muted-foreground">
        <Icono className="size-4" /> {etiqueta}
      </p>
      <p
        className={cn(
          "mt-2 font-display text-2xl md:text-3xl font-bold",
          tono === "warning" && "text-amber-500",
          tono === "success" && "text-emerald-600 dark:text-emerald-400",
          tono === "primary" && "text-primary",
          tono === "default" && "text-foreground",
        )}
      >
        {valor}
      </p>
      {detalle && <p className="mt-1 text-xs text-muted-foreground">{detalle}</p>}
    </div>
  );
}
