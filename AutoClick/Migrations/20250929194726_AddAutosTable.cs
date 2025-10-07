using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AddAutosTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Autos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Ano = table.Column<int>(type: "INTEGER", nullable: false),
                    PlacaVehiculo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ValorFiscal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Carroceria = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Combustible = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Cilindrada = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ColorExterior = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ColorInterior = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    NumeroPuertas = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroPasajeros = table.Column<int>(type: "INTEGER", nullable: false),
                    Transmision = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Traccion = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Kilometraje = table.Column<int>(type: "INTEGER", nullable: false),
                    Condicion = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ExtrasExterior = table.Column<string>(type: "TEXT", nullable: false),
                    ExtrasInterior = table.Column<string>(type: "TEXT", nullable: false),
                    ExtrasMultimedia = table.Column<string>(type: "TEXT", nullable: false),
                    Provincia = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Canton = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UbicacionExacta = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PlanVisibilidad = table.Column<int>(type: "INTEGER", nullable: false),
                    BanderinAdquirido = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagenesUrls = table.Column<string>(type: "TEXT", nullable: false),
                    VideosUrls = table.Column<string>(type: "TEXT", nullable: false),
                    ImagenPrincipal = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    EmailPropietario = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Autos_Usuarios_EmailPropietario",
                        column: x => x.EmailPropietario,
                        principalTable: "Usuarios",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Autos_Activo",
                table: "Autos",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_Ano",
                table: "Autos",
                column: "Ano");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_Canton",
                table: "Autos",
                column: "Canton");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_EmailPropietario",
                table: "Autos",
                column: "EmailPropietario");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_FechaCreacion",
                table: "Autos",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_Marca",
                table: "Autos",
                column: "Marca");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_Modelo",
                table: "Autos",
                column: "Modelo");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_PlacaVehiculo",
                table: "Autos",
                column: "PlacaVehiculo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Autos_Provincia",
                table: "Autos",
                column: "Provincia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Autos");
        }
    }
}
