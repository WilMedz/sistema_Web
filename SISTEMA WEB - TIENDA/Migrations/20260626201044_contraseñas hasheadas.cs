using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class contraseñashasheadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                column: "Contrasena",
                value: "$2a$11$vsp5syeNTxL8Au5WpDKUpeYLReEQ6vr9oI290uaJDa5KaFmsWs0iG");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 2,
                column: "Contrasena",
                value: "$2a$11$7QQNy4dncNB/dAsETC.zUOYDBftCzYZyGTQO/MBf2VW1c/tkeq6zG");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 3,
                column: "Contrasena",
                value: "$2a$11$ViIzQ0FGdPbhUfo5baOlLeeHyYE0Pjm3UUt1KItj.yYUtZ8c..6l2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
