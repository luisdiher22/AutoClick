using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AddOnvoPayTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PagosOnvo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentIntentId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnuncioPublicidadId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    EmailUsuario = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastPaymentError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmationAttempts = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosOnvo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosOnvo_AnunciosPublicidad_AnuncioPublicidadId",
                        column: x => x.AnuncioPublicidadId,
                        principalTable: "AnunciosPublicidad",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WebhookEventsOnvo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaymentIntentId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Processed = table.Column<bool>(type: "bit", nullable: false),
                    ProcessingError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WebhookSecret = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookEventsOnvo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PagosOnvo_AnuncioPublicidadId",
                table: "PagosOnvo",
                column: "AnuncioPublicidadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagosOnvo");

            migrationBuilder.DropTable(
                name: "WebhookEventsOnvo");
        }
    }
}
