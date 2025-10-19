using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeIndexesForPerformance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Índice compuesto para búsquedas frecuentes en Autos
            migrationBuilder.CreateIndex(
                name: "IX_Autos_Marca_Modelo_Ano",
                table: "Autos",
                columns: new[] { "Marca", "Modelo", "Ano" });

            // Índice para consultas por precio
            migrationBuilder.CreateIndex(
                name: "IX_Autos_ValorFiscal_Activo",
                table: "Autos",
                columns: new[] { "ValorFiscal", "Activo" });

            // Índice para consultas por ubicación
            migrationBuilder.CreateIndex(
                name: "IX_Autos_Provincia_Canton",
                table: "Autos",
                columns: new[] { "Provincia", "Canton" });

            // Índice para Dashboard queries
            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_Estado_FechaCreacion",
                table: "Mensajes",
                columns: new[] { "Estado", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_Reclamos_Estado_FechaCreacion", 
                table: "Reclamos",
                columns: new[] { "Estado", "FechaCreacion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Autos_Marca_Modelo_Ano",
                table: "Autos");

            migrationBuilder.DropIndex(
                name: "IX_Autos_ValorFiscal_Activo",
                table: "Autos");

            migrationBuilder.DropIndex(
                name: "IX_Autos_Provincia_Canton",
                table: "Autos");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_Estado_FechaCreacion",
                table: "Mensajes");

            migrationBuilder.DropIndex(
                name: "IX_Reclamos_Estado_FechaCreacion",
                table: "Reclamos");
        }
    }
}
