using System;
using System.Collections.Generic;
using System.Text;

namespace PIA.Datos
{
    public class Usuario
    {
        public int UsuarioId { get; set; }

        public string NombreUsuario { get; set; } = "";

        public string NombreCompleto { get; set; } = "";

        public string PasswordHash { get; set; } = "";

        public int Rol { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }

    }
