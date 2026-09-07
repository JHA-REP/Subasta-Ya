import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useMemo, useState } from "react";
import { Search, Flame, Sparkles } from "lucide-react";
import { AuctionCard } from "@/components/AuctionCard";
import { HotCarousel } from "@/components/HotCarousel";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Slider } from "@/components/ui/slider";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useNow } from "@/hooks/use-now";
import { servicioSubastas } from "@/servicios/servicioSubastas";
import { servicioCategorias } from "@/servicios/servicioCategorias";
import { servicioTiempoReal } from "@/servicios/servicioTiempoReal";
import { formatoMoneda } from "@/utilidades/formatoMonedaYFecha";
import type { Subasta, Categoria, EstadoSubasta } from "@/tipos/subastaTipos";

export const Route = createFileRoute("/")({
  ssr: false,
  head: () => ({
    meta: [
      { title: "Catálogo de subastas en vivo | SubastaYa" },
      {
        name: "description",
        content:
          "Explorá subastas activas, próximas y finalizadas. Filtrá por categoría, precio y tiempo restante.",
      },
      { property: "og:title", content: "Catálogo de subastas en vivo | SubastaYa" },
      {
        property: "og:description",
        content: "Pujá en tiempo real con contador regresivo y regla anti-sniping.",
      },
    ],
  }),
  component: Catalogo,
});

type OpcionFiltroEstado = "Activa" | "Pendiente" | "Finalizada" | "todas";

const listaOpcionesEstado: Array<{ valor: OpcionFiltroEstado; etiqueta: string }> = [
  { valor: "Activa", etiqueta: "Activas" },
  { valor: "Pendiente", etiqueta: "Próximas" },
  { valor: "Finalizada", etiqueta: "Finalizadas" },
  { valor: "todas", etiqueta: "Todas" },
];

