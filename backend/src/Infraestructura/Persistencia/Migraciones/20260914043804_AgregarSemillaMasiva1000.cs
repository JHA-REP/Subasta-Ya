using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infraestructura.Migraciones
{
    /// <inheritdoc />
    public partial class AgregarSemillaMasiva1000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Subastas",
                columns: new[] { "Id", "CategoriaId", "Descripcion", "Estado", "FechaFin", "FechaInicio", "GanadorId", "ImagenUrl", "IncrementoMinimo", "MontoFinal", "PrecioActual", "PrecioInicial", "Titulo", "VendedorId" },
                values: new object[,]
                {
                    { 1000, 2, "Lote 1000: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 18, 23, 47, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1507646227500-4d389b0012be", 100m, null, 427m, 427m, "Bicicleta Plegable - Lote #1000", 2 },
                    { 1001, 4, "Lote 1001: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 13, 16, 1, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 350m, null, 2579m, 2579m, "Cámara Reflex - Lote #1001", 2 },
                    { 1002, 3, "Lote 1002: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 10, 0, 21, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1542291026-7eec264c27ff", 300m, null, 4319m, 4319m, "Teclado Mecánico RGB - Lote #1002", 2 },
                    { 1003, 4, "Lote 1003: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 12, 3, 12, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 250m, null, 3764m, 3764m, "MacBook Air - Lote #1003", 2 },
                    { 1004, 3, "Lote 1004: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 13, 23, 41, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 150m, null, 112m, 112m, "Tablet 11 pulgadas - Lote #1004", 2 },
                    { 1005, 2, "Lote 1005: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 19, 16, 50, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1503602642458-232111445657", 300m, null, 1544m, 1544m, "Auriculares Inalámbricos - Lote #1005", 2 },
                    { 1006, 2, "Lote 1006: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 17, 13, 54, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1503602642458-232111445657", 50m, null, 247m, 247m, "Guitarra Eléctrica - Lote #1006", 2 },
                    { 1007, 3, "Lote 1007: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 9, 8, 43, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1507646227500-4d389b0012be", 200m, null, 173m, 173m, "Proyector 4K - Lote #1007", 2 },
                    { 1008, 2, "Lote 1008: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 17, 3, 15, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 50m, null, 2251m, 2251m, "Sillón de Cuero - Lote #1008", 2 },
                    { 1009, 3, "Lote 1009: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 16, 1, 4, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1503602642458-232111445657", 50m, null, 4411m, 4411m, "Proyector 4K - Lote #1009", 2 },
                    { 1010, 2, "Lote 1010: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 15, 18, 23, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 100m, null, 390m, 390m, "Consola de Videojuegos - Lote #1010", 2 },
                    { 1011, 1, "Lote 1011: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 16, 13, 47, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 450m, null, 3116m, 3116m, "Cámara Reflex - Lote #1011", 2 },
                    { 1012, 4, "Lote 1012: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 12, 2, 58, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 450m, null, 4262m, 4262m, "Auriculares Inalámbricos - Lote #1012", 2 },
                    { 1013, 4, "Lote 1013: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 17, 17, 50, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f", 50m, null, 3999m, 3999m, "Consola de Videojuegos - Lote #1013", 2 },
                    { 1014, 4, "Lote 1014: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 20, 20, 31, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1503602642458-232111445657", 300m, null, 873m, 873m, "iPhone 14 - Lote #1014", 2 },
                    { 1015, 1, "Lote 1015: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 17, 21, 39, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 350m, null, 3099m, 3099m, "Lámpara Inteligente - Lote #1015", 2 },
                    { 1016, 4, "Lote 1016: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 21, 8, 20, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f", 250m, null, 2246m, 2246m, "Guitarra Eléctrica - Lote #1016", 2 },
                    { 1017, 1, "Lote 1017: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 15, 20, 1, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1507646227500-4d389b0012be", 150m, null, 3599m, 3599m, "MacBook Air - Lote #1017", 2 },
                    { 1018, 2, "Lote 1018: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 18, 18, 1, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f", 400m, null, 511m, 511m, "MacBook Air - Lote #1018", 2 },
                    { 1019, 4, "Lote 1019: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 18, 7, 32, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1542291026-7eec264c27ff", 100m, null, 517m, 517m, "Micrófono de Condensador - Lote #1019", 2 },
                    { 1020, 1, "Lote 1020: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 19, 6, 53, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 100m, null, 4326m, 4326m, "Reloj Automático - Lote #1020", 2 },
                    { 1021, 3, "Lote 1021: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 17, 17, 7, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1542291026-7eec264c27ff", 400m, null, 1492m, 1492m, "Lámpara Inteligente - Lote #1021", 2 },
                    { 1022, 4, "Lote 1022: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 13, 21, 7, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f", 350m, null, 1281m, 1281m, "iPhone 14 - Lote #1022", 2 },
                    { 1023, 3, "Lote 1023: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 19, 8, 9, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1503602642458-232111445657", 300m, null, 563m, 563m, "Monitor UltraWide - Lote #1023", 2 },
                    { 1024, 1, "Lote 1024: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 16, 2, 6, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1496181133206-80ce9b88a853", 250m, null, 1009m, 1009m, "Cámara Reflex - Lote #1024", 2 },
                    { 1025, 2, "Lote 1025: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 12, 7, 57, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1483478550801-ceba5fe50e8e", 50m, null, 3319m, 3319m, "Cafetera de Cápsulas - Lote #1025", 2 },
                    { 1026, 1, "Lote 1026: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 10, 6, 2, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9", 150m, null, 2455m, 2455m, "MacBook Air - Lote #1026", 2 },
                    { 1027, 2, "Lote 1027: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 9, 19, 37, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1542291026-7eec264c27ff", 200m, null, 1450m, 1450m, "Micrófono de Condensador - Lote #1027", 2 },
                    { 1028, 3, "Lote 1028: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 8, 18, 56, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9", 450m, null, 523m, 523m, "Auriculares Inalámbricos - Lote #1028", 2 },
                    { 1029, 4, "Lote 1029: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 20, 8, 52, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 450m, null, 3545m, 3545m, "Reloj Automático - Lote #1029", 2 },
                    { 1030, 1, "Lote 1030: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 10, 13, 19, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 50m, null, 2058m, 2058m, "iPhone 14 - Lote #1030", 2 },
                    { 1031, 2, "Lote 1031: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 11, 1, 28, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1483478550801-ceba5fe50e8e", 300m, null, 4916m, 4916m, "Consola de Videojuegos - Lote #1031", 2 },
                    { 1032, 4, "Lote 1032: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 14, 23, 48, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1496181133206-80ce9b88a853", 50m, null, 2025m, 2025m, "Cafetera de Cápsulas - Lote #1032", 2 },
                    { 1033, 2, "Lote 1033: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 15, 8, 22, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1483478550801-ceba5fe50e8e", 350m, null, 838m, 838m, "Tablet 11 pulgadas - Lote #1033", 2 },
                    { 1034, 4, "Lote 1034: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 11, 12, 11, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1483478550801-ceba5fe50e8e", 250m, null, 652m, 652m, "Proyector 4K - Lote #1034", 2 },
                    { 1035, 4, "Lote 1035: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 18, 2, 17, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1496181133206-80ce9b88a853", 400m, null, 1702m, 1702m, "Mochila de Viaje - Lote #1035", 2 },
                    { 1036, 4, "Lote 1036: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 15, 18, 1, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1483478550801-ceba5fe50e8e", 100m, null, 2467m, 2467m, "Proyector 4K - Lote #1036", 2 },
                    { 1037, 4, "Lote 1037: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 21, 4, 52, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1503602642458-232111445657", 450m, null, 3176m, 3176m, "MacBook Air - Lote #1037", 2 },
                    { 1038, 2, "Lote 1038: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 9, 4, 58, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1542291026-7eec264c27ff", 100m, null, 255m, 255m, "Bicicleta Plegable - Lote #1038", 2 },
                    { 1039, 1, "Lote 1039: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 9, 23, 58, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 250m, null, 465m, 465m, "Proyector 4K - Lote #1039", 2 },
                    { 1040, 4, "Lote 1040: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 9, 0, 47, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 50m, null, 3713m, 3713m, "Consola de Videojuegos - Lote #1040", 2 },
                    { 1041, 1, "Lote 1041: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 11, 17, 38, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 50m, null, 4270m, 4270m, "Bicicleta Plegable - Lote #1041", 2 },
                    { 1042, 2, "Lote 1042: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 20, 4, 6, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 150m, null, 4623m, 4623m, "Zapatillas de Running - Lote #1042", 2 },
                    { 1043, 3, "Lote 1043: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 9, 15, 10, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 250m, null, 3468m, 3468m, "Micrófono de Condensador - Lote #1043", 2 },
                    { 1044, 1, "Lote 1044: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 16, 5, 42, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f", 450m, null, 4347m, 4347m, "Cámara Reflex - Lote #1044", 2 },
                    { 1045, 2, "Lote 1045: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 19, 18, 23, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1542291026-7eec264c27ff", 300m, null, 1135m, 1135m, "Cámara Reflex - Lote #1045", 2 },
                    { 1046, 4, "Lote 1046: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 18, 3, 12, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f", 400m, null, 915m, 915m, "Micrófono de Condensador - Lote #1046", 2 },
                    { 1047, 2, "Lote 1047: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 14, 0, 46, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 100m, null, 3790m, 3790m, "Monitor UltraWide - Lote #1047", 2 },
                    { 1048, 2, "Lote 1048: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 9, 6, 44, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 150m, null, 3166m, 3166m, "Bicicleta Plegable - Lote #1048", 2 },
                    { 1049, 1, "Lote 1049: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 18, 10, 31, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 150m, null, 2653m, 2653m, "Silla Ergonómica - Lote #1049", 2 },
                    { 1050, 3, "Lote 1050: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 12, 8, 53, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 200m, null, 4627m, 4627m, "Drone Profesional - Lote #1050", 2 },
                    { 1051, 2, "Lote 1051: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 8, 22, 5, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1483478550801-ceba5fe50e8e", 400m, null, 3301m, 3301m, "Guitarra Eléctrica - Lote #1051", 2 },
                    { 1052, 4, "Lote 1052: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 12, 9, 23, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f", 200m, null, 2001m, 2001m, "Cámara Reflex - Lote #1052", 2 },
                    { 1053, 1, "Lote 1053: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 11, 7, 16, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 100m, null, 257m, 257m, "MacBook Air - Lote #1053", 2 },
                    { 1054, 2, "Lote 1054: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 12, 2, 25, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1542291026-7eec264c27ff", 100m, null, 425m, 425m, "MacBook Air - Lote #1054", 2 },
                    { 1055, 4, "Lote 1055: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 13, 1, 11, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 50m, null, 1275m, 1275m, "Silla Ergonómica - Lote #1055", 2 },
                    { 1056, 2, "Lote 1056: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 18, 16, 31, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f", 250m, null, 4793m, 4793m, "Silla Ergonómica - Lote #1056", 2 },
                    { 1057, 2, "Lote 1057: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 14, 0, 14, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1507646227500-4d389b0012be", 100m, null, 2150m, 2150m, "Bicicleta Plegable - Lote #1057", 2 },
                    { 1058, 2, "Lote 1058: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 21, 4, 41, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 350m, null, 1128m, 1128m, "Proyector 4K - Lote #1058", 2 },
                    { 1059, 1, "Lote 1059: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 16, 2, 45, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 300m, null, 2973m, 2973m, "Monitor UltraWide - Lote #1059", 2 },
                    { 1060, 4, "Lote 1060: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 11, 3, 9, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1507646227500-4d389b0012be", 50m, null, 1197m, 1197m, "Cámara Reflex - Lote #1060", 2 },
                    { 1061, 1, "Lote 1061: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 18, 22, 26, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 150m, null, 336m, 336m, "Tablet 11 pulgadas - Lote #1061", 2 },
                    { 1062, 4, "Lote 1062: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 11, 12, 29, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1483478550801-ceba5fe50e8e", 150m, null, 4818m, 4818m, "Monitor UltraWide - Lote #1062", 2 },
                    { 1063, 4, "Lote 1063: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 9, 10, 56, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 300m, null, 4337m, 4337m, "Mochila de Viaje - Lote #1063", 2 },
                    { 1064, 2, "Lote 1064: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 16, 7, 36, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1542291026-7eec264c27ff", 450m, null, 448m, 448m, "Cafetera de Cápsulas - Lote #1064", 2 },
                    { 1065, 1, "Lote 1065: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 8, 14, 18, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1542291026-7eec264c27ff", 150m, null, 2315m, 2315m, "Lámpara Inteligente - Lote #1065", 2 },
                    { 1066, 4, "Lote 1066: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 14, 5, 20, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 150m, null, 4697m, 4697m, "Reloj Automático - Lote #1066", 2 },
                    { 1067, 2, "Lote 1067: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 18, 6, 51, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 300m, null, 4075m, 4075m, "Silla Ergonómica - Lote #1067", 2 },
                    { 1068, 4, "Lote 1068: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 15, 20, 4, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 450m, null, 2776m, 2776m, "Lámpara Inteligente - Lote #1068", 2 },
                    { 1069, 3, "Lote 1069: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 14, 15, 40, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1542291026-7eec264c27ff", 450m, null, 4247m, 4247m, "Zapatillas de Running - Lote #1069", 2 },
                    { 1070, 3, "Lote 1070: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 10, 19, 34, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 450m, null, 2279m, 2279m, "Drone Profesional - Lote #1070", 2 },
                    { 1071, 3, "Lote 1071: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 11, 22, 27, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1507646227500-4d389b0012be", 250m, null, 3664m, 3664m, "Sillón de Cuero - Lote #1071", 2 },
                    { 1072, 2, "Lote 1072: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 9, 23, 48, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1503602642458-232111445657", 250m, null, 2067m, 2067m, "Tablet 11 pulgadas - Lote #1072", 2 },
                    { 1073, 4, "Lote 1073: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 13, 20, 48, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 400m, null, 2548m, 2548m, "Reloj Automático - Lote #1073", 2 },
                    { 1074, 1, "Lote 1074: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 15, 16, 27, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 150m, null, 902m, 902m, "Cafetera de Cápsulas - Lote #1074", 2 },
                    { 1075, 1, "Lote 1075: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 11, 21, 35, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1496181133206-80ce9b88a853", 400m, null, 2809m, 2809m, "Mochila de Viaje - Lote #1075", 2 },
                    { 1076, 2, "Lote 1076: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 15, 3, 52, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1503602642458-232111445657", 450m, null, 2969m, 2969m, "Guitarra Eléctrica - Lote #1076", 2 },
                    { 1077, 3, "Lote 1077: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 18, 12, 36, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f", 100m, null, 903m, 903m, "Consola de Videojuegos - Lote #1077", 2 },
                    { 1078, 1, "Lote 1078: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 9, 16, 10, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 150m, null, 4807m, 4807m, "Silla Ergonómica - Lote #1078", 2 },
                    { 1079, 2, "Lote 1079: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 8, 21, 32, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 50m, null, 991m, 991m, "Reloj Automático - Lote #1079", 2 },
                    { 1080, 4, "Lote 1080: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 12, 20, 16, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 250m, null, 2401m, 2401m, "iPhone 14 - Lote #1080", 2 },
                    { 1081, 2, "Lote 1081: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 17, 12, 16, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 200m, null, 2155m, 2155m, "Reloj Automático - Lote #1081", 2 },
                    { 1082, 2, "Lote 1082: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 18, 8, 24, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1507646227500-4d389b0012be", 100m, null, 214m, 214m, "Mochila de Viaje - Lote #1082", 2 },
                    { 1083, 4, "Lote 1083: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 19, 0, 55, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1542291026-7eec264c27ff", 50m, null, 2077m, 2077m, "Drone Profesional - Lote #1083", 2 },
                    { 1084, 2, "Lote 1084: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 19, 13, 42, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 350m, null, 1140m, 1140m, "iPhone 14 - Lote #1084", 2 },
                    { 1085, 4, "Lote 1085: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 19, 6, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9", 400m, null, 1334m, 1334m, "Micrófono de Condensador - Lote #1085", 2 },
                    { 1086, 2, "Lote 1086: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 8, 19, 11, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9", 350m, null, 2554m, 2554m, "MacBook Air - Lote #1086", 2 },
                    { 1087, 2, "Lote 1087: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 15, 21, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1523275335684-37898b6baf30", 200m, null, 606m, 606m, "Auriculares Inalámbricos - Lote #1087", 2 },
                    { 1088, 3, "Lote 1088: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 17, 5, 10, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1483478550801-ceba5fe50e8e", 300m, null, 2510m, 2510m, "Silla Ergonómica - Lote #1088", 2 },
                    { 1089, 1, "Lote 1089: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 17, 14, 41, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1496181133206-80ce9b88a853", 250m, null, 2688m, 2688m, "Lámpara Inteligente - Lote #1089", 2 },
                    { 1090, 4, "Lote 1090: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 13, 20, 21, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1496181133206-80ce9b88a853", 300m, null, 4236m, 4236m, "Micrófono de Condensador - Lote #1090", 2 },
                    { 1091, 1, "Lote 1091: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 10, 16, 27, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f", 200m, null, 4005m, 4005m, "Bicicleta Plegable - Lote #1091", 2 },
                    { 1092, 2, "Lote 1092: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 13, 22, 41, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", 450m, null, 945m, 945m, "Consola de Videojuegos - Lote #1092", 2 },
                    { 1093, 2, "Lote 1093: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 10, 4, 54, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1496181133206-80ce9b88a853", 250m, null, 3598m, 3598m, "Tablet 11 pulgadas - Lote #1093", 2 },
                    { 1094, 1, "Lote 1094: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 15, 12, 9, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 300m, null, 4259m, 4259m, "Silla Ergonómica - Lote #1094", 2 },
                    { 1095, 4, "Lote 1095: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 10, 9, 40, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f", 350m, null, 2166m, 2166m, "Drone Profesional - Lote #1095", 2 },
                    { 1096, 3, "Lote 1096: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 10, 15, 23, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 100m, null, 3414m, 3414m, "Reloj Automático - Lote #1096", 2 },
                    { 1097, 1, "Lote 1097: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 20, 3, 39, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1496181133206-80ce9b88a853", 250m, null, 4230m, 4230m, "Mochila de Viaje - Lote #1097", 2 },
                    { 1098, 4, "Lote 1098: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 16, 6, 52, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1496181133206-80ce9b88a853", 100m, null, 2573m, 2573m, "Lámpara Inteligente - Lote #1098", 2 },
                    { 1099, 1, "Lote 1099: excelente artículo generado automáticamente. Cuenta con todas las certificaciones y se encuentra en perfecto estado. Oferta imperdible.", "Activa", new DateTime(2026, 9, 13, 6, 56, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), null, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", 350m, null, 1930m, 1930m, "Silla Ergonómica - Lote #1099", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1000);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1006);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1007);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1008);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1009);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1010);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1011);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1012);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1013);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1014);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1015);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1016);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1017);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1018);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1019);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1020);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1021);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1022);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1023);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1024);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1025);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1026);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1027);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1028);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1029);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1030);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1031);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1032);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1033);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1034);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1035);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1036);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1037);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1038);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1039);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1040);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1041);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1042);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1043);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1044);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1045);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1046);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1047);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1048);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1049);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1050);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1051);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1052);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1053);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1054);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1055);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1056);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1057);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1058);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1059);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1060);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1061);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1062);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1063);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1064);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1065);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1066);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1067);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1068);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1069);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1070);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1071);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1072);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1073);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1074);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1075);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1076);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1077);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1078);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1079);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1080);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1081);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1082);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1083);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1084);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1085);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1086);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1087);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1088);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1089);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1090);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1091);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1092);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1093);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1094);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1095);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1096);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1097);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1098);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1099);
        }
    }
}
