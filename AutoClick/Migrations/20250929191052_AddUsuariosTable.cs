using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuariosTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NumeroTelefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Contrasena = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NombreAgencia = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UltimaConexion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Email);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_FechaRegistro",
                table: "Usuarios",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreAgencia",
                table: "Usuarios",
                column: "NombreAgencia");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NumeroTelefono",
                table: "Usuarios",
                column: "NumeroTelefono");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
