using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class RemoveForeignKeysFromMensajesYReclamos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Usuarios_EmailAdminRespuesta",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Usuarios_EmailCliente",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_Reclamos_Usuarios_EmailAdminRespuesta",
                table: "Reclamos");

            migrationBuilder.DropForeignKey(
                name: "FK_Reclamos_Usuarios_EmailCliente",
                table: "Reclamos");

            migrationBuilder.DropIndex(
                name: "IX_Reclamos_EmailAdminRespuesta",
                table: "Reclamos");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_EmailAdminRespuesta",
                table: "Mensajes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Reclamos_EmailAdminRespuesta",
                table: "Reclamos",
                column: "EmailAdminRespuesta");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_EmailAdminRespuesta",
                table: "Mensajes",
                column: "EmailAdminRespuesta");

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Usuarios_EmailAdminRespuesta",
                table: "Mensajes",
                column: "EmailAdminRespuesta",
                principalTable: "Usuarios",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Usuarios_EmailCliente",
                table: "Mensajes",
                column: "EmailCliente",
                principalTable: "Usuarios",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Reclamos_Usuarios_EmailAdminRespuesta",
                table: "Reclamos",
                column: "EmailAdminRespuesta",
                principalTable: "Usuarios",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Reclamos_Usuarios_EmailCliente",
                table: "Reclamos",
                column: "EmailCliente",
                principalTable: "Usuarios",
                principalColumn: "Email");
        }
    }
}
