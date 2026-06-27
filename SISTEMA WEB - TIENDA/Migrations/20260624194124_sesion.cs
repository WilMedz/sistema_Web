using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class sesion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                value: "AQAAAAIAAYagAAAAEAcHBa0eS72FMQlt12d6Bya8iHUY41DXvByq8fhPyWSGMMmMq8jW6draE/R4v1LoIg==");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 3,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEL9Z9te4LLKI9yOf10bA0Mb/uRiUMcRD1suVPbO2s6DWPwm6v6gSOvAOYalutwstQw==");

            migrationBuilder.UpdateData(
                table: "DireccionesEnvio",
                keyColumn: "DireccionId",
                keyValue: 1,
                column: "Referencia",
                value: "Dirección por defecto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEOepQzy0pvr7KKIF7+0jiXczGmhwTDFpurZir+R/lDHtBG/1TM9qy82pKuJCyvwqpw==");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 2,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEE3U24/OJ9APED3aKnI7OTdyRE8g7UQ0wJJ/mEcmsPHrdL2HvQbyZzDmslHkp7lRQw==\r\n");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 3,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEGhZvdyGUZXWQR2NXL9GvTI2+7Et7Y7vHkDMEYUJnRhjyVf9ls9a/FtUKKhjoXoNPw==");

            migrationBuilder.UpdateData(
                table: "DireccionesEnvio",
                keyColumn: "DireccionId",
                keyValue: 1,
                column: "Referencia",
                value: "Dirección por defecto del sistema");
        }
    }
}
