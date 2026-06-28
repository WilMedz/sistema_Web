using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRegistroCaja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrosCaja",
                columns: table => new
                {
                    RegistroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientesId = table.Column<int>(type: "int", nullable: false),
                    FechaApertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MontoInicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosCaja", x => x.RegistroId);
                    table.ForeignKey(
                        name: "FK_RegistrosCaja_Clientes_ClientesId",
                        column: x => x.ClientesId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosCaja_ClientesId",
                table: "RegistrosCaja",
                column: "ClientesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosCaja");
        }
    }
}
