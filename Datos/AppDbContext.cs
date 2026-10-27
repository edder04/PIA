using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PIA.Datos
{

    public class AppDbContext : DbContext
    {
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
        public DbSet<Usuario> Usuario => Set<Usuario>();
        //==============================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");
                entity.HasKey(u => u.IdUsuario);
                entity.Property(u => u.IdUsuario)
                    .HasColumnName("IdUsuario")
                    .ValueGeneratedOnAdd();
                entity.Property(u => u.Nombres)
                    .HasColumnName("Nombre(s)")
                    .HasMaxLength(100).IsRequired();
                entity.Property(u => u.ApPaterno)
                    .HasColumnName("ApPaterno")
                    .HasMaxLength(100).IsRequired();
                entity.Property(u => u.ApMaterno)
                    .HasColumnName("ApMaterno")
                    .HasMaxLength(100);
                entity.Property(u => u.FechaNacimiento)
                    .HasColumnName("FechaNacimiento")
                    .HasColumnType("date")
                    .IsRequired();
                entity.Property(u => u.NumeroNomina)
                    .HasColumnName("NumeroNomina")
                    .HasMaxLength(30)
                    .IsRequired();
                entity.HasIndex(u => u.NumeroNomina).IsUnique();
                entity.Property(u => u.CorreoElectronico)
                    .HasColumnName("CorreoElectronico")
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(u => u.Rol)
                    .HasColumnName("Rol")
                    .HasMaxLength(50);
                entity.Property(u => u.NombreUsuario)
                    .HasColumnName("NombreUsuario")
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(u => u.PasswordHash)
                    .HasColumnName("PasswordHash")
                    .HasMaxLength(255)
                    .IsRequired();
                entity.Property(u => u.Activo)
                    .HasColumnName("Activo")
                    .IsRequired();
                entity.Property(u => u.FechaCreacion)
                    .HasColumnName("FechaCreacion")
                    .HasColumnType("datetime")
                    .IsRequired();
                entity.Property(u => u.UsuarioRegistro)
                    .HasColumnName("UsuarioRegistró");
            });
        }
        //==============================================
    }
}
