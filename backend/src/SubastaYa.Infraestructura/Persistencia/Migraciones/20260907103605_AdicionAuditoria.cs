using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubastaYa.Infraestructura.Migraciones
{
    /// <inheritdoc />
    public partial class AdicionAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditoriaRegistros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Accion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntidadTipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntidadId = table.Column<int>(type: "int", nullable: false),
                    DetalleJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioOrigen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaRegistros", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaRegistros_Accion",
                table: "AuditoriaRegistros",
                column: "Accion");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaRegistros_Entidad",
                table: "AuditoriaRegistros",
                columns: new[] { "EntidadTipo", "EntidadId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaRegistros_Fecha",
                table: "AuditoriaRegistros",
                column: "FechaRegistro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditoriaRegistros");
        }
    }
}
