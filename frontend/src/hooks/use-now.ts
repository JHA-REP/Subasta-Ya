import { useEffect, useState } from "react";

/** Reloj compartido que refresca cada segundo para cuentas regresivas en pantalla. */
export function useNow(): number {
  const [ahora, setAhora] = useState(() => Date.now());

  useEffect(() => {
    const temporizador = setInterval(() => {
      setAhora(Date.now());
    }, 1000);

    return () => {
      clearInterval(temporizador);
    };
  }, []);

  return ahora;
}
