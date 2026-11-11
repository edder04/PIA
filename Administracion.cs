
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
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
using System.Globalization;
using Color = System.Drawing.Color;

namespace PIA{                                                                                          // Indico que este código pertenece al proyecto PIA.
	public partial class Administracion : Form
	{                                                         // Creo la ventana de administración que hereda las funciones de Form.
		private int? _idEstadioEnCaptura = null;
		private int _zonasAgregadas = 0;
		private vw_Partidos? _partidoParaVenta;
		private vw_Clientes? _clienteParaVenta;
		private vw_DisponibilidadZonas? _zonaParaVenta;
		private readonly HashSet<string> _asientosOcupados = new();
		private readonly HashSet<string> _asientosSeleccionados = new();
		private const int AsientosPorFila = 15;
		private const decimal TasaIva = 0.16m;
		private bool _cargandoVentasCancelables;
		private bool _recargarVentasCancelables;

		public Administracion()
		{                                                                        // Este es el constructor que se ejecuta cuando abro la ventana.

			InitializeComponent();                                                                      // Cargo todos los controles y elementos de la ventana.
			ConfigurarGridCancelaciones();
			btnCancelacion.Click += btnCancelacion_Click;
			txtBuscarCancelarInAdmin.TextChanged += txtBuscarCancelarInAdmin_TextChanged;
			button1.Click += button1_Click;
			cboZonaVentaPaso3.SelectedIndexChanged += cboZonaVentaPaso3_SelectedIndexChanged;
			pnlMenuInAdmin.Visible = false;                                                             // Oculto el menú de administración al iniciar.

			ConfigurarGridReporteVentas();
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

			//=============================================================
			// CONFIGURACIÓN DEL GRID DE CLIENTES EN LA PESTAÑA DE CLIENTES
			//=============================================================
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

			//===========================================================
			// CONFIGURACIÓN DEL GRID DE CLIENTES EN LA PESTAÑA DE VENTAS
			//===========================================================

			gridClientesVentasInAdmin.Columns.Add("IdCliente", "IdCliente");
			gridClientesVentasInAdmin.Columns.Add("Nombre", "Nombre");
			gridClientesVentasInAdmin.Columns.Add("ApPaterno", "Apellido Paterno");
			gridClientesVentasInAdmin.Columns.Add("ApMaterno", "Apellido Materno");
			gridClientesVentasInAdmin.Columns.Add("FechaNacimiento", "Fecha de nacimiento");
			gridClientesVentasInAdmin.Columns.Add("Edad", "Edad");
			gridClientesVentasInAdmin.Columns.Add("Domicilio", "Domicilio");
			gridClientesVentasInAdmin.Columns.Add("RFC", "RFC");
			gridClientesVentasInAdmin.Columns.Add("CorreoElectronico", "Correo");
			gridClientesVentasInAdmin.Columns.Add("TelefonoCasa", "Teléfono casa");
			gridClientesVentasInAdmin.Columns.Add("TelefonoCelular", "Teléfono celular");
			gridClientesVentasInAdmin.Columns.Add("Activo", "Activo");
			gridClientesVentasInAdmin.ReadOnly = true;
			gridClientesVentasInAdmin.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gridClientesVentasInAdmin.MultiSelect = false;
			gridClientesVentasInAdmin.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			CargarGridClientesInVentas();
			//gridClientesVentasInAdmin.SelectionChanged += GridClientesVentasInAdmin_SelectionChanged;

			gridPartidosVentaInAdmin.ReadOnly = true;
			gridPartidosVentaInAdmin.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gridPartidosVentaInAdmin.MultiSelect = false;
			gridPartidosVentaInAdmin.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			if (!gridPartidosVentaInAdmin.Columns.Contains("IdPartido"))
			{
				gridPartidosVentaInAdmin.Columns.Insert(0, new DataGridViewTextBoxColumn
				{
					Name = "IdPartido",
					HeaderText = "IdPartido",
					Visible = false
				});
			}
			CargarGridPartidos();
		}
		//=======================
		// METODO CARGAR PARTIDOS
		//=======================
		private async Task CargarPartidos()
		{
			pnlPartidos.Controls.Clear();

			using var db = new AppDbContext();

			var partidos = await db.vw_Partidos
				.AsNoTracking()
				.Where(p => p.Estado == "Programado" &&
							p.FechaHora >= DateTime.Now)
				.OrderBy(p => p.FechaHora)
				.ToListAsync();

			foreach (var partido in partidos)
			{
				var tarjeta = new UcCardPartido();
				tarjeta.CargarPartido(partido);

				tarjeta.PartidoElegido += partidoElegido =>
				{
					_partidoParaVenta = partidoElegido;

					txtBuscarEquipoVentaPaso2.Text =
						$"{partidoElegido.EquipoLocal} vs. {partidoElegido.EquipoVisitante}";

					tabCtrlInAdmin.SelectedIndex = 1; // Pestaña principal de Ventas
					tabCtrlVenta.SelectedIndex = 0;  // Primer paso del flujo de venta

				};

				pnlPartidos.Controls.Add(tarjeta);
			}
		}
		//==================================
		// LOAD DE LA VENTANA ADMINISTRACION
		//==================================
		private async void Administracion_Load(object sender, EventArgs e)
		{                                   // Este método se ejecuta cuando se carga la ventana de administración.
			pictureBox1.Image = Image
				.FromFile(@"C:\Users\edcru\Downloads\UANL\Agosto-Diciembre 2026\checkMark.gif");        // Cargo el GIF desde la ruta donde tengo guardado el archivo.
			pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;                                     // Ajusto la imagen para que ocupe el espacio del PictureBox.
			await CargarFiltrosReporteAsync();
			await CargarVentasCancelablesAsync();

			await CargarPartidos();
			tabCtrlPartidoEstadio.Visible = false;
			pnlPartidos.Size = new Size(1390, 690);

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
		//==========================================
		// METODO CARGAR GRID CLIENTES
		//==========================================
		private void CargarGridClientesInVentas()
		{
			string texto = txtBuscarCliente.Text;
			using var db = new AppDbContext();
			var clientes = db.vw_Clientes
				.Where(c => c.ApPaterno.Contains(texto) || c.CorreoElectronico.Contains(texto) || c.RFC.Contains(texto));

			gridClientesVentasInAdmin.Rows.Clear();

			foreach (vw_Clientes c in clientes)
			{
				gridClientesVentasInAdmin.Rows.Add(
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

		//=====================================
		// BOTON DE INICIO EN EL MENU PRINCIPAL
		//=====================================
		private void btnInicio_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 0;
			pnlMenuInAdmin.Visible = false;
		}
		//=====================================
		// BOTON DE VENTAS EN EL MENU PRINCIPAL
		//=====================================
		private void btnVentasVentas_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 1;
			pnlMenuInAdmin.Visible = false;
		}

		private void btnClientesVentas_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 2;
			pnlMenuInAdmin.Visible = false;
		}

		private void btnPartidosVentas_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 3;
			pnlMenuInAdmin.Visible = false;
		}

