import {
  createContext,
  useContext,
  useState,
  useEffect,
  type ReactNode,
} from "react";
import type { Usuario, Billetera } from "@/tipos/subastaTipos";
import { servicioUsuarios } from "@/servicios/servicioUsuarios";
import { servicioBilleteras } from "@/servicios/servicioBilleteras";

interface TipoContextoUsuario {
  usuarioActual: Usuario | null;
  listaUsuarios: Usuario[];
  billeteraActual: Billetera | null;
  cargando: boolean;
  eventoSeleccionUsuario: (usuario: Usuario) => void;
  eventoActualizacionBilletera: () => Promise<void>;
}

const claveAlmacenamientoUsuario = "subastaya:usuario_activo_id";

const ContextoUsuario = createContext<TipoContextoUsuario | undefined>(undefined);

export function ProveedorUsuario({ children }: { children: ReactNode }) {
  const [listaUsuarios, setListaUsuarios] = useState<Usuario[]>([]);
  const [usuarioActual, setUsuarioActual] = useState<Usuario | null>(null);
  const [billeteraActual, setBilleteraActual] = useState<Billetera | null>(null);
  const [cargando, setCargando] = useState(true);

  // Carga inicial de usuarios del backend
  useEffect(() => {
    let montado = true;

    const cargaInicial = async () => {
      try {
        const usuarios = await servicioUsuarios.listado();
        if (!montado) return;
        setListaUsuarios(usuarios);

        // Seleccion del usuario guardado o por defecto maria_compradora (Id 3)
        const idGuardado = localStorage.getItem(claveAlmacenamientoUsuario);
        const usuarioEncontrado = idGuardado
          ? usuarios.find((u) => u.id === Number(idGuardado))
          : usuarios.find((u) => u.id === 3) || usuarios[0];

        if (usuarioEncontrado) {
          setUsuarioActual(usuarioEncontrado);
        }
      } catch {
        /* Fallback si el backend aun no responde */
        const usuarioRespaldo: Usuario = {
          id: 3,
          alias: "maria_compradora",
          email: "maria@mail.com",
          rol: "Comprador",
          fechaRegistro: new Date().toISOString(),
        };
        if (montado) {
          setListaUsuarios([usuarioRespaldo]);
          setUsuarioActual(usuarioRespaldo);
        }
      } finally {
        if (montado) setCargando(false);
      }
    };

    cargaInicial();

    return () => {
      montado = false;
    };
  }, []);

  // Carga de billetera al cambiar de usuario
  useEffect(() => {
    let montado = true;

    const cargaBilletera = async () => {
      if (!usuarioActual) return;
      try {
        const billetera = await servicioBilleteras.detallePorUsuario(usuarioActual.id);
        if (montado) setBilleteraActual(billetera);
      } catch {
        if (montado) {
          setBilleteraActual({
            id: usuarioActual.id,
            usuarioId: usuarioActual.id,
            usuarioAlias: usuarioActual.alias,
            saldo: 0,
            saldoRetenido: 0,
            saldoDisponible: 0,
          });
        }
      }
    };

    cargaBilletera();

    return () => {
      montado = false;
    };
  }, [usuarioActual]);

  const eventoSeleccionUsuario = (usuario: Usuario) => {
    setUsuarioActual(usuario);
    localStorage.setItem(claveAlmacenamientoUsuario, String(usuario.id));
  };

  const eventoActualizacionBilletera = async () => {
    if (!usuarioActual) return;
    try {
      const billetera = await servicioBilleteras.detallePorUsuario(usuarioActual.id);
      setBilleteraActual(billetera);
    } catch {
      /* Silencioso */
    }
  };

  return (
    <ContextoUsuario.Provider
      value={{
        usuarioActual,
        listaUsuarios,
        billeteraActual,
        cargando,
        eventoSeleccionUsuario,
        eventoActualizacionBilletera,
      }}
    >
      {children}
    </ContextoUsuario.Provider>
  );
}

export function useUsuario(): TipoContextoUsuario {
  const contexto = useContext(ContextoUsuario);
  if (!contexto) {
    throw new Error("useUsuario debe ser utilizado dentro de un ProveedorUsuario");
  }
  return contexto;
}
