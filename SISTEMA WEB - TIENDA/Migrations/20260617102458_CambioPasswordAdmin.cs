using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class CambioPasswordAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Ciudades",
                keyColumn: "CiudadId",
                keyValue: 1,
                column: "NombreCiudad",
                value: "Cajamarca");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                columns: new[] { "Contrasena", "CorreoElectronico", "Nombres" },
                values: new object[] { "AQAAAAIAAYagAAAAEEE6gsLTq9o4tcS5hz+wK/LK6lWrWANgnHSF6B6LipmSO8D4sc819q9E4X5oaF8aLw==", "admin@aleanastore.com", "Administrador Aleana Store" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RolId", "NombreRol" },
                values: new object[] { 3, "Cajero" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RolId",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Ciudades",
                keyColumn: "CiudadId",
                keyValue: 1,
                column: "NombreCiudad",
                value: "Lima");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                columns: new[] { "Contrasena", "CorreoElectronico", "Nombres" },
                values: new object[] { "AQAAAAIAAYagAAAAEB5qdPtSTvlpFsPyRzroaYQ5YYJ4Loi4FBVdYJeET7t9YrRGBd/TlIQDdBnIpZgZ/g==", "admin@astrastore.com", "Administrador Astra Store" });
        }
    }
}
