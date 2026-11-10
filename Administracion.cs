
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using PIA.Datos;
using PIA.Diseños; 
using PIA.Repositorios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing; 
using System.Text; 
using System.Text.RegularExpressions;
using System.Windows.Forms; 

namespace PIA{                                                                                          // Indico que este código pertenece al proyecto PIA.
	public partial class Administracion : Form
	{                                                         // Creo la ventana de administración que hereda las funciones de Form.
		private int? _idEstadioEnCaptura = null;
		private int _zonasAgregadas = 0;

		public Administracion()
		{                                                                        // Este es el constructor que se ejecuta cuando abro la ventana.

			InitializeComponent();                                                                      // Cargo todos los controles y elementos de la ventana.
			pnlMenuInAdmin.Visible = false;                                                             // Oculto el menú de administración al iniciar.
			GlassEffect.Apply(this);                                                                  // Aplico el efecto visual de vidrio a la ventana.

			RedondearBoton.Aplicar(btnAjustesInAdmin, 20);                                              // Redondeo las esquinas del botón de ajustes.
			RedondearBoton.Aplicar(btnHoyInPartidos, 20);                                               // Redondeo las esquinas del botón de partidos de hoy.
			RedondearBoton.Aplicar(btnProximosInPartidos, 20);                                          // Redondeo las esquinas del botón de próximos partidos.
			RedondearBoton.Aplicar(btnTodosInPartidos, 20);                                             // Redondeo las esquinas del botón para ver todos los partidos.
			RedondearBoton.Aplicar(btnEstaSemanaInPartidos, 20);                                        // Redondeo las esquinas del botón de partidos de esta semana.

			tabCtrlInAdmin.Show();                                                                             // Muestro el control de pestañas de la ventana.

			BarraTitulo.Agregar(this, "NEXO TICKET  |  Ventana de Administrador");                      // Agrego la barra de título personalizada con el nombre de la ventana.

			this.FormBorderStyle = FormBorderStyle.None;                                                // Quito los bordes predeterminados de la ventana.
			this.StartPosition = FormStartPosition.CenterScreen;                                        // Hago que la ventana aparezca en el centro de la pantalla.

			//==========================================
			// CONFIGURACIÓN DEL GRID DE CLIENTES
			//==========================================
			GridClientesInClientes.Columns.Add("IdCliente", "IdCliente");
			GridClientesInClientes.Columns.Add("Nombre", "Nombre");
			GridClientesInClientes.Columns.Add("ApPaterno", "Apellido Paterno");
			GridClientesInClientes.Columns.Add("ApMaterno", "Apellido Materno");
			GridClientesInClientes.Columns.Add("FechaNacimiento", "Fecha de nacimiento");
			GridClientesInClientes.Columns.Add("Edad", "Edad");
			GridClientesInClientes.Columns.Add("Domicilio", "Domicilio");
			GridClientesInClientes.Columns.Add("RFC", "RFC");
			GridClientesInClientes.Columns.Add("CorreoElectronico", "Correo");
			GridClientesInClientes.Columns.Add("TelefonoCasa", "Teléfono casa");
			GridClientesInClientes.Columns.Add("TelefonoCelular", "Teléfono celular");
			GridClientesInClientes.Columns.Add("Activo", "Activo");
			GridClientesInClientes.ReadOnly = true;
			GridClientesInClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			GridClientesInClientes.MultiSelect = false;
			GridClientesInClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			CargarGridClientes();
			GridClientesInClientes.SelectionChanged += GridClientesInClientes_SelectionChanged;

		}

