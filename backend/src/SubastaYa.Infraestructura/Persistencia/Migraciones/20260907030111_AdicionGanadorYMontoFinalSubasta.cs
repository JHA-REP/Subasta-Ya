using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubastaYa.Infraestructura.Migraciones
{
    /// <inheritdoc />
    public partial class AdicionGanadorYMontoFinalSubasta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Subastas",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "PrecioBase",
                table: "Subastas",
                newName: "PrecioInicial");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Billeteras",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "Saldo",
                table: "Billeteras",
                newName: "SaldoDisponible");

            migrationBuilder.AddColumn<int>(
                name: "GanadorId",
                table: "Subastas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Subastas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "IncrementoMinimo",
                table: "Subastas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoFinal",
                table: "Subastas",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioActual",
                table: "Subastas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GanadorId", "ImagenUrl", "IncrementoMinimo", "MontoFinal", "PrecioActual" },
                values: new object[] { null, "https://images.unsplash.com/photo-1603302576837-37561b2e2302", 100m, null, 6000m });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GanadorId", "ImagenUrl", "IncrementoMinimo", "MontoFinal", "PrecioActual" },
                values: new object[] { null, "https://images.unsplash.com/photo-1579783902614-a3fb3927b675", 200m, null, 8500m });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GanadorId", "ImagenUrl", "IncrementoMinimo", "MontoFinal", "PrecioActual" },
                values: new object[] { null, "https://images.unsplash.com/photo-1485965120184-e220f721d03e", 50m, null, 3000m });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "GanadorId", "ImagenUrl", "IncrementoMinimo", "MontoFinal", "PrecioActual" },
                values: new object[] { 5, "https://images.unsplash.com/photo-1593359677879-a4bb92f829d1", 100m, 4500m, 4500m });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Estado", "GanadorId", "ImagenUrl", "IncrementoMinimo", "MontoFinal", "PrecioActual" },
                values: new object[] { "Desierta", null, "https://images.unsplash.com/photo-1584269600464-37b1b58a9fe7", 50m, null, 1500m });

            migrationBuilder.CreateIndex(
                name: "IX_Subastas_GanadorId",
                table: "Subastas",
                column: "GanadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subastas_Usuarios_GanadorId",
                table: "Subastas",
                column: "GanadorId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subastas_Usuarios_GanadorId",
                table: "Subastas");

            migrationBuilder.DropIndex(
                name: "IX_Subastas_GanadorId",
                table: "Subastas");

            migrationBuilder.DropColumn(
                name: "GanadorId",
                table: "Subastas");

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Subastas");

            migrationBuilder.DropColumn(
                name: "IncrementoMinimo",
                table: "Subastas");

            migrationBuilder.DropColumn(
                name: "MontoFinal",
                table: "Subastas");

            migrationBuilder.DropColumn(
                name: "PrecioActual",
                table: "Subastas");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                table: "Subastas",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "PrecioInicial",
                table: "Subastas",
                newName: "PrecioBase");

            migrationBuilder.RenameColumn(
                name: "SaldoDisponible",
                table: "Billeteras",
                newName: "Saldo");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                table: "Billeteras",
                newName: "Version");

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5,
                column: "Estado",
                value: "Finalizada");
        }
    }
}
