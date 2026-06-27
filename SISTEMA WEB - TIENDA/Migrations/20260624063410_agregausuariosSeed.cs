using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class agregausuariosSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "ClienteId", "CiudadId", "Contrasena", "CorreoElectronico", "FechaNacimiento", "Nombres", "RolId" },
                values: new object[] { 3, 1, "AQAAAAIAAYagAAAAEGhZvdyGUZXWQR2NXL9GvTI2+7Et7Y7vHkDMEYUJnRhjyVf9ls9a/FtUKKhjoXoNPw==", "cliente@aleanastore.com", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cliente Demo", 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEIZINumUKq++fAJVGcKuJ56ZW+zDVZcWjry4kEFq5faiTZUgWxr+BIpaoixqRhpTMw==");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 2,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEEHxsixMhZGQ64uYyLBPmS7kIedbpf19wzf0wZKnUeUrLokmh7nTVHTTH9PfXtvuIw==");
        }
    }
}