		private async void Administracion_Load(object sender, EventArgs e)
		{                                   // Este método se ejecuta cuando se carga la ventana de administración.
			pictureBox1.Image = Image
				.FromFile(@"C:\Users\edcru\Downloads\UANL\Agosto-Diciembre 2026\checkMark.gif");        // Cargo el GIF desde la ruta donde tengo guardado el archivo.
			pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;                                     // Ajusto la imagen para que ocupe el espacio del PictureBox.

			try
			{
				var repo = new RepoEstadio();

				cboEstadioInAdmin.DataSource = await repo.ObtenerTodosAsync();
				cboEstadioInAdmin.DisplayMember = "Nombre";
				cboEstadioInAdmin.ValueMember = "IdEstadio";
				cboEstadioInAdmin.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"No se pudieron cargar los estadios.\n{ex.Message}");
			}



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
		//==========================================
		// BOTON GUARDAR CLIENTE
		//==========================================
		private async void btnAgregarClienteInAdmin_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtNombreClienteInAdmin.Text) ||
				string.IsNullOrWhiteSpace(txtApPaternoClienteInAdmin.Text) ||
				string.IsNullOrWhiteSpace(dateFechaNacimientoClienteinAdmin.Text) ||
				string.IsNullOrWhiteSpace(txtRFCClienteInAdmin.Text) ||
				string.IsNullOrWhiteSpace(txtCorreoClienteInAdmin.Text) ||
				string.IsNullOrWhiteSpace(txtCelularClienteInAdmin.Text))
			{
				MessageBox.Show("Complete los campos obligatorios.");
				return;
			}
			else
			{
				try
				{
					var cliente = new Cliente
					{
						Nombres = txtNombreClienteInAdmin.Text,
						ApPaterno = txtApPaternoClienteInAdmin.Text,
						ApMaterno = txtApMaternoClienteInAdmin.Text,
						FechaNacimiento = dateFechaNacimientoClienteinAdmin.Value,
						RFC = txtRFCClienteInAdmin.Text,
						CorreoElectronico = txtCorreoClienteInAdmin.Text,
						TelefonoCasa = txtTelefonoCasaInAdmin.Text,
						TelefonoCelular = txtCelularClienteInAdmin.Text,
						Domicilio = $@"{txtCalleClienteInAdmin.Text} {txtNoExteriorCleinteInAdmin.Text} {txtNoInteriorClienteInAdmin.Text}, {txtCPClienteInAdmin.Text}, {txtColoniaClienteInAdmin.Text}, {txtCiudadClienteInAdmin.Text}, {txtEstadoClienteInAdmin.Text}, {txtPaisClienteInAdmin.Text}",
						CreadoPor = Sesion.IdUsuario,
						Activo = true
					};

					var repositorio = new RepoCliente();
					await repositorio.AgregarAsync(cliente);

					MessageBox.Show("Cliente agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);     // Muestro un mensaje indicando que el cliente se agregó correctamente.
					tabClientesInAdmin.SelectedIndex = 0;                                                       // Cambio a la primera pestaña de la sección de clientes.
					CargarGridClientes();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "No se pudo agregar el cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				finally
				{
					LimpiarCamposNuevoCliente();
				}
			}
		}

		private void btnGuardarClienteInAdmin_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Cliente editado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.None);
			tabClientesInAdmin.SelectedIndex = 0;
		}

		private void GridClientesInClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

		private void txtBuscarCliente_TextChanged(object sender, EventArgs e)
		{
			CargarGridClientes();
		}

		//==========================================
		// METODO CARGAR GRID CLIENTES
		//==========================================
		private void CargarGridClientes()
		{
			string texto = txtBuscarCliente.Text;
			using var db = new AppDbContext();
			var clientes = db.vw_Clientes
				.Where(c => c.ApPaterno.Contains(texto) || c.CorreoElectronico.Contains(texto) || c.RFC.Contains(texto));

			GridClientesInClientes.Rows.Clear();

			foreach (vw_Clientes c in clientes)
			{
				GridClientesInClientes.Rows.Add(
				c.IdCliente,
				c.Nombres,
				c.ApPaterno,
				c.ApMaterno,
				c.FechaNacimiento.ToShortDateString(),
				c.Edad,
				c.Domicilio,
				c.RFC,
				c.CorreoElectronico,
				c.TelefonoCasa,
				c.TelefonoCelular,
				c.Activo);
			}
		}
		//==========================================
		// METODO LIMPIAR CAMPOS NUEVO CLIENTE
		//==========================================
		private void LimpiarCamposNuevoCliente()
		{
			txtNombreClienteInAdmin.Clear();
			txtApPaternoClienteInAdmin.Clear();
			txtApMaternoClienteInAdmin.Clear();
			txtRFCClienteInAdmin.Clear();
			txtCorreoClienteInAdmin.Clear();
			txtTelefonoCasaInAdmin.Clear();
			txtCelularClienteInAdmin.Clear();
			dateFechaNacimientoClienteinAdmin.Value = DateTime.Today;
			txtCalleClienteInAdmin.Clear();
			txtNoExteriorCleinteInAdmin.Clear();
			txtNoInteriorClienteInAdmin.Clear();
			txtCPClienteInAdmin.Clear();
			txtColoniaClienteInAdmin.Clear();
			txtCiudadClienteInAdmin.Clear();
			txtEstadoClienteInAdmin.Clear();
			txtPaisClienteInAdmin.Clear();
			txtNombreClienteInAdmin.Focus();
		}
		//==========================================
		// CONFIGURACION DEL COMBOBOX DE EDITAR CLIENTE
		//==========================================
		private void cboBoxEditarClienteInAdmin_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboBoxEditarClienteInAdmin.SelectedIndex != 3)
			{
				lblNuevoDatoEditarClienteInAdmin.Visible = true;
				txtNuevoDatoEditarClienteInAdmin.Visible = true;
				lblCalleEditarClienteInAdmin.Visible = false;
				txtCalleEditarClienteInAdmin.Visible = false;
				lblNoExteriorEditarClienteInAdmin.Visible = false;
				txtNoExteriorEditarClienteInAdmin.Visible = false;
				lblNoInteriorEditarClienteInAdmin.Visible = false;
				txtNoInteriorEditarClienteInAdmin.Visible = false;
				lblCPEditarClienteInAdmin.Visible = false;
				txtCPEditarClienteInAdmin.Visible = false;
				lblColoniaEditarClienteInAdmin.Visible = false;
				txtColoniaEditarClienteInAdmin.Visible = false;
				lblCiudadEditarClienteInAdmin.Visible = false;
				txtCiudadEditarClienteInAdmin.Visible = false;
				lblEstadoEditarClienteInAdmin.Visible = false;
				txtEstadoEditarClienteInAdmin.Visible = false;
				lblPaisEditarClienteInAdmin.Visible = false;
				txtPaisEditarClienteInAdmin.Visible = false;
			}
			else
			{
				lblNuevoDatoEditarClienteInAdmin.Visible = false;
				txtNuevoDatoEditarClienteInAdmin.Visible = false;
				lblCalleEditarClienteInAdmin.Visible = true;
				txtCalleEditarClienteInAdmin.Visible = true;
				lblNoExteriorEditarClienteInAdmin.Visible = true;
				txtNoExteriorEditarClienteInAdmin.Visible = true;
				lblNoInteriorEditarClienteInAdmin.Visible = true;
				txtNoInteriorEditarClienteInAdmin.Visible = true;
				lblCPEditarClienteInAdmin.Visible = true;
				txtCPEditarClienteInAdmin.Visible = true;
				lblColoniaEditarClienteInAdmin.Visible = true;
				txtColoniaEditarClienteInAdmin.Visible = true;
				lblCiudadEditarClienteInAdmin.Visible = true;
				txtCiudadEditarClienteInAdmin.Visible = true;
				lblEstadoEditarClienteInAdmin.Visible = true;
				txtEstadoEditarClienteInAdmin.Visible = true;
				lblPaisEditarClienteInAdmin.Visible = true;
				txtPaisEditarClienteInAdmin.Visible = true;
			}
		}
		//==========================================
		// BOTON GUARDAR CAMBIOS EDITAR CLIENTE
		//==========================================
		private async void button7_Click(object sender, EventArgs e)
		{
			var fila = GridClientesInClientes.CurrentRow;

			if (fila is null || fila.IsNewRow ||
				fila.Cells["IdCliente"].Value is null)
			{
				MessageBox.Show("Selecciona un cliente del grid.");
				return;
			}
			int idCliente = Convert.ToInt32(fila.Cells["IdCliente"].Value);

			if (cboBoxEditarClienteInAdmin.SelectedItem is null)
			{
				MessageBox.Show("Selecciona qué dato quieres cambiar.");
				return;
			}

			string campoSeleccionado = cboBoxEditarClienteInAdmin.SelectedItem.ToString()!;
			string nuevoDato;

			if (cboBoxEditarClienteInAdmin.SelectedIndex == 3)
			{
				if (string.IsNullOrWhiteSpace(txtCalleEditarClienteInAdmin.Text) ||
					string.IsNullOrWhiteSpace(txtNoExteriorEditarClienteInAdmin.Text) ||
					string.IsNullOrWhiteSpace(txtCPEditarClienteInAdmin.Text) ||
					string.IsNullOrWhiteSpace(txtColoniaEditarClienteInAdmin.Text) ||
					string.IsNullOrWhiteSpace(txtCiudadEditarClienteInAdmin.Text) ||
					string.IsNullOrWhiteSpace(txtEstadoEditarClienteInAdmin.Text) ||
					string.IsNullOrWhiteSpace(txtPaisEditarClienteInAdmin.Text))
				{
					MessageBox.Show("Completa los campos obligatorios del domicilio.");
					return;
				}

				string numeroInterior = string.IsNullOrWhiteSpace(
					txtNoInteriorEditarClienteInAdmin.Text)
						? ""
						: $" Int. {txtNoInteriorEditarClienteInAdmin.Text.Trim()}";

				nuevoDato =
					$"{txtCalleEditarClienteInAdmin.Text.Trim()} " +
					$"No. {txtNoExteriorEditarClienteInAdmin.Text.Trim()}" +
					$"{numeroInterior}, " +
					$"C.P. {txtCPEditarClienteInAdmin.Text.Trim()}, " +
					$"{txtColoniaClienteInAdmin.Text.Trim()}, " +
					$"{txtCiudadEditarClienteInAdmin.Text.Trim()}, " +
					$"{txtEstadoEditarClienteInAdmin.Text.Trim()}, " +
					$"{txtPaisEditarClienteInAdmin.Text.Trim()}";
			}
			else
			{
				nuevoDato = txtNuevoDatoEditarClienteInAdmin.Text.Trim();

				if (string.IsNullOrWhiteSpace(nuevoDato))
				{
					MessageBox.Show("Escribe el nuevo valor.");
					return;
				}
			}

			try
			{
				var repo = new RepoCliente();

				await repo.ActualizarClienteAsync(idCliente, campoSeleccionado, nuevoDato);

				MessageBox.Show(
					"Cambios guardados correctamente.",
					"Éxito",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);

				CargarGridClientes();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "No se pudieron guardar los cambios", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			tabClientesInAdmin.SelectedIndex = 0;
			cboBoxEditarClienteInAdmin.SelectedIndex = -1;
			txtNuevoDatoEditarClienteInAdmin.Clear();
			txtCalleEditarClienteInAdmin.Clear();
			txtNoExteriorEditarClienteInAdmin.Clear();
			txtNoInteriorEditarClienteInAdmin.Clear();
			txtCPEditarClienteInAdmin.Clear();
			txtColoniaEditarClienteInAdmin.Clear();
			txtCiudadEditarClienteInAdmin.Clear();
			txtEstadoEditarClienteInAdmin.Clear();
		}

		//==========================================
		// BOTON EDITAR CLIENTE
		//==========================================
		private void btnEditarClienteAdmin_Click(object sender, EventArgs e)
		{
			tabClientesInAdmin.SelectedIndex = 2;
		}
		//==========================================
		// EVENTO SELECCION DE FILA EN GRID CLIENTES
		//==========================================
		private void GridClientesInClientes_SelectionChanged(object sender, EventArgs e)
		{
			var fila = GridClientesInClientes.CurrentRow;

			if (fila is null || fila.IsNewRow)
				return;

			int idCliente = Convert.ToInt32(fila.Cells["IdCliente"].Value);

			lblNombreCompletoEditarClienteInAdmin.Text =
				$"{fila.Cells["Nombre"].Value} " +
				$"{fila.Cells["ApPaterno"].Value} " +
				$"{fila.Cells["ApMaterno"].Value}".Trim();


			lblFechaNacimientoEditarClienteInAdmin.Text = fila.Cells["FechaNacimiento"].Value?.ToString() ?? "";
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{

		}

		//======================
		// BOTON AGREGAR ESTADIO
		//======================
		private async void btnAgregarEstadioInAdmin_Click(object sender, EventArgs e)
		{
			try
			{
				var estadio = new Estadio
				{
					Nombre = txtNombreEstadioInAdmin.Text.Trim(),
					Ciudad = txtCiudadEstadioInAdmin.Text.Trim(),
					CapacidadTotal = int.Parse(txtCapacidadEstadioInAdmin.Text),
					CreadoPor = Sesion.IdUsuario
				};

				_idEstadioEnCaptura = await new RepoEstadio().AgregarEstadioAsync(estadio);
				_zonasAgregadas = 0;

				txtNombreEstadioInAdmin.ReadOnly = true;
				txtCiudadEstadioInAdmin.ReadOnly = true;
				txtCapacidadEstadioInAdmin.ReadOnly = true;
				btnAgregarEstadioInAdmin.Enabled = false;
				btnAgregarZonaInAdmin.Enabled = true;

				MessageBox.Show(
					$"Estadio guardado. ID: {_idEstadioEnCaptura}. Ahora agrega sus zonas.");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"No se pudo agregar el estadio.\n{ex.Message}");
			}
		}
		//===================
		// BOTON AGREGAR ZONA
		//===================
		private async void btnAgregarZonaInAdmin_Click(object sender, EventArgs e)
		{
			if (_idEstadioEnCaptura is null)
			{
				MessageBox.Show("Primero agrega un estadio.");
				return;
			}

			MessageBox.Show(
			$"Precio recibido: '{txtPrecioZonaInAdmin.Text}'\n" +
			$"Asientos recibidos: '{txtAsientosZonaInAdmin.Text}'");

			if (!decimal.TryParse(txtPrecioZonaInAdmin.Text, out decimal precio) ||
				!int.TryParse(txtAsientosZonaInAdmin.Text, out int cupo))
			{
				MessageBox.Show("Revisa el precio y la cantidad de asientos.");
				return;
			}

			try
			{
				var zona = new Zona
				{
					IdEstadio = _idEstadioEnCaptura.Value,
					NombreZona = txtNombreZonaInAdmin.Text.Trim(),
					PrecioBase = precio,
					CupoZona = cupo,
					CreadoPor = Sesion.IdUsuario
				};

				await new RepoZona().AgregarZonaAsync(zona);
				_zonasAgregadas++;

				txtNombreZonaInAdmin.Clear();
				txtPrecioZonaInAdmin.Clear();
				txtAsientosZonaInAdmin.Clear();

				MessageBox.Show("Zona agregada al estadio.");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"No se pudo agregar la zona.\n{ex.Message}");
			}
		}

		//================================
		// BOTON FINALIZAR AGREGAR PARTIDO
		//================================
		private void btnFinalizarAgregarPartidoInAdmin_Click(object sender, EventArgs e)
		{
			if (_idEstadioEnCaptura is null)
			{
				MessageBox.Show("Primero agrega un estadio.");
				return;
			}

			if (_zonasAgregadas == 0)
			{
				MessageBox.Show("Agrega al menos una zona antes de finalizar.");
				return;
			}

			txtNombreEstadioInAdmin.ReadOnly = false;
			txtCiudadEstadioInAdmin.ReadOnly = false;
			txtCapacidadEstadioInAdmin.ReadOnly = false;
			btnAgregarEstadioInAdmin.Enabled = true;
			btnAgregarZonaInAdmin.Enabled = false;

			_idEstadioEnCaptura = null;
			_zonasAgregadas = 0;

			MessageBox.Show("Registro del estadio y sus zonas finalizado.");
		}
	}


}
