using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class nuevamigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DescripcionEstado",
                table: "EstadosPedido",
                newName: "NombreEstado");

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "CategoriaId", "NombreCategoria" },
                values: new object[,]
                {
                    { 1, "Casacas" },
                    { 2, "Polos" },
                    { 3, "Pantalones" },
                    { 4, "Vestidos" },
                    { 5, "Accesorios" }
                });

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 3,
                column: "Nombres",
                value: "Cliente Prueba");

            migrationBuilder.InsertData(
                table: "DireccionesEnvio",
                columns: new[] { "DireccionId", "CalleAvenida", "ClientesId", "Distrito", "Referencia" },
                values: new object[] { 1, "Jr. Principal 123", 1, "Cajamarca", "Dirección por defecto del sistema" });

            migrationBuilder.InsertData(
                table: "EstadosPedido",
                columns: new[] { "EstadoId", "NombreEstado" },
                values: new object[,]
                {
                    { 1, "Pendiente" },
                    { 2, "Confirmado" },
                    { 3, "En camino" },
                    { 4, "Entregado" },
                    { 5, "Cancelado" }
                });

            migrationBuilder.InsertData(
                table: "Marcas",
                columns: new[] { "MarcaId", "NombreMarca" },
                values: new object[,]
                {
                    { 1, "Aleana" },
                    { 2, "Sin Marca" }
                });

            migrationBuilder.InsertData(
                table: "MetodosPago",
                columns: new[] { "MetodoPagoId", "DescripcionPago" },
                values: new object[,]
                {
                    { 1, "Yape / Plin" },
                    { 2, "Tarjeta" },
                    { 3, "Efectivo" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "CategoriaId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "CategoriaId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "CategoriaId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "CategoriaId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "CategoriaId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "DireccionesEnvio",
                keyColumn: "DireccionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EstadosPedido",
                keyColumn: "EstadoId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EstadosPedido",
                keyColumn: "EstadoId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EstadosPedido",
                keyColumn: "EstadoId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EstadosPedido",
                keyColumn: "EstadoId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EstadosPedido",
                keyColumn: "EstadoId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Marcas",
                keyColumn: "MarcaId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Marcas",
                keyColumn: "MarcaId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MetodosPago",
                keyColumn: "MetodoPagoId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MetodosPago",
                keyColumn: "MetodoPagoId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MetodosPago",
                keyColumn: "MetodoPagoId",
                keyValue: 3);

            migrationBuilder.RenameColumn(
                name: "NombreEstado",
                table: "EstadosPedido",
                newName: "DescripcionEstado");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "ClienteId",
                keyValue: 3,
                column: "Nombres",
                value: "Cliente Demo");
        }
    }
}
