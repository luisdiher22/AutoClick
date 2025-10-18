using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AddVentasExternasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VentasExternas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Link = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Año = table.Column<int>(type: "INTEGER", nullable: false),
                    Kilometraje = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecioVenta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Placa = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ValorFiscal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PromedioValorMercado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PromedioValorFiscal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentasExternas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VentasExternas");
        }
    }
}
