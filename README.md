# SubastaYa — Plataforma de Subastas en Tiempo Real

Plataforma integral de subastas en línea con liquidación financiera transaccional, comunicación bidireccional en tiempo real vía WebSockets (SignalR), arquitectura limpia y frontend moderno reactivo.

## Índice

1. [Descripción General](#1-descripción-general)
2. [Estructura del Proyecto y Clean Architecture](#2-estructura-del-proyecto-y-clean-architecture)
3. [Creación de la Solución paso a paso (.NET CLI)](#3-creación-de-la-solución-paso-a-paso-net-cli)
4. [Arquitectura y Funcionamiento Backend](#4-arquitectura-y-funcionamiento-backend)
5. [Arquitectura y Funcionamiento Frontend](#5-arquitectura-y-funcionamiento-frontend)
6. [Comunicación e Integración (REST + SignalR)](#6-comunicación-e-integración-rest--signalr)
7. [Catálogo de Endpoints de la API](#7-catálogo-de-endpoints-de-la-api)
8. [Reglas de Negocio Clave](#8-reglas-de-negocio-clave)
9. [Guía de Ejecución](#9-guía-de-ejecución)
10. [Usuarios y Datos de Prueba](#10-usuarios-y-datos-de-prueba)


## 1. Descripción General

**SubastaYa** permite a los usuarios publicar artículos, realizar ofertas en tiempo real y gestionar sus fondos mediante una billetera virtual integrada. El sistema garantiza:

* Consistencia transaccional completa (ACID) en operaciones monetarias (retención y liberación automática de saldos).
* Auditoría inmutable de eventos críticos.
* Protección contra ofertas de último segundo (Anti-Sniping).
* Cierre y liquidación automática de subastas mediante trabajadores en segundo plano (Background Workers).
* Actualización en vivo de pujas y temporizadores mediante WebSockets con SignalR.


## 2. Estructura del Proyecto y Clean Architecture

El backend sigue los principios de **Clean Architecture (Onion Architecture)**. La regla fundamental es que las dependencias siempre apuntan hacia adentro, hacia el núcleo de Dominio:

```
Subasta-Ya/
├── backend/
│   ├── SubastaYa.slnx                          ← Archivo de solución (contenedor)
│   └── src/
│       ├── SubastaYa.Dominio/                   ← Núcleo puro: Entidades, Enums, Reglas, Excepciones (0 dependencias externas)
│       ├── SubastaYa.Aplicacion/                ← Casos de Uso: Comandos, Consultas, DTOs, Interfaces
│       ├── SubastaYa.Infraestructura/           ← Persistencia: EF Core, Repositorios, SQL Server, SignalR Hubs
│       ├── SubastaYa.API/                       ← Punto de entrada HTTP: Controladores REST, Swagger, CORS
│       ├── SubastaYa.Worker/                    ← Servicio en segundo plano: corre cada 30s y finaliza subastas vencidas
│       └── SubastaYa.Tests/                     ← Pruebas unitarias con xUnit y Moq
└── frontend/                                    ← React 19 + TypeScript + Vite
```


## 3. Creación de la Solución paso a paso (.NET CLI)

A continuación se documenta cómo fue construida la solución desde cero. Estos son los comandos correctos y en el orden correcto ejecutados desde la carpeta `backend/`:

### Paso A — Crear el archivo de solución (.sln)

El `.sln` es solo un contenedor que agrupa proyectos. No es un proyecto en sí mismo:

```
dotnet new sln -n SubastaYa
```

### Paso B — Crear los proyectos (de adentro hacia afuera)

Se crean primero las capas internas (sin dependencias) y luego las externas:

```
# 1. Dominio — Núcleo sin dependencias
dotnet new classlib -o src/SubastaYa.Dominio -f net8.0

# 2. Aplicación — Casos de uso
dotnet new classlib -o src/SubastaYa.Aplicacion -f net8.0

# 3. Infraestructura — Acceso a datos y servicios técnicos
dotnet new classlib -o src/SubastaYa.Infraestructura -f net8.0

# 4. API Web — Punto de entrada HTTP con controladores clásicos
dotnet new webapi -o src/SubastaYa.API -f net8.0 --use-controllers

# 5. Worker — Servicio en segundo plano
dotnet new worker -o src/SubastaYa.Worker -f net8.0

# 6. Tests — Pruebas unitarias con xUnit
dotnet new xunit -o src/SubastaYa.Tests -f net8.0
```

**Explicación de los flags:**
* `-o <ruta>`: Carpeta de salida donde se crea el proyecto.
* `-f net8.0`: Framework destino (.NET 8).
* `--use-controllers`: Genera la Web API con la arquitectura tradicional de Controllers (en vez de Minimal APIs).

### Paso C — Agregar todos los proyectos a la solución

```
dotnet sln add src/SubastaYa.Dominio src/SubastaYa.Aplicacion src/SubastaYa.Infraestructura src/SubastaYa.API src/SubastaYa.Worker src/SubastaYa.Tests
```

### Paso D — Enlazar las referencias entre capas (la parte más importante)

Estos comandos establecen qué capa puede "ver" a cuál, respetando la regla de dependencia de Clean Architecture:

```
# Aplicacion solo conoce a Dominio
dotnet add src/SubastaYa.Aplicacion reference src/SubastaYa.Dominio

# Infraestructura implementa las interfaces de Aplicacion y usa las entidades de Dominio
dotnet add src/SubastaYa.Infraestructura reference src/SubastaYa.Dominio src/SubastaYa.Aplicacion

# API necesita Aplicacion (casos de uso) e Infraestructura (para registrar servicios en DI)
dotnet add src/SubastaYa.API reference src/SubastaYa.Aplicacion src/SubastaYa.Infraestructura

# Worker necesita Aplicacion (manejadores) e Infraestructura (DbContext, repositorios)
dotnet add src/SubastaYa.Worker reference src/SubastaYa.Aplicacion src/SubastaYa.Infraestructura

# Tests referencia las capas que necesita probar
dotnet add src/SubastaYa.Tests reference src/SubastaYa.Dominio src/SubastaYa.Aplicacion src/SubastaYa.Infraestructura
```

Esto genera las etiquetas `<ProjectReference>` dentro de cada archivo `.csproj`, permitiendo usar `using SubastaYa.Dominio.Entidades;` etc.


## 4. Arquitectura y Funcionamiento Backend

El backend está desarrollado sobre **.NET 8 (C#)** siguiendo CQRS (Command Query Responsibility Segregation).

### Componentes Clave

1. **Persistencia y Control de Concurrencia**:
   * Entity Framework Core sobre Microsoft SQL Server.
   * Optimistic Locking: token de concurrencia `RowVersion` (`byte[]`) en la entidad `Subasta` para prevenir condiciones de carrera.
   * Transacciones ACID: cada operación que compromete saldo corre bajo transacciones atómicas.

2. **Auditoría Inmutable**:
   * Servicio `AuditoriaServicio` con tabla `RegistrosAuditoria`.
   * Registra cambios de estado, liquidaciones, extensiones de tiempo y declaraciones desiertas.

3. **Background Services (SubastaYa.Worker)**:
   * **`ProcesadorSubastas`** (ciclo cada 30 segundos):
     * Inspecciona subastas en estado `Activa` cuya `FechaFin` haya expirado.
     * Con ganador: debita saldo retenido del ganador, acredita al vendedor, genera movimientos contables, audita y notifica vía SignalR.
     * Sin pujas: cambia estado a `Desierta`, audita y notifica.
   * **`TemporizadorSubastas`**: emite sincronización de tiempo restante a todos los clientes conectados al Hub.


## 5. Arquitectura y Funcionamiento Frontend

Desarrollado con **React 19**, **TypeScript** y **Vite**.

### Tecnologías Utilizadas
* **Framework**: React 19 + TypeScript.
* **Enrutamiento**: `@tanstack/react-router`.
* **Estilos**: Tailwind CSS + Shadcn UI (Radix UI).
* **Tiempo Real**: `@microsoft/signalr` para WebSockets.
* **Notificaciones**: `sonner` (Toasts).
* **Validaciones**: `zod`.

### Funcionalidades
* **Página de Inicio (`/`)**: Carrusel de subastas destacadas, filtros por categoría, búsqueda en vivo y tarjetas con temporizador sincronizado.
* **Detalle de Subasta (`/subasta/:id`)**: Temporizador en cuenta regresiva, panel de ofertas en vivo por WebSocket, historial de pujas con alias anonimizados (`m***a`).
* **Publicación (`/publicar`)**: Formulario con validación en tiempo real.
* **Billetera (`/billetera`)**: Saldo disponible, saldo retenido, carga de fondos e historial de movimientos.
* **Selector de Usuario**: Barra superior para alternar entre usuarios semilla y simular interacciones.


## 6. Comunicación e Integración (REST + SignalR)

El flujo combina llamadas HTTP REST para consultas/comandos con un canal WebSocket persistente para actualizaciones en vivo:

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
* **`UnirseASubasta(subastaId)`**: Suscribe la conexión al canal de la subasta.
* **`SalirDeSubasta(subastaId)`**: Desuscribe la conexión.
* **Eventos push**:
  * `NuevaPujaRegistrada`: nueva oferta líder.
  * `ExtensionTiempoAntiSniping`: extensión de tiempo por regla anti-sniping.
  * `SubastaFinalizada`: ganador y monto final.
  * `SubastaDesierta`: subasta cerrada sin ofertas.
  * `ActualizacionTemporizador`: sincronización periódica de tiempos.


## 7. Catálogo de Endpoints de la API

La API REST corre por defecto en `http://localhost:5218`. Documentación Swagger disponible en `/swagger`.

### Subastas (`/api/subastas`)
| Método | Ruta | Descripción |
|:---|:---|:---|
| `GET` | `/api/subastas` | Listado general de subastas con estado y categoría. |
| `GET` | `/api/subastas/{id}` | Detalle completo de una subasta por ID. |
| `POST` | `/api/subastas` | Crea una nueva subasta. |

### Pujas (`/api/pujas`)
| Método | Ruta | Descripción |
|:---|:---|:---|
| `GET` | `/api/subastas/{subastaId}/pujas` | Historial de ofertas de una subasta. |
| `POST` | `/api/pujas` | Registra una oferta, reteniendo saldo y evaluando anti-sniping. |

### Billeteras (`/api/billeteras`)
| Método | Ruta | Descripción |
|:---|:---|:---|
| `GET` | `/api/billeteras/usuario/{usuarioId}` | Consulta saldo disponible, retenido y movimientos. |
| `POST` | `/api/billeteras/acreditacion` | Acredita saldo en la billetera del usuario. |

### Categorías y Usuarios
| Método | Ruta | Descripción |
|:---|:---|:---|
| `GET` | `/api/categorias` | Lista de categorías disponibles. |
| `GET` | `/api/usuarios` | Lista de usuarios registrados. |


## 8. Reglas de Negocio Clave

1. **Retención de Saldo en Pujas**: Al ofertar, el monto se descuenta del `SaldoDisponible` y se suma al `SaldoRetenido`. Si otro usuario supera la puja, el saldo del postor anterior se libera inmediatamente.
2. **Anti-Sniping**: Si una puja ingresa dentro de los últimos 2 minutos, la fecha de finalización se extiende automáticamente 2 minutos más.
3. **Restricción de Oferta**: Un vendedor no puede pujar en su propia subasta.
4. **Incremento Mínimo**: Toda nueva oferta debe superar a la actual por al menos el `IncrementoMinimo` configurado.
5. **Liquidación Atómica**: Al vencer la subasta, el Worker debita al ganador y acredita al vendedor dentro de una única transacción ACID.


## 9. Guía de Ejecución

### Requisitos Previos
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.
* [Node.js 18+](https://nodejs.org/) y `npm`.
* [SQL Server LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) o instancia SQL Server local.

### Para su descarga y ejecución:

**Paso 1 — Clonar el repositorio**

```bash
git clone <url-del-repositorio>
cd Subasta-Ya
```

**Paso 2 — Inicializar la base de datos**

Aplicar las migraciones de Entity Framework Core para crear la base de datos `SubastaYaDb_Dev` y cargar los datos semilla:

```
dotnet ef database update --project backend/src/SubastaYa.Infraestructura --startup-project backend/src/SubastaYa.API
```

(Opcional) Ejecutar las pruebas unitarias:

```
dotnet test backend/src/SubastaYa.Tests/SubastaYa.Tests.csproj
```

**Paso 3 — Ejecutar el Backend**

Abrir una terminal en la raíz del proyecto:

```
dotnet run --project backend/src/SubastaYa.API
```

* API REST: `http://localhost:5218`
* Swagger UI: `http://localhost:5218/swagger`
* SignalR Hub: `ws://localhost:5218/hubs/subastas`

(Opcional, en otra terminal, si se ejecuta el Worker como proceso independiente):

```
dotnet run --project backend/src/SubastaYa.Worker
```

**Paso 4 — Ejecutar el Frontend**

Abrir una segunda terminal:

```
cd frontend
npm install
npm run dev
```

* Frontend Web: `http://localhost:8080` (o el puerto asignado por Vite en consola).


## 10. Usuarios y Datos de Prueba

El sistema incluye datos semilla listos para probar todos los casos de uso:

| Usuario | Rol | Alias | Saldo Disp. | Saldo Ret. | Estado / Escenario |
|:---|:---|:---|:---|:---|:---|
| **Id: 5** | Comprador | `ana_vip` | $44.000 | $6.000 | Líder actual en la subasta *Notebook Gamer MSI*. |
| **Id: 3** | Comprador | `maria_compradora` | $10.000 | $8.500 | Líder actual en la subasta *Cuadro Óleo Original*. |
| **Id: 4** | Comprador | `pedro_postor` | $200 | $0 | Usuario con saldo insuficiente para validar rechazo de pujas. |
| **Id: 2** | Vendedor | `juan_vendedor` | $5.000 | $0 | Propietario de las subastas publicadas. |
| **Id: 1** | Administrador | `admin` | $0 | $0 | Perfil de administración del sistema. |
