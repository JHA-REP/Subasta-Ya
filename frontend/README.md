# Bid Haven

"SubastaYa" es una plataforma web de subastas en tiempo real y comercio electrónico

visual atractiva y fluida para compradores y vendedores con animacion para cuando esta por finalizar una subasta, con ampliacion a 10 segungos cada vez que alguien puje dentro de los ultimos 10 segundos

La aplicación web debe contar con una interfaz responsiva, intuitiva y visualmente cuidada.

Se espera  una experiencia de usuario interactiva dividida en

los siguientes módulos:

Módulo 1: Catálogo y Exploración de Subastas

● Filtros y Búsqueda: El usuario debe poder explorar subastas filtrando por:

○ Estado: Activas (en curso), Próximas (programadas para iniciar a futuro) y

Finalizadas.

○ Categoría: Ej. Tecnología, Coleccionables, Vehículos, Arte, etc.

○ Rango de precios y ordenamiento (por menor tiempo restante o mayor

puja).

 

● Cards de Producto Informativas: Cada tarjeta en el catálogo debe mostrar imagen

referencial, título, categoría, oferta más alta actual, cantidad de ofertas realizadas y

un contador regresivo visible.

Módulo 2: Creación y Publicación de Subastas (Vendedor)

● Formulario Interactivo de Publicación:

○ Datos del producto: Título, descripción detallada, URL de imagen y categoría.

○ Configuración económica: Precio base inicial y monto mínimo de incremento

por puja.

○ Ventana temporal: Fecha/hora de inicio y fecha/hora de finalización.

● Validaciones en pantalla: La fecha de finalización debe ser posterior a la de inicio;

el incremento mínimo y el precio base deben ser valores positivos coherentes.

Módulo 3: Sala de Subasta en Vivo (Live Bidding Room)

Es la vista principal de interacción en tiempo real:

● Temporizador en Vivo: Reloj visual con cuenta regresiva. Si el tiempo entra en

zona crítica (último minuto), debe cambiar de color (ej. amarillo/rojo) para alertar a

los postores.

● Historial de Ofertas en Pantalla: Lista cronológica con las últimas pujas (monto,

usuario anonimizado/seudónimo y hora exacta de la oferta).

● Consola de Puja Dinámica:

○ Sugerencia automática del próximo valor a ofertar: $\text{Puja Actual} +

\text{Incremento Mínimo}$.

○ Posibilidad de ingresar un monto personalizado superior.

○ Indicador visual inmediato de estado:

■ Liderando: Si la oferta más alta pertenece al usuario actual.

■ Superado (Outbid): Si otro usuario acaba de enviar una oferta

superior.

● Feedback y Alertas: Mensajes visuales claros (toasts / modales) cuando se

confirma una puja, cuando los fondos son insuficientes o si el tiempo de la subasta

fue extendido por regla anti-sniping.

Módulo 4: Billetera Virtual y Gestión de Fondos

● Panel de Saldo: Visualización clara de tres métricas financieras:

1. Saldo Total: Fondos totales depositados en la cuenta.

2. Saldo Retenido / En Garantía: Monto actualmente bloqueado en subastas

activas donde el usuario es el postor líder.

3. Saldo Disponible: $\text{Saldo Total} - \text{Saldo Retenido}$ (único dinero

disponible para nuevas pujas o retiros).

● Carga de Saldo Simulada: Formulario para acreditar saldo ficticio a la billetera.

● Historial de Movimientos: Tabla con el detalle de ingresos, retenciones por ofertas,

liberaciones por superación y débitos finales por subastas ganadas.

Módulo 5: Panel de Usuario ("Mis Actividades")

● Pestaña "Mis Compras / Pujas": Listado de subastas donde participó, indicando si

ganó el producto o si la subasta sigue abierta.

● Pestaña "Mis Publicaciones": Subastas creadas por el vendedor, con métricas de

recaudación y estado de adjudicación.

This project was built with [Lovable](https://lovable.dev).

## Build with Lovable

Continue developing this project in the [Lovable editor](https://lovable.dev/projects/4ab7d65a-c762-4bad-a979-07d194a870f9).

- **Ship faster**: describe what you want to build and Lovable handles the code.
- **Stay in sync**: every change made in Lovable is committed straight to this repository.
- **Full ownership**: this code is yours. Push to `main` on GitHub and your changes sync back into Lovable, ready for your next prompt.

## Development

Prefer working locally? You need Node.js and npm — [install with nvm](https://github.com/nvm-sh/nvm#installing-and-updating).

```sh
git clone <this-repository-url>
cd <repository-name>
npm i
npm run dev
```
