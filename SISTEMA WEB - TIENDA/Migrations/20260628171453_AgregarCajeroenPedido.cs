using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCajeroenPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CajeroId",
                table: "Pedidos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_CajeroId",
                table: "Pedidos",
                column: "CajeroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes_CajeroId",
                table: "Pedidos",
                column: "CajeroId",
                principalTable: "Clientes",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes_CajeroId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_CajeroId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "CajeroId",
                table: "Pedidos");
        }
    }
}
