using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AddExtrasFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtrasAntiRobo",
                table: "Autos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtrasRendimiento",
                table: "Autos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtrasSeguridad",
                table: "Autos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtrasAntiRobo",
                table: "Autos");

            migrationBuilder.DropColumn(
                name: "ExtrasRendimiento",
                table: "Autos");

            migrationBuilder.DropColumn(
                name: "ExtrasSeguridad",
                table: "Autos");
        }
    }
}
