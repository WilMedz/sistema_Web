using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class quitarcontraseñashasheadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                column: "Contrasena",
                value: "Admin2026");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 2,
                column: "Contrasena",
                value: "Cajero2026");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 3,
                column: "Contrasena",
                value: "Cliente2026");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEHgJBZDt5Fv1vatrghL14WEOulS0UPykEDiianRLuSDvs/e6MFhdDB44MVkPAH8Q6w==");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 2,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEN6H5cPlaRa+efXidmQNnTP5JCSBshSRMoPYGcYAyU144bH2yNBnU7lddeufd+kd8w==");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 3,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEP/x+1AG+TOiyROki5qbmseMcp5upeaagsE3QFbrP8EkxmFM6727UA7k4in9IkVfcg==");
        }
    }
}
