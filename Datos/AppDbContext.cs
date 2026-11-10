
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PIA.Datos{                                                                                    // Indico que este código pertenece al proyecto PIA.
    public class AppDbContext : DbContext                                                               // Creo la clase que me permite conectar mi programa con la base de datos.
    {
        //==============================================
        protected override void OnConfiguring(                                                          // Configuro la conexión que voy a utilizar para acceder a la base de datos.
            DbContextOptionsBuilder optionsBuilder)                                                     // Recibo las opciones que se utilizan para configurar la conexión.
        {
            if (!optionsBuilder.IsConfigured)                                                           // Compruebo si la conexión todavía no ha sido configurada.
            {
                optionsBuilder.UseSqlServer(                                                            // Indico que voy a utilizar SQL Server como motor de base de datos.
                    "Server=Edder\\SQLEXPRESS;" +                                                          // Indico el nombre del servidor y la instancia de SQL Server.
                    "Database=PIA_V3;" +                                                           // Indico el nombre de la base de datos a la que me voy a conectar.
                    "Trusted_Connection=True;" +                                                        // Utilizo la autenticación de Windows para conectarme al servidor.
                    "TrustServerCertificate=True;");                                                    // Permito confiar en el certificado del servidor durante la conexión.
            }
        }
        public DbSet<Usuario> Usuario => Set<Usuario>();                                                // Represento la tabla Usuario para poder consultar y trabajar con sus registros.
		public DbSet<AuditoriaUsuario> AuditoriaUsuario => Set<AuditoriaUsuario>();
        public DbSet<Cliente> Cliente => Set<Cliente>();
		public DbSet<AuditoriaCliente> AuditoriaCliente => Set<AuditoriaCliente>();
		public DbSet<Estadio> Estadio => Set<Estadio>();
		public DbSet<Zona> Zona => Set<Zona>();
		public DbSet<Partido> Partido => Set<Partido>();
		public DbSet<Venta> Venta => Set<Venta>();
		public DbSet<DetalleVenta> DetalleVenta => Set<DetalleVenta>();
		public DbSet<Boleto> Boleto => Set<Boleto>();
		public DbSet<Factura> Factura => Set<Factura>();
		public DbSet<Cancelacion> Cancelacion => Set<Cancelacion>();
       public DbSet<vw_Usuarios> vw_Usuarios => Set<vw_Usuarios>();
		public DbSet<vw_Clientes> vw_Clientes => Set <vw_Clientes> ();
		public DbSet<vw_Estadios> vw_Estadios => Set<vw_Estadios>();
		public DbSet<vw_Partidos> vw_Partidos => Set<vw_Partidos>();
		public DbSet<vw_DisponibilidadZonas> vw_DisponibilidadZonas => Set<vw_DisponibilidadZonas>();
		public DbSet<vw_Ventas> vw_Ventas => Set<vw_Ventas>();
		public DbSet<vw_BoletosVendidos> vw_BoletosVendidos => Set<vw_BoletosVendidos>();
		public DbSet<vw_Facturas> vw_Facturas => Set<vw_Facturas>();
		public DbSet<vw_Cancelaciones> vw_Cancelaciones => Set<vw_Cancelaciones>();
		public DbSet<vw_IngresosPorPartido> vw_IngresosPorPartido => Set<vw_IngresosPorPartido>();
		public DbSet<vw_AsientosOcupados> vw_AsientosOcupados => Set<vw_AsientosOcupados>();
		public DbSet<vw_AuditoriaUsuarios> vw_AuditoriaUsuarios => Set<vw_AuditoriaUsuarios>();
		public DbSet<vw_AuditoriaClientes> vw_AuditoriaClientes => Set<vw_AuditoriaClientes>();

		//==============================================
		// Mapeo de la tabla Usuario
		protected override void OnModelCreating(ModelBuilder modelBuilder)                              // Configuro cómo se relaciona la clase Usuario con la tabla de SQL Server.
        {
            base.OnModelCreating(modelBuilder);                                                         // Ejecuto la configuración básica que proporciona Entity Framework Core.

			modelBuilder.Entity<Usuario>(entity =>
			{
				entity.ToTable("Usuarios", t => {
					t.HasCheckConstraint("CK_Usuarios_Rol", "[Rol] IN ('Administrador', 'Operativo')");
					t.HasTrigger("trg_AuditoriaUsuarios_Update");
				});
				entity.HasKey(e => e.IdUsuario);
				entity.Property(e => e.IdUsuario).HasColumnName("IdUsuario").ValueGeneratedOnAdd();
				entity.Property(e => e.Nombres).HasColumnName("Nombre(s)").HasColumnType("varchar(100)").HasMaxLength(100).IsRequired();
				entity.Property(e => e.ApPaterno).HasColumnType("varchar(100)").HasMaxLength(100).IsRequired();
				entity.Property(e => e.ApMaterno).HasColumnType("varchar(100)").HasMaxLength(100);
				entity.Property(e => e.FechaNacimiento).HasColumnType("date").IsRequired();
				entity.Property(e => e.NumeroNomina).HasColumnType("nvarchar(50)").HasMaxLength(50).IsRequired();
				entity.HasIndex(e => e.NumeroNomina).IsUnique();
				entity.Property(e => e.CorreoElectronico).HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired();
				entity.HasIndex(e => e.CorreoElectronico).IsUnique();
				entity.Property(e => e.Rol).HasColumnType("varchar(50)").HasMaxLength(50);
				entity.Property(e => e.NombreUsuario).HasColumnType("varchar(100)").HasMaxLength(100).IsRequired();
				entity.Property(e => e.Contraseña).HasColumnName("Contraseña").HasColumnType("nvarchar(255)").HasMaxLength(255).IsRequired();
				entity.Property(e => e.Activo).HasDefaultValue(true).IsRequired();
				entity.Property(e => e.CreadoPor).HasColumnType("int");
				entity.Property(e => e.FechaRegistro).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();
			});

			modelBuilder.Entity<AuditoriaUsuario>(entity =>
			{
				entity.ToTable("AuditoriaUsuario");
				entity.HasKey(e => e.IdAuditoria);
				entity.Property(e => e.IdAuditoria).ValueGeneratedOnAdd();
				entity.Property(e => e.IdUsuario).HasColumnType("varchar(100)").HasMaxLength(100);
				entity.Property(e => e.DatoACambiar).HasColumnType("nvarchar(255)").HasMaxLength(255);
				entity.Property(e => e.DatoAnterior).HasColumnType("nvarchar(max)");
				entity.Property(e => e.DatoNuevo).HasColumnType("nvarchar(max)");
				entity.Property(e => e.ModificadoPor).IsRequired();
				entity.Property(e => e.FechaModificacion).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();
				entity.HasOne<Usuario>().WithMany().HasForeignKey(e => e.ModificadoPor).HasConstraintName("FK_Usuario_Modifico_Usuario");
			});



			modelBuilder.Entity<Cliente>(entity =>
			{
				entity.ToTable("Clientes", t =>
				{
				t.HasCheckConstraint("CHK_RFC_Longitud", "LEN([RFC]) IN (12, 13)");
				t.HasTrigger("trg_AuditoriaClientes_Update");

				});
				entity.HasKey(e => e.IdCliente);
				entity.Property(e => e.IdCliente).ValueGeneratedOnAdd();
				entity.Property(e => e.Nombres).HasColumnName("Nombre(s)").HasColumnType("nvarchar(150)").HasMaxLength(150).IsRequired();
				entity.Property(e => e.ApPaterno).HasColumnType("varchar(100)").HasMaxLength(100).IsRequired();
				entity.Property(e => e.ApMaterno).HasColumnType("varchar(100)").HasMaxLength(100);
				entity.Property(e => e.FechaNacimiento).HasColumnType("date").IsRequired();
				entity.Property(e => e.Domicilio).HasColumnType("nvarchar(250)").HasMaxLength(250).IsRequired();
				entity.Property(e => e.RFC).HasColumnType("nvarchar(13)").HasMaxLength(13).IsRequired();
				entity.HasIndex(e => e.RFC).IsUnique();
				entity.Property(e => e.CorreoElectronico).HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired();
				entity.Property(e => e.TelefonoCasa).HasColumnType("nvarchar(15)").HasMaxLength(15);
				entity.Property(e => e.TelefonoCelular).HasColumnType("nvarchar(15)").HasMaxLength(15).IsRequired();
				entity.Property(e => e.Activo).HasDefaultValue(true).IsRequired();
				entity.Property(e => e.CreadoPor).IsRequired();
				entity.Property(e => e.FechaHoraRegistro).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();
				entity.HasOne<Usuario>().WithMany().HasForeignKey(e => e.CreadoPor).HasConstraintName("FK_Clientes_Usuarios_Creador");
			});

	
			modelBuilder.Entity<AuditoriaCliente>(entity =>
			{
				entity.ToTable("AuditoriaCliente");
				entity.HasKey(e => e.IdAuditoria);
				entity.Property(e => e.IdAuditoria).ValueGeneratedOnAdd();
				entity.Property(e => e.IdCliente).HasColumnType("varchar(100)").HasMaxLength(100);
				entity.Property(e => e.DatoACambiar).HasColumnType("nvarchar(255)").HasMaxLength(255);
				entity.Property(e => e.DatoAnterior).HasColumnType("nvarchar(max)");
				entity.Property(e => e.DatoNuevo).HasColumnType("nvarchar(max)");
				entity.Property(e => e.ModificadoPor).IsRequired();
				entity.Property(e => e.FechaModificacion).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();
				entity.HasOne<Usuario>().WithMany().HasForeignKey(e => e.ModificadoPor).HasConstraintName("FK_Usuario_Modifico_Cliente");
			});

			modelBuilder.Entity<Estadio>(entity =>
			{
				entity.ToTable("Estadios", t => t.HasCheckConstraint("CK_Estadios_CapacidadTotal", "[CapacidadTotal] > 0"));
				entity.HasKey(e => e.IdEstadio);
				entity.Property(e => e.IdEstadio).ValueGeneratedOnAdd();
				entity.Property(e => e.Nombre).HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired();
				entity.HasIndex(e => e.Nombre).IsUnique();
				entity.Property(e => e.Ciudad).HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired();
				entity.Property(e => e.CapacidadTotal).IsRequired();
				entity.Property(e => e.MapaEstadio).HasColumnType("varchar(500)").HasMaxLength(500);
				entity.Property(e => e.CreadoPor).IsRequired();
				entity.Property(e => e.FechaHoraRegistro).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();
				entity.HasOne<Usuario>().WithMany().HasForeignKey(e => e.CreadoPor).HasConstraintName("FK_Estadios_Usuarios");
			});

			modelBuilder.Entity<Zona>(entity =>
			{
				entity.ToTable("Zonas", t =>
				{
					t.HasCheckConstraint("CK_Zonas_PrecioBase", "[PrecioBase] >= 0");
					t.HasCheckConstraint("CK_Zonas_CupoZona", "[CupoZona] > 0");
				});
				entity.HasKey(e => e.IdZona);
				entity.Property(e => e.IdZona).ValueGeneratedOnAdd();
				entity.Property(e => e.IdEstadio).IsRequired();
				entity.Property(e => e.NombreZona).HasColumnType("nvarchar(50)").HasMaxLength(50).IsRequired();
				entity.Property(e => e.PrecioBase).HasColumnType("decimal(10,2)").IsRequired();
				entity.Property(e => e.CupoZona).IsRequired();
				entity.Property(e => e.CreadoPor).IsRequired();
				entity.Property(e => e.FechaHoraRegistro).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();
				entity.HasIndex(e => new { e.IdEstadio, e.NombreZona }).IsUnique().HasDatabaseName("UQ_Zona_Estadio");
				entity.HasOne<Estadio>().WithMany().HasForeignKey(e => e.IdEstadio).HasConstraintName("FK_Zonas_Estadios");
				entity.HasOne<Usuario>().WithMany().HasForeignKey(e => e.CreadoPor).HasConstraintName("FK_Zonas_Usuarios");
			});

			modelBuilder.Entity<Partido>(entity =>
			{
				entity.ToTable("Partidos", t => t.HasCheckConstraint("CHK_EstadoPartido", "[Estado] IN ('Programado', 'Terminado', 'Cancelado')"));
				entity.HasKey(e => e.IdPartido);
				entity.Property(e => e.IdPartido).ValueGeneratedOnAdd();
				entity.Property(e => e.IdEstadio).IsRequired();
				entity.Property(e => e.FechaHora).HasColumnType("datetime").IsRequired();
				entity.Property(e => e.EquipoLocal).HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired();
				entity.Property(e => e.EquipoVisitante).HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired();
				entity.Property(e => e.Estado).HasColumnType("nvarchar(20)").HasMaxLength(20).HasDefaultValue("Programado").IsRequired();
				entity.Property(e => e.CreadoPor).IsRequired();
				entity.Property(e => e.FechaHoraRegistro).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();
				entity.HasOne<Estadio>().WithMany().HasForeignKey(e => e.IdEstadio).HasConstraintName("FK_Partidos_Estadios");
				entity.HasOne<Usuario>().WithMany().HasForeignKey(e => e.CreadoPor).HasConstraintName("FK_Partidos_Usuarios");
			});

			modelBuilder.Entity<Venta>(entity =>
			{
				entity.ToTable("Ventas", t =>
				{
					t.HasCheckConstraint("CHK_MetodoPago", "[MetodoPago] IN ('Crédito', 'Débito', 'Efectivo')");
					t.HasCheckConstraint("CHK_EstadoVenta", "[EstadoVenta] IN ('Activa', 'Cancelada')");
				});
				entity.HasKey(e => e.IdVenta);
				entity.Property(e => e.IdVenta).ValueGeneratedOnAdd();
				entity.Property(e => e.Folio).HasColumnType("uniqueidentifier").HasDefaultValueSql("NEWID()").IsRequired();
				entity.HasIndex(e => e.Folio).IsUnique();
				entity.Property(e => e.IdCliente).IsRequired();
				entity.Property(e => e.IdUsuario).IsRequired();
				entity.Property(e => e.FechaHoraVenta).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();
				entity.Property(e => e.Subtotal).HasColumnType("decimal(10,2)").IsRequired();
				entity.Property(e => e.IVA).HasColumnType("decimal(10,2)").IsRequired();
				entity.Property(e => e.Total).HasColumnType("decimal(10,2)").IsRequired();
				entity.Property(e => e.MetodoPago).HasColumnType("nvarchar(20)").HasMaxLength(20).IsRequired();
				entity.Property(e => e.EstadoVenta).HasColumnType("nvarchar(20)").HasMaxLength(20).HasDefaultValue("Activa").IsRequired();
				entity.Property(e => e.TarjetaTitular).HasColumnType("nvarchar(150)").HasMaxLength(150);
				entity.Property(e => e.TarjetaNumero).HasColumnType("nvarchar(20)").HasMaxLength(20);
				entity.Property(e => e.TarjetaExpiracion).HasColumnType("nvarchar(5)").HasMaxLength(5);
				entity.Property(e => e.TarjetaCVV).HasColumnType("nvarchar(4)").HasMaxLength(4);
				entity.HasOne<Cliente>().WithMany().HasForeignKey(e => e.IdCliente).HasConstraintName("FK_Ventas_Clientes");
				entity.HasOne<Usuario>().WithMany().HasForeignKey(e => e.IdUsuario).HasConstraintName("FK_Ventas_Usuarios");
			});

			modelBuilder.Entity<DetalleVenta>(entity =>
			{
				entity.ToTable("DetalleVenta");
				entity.HasKey(e => e.IdDetalleVenta);
				entity.Property(e => e.IdDetalleVenta).ValueGeneratedOnAdd();
				entity.Property(e => e.IdVenta).IsRequired();
				entity.Property(e => e.IdPartido).IsRequired();
				entity.Property(e => e.IdZona).IsRequired();
				entity.Property(e => e.Cantidad).IsRequired();
				entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10,2)").IsRequired();
				entity.Property(e => e.Subtotal).HasColumnType("decimal(10,2)").IsRequired();
				entity.Property(e => e.IVA).HasColumnType("decimal(10,2)").IsRequired();
				entity.Property(e => e.Total).HasColumnType("decimal(10,2)").IsRequired();
				entity.HasOne<Venta>().WithMany().HasForeignKey(e => e.IdVenta).HasConstraintName("FK_DetalleVenta_Venta");
				entity.HasOne<Partido>().WithMany().HasForeignKey(e => e.IdPartido).HasConstraintName("FK_DetalleVenta_Partido");
				entity.HasOne<Zona>().WithMany().HasForeignKey(e => e.IdZona).HasConstraintName("FK_DetalleVenta_Zona");
			});

			modelBuilder.Entity<Boleto>(entity =>
			{
				entity.ToTable("Boletos", t => t.HasCheckConstraint("CHK_EstadoBoleto", "[EstadoBoleto] IN ('Vendido', 'Cancelado')"));
				entity.HasKey(e => e.IdBoleto);
				entity.Property(e => e.IdBoleto).ValueGeneratedOnAdd();
				entity.Property(e => e.IdVenta).IsRequired();
				entity.Property(e => e.IdPartido).IsRequired();
				entity.Property(e => e.IdZona).IsRequired();
				entity.Property(e => e.Fila).HasColumnType("nvarchar(10)").HasMaxLength(10).IsRequired();
				entity.Property(e => e.Asiento).HasColumnType("nvarchar(10)").HasMaxLength(10).IsRequired();
				entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10,2)").IsRequired();
				entity.Property(e => e.EstadoBoleto).HasColumnType("nvarchar(20)").HasMaxLength(20).HasDefaultValue("Vendido").IsRequired();
				entity.HasIndex(e => new { e.IdPartido, e.IdZona, e.Fila, e.Asiento }).IsUnique().HasDatabaseName("UQ_Asiento_Partido");
				entity.HasOne<Venta>().WithMany().HasForeignKey(e => e.IdVenta).HasConstraintName("FK_Boletos_Ventas");
				entity.HasOne<Partido>().WithMany().HasForeignKey(e => e.IdPartido).HasConstraintName("FK_Boletos_Partidos");
				entity.HasOne<Zona>().WithMany().HasForeignKey(e => e.IdZona).HasConstraintName("FK_Boletos_Zonas");
			});

			modelBuilder.Entity<Factura>(entity =>
			{
				entity.ToTable("Facturas");
				entity.HasKey(e => e.IdFactura);
				entity.Property(e => e.IdFactura).ValueGeneratedOnAdd();
				entity.Property(e => e.IdVenta).IsRequired();
				entity.HasIndex(e => e.IdVenta).IsUnique();
				entity.Property(e => e.FolioFactura).HasColumnType("nvarchar(50)").HasMaxLength(50).IsRequired();
				entity.HasIndex(e => e.FolioFactura).IsUnique();
				entity.Property(e => e.FechaHoraEmision).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();
				entity.Property(e => e.EmisorNombre).HasColumnType("nvarchar(150)").HasMaxLength(150).IsRequired();
				entity.Property(e => e.EmisorRFC).HasColumnType("nvarchar(13)").HasMaxLength(13).IsRequired();
				entity.Property(e => e.EmisorDomicilio).HasColumnType("nvarchar(250)").HasMaxLength(250).IsRequired();
				entity.Property(e => e.ReceptorNombre).HasColumnType("nvarchar(150)").HasMaxLength(150).IsRequired();
				entity.Property(e => e.ReceptorRFC).HasColumnType("nvarchar(13)").HasMaxLength(13).IsRequired();
				entity.Property(e => e.ReceptorDomicilio).HasColumnType("nvarchar(250)").HasMaxLength(250).IsRequired();
				entity.Property(e => e.UsoCFDI).HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired();
				entity.Property(e => e.MetodoPago).HasColumnType("nvarchar(20)").HasMaxLength(20).IsRequired();
				entity.Property(e => e.Subtotal).HasColumnType("decimal(10,2)").IsRequired();
				entity.Property(e => e.IVA).HasColumnType("decimal(10,2)").IsRequired();
				entity.Property(e => e.Total).HasColumnType("decimal(10,2)").IsRequired();
				entity.HasOne<Venta>().WithMany().HasForeignKey(e => e.IdVenta).HasConstraintName("FK_Facturas_Ventas");
			});

			modelBuilder.Entity<Cancelacion>(entity =>
			{
				entity.ToTable("Cancelaciones");
				entity.HasKey(e => e.IdCancelacion);
				entity.Property(e => e.IdCancelacion).ValueGeneratedOnAdd();
				entity.Property(e => e.IdVenta).IsRequired();
				entity.HasIndex(e => e.IdVenta).IsUnique();
				entity.Property(e => e.IdUsuario).IsRequired();
				entity.Property(e => e.FechaHoraCancelacion).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();
				entity.Property(e => e.Motivo).HasColumnType("nvarchar(max)").IsRequired();
				entity.HasOne<Venta>().WithMany().HasForeignKey(e => e.IdVenta).HasConstraintName("FK_Cancelaciones_Ventas");
				entity.HasOne<Usuario>().WithMany().HasForeignKey(e => e.IdUsuario).HasConstraintName("FK_Cancelaciones_Usuarios");
			});

			modelBuilder.Entity<vw_Usuarios>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_Usuarios", "dbo");
			});
			modelBuilder.Entity<vw_Clientes>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_Clientes", "dbo");
			});
			modelBuilder.Entity<vw_Estadios>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_Estadios", "dbo");
			});
			modelBuilder.Entity<vw_Partidos>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_Partidos", "dbo");
			});
			modelBuilder.Entity<vw_DisponibilidadZonas>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_DisponibilidadZonas", "dbo");
			});
			modelBuilder.Entity<vw_Ventas>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_Ventas", "dbo");
			});
			modelBuilder.Entity<vw_BoletosVendidos>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_BoletosVendidos", "dbo");
			});
			modelBuilder.Entity<vw_Facturas>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_Facturas", "dbo");
			});
			modelBuilder.Entity<vw_Cancelaciones>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_Cancelaciones", "dbo");
			});
			modelBuilder.Entity<vw_IngresosPorPartido>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_IngresosPorPartido", "dbo");
			});
			modelBuilder.Entity<vw_AsientosOcupados>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_AsientosOcupados", "dbo");
			});
			modelBuilder.Entity<vw_AuditoriaUsuarios>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_AuditoriaUsuarios", "dbo");
			});
			modelBuilder.Entity<vw_AuditoriaClientes>(entity =>
			{
				entity.HasNoKey();
				entity.ToView("vw_AuditoriaClientes", "dbo");
			});





		}
	}
}
