
using System; 
using System.Collections.Generic;
using System.Text;

namespace PIA.Datos{                                                                                    // Indico que este código pertenece al proyecto PIA.

    public class Usuario{                                                                               // Creo la clase Usuario para representar la información de cada usuario en mi base de datos.
        public int IdUsuario { get; set; }                                                              // Guardo el identificador único de cada usuario.
        public string Nombres { get; set; } = string.Empty;                                             // Guardo los nombres del usuario y comienzo con un texto vacío.
        public string ApPaterno { get; set; } = string.Empty;                                           // Guardo el apellido paterno del usuario y comienzo con un texto vacío.
        public string? ApMaterno { get; set; }                                                          // Guardo el apellido materno, pero puede quedar vacío o ser nulo.
        public DateTime FechaNacimiento { get; set; }                                                   // Guardo la fecha de nacimiento del usuario.
        public string NumeroNomina { get; set; } = string.Empty;                                        // Guardo el número de nómina del usuario.
        public string CorreoElectronico { get; set; } = string.Empty;                                   // Guardo el correo electrónico del usuario.
        public string? Rol { get; set; }                                                                // Guardo el rol del usuario, por ejemplo, Administrador u Operativo.
        public string NombreUsuario { get; set; } = string.Empty;                                       // Guardo el nombre que utiliza para iniciar sesión.
        public string PasswordHash { get; set; } = string.Empty;                                        // Guardo el valor de la contraseña o su hash, según cómo se maneje en el sistema.
        public bool Activo { get; set; }                                                                // Indico si la cuenta del usuario está activa o desactivada.
        public DateTime FechaCreacion { get; set; }                                                     // Guardo la fecha y hora en que se creó el registro del usuario.
        public int? UsuarioRegistro { get; set; }                                                       // Guardo el identificador del usuario que registró a esta persona, si existe.
    }
}
