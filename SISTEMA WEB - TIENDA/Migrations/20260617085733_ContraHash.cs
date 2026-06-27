using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class ContraHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrecioUnitarioSnapshot",
                table: "DetallesPedido",
                newName: "PrecioUnitario");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEB5qdPtSTvlpFsPyRzroaYQ5YYJ4Loi4FBVdYJeET7t9YrRGBd/TlIQDdBnIpZgZ/g==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrecioUnitario",
                table: "DetallesPedido",
                newName: "PrecioUnitarioSnapshot");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                column: "Contrasena",
                value: "Astra2026!");
        }
    }
}
