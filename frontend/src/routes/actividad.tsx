import { createFileRoute, Link } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { Badge } from "@/components/ui/badge";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Countdown } from "@/components/Countdown";
import { useNow } from "@/hooks/use-now";
import { useUsuario } from "@/contextos/contextoUsuario";
import { servicioSubastas } from "@/servicios/servicioSubastas";
import { formatoMoneda } from "@/utilidades/formatoMonedaYFecha";
import type { Subasta } from "@/tipos/subastaTipos";

export const Route = createFileRoute("/actividad")({
  ssr: false,
  head: () => ({
    meta: [
      { title: "Mis actividades: pujas y publicaciones | SubastaYa" },
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
  const [listaSubastas, setListaSubastas] = useState<Subasta[]>([]);
  const [cargando, setCargando] = useState(true);

  useEffect(() => {
    servicioSubastas
      .listado()
      .then(setListaSubastas)
      .catch(() => {})
      .finally(() => setCargando(false));
  }, []);

  const misPujas = listaSubastas.filter(
    (a) => usuarioActual && (a.ganadorId === usuarioActual.id || a.vendedorId !== usuarioActual.id),
  );

  const misPublicaciones = listaSubastas.filter(
    (a) => usuarioActual && a.vendedorId === usuarioActual.id,
  );

  return (
    <main className="mx-auto max-w-6xl px-4 py-10">
      <h1 className="text-3xl font-bold md:text-4xl">Mis actividades</h1>
      <p className="mt-2 text-muted-foreground">
        Actividad para el usuario activo{" "}
        <span className="font-semibold text-foreground">
          {usuarioActual ? `${usuarioActual.alias} (#${usuarioActual.id})` : "no seleccionado"}
        </span>
        .
      </p>

      <Tabs defaultValue="pujas" className="mt-8">
        <TabsList>
          <TabsTrigger value="pujas">Mis compras / pujas ({misPujas.length})</TabsTrigger>
          <TabsTrigger value="publicaciones">
            Mis publicaciones ({misPublicaciones.length})
          </TabsTrigger>
        </TabsList>

        <TabsContent value="pujas" className="mt-5 space-y-3">
          {cargando ? (
            <div className="py-8 text-center text-muted-foreground">Cargando subastas…</div>
          ) : misPujas.length === 0 ? (
            <ContenedorVacio texto="Todavía no participaste en ninguna subasta." />
          ) : (
            misPujas.map((a) => {
              const esActiva = a.estado === "Activa";
              const esFinalizada = a.estado === "Finalizada" || a.estado === "Desierta";
              const lider = usuarioActual && a.ganadorId === usuarioActual.id;
              const ganada = a.estado === "Finalizada" && lider;
              const precioActual = a.montoMayorPuja ?? a.precioBase;

              return (
                <FilaActividad key={a.id} subasta={a} now={now}>
                  <div className="text-right">
                    <p className="text-xs text-muted-foreground">Oferta actual</p>
                    <p className="font-mono text-sm font-semibold">{formatoMoneda(precioActual)}</p>
                  </div>
                  <Badge
                    variant={ganada ? "default" : esFinalizada ? "secondary" : "outline"}
                    className={
                      ganada
                        ? "bg-success text-success-foreground"
                        : lider && esActiva
                          ? "border-success/60 text-success"
                          : esActiva
                            ? "border-destructive/60 text-destructive"
                            : ""
                    }
                  >
                    {ganada
                      ? "Ganada"
                      : a.estado === "Desierta"
                        ? "Desierta"
                        : esFinalizada
                          ? "Finalizada"
                          : a.estado === "Pendiente"
                            ? "Programada"
                            : lider
                              ? "Liderando"
                              : "Superado"}
                  </Badge>
                </FilaActividad>
              );
            })
          )}
        </TabsContent>

        <TabsContent value="publicaciones" className="mt-5 space-y-3">
          {cargando ? (
            <div className="py-8 text-center text-muted-foreground">Cargando publicaciones…</div>
          ) : misPublicaciones.length === 0 ? (
            <ContenedorVacio texto="Aún no publicaste subastas como vendedor. Creá una desde 'Publicar'." />
          ) : (
            misPublicaciones.map((a) => {
              const precioActual = a.montoMayorPuja ?? a.precioBase;
              return (
                <FilaActividad key={a.id} subasta={a} now={now}>
                  <div className="text-right">
                    <p className="text-xs text-muted-foreground">Recaudación actual</p>
                    <p className="font-mono text-sm font-semibold text-primary">
                      {formatoMoneda(precioActual)}
                    </p>
                  </div>
                  <Badge variant={a.estado === "Finalizada" ? "secondary" : "outline"}>
                    {a.estado === "Finalizada"
                      ? a.ganadorAlias
                        ? `Adjudicada a ${a.ganadorAlias}`
                        : "Adjudicada"
                      : a.estado === "Desierta"
                        ? "Desierta"
                        : a.estado === "Pendiente"
                          ? "Programada"
                          : `${a.cantidadPujas} ofertas`}
                  </Badge>
                </FilaActividad>
              );
            })
          )}
        </TabsContent>
      </Tabs>
    </main>
  );
}

function FilaActividad({
  subasta,
  now,
  children,
}: {
  subasta: Subasta;
  now: number;
  children: React.ReactNode;
}) {
  return (
    <Link
      to="/subasta/$id"
      params={{ id: String(subasta.id) }}
      className="surface-card flex flex-wrap items-center gap-4 rounded-xl p-3 transition-colors hover:border-primary/50"
    >
      <img
        src={subasta.imagenUrl}
        alt={subasta.titulo}
        loading="lazy"
        className="size-16 rounded-lg object-cover"
      />
      <div className="min-w-40 flex-1">
        <p className="font-medium">{subasta.titulo}</p>
        <p className="text-xs text-muted-foreground">{subasta.categoria || "General"}</p>
      </div>
      <Countdown endAt={subasta.fechaFin} startAt={subasta.fechaInicio} now={now} />
      {children}
    </Link>
  );
}

function ContenedorVacio({ texto }: { texto: string }) {
  return (
    <div className="surface-card rounded-xl p-10 text-center text-sm text-muted-foreground">
      {texto}
    </div>
  );
}
