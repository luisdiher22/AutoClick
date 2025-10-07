using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullPlates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Autos_PlacaVehiculo",
                table: "Autos");

            migrationBuilder.AlterColumn<string>(
                name: "PlacaVehiculo",
                table: "Autos",
                type: "TEXT",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "IX_Autos_PlacaVehiculo",
                table: "Autos",
                column: "PlacaVehiculo",
                unique: true,
                filter: "PlacaVehiculo IS NOT NULL AND PlacaVehiculo != ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Autos_PlacaVehiculo",
                table: "Autos");

            migrationBuilder.AlterColumn<string>(
                name: "PlacaVehiculo",
                table: "Autos",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Autos_PlacaVehiculo",
                table: "Autos",
                column: "PlacaVehiculo",
                unique: true);
        }
    }
}
