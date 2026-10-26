using PIA.Datos;
using Microsoft.EntityFrameworkCore;
using PIA.Diseños;

namespace PIA
{
    public partial class InicioSesion : Form
    {
        public InicioSesion()
        {
            InitializeComponent();
            BarraTitulo.Agregar(this, "NEXO TICKET  |  Inicio de sesión");
            this.AcceptButton = btnAceptar;
            GlassEffect.Apply(this);

        }
        public string UsuarioActivo { get; private set; } = "";

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txtUsuario.Text;
            string contraseña = txtContraseña.Text;


        Form ventanaDestino;

            if (nombreUsuario == "Admin01" && contraseña == "admin")
            {
                ventanaDestino = new Administracion();
                this.Hide();
                ventanaDestino.Show();
                UsuarioActivo = txtUsuario.Text;
                

            }
            if(nombreUsuario =="Operativo01" && contraseña == "op01")
            {
                ventanaDestino = new Ventas();
                this.Hide();
                ventanaDestino.Show();
                UsuarioActivo = txtUsuario.Text;
            }
            else
            {
                MessageBox.Show("Ingresa tu usuario y contraseña.", "Inicio de sesion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            /*if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contraseña))
            {
                MessageBox.Show("Ingresa tu usuario y contraseña.", "Inicio de sesion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var db = new AppDbContext();

                var usuario = db.Usuario
                    .AsNoTracking()
                    .SingleOrDefault(u => u.NombreUsuario == nombreUsuario && u.Activo);

                if (usuario == null || usuario.PasswordHash != contraseña)
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.");
                    return;
                }

                Form ventanaDestino;

                switch (usuario.Rol)
                {
                    case 1: ventanaDestino = new Administracion(); break;
                    case 2: ventanaDestino = new Ventas(); break;
                    default:
                        MessageBox.Show("El usuario tiene un rol no válido.", "Acceso denegado",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                this.Hide();
                ventanaDestino.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo validar el acceso.\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void InicioSesion_Load(object sender, EventArgs e) { }

        private void InicioSesion_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}