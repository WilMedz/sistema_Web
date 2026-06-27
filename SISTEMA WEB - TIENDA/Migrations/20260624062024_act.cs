using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class act : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 2,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEEHxsixMhZGQ64uYyLBPmS7kIedbpf19wzf0wZKnUeUrLokmh7nTVHTTH9PfXtvuIw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 2,
                column: "Contrasena",
                value: "AQAAAAIAAYagAAAAEE/tOXnQvYyzRJk64Y2CWCw1cuObjxjq5ED4vImYSmN3STnnsZ26vQexcpcRVgUfPw==");
        }
    }
}
