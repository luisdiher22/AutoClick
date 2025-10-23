using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AddSolicitudesEmpresaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesEmpresa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEmpresa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RepresentanteLegal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Industria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DescripcionEmpresa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    NotasInternas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesEmpresa", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEmpresa_CorreoElectronico",
                table: "SolicitudesEmpresa",
                column: "CorreoElectronico");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEmpresa_Estado",
                table: "SolicitudesEmpresa",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEmpresa_FechaCreacion",
                table: "SolicitudesEmpresa",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEmpresa_NombreEmpresa",
                table: "SolicitudesEmpresa",
                column: "NombreEmpresa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesEmpresa");
        }
    }
}
