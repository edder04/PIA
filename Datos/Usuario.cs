using System;
using System.Collections.Generic;
using System.Text;

namespace PIA.Datos
{
    // =============================================
    // Creacion de la clase Usuario que representa la tabla Usuario
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombres { get; set; } = string.Empty; 
        public string ApPaterno { get; set; } = string.Empty;
        public string? ApMaterno { get; set; }          
        public DateTime FechaNacimiento { get; set; }
        public string NumeroNomina { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string? Rol { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? UsuarioRegistro { get; set; }
    }
}