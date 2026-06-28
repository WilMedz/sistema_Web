using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SISTEMA_WEB___TIENDA.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCategoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.CategoriaId);
                });

            migrationBuilder.CreateTable(
                name: "Ciudades",
                columns: table => new
                {
                    CiudadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCiudad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ciudades", x => x.CiudadId);
                });

            migrationBuilder.CreateTable(
                name: "EstadosPedido",
                columns: table => new
                {
                    EstadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEstado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosPedido", x => x.EstadoId);
                });

            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    MarcaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreMarca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.MarcaId);
                });

            migrationBuilder.CreateTable(
                name: "MetodosPago",
                columns: table => new
                {
                    MetodoPagoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescripcionPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodosPago", x => x.MetodoPagoId);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    ProveedorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazonSocial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RUC = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Contacto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.ProveedorId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreRol = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RolId);
                });

            migrationBuilder.CreateTable(
                name: "Prendas",
                columns: table => new
                {
                    PrendaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombrePrenda = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecioLista = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecioOferta = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ImagenPrincipalUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    MarcaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prendas", x => x.PrendaId);
                    table.ForeignKey(
                        name: "FK_Prendas_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "CategoriaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prendas_Marcas_MarcaId",
                        column: x => x.MarcaId,
                        principalTable: "Marcas",
                        principalColumn: "MarcaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CiudadId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClienteId);
                    table.ForeignKey(
                        name: "FK_Clientes_Ciudades_CiudadId",
                        column: x => x.CiudadId,
                        principalTable: "Ciudades",
                        principalColumn: "CiudadId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clientes_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VariantesPrenda",
                columns: table => new
                {
                    VarianteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Talla = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    PrendaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantesPrenda", x => x.VarianteId);
                    table.ForeignKey(
                        name: "FK_VariantesPrenda_Prendas_PrendaId",
                        column: x => x.PrendaId,
                        principalTable: "Prendas",
                        principalColumn: "PrendaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DireccionesEnvio",
                columns: table => new
                {
                    DireccionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalleAvenida = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Distrito = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ClientesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DireccionesEnvio", x => x.DireccionId);
                    table.ForeignKey(
                        name: "FK_DireccionesEnvio_Clientes_ClientesId",
                        column: x => x.ClientesId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    PedidoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaPedido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalCompra = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ClientesId = table.Column<int>(type: "int", nullable: false),
                    DireccionId = table.Column<int>(type: "int", nullable: false),
                    MetodoPagoId = table.Column<int>(type: "int", nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.PedidoId);
                    table.ForeignKey(
                        name: "FK_Pedidos_Clientes_ClientesId",
                        column: x => x.ClientesId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_DireccionesEnvio_DireccionId",
                        column: x => x.DireccionId,
                        principalTable: "DireccionesEnvio",
                        principalColumn: "DireccionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_EstadosPedido_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "EstadosPedido",
                        principalColumn: "EstadoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pedidos_MetodosPago_MetodoPagoId",
                        column: x => x.MetodoPagoId,
                        principalTable: "MetodosPago",
                        principalColumn: "MetodoPagoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesPedido",
                columns: table => new
                {
                    DetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    VarianteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesPedido", x => x.DetalleId);
                    table.ForeignKey(
                        name: "FK_DetallesPedido_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "PedidoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesPedido_VariantesPrenda_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "VariantesPrenda",
                        principalColumn: "VarianteId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "Ciudades",
                columns: new[] { "CiudadId", "NombreCiudad" },
                values: new object[] { 1, "Cajamarca" });

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

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RolId", "NombreRol" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Cliente" },
                    { 3, "Cajero" }
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "ClienteId", "CiudadId", "Contrasena", "CorreoElectronico", "FechaNacimiento", "Nombres", "RolId" },
                values: new object[,]
                {
                    { 1, 1, "$2a$11$dIhzGeX6bj/aLtA6W9SBIe.eq7waJlULyimU0VTFC87LQrnTWNoAK", "admin@aleanastore.com", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Administrador Aleana Store", 1 },
                    { 2, 1, "$2a$11$p45sXe/0Nm0WMRQFCeg7vuqwz6IBLLHW61bToDgBV7u1Wi5uxUFM6", "cajero@aleanastore.com", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cajero Aleana Store", 3 },
                    { 3, 1, "$2a$11$o1xX0V900pVHmpGfM7TFwetAnzIwz58gzQQKA5IMi5Klyx8LibLSG", "cliente@aleanastore.com", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cliente Prueba", 2 }
                });

            migrationBuilder.InsertData(
                table: "DireccionesEnvio",
                columns: new[] { "DireccionId", "CalleAvenida", "ClientesId", "Distrito", "Referencia" },
                values: new object[] { 1, "Jr. Principal 123", 1, "Cajamarca", "Dirección por defecto" });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CiudadId",
                table: "Clientes",
                column: "CiudadId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_RolId",
                table: "Clientes",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedido_PedidoId",
                table: "DetallesPedido",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedido_VarianteId",
                table: "DetallesPedido",
                column: "VarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_DireccionesEnvio_ClientesId",
                table: "DireccionesEnvio",
                column: "ClientesId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClientesId",
                table: "Pedidos",
                column: "ClientesId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_DireccionId",
                table: "Pedidos",
                column: "DireccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_EstadoId",
                table: "Pedidos",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_MetodoPagoId",
                table: "Pedidos",
                column: "MetodoPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_Prendas_CategoriaId",
                table: "Prendas",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Prendas_MarcaId",
                table: "Prendas",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantesPrenda_PrendaId",
                table: "VariantesPrenda",
                column: "PrendaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesPedido");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "VariantesPrenda");

            migrationBuilder.DropTable(
                name: "DireccionesEnvio");

            migrationBuilder.DropTable(
                name: "EstadosPedido");

            migrationBuilder.DropTable(
                name: "MetodosPago");

            migrationBuilder.DropTable(
                name: "Prendas");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Marcas");

            migrationBuilder.DropTable(
                name: "Ciudades");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
