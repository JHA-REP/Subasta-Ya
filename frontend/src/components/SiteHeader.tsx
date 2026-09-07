import { Link } from "@tanstack/react-router";
import { Gavel, Wallet, PlusCircle, LayoutGrid, User, Users } from "lucide-react";
import { useUsuario } from "@/contextos/contextoUsuario";
import { formatoMoneda } from "@/utilidades/formatoMonedaYFecha";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

const enlacesNavegacion = [
  { ruta: "/", etiqueta: "Catálogo", icono: LayoutGrid },
  { ruta: "/publicar", etiqueta: "Publicar", icono: PlusCircle },
  { ruta: "/billetera", etiqueta: "Billetera", icono: Wallet },
  { ruta: "/actividad", etiqueta: "Mis actividades", icono: User },
] as const;

export function SiteHeader() {
  const {
    usuarioActual,
    listaUsuarios,
    billeteraActual,
    eventoSeleccionUsuario,
  } = useUsuario();

  const saldoDisponible = billeteraActual?.saldoDisponible ?? 0;

  return (
    <header className="sticky top-0 z-40 border-b border-border bg-background/85 backdrop-blur-xl">
      <div className="mx-auto flex max-w-7xl flex-wrap items-center gap-x-6 gap-y-3 px-4 py-3">
        <Link to="/" className="flex items-center gap-2">
          <span className="flex size-9 items-center justify-center rounded-lg bg-primary text-primary-foreground">
            <Gavel className="size-5" aria-hidden />
          </span>
          <span className="font-display text-lg font-bold tracking-tight">
            Subasta<span className="text-ember">Ya</span>
          </span>
        </Link>

        <nav className="order-3 flex w-full items-center gap-1 overflow-x-auto md:order-2 md:w-auto">
          {enlacesNavegacion.map(({ ruta, etiqueta, icono: Icono }) => (
            <Link
              key={ruta}
              to={ruta}
              activeOptions={{ exact: ruta === "/" }}
              activeProps={{ className: "bg-secondary text-foreground" }}
              className="inline-flex shrink-0 items-center gap-1.5 rounded-md px-3 py-2 text-sm font-medium text-muted-foreground transition-colors hover:bg-secondary hover:text-foreground"
            >
              <Icono className="size-4" aria-hidden />
              {etiqueta}
            </Link>
          ))}
        </nav>

        <div className="order-2 ml-auto flex items-center gap-3 md:order-3">
          {/* Selector de usuario activo para pruebas */}
          {listaUsuarios.length > 0 && usuarioActual && (
            <div className="flex items-center gap-1.5">
              <Users className="size-4 text-muted-foreground hidden sm:inline" aria-hidden />
              <Select
                value={String(usuarioActual.id)}
                onValueChange={(valor) => {
                  const seleccionado = listaUsuarios.find((u) => u.id === Number(valor));
                  if (seleccionado) eventoSeleccionUsuario(seleccionado);
                }}
              >
                <SelectTrigger className="h-9 w-36 sm:w-44 text-xs font-medium">
                  <SelectValue placeholder="Usuario activo" />
                </SelectTrigger>
                <SelectContent align="end">
                  {listaUsuarios.map((u) => (
                    <SelectItem key={u.id} value={String(u.id)} className="text-xs">
                      {u.alias} ({u.rol})
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          )}

          {/* Saldo disponible real en billetera */}
          <Link
            to="/billetera"
            className="rounded-lg border border-border bg-card px-3 py-1.5 text-right transition-colors hover:border-primary/50"
          >
            <p className="text-[10px] uppercase tracking-wider text-muted-foreground">Disponible</p>
            <p className="font-mono text-sm font-semibold text-success">
              {formatoMoneda(saldoDisponible)}
            </p>
          </Link>
        </div>
      </div>
    </header>
  );
}
