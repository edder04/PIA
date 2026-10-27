
using PIA.Diseños;
using System;
using System.Drawing;
using System.Windows.Forms;


namespace PIA{                                                                                          // Indico que este código pertenece al proyecto PIA.

	public partial class Ventas : Form {                                                                // Creo la ventana de ventas que hereda las funciones de Form.
		public Ventas() {                                                                               // Este es el constructor que se ejecuta cuando abro la ventana.
			InitializeComponent();                                                                      // Cargo todos los controles y elementos de la ventana.
			pnlMenu.Visible = false;                                                                    // Oculto el menú al iniciar la ventana.
			GlassEffect.Apply(this);                                                                    // Aplico el efecto visual de vidrio a la venta 
			RedondearBoton.Aplicar(btnAjustes, 20);                                                     //				Redondeo las esquinas del botón de ajustes.
			RedondearBoton.Aplicar(btnHoyInPartidos, 20);                                               // Redondeo las esquinas del botón de partidos de hoy.
			RedondearBoton.Aplicar(btnProximosInPartidos, 20);                                          // Redondeo las esquinas del botón de próximos partidos.
			RedondearBoton.Aplicar(btnTodosInPartidos, 20);                                             // Redondeo las esquinas del botón para ver todos los partidos.
			RedondearBoton.Aplicar(btnEstaSemanaInPartidos, 20);                                        // Redondeo las esquinas del botón de partidos de esta semana.        

			tabCtrlVentas.Show();                                                                       // Muestro el control de pestañas de ventas.

			BarraTitulo.Agregar(this, "NEXO TICKET  |  Venta de boletos");                              // Agrego la barra de título personalizada con el nombre de la ventana.

			this.FormBorderStyle = FormBorderStyle.None;                                                // Quito los bordes predeterminados de la ventana.
			this.StartPosition = FormStartPosition.CenterScreen;                                        // Hago que la ventana aparezca en el centro de la pantalla.
		}

		private void Ventas_Load(object sender, EventArgs e){                                           // Este método se ejecuta cuando se carga la ventana de ventas.
			pictureBox1.Image = Image
				.FromFile(@"C:\Users\edcru\Downloads\UANL\Agosto-Diciembre 2026\checkMark.gif");        // Cargo el GIF desde la ruta donde tengo guardado el archivo.
			pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;                                     // Ajusto la imagen para que ocupe el espacio del PictureBox.
			CargarPartidos();                                                                           // Llamo al método que carga las tarjetas de los partidos.
		}
		private void btnInicio_Click(object sender, EventArgs e){										// Este método se ejecuta cuando presiono el botón Inicio.
			tabCtrlVentas.SelectedIndex = 0;															// Cambio a la primera pestaña del menú principal.
			pnlMenu.Visible = false;																	// Oculto el menú después de seleccionar Inicio.
		}

		private void btnVentasVentas_Click_1(object sender, EventArgs e){								// Este método se ejecuta cuando presiono el botón Ventas.
			tabCtrlVentas.SelectedIndex = 1;															// Cambio a la segunda pestaña, donde se encuentra la sección de ventas.
			pnlMenu.Visible = false;																	// Oculto el menú después de seleccionar Ventas.
		}

		private void btnClientesVentas_Click(object sender, EventArgs e){								// Este método se ejecuta cuando presiono el botón Clientes.
			tabCtrlVentas.SelectedIndex = 2;															// Cambio a la tercera pestaña, donde se encuentra la sección de clientes.
			pnlMenu.Visible = false;																	// Oculto el menú después de seleccionar Clientes.
		}

		private void btnPartidosVentas_Click(object sender, EventArgs e){								// Este método se ejecuta cuando presiono el botón Partidos.
			tabCtrlVentas.SelectedIndex = 3;															// Cambio a la cuarta pestaña, donde se encuentra la sección de partidos.
			pnlMenu.Visible = false;																	// Oculto el menú después de seleccionar Partidos.
		}

		private void btnFacturasVentas_Click(object sender, EventArgs e){								// Este método se ejecuta cuando presiono el botón Facturas.
			tabCtrlVentas.SelectedIndex = 4;															// Cambio a la quinta pestaña, donde se encuentra la sección de facturas.
			pnlMenu.Visible = false;																	// Oculto el menú después de seleccionar Facturas.
		}

		private void btnReportesVentas_Click(object sender, EventArgs e){								// Este método se ejecuta cuando presiono el botón Reportes.
			tabCtrlVentas.SelectedIndex = 5;															// Cambio a la sexta pestaña, donde se encuentra la sección de reportes.
			pnlMenu.Visible = false;																	// Oculto el menú después de seleccionar Reportes.
		}

