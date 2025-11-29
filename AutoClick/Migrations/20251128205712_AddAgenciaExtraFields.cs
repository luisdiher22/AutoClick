using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AddAgenciaExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Canton",
                table: "Usuarios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CedulaJuridica",
                table: "Usuarios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenBannerUrl",
                table: "Usuarios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenPerfilUrl",
                table: "Usuarios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provincia",
                table: "Usuarios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Canton",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CedulaJuridica",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ImagenBannerUrl",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ImagenPerfilUrl",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Provincia",
                table: "Usuarios");
        }
    }
}
