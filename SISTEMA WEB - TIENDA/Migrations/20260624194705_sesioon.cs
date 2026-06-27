using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class sesioon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 2,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEI1F7g7qz72xOXNxCOwZyXwWKb4mMe/Vr9V0fs+2xX4M7hEazYY/ulz1AW2Qw9myKQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 2,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEAcHBa0eS72FMQlt12d6Bya8iHUY41DXvByq8fhPyWSGMMmMq8jW6draE/R4v1LoIg==");
        }
    }
}
