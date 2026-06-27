using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class comtraseñacajero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEIZINumUKq++fAJVGcKuJ56ZW+zDVZcWjry4kEFq5faiTZUgWxr+BIpaoixqRhpTMw==");

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "ClienteId", "CiudadId", "Contrasena", "CorreoElectronico", "FechaNacimiento", "Nombres", "RolId" },
                values: new object[] { 2, 1, "AQAAAAIAAYagAAAAEE/tOXnQvYyzRJk64Y2CWCw1cuObjxjq5ED4vImYSmN3STnnsZ26vQexcpcRVgUfPw==", "cajero@aleanastore.com", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cajero Aleana Store", 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEEE6gsLTq9o4tcS5hz+wK/LK6lWrWANgnHSF6B6LipmSO8D4sc819q9E4X5oaF8aLw==");
        }
    }
}
