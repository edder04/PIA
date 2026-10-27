
using PIA.Diseños; 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing; 
using System.Text; 
using System.Windows.Forms; 

namespace PIA{                                                                                          // Indico que este código pertenece al proyecto PIA.
	public partial class Administracion : Form
	{                                                         // Creo la ventana de administración que hereda las funciones de Form.
		public Administracion()
		{                                                                        // Este es el constructor que se ejecuta cuando abro la ventana.

			InitializeComponent();                                                                      // Cargo todos los controles y elementos de la ventana.
			pnlMenuInAdmin.Visible = false;                                                             // Oculto el menú de administración al iniciar.
			GlassEffect.Apply(this);                                                                    // Aplico el efecto visual de vidrio a la ventana.

			RedondearBoton.Aplicar(btnAjustesInAdmin, 20);                                              // Redondeo las esquinas del botón de ajustes.
			RedondearBoton.Aplicar(btnHoyInPartidos, 20);                                               // Redondeo las esquinas del botón de partidos de hoy.
			RedondearBoton.Aplicar(btnProximosInPartidos, 20);                                          // Redondeo las esquinas del botón de próximos partidos.
			RedondearBoton.Aplicar(btnTodosInPartidos, 20);                                             // Redondeo las esquinas del botón para ver todos los partidos.
			RedondearBoton.Aplicar(btnEstaSemanaInPartidos, 20);                                        // Redondeo las esquinas del botón de partidos de esta semana.

			tabCtrlInAdmin.Show();                                                                             // Muestro el control de pestañas de la ventana.

			BarraTitulo.Agregar(this, "NEXO TICKET  |  Ventana de Administrador");                      // Agrego la barra de título personalizada con el nombre de la ventana.

			this.FormBorderStyle = FormBorderStyle.None;                                                // Quito los bordes predeterminados de la ventana.
			this.StartPosition = FormStartPosition.CenterScreen;                                        // Hago que la ventana aparezca en el centro de la pantalla.
		}

		private void Administracion_Load(object sender, EventArgs e)
		{                                   // Este método se ejecuta cuando se carga la ventana de administración.
			pictureBox1.Image = Image
				.FromFile(@"C:\Users\edcru\Downloads\UANL\Agosto-Diciembre 2026\checkMark.gif");        // Cargo el GIF desde la ruta donde tengo guardado el archivo.
			pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;                                     // Ajusto la imagen para que ocupe el espacio del PictureBox.
		}

		private void btnInicio_Click(object sender, EventArgs e)
		{                                       // Este método se ejecuta cuando presiono el botón Inicio.
			tabCtrlInAdmin.SelectedIndex = 0;                                                                  // Cambio a la primera pestaña del menú principal.
			pnlMenuInAdmin.Visible = false;                                                             // Oculto el menú después de seleccionar Inicio.
		}

		private void btnVentasVentas_Click(object sender, EventArgs e)
		{                                 // Este método se ejecuta cuando presiono el botón Ventas.
			tabCtrlInAdmin.SelectedIndex = 1;                                                                  // Cambio a la segunda pestaña, donde se encuentra la sección de ventas.
			pnlMenuInAdmin.Visible = false;                                                             // Oculto el menú después de seleccionar Ventas.
		}

		private void btnClientesVentas_Click(object sender, EventArgs e)
		{                               // Este método se ejecuta cuando presiono el botón Clientes.
			tabCtrlInAdmin.SelectedIndex = 2;                                                                  // Cambio a la tercera pestaña, donde se encuentra la sección de clientes.
			pnlMenuInAdmin.Visible = false;                                                             // Oculto el menú después de seleccionar Clientes.
		}

		private void btnPartidosVentas_Click(object sender, EventArgs e)
		{                               // Este método se ejecuta cuando presiono el botón Partidos.
			tabCtrlInAdmin.SelectedIndex = 3;                                                                  // Cambio a la cuarta pestaña, donde se encuentra la sección de partidos.
			pnlMenuInAdmin.Visible = false;                                                             // Oculto el menú después de seleccionar Partidos.
		}

		private void btnFacturasVentas_Click(object sender, EventArgs e)
		{                               // Este método se ejecuta cuando presiono el botón Facturas.
			tabCtrlInAdmin.SelectedIndex = 4;                                                                  // Cambio a la quinta pestaña, donde se encuentra la sección de facturas.
			pnlMenuInAdmin.Visible = false;                                                             // Oculto el menú después de seleccionar Facturas.
		}

