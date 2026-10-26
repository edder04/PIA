using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PIA.Datos
{

    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios => Set<Usuario>();


        //==============================================
        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=ED\\SQLEXPRESS;" +
                    "Database=piaMadAndPa;" +
                    "Trusted_Connection=True;" +
                    "TrustServerCertificate=True;");
            }
        }
        //==============================================

        //==============================================
        protected override void OnModelCreating(ModelBuilder modelBuilder){
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.UsuarioId);            //Llave Primaria
                entity.Property(u => u.UsuarioId)
                    .UseIdentityColumn(1, 1);               //Autoincrementable 1 en 1
                entity.Property(u => u.NombreUsuario)
                    .HasMaxLength(50)                       //Longitud de 50 Caracteres
                    .IsRequired();                          //Es Obligatorio y no puede ser Null
                entity.HasIndex(u => u.NombreUsuario)       //Crea un indice
                    .IsUnique();                            //El Indice es Unico y no se puede Repetir
                entity.Property(u => u.NombreCompleto)
                    .HasMaxLength(150)                      //Longitud de 150 Caracteres
                    .IsRequired();                          //Es Obligatorio y no puede ser Null
                entity.Property(u => u.PasswordHash)
                    .HasMaxLength(255)                      //Longitud de 255 Caracteres
                    .IsRequired();                          //Es Obligatorio y no puede ser Null
                entity.Property(u => u.Rol)
                    .IsRequired();                          //Es Obligatorio y no puede ser Null
                entity.ToTable(t => t.HasCheckConstraint(   //Aplicar Restricciones o Condiciones
                    "CK_Usuarios_Rol",                      //Nombre de la Restriccion
                    "[Rol] IN (1, 2)"));                    //Indica que solamente acepta los valores 1 y 2
                entity.Property(u => u.Activo)
                    .HasDefaultValue(true);                 //Automaticamente lo pone Activo
                entity.Property(u => u.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");       //Utiliza GETDATE del SQL
            });
        }
        //==============================================
    }
}
