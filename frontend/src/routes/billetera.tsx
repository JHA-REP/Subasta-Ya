import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { Lock, PiggyBank, Wallet } from "lucide-react";
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
import type { MovimientoSesion, TipoMovimiento } from "@/tipos/subastaTipos";

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

const etiquetasTipos: Record<TipoMovimiento, string> = {
  Carga: "Ingreso",
  Retencion: "Retención",
  Liberacion: "Liberación",
  Debito: "Débito",
  Credito: "Recaudación",
};

function Billetera() {
  const { usuarioActual, billeteraActual, eventoActualizacionBilletera } = useUsuario();
  const [montoEntrada, setMontoEntrada] = useState("");
  const [procesandoAcreditacion, setProcesandoAcreditacion] = useState(false);
  const [listaMovimientos, setListaMovimientos] = useState<MovimientoSesion[]>([]);

  const claveMovimientos = `subastaya:movimientos_${usuarioActual?.id ?? "invitado"}`;

  useEffect(() => {
    try {
      const guardados = localStorage.getItem(claveMovimientos);
      if (guardados) {
        setListaMovimientos(JSON.parse(guardados));
      } else {
        setListaMovimientos([]);
      }
    } catch {
      setListaMovimientos([]);
    }
  }, [claveMovimientos]);

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

      const nuevoMovimiento: MovimientoSesion = {
        id: Math.random().toString(36).slice(2, 10),
        fecha: Date.now(),
        tipo: "Carga",
        detalle: "Acreditación de saldo en cuenta",
        monto: valor,
      };

      const nuevaLista = [nuevoMovimiento, ...listaMovimientos];
      setListaMovimientos(nuevaLista);
      localStorage.setItem(claveMovimientos, JSON.stringify(nuevaLista));

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
            Carga simulada para pruebas: actualiza tu saldo real en la base de datos de SubastaYa.
          </p>
        </form>

        <div className="surface-card rounded-xl p-5">
          <h2 className="text-lg font-semibold">Historial de movimientos de sesión</h2>
          <div className="mt-4 max-h-[28rem] overflow-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Fecha</TableHead>
                  <TableHead>Tipo</TableHead>
                  <TableHead>Detalle</TableHead>
                  <TableHead className="text-right">Monto</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {listaMovimientos.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={4} className="text-center text-muted-foreground">
                      Sin movimientos registrados en esta sesión. Realizá una carga de saldo para comenzar.
                    </TableCell>
                  </TableRow>
                )}
                {listaMovimientos.map((m) => (
                  <TableRow key={m.id}>
                    <TableCell className="whitespace-nowrap font-mono text-xs text-muted-foreground">
                      {formatoFecha(m.fecha)}
                    </TableCell>
                    <TableCell>
                      <Badge variant={m.tipo === "Debito" ? "destructive" : "secondary"}>
                        {etiquetasTipos[m.tipo] || m.tipo}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-sm">{m.detalle}</TableCell>
                    <TableCell
                      className={cn(
                        "text-right font-mono text-sm",
                        m.tipo === "Carga" || m.tipo === "Credito"
                          ? "text-success"
                          : m.tipo === "Debito"
                            ? "text-destructive"
                            : "text-muted-foreground",
                      )}
                    >
                      {m.tipo === "Carga" || m.tipo === "Credito" ? "+" : ""}
                      {formatoMoneda(m.monto)}
                    </TableCell>
                  </TableRow>
                ))}
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
