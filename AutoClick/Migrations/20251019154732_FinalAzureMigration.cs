using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoClick.Migrations
{
    /// <inheritdoc />
    public partial class FinalAzureMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumeroTelefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NombreAgencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EsAdministrador = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "VentasExternas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Link = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Marca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Año = table.Column<int>(type: "int", nullable: true),
                    Kilometraje = table.Column<int>(type: "int", nullable: true),
                    PrecioVenta = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Placa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ValorFiscal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PromedioValorMercado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PromedioValorFiscal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentasExternas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Autos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    PlacaVehiculo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ValorFiscal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Divisa = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Carroceria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Combustible = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Cilindrada = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ColorExterior = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ColorInterior = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NumeroPuertas = table.Column<int>(type: "int", nullable: false),
                    NumeroPasajeros = table.Column<int>(type: "int", nullable: false),
                    Transmision = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Traccion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Kilometraje = table.Column<int>(type: "int", nullable: false),
                    Condicion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ExtrasExterior = table.Column<string>(type: "TEXT", nullable: false),
                    ExtrasInterior = table.Column<string>(type: "TEXT", nullable: false),
                    ExtrasMultimedia = table.Column<string>(type: "TEXT", nullable: false),
                    ExtrasSeguridad = table.Column<string>(type: "TEXT", nullable: false),
                    ExtrasRendimiento = table.Column<string>(type: "TEXT", nullable: false),
                    ExtrasAntiRobo = table.Column<string>(type: "TEXT", nullable: false),
                    Provincia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Canton = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UbicacionExacta = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    PlanVisibilidad = table.Column<int>(type: "int", nullable: false),
                    BanderinAdquirido = table.Column<int>(type: "int", nullable: false),
                    ImagenesUrls = table.Column<string>(type: "TEXT", nullable: false),
                    VideosUrls = table.Column<string>(type: "TEXT", nullable: false),
                    ImagenPrincipal = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EmailPropietario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Autos_Usuarios_EmailPropietario",
                        column: x => x.EmailPropietario,
                        principalTable: "Usuarios",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mensajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailCliente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoConsulta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContenidoMensaje = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Prioridad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Media"),
                    RespuestaAdmin = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailAdminRespuesta = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mensajes_Usuarios_EmailAdminRespuesta",
                        column: x => x.EmailAdminRespuesta,
                        principalTable: "Usuarios",
                        principalColumn: "Email");
                    table.ForeignKey(
                        name: "FK_Mensajes_Usuarios_EmailCliente",
                        column: x => x.EmailCliente,
                        principalTable: "Usuarios",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "Reclamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailCliente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TipoProblema = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Prioridad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Media"),
                    RespuestaAdmin = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailAdminRespuesta = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reclamos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reclamos_Usuarios_EmailAdminRespuesta",
                        column: x => x.EmailAdminRespuesta,
                        principalTable: "Usuarios",
                        principalColumn: "Email");
                    table.ForeignKey(
                        name: "FK_Reclamos_Usuarios_EmailCliente",
                        column: x => x.EmailCliente,
                        principalTable: "Usuarios",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Autos_Activo",
                table: "Autos",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_Ano",
                table: "Autos",
                column: "Ano");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_Canton",
                table: "Autos",
                column: "Canton");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_EmailPropietario",
                table: "Autos",
                column: "EmailPropietario");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_FechaCreacion",
                table: "Autos",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_Marca",
                table: "Autos",
                column: "Marca");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_Modelo",
                table: "Autos",
                column: "Modelo");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_PlacaVehiculo",
                table: "Autos",
                column: "PlacaVehiculo",
                unique: true,
                filter: "PlacaVehiculo IS NOT NULL AND PlacaVehiculo != ''");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_Provincia",
                table: "Autos",
                column: "Provincia");

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
                name: "Autos");

            migrationBuilder.DropTable(
                name: "Mensajes");

            migrationBuilder.DropTable(
                name: "Reclamos");

            migrationBuilder.DropTable(
                name: "VentasExternas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
