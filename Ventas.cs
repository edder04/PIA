using PIA.Diseños;
using System;
using System.Drawing;
using System.Windows.Forms;


namespace PIA
{


    public partial class Ventas : Form
    {



        public Ventas()
        {
            InitializeComponent();
            pnlMenu.Visible = false;
            GlassEffect.Apply(this);
            RedondearBoton.Aplicar(btnInicio, 20);
            RedondearBoton.Aplicar(btnVentasVentas, 20);
            RedondearBoton.Aplicar(btnClientesVentas, 20);
            RedondearBoton.Aplicar(btnPartidosVentas, 20);
            RedondearBoton.Aplicar(btnFacturasVentas, 20);
            RedondearBoton.Aplicar(btnReportesVentas, 20);
            RedondearBoton.Aplicar(btnAjustes, 20);
            RedondearBoton.Aplicar(btnHoyInPartidos, 20);
            RedondearBoton.Aplicar(btnProximosInPartidos, 20);
            RedondearBoton.Aplicar(btnTodosInPartidos, 20);
            RedondearBoton.Aplicar(btnEstaSemanaInPartidos, 20);
            RedondearBoton.Aplicar(btnSalir, 20);

            tabCtrlVentas.Show();

            BarraTitulo.Agregar(this, "NEXO TICKET  |  Venta de boletos");

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

        }

        private void Ventas_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(@"C:\Users\edcru\Downloads\UANL\Agosto-Diciembre 2026\checkMark.gif");
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            CargarPartidos();
        }


        private void btnInicio_Click(object sender, EventArgs e)
        {
            tabCtrlVentas.SelectedIndex = 0;
            pnlMenu.Visible = false;
        }
        private void btnVentasVentas_Click_1(object sender, EventArgs e)
        {
            tabCtrlVentas.SelectedIndex = 1;
            pnlMenu.Visible = false;
        }

        private void btnClientesVentas_Click(object sender, EventArgs e)
        {
            tabCtrlVentas.SelectedIndex = 2;
            pnlMenu.Visible = false;
        }

        private void btnPartidosVentas_Click(object sender, EventArgs e)
        {
            tabCtrlVentas.SelectedIndex = 3;
            pnlMenu.Visible = false;
        }

        private void btnFacturasVentas_Click(object sender, EventArgs e)
        {
            tabCtrlVentas.SelectedIndex = 4;
            pnlMenu.Visible = false;
        }

        private void btnReportesVentas_Click(object sender, EventArgs e)
        {
            tabCtrlVentas.SelectedIndex = 5;
            pnlMenu.Visible = false;
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            pnlMenu.Visible = false;
            InicioSesion ventanaActual = new InicioSesion();
            ventanaActual.Show();
            this.Close();
        }

        private void CargarPartidos()
        {
            flowPanelPartidos.Controls.Clear();

            UcCardPartido partido1 = new UcCardPartido();
            UcCardPartido partido2 = new UcCardPartido();
            UcCardPartido partido3 = new UcCardPartido();
            UcCardPartido partido4 = new UcCardPartido();

            flowPanelPartidos.Controls.Add(partido1);
            flowPanelPartidos.Controls.Add(partido2);
            flowPanelPartidos.Controls.Add(partido3);
            flowPanelPartidos.Controls.Add(partido4);
        }

        private void btnAjustes_Click(object sender, EventArgs e)
        {
            pnlMenu.Visible = !pnlMenu.Visible;

            if (pnlMenu.Visible)
            {
                pnlMenu.BringToFront();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void btnNuevoClienteInCliente_Click(object sender, EventArgs e)
        {
            tabClientesInVentas.SelectedIndex = 1;
        }

        private void btnEditarCliente_Click(object sender, EventArgs e)
        {
            tabClientesInVentas.SelectedIndex = 2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabCtrlVentas.SelectedIndex = 2;
            tabClientesInVentas.SelectedIndex = 1;
        }

        private void btnContinuarVentaPaso1_Click(object sender, EventArgs e)
        {
            tabCtrlVenta.SelectedIndex = 1;
        }

        private void btnAtrasVentaPaso2_Click(object sender, EventArgs e)
        {
            tabCtrlVenta.SelectedIndex = 0;
        }

        private void btnContinuarVentaPaso2_Click(object sender, EventArgs e)
        {
            tabCtrlVenta.SelectedIndex = 2;
        }

        private void btnAtrasVentaPaso3_Click(object sender, EventArgs e)
        {
            tabCtrlVenta.SelectedIndex = 1;
        }

        private void btnContinuarVentaPaso3_Click(object sender, EventArgs e)
        {
            tabCtrlVenta.SelectedIndex = 3;
        }

        private void btnAtrasVentaPaso4_Click(object sender, EventArgs e)
        {
            tabCtrlVenta.SelectedIndex = 2;
        }

        private void btnConfirmarVentaPaso4_Click(object sender, EventArgs e)
        {
            tabCtrlVenta.SelectedIndex = 4;
        }

        private void btnNuevaVentaVentaRealizada_Click(object sender, EventArgs e)
        {
            tabCtrlVenta.SelectedIndex = 0;
        }

        private void tabPgInicio_Click(object sender, EventArgs e)
        {

        }
    }
}