function Catalogo() {
  const now = useNow();

  const [listaSubastas, setListaSubastas] = useState<Subasta[]>([]);
  const [listaCategorias, setListaCategorias] = useState<Categoria[]>([]);
  const [cargando, setCargando] = useState(true);

  const [estadoFiltro, setEstadoFiltro] = useState<OpcionFiltroEstado>("Activa");
  const [categoriaFiltro, setCategoriaFiltro] = useState<string>("todas");
  const [terminoBusqueda, setTerminoBusqueda] = useState("");
  const [precioMaximoFiltro, setPrecioMaximoFiltro] = useState<number | null>(null);
  const [criterioOrden, setCriterioOrden] = useState<
    "tiempo" | "puja-desc" | "puja-asc" | "ofertas"
  >("tiempo");

  const consultaDatosIniciales = async () => {
    try {
      const [subastas, categorias] = await Promise.all([
        servicioSubastas.listado(),
        servicioCategorias.listado(),
      ]);
      setListaSubastas(subastas);
      setListaCategorias(categorias);
    } catch {
      /* Silencioso */
    } finally {
      setCargando(false);
    }
  };

  useEffect(() => {
    consultaDatosIniciales();

    // Actualizacion en tiempo real al recibir eventos SignalR
    const cancelacionPuja = servicioTiempoReal.registroPuja(() => {
      servicioSubastas.listado().then(setListaSubastas).catch(() => {});
    });

    const cancelacionFinalizada = servicioTiempoReal.registroFinalizada(() => {
      servicioSubastas.listado().then(setListaSubastas).catch(() => {});
    });

    const cancelacionDesierta = servicioTiempoReal.registroDesierta(() => {
      servicioSubastas.listado().then(setListaSubastas).catch(() => {});
    });

    return () => {
      cancelacionPuja();
      cancelacionFinalizada();
      cancelacionDesierta();
    };
  }, []);

  const topePrecio = useMemo(() => {
    if (listaSubastas.length === 0) return 100000;
    const maximo = Math.max(
      ...listaSubastas.map((s) => s.montoMayorPuja ?? s.precioBase),
    );
    return Math.max(10000, maximo * 1.05);
  }, [listaSubastas]);

  const maximoEfectivo = precioMaximoFiltro ?? topePrecio;

  const subastasFiltradas = useMemo(() => {
    const filtradas = listaSubastas.filter((s) => {
      if (estadoFiltro !== "todas") {
        if (estadoFiltro === "Finalizada") {
          if (s.estado !== "Finalizada" && s.estado !== "Desierta") return false;
        } else if (s.estado !== (estadoFiltro as EstadoSubasta)) {
          return false;
        }
      }
      if (categoriaFiltro !== "todas" && s.categoria !== categoriaFiltro) return false;
      if (
        terminoBusqueda &&
        !`${s.titulo} ${s.descripcion}`.toLowerCase().includes(terminoBusqueda.toLowerCase())
      ) {
        return false;
      }
      const precio = s.montoMayorPuja ?? s.precioBase;
      if (precio > maximoEfectivo) return false;
      return true;
    });

    return filtradas.sort((a, b) => {
      const precioA = a.montoMayorPuja ?? a.precioBase;
      const precioB = b.montoMayorPuja ?? b.precioBase;

      switch (criterioOrden) {
        case "puja-desc":
          return precioB - precioA;
        case "puja-asc":
          return precioA - precioB;
        case "ofertas":
          return b.cantidadPujas - a.cantidadPujas;
        default:
          return new Date(a.fechaFin).getTime() - new Date(b.fechaFin).getTime();
      }
    });
  }, [
    listaSubastas,
    estadoFiltro,
    categoriaFiltro,
    terminoBusqueda,
    maximoEfectivo,
    criterioOrden,
  ]);

  const cantidadActivas = listaSubastas.filter((s) => s.estado === "Activa").length;

  return (
    <main>
      <section className="hero-glow border-b border-border">
        <div className="mx-auto max-w-7xl px-4 py-14 md:py-20">
          <p className="animate-rise inline-flex items-center gap-2 rounded-full border border-primary/40 bg-primary/10 px-3 py-1 text-xs font-medium text-primary">
            <Flame className="size-3.5" aria-hidden /> {cantidadActivas} subastas en curso ahora mismo
          </p>
          <h1 className="animate-rise mt-5 max-w-3xl text-4xl font-bold leading-[1.05] md:text-6xl">
            El martillo cae <span className="text-ember">en vivo</span>. Pujá antes de que se apague
            el reloj.
          </h1>
          <p className="animate-rise mt-4 max-w-xl text-base text-muted-foreground md:text-lg">
            Contadores en tiempo real, historial de ofertas al instante y extensión automática de
            segundos cuando alguien puja sobre la hora.
          </p>
        </div>
      </section>

      <HotCarousel subastas={listaSubastas} now={now} />

      <section className="mx-auto max-w-7xl px-4 py-8">
        <div className="surface-card rounded-xl p-4">
          <div className="flex flex-wrap items-center gap-2">
            {listaOpcionesEstado.map((s) => (
              <Button
                key={s.valor}
                size="sm"
                variant={estadoFiltro === s.valor ? "default" : "secondary"}
                onClick={() => setEstadoFiltro(s.valor)}
              >
                {s.etiqueta}
              </Button>
            ))}
          </div>

          <div className="mt-4 grid gap-4 md:grid-cols-2 lg:grid-cols-4">
            <div className="relative">
              <Search
                className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground"
                aria-hidden
              />
              <Input
                value={terminoBusqueda}
                onChange={(e) => setTerminoBusqueda(e.target.value)}
                placeholder="Buscar producto…"
                className="pl-9"
                aria-label="Buscar subastas"
              />
            </div>

            <Select value={categoriaFiltro} onValueChange={setCategoriaFiltro}>
              <SelectTrigger aria-label="Categoría">
                <SelectValue placeholder="Categoría" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todas">Todas las categorías</SelectItem>
                {listaCategorias.map((c) => (
                  <SelectItem key={c.id} value={c.nombre}>
                    {c.nombre}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            <Select value={criterioOrden} onValueChange={(v) => setCriterioOrden(v as typeof criterioOrden)}>
              <SelectTrigger aria-label="Ordenar">
                <SelectValue placeholder="Ordenar" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="tiempo">Menor tiempo restante</SelectItem>
                <SelectItem value="puja-desc">Mayor puja</SelectItem>
                <SelectItem value="puja-asc">Menor puja</SelectItem>
                <SelectItem value="ofertas">Más ofertas</SelectItem>
              </SelectContent>
            </Select>

            <div className="px-1">
              <p className="mb-2 text-xs text-muted-foreground">
                Precio hasta <span className="font-mono text-foreground">{formatoMoneda(maximoEfectivo)}</span>
              </p>
              <Slider
                value={[maximoEfectivo]}
                min={0}
                max={topePrecio}
                step={Math.max(1000, Math.round(topePrecio / 200))}
                onValueChange={(v) => setPrecioMaximoFiltro(v[0] ?? topePrecio)}
                aria-label="Precio máximo"
              />
            </div>
          </div>
        </div>

        {cargando ? (
          <div className="mt-14 flex justify-center text-muted-foreground">
            Cargando catálogo en vivo…
          </div>
        ) : subastasFiltradas.length === 0 ? (
          <div className="surface-card mt-10 flex flex-col items-center gap-2 rounded-xl p-14 text-center">
            <Sparkles className="size-6 text-muted-foreground" aria-hidden />
            <p className="font-medium">No hay subastas con esos filtros</p>
            <p className="text-sm text-muted-foreground">Probá ampliar el rango o cambiar el estado.</p>
          </div>
        ) : (
          <div className="mt-8 grid gap-5 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
            {subastasFiltradas.map((s) => (
              <AuctionCard key={s.id} subasta={s} now={now} />
            ))}
          </div>
        )}
      </section>
    </main>
  );
}
