using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PIA.Diseños
{
    public static class BarraTitulo
    {
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private static readonly IntPtr HTCAPTION = new IntPtr(2);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam);

        public static void Agregar(Form form, string titulo)
        {
            if (form == null || form.IsDisposed)
                return;

            // Evitar crear barras duplicadas
            if (form.Controls["pnlBarraTitulo"] != null)
                return;

            form.FormBorderStyle = FormBorderStyle.None;

            Panel barra = new Panel
            {
                Name = "pnlBarraTitulo",
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.FromArgb(25, 25, 25)
            };

            Label lblTitulo = new Label
            {
                Text = titulo,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(7, 0),
                Height = 30,
                BackColor = Color.Transparent,
                AutoEllipsis = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left |
                         AnchorStyles.Right
            };

            Button CrearBoton(string texto)
            {
                Button boton = new Button
                {
                    Text = texto,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(25, 25, 25),
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(46, 30),
                    Font = new Font("Segoe UI Symbol", 11),
                    Cursor = Cursors.Hand,
                    TabStop = false,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                boton.FlatAppearance.BorderSize = 0;
                boton.FlatAppearance.MouseOverBackColor =
                    Color.FromArgb(55, 55, 55);

                return boton;
            }

            Button btnMinimizar = CrearBoton("➖");
           
            Button btnCerrar = CrearBoton("✖️");

            btnCerrar.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(196, 43, 28);

            btnMinimizar.Click += (s, e) =>
                form.WindowState = FormWindowState.Minimized;

            btnCerrar.Click += (s, e) => form.Close();

            // Posicionar los botones manualmente
            void AjustarBarra(object sender, EventArgs e)
            {
                btnCerrar.Location = new Point(
                    barra.ClientSize.Width - btnCerrar.Width, 0);

                btnMinimizar.Location = new Point(
                    btnCerrar.Left - btnMinimizar.Width, 0);

                lblTitulo.Width = Math.Max(
                    0, btnMinimizar.Left - lblTitulo.Left - 8);
            }

            barra.Controls.Add(lblTitulo);
            barra.Controls.Add(btnMinimizar);
            //barra.Controls.Add(btnMaximizar);
            barra.Controls.Add(btnCerrar);

            barra.Resize += AjustarBarra;
            AjustarBarra(null, EventArgs.Empty);

            void IniciarArrastre(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();

                    SendMessage(
                        form.Handle,
                        WM_NCLBUTTONDOWN,
                        HTCAPTION,
                        IntPtr.Zero);
                }
            }

            barra.MouseDown += IniciarArrastre;
            lblTitulo.MouseDown += IniciarArrastre;

            form.Controls.Add(barra);
            barra.BringToFront();
        }
    }
}
