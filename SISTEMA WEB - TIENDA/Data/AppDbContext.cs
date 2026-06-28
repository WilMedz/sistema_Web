using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Models;

namespace SISTEMA_WEB___TIENDA.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Roles> Roles { get; set; }
        public DbSet<Ciudad> Ciudades { get; set; }
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<DireccionEnvio> DireccionesEnvio { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Prenda> Prendas { get; set; }
        public DbSet<VariantePrenda> VariantesPrenda { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }
        public DbSet<EstadoPedido> EstadosPedido { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<RegistroCaja> RegistrosCaja { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Prenda>().Property(p => p.PrecioLista).HasPrecision(18, 2);
            modelBuilder.Entity<Prenda>().Property(p => p.PrecioOferta).HasPrecision(18, 2);
            modelBuilder.Entity<Pedido>().Property(p => p.TotalCompra).HasPrecision(18, 2);
            modelBuilder.Entity<DetallePedido>().Property(d => d.PrecioUnitario).HasPrecision(18, 2);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.DireccionEnvio)
                .WithMany(d => d.Pedidos)
                .HasForeignKey(p => p.DireccionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Clientes)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.ClientesId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Roles>().HasData(
                new Roles { RolId = 1, NombreRol = "Administrador" },
                new Roles { RolId = 2, NombreRol = "Cliente" },
                new Roles { RolId = 3, NombreRol = "Cajero" } );

            modelBuilder.Entity<Ciudad>().HasData(
                new Ciudad { CiudadId = 1, NombreCiudad = "Cajamarca" } );

            modelBuilder.Entity<Categoria>().HasData(
            new Categoria { CategoriaId = 1, NombreCategoria = "Casacas" },
            new Categoria { CategoriaId = 2, NombreCategoria = "Polos" },
            new Categoria { CategoriaId = 3, NombreCategoria = "Pantalones" },
            new Categoria { CategoriaId = 4, NombreCategoria = "Vestidos" },
            new Categoria { CategoriaId = 5, NombreCategoria = "Accesorios" });


            modelBuilder.Entity<Marca>().HasData(
            new Marca { MarcaId = 1, NombreMarca = "Aleana" },
            new Marca { MarcaId = 2, NombreMarca = "Sin Marca" } );

            modelBuilder.Entity<MetodoPago>().HasData(
            new MetodoPago { MetodoPagoId = 1, DescripcionPago = "Yape / Plin" },
            new MetodoPago { MetodoPagoId = 2, DescripcionPago = "Tarjeta" },
            new MetodoPago { MetodoPagoId = 3, DescripcionPago = "Efectivo" });

            modelBuilder.Entity<EstadoPedido>().HasData(
            new EstadoPedido { EstadoId = 1, NombreEstado = "Pendiente" },
            new EstadoPedido { EstadoId = 2, NombreEstado = "Confirmado" },
            new EstadoPedido { EstadoId = 3, NombreEstado = "En camino" },
            new EstadoPedido { EstadoId = 4, NombreEstado = "Entregado" },
            new EstadoPedido { EstadoId = 5, NombreEstado = "Cancelado" });

            modelBuilder.Entity<DireccionEnvio>().HasData(
            new DireccionEnvio
            {
               DireccionId = 1,
               CalleAvenida = "Jr. Principal 123",
               Distrito = "Cajamarca",
               Referencia = "Dirección por defecto",
               ClientesId = 1  // Admin
            } );


            //contraseñas hasheadas

            modelBuilder.Entity<Clientes>().HasData(
            new Clientes
            {
                ClienteId = 1,
                Nombres = "Administrador Aleana Store",
                CorreoElectronico = "admin@aleanastore.com",
                Contrasena = "$2a$11$dIhzGeX6bj/aLtA6W9SBIe.eq7waJlULyimU0VTFC87LQrnTWNoAK",//Admin2026
                CiudadId = 1,
                RolId = 1,
                FechaNacimiento = new DateTime(2000, 1, 1)
            },
            new Clientes
            {
                ClienteId = 2,
                Nombres = "Cajero Aleana Store",
                CorreoElectronico = "cajero@aleanastore.com",
                Contrasena = "$2a$11$p45sXe/0Nm0WMRQFCeg7vuqwz6IBLLHW61bToDgBV7u1Wi5uxUFM6",//Cajero2026
                CiudadId = 1,
                RolId = 3,
                FechaNacimiento = new DateTime(2000, 1, 1)
            },
            new Clientes
            {
                ClienteId = 3,
                Nombres = "Cliente Prueba",
                CorreoElectronico = "cliente@aleanastore.com",
                Contrasena = "$2a$11$o1xX0V900pVHmpGfM7TFwetAnzIwz58gzQQKA5IMi5Klyx8LibLSG",//Cliente2026
                CiudadId = 1,
                RolId = 2,
                FechaNacimiento = new DateTime(2000, 1, 1)
            }
        );
        
        }

    }
}
