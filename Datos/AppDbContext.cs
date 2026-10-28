
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
                    "Server=ED\\SQLEXPRESS;" +                                                          // Indico el nombre del servidor y la instancia de SQL Server.
                    "Database=piaMadAndPa;" +                                                           // Indico el nombre de la base de datos a la que me voy a conectar.
                    "Trusted_Connection=True;" +                                                        // Utilizo la autenticación de Windows para conectarme al servidor.
                    "TrustServerCertificate=True;");                                                    // Permito confiar en el certificado del servidor durante la conexión.
            }
        }
        public DbSet<Usuario> Usuario => Set<Usuario>();                                                // Represento la tabla Usuario para poder consultar y trabajar con sus registros.
        public DbSet<Cliente> Cliente => Set<Cliente>();
        public DbSet<vw_ConsultaUsuario> vw_ConsultaUsuario => Set<vw_ConsultaUsuario>();
		public DbSet<vw_ConsultaCliente> vw_ConsultaCliente => Set <vw_ConsultaCliente> ();

        //==============================================
        // Mapeo de la tabla Usuario
        protected override void OnModelCreating(ModelBuilder modelBuilder)                              // Configuro cómo se relaciona la clase Usuario con la tabla de SQL Server.
        {
            base.OnModelCreating(modelBuilder);                                                         // Ejecuto la configuración básica que proporciona Entity Framework Core.

            modelBuilder.Entity<Usuario>(entity => {                                                    // Indico que voy a configurar la estructura de la entidad Usuario.            
                entity.ToTable("Usuario");                                                              // Indico que la entidad se guarda en la tabla llamada Usuario.
                entity.HasKey(u => u.IdUsuario);                                                        // Establezco IdUsuario como la llave primaria de la tabla.

                entity.Property(u => u.IdUsuario)                                                       // Configuro la propiedad que identifica a cada usuario.
                    .HasColumnName("IdUsuario")                                                         // Indico que esta propiedad corresponde a la columna IdUsuario.
                    .ValueGeneratedOnAdd();                                                             // Indico que el valor se genera automáticamente al agregar un registro.

                entity.Property(u => u.Nombres)                                                         // Configuro la propiedad donde guardo los nombres del usuario.
                    .HasColumnName("Nombre(s)")                                                         // Indico que en la base de datos la columna se llama Nombre(s).
                    .HasMaxLength(100).IsRequired();                                                    // Establezco un máximo de 100 caracteres y hago que el campo sea obligatorio.

                entity.Property(u => u.ApPaterno)                                                       // Configuro la propiedad del apellido paterno.
                    .HasColumnName("ApPaterno")                                                         // Indico el nombre de la columna en la base de datos.
                    .HasMaxLength(100).IsRequired();                                                    // Permito hasta 100 caracteres y hago que el campo sea obligatorio.

                entity.Property(u => u.ApMaterno)                                                       // Configuro la propiedad del apellido materno.
                    .HasColumnName("ApMaterno")                                                         // Indico el nombre de la columna en la base de datos.
                    .HasMaxLength(100);                                                                 // Establezco un máximo de 100 caracteres para este campo.

                entity.Property(u => u.FechaNacimiento)                                                 // Configuro la propiedad de la fecha de nacimiento.
                    .HasColumnName("FechaNacimiento")                                                   // Indico el nombre de la columna en la base de datos.
                    .HasColumnType("date")                                                              // Indico que el tipo de dato será date, para guardar solamente la fecha.
                    .IsRequired();                                                                      // Hago que la fecha de nacimiento sea obligatoria.

                entity.Property(u => u.NumeroNomina)                                                    // Configuro la propiedad del número de nómina.
                    .HasColumnName("NumeroNomina")                                                      // Indico el nombre de la columna en la base de datos.
                    .HasMaxLength(30)                                                                   // Establezco un máximo de 30 caracteres.
                    .IsRequired();                                                                      // Hago que el número de nómina sea obligatorio.

                entity.HasIndex(u => u.NumeroNomina).IsUnique();                                        // Evito que dos usuarios tengan el mismo número de nómina.

                entity.Property(u => u.CorreoElectronico)                                               // Configuro la propiedad del correo electrónico.
                    .HasColumnName("CorreoElectronico")                                                 // Indico el nombre de la columna en la base de datos.
                    .HasMaxLength(100)                                                                  // Establezco un máximo de 100 caracteres.
                    .IsRequired();                                                                      // Hago que el correo electrónico sea obligatorio.

                entity.Property(u => u.Rol)                                                             // Configuro la propiedad que indica el rol del usuario.
                    .HasColumnName("Rol")                                                               // Indico el nombre de la columna en la base de datos.
                    .HasMaxLength(50);                                                                  // Establezco un máximo de 50 caracteres para el rol.

                entity.Property(u => u.NombreUsuario)                                                   // Configuro la propiedad del nombre de usuario para iniciar sesión.
                    .HasColumnName("NombreUsuario")                                                     // Indico el nombre de la columna en la base de datos.
                    .HasMaxLength(100)                                                                  // Establezco un máximo de 100 caracteres.
                    .IsRequired();                                                                      // Hago que el nombre de usuario sea obligatorio.

                entity.Property(u => u.PasswordHash)                                                    // Configuro la propiedad donde guardo el valor de la contraseña o su hash.
                    .HasColumnName("PasswordHash")                                                      // Indico el nombre de la columna en la base de datos.
                    .HasMaxLength(255)                                                                  // Establezco un máximo de 255 caracteres.
                    .IsRequired();                                                                      // Hago que este campo sea obligatorio.

                entity.Property(u => u.Activo)                                                          // Configuro la propiedad que indica si la cuenta está activa.
                    .HasColumnName("Activo")                                                            // Indico el nombre de la columna en la base de datos.
                    .IsRequired();                                                                      // Hago que el estado de la cuenta sea obligatorio.

                entity.Property(u => u.FechaCreacion)                                                   // Configuro la propiedad de la fecha de creación del registro.
                    .HasColumnName("FechaCreacion")                                                     // Indico el nombre de la columna en la base de datos.
                    .HasColumnType("datetime")                                                          // Indico que se guardará la fecha junto con la hora.
                    .IsRequired();                                                                      // Hago que la fecha de creación sea obligatoria.

                entity.Property(u => u.UsuarioRegistro)                                                 // Configuro la propiedad que guarda el identificador de quien registró al usuario.
                    .HasColumnName("UsuarioRegistró");                                                  // Indico el nombre de la columna correspondiente en la base de datos.
            });
			//==============================================
			// Mapeo de la tabla Cliente
			modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Cliente");
                entity.HasKey(c => c.IdCliente);
                entity.Property(c => c.IdCliente)
                    .ValueGeneratedOnAdd();
                entity.Property(c => c.Nombres)
                    .HasColumnName("Nombre(s)")
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(c => c.ApPaterno)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(c => c.ApMaterno)
                    .HasMaxLength(100);
                entity.Property(c => c.FechaNacimiento)
                    .HasColumnType("date")
                    .IsRequired();
                entity.Property(c => c.Domicilio)
                    .HasMaxLength(250)
                    .IsRequired();
                entity.Property(c => c.RFC)
                    .HasMaxLength(13)
                    .IsRequired();
                entity.HasIndex(c => c.RFC)
                    .IsUnique();
                entity.Property(c => c.CorreoElectronico)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(c => c.TelefonoCasa)
                    .HasMaxLength(15);
                entity.Property(c => c.Celular)
                    .HasMaxLength(15);
                entity.Property(c => c.Activo)
                    .IsRequired();
                entity.Property(c => c.FechaCreacion)
                    .HasColumnType("datetime")
                    .IsRequired();
                entity.Property(c => c.FechaModificacion)
                    .HasColumnType("datetime");

                // Las dos llaves foráneas hacia Usuario (opcional, la BD ya las valida)
                entity.HasOne<Usuario>().WithMany()
                    .HasForeignKey(c => c.UsuarioCreacion)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<Usuario>().WithMany()
                    .HasForeignKey(c => c.UsuarioModificacion)
                    .OnDelete(DeleteBehavior.Restrict);
                });
			//==============================================
			// Mapeo de la vista ConsultaCliente
			modelBuilder.Entity<vw_ConsultaCliente>(entity =>
            {
                entity.HasNoKey();                               
                entity.ToView("vw_ConsultaCliente", "dbo");
            });

			//==============================================
			// Mapeo de la vista ConsultaUsuario
			modelBuilder.Entity<vw_ConsultaUsuario>(entity =>
            {
                entity.HasNoKey();                               
                entity.ToView("vw_ConsultaUsuario", "dbo");
            });

        }
        

        
    }
}