		private void btnReportesVentas_Click(object sender, EventArgs e)
		{                               // Este método se ejecuta cuando presiono el botón Reportes.
			tabCtrlInAdmin.SelectedIndex = 5;                                                                  // Cambio a la sexta pestaña, donde se encuentra la sección de reportes.
			pnlMenuInAdmin.Visible = false;                                                             // Oculto el menú después de seleccionar Reportes.
		}

		private void btnUsuarios_Click(object sender, EventArgs e)
		{                                     // Este método se ejecuta cuando presiono el botón Usuarios.
			tabCtrlInAdmin.SelectedIndex = 6;                                                                  // Cambio a la séptima pestaña, donde se encuentra la sección de usuarios.
			pnlMenuInAdmin.Visible = false;                                                             // Oculto el menú después de seleccionar Usuarios.
			tabCtrlUsauriosInAdmin.SelectedIndex = 0;                                                              // Cambio a la primera pestaña de la sección de usuarios.
		}

		private void btnSalir_Click(object sender, EventArgs e)
		{                                        // Este método se ejecuta cuando presiono el botón Salir.
			pnlMenuInAdmin.Visible = false;                                                             // Oculto el menú antes de salir de la ventana.
			InicioSesion ventanaActual = new InicioSesion();                                            // Creo una nueva ventana de inicio de sesión.
			ventanaActual.Show();                                                                       // Muestro la ventana de inicio de sesión.
			this.Close();                                                                               // Cierro la ventana actual de administración.
		}

		private void btnAjustesInAdmin_Click(object sender, EventArgs e)
		{                               // Este método se ejecuta cuando presiono el botón de ajustes.
			pnlMenuInAdmin.Visible = !pnlMenuInAdmin.Visible;                                           // Muestro el menú si está oculto o lo oculto si está visible.

			if (pnlMenuInAdmin.Visible)
			{                                                                // Compruebo si el menú está visible.
				pnlMenuInAdmin.BringToFront();                                                          // Coloco el menú al frente para que no quede detrás de otros controles.
			}
		}

		private void btnNuevoClienteInCliente_Click(object sender, EventArgs e)
		{                        // Este método se ejecuta cuando presiono el botón para registrar un nuevo cliente.
			tabClientesInAdmin.SelectedIndex = 1;                                                       // Cambio a la segunda pestaña de la sección de clientes.
		}

		private void btnEditarCliente_Click(object sender, EventArgs e)
		{                                // Este método se ejecuta cuando presiono el botón para editar un cliente.
			tabClientesInAdmin.SelectedIndex = 2;                                                       // Cambio a la tercera pestaña de la sección de clientes.
		}

		private void button2_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 2;                                                           // Cambio a la cuarta pestaña, donde se encuentra la sección de clientes.
			tabClientesInAdmin.SelectedIndex = 1;                                                        // Cambio a la primera pestaña de la sección de clientes.
		}

		private void btnContinuarVentaPaso1_Click(object sender, EventArgs e)
		{
			tabCtrlVenta.SelectedIndex = 1;                                                                          // Cambio a la segunda pestaña de la sección de ventas.	
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

		private void btnFacturarVentaRealizada_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 4;                                                                  // Cambio a la quinta pestaña, donde se encuentra la sección de facturas.	
		}

		private void button11_Click(object sender, EventArgs e)
		{
			tabCtrlUsauriosInAdmin.SelectedIndex = 1;                                                              // Cambio a la segunda pestaña de la sección de usuarios.
		}

		private void button9_Click(object sender, EventArgs e)
		{
			tabCtrlUsauriosInAdmin.SelectedIndex = 2;                                                              // Cambio a la tercera pestaña de la sección de usuarios.
		}

		private void button13_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Usuario agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);     // Muestro un mensaje indicando que el usuario se agregó correctamente.
			tabCtrlUsauriosInAdmin.SelectedIndex = 0;                                                              // Cambio a la primera pestaña de la sección de usuarios.
		}

		private void button14_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Usuario editado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);     // Muestro un mensaje indicando que el usuario se editó correctamente.
			tabCtrlUsauriosInAdmin.SelectedIndex = 0;                                                              // Cambio a la primera pestaña de la sección de usuarios.
		}

		private void button5_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Usuario agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);     // Muestro un mensaje indicando que el usuario se agregó correctamente.
			tabClientesInAdmin.SelectedIndex = 0;
		}

		private void button7_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Usuario editado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);     // Muestro un mensaje indicando que el usuario se editó correctamente.
			tabClientesInAdmin.SelectedIndex = 0;
		}
	}
}
