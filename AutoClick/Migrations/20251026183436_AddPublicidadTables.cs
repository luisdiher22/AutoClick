using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicidadTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpresasPublicidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEmpresa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaSalida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstadoCobros = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Al día"),
                    Activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresasPublicidad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnunciosPublicidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaPublicidadId = table.Column<int>(type: "int", nullable: false),
                    UrlImagen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumeroVistas = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    NumeroClics = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnunciosPublicidad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnunciosPublicidad_EmpresasPublicidad_EmpresaPublicidadId",
                        column: x => x.EmpresaPublicidadId,
                        principalTable: "EmpresasPublicidad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnunciosPublicidad_Activo",
                table: "AnunciosPublicidad",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_AnunciosPublicidad_EmpresaPublicidadId",
                table: "AnunciosPublicidad",
                column: "EmpresaPublicidadId");

            migrationBuilder.CreateIndex(
                name: "IX_AnunciosPublicidad_FechaPublicacion",
                table: "AnunciosPublicidad",
                column: "FechaPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasPublicidad_Activa",
                table: "EmpresasPublicidad",
                column: "Activa");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasPublicidad_FechaInicio",
                table: "EmpresasPublicidad",
                column: "FechaInicio");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasPublicidad_NombreEmpresa",
                table: "EmpresasPublicidad",
                column: "NombreEmpresa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnunciosPublicidad");

            migrationBuilder.DropTable(
                name: "EmpresasPublicidad");
        }
    }
}