		private void btnFacturasVentas_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 4;
			pnlMenuInAdmin.Visible = false;
		}

		private void btnReportesVentas_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 5;
			pnlMenuInAdmin.Visible = false;
		}

		private void btnUsuarios_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 6;
			pnlMenuInAdmin.Visible = false;
			tabCtrlUsauriosInAdmin.SelectedIndex = 0;
		}

		//=======================
		// CONFIGURA EL GRID DE VENTAS PARA CANCELACION
		//=======================
		private void ConfigurarGridCancelaciones()
		{
			dgvCancelaciones.Columns.Clear();
			dgvCancelaciones.Columns.Add("FOLIO", "FOLIO");
			dgvCancelaciones.Columns.Add("FECHA", "FECHA DE VENTA");
			dgvCancelaciones.Columns.Add("CLIENTE", "CLIENTE");
			dgvCancelaciones.Columns.Add("RFC", "RFC");
			dgvCancelaciones.Columns.Add("BOLETOS", "BOLETOS");
			dgvCancelaciones.Columns.Add("TOTAL", "TOTAL");
			dgvCancelaciones.Columns.Add("ESTADO", "ESTADO");
			dgvCancelaciones.Columns.Add("MOTIVO", "MOTIVO DE CANCELACION");
			dgvCancelaciones.ReadOnly = true;
			dgvCancelaciones.AllowUserToAddRows = false;
			dgvCancelaciones.AllowUserToDeleteRows = false;
			dgvCancelaciones.MultiSelect = false;
			dgvCancelaciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgvCancelaciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dgvCancelaciones.BackgroundColor = Color.FromArgb(60, 60, 60);
			dgvCancelaciones.EnableHeadersVisualStyles = false;
			dgvCancelaciones.DefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60);
			dgvCancelaciones.DefaultCellStyle.ForeColor = Color.White;
			dgvCancelaciones.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 90, 160);
			dgvCancelaciones.DefaultCellStyle.SelectionForeColor = Color.White;
			dgvCancelaciones.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60);
			dgvCancelaciones.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
			dgvCancelaciones.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60);
			dgvCancelaciones.AlternatingRowsDefaultCellStyle.ForeColor = Color.White;
		}

		//=======================
		// ABRE LA PESTANA DE CANCELACIONES
		//=======================
		private async void btnCancelacion_Click(object? sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 7;
			pnlMenuInAdmin.Visible = false;
			await CargarVentasCancelablesAsync();
		}

		//=======================
		// FILTRA LAS VENTAS POR FOLIO, RFC, CLIENTE O ESTADO
		//=======================
		private async void txtBuscarCancelarInAdmin_TextChanged(object? sender, EventArgs e)
		{
			await CargarVentasCancelablesAsync();
		}

		//=======================
		// CARGA VENTAS Y CANCELACIONES EN EL GRID
		//=======================
		private async Task CargarVentasCancelablesAsync()
		{
			if (_cargandoVentasCancelables)
			{
				_recargarVentasCancelables = true;
				return;
			}

			_cargandoVentasCancelables = true;
			try
			{
				string busqueda = txtBuscarCancelarInAdmin.Text.Trim();
				using var db = new AppDbContext();
				var ventas = await db.vw_Ventas.AsNoTracking()
					.OrderByDescending(v => v.FechaHoraVenta)
					.ToListAsync();
				var cancelaciones = await db.vw_Cancelaciones.AsNoTracking()
					.ToListAsync();
				var motivoPorVenta = cancelaciones
					.GroupBy(c => c.IdVenta)
					.ToDictionary(g => g.Key, g => g.OrderByDescending(c => c.FechaHoraCancelacion).First().Motivo);

				var filtradas = ventas.Where(v =>
					string.IsNullOrWhiteSpace(busqueda) ||
					v.Folio.ToString().Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
					v.Cliente.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
					v.RFC.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
					v.EstadoVenta.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
					(motivoPorVenta.TryGetValue(v.IdVenta, out string? motivo) &&
					 motivo.Contains(busqueda, StringComparison.OrdinalIgnoreCase)))
					.ToList();

				dgvCancelaciones.Rows.Clear();
				foreach (var venta in filtradas)
				{
					motivoPorVenta.TryGetValue(venta.IdVenta, out string? motivo);
					int fila = dgvCancelaciones.Rows.Add(
						venta.Folio,
						venta.FechaHoraVenta.ToString("dd/MM/yyyy HH:mm"),
						venta.Cliente,
						venta.RFC,
						$"{venta.TotalBoletos - venta.BoletosCancelados} / {venta.TotalBoletos}",
						venta.Total.ToString("C2"),
						venta.EstadoVenta,
						motivo ?? string.Empty);
					dgvCancelaciones.Rows[fila].Tag = venta;
				}
				dgvCancelaciones.ClearSelection();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudieron cargar las ventas", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				_cargandoVentasCancelables = false;
				if (_recargarVentasCancelables)
				{
					_recargarVentasCancelables = false;
					await CargarVentasCancelablesAsync();
				}
			}
		}

		//=======================
		// REGISTRA LA CANCELACION DE LA VENTA SELECCIONADA
		//=======================
		private async void button1_Click(object? sender, EventArgs e)
		{
			if (dgvCancelaciones.SelectedRows.Count == 0 ||
				dgvCancelaciones.SelectedRows[0].Tag is not vw_Ventas venta)
			{
				MessageBox.Show("Selecciona una venta del grid.");
				return;
			}

			if (!string.Equals(venta.EstadoVenta, "Activa", StringComparison.OrdinalIgnoreCase))
			{
				MessageBox.Show("Solo se pueden cancelar ventas que estén activas.");
				return;
			}

			string motivo = txtMotivoCancelarInAdmin.Text.Trim();
			if (string.IsNullOrWhiteSpace(motivo))
			{
				MessageBox.Show("Escribe el motivo de la cancelación.");
				txtMotivoCancelarInAdmin.Focus();
				return;
			}

			var confirmar = MessageBox.Show(
				$"¿Autorizas cancelar la venta {venta.Folio}? El trigger actualizará la venta y sus boletos.",
				"Confirmar cancelación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (confirmar != DialogResult.Yes)
				return;

			button1.Enabled = false;
			try
			{
				await new RepoCancelacion().RegistrarAsync(new Cancelacion
				{
					IdVenta = venta.IdVenta,
					IdUsuario = Sesion.IdUsuario,
					Motivo = motivo
				});

				MessageBox.Show("La cancelación fue registrada correctamente.", "Cancelación autorizada",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				txtMotivoCancelarInAdmin.Clear();
				await CargarVentasCancelablesAsync();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudo cancelar la venta", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				button1.Enabled = true;
			}
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

		private void button2_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 2;
			tabClientesInAdmin.SelectedIndex = 1;
		}

		//==============================
		// BOTON CONTINUAR VENTA PASO 1
		//==============================
		private async void btnContinuarVentaPaso1_Click(object sender, EventArgs e)
		{
			var fila = gridClientesVentasInAdmin.CurrentRow;

			if (fila is null || fila.IsNewRow ||
				fila.Cells["IdCliente"].Value is null)
			{
				MessageBox.Show("Selecciona un cliente.");
				return;
			}

			int idCliente = Convert.ToInt32(fila.Cells["IdCliente"].Value);

			using var db = new AppDbContext();

			_clienteParaVenta = await db.vw_Clientes
				.AsNoTracking()
				.SingleOrDefaultAsync(c => c.IdCliente == idCliente);

			if (_clienteParaVenta is null)
			{
				MessageBox.Show("No se encontró el cliente.");
				return;
			}

			tabCtrlVenta.SelectedIndex = 1;
		}

		//==============================
		// BOTON ATRAS VENTA PASO 2
		//==============================
		private void btnAtrasVentaPaso2_Click(object sender, EventArgs e)
		{
			tabCtrlVenta.SelectedIndex = 0;
		}

		//==============================
		// BOTON CONTINUAR VENTA PASO 2
		//==============================
		private async void btnContinuarVentaPaso2_Click(object sender, EventArgs e)
		{
			var fila = gridPartidosVentaInAdmin.CurrentRow;

			if (fila is null || fila.IsNewRow ||
				fila.Cells["IdPartido"].Value is null)
			{
				MessageBox.Show("Selecciona un partido.");
				return;
			}

			int idPartido = Convert.ToInt32(fila.Cells["IdPartido"].Value);

			using var db = new AppDbContext();

			_partidoParaVenta = await db.vw_Partidos
				.AsNoTracking()
				.SingleOrDefaultAsync(p => p.IdPartido == idPartido);

			if (_partidoParaVenta is null)
			{
				MessageBox.Show("No se encontró el partido.");
				return;
			}

			lblInfoPartidoVentaPaso3.Text =
				$"{_partidoParaVenta.EquipoLocal} vs. {_partidoParaVenta.EquipoVisitante}";
			lblInfoEstadioVentaPaso3.Text = _partidoParaVenta.Estadio;
			lblFechaVentaPaso3.Text = _partidoParaVenta.FechaHora.ToString("dd/MM/yyyy HH:mm");

			var zonas = await db.vw_DisponibilidadZonas
				.AsNoTracking()
				.Where(z => z.IdPartido == idPartido)
				.ToListAsync();

			cboZonaVentaPaso3.DataSource = zonas;
			cboZonaVentaPaso3.DisplayMember = "NombreZona";
			cboZonaVentaPaso3.ValueMember = "IdZona";
			cboZonaVentaPaso3.SelectedIndex = -1;

			tabCtrlVenta.SelectedIndex = 2;
		}

		//==============================
		// BOTON ATRAS VENTA PASO 3
		//==============================
		private void btnAtrasVentaPaso3_Click(object sender, EventArgs e)
		{
			tabCtrlVenta.SelectedIndex = 1;
		}

		//==============================
		// BOTON CONTINUAR VENTA PASO 3
		//==============================
		private void btnContinuarVentaPaso3_Click(object sender, EventArgs e)
		{
			if (_asientosSeleccionados.Count == 0)
			{
				MessageBox.Show("Selecciona al menos un asiento.");
				return;
			}

			PrepararPaso4();
			tabCtrlVenta.SelectedIndex = 3;
		}

		//==============================
		// BOTON ATRAS VENTA PASO 4
		//==============================
		private void btnAtrasVentaPaso4_Click(object sender, EventArgs e)
		{
			tabCtrlVenta.SelectedIndex = 2;
		}

		//==============================
		// BOTON CONTINUAR VENTA PASO 4
		//==============================
		private async void btnConfirmarVentaPaso4_Click(object sender, EventArgs e)
		{
			if (_clienteParaVenta is null ||
				_partidoParaVenta is null ||
				_zonaParaVenta is null ||
				_asientosSeleccionados.Count == 0)
			{
				MessageBox.Show("Faltan datos de cliente, partido, zona o asientos.");
				return;
			}

			string metodoPago;

			if (radioBtnEfectivoVentaPaso4.Checked)
			{
				metodoPago = "Efectivo";
			}
			else if (radioBtnCreditoVentaPaso4.Checked)
			{
				if (string.IsNullOrWhiteSpace(txtTitularVentaPaso4.Text))
				{
					MessageBox.Show("Escribe el nombre del titular de la tarjeta.");
					txtTitularVentaPaso4.Focus();
					return;
				}

				string numero = Regex.Replace(
					txtNumeroTarjetaVentaPaso4.Text,
					@"[\s-]",
					"");

				string vencimiento = txtVencimientoTarjetaVentaPaso4.Text.Trim();
				string cvv = txtCVVVentaPaso4.Text.Trim();

				if (!Regex.IsMatch(numero, @"^\d+$"))
				{
					MessageBox.Show("El número de tarjeta solo debe contener dígitos.");
					txtNumeroTarjetaVentaPaso4.Focus();
					return;
				}

				string? marcaTarjeta = DetectarMarcaTarjeta(numero);

				if (marcaTarjeta is null || !PasaLuhn(numero))
				{
					MessageBox.Show("El número de tarjeta no es válido.");
					txtNumeroTarjetaVentaPaso4.Focus();
					return;
				}

				if (!DateTime.TryParseExact(
						vencimiento,
						new[] { "MM/yy", "MM/yyyy" },
						CultureInfo.InvariantCulture,
						DateTimeStyles.None,
						out DateTime fechaVencimiento))
				{
					MessageBox.Show("Escribe el vencimiento como MM/AA, por ejemplo 08/28.");
					txtVencimientoTarjetaVentaPaso4.Focus();
					return;
				}

				DateTime finDelMes = new DateTime(
						fechaVencimiento.Year,
						fechaVencimiento.Month,
						1)
					.AddMonths(1)
					.AddTicks(-1);

				if (finDelMes < DateTime.Today)
				{
					MessageBox.Show("La tarjeta está vencida.");
					txtVencimientoTarjetaVentaPaso4.Focus();
					return;
				}

				int longitudCvv = marcaTarjeta == "American Express" ? 4 : 3;

				if (!Regex.IsMatch(cvv, $@"^\d{{{longitudCvv}}}$"))
				{
					MessageBox.Show(
						$"El CVV de {marcaTarjeta} debe tener {longitudCvv} dígitos.");
					txtCVVVentaPaso4.Focus();
					return;
				}

				// Guarda solo el método. Esta comprobación de formato no autoriza un cobro.
				metodoPago = "Crédito";
			}
			else
			{
				MessageBox.Show("Selecciona efectivo o tarjeta.");
				return;
			}

			decimal precioUnitario = _zonaParaVenta.PrecioBase;
			int cantidad = _asientosSeleccionados.Count;
			decimal subtotal = precioUnitario * cantidad;
			decimal iva = subtotal * TasaIva;
			decimal total = subtotal + iva;

			var venta = new Venta
			{
				IdCliente = _clienteParaVenta.IdCliente,
				IdUsuario = Sesion.IdUsuario,
				FechaHoraVenta = DateTime.Now,
				Subtotal = subtotal,
				IVA = iva,
				Total = total,
				MetodoPago = metodoPago,
				EstadoVenta = "Activa"
			};

			var detalle = new DetalleVenta
			{
				IdPartido = _partidoParaVenta.IdPartido,
				IdZona = _zonaParaVenta.IdZona,
				Cantidad = cantidad,
				PrecioUnitario = precioUnitario,
				Subtotal = subtotal,
				IVA = iva,
				Total = total
			};

			var boletos = new List<Boleto>();

			foreach (string clave in _asientosSeleccionados)
			{
				string[] partes = clave.Split('-');

				if (partes.Length != 2)
				{
					MessageBox.Show($"El asiento '{clave}' tiene un formato inesperado.");
					return;
				}

				boletos.Add(new Boleto
				{
					IdPartido = _partidoParaVenta.IdPartido,
					IdZona = _zonaParaVenta.IdZona,
					Fila = partes[0],
					Asiento = partes[1],
					PrecioUnitario = precioUnitario,
					EstadoBoleto = "Vendido"
				});
			}

			btnConfirmarVentaPaso4.Enabled = false;

			try
			{
				int idVenta = await new RepoVenta()
					.RegistrarVentaAsync(venta, detalle, boletos);

				MessageBox.Show($"Venta registrada correctamente. Folio: {idVenta}");

				// Solo avanza si se guardó la venta correctamente.
				tabCtrlVenta.SelectedIndex = 4;
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					ex.InnerException?.Message ?? ex.Message,
					"No se pudo registrar la venta",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			finally
			{
				btnConfirmarVentaPaso4.Enabled = true;

				// No se conservan datos sensibles en el formulario.
				txtTitularVentaPaso4.Clear();
				txtNumeroTarjetaVentaPaso4.Clear();
				txtVencimientoTarjetaVentaPaso4.Clear();
				txtCVVVentaPaso4.Clear();
			}
		}
		//=================================
		// METODO DETECTAR MARCA DE TARJETA
		//=================================
		private static string? DetectarMarcaTarjeta(string numero)
		{
			if (numero.StartsWith("4") &&
				numero.Length is 13 or 16 or 19)
			{
				return "Visa";
			}

			if (numero.Length == 16)
			{
				int primerosDos = int.Parse(numero[..2]);
				int primerosCuatro = int.Parse(numero[..4]);

				if (primerosDos is >= 51 and <= 55 ||
					primerosCuatro is >= 2221 and <= 2720)
				{
					return "Mastercard";
				}
			}

			if (numero.Length == 15 &&
				(numero.StartsWith("34") || numero.StartsWith("37")))
			{
				return "American Express";
			}

			return null;
		}

		private static bool PasaLuhn(string numero)
		{
			int suma = 0;
			bool duplicar = false;

			for (int i = numero.Length - 1; i >= 0; i--)
			{
				int digito = numero[i] - '0';

				if (duplicar)
				{
					digito *= 2;
					if (digito > 9)
						digito -= 9;
				}

				suma += digito;
				duplicar = !duplicar;
			}

			return suma % 10 == 0;
		}

		//========================================
		// BOTON NUEVA VENTA DESDE VENTA REALIZADA
		//========================================
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

		//=====================
		// BOTON EDITAR CLIENTE
		//=====================
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
			txtNombreEstadioInAdmin.Clear();
			txtCiudadEstadioInAdmin.ReadOnly = false;
			txtCiudadEstadioInAdmin.Clear();
			txtCapacidadEstadioInAdmin.ReadOnly = false;
			txtCapacidadEstadioInAdmin.Clear();
			btnAgregarEstadioInAdmin.Enabled = true;
			btnAgregarZonaInAdmin.Enabled = false;

			_idEstadioEnCaptura = null;
			_zonasAgregadas = 0;

			MessageBox.Show("Registro del estadio y sus zonas finalizado.");

			tabCtrlPartidoEstadio.Visible = false;
			pnlPartidos.Size = new Size(1390, 690);
		}

		//======================
		// BOTON AGREGAR PARTIDO
		//======================
		private async void btnAgregarPartidoInAdmin_Click(object sender, EventArgs e)
		{
			if (cboEstadioInAdmin.SelectedValue is null ||
				string.IsNullOrWhiteSpace(txtHoraPartidoInAdmin.Text) ||
				string.IsNullOrWhiteSpace(txtEquipoLocalPartidoInAdmin.Text) ||
				string.IsNullOrWhiteSpace(txtEquipoVisitantePartidoInAdmin.Text))
			{
				MessageBox.Show("Verifica que todos los campos estén completos.");
				return;
			}

			if (!int.TryParse(cboEstadioInAdmin.SelectedValue.ToString(), out int idEstadio))
			{
				MessageBox.Show("Selecciona un estadio válido.");
				return;
			}

			if (!TimeSpan.TryParse(txtHoraPartidoInAdmin.Text.Trim(), out TimeSpan hora))
			{
				MessageBox.Show("Escribe una hora válida, por ejemplo: 19:30.");
				return;
			}

			if (string.IsNullOrWhiteSpace(txtEstadoPartidoInAdmin.Text))
			{
				MessageBox.Show("Indica el estado del partido.");
				return;
			}

			try
			{
				var partido = new Partido
				{
					IdEstadio = idEstadio,
					FechaHora = dteFechaPartidoInAdmin.Value.Date.Add(hora),
					EquipoLocal = txtEquipoLocalPartidoInAdmin.Text.Trim(),
					EquipoVisitante = txtEquipoVisitantePartidoInAdmin.Text.Trim(),
					Estado = txtEstadoPartidoInAdmin.Text.Trim(),
					CreadoPor = Sesion.IdUsuario
				};

				await new RepoPartido().AgregarPartidoAsync(partido);

				MessageBox.Show("Partido agregado correctamente.");

				cboEstadioInAdmin.SelectedIndex = -1;
				dteFechaPartidoInAdmin.Value = DateTime.Today;
				txtHoraPartidoInAdmin.Clear();
				txtEquipoLocalPartidoInAdmin.Clear();
				txtEquipoVisitantePartidoInAdmin.Clear();
				txtEstadoPartidoInAdmin.Clear();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"No se pudo agregar el partido.\n{ex.Message}");
			}
		}

		//========================
		// BOTON FINALIZAR PARTIDO
		//========================
		private void btnFinalizarPartidoInAdmin_Click(object sender, EventArgs e)
		{
			tabCtrlPartidoEstadio.Visible = false;
			pnlPartidos.Size = new Size(1390, 690);
		}

		private void btnAgregarPartido_Click(object sender, EventArgs e)
		{
			tabCtrlPartidoEstadio.Visible = true;
			tabCtrlPartidoEstadio.SelectedIndex = 0;
			pnlPartidos.Size = new Size(700, 690);
		}

		private void btnAgregarEstadio_Click(object sender, EventArgs e)
		{
			tabCtrlPartidoEstadio.Visible = true;
			tabCtrlPartidoEstadio.SelectedIndex = 1;
			pnlPartidos.Size = new Size(700, 690);
		}

		private void btnRegresarEstadioZonaInAdmin_Click(object sender, EventArgs e)
		{
			tabCtrlPartidoEstadio.Visible = false;
			pnlPartidos.Size = new Size(1390, 690);
		}

		//======================================
		// ACTUALIZAR GRID DE PARTIDOS EN VENTAS
		//======================================
		private void txtBuscarEquipoVentaPaso2_TextChanged(object sender, EventArgs e)
		{
			CargarGridPartidos();
		}

		//=========================================
		// METODO CARGAR GRID DE PARTIDOS EN VENTAS
		//=========================================
		private void CargarGridPartidos()
		{
			string texto = txtBuscarEquipoVentaPaso2.Text.Trim();

			using var db = new AppDbContext();

			var consulta = db.vw_Partidos.AsNoTracking();

			if (!string.IsNullOrWhiteSpace(texto))
			{
				consulta = consulta.Where(p =>
					p.EquipoLocal.Contains(texto) ||
					p.EquipoVisitante.Contains(texto) ||
					p.Estadio.Contains(texto) ||
					(p.EquipoLocal + " vs. " + p.EquipoVisitante).Contains(texto));
			}

			var partidos = consulta.ToList();

			gridPartidosVentaInAdmin.Rows.Clear();

			foreach (var p in partidos)
			{
				gridPartidosVentaInAdmin.Rows.Add(
					p.IdPartido,
					p.FechaHora.ToString("g"),
					p.EquipoLocal,
					p.EquipoVisitante,
					p.Estadio,
					p.Estado,
					p.Ciudad,
					p.BoletosDisponibles);
			}
		}
		//=========================================
		// EVENTO SELECCION DE ZONA EN VENTA PASO 3
		//=========================================
		private async void cboZonaVentaPaso3_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboZonaVentaPaso3.SelectedItem is not vw_DisponibilidadZonas zona)
				return;

			_zonaParaVenta = zona;

			lblInfoPrecioVentaPaso3.Text = zona.PrecioBase.ToString("C");
			lblInfoDisponiblesVentaPaso3.Text = zona.BoletosDisponibles.ToString();
			await CargarGridAsientosAsync();

		}

		//===================================================
		// METODO CONFIGURAR GRID DE ASIENTOS EN VENTA PASO 3
		//===================================================
		private void ConfigurarGridAsientos()
		{
			dgvAsientosVentaPaso3.Columns.Clear();
			dgvAsientosVentaPaso3.Rows.Clear();

			dgvAsientosVentaPaso3.AllowUserToAddRows = false;
			dgvAsientosVentaPaso3.ReadOnly = true;
			dgvAsientosVentaPaso3.MultiSelect = false;
			dgvAsientosVentaPaso3.SelectionMode = DataGridViewSelectionMode.CellSelect;
			dgvAsientosVentaPaso3.RowHeadersVisible = true;
			dgvAsientosVentaPaso3.AutoSizeColumnsMode =
				DataGridViewAutoSizeColumnsMode.Fill;

			for (int asiento = 1; asiento <= AsientosPorFila; asiento++)
			{
				dgvAsientosVentaPaso3.Columns.Add(
					$"Asiento{asiento:00}",
					asiento.ToString("00"));
			}

			dgvAsientosVentaPaso3.CellClick += dgvAsientosVentaPaso3_CellClick;
		}

		//===============================================
		// METODO CARGAR GRID DE ASIENTOS EN VENTA PASO 3
		//===============================================
		private async Task CargarGridAsientosAsync()
		{
			if (_partidoParaVenta is null || _zonaParaVenta is null)
				return;

			_asientosSeleccionados.Clear();
			_asientosOcupados.Clear();

			ConfigurarGridAsientos();

			using var db = new AppDbContext();

			var ocupados = await db.vw_AsientosOcupados
				.AsNoTracking()
				.Where(a => a.IdPartido == _partidoParaVenta.IdPartido &&
							a.IdZona == _zonaParaVenta.IdZona)
				.Select(a => new { a.Fila, a.Asiento })
				.ToListAsync();

			foreach (var asiento in ocupados)
				_asientosOcupados.Add($"{asiento.Fila}-{asiento.Asiento}");

			int cantidadFilas =
				(_zonaParaVenta.CupoZona + AsientosPorFila - 1) / AsientosPorFila;

			for (int filaIndex = 0; filaIndex < cantidadFilas; filaIndex++)
			{
				int indiceFila = dgvAsientosVentaPaso3.Rows.Add();
				var filaGrid = dgvAsientosVentaPaso3.Rows[indiceFila];

				filaGrid.HeaderCell.Value = "F" + (filaIndex + 1).ToString("D3");

				for (int columna = 0; columna < AsientosPorFila; columna++)
				{
					int indiceAsiento = filaIndex * AsientosPorFila + columna;
					var celda = filaGrid.Cells[columna];

					if (indiceAsiento >= _zonaParaVenta.CupoZona)
					{
						celda.Value = "";
						continue;
					}

					string fila = (filaIndex + 1).ToString("D3");
					string numero = (columna + 1).ToString("D2");
					string clave = $"{fila}-{numero}";

					celda.Value = numero;
					celda.Tag = clave;

					bool ocupado = _asientosOcupados.Contains(clave);
					celda.Style.BackColor = ocupado
						? System.Drawing.Color.IndianRed
						: System.Drawing.Color.LightGray;

					celda.ToolTipText = ocupado ? "Ocupado" : "Disponible";
				}
			}

			lblBoletosSeleccionadosVentaPaso3.Text = "0";
		}

		//===============================================
		// EVENTO CLICK EN CELDA DEL GRID DE ASIENTOS
		//===============================================
		private void dgvAsientosVentaPaso3_CellClick(
	object? sender,
	DataGridViewCellEventArgs e)
		{
			// Ignora encabezados y clics fuera de las celdas
			if (e.RowIndex < 0 || e.ColumnIndex < 0)
				return;

			var celda = dgvAsientosVentaPaso3
				.Rows[e.RowIndex]
				.Cells[e.ColumnIndex];

			// Las celdas fuera del cupo no tienen Tag
			if (celda.Tag is not string clave)
				return;

			// Los asientos ocupados no se pueden seleccionar
			if (_asientosOcupados.Contains(clave))
				return;

			if (_asientosSeleccionados.Contains(clave))
			{
				// Quitar selección
				_asientosSeleccionados.Remove(clave);
				celda.Style.BackColor = System.Drawing.Color.LightGray;
			}
			else
			{
				// Aplicar el límite de 4
				if (_asientosSeleccionados.Count >= 4)
				{
					MessageBox.Show("Solo puedes seleccionar hasta 4 asientos.");
					return;
				}

				_asientosSeleccionados.Add(clave);
				celda.Style.BackColor = System.Drawing.Color.LightGreen;
			}

			ActualizarTotalesVenta();
		}

		private void ActualizarTotalesVenta()
		{
			decimal precioUnitario = _zonaParaVenta?.PrecioBase ?? 0m;
			int cantidad = _asientosSeleccionados.Count;

			decimal subtotal = precioUnitario * cantidad;
			decimal iva = subtotal * TasaIva;
			decimal total = subtotal + iva;

			lblInfoSubtotalVentaPaso3.Text = $"SUBTOTAL: {subtotal:C2}";
			lvlInfoIVAVentaPaso3.Text = $"IVA: {iva:C2}";
			lblInfoTotalVentaPaso3.Text = $"TOTAL: {total:C2}";
			lblBoletosSeleccionadosVentaPaso3.Text =
				$"SELECCIONADOS: {cantidad} / 4";
		}

		private void PrepararPaso4()
		{
			if (_clienteParaVenta is null ||
				_partidoParaVenta is null ||
				_zonaParaVenta is null ||
				_asientosSeleccionados.Count == 0)
			{
				MessageBox.Show("Selecciona cliente, partido, zona y al menos un asiento.");
				return;
			}

			lblNombreVentaPaso4.Text =
				$"{_clienteParaVenta.Nombres} {_clienteParaVenta.ApPaterno} {_clienteParaVenta.ApMaterno}";

			lblInfoPartidoVentaPaso4.Text =
				$"{_partidoParaVenta.EquipoLocal} vs. {_partidoParaVenta.EquipoVisitante}";

			lblInfoZonaVentaPaso4.Text = _zonaParaVenta.NombreZona;
			lblInfoBoletosVentaPaso4.Text = _asientosSeleccionados.Count.ToString();

			lblInfoAsientosVentaPaso4.Text = string.Join(
				", ",
				_asientosSeleccionados.OrderBy(a => a));

			lblInfoTotalVentaPaso4.Text =
				$"TOTAL A COBRAR: {(_zonaParaVenta.PrecioBase * _asientosSeleccionados.Count * 1.16m):C2}";
		}

		private void radioBtnEfectivoVentaPaso4_CheckedChanged(object sender, EventArgs e)
		{

		}
		//=========================================
		// CLASE PARA FILTRO DE PARTIDOS EN REPORTE
		//=========================================
		private sealed class FiltroPartidoReporte
		{
			public int? IdPartido { get; set; }
			public string Descripcion { get; set; } = "";
		}

		//=========================================
		// METODO CARGAR FILTROS DE REPORTE
		//=========================================
		private async Task CargarFiltrosReporteAsync()
		{
			using var db = new AppDbContext();

			var estadios = await db.vw_BoletosVendidos
				.AsNoTracking()
				.Select(b => b.Estadio)
				.Distinct()
				.OrderBy(e => e)
				.ToListAsync();

			estadios.Insert(0, "Todos");

			cboEstadioReporte.DataSource = estadios;
			cboEstadioReporte.SelectedIndex = 0;

			var partidosDb = await db.vw_BoletosVendidos
				.AsNoTracking()
				.Select(b => new
				{
					b.IdPartido,
					b.EquipoLocal,
					b.EquipoVisitante
				})
				.Distinct()
				.ToListAsync();

			var partidos = partidosDb
				.Select(p => new FiltroPartidoReporte
				{
					IdPartido = p.IdPartido,
					Descripcion = $"{p.EquipoLocal} vs. {p.EquipoVisitante}"
				})
				.OrderBy(p => p.Descripcion)
				.ToList();

			partidos.Insert(0, new FiltroPartidoReporte
			{
				IdPartido = null,
				Descripcion = "Todos"
			});

			cboPartidoReporte.DataSource = partidos;
			cboPartidoReporte.DisplayMember = nameof(FiltroPartidoReporte.Descripcion);
			cboPartidoReporte.ValueMember = nameof(FiltroPartidoReporte.IdPartido);
			cboPartidoReporte.SelectedIndex = 0;
		}


		private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		//=========================================
		// BOTON FILTRAR REPORTE
		//=========================================
		private async void btnFiltrarReporte_Click_1(object sender, EventArgs e)
		{
			DateTime desde = dtpDesdeReporte.Value.Date;
			DateTime hasta = dtpHastaReporte.Value.Date;

			if (desde > hasta)
			{
				MessageBox.Show("La fecha Desde no puede ser posterior a Hasta.");
				return;
			}

			try
			{
				using var db = new AppDbContext();

				var consulta = db.vw_BoletosVendidos
					.AsNoTracking()
					.Where(b =>
						b.FechaHoraVenta >= desde &&
						b.FechaHoraVenta < hasta.AddDays(1) &&
						b.EstadoVenta == "Activa" &&
						b.EstadoBoleto == "Vendido");

				if (cboEstadioReporte.SelectedItem is string estadio &&
					estadio != "Todos")
				{
					consulta = consulta.Where(b => b.Estadio == estadio);
				}

				if (cboPartidoReporte.SelectedValue is int idPartido)
				{
					consulta = consulta.Where(b => b.IdPartido == idPartido);
				}

				var boletos = await consulta.ToListAsync();

				var filas = boletos
					.GroupBy(b => new
					{
						b.FolioVenta,
						b.Estadio,
						b.IdPartido,
						b.EquipoLocal,
						b.EquipoVisitante,
						b.IdZona,
						b.NombreZona
					})
					.Select(g =>
					{
						decimal subtotal = g.Sum(b => b.PrecioUnitario);

						return new
						{
							Folio = g.Key.FolioVenta,
							g.Key.Estadio,
							Partido = $"{g.Key.EquipoLocal} vs. {g.Key.EquipoVisitante}",
							Zona = g.Key.NombreZona,
							Cantidad = g.Count(),
							Subtotal = subtotal,
							Total = subtotal * (1m + TasaIva)
						};
					})
					.OrderBy(f => f.Folio)
					.ToList();

				dgvReporteVentas.Rows.Clear();

				foreach (var fila in filas)
				{
					dgvReporteVentas.Rows.Add(
						fila.Folio.ToString(),
						fila.Estadio,
						fila.Partido,
						fila.Zona,
						fila.Cantidad,
						fila.Subtotal,
						fila.Total);
				}

				int cantidadBoletos = filas.Sum(f => f.Cantidad);
				decimal ingresos = filas.Sum(f => f.Total);

				lblBoletosVendidosReporte.Text =
					$"BOLETOS VENDIDOS: {cantidadBoletos}";

				lblIngresosReporte.Text =
					$"INGRESOS: {ingresos:C2}";
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					ex.InnerException?.Message ?? ex.Message,
					"No se pudo cargar el reporte",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}
		//============================================
		// METODO CONFIGURAR GRID DE REPORTE DE VENTAS
		//============================================
		private void ConfigurarGridReporteVentas()
		{
			dgvReporteVentas.Columns.Clear();
			dgvReporteVentas.AutoGenerateColumns = false;
			dgvReporteVentas.AllowUserToAddRows = false;
			dgvReporteVentas.ReadOnly = true;
			dgvReporteVentas.RowHeadersVisible = false;
			dgvReporteVentas.AutoSizeColumnsMode =
				DataGridViewAutoSizeColumnsMode.Fill;

			dgvReporteVentas.Columns.Add("Folio", "FOLIO");
			dgvReporteVentas.Columns.Add("Estadio", "ESTADIO");
			dgvReporteVentas.Columns.Add("Partido", "PARTIDO");
			dgvReporteVentas.Columns.Add("Zona", "ZONA");
			dgvReporteVentas.Columns.Add("Cantidad", "CANTIDAD");
			dgvReporteVentas.Columns.Add("Subtotal", "SUBTOTAL");
			dgvReporteVentas.Columns.Add("Total", "TOTAL");

			dgvReporteVentas.Columns["Subtotal"]!
				.DefaultCellStyle.Format = "C2";

			dgvReporteVentas.Columns["Total"]!
				.DefaultCellStyle.Format = "C2";
		}

		//==============================
		// BOTON LIMPIAR FILTROS REPORTE
		//==============================
		private void button4_Click(object sender, EventArgs e)
		{
			dtpDesdeReporte.Value = DateTime.Today;
			dtpHastaReporte.Value = DateTime.Today;

			if (cboEstadioReporte.Items.Count > 0)
				cboEstadioReporte.SelectedIndex = 0; // "Todos"

			if (cboPartidoReporte.Items.Count > 0)
				cboPartidoReporte.SelectedIndex = 0; // "Todos"

			dgvReporteVentas.Rows.Clear();

			lblBoletosVendidosReporte.Text = "BOLETOS VENDIDOS: 0";
			lblIngresosReporte.Text = "INGRESOS: $0.00";
		}

		private void label36_Click(object sender, EventArgs e)
		{

		}
	}

}
