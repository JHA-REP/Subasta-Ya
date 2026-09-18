using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infraestructura.Migraciones
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClaveHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Billeteras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SaldoRetenido = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billeteras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Billeteras_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subastas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    PrecioBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    VendedorId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subastas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subastas_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subastas_Usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosContables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BilleteraId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosContables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosContables_Billeteras_BilleteraId",
                        column: x => x.BilleteraId,
                        principalTable: "Billeteras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pujas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubastaId = table.Column<int>(type: "int", nullable: false),
                    PostorId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaPuja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pujas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pujas_Subastas_SubastaId",
                        column: x => x.SubastaId,
                        principalTable: "Subastas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pujas_Usuarios_PostorId",
                        column: x => x.PostorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Dispositivos y gadgets electrónicos", "Electrónica" },
                    { 2, "Artículos para el hogar y decoración", "Hogar" },
                    { 3, "Equipamiento y artículos deportivos", "Deportes" },
                    { 4, "Obras de arte, pinturas y esculturas", "Arte" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Alias", "ClaveHash", "Email", "FechaRegistro", "Rol" },
                values: new object[,]
                {
                    { 1, "admin", "PrP+ZrMeO00Q+nC1ytSccRIpSvauTkdqHEBRVdRaoSE=", "admin@com", new DateTime(2026, 7, 31, 12, 0, 0, 0, DateTimeKind.Utc), "Administrador" },
                    { 2, "juan_vendedor", "NhyFoVwMoMd3D6nVNXH2gDqgcOXdVgcgL2tAAMWo64g=", "juan@mail.com", new DateTime(2026, 8, 5, 12, 0, 0, 0, DateTimeKind.Utc), "Vendedor" },
                    { 3, "maria_compradora", "0TgymXq9YMECHsRDfhFEykEpOXQUGo68Feu0Wj4onnE=", "maria@mail.com", new DateTime(2026, 8, 10, 12, 0, 0, 0, DateTimeKind.Utc), "Comprador" },
                    { 4, "pedro_postor", "4SFWNm2mTLNbFQO59OhfFMdd4iQezexdyvD6hDW/2tE=", "pedro@mail.com", new DateTime(2026, 8, 12, 12, 0, 0, 0, DateTimeKind.Utc), "Comprador" },
                    { 5, "ana_vip", "fPHusZzQGjLTGP9VJejilp/m1XXfEdAsAgbpKgU7GY8=", "ana@mail.com", new DateTime(2026, 8, 15, 12, 0, 0, 0, DateTimeKind.Utc), "Comprador" }
                });

            migrationBuilder.InsertData(
                table: "Billeteras",
                columns: new[] { "Id", "Saldo", "SaldoRetenido", "UsuarioId" },
                values: new object[,]
                {
                    { 1, 0m, 0m, 1 },
                    { 2, 5000m, 0m, 2 },
                    { 3, 10000m, 3500m, 3 },
                    { 4, 200m, 0m, 4 },
                    { 5, 50000m, 1500m, 5 }
                });

            migrationBuilder.InsertData(
                table: "Subastas",
                columns: new[] { "Id", "CategoriaId", "Descripcion", "Estado", "FechaFin", "FechaInicio", "PrecioBase", "Titulo", "VendedorId" },
                values: new object[,]
                {
                    { 1, 1, "Notebook gamer MSI con RTX 4060, 16GB RAM, 512GB SSD. Estado impecable.", "Activa", new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 28, 12, 0, 0, 0, DateTimeKind.Utc), 5000m, "Notebook Gamer MSI", 2 },
                    { 2, 4, "Cuadro al óleo original de artista emergente. Técnica mixta sobre lienzo 80x60.", "Activa", new DateTime(2026, 8, 30, 12, 30, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 27, 12, 0, 0, 0, DateTimeKind.Utc), 8000m, "Cuadro Óleo Original", 2 },
                    { 3, 3, "Bicicleta de montaña rodado 29, cuadro de aluminio, 21 velocidades.", "Pendiente", new DateTime(2026, 9, 8, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 1, 12, 0, 0, 0, DateTimeKind.Utc), 3000m, "Bicicleta Montaña R29", 2 },
                    { 4, 1, "Smart TV LED 55 pulgadas 4K UHD con sistema operativo integrado.", "Finalizada", new DateTime(2026, 8, 27, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 20, 12, 0, 0, 0, DateTimeKind.Utc), 4000m, "Smart TV 55 Pulgadas", 2 },
                    { 5, 2, "Set de 5 sartenes profesionales con revestimiento cerámico antiadherente.", "Finalizada", new DateTime(2026, 8, 28, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 20, 12, 0, 0, 0, DateTimeKind.Utc), 1500m, "Set de Sartenes Profesional", 2 }
                });

            migrationBuilder.InsertData(
                table: "MovimientosContables",
                columns: new[] { "Id", "BilleteraId", "Concepto", "FechaMovimiento", "Monto", "Tipo" },
                values: new object[,]
                {
                    { 1, 3, "Carga inicial de saldo", new DateTime(2026, 8, 15, 12, 0, 0, 0, DateTimeKind.Utc), 15000m, "Carga" },
                    { 2, 3, "Retención por pujas activas en subastas", new DateTime(2026, 8, 29, 12, 0, 0, 0, DateTimeKind.Utc), -3500m, "Retencion" },
                    { 3, 5, "Carga inicial de saldo", new DateTime(2026, 8, 15, 12, 0, 0, 0, DateTimeKind.Utc), 55000m, "Carga" },
                    { 4, 5, "Retención por puja en subasta Cuadro Óleo", new DateTime(2026, 8, 30, 10, 0, 0, 0, DateTimeKind.Utc), -1500m, "Retencion" },
                    { 5, 4, "Carga inicial de saldo", new DateTime(2026, 8, 15, 12, 0, 0, 0, DateTimeKind.Utc), 200m, "Carga" },
                    { 6, 2, "Carga inicial de saldo vendedor", new DateTime(2026, 8, 15, 12, 0, 0, 0, DateTimeKind.Utc), 5000m, "Carga" }
                });

            migrationBuilder.InsertData(
                table: "Pujas",
                columns: new[] { "Id", "FechaPuja", "Monto", "PostorId", "SubastaId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 29, 12, 0, 0, 0, DateTimeKind.Utc), 5500m, 3, 1 },
                    { 2, new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), 6000m, 5, 1 },
                    { 3, new DateTime(2026, 8, 30, 10, 0, 0, 0, DateTimeKind.Utc), 8500m, 3, 2 },
                    { 4, new DateTime(2026, 8, 25, 12, 0, 0, 0, DateTimeKind.Utc), 4500m, 5, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Billeteras_UsuarioId",
                table: "Billeteras",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosContables_BilleteraId",
                table: "MovimientosContables",
                column: "BilleteraId");

            migrationBuilder.CreateIndex(
                name: "IX_Pujas_PostorId",
                table: "Pujas",
                column: "PostorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pujas_SubastaId",
                table: "Pujas",
                column: "SubastaId");

            migrationBuilder.CreateIndex(
                name: "IX_Subastas_CategoriaId",
                table: "Subastas",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Subastas_VendedorId",
                table: "Subastas",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Alias",
                table: "Usuarios",
                column: "Alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosContables");

            migrationBuilder.DropTable(
                name: "Pujas");

            migrationBuilder.DropTable(
                name: "Billeteras");

            migrationBuilder.DropTable(
                name: "Subastas");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
