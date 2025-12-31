using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoIdToPagosOnvo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AutoId",
                table: "PagosOnvo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PagosOnvo_AutoId",
                table: "PagosOnvo",
                column: "AutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PagosOnvo_Autos_AutoId",
                table: "PagosOnvo",
                column: "AutoId",
                principalTable: "Autos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PagosOnvo_Autos_AutoId",
                table: "PagosOnvo");

            migrationBuilder.DropIndex(
                name: "IX_PagosOnvo_AutoId",
                table: "PagosOnvo");

            migrationBuilder.DropColumn(
                name: "AutoId",
                table: "PagosOnvo");
        }
    }
}
