using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AddSoporteTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mensajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmailCliente = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TipoConsulta = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Asunto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ContenidoMensaje = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Prioridad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Media"),
                    RespuestaAdmin = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmailAdminRespuesta = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mensajes_Usuarios_EmailAdminRespuesta",
                        column: x => x.EmailAdminRespuesta,
                        principalTable: "Usuarios",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Mensajes_Usuarios_EmailCliente",
                        column: x => x.EmailCliente,
                        principalTable: "Usuarios",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Reclamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmailCliente = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TipoProblema = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Asunto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Prioridad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Media"),
                    RespuestaAdmin = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmailAdminRespuesta = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reclamos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reclamos_Usuarios_EmailAdminRespuesta",
                        column: x => x.EmailAdminRespuesta,
                        principalTable: "Usuarios",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reclamos_Usuarios_EmailCliente",
                        column: x => x.EmailCliente,
                        principalTable: "Usuarios",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_EmailAdminRespuesta",
                table: "Mensajes",
                column: "EmailAdminRespuesta");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_EmailCliente",
                table: "Mensajes",
                column: "EmailCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_Estado",
                table: "Mensajes",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_FechaCreacion",
                table: "Mensajes",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_Prioridad",
                table: "Mensajes",
                column: "Prioridad");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_TipoConsulta",
                table: "Mensajes",
                column: "TipoConsulta");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamos_EmailAdminRespuesta",
                table: "Reclamos",
                column: "EmailAdminRespuesta");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamos_EmailCliente",
                table: "Reclamos",
                column: "EmailCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamos_Estado",
                table: "Reclamos",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamos_FechaCreacion",
                table: "Reclamos",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamos_Prioridad",
                table: "Reclamos",
                column: "Prioridad");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamos_TipoProblema",
                table: "Reclamos",
                column: "TipoProblema");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mensajes");

            migrationBuilder.DropTable(
                name: "Reclamos");
        }
    }
}
