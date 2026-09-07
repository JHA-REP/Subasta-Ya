# SubastaYa — Plataforma de Subastas en Tiempo Real

Plataforma integral de subastas en línea con liquidación financiera transaccional, comunicación bidireccional en tiempo real vía WebSockets (SignalR), arquitectura limpia (Clean Architecture) y frontend reactivo moderno.

---

## Índice

1. [Descripción General](#descripción-general)
2. [Arquitectura y Funcionamiento Backend](#arquitectura-y-funcionamiento-backend)
3. [Arquitectura y Funcionamiento Frontend](#arquitectura-y-funcionamiento-frontend)
4. [Comunicación e Integración (REST + SignalR)](#comunicación-e-integración-rest--signalr)
5. [Catálogo de Endpoints de la API](#catálogo-de-endpoints-de-la-api)
6. [Reglas de Negocio Clave](#reglas-de-negocio-clave)
7. [Guía Paso a Paso para la Ejecución](#guía-paso-a-paso-para-la-ejecución)
8. [Usuarios y Datos de Prueba](#usuarios-y-datos-de-prueba)

---

## Descripción General

**SubastaYa** permite a los usuarios publicar artículos, realizar ofertas en tiempo real y gestionar sus fondos mediante una billetera virtual integrada. El sistema garantiza consistencia transaccional completa (ACID) en operaciones monetarias, auditoría inmutable de eventos críticos, protección contra ofertas de último segundo (*anti-sniping*) y cierre automático de subastas mediante trabajadores en segundo plano (*Background Workers*).

---

## Arquitectura y Funcionamiento Backend

El backend está desarrollado sobre **.NET 9 (C#)** siguiendo los principios de **Clean Architecture (Onion Architecture)** y el patrón **CQRS** (Command Query Responsibility Segregation). Toda la nomenclatura del código respeta convención en castellano y nombres basados en sustantivos.

### Capas del Sistema

```
backend/src/
├── SubastaYa.Dominio          # Núcleo: Entidades, Objetos de Valor, Enums, Excepciones
├── SubastaYa.Aplicacion       # Casos de Uso (Comandos, Consultas, Manejadores, DTOs, Mapeos)
├── SubastaYa.Infraestructura   # Persistencia EF Core, Migraciones, Repositorios, SignalR, Workers
└── SubastaYa.API              # Controladores REST, Middleware global, Configuración Swagger y CORS
```

### Componentes Clave

1. **Persistencia y Control de Concurrencia**:
   * **Entity Framework Core** sobre **Microsoft SQL Server**.
   * **Optimistic Locking**: La entidad `Subasta` incorpora un token de concurrencia `RowVersion` (`byte[]`) que previene condiciones de carrera o doble liquidación concurrente.
   * **Transacciones ACID**: Cada operación que compromete saldo (pujas, liquidación, acreditaciones) corre bajo transacciones atómicas con aislamiento de base de datos.

2. **Auditoría Inmutable**:
   * Servicio `AuditoriaServicio` con tabla `RegistrosAuditoria`.
   * Registra cambios de estado de subastas, liquidaciones ejecutadas por el worker, extensiones de tiempo y declaraciones desiertas.

3. **Background Services (Trabajadores en Segundo Plano)**:
   * **`ProcesadorSubastas`** (ciclo cada 30 s):
     * Inspecciona subastas en estado `Activa` cuya `FechaFin` haya expirado.
     * **Con ganador**: Debita el saldo retenido del ganador, acredita al vendedor, genera movimientos contables, audita y notifica a clientes conectados.
     * **Sin pujas**: Cambia el estado a `Desierta`, audita y emite notificación.
   * **`TemporizadorSubastas`** (ciclo cada 10 s):
     * Emite el estado de tiempo restante y sincronización a todos los clientes suscritos al Hub de SignalR.

---

## Arquitectura y Funcionamiento Frontend

El frontend está desarrollado con **React 19**, **TypeScript** y **Vite**, empaquetado con diseño moderno y soporte en tiempo real.

### Tecnologías Utilizadas

* **Framework**: React 19 + TypeScript.
* **Enrutamiento**: `@tanstack/react-router`.
* **Estilos**: Tailwind CSS + Shadcn UI (Radix UI primitives).
* **Tiempo Real**: `@microsoft/signalr` para WebSockets bidireccionales.
* **Notificaciones**: `sonner` (Toasts interactivos).
* **Validaciones**: `zod` para validación estricta de esquemas de formulario.

### Funcionalidades de la Interfaz

* **Página de Inicio (`/`)**: Carrusel de subastas destacadas (*Hot Carousel*), filtros por categoría, búsqueda en vivo y tarjetas de subastas con temporizador sincronizado.
* **Detalle de Subasta (`/subasta/:id`)**:
  * Temporizador dinámico en cuenta regresiva con badge de alerta cuando restan menos de 2 minutos (*Anti-Sniping*).
  * Panel de ofertas en vivo conectado por WebSocket: si otro usuario puja, el precio se actualiza de inmediato sin recargar la página.
  * Historial de pujas con alias anonimizados (`m***a`, `a***p`).
* **Publicación de Subastas (`/publicar`)**:
  * Formulario con validación en tiempo real para título, descripción, imagen, categoría, precio inicial, incremento mínimo y ventana temporal.
* **Billetera Virtual (`/billetera`)**:
  * Visualización de Saldo Disponible y Saldo Retenido por ofertas activas.
  * Formulario para acreditación inmediata de fondos.
  * Historial completo de movimientos contables (Cargas, Retenciones, Liberaciones, Débitos, Créditos).
* **Actividad en Vivo (`/actividad`)**:
  * Monitor global de eventos del sistema (nuevas subastas, extensiones anti-sniping, cierres y liquidaciones).
* **Selector de Usuario Activo**:
  * En la barra de navegación superior permite alternar instantáneamente entre los distintos usuarios del sistema (`ana_vip`, `maria_compradora`, `pedro_postor`, `juan_vendedor`) para simular interacciones entre compradores y vendedores sin cerrar sesión.

---

## Comunicación e Integración (REST + SignalR)

El flujo de comunicación combina llamadas síncronas HTTP REST para consultas/comandos puntuales con un canal persistente de WebSocket para actualizaciones en vivo:

```
┌────────────────────────────────────────────────────────┐
│                   Frontend (React/TS)                  │
└───────────────┬────────────────────────▲───────────────┘
                │                        │
     HTTP REST  │             SignalR    │ Eventos push
   POST/GET/PUT │            WebSockets  │ (NuevaPuja, Extension,
                ▼                        │  SubastaFinalizada)
┌────────────────────────────────────────┴───────────────┐
│               Backend ASP.NET Core API                 │
│  ┌──────────────────────┐    ┌──────────────────────┐  │
│  │ Controladores REST   │    │ SubastaHub (SignalR) │  │
│  └──────────┬───────────┘    └──────────▲───────────┘  │
│             │                           │              │
│             ▼                           │              │
│       Manejadores CQRS ───────── NotificadorSignalR    │
│             │                                          │
│             ▼                                          │
│    Entity Framework Core ─── SQL Server (SubastaYaDb)  │
└────────────────────────────────────────────────────────┘
```

### Eventos SignalR (`/hubs/subastas`)

* **`UnirseASubasta(subastaId)`**: Suscribe la conexión a un grupo exclusivo de la subasta.
* **`SalirDeSubasta(subastaId)`**: Desuscribe la conexión al salir de la pantalla.
* **Eventos recibidos por el cliente**:
  * `NuevaPujaRegistrada`: Notifica nueva oferta líder, monto y postor anonimizado.
  * `ExtensionTiempoAntiSniping`: Notifica si la subasta se extendió al recibir una puja cerca del cierre.
  * `SubastaFinalizada`: Notifica ganador y monto final adjudicado.
  * `SubastaDesierta`: Notifica que la subasta cerró sin postores.
  * `ActualizacionTemporizador`: Sincronización periódica de tiempos de las subastas activas.

---

## Catálogo de Endpoints de la API

La API REST corre por defecto en `http://localhost:5218`. La documentación interactiva Swagger está disponible en `/swagger`.

### 1. Subastas (`/api/subastas`)
| Método | Ruta | Descripción |
| :--- | :--- | :--- |
| `GET` | `/api/subastas` | Listado general de subastas con estado y categoría. |
| `GET` | `/api/subastas/{id}` | Detalle completo de una subasta por su identificador. |
| `POST` | `/api/subastas` | Crea una nueva subasta (`titulo`, `descripcion`, `precioInicial`, `incrementoMinimo`, `imagenUrl`, `fechaInicio`, `fechaFin`, `vendedorId`, `categoriaId`). |

### 2. Pujas (`/api/pujas`)
| Método | Ruta | Descripción |
| :--- | :--- | :--- |
| `GET` | `/api/subastas/{subastaId}/pujas` | Historial de ofertas de una subasta específica. |
| `POST` | `/api/pujas` | Registra una nueva oferta (`subastaId`, `postorId`, `monto`). Realiza validación de saldo, retención de fondos, liberación del líder anterior y evaluación anti-sniping. |

### 3. Billeteras (`/api/billeteras`)
| Método | Ruta | Descripción |
| :--- | :--- | :--- |
| `GET` | `/api/billeteras/usuario/{usuarioId}` | Consulta saldo disponible, retenido y movimientos contables del usuario. |
| `POST` | `/api/billeteras/acreditacion` | Acredita saldo en la billetera (`billeteraId`, `monto`, `concepto`). |

### 4. Categorías (`/api/categorias`)
| Método | Ruta | Descripción |
| :--- | :--- | :--- |
| `GET` | `/api/categorias` | Lista de categorías disponibles (Electrónica, Hogar, Deportes, Arte). |
| `GET` | `/api/categorias/{id}` | Detalle de una categoría específica. |

### 5. Usuarios (`/api/usuarios`)
| Método | Ruta | Descripción |
| :--- | :--- | :--- |
| `GET` | `/api/usuarios` | Lista de usuarios registrados en el sistema. |
| `GET` | `/api/usuarios/{id}` | Consulta de perfil y rol de usuario. |

---

## Reglas de Negocio Clave

1. **Retención de Saldo en Pujas**: Al ofertar, el monto ofertado se descuenta del `SaldoDisponible` y se suma al `SaldoRetenido`. Si otro usuario supera la puja, el saldo del postor anterior se libera inmediatamente devolviéndose a su `SaldoDisponible`.
2. **Anti-Sniping**: Si una puja ingresa dentro de los últimos 2 minutos de la subasta, la fecha de finalización se extiende automáticamente por 2 minutos adicionales para permitir contraofertas justas.
3. **Restricción de Oferta**: Un vendedor no puede pujar en su propia subasta.
4. **Incremento Mínimo**: Toda nueva oferta debe superar a la oferta actual por al menos el `IncrementoMinimo` configurado.
5. **Liquidación Atómica**: Al vencer la subasta, el worker confirma el débito del saldo retenido del ganador y lo acredita en la billetera del vendedor dentro de una única transacción ACID.

---

## Guía Paso a Paso para la Ejecución

### Requisitos Previos
* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
* [Node.js 18+](https://nodejs.org/) y `npm` instalados.
* [SQL Server LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) (incluido habitualmente con Visual Studio o instalable individualmente).

---

### Paso 1: Clonar el Repositorio
```bash
git clone <url-del-repositorio>
cd Subasta-Ya
```

---

### Paso 2: Inicializar la Base de Datos
Desde la raíz del proyecto, aplicar las migraciones de Entity Framework Core para crear la base de datos `SubastaYaDb_Dev` y cargar los datos semilla iniciales:

```powershell
dotnet ef database update --project backend/src/SubastaYa.Infraestructura --startup-project backend/src/SubastaYa.API
```

*(Opcional) Si querés correr las pruebas unitarias para validar que todo esté en orden:*
```powershell
dotnet test backend/src/SubastaYa.Tests/SubastaYa.Tests.csproj
```

---

### Paso 3: Ejecutar el Backend API
Abrir una terminal y correr el proyecto API:

```powershell
dotnet run --project backend/src/SubastaYa.API
```

* La API quedará disponible en: `http://localhost:5218`
* Documentación Swagger UI: `http://localhost:5218/swagger`
* Hub de SignalR: `ws://localhost:5218/hubs/subastas`

---

### Paso 4: Instalar Dependencias y Ejecutar el Frontend
Abrir una **segunda terminal** en la carpeta `frontend`:

```powershell
cd frontend
npm install
npm run dev
```

* La aplicación web iniciará en: `http://localhost:8080` (o el puerto asignado por Vite en consola).

---

## Usuarios y Datos de Prueba

El sistema incluye datos semilla listos para probar todos los casos de uso:

| Usuario | Rol | Alias | Saldo Disp. | Saldo Ret. | Estado / Escenario |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Id: 5** | Comprador | `ana_vip` | $44.000 | $6.000 | Líder actual en la subasta *Notebook Gamer MSI*. |
| **Id: 3** | Comprador | `maria_compradora` | $10.000 | $8.500 | Líder actual en la subasta *Cuadro Óleo Original*. |
| **Id: 4** | Comprador | `pedro_postor` | $200 | $0 | Usuario con saldo insuficiente para validar rechazo de pujas. |
| **Id: 2** | Vendedor | `juan_vendedor` | $5.000 | $0 | Propietario de las subastas publicadas. |
| **Id: 1** | Administrador | `admin` | $0 | $0 | Perfil de administración del sistema. |
