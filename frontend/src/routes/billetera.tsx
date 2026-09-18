import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState, useCallback } from "react";
import { toast } from "sonner";
import { Lock, PiggyBank, RotateCw, Wallet } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { cn } from "@/lib/utils";
import { useUsuario } from "@/contextos/contextoUsuario";
import { servicioBilleteras } from "@/servicios/servicioBilleteras";
import { formatoFecha, formatoMoneda } from "@/utilidades/formatoMonedaYFecha";
import type { MovimientoContableDto, TipoMovimiento } from "@/tipos/subastaTipos";

export const Route = createFileRoute("/billetera")({
  ssr: false,
  head: () => ({
    meta: [
      { title: "Billetera virtual y fondos | SubastaYa" },
      {
        name: "description",
        content:
          "Consultá saldo total, retenido y disponible, cargá fondos y revisá el historial de movimientos.",
      },
      { property: "og:title", content: "Billetera virtual y fondos | SubastaYa" },
      {
        property: "og:description",
        content: "Gestión de garantías, retenciones y débitos por subastas ganadas.",
      },
    ],
  }),
  component: Billetera,
});

const configuracionTipos: Record<
  TipoMovimiento,
  { etiqueta: string; claseBadge: string; claseMonto: string; prefijo: string }
> = {
  Carga: {
    etiqueta: "Ingreso",
    claseBadge: "bg-emerald-500/15 text-emerald-600 dark:text-emerald-400 border-emerald-500/30",
    claseMonto: "text-emerald-600 dark:text-emerald-400 font-semibold",
    prefijo: "+",
  },
  Retencion: {
    etiqueta: "Retención",
    claseBadge: "bg-amber-500/15 text-amber-600 dark:text-amber-400 border-amber-500/30",
    claseMonto: "text-amber-600 dark:text-amber-400 font-medium",
    prefijo: "🔒 ",
  },
  Liberacion: {
    etiqueta: "Liberación",
    claseBadge: "bg-sky-500/15 text-sky-600 dark:text-sky-400 border-sky-500/30",
    claseMonto: "text-sky-600 dark:text-sky-400 font-medium",
    prefijo: "🔓 ",
  },
  Debito: {
    etiqueta: "Débito ganado",
    claseBadge: "bg-rose-500/15 text-rose-600 dark:text-rose-400 border-rose-500/30",
    claseMonto: "text-rose-600 dark:text-rose-400 font-semibold",
    prefijo: "-",
  },
  Credito: {
    etiqueta: "Venta liquidada",
    claseBadge: "bg-emerald-500/15 text-emerald-600 dark:text-emerald-400 border-emerald-500/30",
    claseMonto: "text-emerald-600 dark:text-emerald-400 font-semibold",
    prefijo: "+",
  },
};

