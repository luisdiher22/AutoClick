using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class RemoveValorFiscal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Primero eliminar el índice que depende de ValorFiscal
            migrationBuilder.DropIndex(
                name: "IX_Autos_ValorFiscal_Activo",
                table: "Autos");

            // Luego eliminar la columna
            migrationBuilder.DropColumn(
                name: "ValorFiscal",
                table: "Autos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Primero agregar la columna de vuelta
            migrationBuilder.AddColumn<decimal>(
                name: "ValorFiscal",
                table: "Autos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            // Luego recrear el índice
            migrationBuilder.CreateIndex(
                name: "IX_Autos_ValorFiscal_Activo",
                table: "Autos",
                columns: new[] { "ValorFiscal", "Activo" });
        }
    }
}
