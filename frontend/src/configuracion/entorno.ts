/**
 * Configuracion de variables de entorno del sistema.
 * Centraliza las rutas de conexion con el backend de SubastaYa.
 */

const rutaApiPorDefecto = "http://localhost:5218/api";
const rutaHubPorDefecto = "http://localhost:5218/hubs/subastas";

export const configuracionEntorno = {
  urlApi: import.meta.env.VITE_API_URL || rutaApiPorDefecto,
  urlHubSubastas: import.meta.env.VITE_HUB_URL || rutaHubPorDefecto,
};