function Billetera() {
  const { usuarioActual, billeteraActual, eventoActualizacionBilletera } = useUsuario();
  const [montoEntrada, setMontoEntrada] = useState("");
  const [procesandoAcreditacion, setProcesandoAcreditacion] = useState(false);
  const [movimientosLocales, setMovimientosLocales] = useState<MovimientoContableDto[]>([]);
  const [cargandoMovimientos, setCargandoMovimientos] = useState(false);

  const cargarMovimientos = useCallback(async () => {
    if (!usuarioActual) return;
    setCargandoMovimientos(true);
    try {
      const datos = await servicioBilleteras.movimientosPorUsuario(usuarioActual.id);
      setMovimientosLocales(datos);
    } catch {
      // Fallback a los movimientos de billeteraActual si falla la consulta puntual
      if (billeteraActual?.movimientos) {
        setMovimientosLocales(billeteraActual.movimientos);
      }
    } finally {
      setCargandoMovimientos(false);
    }
  }, [usuarioActual, billeteraActual?.movimientos]);

  // Sincronizar movimientos cuando cambia usuario o billetera actual
  useEffect(() => {
    if (billeteraActual?.movimientos && billeteraActual.movimientos.length > 0) {
      setMovimientosLocales(billeteraActual.movimientos);
    } else {
      cargarMovimientos();
    }
  }, [billeteraActual, cargarMovimientos]);

  const saldoTotal = billeteraActual?.saldo ?? 0;
  const saldoRetenido = billeteraActual?.saldoRetenido ?? 0;
  const saldoDisponible = billeteraActual?.saldoDisponible ?? 0;

  const eventoEnvioCarga = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!usuarioActual) {
      toast.error("Seleccioná un usuario activo en el encabezado.");
      return;
    }

    const valor = Number(montoEntrada);
    if (!Number.isFinite(valor) || valor <= 0) {
      toast.error("Monto inválido", { description: "Ingresá un importe mayor a cero." });
      return;
    }

    if (valor > 50_000_000) {
      toast.error("Monto demasiado alto", { description: "El tope por carga es $50.000.000." });
      return;
    }

    setProcesandoAcreditacion(true);
    try {
      await servicioBilleteras.acreditacion(usuarioActual.id, { monto: valor });
      await eventoActualizacionBilletera();
      await cargarMovimientos();

      setMontoEntrada("");
      toast.success("Saldo acreditado", {
        description: `Sumaste ${formatoMoneda(valor)} a tu billetera.`,
      });
    } catch (error: unknown) {
      const mensaje = error instanceof Error ? error.message : "Error al acreditar fondos.";
      toast.error("Error en la acreditación", { description: mensaje });
    } finally {
      setProcesandoAcreditacion(false);
    }
  };

  const listaMovimientos = movimientosLocales.length > 0
    ? movimientosLocales
    : (billeteraActual?.movimientos ?? []);

  return (
    <main className="mx-auto max-w-6xl px-4 py-10">
      <h1 className="text-3xl font-bold md:text-4xl">Billetera virtual</h1>
      <p className="mt-2 text-muted-foreground">
        Tus fondos se retienen automáticamente mientras liderás una subasta activa.
      </p>

      <div className="mt-8 grid gap-4 md:grid-cols-3">
        <TarjetaEstadistica
          icono={Wallet}
          etiqueta="Saldo total"
          valor={formatoMoneda(saldoTotal)}
          tono="default"
        />
        <TarjetaEstadistica
          icono={Lock}
          etiqueta="Retenido en garantía"
          valor={formatoMoneda(saldoRetenido)}
          tono="warning"
        />
        <TarjetaEstadistica
          icono={PiggyBank}
          etiqueta="Disponible"
          valor={formatoMoneda(saldoDisponible)}
          tono="success"
        />
      </div>

      <div className="mt-6 grid gap-6 lg:grid-cols-[1fr_2fr]">
        <form onSubmit={eventoEnvioCarga} className="surface-card h-fit space-y-4 rounded-xl p-5">
          <h2 className="text-lg font-semibold">Cargar saldo</h2>
          <div className="space-y-2">
            <Label htmlFor="monto">Monto a acreditar</Label>
            <Input
              id="monto"
              type="number"
              min={1}
              value={montoEntrada}
              onChange={(e) => setMontoEntrada(e.target.value)}
              placeholder="500000"
            />
          </div>
          <div className="flex flex-wrap gap-2">
            {[10000, 50000, 100000, 500000].map((v) => (
              <Button
                key={v}
                type="button"
                size="sm"
                variant="secondary"
                onClick={() => setMontoEntrada(String(v))}
              >
                +{formatoMoneda(v)}
              </Button>
            ))}
          </div>
          <Button type="submit" className="w-full" disabled={procesandoAcreditacion}>
            {procesandoAcreditacion ? "Procesando…" : "Acreditar fondos"}
          </Button>
          <p className="text-xs text-muted-foreground">
            Carga simulada para pruebas: registra el movimiento contable real en la base de datos de SubastaYa.
          </p>
        </form>

        <div className="surface-card rounded-xl p-5">
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-lg font-semibold">Historial de movimientos</h2>
              <p className="text-xs text-muted-foreground">
                Libro mayor contable inmutable con ingresos, retenciones, liberaciones y liquidaciones.
              </p>
            </div>
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={cargarMovimientos}
              disabled={cargandoMovimientos}
              className="gap-1.5 text-xs"
              title="Actualizar movimientos"
            >
              <RotateCw className={cn("size-3.5", cargandoMovimientos && "animate-spin")} />
              Actualizar
            </Button>
          </div>

          <div className="mt-4 max-h-[28rem] overflow-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Fecha</TableHead>
                  <TableHead>Tipo</TableHead>
                  <TableHead>Detalle / Motivo</TableHead>
                  <TableHead className="text-right">Monto</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {listaMovimientos.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={4} className="py-8 text-center text-muted-foreground">
                      {cargandoMovimientos
                        ? "Cargando movimientos contables…"
                        : "Sin movimientos registrados en tu cuenta. Realizá una carga de saldo o participá de una subasta para comenzar."}
                    </TableCell>
                  </TableRow>
                )}
                {listaMovimientos.map((m) => {
                  const config = configuracionTipos[m.tipo] ?? {
                    etiqueta: m.tipo,
                    claseBadge: "bg-muted text-muted-foreground",
                    claseMonto: "text-foreground",
                    prefijo: "",
                  };

                  return (
                    <TableRow key={m.id}>
                      <TableCell className="whitespace-nowrap font-mono text-xs text-muted-foreground">
                        {formatoFecha(m.fechaMovimiento)}
                      </TableCell>
                      <TableCell>
                        <Badge
                          variant="outline"
                          className={cn("whitespace-nowrap font-medium", config.claseBadge)}
                        >
                          {config.etiqueta}
                        </Badge>
                      </TableCell>
                      <TableCell className="max-w-[18rem] text-sm md:max-w-xs">
                        <span className="line-clamp-2" title={m.concepto}>
                          {m.concepto}
                        </span>
                      </TableCell>
                      <TableCell className={cn("text-right font-mono text-sm whitespace-nowrap", config.claseMonto)}>
                        {config.prefijo}
                        {formatoMoneda(m.monto)}
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          </div>
        </div>
      </div>
    </main>
  );
}

function TarjetaEstadistica({
  icono: Icono,
  etiqueta,
  valor,
  tono,
}: {
  icono: React.ComponentType<{ className?: string }>;
  etiqueta: string;
  valor: string;
  tono: "default" | "warning" | "success";
}) {
  return (
    <div className="surface-card rounded-xl p-5">
      <p className="flex items-center gap-2 text-xs uppercase tracking-wider text-muted-foreground">
        <Icono className="size-4" /> {etiqueta}
      </p>
      <p
        className={cn(
          "mt-2 font-display text-3xl font-bold",
          tono === "warning" && "text-warning",
          tono === "success" && "text-success",
          tono === "default" && "text-foreground",
        )}
      >
        {valor}
      </p>
    </div>
  );
}
