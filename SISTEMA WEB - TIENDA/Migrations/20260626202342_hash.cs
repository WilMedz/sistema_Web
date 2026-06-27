using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class hash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 1,
                column: "Contrasena",
                value: "$2a$11$dIhzGeX6bj/aLtA6W9SBIe.eq7waJlULyimU0VTFC87LQrnTWNoAK");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 2,
                column: "Contrasena",
                value: "$2a$11$p45sXe/0Nm0WMRQFCeg7vuqwz6IBLLHW61bToDgBV7u1Wi5uxUFM6");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 3,
                column: "Contrasena",
                value: "$2a$11$o1xX0V900pVHmpGfM7TFwetAnzIwz58gzQQKA5IMi5Klyx8LibLSG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
