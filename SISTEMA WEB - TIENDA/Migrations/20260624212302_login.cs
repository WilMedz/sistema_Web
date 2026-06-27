using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class login : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEPUfABeeROzFx256AAG2yxeInOiwh01r/Hgaok9gjlpWEyvXmpVxCVPhD0RClcxpSw==");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 2,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEI1F7g7qz72xOXNxCOwZyXwWKb4mMe/Vr9V0fs+2xX4M7hEazYY/ulz1AW2Qw9myKQ==");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 3,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEL9Z9te4LLKI9yOf10bA0Mb/uRiUMcRD1suVPbO2s6DWPwm6v6gSOvAOYalutwstQw==");
        }
    }
}
