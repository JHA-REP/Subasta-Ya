using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubastaYa.Infraestructura.Migraciones
{
    /// <inheritdoc />
    public partial class ActualizacionSemilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 3,
                column: "SaldoRetenido",
                value: 8500m);

            migrationBuilder.UpdateData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "SaldoDisponible", "SaldoRetenido" },
                values: new object[] { 44000m, 6000m });

            migrationBuilder.UpdateData(
                table: "MovimientosContables",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaMovimiento",
                value: new DateTime(2026, 8, 23, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "MovimientosContables",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaMovimiento",
                value: new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "MovimientosContables",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaMovimiento",
                value: new DateTime(2026, 8, 23, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "MovimientosContables",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Concepto", "FechaMovimiento", "Monto" },
                values: new object[] { "Retención por puja en subasta Notebook Gamer MSI", new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Utc), -6000m });

            migrationBuilder.UpdateData(
                table: "MovimientosContables",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaMovimiento",
                value: new DateTime(2026, 8, 23, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "MovimientosContables",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaMovimiento",
                value: new DateTime(2026, 8, 23, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 6, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 7, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 12, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 7, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 16, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 9, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 28, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 28, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 8, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 13, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 18, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 20, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 23, 12, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 3,
                column: "SaldoRetenido",
                value: 3500m);

            migrationBuilder.UpdateData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "SaldoDisponible", "SaldoRetenido" },
                values: new object[] { 50000m, 1500m });

            migrationBuilder.UpdateData(
                table: "MovimientosContables",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaMovimiento",
                value: new DateTime(2026, 8, 15, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "MovimientosContables",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaMovimiento",
                value: new DateTime(2026, 8, 29, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "MovimientosContables",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaMovimiento",
                value: new DateTime(2026, 8, 15, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "MovimientosContables",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Concepto", "FechaMovimiento", "Monto" },
                values: new object[] { "Retención por puja en subasta Cuadro Óleo", new DateTime(2026, 8, 30, 10, 0, 0, 0, DateTimeKind.Utc), -1500m });

            migrationBuilder.UpdateData(
                table: "MovimientosContables",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaMovimiento",
                value: new DateTime(2026, 8, 15, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "MovimientosContables",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaMovimiento",
                value: new DateTime(2026, 8, 15, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaPuja",
                value: new DateTime(2026, 8, 29, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaPuja",
                value: new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaPuja",
                value: new DateTime(2026, 8, 30, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaPuja",
                value: new DateTime(2026, 8, 25, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 28, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 8, 30, 12, 30, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 27, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 8, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 1, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 8, 27, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 20, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 8, 28, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 20, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 7, 31, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 5, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 10, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 12, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 15, 12, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}
