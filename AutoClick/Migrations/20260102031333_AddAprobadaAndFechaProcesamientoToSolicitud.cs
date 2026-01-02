using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class AddAprobadaAndFechaProcesamientoToSolicitud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Aprobada",
                table: "SolicitudPreAprobacion",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaProcesamiento",
                table: "SolicitudPreAprobacion",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aprobada",
                table: "SolicitudPreAprobacion");

            migrationBuilder.DropColumn(
                name: "FechaProcesamiento",
                table: "SolicitudPreAprobacion");
        }
    }
}
