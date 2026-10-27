using PIA.Diseños;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PIA
{
    public partial class Administracion : Form
    {
        public Administracion()
        {
            InitializeComponent();
            pnlMenuInAdmin.Visible = false;
            //GlassEffect.Apply(pnlMenuInAdmin);
            GlassEffect.Apply(this);


            RedondearBoton.Aplicar(btnAjustesInAdmin, 20);
            RedondearBoton.Aplicar(btnHoyInPartidos, 20);
            RedondearBoton.Aplicar(btnProximosInPartidos, 20);
            RedondearBoton.Aplicar(btnTodosInPartidos, 20);
            RedondearBoton.Aplicar(btnEstaSemanaInPartidos, 20);


            tabCtrl.Show();

            BarraTitulo.Agregar(this, "NEXO TICKET  |  Ventana de Administrador");

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        private void Administracion_Load(object sender, EventArgs e)
        {

            pictureBox1.Image = Image.FromFile(@"C:\Users\edcru\Downloads\UANL\Agosto-Diciembre 2026\checkMark.gif");
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }


        private void btnInicio_Click(object sender, EventArgs e)
        {
            tabCtrl.SelectedIndex = 0;
            pnlMenuInAdmin.Visible = false;
        }

        private void btnVentasVentas_Click(object sender, EventArgs e)
        {
            tabCtrl.SelectedIndex = 1;
            pnlMenuInAdmin.Visible = false;
        }

        private void btnClientesVentas_Click(object sender, EventArgs e)
        {
            tabCtrl.SelectedIndex = 2;
            pnlMenuInAdmin.Visible = false;
        }

        private void btnPartidosVentas_Click(object sender, EventArgs e)
        {
            tabCtrl.SelectedIndex = 3;
            pnlMenuInAdmin.Visible = false;
        }

        private void btnFacturasVentas_Click(object sender, EventArgs e)
        {
            tabCtrl.SelectedIndex = 4;
            pnlMenuInAdmin.Visible = false;
        }

        private void btnReportesVentas_Click(object sender, EventArgs e)
        {
            tabCtrl.SelectedIndex = 5;
            pnlMenuInAdmin.Visible = false;
        }
        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            tabCtrl.SelectedIndex = 6;
            pnlMenuInAdmin.Visible = false;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            pnlMenuInAdmin.Visible = false;
            InicioSesion ventanaActual = new InicioSesion();
            ventanaActual.Show();
            this.Close();
        }

        private void btnAjustesInAdmin_Click(object sender, EventArgs e)
        {
            pnlMenuInAdmin.Visible = !pnlMenuInAdmin.Visible;

            if (pnlMenuInAdmin.Visible)
            {
                pnlMenuInAdmin.BringToFront();
            }

        }

        private void btnNuevoClienteInCliente_Click(object sender, EventArgs e)
        {
            tabClientesInAdmin.SelectedIndex = 1;
        }

        private void btnEditarCliente_Click(object sender, EventArgs e)
        {
            tabClientesInAdmin.SelectedIndex = 2;
        }

        private void label65_Click(object sender, EventArgs e)
        {

        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }
    }
}