		private void btnSalir_Click(object sender, EventArgs e){										// Este método se ejecuta cuando presiono el botón Salir.
			pnlMenu.Visible = false;																	// Oculto el menú antes de salir de la ventana.
			InicioSesion ventanaActual = new InicioSesion();											// Creo una nueva ventana de inicio de sesión.
			ventanaActual.Show();																		// Muestro nuevamente la ventana de inicio de sesión.
			this.Close();																				// Cierro la ventana actual de ventas.
		}

		private void CargarPartidos(){																	// Creo este método para cargar las tarjetas de los partidos.
			flowPanelPartidos.Controls.Clear();															// Limpio el panel para quitar las tarjetas que ya tenía.

			UcCardPartido partido1 = new UcCardPartido();												// Creo la primera tarjeta de partido.
			UcCardPartido partido2 = new UcCardPartido();												// Creo la segunda tarjeta de partido.
			UcCardPartido partido3 = new UcCardPartido();												// Creo la tercera tarjeta de partido.
			UcCardPartido partido4 = new UcCardPartido();												// Creo la cuarta tarjeta de partido.

			flowPanelPartidos.Controls.Add(partido1);													// Agrego la primera tarjeta al panel.
			flowPanelPartidos.Controls.Add(partido2);													// Agrego la segunda tarjeta al panel.
			flowPanelPartidos.Controls.Add(partido3);													// Agrego la tercera tarjeta al panel.
			flowPanelPartidos.Controls.Add(partido4);													// Agrego la cuarta tarjeta al panel.
		}

		private void btnAjustes_Click(object sender, EventArgs e){										// Este método se ejecuta cuando presiono el botón de ajustes.
			pnlMenu.Visible = !pnlMenu.Visible;															// Muestro el menú si está oculto o lo oculto si está visible.

			if (pnlMenu.Visible){																		// Compruebo si el menú está visible.
				pnlMenu.BringToFront();																	// Coloco el menú al frente para que no quede detrás de otros controles.
			}
		}

		private void btnNuevoClienteInCliente_Click(object sender, EventArgs e){						// Este método se ejecuta cuando presiono el botón para registrar un nuevo cliente.
			tabClientesInVentas.SelectedIndex = 1;														// Cambio a la segunda pestaña de la sección de clientes.
		}

		private void btnEditarCliente_Click(object sender, EventArgs e){								// Este método se ejecuta cuando presiono el botón para editar un cliente.
			tabClientesInVentas.SelectedIndex = 2;														// Cambio a la tercera pestaña de la sección de clientes.
		}

		private void button2_Click(object sender, EventArgs e){											// Este método se ejecuta cuando presiono este botón.
			tabCtrlVentas.SelectedIndex = 2;															// Abro la tercera pestaña del menú principal, que corresponde a Clientes.
			tabClientesInVentas.SelectedIndex = 1;														// Abro la segunda pestaña interna de clientes.
		}

		private void btnContinuarVentaPaso1_Click(object sender, EventArgs e){							// Este método se ejecuta cuando presiono Continuar en el primer paso de la venta.
			tabCtrlVenta.SelectedIndex = 1;																// Avanzo al segundo paso del proceso de venta.
		}

		private void btnAtrasVentaPaso2_Click(object sender, EventArgs e){								// Este método se ejecuta cuando presiono Atrás en el segundo paso.
			tabCtrlVenta.SelectedIndex = 0;																// Regreso al primer paso del proceso de venta.
		}

		private void btnContinuarVentaPaso2_Click(object sender, EventArgs e){							// Este método se ejecuta cuando presiono Continuar en el segundo paso.
			tabCtrlVenta.SelectedIndex = 2;																// Avanzo al tercer paso del proceso de venta.
		}

		private void btnAtrasVentaPaso3_Click(object sender, EventArgs e){								// Este método se ejecuta cuando presiono Atrás en el tercer paso.
			tabCtrlVenta.SelectedIndex = 1;																// Regreso al segundo paso del proceso de venta.
		}

		private void btnContinuarVentaPaso3_Click(object sender, EventArgs e){							// Este método se ejecuta cuando presiono Continuar en el tercer paso.
			tabCtrlVenta.SelectedIndex = 3;																// Avanzo al cuarto paso del proceso de venta.
		}

		private void btnAtrasVentaPaso4_Click(object sender, EventArgs e){								// Este método se ejecuta cuando presiono Atrás en el cuarto paso.
			tabCtrlVenta.SelectedIndex = 2;																// Regreso al tercer paso del proceso de venta.
		}

		private void btnConfirmarVentaPaso4_Click(object sender, EventArgs e){							// Este método se ejecuta cuando presiono Confirmar en el cuarto paso.
			tabCtrlVenta.SelectedIndex = 4;																// Avanzo a la quinta pestaña, donde continúa el proceso después de confirmar.
		}

		private void btnNuevaVentaVentaRealizada_Click(object sender, EventArgs e){						// Este método se ejecuta cuando presiono el botón para realizar una nueva venta.
			tabCtrlVenta.SelectedIndex = 0;																// Regreso al primer paso para comenzar otra venta.
		}
	}
}
