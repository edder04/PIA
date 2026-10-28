using PIA.Datos;                                                                                        
using Microsoft.EntityFrameworkCore;                                                                  
using PIA.Diseños;                                                                                     
namespace PIA{                                                                                          // Indico que este código pertenece al proyecto PIA.

    public partial class InicioSesion : Form{                                                           // Creo la ventana de inicio de sesión.    
        public InicioSesion(){                                                                          // Este es el constructor que se ejecuta al abrir la ventana.        
            InitializeComponent();                                                                      // Cargo los controles y elementos de la ventana.
            BarraTitulo.Agregar(this, "NEXO TICKET  |  Inicio de sesión");                              // Agrego la barra de título personalizada.
            GlassEffect.Apply(this);                                                                    // Aplico el efecto visual de vidrio a la ventana.
            txtUsuario.Focus();                                                                         // Coloco el cursor en el campo del usuario para empezar a escribir.
        }

        
        private void btnAceptar_Click(object sender, EventArgs e){                                      // Este método se ejecuta cuando hago clic en el botón Aceptar.        
            string nombreUsuario = txtUsuario.Text;                                                     // Guardo el usuario que escribí en el campo de texto.
            string contraseña = txtContraseña.Text;                                                     // Guardo la contraseña que escribí.
            Form ventanaDestino;                                                                        // Creo una variable para guardar la ventana que voy a abrir según el rol.
                                                                                                                    
            /*                                                                                
            if (nombreUsuario == "Admin01" && contraseña == "admin")                                    // Este código era para validar el acceso usando usuarios y contraseñas escritos directamente.
            {
                ventanaDestino = new Administracion();                                                  // Abro la ventana de administración.
                this.Hide();                                                                            // Oculto la ventana de inicio de sesión.
                ventanaDestino.Show();                                                                  // Muestro la ventana de administración.
                UsuarioActivo = txtUsuario.Text;                                                        // Guardo el nombre del usuario que inició sesión.
            }

            if (nombreUsuario == "Operativo01" && contraseña == "op01")
            {
                ventanaDestino = new Ventas();                                                          // Abro la ventana de ventas.
                this.Hide();                                                                            // Oculto la ventana de inicio de sesión.
                ventanaDestino.Show();                                                                  // Muestro la ventana de ventas.
                UsuarioActivo = txtUsuario.Text;                                                        // Guardo el nombre del usuario que inició sesión.
            }
            else
            {
                    MessageBox.Show("Ingresa tu usuario y contraseña.", "Inicio de sesion",             // Muestro una advertencia si los datos no coinciden.
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            */

            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contraseña)){     // Compruebo que ninguno de los dos campos esté vacío o contenga solamente espacios.                            
                    MessageBox.Show("Ingresa tu usuario y contraseña.", "Inicio de sesion",             // Muestro un mensaje para pedir que se ingresen los datos.
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;                                                                                 // Detengo el proceso para que no continúe si faltan datos.
            }

            try{                                                                                        // Intento validar el acceso y controlo los posibles errores.            
                using var db = new AppDbContext();                                                      // Me conecto a la base de datos usando mi contexto.
                
                var usuario = db.Usuario                                                                // Busco un usuario que coincida con el nombre ingresado y que esté activo.
                    .AsNoTracking()                                                                     // Hago la consulta sin que Entity Framework tenga que seguir los cambios del usuario.
                    .SingleOrDefault(u => u.NombreUsuario == nombreUsuario && u.Activo);                // Obtengo el usuario si existe y está activo.
                
                if (usuario == null || usuario.PasswordHash != contraseña){                             // Compruebo si el usuario no existe o si la contraseña no coincide.                
                    MessageBox.Show("Usuario o contraseña incorrectos.");                               // Muestro un mensaje si los datos no son correctos.
                    return;                                                                             // Detengo el proceso porque no se puede permitir el acceso.
                }
                                                           
                switch (usuario.Rol){                                                                   // Reviso el rol del usuario para decidir qué ventana debe abrirse.                
                    case "Administrador": ventanaDestino = new Administracion(); break;                 // Si es administrador, abro la ventana de administración.
                    case "Operativo": ventanaDestino = new Ventas(); break;                             // Si es operativo, abro la ventana de ventas.
                    default:
                        MessageBox.Show("El usuario tiene un rol no válido.", "Acceso denegado",        // Si el rol no coincide con ninguno de los permitidos, muestro un error.
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;                                                                         // Detengo el proceso porque el rol no es válido.
                }

				Sesion.IdUsuario = usuario.IdUsuario;
				Sesion.NombreUsuario = usuario.NombreUsuario;
				Sesion.Rol = usuario.Rol;

				this.Hide();                                                                            // Oculto la ventana de inicio de sesión cuando el acceso es correcto.
                ventanaDestino.Show();                                                                  // Abro la ventana que corresponde al rol del usuario.
            }
            catch (Exception ex){                                                                       // Capturo cualquier error que ocurra durante la validación.                                                                                                                  
                MessageBox.Show("No se pudo validar el acceso.\n" + ex.Message, "Error",                // Muestro un mensaje de error junto con la información del problema.
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
                                                                                
        private void InicioSesion_FormClosing(object sender, FormClosingEventArgs e){        
            Application.Exit();                                                                         // Cierro toda la aplicación, no solamente esta ventana.
        }
                                                                      
        private void txtUsuario_KeyDown(object sender, KeyEventArgs e){                                 // Este método detecta cuando presiono una tecla en el campo de usuario.                                                                                                               
            if (e.KeyCode == Keys.Enter){                                                               // Compruebo si la tecla que presioné fue Enter.
                e.SuppressKeyPress = true;                                                              // Evito que Enter genere una acción adicional en el campo.
                txtContraseña.Focus();                                                                  // Paso el cursor al campo de la contraseña.
            }
        }
                                                                              
        private void txtContraseña_KeyDown(object sender, KeyEventArgs e){                              // Este método detecta cuando presiono una tecla en el campo de contraseña.                                                                                            
            if (e.KeyCode == Keys.Enter){                                                               // Compruebo si presioné Enter para intentar iniciar sesión.            
                e.SuppressKeyPress = true;                                                              // Evito que Enter genere una acción adicional en el campo.
                btnAceptar.PerformClick();                                                              // Ejecuto el mismo evento que si hiciera clic en Aceptar.
            }
        }
    }
}
