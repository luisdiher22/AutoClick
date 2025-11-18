using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AddUnidadKilometraje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnidadKilometraje",
                table: "Autos",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Km");
            
            // Actualizar registros existentes que tienen valor vacío
            migrationBuilder.Sql("UPDATE Autos SET UnidadKilometraje = 'Km' WHERE UnidadKilometraje = ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnidadKilometraje",
                table: "Autos");
        }
    }
}
