using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Dominio.Entidades;
using Dominio.Enumeraciones;

namespace Infraestructura.Datos;

/// <summary>
/// Datos semilla para pruebas de todos los escenarios requeridos:
/// - Subasta activa estándar
/// - Subasta activa crítica
/// - Subasta próxima
/// - Subasta vencida con ganador
/// - Subasta vencida desierta
/// - Billetera con saldo retenido
/// - Usuario sin fondos
/// </summary>
public static class DatosSemilla
{
    // Fecha de referencia para datos semilla reproducibles y sincronizados con el estado de desarrollo actual
    private static readonly DateTime FechaReferencia = new(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc);

    public static void Aplicacion(ModelBuilder constructor)
    {
        SemillaUsuarios(constructor);
        SemillaBilleteras(constructor);
        SemillaCategorias(constructor);
        SemillaSubastas(constructor);
        SemillaPujas(constructor);
        SemillaMovimientos(constructor);
    }

    private static string HashClave(string clave)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(clave));
        return Convert.ToBase64String(bytes);
    }

    private static void SemillaUsuarios(ModelBuilder constructor)
    {
        constructor.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                Alias = "admin",
                Email = "admin@com",
                ClaveHash = HashClave("Admin123!"),
                Rol = RolUsuario.Administrador,
                FechaRegistro = FechaReferencia.AddDays(-30)
            },
            new Usuario
            {
                Id = 2,
                Alias = "juan_vendedor",
                Email = "juan@mail.com",
                ClaveHash = HashClave("Juan123!"),
                Rol = RolUsuario.Vendedor,
                FechaRegistro = FechaReferencia.AddDays(-25)
            },
            new Usuario
            {
                Id = 3,
                Alias = "maria_compradora",
                Email = "maria@mail.com",
                ClaveHash = HashClave("Maria123!"),
                Rol = RolUsuario.Comprador,
                FechaRegistro = FechaReferencia.AddDays(-20)
            },
            new Usuario
            {
                Id = 4,
                Alias = "pedro_postor",
                Email = "pedro@mail.com",
                ClaveHash = HashClave("Pedro123!"),
                Rol = RolUsuario.Comprador,
                FechaRegistro = FechaReferencia.AddDays(-18)
            },
            new Usuario
            {
                Id = 5,
                Alias = "ana_vip",
                Email = "ana@mail.com",
                ClaveHash = HashClave("Ana123!"),
                Rol = RolUsuario.Comprador,
                FechaRegistro = FechaReferencia.AddDays(-15)
            }
        );
    }

    private static void SemillaBilleteras(ModelBuilder constructor)
    {
        constructor.Entity<Billetera>().HasData(
            new Billetera
            {
                Id = 1,
                UsuarioId = 1,
                SaldoDisponible = 0m,
                SaldoRetenido = 0m
            },
            new Billetera
            {
                Id = 2,
                UsuarioId = 2,
                SaldoDisponible = 5000m,
                SaldoRetenido = 0m
            },
            // maria_compradora: billetera con saldo retenido ✓
            new Billetera
            {
                Id = 3,
                UsuarioId = 3,
                SaldoDisponible = 10000m,
                SaldoRetenido = 8500m
            },
            // pedro_postor: usuario sin fondos ✓
            new Billetera
            {
                Id = 4,
                UsuarioId = 4,
                SaldoDisponible = 200m,
                SaldoRetenido = 0m
            },
            new Billetera
            {
                Id = 5,
                UsuarioId = 5,
                SaldoDisponible = 44000m,
                SaldoRetenido = 6000m
            }
        );
    }

    private static void SemillaCategorias(ModelBuilder constructor)
    {
        constructor.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Electrónica", Descripcion = "Dispositivos y gadgets electrónicos" },
            new Categoria { Id = 2, Nombre = "Hogar", Descripcion = "Artículos para el hogar y decoración" },
            new Categoria { Id = 3, Nombre = "Deportes", Descripcion = "Equipamiento y artículos deportivos" },
            new Categoria { Id = 4, Nombre = "Arte", Descripcion = "Obras de arte, pinturas y esculturas" }
        );
    }

    private static void SemillaSubastas(ModelBuilder constructor)
    {
        var subastas = new List<Subasta>
        {
            // Subasta activa estándar ✓
            new Subasta
            {
                Id = 1,
                Titulo = "Notebook Gamer MSI",
                Descripcion = "Notebook gamer MSI con RTX 4060, 16GB RAM, 512GB SSD. Estado impecable.",
                PrecioInicial = 5000m,
                PrecioActual = 6000m,
                IncrementoMinimo = 100m,
                ImagenUrl = "https://images.unsplash.com/photo-1603302576837-37561b2e2302",
                CategoriaId = 1,
                VendedorId = 2,
                Estado = EstadoSubasta.Activa,
                FechaInicio = FechaReferencia.AddDays(-2),
                FechaFin = FechaReferencia.AddDays(5)
            },
            // Subasta activa crítica (próxima a vencer) ✓
            new Subasta
            {
                Id = 2,
                Titulo = "Cuadro Óleo Original",
                Descripcion = "Cuadro al óleo original de artista emergente. Técnica mixta sobre lienzo 80x60.",
                PrecioInicial = 8000m,
                PrecioActual = 8500m,
                IncrementoMinimo = 200m,
                ImagenUrl = "https://images.unsplash.com/photo-1579783902614-a3fb3927b675",
                CategoriaId = 4,
                VendedorId = 2,
                Estado = EstadoSubasta.Activa,
                FechaInicio = FechaReferencia.AddDays(-3),
                FechaFin = FechaReferencia.AddHours(8)
            },
            // Subasta próxima ✓
            new Subasta
            {
                Id = 3,
                Titulo = "Bicicleta Montaña R29",
                Descripcion = "Bicicleta de montaña rodado 29, cuadro de aluminio, 21 velocidades.",
                PrecioInicial = 3000m,
                PrecioActual = 3000m,
                IncrementoMinimo = 50m,
                ImagenUrl = "https://images.unsplash.com/photo-1485965120184-e220f721d03e",
                CategoriaId = 3,
                VendedorId = 2,
                Estado = EstadoSubasta.Pendiente,
                FechaInicio = FechaReferencia.AddDays(2),
                FechaFin = FechaReferencia.AddDays(9)
            },
            // Subasta vencida con ganador ✓
            new Subasta
            {
                Id = 4,
                Titulo = "Smart TV 55 Pulgadas",
                Descripcion = "Smart TV LED 55 pulgadas 4K UHD con sistema operativo integrado.",
                PrecioInicial = 4000m,
                PrecioActual = 4500m,
                IncrementoMinimo = 100m,
                ImagenUrl = "https://images.unsplash.com/photo-1593359677879-a4bb92f829d1",
                CategoriaId = 1,
                VendedorId = 2,
                Estado = EstadoSubasta.Finalizada,
                GanadorId = 5,
                MontoFinal = 4500m,
                FechaInicio = FechaReferencia.AddDays(-10),
                FechaFin = FechaReferencia.AddDays(-3)
            },
            // Subasta vencida desierta ✓
            new Subasta
            {
                Id = 5,
                Titulo = "Set de Sartenes Profesional",
                Descripcion = "Set de 5 sartenes profesionales con revestimiento cerámico antiadherente.",
                PrecioInicial = 1500m,
                PrecioActual = 1500m,
                IncrementoMinimo = 50m,
                ImagenUrl = "https://images.unsplash.com/photo-1584269600464-37b1b58a9fe7",
                CategoriaId = 2,
                VendedorId = 2,
                Estado = EstadoSubasta.Desierta,
                FechaInicio = FechaReferencia.AddDays(-10),
                FechaFin = FechaReferencia.AddDays(-2)
            }
        };

        var random = new Random(12345);
        var titulos = new[] { "iPhone 14", "Sillón de Cuero", "Cámara Reflex", "Guitarra Eléctrica", "Reloj Automático", "Consola de Videojuegos", "Zapatillas de Running", "Drone Profesional", "Monitor UltraWide", "Tablet 11 pulgadas", "MacBook Air", "Silla Ergonómica", "Auriculares Inalámbricos", "Micrófono de Condensador", "Teclado Mecánico RGB", "Proyector 4K", "Bicicleta Plegable", "Lámpara Inteligente", "Cafetera de Cápsulas", "Mochila de Viaje" };
        var imagenes = new[] {
            "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9",
            "https://images.unsplash.com/photo-1505740420928-5e560c06d30e",
            "https://images.unsplash.com/photo-1523275335684-37898b6baf30",
            "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f",
            "https://images.unsplash.com/photo-1503602642458-232111445657",
            "https://images.unsplash.com/photo-1496181133206-80ce9b88a853",
            "https://images.unsplash.com/photo-1542291026-7eec264c27ff",
            "https://images.unsplash.com/photo-1507646227500-4d389b0012be",
            "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf",
            "https://images.unsplash.com/photo-1483478550801-ceba5fe50e8e"
        };

        for (int i = 1000; i < 1100; i++)
        {
            var precioBase = random.Next(100, 5000);
            var fechaInicio = FechaReferencia.AddDays(random.Next(-5, 0));
            // Finalizan en un rango de 1 a 14 días (máximo 2 semanas)
            var fechaFin = FechaReferencia.AddDays(random.Next(1, 14)).AddHours(random.Next(0, 23)).AddMinutes(random.Next(0, 59));

            subastas.Add(new Subasta
            {
                Id = i,
                Titulo = $"{titulos[random.Next(titulos.Length)]} - Lote #{i}",
                Descripcion = $"Lote {i}: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.",
                PrecioInicial = precioBase,
                PrecioActual = precioBase,
                IncrementoMinimo = random.Next(1, 10) * 50,
                ImagenUrl = imagenes[random.Next(imagenes.Length)],
                CategoriaId = random.Next(1, 5),
                VendedorId = 2,
                Estado = EstadoSubasta.Activa,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });
        }

        constructor.Entity<Subasta>().HasData(subastas);
    }

    private static void SemillaPujas(ModelBuilder constructor)
    {
        constructor.Entity<Puja>().HasData(
            // Pujas en subasta activa estándar (Id=1)
            new Puja
            {
                Id = 1,
                SubastaId = 1,
                PostorId = 3,
                Monto = 5500m,
                FechaPuja = FechaReferencia.AddDays(-1)
            },
            new Puja
            {
                Id = 2,
                SubastaId = 1,
                PostorId = 5,
                Monto = 6000m,
                FechaPuja = FechaReferencia.AddHours(-12)
            },
            // Puja en subasta activa crítica (Id=2)
            new Puja
            {
                Id = 3,
                SubastaId = 2,
                PostorId = 3,
                Monto = 8500m,
                FechaPuja = FechaReferencia.AddHours(-2)
            },
            // Puja en subasta vencida con ganador (Id=4)
            new Puja
            {
                Id = 4,
                SubastaId = 4,
                PostorId = 5,
                Monto = 4500m,
                FechaPuja = FechaReferencia.AddDays(-5)
            }
        );
    }

    private static void SemillaMovimientos(ModelBuilder constructor)
    {
        constructor.Entity<MovimientoContable>().HasData(
            // Carga inicial maria_compradora
            new MovimientoContable
            {
                Id = 1,
                BilleteraId = 3,
                Tipo = TipoMovimiento.Carga,
                Monto = 15000m,
                Concepto = "Carga inicial de saldo",
                FechaMovimiento = FechaReferencia.AddDays(-15)
            },
            // Retención por pujas activas de maria_compradora
            new MovimientoContable
            {
                Id = 2,
                BilleteraId = 3,
                Tipo = TipoMovimiento.Retencion,
                Monto = -3500m,
                Concepto = "Retención por pujas activas en subastas",
                FechaMovimiento = FechaReferencia.AddDays(-1)
            },
            // Carga inicial ana_vip
            new MovimientoContable
            {
                Id = 3,
                BilleteraId = 5,
                Tipo = TipoMovimiento.Carga,
                Monto = 55000m,
                Concepto = "Carga inicial de saldo",
                FechaMovimiento = FechaReferencia.AddDays(-15)
            },
            // Retención por puja de ana_vip en subasta 1
            new MovimientoContable
            {
                Id = 4,
                BilleteraId = 5,
                Tipo = TipoMovimiento.Retencion,
                Monto = -6000m,
                Concepto = "Retención por puja en subasta Notebook Gamer MSI",
                FechaMovimiento = FechaReferencia.AddHours(-12)
            },
            // Carga inicial pedro_postor (sin fondos)
            new MovimientoContable
            {
                Id = 5,
                BilleteraId = 4,
                Tipo = TipoMovimiento.Carga,
                Monto = 200m,
                Concepto = "Carga inicial de saldo",
                FechaMovimiento = FechaReferencia.AddDays(-15)
            },
            // Carga inicial juan_vendedor
            new MovimientoContable
            {
                Id = 6,
                BilleteraId = 2,
                Tipo = TipoMovimiento.Carga,
                Monto = 5000m,
                Concepto = "Carga inicial de saldo vendedor",
                FechaMovimiento = FechaReferencia.AddDays(-15)
            }
        );
    }
}

