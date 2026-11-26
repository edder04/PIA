using Microsoft.EntityFrameworkCore;
using PIA.Datos;
using PIA.Diseños;
using PIA.Repositorios;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PIA
{
	public partial class Ventas : Form
	{
		private const int AsientosPorFila = 15;
		private const int MaximoAsientos = 4;
		private const decimal TasaIva = 0.16m;

		private vw_Clientes? _clienteParaVenta;
		private vw_Partidos? _partidoParaVenta;
		private vw_DisponibilidadZonas? _zonaParaVenta;
		private readonly HashSet<string> _asientosOcupados = new();
		private readonly HashSet<string> _asientosSeleccionados = new();
		private bool _cargandoClientes;
		private bool _cargandoFiltrosReporte;
		private bool _cargandoFiltrosOcupacion;
		private bool _filtrosReporteInicializados;
		private bool _ventaRegistrada;
		private int _versionCargaAsientos;
		private Label? _lblCargandoAsientos;
		private bool _cargandoAsientos;
		private Cursor? _cursorAnteriorCargaAsientos;
	private readonly RadioButton _radioBtnDebitoVentaPaso4 = new();
		private int? _idClienteEditando;
		private HistorialClienteReporte? _historialClienteActual;

		// Método: Ventas. Inicializa controles y enlaza los eventos de la ventana.
		public Ventas()
		{
			InitializeComponent();

			pnlMenu.Visible = false;
			GlassEffect.Apply(this);
			RedondearBoton.Aplicar(btnAjustes, 20);
			RedondearBoton.Aplicar(btnHoyInPartidos, 20);
			RedondearBoton.Aplicar(btnProximosInPartidos, 20);
			RedondearBoton.Aplicar(btnTodosInPartidos, 20);
			RedondearBoton.Aplicar(btnEstaSemanaInPartidos, 20);

			tabCtrlVentas.Show();
			BarraTitulo.Agregar(this, "NEXO TICKET  |  Venta de boletos");
			FormBorderStyle = FormBorderStyle.None;
			StartPosition = FormStartPosition.CenterScreen;

			ConfigurarGridClientes();
			ConfigurarGridHistorialCliente();
			gridReporteInOper.Columns.Insert(0, new DataGridViewTextBoxColumn
			{
				Name = "IDVENTA",
				HeaderText = "ID VENTA",
				ReadOnly = true
			});
			gridReporteInOper.Columns.Add("FECHAPARTIDO", "FECHA DEL PARTIDO");
			gridReporteInOper.Columns.Add("METODOPAGO", "MÉTODO DE PAGO");
			btnHistorial.Click += btnHistorial_Click;
			btnExportarInHistorialCliente.Click += btnExportarHistorialCliente_Click;
			btnRegresarInHistorialCliente.Click += btnRegresarHistorialCliente_Click;
			ConfigurarGridAsientos();
			ConfigurarEstiloGrids(this);
			ConfigurarGridsOcupacion();
			ConfigurarTarjeta();

			txtNombreCliente.TextChanged += txtNombreCliente_TextChanged;
			txtBuscarClienteInOper.TextChanged += txtBuscarClienteInOper_TextChanged;
			btnAgregarNuevoClienteInOper.Click += btnAgregarNuevoClienteInOper_Click;
			btnGuardarCambiosEditarClienteInOper.Click += btnGuardarCambiosEditarClienteInOper_Click;
			cboBoxEditarClienteInOper.SelectedIndexChanged += cboBoxEditarClienteInOper_SelectedIndexChanged;
			cboZonaVentaPaso3.SelectedIndexChanged += cboZonaVentaPaso3_SelectedIndexChanged;
			cboEstadioReporteInOper.SelectedIndexChanged += cboEstadioReporteInOper_SelectedIndexChanged;
			btnFiltrarReporteInOper.Click += btnFiltrarReporteInOper_Click;
			btnLimparGridReporteInOper.Click += btnLimparGridReporteInOper_Click;
			btnExportarReporteVentaInOper.Click += btnExportarReporteVentaInOper_Click;
			cboEstadioODInOper.SelectedIndexChanged += cboEstadioODInOper_SelectedIndexChanged;
			cboPartidoODInOper.SelectedIndexChanged += cboPartidoODInOper_SelectedIndexChanged;
			dgvDisponibilidadInOcupacion.CellClick += dgvDisponibilidadODInOper_CellClick;
			btnReporteVentaInOper.Click += btnReporteVentaInOper_Click;
			btnOcupacionDisponibilidadInOper.Click += btnOcupacionDisponibilidadInOper_Click;
			radioBtnEfectivoVentaPaso4.CheckedChanged += radioBtnMetodoPago_CheckedChanged;
			radioBtnTarjetaVentaPaso4.CheckedChanged += radioBtnMetodoPago_CheckedChanged;
			_radioBtnDebitoVentaPaso4.AutoSize = true;
			_radioBtnDebitoVentaPaso4.Font = radioBtnTarjetaVentaPaso4.Font;
			_radioBtnDebitoVentaPaso4.ForeColor = Color.White;
			_radioBtnDebitoVentaPaso4.Location = new Point(680, 300);
			_radioBtnDebitoVentaPaso4.Name = "radioBtnDebitoVentaPaso4";
			_radioBtnDebitoVentaPaso4.Text = "DÉBITO";
			_radioBtnDebitoVentaPaso4.CheckedChanged += radioBtnMetodoPago_CheckedChanged;
			tabPage4.Controls.Add(_radioBtnDebitoVentaPaso4);
			ActualizarVisibilidadCamposEdicion();
			ActualizarCamposPago();
		}


		//==================================================================
		// Carga los catálogos y prepara la pantalla para iniciar una venta.
		//==================================================================
		private async void Ventas_Load(object sender, EventArgs e)
		{
			string nombreUsuario = string.IsNullOrWhiteSpace(Sesion.NombreUsuario)
				? "USUARIO"
				: Sesion.NombreUsuario.Trim().ToUpper(new CultureInfo("es-MX"));
			lblNombreUsuario.Text = nombreUsuario;
			lblBienvenidaInicio.Text = $"BUENOS DÍAS, {nombreUsuario}";
			await CargarClientesAsync();
			await CargarGridClientesOperAsync();
			await CargarPartidosAsync();
			await CargarFiltrosReporteAsync();
			await CargarFiltrosOcupacionAsync();
			await CargarResumenInicioAsync();
		}

		//======================================================================
		// Define columnas y opciones de visualización para el grid de clientes.
		//======================================================================
		private void ConfigurarGridClientes()
		{
			gridClientesVentasPaso1.ReadOnly = true;
			gridClientesVentasPaso1.AllowUserToAddRows = false;
			gridClientesVentasPaso1.MultiSelect = false;
			gridClientesVentasPaso1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gridClientesVentasPaso1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

			GridClientesInOper.ReadOnly = true;
			GridClientesInOper.AllowUserToAddRows = false;
			GridClientesInOper.MultiSelect = false;
			GridClientesInOper.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		}

		//=====================================================
		// Aplica el estilo visual común a los grids de ventas.
		//=====================================================
		private static void ConfigurarEstiloGrids(Control padre)
		{
			var fondo = System.Drawing.Color.FromArgb(60, 60, 60);
			foreach (Control control in padre.Controls)
			{
				if (control is DataGridView grid)
				{
					grid.AllowUserToAddRows = false;
					grid.AllowUserToDeleteRows = false;
					grid.BackgroundColor = fondo;
					grid.GridColor = System.Drawing.Color.FromArgb(90, 90, 90);
					grid.EnableHeadersVisualStyles = false;
					grid.DefaultCellStyle.BackColor = fondo;
					grid.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
					grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(0, 90, 160);
					grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
					grid.RowsDefaultCellStyle.BackColor = fondo;
					grid.RowsDefaultCellStyle.ForeColor = System.Drawing.Color.White;
					grid.ColumnHeadersDefaultCellStyle.BackColor = fondo;
					grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
					grid.RowHeadersDefaultCellStyle.BackColor = fondo;
					grid.RowHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
					grid.AlternatingRowsDefaultCellStyle.BackColor = fondo;
					grid.AlternatingRowsDefaultCellStyle.ForeColor = System.Drawing.Color.White;
				}

				if (control.HasChildren)
					ConfigurarEstiloGrids(control);
			}
		}

		//======================================================================
		// Configura el grid virtual y su presentación para el mapa de asientos.
		//======================================================================
		private void ConfigurarGridAsientos()
		{
			dgvAsientosInOper.AllowUserToAddRows = false;
			dgvAsientosInOper.AllowUserToDeleteRows = false;
			dgvAsientosInOper.AllowUserToResizeRows = false;
			dgvAsientosInOper.ReadOnly = true;
			dgvAsientosInOper.MultiSelect = false;
			dgvAsientosInOper.VirtualMode = true;
			dgvAsientosInOper.RowHeadersVisible = true;
			dgvAsientosInOper.RowHeadersWidth = 70;
			dgvAsientosInOper.RowHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
			dgvAsientosInOper.RowHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
			dgvAsientosInOper.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dgvAsientosInOper.SelectionMode = DataGridViewSelectionMode.CellSelect;
			dgvAsientosInOper.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dgvAsientosInOper.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dgvAsientosInOper.CellClick -= gridAsientos_CellClick;
			dgvAsientosInOper.CellClick += gridAsientos_CellClick;
			dgvAsientosInOper.CellValueNeeded -= dgvAsientosInOper_CellValueNeeded;
			dgvAsientosInOper.CellValueNeeded += dgvAsientosInOper_CellValueNeeded;
			dgvAsientosInOper.CellFormatting -= dgvAsientosInOper_CellFormatting;
			dgvAsientosInOper.CellFormatting += dgvAsientosInOper_CellFormatting;
			dgvAsientosInOper.RowPostPaint -= dgvAsientosInOper_RowPostPaint;
			dgvAsientosInOper.RowPostPaint += dgvAsientosInOper_RowPostPaint;
		}

		//=====================================================
		// Carga los datos del partido en la tarjeta de inicio.
		//=====================================================
		private void ConfigurarTarjeta()
		{
			radioBtnEfectivoVentaPaso4.Checked = true;
		}
		//=================================================================
		// Consulta y muestra los datos resumidos de la pantalla de inicio.
		//=================================================================
		private async Task CargarResumenInicioAsync()
		{
			using var db = new AppDbContext();
			DateTime hoy = DateTime.Today;

			var ventasHoy = (await RepoConsulta.VentasAsync(db))
				.Where(v => v.FechaHoraVenta >= hoy &&
							v.FechaHoraVenta < hoy.AddDays(1) &&
							v.EstadoVenta == "Activa")
				.ToList();

			lblInfoVentasDelDiaInicio.Text = ventasHoy.Count.ToString();
			lblInfoIngresosInicio.Text = ventasHoy.Sum(v => v.Total).ToString("C2");
			lblInfoBoletosVendidosInicio.Text =
				ventasHoy.Sum(v => v.TotalBoletos - v.BoletosCancelados).ToString();
		}

		//=======================================================
		// Consulta los clientes y enlaza los resultados al grid.
		//=======================================================
		private async Task CargarClientesAsync()
		{
			string texto = txtNombreCliente.Text.Trim();
			if (texto.Equals("NOMBRE CLIENTE", StringComparison.OrdinalIgnoreCase))
				texto = string.Empty;

			_cargandoClientes = true;
			try
			{
				using var db = new AppDbContext();
				var consulta = (await RepoConsulta.ClientesAsync(db)).Where(c => c.Activo);

				if (!string.IsNullOrWhiteSpace(texto))
				{
					consulta = consulta.Where(c =>
						c.Nombres.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
						c.ApPaterno.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
						(c.ApMaterno != null && c.ApMaterno.Contains(texto, StringComparison.OrdinalIgnoreCase)) ||
						c.RFC.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
						c.CorreoElectronico.Contains(texto, StringComparison.OrdinalIgnoreCase));
				}

				var clientes = consulta.OrderBy(c => c.ApPaterno).ThenBy(c => c.Nombres).ToList();

				gridClientesVentasPaso1.Rows.Clear();

				foreach (var cliente in clientes)
				{
					int index = gridClientesVentasPaso1.Rows.Add(
						$"{cliente.Nombres} {cliente.ApPaterno} {cliente.ApMaterno}".Trim(),
						cliente.RFC,
						cliente.TelefonoCelular);

					gridClientesVentasPaso1.Rows[index].Tag = cliente;
				}

				gridClientesVentasPaso1.ClearSelection(
);
			}
			finally
			{
				_cargandoClientes = false;
			}
		}

		//======================================================
		// Valida en tiempo real los datos del nombre capturado.
		//======================================================
		private async void txtNombreCliente_TextChanged(object? sender, EventArgs e)
		{
			if (!_cargandoClientes)
				await CargarClientesAsync();
		}

		//===================================================================
		// Filtra los clientes del grid conforme cambia el texto de búsqueda.
		//===================================================================
		private async void txtBuscarClienteInOper_TextChanged(object? sender, EventArgs e)
		{
			await CargarGridClientesOperAsync();
		}

		//==========================================================================
		// Consulta y muestra los clientes que pueden usarse en una venta operativa.
		//==========================================================================
		private async Task CargarGridClientesOperAsync()
		{
			string texto = txtBuscarClienteInOper.Text.Trim();
			using var db = new AppDbContext();
			var consulta = (await RepoConsulta.ClientesAsync(db)).Where(c => c.Activo);

			if (!string.IsNullOrWhiteSpace(texto))
			{
				consulta = consulta.Where(c =>
					c.Nombres.Contains(texto, StringComparison.OrdinalIgnoreCase) || c.ApPaterno.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
					(c.ApMaterno != null && c.ApMaterno.Contains(texto, StringComparison.OrdinalIgnoreCase)) ||
					c.RFC.Contains(texto, StringComparison.OrdinalIgnoreCase) || c.CorreoElectronico.Contains(texto, StringComparison.OrdinalIgnoreCase));
			}

			var clientes = consulta.OrderBy(c => c.ApPaterno).ThenBy(c => c.Nombres).ToList();

			GridClientesInOper.Rows.Clear();
			foreach (var cliente in clientes)
			{
				int fila = GridClientesInOper.Rows.Add(
					$"{cliente.Nombres} {cliente.ApPaterno} {cliente.ApMaterno}".Trim(),
					cliente.RFC, cliente.TelefonoCelular,
					cliente.CorreoElectronico, cliente.Activo ? "Activo" : "Inactivo");
				GridClientesInOper.Rows[fila].Tag = cliente;
			}
			GridClientesInOper.ClearSelection();
		}

		//================================================================
		// Valida y registra un cliente desde el flujo de venta operativa.
		//================================================================
		private async void btnAgregarNuevoClienteInOper_Click(object? sender, EventArgs e)
		{
			string nombres = txtNombreNuevoClienteInOper.Text.Trim();
			string paterno = txtApPaternoNuevoClienteInOper.Text.Trim();
			string materno = txtApMaternoNuevoClienteInOper.Text.Trim();
			string rfc = txtRFCNuevoClienteInOper.Text.Trim().ToUpperInvariant();
			string correo = txtCorreoNuevoClienteInOper.Text.Trim();
			string celular = txtTelefonoCelularNuevoClienteInOper.Text.Trim();
			bool Error(Control control, string mensaje)
			{
				MessageBox.Show(mensaje, "Datos del cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				control.Focus();
				return true;
			}
			if (string.IsNullOrWhiteSpace(nombres)) { Error(txtNombreNuevoClienteInOper, "Nombre(s): captura este dato."); return; }
			if (nombres.Length > 150) { Error(txtNombreNuevoClienteInOper, "Nombre(s): no puede exceder 150 caracteres."); return; }
			if (string.IsNullOrWhiteSpace(paterno)) { Error(txtApPaternoNuevoClienteInOper, "Apellido paterno: captura este dato."); return; }
			if (paterno.Length > 100) { Error(txtApPaternoNuevoClienteInOper, "Apellido paterno: no puede exceder 100 caracteres."); return; }
			if (materno.Length > 100) { Error(txtApMaternoNuevoClienteInOper, "Apellido materno: no puede exceder 100 caracteres."); return; }
			if (!DateTime.TryParse(txtFechaNacimientoNuevoClienteInOper.Text.Trim(),
				CultureInfo.GetCultureInfo("es-MX"), DateTimeStyles.None, out DateTime nacimiento))
			{ Error(txtFechaNacimientoNuevoClienteInOper, "Fecha de nacimiento: escribe una fecha válida con formato dd/MM/aaaa."); return; }
			if (nacimiento.Date >= DateTime.Today)
			{ Error(txtFechaNacimientoNuevoClienteInOper, "Fecha de nacimiento: debe ser anterior al día de hoy."); return; }
			if (!PoliticaCliente.EsRFCValido(rfc)) { Error(txtRFCNuevoClienteInOper, "RFC: captura un RFC válido de persona física o moral."); return; }
			if (!PoliticaCliente.EsCorreoValido(correo) || correo.Length > 100)
			{ Error(txtCorreoNuevoClienteInOper, "Correo electrónico: captura un correo válido de hasta 100 caracteres."); return; }
			if (!PoliticaCliente.EsTelefonoValido(celular))
			{ Error(txtTelefonoCelularNuevoClienteInOper, "Celular: captura de 7 a 15 dígitos; puedes usar +, espacios, paréntesis o guiones."); return; }
			string telefonoCasa = txtTelefonoCasaNuevoClienteInOper.Text.Trim();
			if (telefonoCasa.Length > 0 && !PoliticaCliente.EsTelefonoValido(telefonoCasa))
			{ Error(txtTelefonoCasaNuevoClienteInOper, "Teléfono de casa: usa de 7 a 15 dígitos o deja el campo vacío."); return; }
			var requeridos = new (Control Control, string Etiqueta)[]
			{
				(txtCalleNuevoClienteInOper, "Calle"), (txtNoExteriorNuevoClienteInOper, "Número exterior"),
				(txtCPNuevoClienteInOper, "Código postal"), (txtColoniaNuevoClienteInOper, "Colonia"),
				(txtCiudadNuevoClienteInOper, "Ciudad"), (txtEstadoNuevoClienteInOper, "Estado"),
				(txtPaisNuevoClienteInOper, "País")
			};
			foreach (var (control, etiqueta) in requeridos)
				if (string.IsNullOrWhiteSpace(control.Text)) { Error(control, $"{etiqueta}: captura este dato del domicilio."); return; }

			string interior = txtNoInteriorNuevoClienteInOper.Text.Trim();
			string domicilio = $"{txtCalleNuevoClienteInOper.Text.Trim()} {txtNoExteriorNuevoClienteInOper.Text.Trim()}" +
				(interior.Length > 0 ? $" Int. {interior}" : "") +
				$", C.P. {txtCPNuevoClienteInOper.Text.Trim()}, {txtColoniaNuevoClienteInOper.Text.Trim()}, " +
				$"{txtCiudadNuevoClienteInOper.Text.Trim()}, {txtEstadoNuevoClienteInOper.Text.Trim()}, {txtPaisNuevoClienteInOper.Text.Trim()}";
			if (domicilio.Length > 250)
			{ Error(txtPaisNuevoClienteInOper, "Domicilio: la dirección completa supera 250 caracteres; acorta alguno de sus campos."); return; }

			try
			{
				var cliente = new Cliente
				{
					Nombres = nombres,
					ApPaterno = paterno,
					ApMaterno = string.IsNullOrWhiteSpace(materno) ? null : materno,
					FechaNacimiento = nacimiento.Date,
					RFC = rfc,
					CorreoElectronico = correo,
					TelefonoCasa = string.IsNullOrWhiteSpace(telefonoCasa) ? null : telefonoCasa,
					TelefonoCelular = celular,
					Domicilio = domicilio,
					CreadoPor = Sesion.IdUsuario,
					Activo = true
				};

				await new RepoCliente().AgregarAsync(cliente);
				MessageBox.Show("Cliente agregado correctamente.", "Éxito",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				LimpiarFormularioNuevoClienteOper();
				tabClientesInVentas.SelectedIndex = 0;
				await CargarGridClientesOperAsync();
				await CargarClientesAsync();
			}
			catch (Exception ex)
			{
				string detalle = ex.GetBaseException().Message;
				if (detalle.Contains("duplicate", StringComparison.OrdinalIgnoreCase) || detalle.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
					detalle = "RFC o correo electrónico: ya existe un cliente registrado con ese dato.";
				MessageBox.Show("Cliente: no se pudo guardar.\n" + detalle,
					"Datos del cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//=============================================================
		// Limpia los campos del formulario de nuevo cliente en ventas.
		//=============================================================
		private void LimpiarFormularioNuevoClienteOper()
		{
			txtNombreNuevoClienteInOper.Clear(); txtApPaternoNuevoClienteInOper.Clear();
			txtApMaternoNuevoClienteInOper.Clear(); txtFechaNacimientoNuevoClienteInOper.Clear();
			txtRFCNuevoClienteInOper.Clear(); txtCorreoNuevoClienteInOper.Clear();
			txtTelefonoCasaNuevoClienteInOper.Clear(); txtTelefonoCelularNuevoClienteInOper.Clear();
			txtCalleNuevoClienteInOper.Clear(); txtNoExteriorNuevoClienteInOper.Clear();
			txtNoInteriorNuevoClienteInOper.Clear(); txtCPNuevoClienteInOper.Clear();
			txtColoniaNuevoClienteInOper.Clear(); txtCiudadNuevoClienteInOper.Clear();
			txtEstadoNuevoClienteInOper.Clear(); txtPaisNuevoClienteInOper.Clear();
		}

		//=======================================================
		// Valida el cliente y avanza a la selección del partido.
		//=======================================================
		private async void btnContinuarVentaPaso1_Click(object sender, EventArgs e)
		{
			if (gridClientesVentasPaso1.CurrentRow?.Tag is not vw_Clientes cliente)
			{
				MessageBox.Show("Selecciona un cliente del grid.");
				return;
			}

			_clienteParaVenta = cliente;
			tabCtrlVenta.SelectedIndex = 1;
			await CargarPartidosAsync();
		}

		//==========================================================
		// Consulta los partidos disponibles para iniciar una venta.
		//==========================================================
		private async Task CargarPartidosAsync()
		{
			using var db = new AppDbContext();
			var partidos = (await RepoConsulta.PartidosAsync(db))
				.Where(p => p.Estado == "Programado" && p.FechaHora >= DateTime.Now)
				.OrderBy(p => p.FechaHora).ToList();

			flowPanVentasPaso2.Controls.Clear();
			flowPanVentasPaso2.AutoScroll = true;
			flowPanelPartidos.Controls.Clear
();
			flowPanelPartidos.AutoScroll = true;

			foreach (var partido in partidos)
			{
				var cardVenta = new UcCardPartido();
				cardVenta.CargarPartido(partido);
				cardVenta.PartidoElegido += PartidoElegidoEnPaso2;
				flowPanVentasPaso2.Controls.Add(cardVenta);


				var cardInicio = new UcCardPartido();
				cardInicio.CargarPartido(partido);
				cardInicio.PartidoElegido += elegido =>
				{
					_partidoParaVenta = elegido;
					tabCtrlVentas.SelectedIndex = 1;
					tabCtrlVenta.SelectedIndex = 0;
				};
				flowPanelPartidos.Controls.Add(cardInicio);
			}
		}

		//==================================================================
		// Guarda el partido seleccionado y actualiza las zonas disponibles.
		//==================================================================
		private void PartidoElegidoEnPaso2(vw_Partidos partido)
		{
			_partidoParaVenta = partido;
			txtEquipoVentaPaso2.Text =
				$"{partido.EquipoLocal} vs. {partido.EquipoVisitante}";
			txtFechaVentaPaso2.Text = partido.FechaHora.ToString("dd/MM/yyyy HH:mm");
			txtEstadioVentaPaso2.Text = partido.Estadio;
		}

		//===================================================
		// Valida el partido y avanza a la selección de zona.
		//===================================================
		private async void btnContinuarVentaPaso2_Click(object sender, EventArgs e)
		{
			if (_partidoParaVenta is null)
			{
				MessageBox.Show("Selecciona un partido.");
				return;
			}

			try
			{
				using var db = new AppDbContext();

				lblInfoPartidoVentaPaso3.Text =
					$"{_partidoParaVenta.EquipoLocal} vs. {_partidoParaVenta.EquipoVisitante}";
				lblInfoEstadioVentaPaso3.Text = _partidoParaVenta.Estadio;
				lblFechaVentaPaso3.Text =
					_partidoParaVenta.FechaHora.ToString("dd/MM/yyyy HH:mm");

				var zonas = (await RepoConsulta.DisponibilidadZonasAsync(db))
					.Where(z => z.IdPartido == _partidoParaVenta.IdPartido &&
								z.BoletosDisponibles > 0)
					.OrderBy(z => z.NombreZona)
					.ToList();

				cboZonaVentaPaso3.DataSource = null;
				_zonaParaVenta = null;
				_asientosSeleccionados.Clear();
				_asientosOcupados.Clear();
				dgvAsientosInOper.Rows.Clear();

				cboZonaVentaPaso3.DisplayMember = nameof(vw_DisponibilidadZonas.NombreZona);
				cboZonaVentaPaso3.ValueMember = nameof(vw_DisponibilidadZonas.IdZona);
				cboZonaVentaPaso3.DataSource = zonas;
				ActualizarTotalesVenta();

				if (zonas.Count == 0)
				{
					MessageBox.Show("Este partido no tiene zonas con boletos disponibles.");
					return;
				}

				cboZonaVentaPaso3.SelectedIndex = 0;
				tabCtrlVenta.SelectedIndex = 2;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudieron cargar las zonas",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		//===============================================================
		// Carga la disponibilidad de asientos para la zona seleccionada.
		//===============================================================
		private async void cboZonaVentaPaso3_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (cboZonaVentaPaso3.SelectedItem is not vw_DisponibilidadZonas zona)
				return;

			_zonaParaVenta = zona;
			_asientosSeleccionados.Clear();
			lblInfoPrecioVentaPaso3.Text = zona.PrecioBase.ToString("C2");
			lblInfoDisponiblesVentaPaso3.Text = zona.BoletosDisponibles.ToString();

			await CargarGridAsientosAsync();
		}

		//===============================================================
		// Consulta y dibuja de forma asíncrona los asientos disponibles.
		//===============================================================
		private async Task CargarGridAsientosAsync()
		{
			if (_partidoParaVenta is null || _zonaParaVenta is null)
				return;

			int version = ++_versionCargaAsientos;
			int idPartido = _partidoParaVenta.IdPartido;
			var zona = _zonaParaVenta;
			if (!_cargandoAsientos)
				_cursorAnteriorCargaAsientos = Cursor;
			_cargandoAsientos = true;
			MostrarCargaAsientos(true);
			Cursor = Cursors.WaitCursor;

			try
			{
				// Deja que WinForms pinte el aviso antes de consultar y preparar el mapa.
				await Task.Yield();
				dgvAsientosInOper.Rows.Clear();
				dgvAsientosInOper.Columns.Clear();
				_asientosSeleccionados.Clear();
				_asientosOcupados.Clear();

				using var db = new AppDbContext();
				var ocupados = (await RepoConsulta.AsientosOcupadosAsync(db))
					.Where(a => a.IdPartido == idPartido && a.IdZona == zona.IdZona)
					.Select(a => new { a.Fila, a.Asiento }).ToList();

				if (version != _versionCargaAsientos || _partidoParaVenta?.IdPartido != idPartido || _zonaParaVenta?.IdZona != zona.IdZona)
					return;

				foreach (var asiento in ocupados)
					_asientosOcupados.Add($"{asiento.Fila.Trim()}-{asiento.Asiento.Trim()}");

				for (int n = 1; n <= AsientosPorFila; n++)
					dgvAsientosInOper.Columns.Add($"Asiento{n:00}", n.ToString("00"));

				int filas = (zona.CupoZona + AsientosPorFila - 1) / AsientosPorFila;
				dgvAsientosInOper.RowCount = filas;
				dgvAsientosInOper.Invalidate();
				dgvAsientosInOper.Refresh();
				dgvAsientosInOper.ClearSelection();
				ActualizarTotalesVenta();
			}
			catch (Exception ex)
			{
				if (version == _versionCargaAsientos)
					MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
						"No se pudieron cargar los asientos", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				if (version == _versionCargaAsientos)
				{
					MostrarCargaAsientos(false);
					_cargandoAsientos = false;
					Cursor = _cursorAnteriorCargaAsientos ?? Cursors.Default;
					_cursorAnteriorCargaAsientos = null;
				}
			}
		}

		//=============================================================
		// Muestra u oculta el indicador de carga del grid de asientos.
		//=============================================================
		private void MostrarCargaAsientos(bool visible)
		{
			if (_lblCargandoAsientos is null)
			{
				_lblCargandoAsientos = new Label
				{
					Dock = DockStyle.Fill,
					Text = "Cargando asientos...",
					TextAlign = ContentAlignment.MiddleCenter,
					Font = new Font("Segoe UI", 14, FontStyle.Bold),
					ForeColor = System.Drawing.Color.White,
					BackColor = System.Drawing.Color.FromArgb(230, 30, 30, 30),
					Visible = false
				};
				dgvAsientosInOper.Controls.Add(_lblCargandoAsientos);
			}

			_lblCargandoAsientos.Visible = visible;
			if (visible)
				_lblCargandoAsientos.BringToFront();
		}

		//===================================================================================
		// Proporciona al grid virtual el valor de cada asiento visible en ventas operativas.
		//===================================================================================
		private void dgvAsientosInOper_CellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
		{
			if (_zonaParaVenta is null || e.RowIndex < 0 || e.ColumnIndex < 0 ||
				e.RowIndex * AsientosPorFila + e.ColumnIndex >= _zonaParaVenta.CupoZona)
			{
				e.Value = string.Empty;
				return;
			}

			e.Value = (e.ColumnIndex + 1).ToString("D2");
		}

		//====================================================================
		// Aplica el formato visual correspondiente al estado de cada asiento.
		//====================================================================
		private void dgvAsientosInOper_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
		{
			if (_zonaParaVenta is null || e.RowIndex < 0 || e.ColumnIndex < 0 ||
				e.RowIndex * AsientosPorFila + e.ColumnIndex >= _zonaParaVenta.CupoZona)
				return;

			string clave = $"{e.RowIndex + 1:D3}-{e.ColumnIndex + 1:D2}";
			e.CellStyle.BackColor = _asientosOcupados.Contains(clave)
				? System.Drawing.Color.IndianRed
				: _asientosSeleccionados.Contains(clave)
					? System.Drawing.Color.SeaGreen
					: System.Drawing.Color.FromArgb(60, 60, 60);
			e.CellStyle.ForeColor = System.Drawing.Color.White;
			e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
			e.CellStyle.SelectionForeColor = System.Drawing.Color.White;
		}

		//=====================================================
		// Dibuja los encabezados de fila del grid de asientos.
		//=====================================================
		private void dgvAsientosInOper_RowPostPaint(object? sender, DataGridViewRowPostPaintEventArgs e)
		{
			string encabezado = $"F{e.RowIndex + 1:D3}";
			var bounds = new System.Drawing.Rectangle(e.RowBounds.Left, e.RowBounds.Top,
				dgvAsientosInOper.RowHeadersWidth, e.RowBounds.Height);
			TextRenderer.DrawText(e.Graphics, encabezado, dgvAsientosInOper.Font, bounds,
				System.Drawing.Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
		}

		//=======================================================
		// Selecciona o quita de la selección el asiento pulsado.
		//=======================================================
		private void gridAsientos_CellClick(object? sender, DataGridViewCellEventArgs e)
		{
			if
 (e.RowIndex < 0 || e.ColumnIndex < 0)
				return;

			if (_zonaParaVenta is null || e.RowIndex * AsientosPorFila + e.ColumnIndex >= _zonaParaVenta.CupoZona)
				return;
			string clave = $"{e.RowIndex + 1:D3}-{e.ColumnIndex + 1:D2}";
			if (_asientosOcupados.Contains(clave))
				return;

			if (_asientosSeleccionados.Contains(clave))
			{
				_asientosSeleccionados.Remove(clave);
			}
			else
			{
				if (_asientosSeleccionados.Count >= MaximoAsientos)

				{
					MessageBox.Show($"Solo puedes seleccionar hasta {MaximoAsientos} asientos.");
					return;
				}

				_asientosSeleccionados.Add(clave);
			}

			dgvAsientosInOper.InvalidateCell(e.ColumnIndex, e.RowIndex);
			dgvAsientosInOper.ClearSelection();
			ActualizarTotalesVenta();
		}

		//=====================================================================
		// Recalcula el subtotal, IVA y total según los asientos seleccionados.
		//=====================================================================
		private void ActualizarTotalesVenta()
		{
			decimal subtotal = (_zonaParaVenta?.PrecioBase ?? 0m) *
							   _asientosSeleccionados.Count;
			decimal iva = subtotal * TasaIva;
			decimal total = subtotal + iva;

			lblSeleccionadosVentaPaso3.Text = $"SELECCIONADOS: {_asientosSeleccionados.Count} / {MaximoAsientos}";
			lblInfoSubtotalVentaPaso3.Text = subtotal.ToString("C2");
			lvlInfoIVAVentaPaso3.Text = iva.ToString("C2");
			lblInfoTotalVentaPaso3.Text = total.ToString("C2");
		}

		//=============================================================
		// Muestra el resumen de la compra antes de confirmar la venta.
		//=============================================================
		private bool PrepararPaso4()
		{
			if (_clienteParaVenta is null ||
				_partidoParaVenta is null ||
				_zonaParaVenta is null ||
				_asientosSeleccionados.Count == 0)
			{

				MessageBox.Show("Selecciona cliente, partido, zona y al menos un asiento.");
				return false;
			}

			lblNombreVentaPaso4.Text =
				$"{_clienteParaVenta.Nombres} {_clienteParaVenta.ApPaterno} {_clienteParaVenta.ApMaterno}".Trim();
			lblInfoPartidoVentaPaso4.Text =
				$"{_partidoParaVenta.EquipoLocal} vs. {_partidoParaVenta.EquipoVisitante}";
			lblInfoZonaVentaPaso4.Text = _zonaParaVenta.NombreZona;
			lblInfoBoletosVentaPaso4.Text = _asientosSeleccionados.Count.ToString();
			lblInfoAsientosVentaPaso4.Text =
				string.Join(", ", _asientosSeleccionados.OrderBy(a => a));

			decimal total = _zonaParaVenta.PrecioBase *
							_asientosSeleccionados.Count *
							(1m + TasaIva);
			lblInfoTotalVentaPaso4.Text = total.ToString("C2");
			return true;
		}

		//====================================================
		// Valida los asientos seleccionados y avanza al pago.
		//====================================================
		private void btnContinuarVentaPaso3_Click(object sender, EventArgs e)
		{
			if (PrepararPaso4())
				tabCtrlVenta.SelectedIndex = 3;
		}

		//==========================================================
		// Registra la venta confirmada y genera los boletos en PDF.
		//==========================================================
		private async void btnConfirmarVentaPaso4_Click(object sender, EventArgs e)
		{
			if (_ventaRegistrada)
			{
				MessageBox.Show("Esta venta ya fue registrada.");
				return;
			}

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
			else if (radioBtnTarjetaVentaPaso4.Checked || _radioBtnDebitoVentaPaso4.Checked)
			{
				if (string.IsNullOrWhiteSpace(txtTitularVentaPaso4.Text))
				{
					MessageBox.Show("Escribe el nombre del titular de la tarjeta.");
					txtTitularVentaPaso4.Focus();
					return;
				}

				string numero = Regex.Replace(txtNumeroTarjetaVentaPaso4.Text, @"[\s-]", "");
				string vencimiento = txtVencimientoTarjetaVentaPaso4.Text.Trim();
				string cvv = txtCVVVentaPaso4.Text.Trim();

				if (!Regex.IsMatch(numero, @"^\d+$"))
				{
					MessageBox.Show("El número de tarjeta solo debe contener dígitos.");
					txtNumeroTarjetaVentaPaso4.Focus();
					return;
				}

				string? marca = DetectarMarcaTarjeta(numero);
				if (marca is null || !PasaLuhn(numero))
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

				DateTime finMes = new DateTime(
						fechaVencimiento.Year,
						fechaVencimiento.Month,
						1)
					.AddMonths(1)
					.AddTicks(-1);

				if (finMes < DateTime.Today)
				{
					MessageBox.Show("La tarjeta está vencida.");
					txtVencimientoTarjetaVentaPaso4.Focus();
					return;
				}

				int cvvEsperado = marca == "American Express" ? 4 : 3;
				if (!Regex.IsMatch(cvv, $@"^\d{{{cvvEsperado}}}$"))
				{
					MessageBox.Show($"El CVV de {marca} debe tener {cvvEsperado} dígitos.");
					txtCVVVentaPaso4.Focus();
					return;
				}

				// El cobro es simulado: solo guardamos el tipo, nunca el PAN ni el CVV.
				metodoPago = _radioBtnDebitoVentaPaso4.Checked ? "Débito" : "Crédito";
			}
			else
			{
				MessageBox.Show("Selecciona efectivo o tarjeta.");
				return;
			}

			decimal precio = _zonaParaVenta.PrecioBase;
			int cantidad = _asientosSeleccionados.Count;
			decimal subtotal = precio * cantidad;
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
				PrecioUnitario = precio,
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
					MessageBox.Show($"El asiento '{clave}' tiene un formato inválido.");
					return;
				}

				boletos.Add(new Boleto
				{
					IdPartido = _partidoParaVenta.IdPartido,
					IdZona = _zonaParaVenta.IdZona,
					Fila = partes[0],
					Asiento = partes[1],
					PrecioUnitario = precio,
					EstadoBoleto = "Vendido"
				});
			}

			btnConfirmarVentaPaso4.Enabled = false;
			try
			{
				int idVenta = await new RepoVenta()
					.RegistrarVentaAsync(venta, detalle, boletos);

				_ventaRegistrada = true;
				lblMensajeVentaRealizada.Text = "LA VENTA SE REGISTRÓ CORRECTAMENTE";

				tabCtrlVenta.SelectedIndex = 4;
				await CargarResumenInicioAsync();
				ExportarBoletoVentaPdf(venta, subtotal, iva, total, metodoPago);
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
				txtTitularVentaPaso4.Clear();
				txtNumeroTarjetaVentaPaso4.Clear();
				txtVencimientoTarjetaVentaPaso4.Clear();
				txtCVVVentaPaso4.Clear();
			}
		}

		//=================================================================
		// Actualiza los campos de captura según el método de pago elegido.
		//=================================================================
		private void radioBtnMetodoPago_CheckedChanged(object? sender, EventArgs e)
		{
			ActualizarCamposPago();
		}

		//====================================================================
		// Muestra los campos que corresponden al método de pago seleccionado.
		//====================================================================
		private void ActualizarCamposPago()
		{
			bool tarjeta = radioBtnTarjetaVentaPaso4.Checked || _radioBtnDebitoVentaPaso4.Checked;

			lblTitularVentaPaso4.Visible = tarjeta;
			txtTitularVentaPaso4.Visible = tarjeta;
			lblNumeroTarjetaVentaPaso4.Visible = tarjeta;
			txtNumeroTarjetaVentaPaso4.Visible = tarjeta;
			lblVencimientoTarjetaVentaPaso4.Visible = tarjeta;
			txtVencimientoTarjetaVentaPaso4.Visible = tarjeta;

			lblCVVVentaPaso4.Visible = tarjeta;
			txtCVVVentaPaso4.Visible = tarjeta;
		}

		//================================================================
		// Determina la marca de tarjeta a partir de sus primeros dígitos.
		//================================================================
		private static string? DetectarMarcaTarjeta(string numero)
		{
			if (numero.StartsWith("4") && numero.Length is 13 or 16 or 19)
				return "Visa";

			if (numero.Length == 16)
			{
				int dos = int.Parse(numero[..2]);
				int cuatro = int.Parse(numero[..4]);
				if (dos is >= 51 and <= 55 || cuatro is >= 2221 and <= 2720)
					return "Mastercard";
			}

			if (numero.Length == 15 &&
				(numero.StartsWith("34") || numero.StartsWith("37")))
				return "American Express";

			return null;
		}

		//=========================================================
		// Comprueba el número de tarjeta con el algoritmo de Luhn.
		//=========================================================
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

		//==========================================
		// Regresa al paso de selección del cliente.
		//==========================================
		private void btnAtrasVentaPaso2_Click(object sender, EventArgs e)
		{
			tabCtrlVenta.SelectedIndex = 0;
		}

		//==========================================
		// Regresa al paso de selección del partido.
		//==========================================
		private void btnAtrasVentaPaso3_Click(object sender, EventArgs e)
		{

			tabCtrlVenta.SelectedIndex = 1;
		}

		//==========================================
		// Regresa al paso de selección de asientos.
		//==========================================
		private void btnAtrasVentaPaso4_Click(object sender, EventArgs e)
		{
			tabCtrlVenta.SelectedIndex = 2;
		}

		//==================================================
		// Restablece el formulario para iniciar otra venta.
		//==================================================
		private async void btnNuevaVentaVentaRealizada_Click(object sender, EventArgs e)
		{
			_clienteParaVenta = null;
			_partidoParaVenta = null;
			_zonaParaVenta = null;
			_asientosOcupados.Clear();
			_asientosSeleccionados.Clear();
			_ventaRegistrada = false;

			txtNombreCliente.Clear();
			gridClientesVentasPaso1.ClearSelection();
			cboZonaVentaPaso3.DataSource = null;
			dgvAsientosInOper.Rows.Clear();

			lblInfoPartidoVentaPaso3.Text = string.Empty;
			lblInfoEstadioVentaPaso3.Text = string.Empty;
			lblFechaVentaPaso3.Text = string.Empty;
			lblInfoPrecioVentaPaso3.Text =
"$0.00";
			lblInfoDisponiblesVentaPaso3.Text = "0";
			ActualizarTotalesVenta()
;

			radioBtnEfectivoVentaPaso4.Checked = true;
			txtTitularVentaPaso4.Clear();
			txtNumeroTarjetaVentaPaso4.Clear();
			txtVencimientoTarjetaVentaPaso4.Clear();
			txtCVVVentaPaso4.Clear();
			btnConfirmarVentaPaso4.Enabled = true;

			tabCtrlVenta.SelectedIndex = 0;
			await CargarClientesAsync();
			await CargarPartidosAsync();
		}

		//==============================================
		// Muestra el panel de inicio de administración.
		//==============================================
		private void btnInicio_Click(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 0;
			pnlMenu.Visible = false;
		}

		//=======================================
		// Abre la sección para registrar ventas.
		//=======================================
		private async void btnVentasVentas_Click_1(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 1;
			pnlMenu.Visible = false;
			await CargarPartidosAsync();
		}

		//====================================================
		// Abre la sección para buscar y administrar clientes.
		//====================================================
		private void btnClientesVentas_Click(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 2;
			pnlMenu.Visible = false;
		}

		//=======================================================
		// Abre la sección para consultar y administrar partidos.
		//=======================================================
		private async void btnPartidosVentas_Click(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 3;
			pnlMenu.Visible = false;
			await CargarPartidosAsync();
		}

		//=============================
		// Abre la sección de reportes.
		//=============================
		private async void btnReportesVentas_Click(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 4;

			pnlMenu.Visible = false;
			await CargarFiltrosReporteAsync();
			await CargarFiltrosOcupacionAsync();
		}

		//========================================================
		// Cierra la sesión del administrador y regresa al acceso.
		//========================================================
		private void btnSalir_Click(object sender, EventArgs e)
		{
			pnlMenu.Visible = false;
			new InicioSesion().Show();
			Close();
		}

		//=======================================
		// Abre la sección de ajustes de usuario.
		//=======================================
		private void btnAjustes_Click(object sender, EventArgs e)
		{
			pnlMenu.Visible = !pnlMenu.Visible;
			if (pnlMenu.Visible)
				pnlMenu.BringToFront();
		}

		//=========================================================
		// Configura la tabla del historial de compras del cliente.
		//=========================================================
		private void ConfigurarGridHistorialCliente()
		{
			dgvHistorialInHistorialCliente.Columns.Clear();
			dgvHistorialInHistorialCliente.Columns.Add("Fecha", "FECHA");
			dgvHistorialInHistorialCliente.Columns.Add("Folio", "FOLIO");
			dgvHistorialInHistorialCliente.Columns.Add("Partidos", "PARTIDO(S)");
			dgvHistorialInHistorialCliente.Columns.Add("Boletos", "BOLETOS");
			dgvHistorialInHistorialCliente.Columns.Add("Total", "TOTAL");
			dgvHistorialInHistorialCliente.Columns.Add("Estado", "ESTADO");
			dgvHistorialInHistorialCliente.Columns.Add("Pago", "MÉTODO DE PAGO");
			dgvHistorialInHistorialCliente.Columns.Add("Factura", "FACTURA");
			dgvHistorialInHistorialCliente.ReadOnly = true;
			dgvHistorialInHistorialCliente.AllowUserToAddRows = false;
			dgvHistorialInHistorialCliente.AllowUserToDeleteRows = false;
			dgvHistorialInHistorialCliente.MultiSelect = false;
			dgvHistorialInHistorialCliente.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgvHistorialInHistorialCliente.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		}

		//============================================
		// Abre el historial del cliente seleccionado.
		//============================================
		private async void btnHistorial_Click(object? sender, EventArgs e)
		{
			if (GridClientesInOper.SelectedRows.Count == 0 ||
				GridClientesInOper.SelectedRows[0].Tag is not vw_Clientes cliente)
			{
				MessageBox.Show("Selecciona un cliente del grid para consultar su historial.",
					"Historial del cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			try
			{
				await MostrarHistorialClienteAsync(cliente.IdCliente);
				tabClientesInVentas.SelectedTab = tabHistorialCliente;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"No se pudo cargar el historial del cliente.\n{ex.Message}",
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//===========================================================
		// Consulta y presenta el historial del cliente seleccionado.
		//===========================================================
		private async Task MostrarHistorialClienteAsync(int idCliente)
		{
			var reporte = await HistorialClienteRepositorio.ObtenerAsync(idCliente);
			if (reporte is null)
				throw new InvalidOperationException("No se encontró el cliente seleccionado.");

			_historialClienteActual = reporte;
			lblNombreInfoInHistorialCliente.Text =
				$"{reporte.Cliente.Nombres} {reporte.Cliente.ApPaterno} {reporte.Cliente.ApMaterno}".Trim();
			lblRFCInHistorialCliente.Text = reporte.Cliente.RFC;
			lblClienteInHistorialCliente.Text = $"CLIENTE #{reporte.Cliente.IdCliente}";
			lblComprasInHistorialCliente.Text = $"COMPRAS: {reporte.Ventas.Count}";
			lblTotalInHistorialCliente.Text = $"TOTAL: {reporte.TotalGastado.ToString("C2", CultureInfo.GetCultureInfo("es-MX"))}";
			lblBoletosInHistorialCliente.Text = $"BOLETOS: {reporte.TotalBoletosValidos}";

			dgvHistorialInHistorialCliente.Rows.Clear();
			foreach (var venta in reporte.Ventas)
			{
				dgvHistorialInHistorialCliente.Rows.Add(
					venta.FechaHoraVenta.ToString("dd/MM/yyyy HH:mm"),
					venta.Folio.ToString(),
					reporte.ObtenerPartidos(venta.IdVenta),
					$"{Math.Max(0, venta.TotalBoletos - venta.BoletosCancelados)} de {venta.TotalBoletos}",
					venta.Total.ToString("C2", CultureInfo.GetCultureInfo("es-MX")),
					venta.EstadoVenta,
					venta.MetodoPago,
					venta.Facturada == 1 ? "Sí" : "No");
			}
		}

		//======================================================
		// Regresa desde el historial a la búsqueda de clientes.
		//======================================================
		private void btnRegresarHistorialCliente_Click(object? sender, EventArgs e)
		{
			tabClientesInVentas.SelectedTab = BuscarCliente;
		}

		//=================================================
		// Exporta a PDF el historial del cliente mostrado.
		//=================================================
		private void btnExportarHistorialCliente_Click(object? sender, EventArgs e)
		{
			if (_historialClienteActual is null)
			{
				MessageBox.Show("Primero abre el historial de un cliente.",
					"Historial del cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			using var dialogo = new SaveFileDialog
			{
				Filter = "Archivo PDF (*.pdf)|*.pdf",
				FileName = $"HistorialCliente_{_historialClienteActual.Cliente.IdCliente}_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
				Title = "Exportar historial del cliente"
			};
			if (dialogo.ShowDialog(this) != DialogResult.OK)
				return;

			try
			{
				GeneradorPdfBasico.Guardar(dialogo.FileName, "HISTORIAL DEL CLIENTE",
					_historialClienteActual.CrearLineasPdf());
				MessageBox.Show("El historial se exportó correctamente.",
					"PDF creado", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"No se pudo crear el PDF.\n{ex.Message}",
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		//===================================================
		// Abre el formulario para capturar un cliente nuevo.
		//===================================================
		private void btnNuevoClienteInCliente_Click(object sender, EventArgs e)
		{
			tabClientesInVentas.SelectedIndex = 1;
		}

		//==========================================
		// Abre la edición del cliente seleccionado.
		//==========================================
		private void btnEditarCliente_Click(object sender, EventArgs e)
		{
			if (GridClientesInOper.SelectedRows.Count == 0 ||
				GridClientesInOper.SelectedRows[0].Tag is not vw_Clientes cliente)
			{
				MessageBox.Show("Selecciona un cliente del grid para editarlo.");
				return;
			}

			_idClienteEditando = cliente.IdCliente;
			lblNombreCompletoEditarClienteInOper.Text =
				$"{cliente.Nombres} {cliente.ApPaterno} {cliente.ApMaterno}".Trim();
			lblFechaNacimientoEditarClienteInOper.Text = cliente.FechaNacimiento.ToString("dd/MM/yyyy");
			txtNuevoDatoEditarClienteInOper.Text = string.Empty;
			cboBoxEditarClienteInOper.SelectedIndex = -1;
			txtCalleEditarClienteInOper.Text = cliente.Domicilio;
			txtNoExteriorEditarClienteInOper.Clear();
			txtNoInteriorEditarClienteInOper.Clear();
			txtCPEditarClienteInOper.Clear();
			txtColoniaEditarClienteInOper.Clear();
			txtCiudadEditarClienteInOper.Clear();
			txtEstadoEditarClienteInOper.Clear();
			txtPaisEditarClienteInOper.Clear();
			ActualizarVisibilidadCamposEdicion();
			tabClientesInVentas.SelectedIndex = 2;
		}

		//=========================================================================
		// Muestra el campo que corresponde al dato de cliente elegido para editar.
		//=========================================================================
		private void cboBoxEditarClienteInOper_SelectedIndexChanged(object? sender, EventArgs e)
		{
			ActualizarVisibilidadCamposEdicion();
		}

		//====================================================================
		// Muestra solo el control necesario para editar el dato seleccionado.
		//====================================================================
		private void ActualizarVisibilidadCamposEdicion()
		{
			bool editarDomicilio = cboBoxEditarClienteInOper.SelectedItem?.ToString() == "DOMICILIO";

			lblNuevoDatoEditarClienteInOper.Visible = !editarDomicilio;
			txtNuevoDatoEditarClienteInOper.Visible = !editarDomicilio;

			Control[] camposDomicilio =
			{
				lblCalleEditarClienteInOper, txtCalleEditarClienteInOper,
				lblNoExteriorEditarClienteInOper, txtNoExteriorEditarClienteInOper,
				lblNoInteriorEditarClienteInOper, txtNoInteriorEditarClienteInOper,
				lblCPEditarClienteInOper, txtCPEditarClienteInOper,
				lblColoniaEditarClienteInOper, txtColoniaEditarClienteInOper,
				lblCiudadEditarClienteInOper, txtCiudadEditarClienteInOper,
				lblEstadoEditarClienteInOper, txtEstadoEditarClienteInOper,
				lblPaisEditarClienteInOper, txtPaisEditarClienteInOper
			};

			foreach (Control control in camposDomicilio)
				control.Visible = editarDomicilio;
		}

		private sealed class ReportePartidoFiltro
		{
			public int? IdPartido { get; init; }
			public string Descripcion { get; init; } = string.Empty;
		}

		//=========================================================
		// Carga los estadios disponibles como filtros del reporte.
		//=========================================================
		private async Task CargarFiltrosReporteAsync()
		{
			string estadioAnterior = cboEstadioReporteInOper.SelectedItem?.ToString() ?? "Todos los estadios";
			int? partidoAnterior = cboPartidoReporteInOper.SelectedValue is int idPartido ? idPartido : null;
			_cargandoFiltrosReporte = true;
			try
			{
				using var db = new AppDbContext();
				var estadios = (await RepoConsulta.EstadiosAsync(db))
					.OrderBy(e => e.Nombre).Select(e => e.Nombre).Distinct().ToList();

				cboEstadioReporteInOper.Items.Clear();
				cboEstadioReporteInOper.Items.Add("Todos los estadios");
				foreach (string estadio in estadios)
					cboEstadioReporteInOper.Items.Add(estadio);
				int indiceEstadio = cboEstadioReporteInOper.Items.IndexOf(estadioAnterior);
				cboEstadioReporteInOper.SelectedIndex = indiceEstadio >= 0 ? indiceEstadio : 0;

				if (!_filtrosReporteInicializados)
				{
					dateTimePicker1.Value = DateTime.Today.AddDays(-30);
					dateTimePicker2.Value = DateTime.Today;
					_filtrosReporteInicializados = true;
				}
			}
			finally
			{
				_cargandoFiltrosReporte = false;
			}

			await CargarPartidosFiltroReporteAsync(cboEstadioReporteInOper.SelectedItem?.ToString() ?? "Todos los estadios", partidoAnterior);
		}

		//==============================================================
		// Actualiza la lista de partidos según el estadio seleccionado.
		//==============================================================
		private async void cboEstadioReporteInOper_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (_cargandoFiltrosReporte)
				return;



			await CargarPartidosFiltroReporteAsync(cboEstadioReporteInOper.SelectedItem?.ToString());
		}

		//==================================================================
		// Consulta y carga la información necesaria para filtros ocupacion.
		//==================================================================
		private async Task CargarFiltrosOcupacionAsync()
		{
			_cargandoFiltrosOcupacion = true;
			try
			{
				using var db = new AppDbContext();
				var estadios = (await RepoConsulta.EstadiosAsync(db))
					.OrderBy(e => e.Nombre)
					.Select(e => new FiltroEstadioOcupacion
					{
						IdEstadio = e.IdEstadio,
						Nombre = e.Nombre
					}).ToList();

				cboEstadioODInOper.DataSource = null;
				cboEstadioODInOper.DisplayMember = nameof(FiltroEstadioOcupacion.Nombre);
				cboEstadioODInOper.ValueMember = nameof(FiltroEstadioOcupacion.IdEstadio);
				cboEstadioODInOper.DataSource = estadios;
				cboEstadioODInOper.SelectedIndex = -1;
				cboPartidoODInOper.DataSource = null;
				dgvDisponibilidadInOcupacion.Rows.Clear();
				dgvAsientosInOcupacion.Rows.Clear();
				dgvAsientosInOcupacion.Columns.Clear();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudieron cargar los estadios", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				_cargandoFiltrosOcupacion = false;
			}
		}

		//================================================================
		// Configura los controles y el comportamiento de grids ocupacion.
		//================================================================
		private void ConfigurarGridsOcupacion()
		{
			dgvDisponibilidadInOcupacion.ReadOnly = true;
			dgvDisponibilidadInOcupacion.AllowUserToAddRows = false;
			dgvDisponibilidadInOcupacion.AllowUserToDeleteRows = false;
			dgvDisponibilidadInOcupacion.MultiSelect = false;
			dgvDisponibilidadInOcupacion.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgvDisponibilidadInOcupacion.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

			dgvAsientosInOcupacion.ReadOnly = true;
			dgvAsientosInOcupacion.AllowUserToAddRows = false;
			dgvAsientosInOcupacion.AllowUserToDeleteRows = false;
			dgvAsientosInOcupacion.MultiSelect = false;
			dgvAsientosInOcupacion.SelectionMode = DataGridViewSelectionMode.CellSelect;
			dgvAsientosInOcupacion.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		}

		//=======================================================================
		// Actualiza los datos dependientes cuando cambia la opción seleccionada.
		//=======================================================================
		private async void cboEstadioODInOper_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (_cargandoFiltrosOcupacion || cboEstadioODInOper.SelectedValue is not int idEstadio)
				return;

			_cargandoFiltrosOcupacion = true;
			try
			{
				using var db = new AppDbContext();
				var partidosDb = (await RepoConsulta.PartidosAsync(db))
					.Where(p => p.IdEstadio == idEstadio)
					.OrderByDescending(p => p.FechaHora)
					.Select(p => new
					{
						p.IdPartido,
						p.EquipoLocal,
						p.EquipoVisitante,
						p.FechaHora,
						p.Estado
					}).ToList();

				var partidos = partidosDb.Select(p => new FiltroPartidoOcupacion
				{
					IdPartido = p.IdPartido,
					Descripcion = $"{p.EquipoLocal} vs. {p.EquipoVisitante} | {p.FechaHora:dd/MM/yyyy HH:mm} | {p.Estado}"
				}).ToList();

				cboPartidoODInOper.DataSource = null;
				cboPartidoODInOper.DisplayMember = nameof(FiltroPartidoOcupacion.Descripcion);
				cboPartidoODInOper.ValueMember = nameof(FiltroPartidoOcupacion.IdPartido);
				cboPartidoODInOper.DataSource = partidos;
				cboPartidoODInOper.SelectedIndex = -1;
				dgvDisponibilidadInOcupacion.Rows.Clear();
				dgvAsientosInOcupacion.Rows.Clear();
				dgvAsientosInOcupacion.Columns.Clear();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudieron cargar los partidos", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				_cargandoFiltrosOcupacion = false;
			}
		}

		//=======================================================================
		// Actualiza los datos dependientes cuando cambia la opción seleccionada.
		//=======================================================================
		private async void cboPartidoODInOper_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (_cargandoFiltrosOcupacion || cboPartidoODInOper.SelectedValue is not int idPartido)
				return;

			try
			{
				using var db = new AppDbContext();
				var zonas = (await RepoConsulta.DisponibilidadZonasAsync(db))
					.Where(z => z.IdPartido == idPartido).OrderBy(z => z.NombreZona).ToList();

				dgvDisponibilidadInOcupacion.Rows.Clear();
				dgvAsientosInOcupacion.Rows.Clear();
				dgvAsientosInOcupacion.Columns.Clear();
				foreach (var zona in zonas)
				{
					decimal porcentaje = zona.CupoZona <= 0
						? 0m
						: zona.BoletosVendidos * 100m / zona.CupoZona;
					int fila = dgvDisponibilidadInOcupacion.Rows.Add(
						zona.NombreZona,
						zona.CupoZona,
						zona.BoletosDisponibles,
						zona.BoletosVendidos,
						$"{porcentaje:N1}%");
					dgvDisponibilidadInOcupacion.Rows[fila].Tag = zona;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudo cargar la disponibilidad", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//=========================================================================
		// Procesa la selección de la celda y actualiza la información relacionada.
		//=========================================================================
		private async void dgvDisponibilidadODInOper_CellClick(object? sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || dgvDisponibilidadInOcupacion.Rows[e.RowIndex].Tag is not vw_DisponibilidadZonas zona)
				return;

			if (cboPartidoODInOper.SelectedValue is not int idPartido || idPartido != zona.IdPartido)
				return;

			dgvAsientosInOcupacion.Rows.Clear();
			dgvAsientosInOcupacion.Columns.Clear();
			const int asientosPorFila = 15;
			for (int n = 1; n <= asientosPorFila; n++)
				dgvAsientosInOcupacion.Columns.Add($"Asiento{n:00}", n.ToString("00"));

			HashSet<string> ocupados;
			try
			{
				using var db = new AppDbContext();
				var ocupadosDb = (await RepoConsulta.AsientosOcupadosAsync(db))
					.Where(a => a.IdPartido == idPartido && a.IdZona == zona.IdZona)
					.Select(a => new { a.Fila, a.Asiento }).ToList();
				ocupados = ocupadosDb
					.Select(a => $"{a.Fila.Trim()}-{a.Asiento.Trim()}")
					.ToHashSet(StringComparer.OrdinalIgnoreCase);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudo consultar la ocupacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			int totalFilas = (zona.CupoZona + asientosPorFila - 1) / asientosPorFila;
			for (int filaIndex = 0; filaIndex < totalFilas; filaIndex++)
			{
				int indexFila = dgvAsientosInOcupacion.Rows.Add();
				DataGridViewRow fila = dgvAsientosInOcupacion.Rows[indexFila];
				fila.HeaderCell.Value = $"F{filaIndex + 1:D3}";

				for (int columna = 0; columna < asientosPorFila; columna++)
				{
					int numeroAbsoluto = filaIndex * asientosPorFila + columna;
					DataGridViewCell celda = fila.Cells[columna];
					if (numeroAbsoluto >= zona.CupoZona)
					{
						celda.Value = string.Empty;
						continue;
					}

					string codigo = $"{filaIndex + 1:D3}-{columna + 1:D2}";
					bool estaOcupado = ocupados.Contains(codigo);
					celda.Value = $"{columna + 1:D2}";
					celda.ToolTipText = codigo + (estaOcupado ? " | Ocupado" : " | Disponible");
					celda.Style.BackColor = estaOcupado
						? System.Drawing.Color.IndianRed
						: System.Drawing.Color.FromArgb(60, 60, 60);
					celda.Style.ForeColor = System.Drawing.Color.White;
					celda.Style.SelectionBackColor = celda.Style.BackColor;
					celda.Style.SelectionForeColor = System.Drawing.Color.White;
				}
			}
			dgvAsientosInOcupacion.ClearSelection();
		}

		//======================================
		// Abre el reporte de ventas operativas.
		//======================================
		private async void btnReporteVentaInOper_Click(object? sender, EventArgs e)
		{
			tabReportes.SelectedIndex = 0;
			await CargarFiltrosReporteAsync();
		}

		//===============================================
		// Abre el reporte de ocupación y disponibilidad.
		//===============================================
		private async void btnOcupacionDisponibilidadInOper_Click(object? sender, EventArgs e)
		{
			tabReportes.SelectedIndex = 1;
			await CargarFiltrosOcupacionAsync();
		}

		private sealed class FiltroEstadioOcupacion
		{
			public int IdEstadio { get; init; }
			public string Nombre { get; init; } = string.Empty;
		}

		private sealed class FiltroPartidoOcupacion
		{
			public int IdPartido { get; init; }
			public string Descripcion { get; init; } = string.Empty;
		}

		//====================================================================
		// Carga partidos que corresponden al estadio elegido para el reporte.
		//====================================================================
		private async Task CargarPartidosFiltroReporteAsync(string? estadio, int? partidoSeleccionado = null)
		{
			_cargandoFiltrosReporte = true;
			try
			{
				using var db = new AppDbContext();
				var consulta = (await RepoConsulta.PartidosAsync(db)).AsEnumerable();
				if (!string.IsNullOrWhiteSpace(estadio) && estadio != "Todos los estadios")
					consulta = consulta.Where(p => p.Estadio == estadio);

				var partidosDb = consulta.OrderByDescending(p => p.FechaHora)
					.Select(p => new
					{
						p.IdPartido,
						p.EquipoLocal,
						p.EquipoVisitante,
						p.FechaHora
					}).ToList();
				var partidos = partidosDb.Select(p => new ReportePartidoFiltro
				{
					IdPartido = p.IdPartido,
					Descripcion = $"{p.EquipoLocal} vs. {p.EquipoVisitante} | {p.FechaHora:dd/MM/yyyy HH:mm}"
				}).ToList();

				partidos.Insert(0, new ReportePartidoFiltro
				{
					IdPartido = null,
					Descripcion = "Todos los partidos"
				});

				cboPartidoReporteInOper.DataSource = null;
				cboPartidoReporteInOper.DisplayMember = nameof(ReportePartidoFiltro.Descripcion);
				cboPartidoReporteInOper.ValueMember = nameof(ReportePartidoFiltro.IdPartido);
				cboPartidoReporteInOper.DataSource = partidos;
				if (partidoSeleccionado.HasValue && partidos.Any(p => p.IdPartido == partidoSeleccionado.Value))
					cboPartidoReporteInOper.SelectedValue = partidoSeleccionado.Value;
				else
					cboPartidoReporteInOper.SelectedIndex = 0;
			}
			finally
			{
				_cargandoFiltrosReporte = false;
			}
		}

		//==========================================================
		// Valida fechas y aplica los filtros del reporte de ventas.
		//==========================================================
		private async void btnFiltrarReporteInOper_Click(object? sender, EventArgs e)
		{
			await FiltrarReporteAsync();
		}

		//=======================================================================
		// Consulta y presenta las ventas que coinciden con los filtros elegidos.
		//=======================================================================
		private async Task FiltrarReporteAsync()
		{
			DateTime desde = dateTimePicker1.Value.Date;
			DateTime hasta = dateTimePicker2.Value.Date;
			if (desde > hasta)
			{
				MessageBox.Show("La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'.");
				return;
			}

			try
			{
				int? idPartido = cboPartidoReporteInOper.SelectedValue is int id ? id : null;
				string? estadio = cboEstadioReporteInOper.SelectedItem?.ToString();
				DateTime hastaExclusivo = hasta.AddDays(1);

				using var db = new AppDbContext();
				var boletosQuery = (await RepoConsulta.BoletosVendidosAsync(db))
					.Where(b => b.FechaHoraVenta >= desde && b.FechaHoraVenta < hastaExclusivo &&
								b.EstadoBoleto == "Vendido" && b.EstadoVenta == "Activa");

				if (!string.IsNullOrWhiteSpace(estadio) && estadio != "Todos los estadios")
					boletosQuery = boletosQuery.Where(b => b.Estadio == estadio);
				if (idPartido.HasValue)
					boletosQuery = boletosQuery.Where(b => b.IdPartido == idPartido.Value);

				var boletos = boletosQuery.ToList();
				var idsVenta = boletos.Select(b => b.IdVenta).Distinct().ToList();
				var ventas = (await RepoConsulta.VentasAsync(db))
					.Where(v => idsVenta.Contains(v.IdVenta) && v.EstadoVenta == "Activa")
					.ToDictionary(v => v.IdVenta);

				gridReporteInOper.Rows.Clear();
				var grupos = boletos.GroupBy(b => new
				{
					b.IdVenta,
					b.FolioVenta,
					b.FechaHoraVenta,
					b.Estadio,
					b.IdPartido,
					b.FechaPartido,
					b.EquipoLocal,
					b.EquipoVisitante,
					b.IdZona,
					b.NombreZona
				}).OrderByDescending(g => g.Key.FechaHoraVenta).ToList();

				foreach (var grupo in grupos)
				{
					if (!ventas.TryGetValue(grupo.Key.IdVenta, out var venta))
						continue;

					int fila = gridReporteInOper.Rows.Add(
						grupo.Key.IdVenta,
						grupo.Key.FolioVenta.ToString(),
						grupo.Key.FechaHoraVenta.ToString("dd/MM/yyyy HH:mm"),
						grupo.Key.Estadio,
						$"{grupo.Key.EquipoLocal} vs. {grupo.Key.EquipoVisitante}",
						grupo.Key.NombreZona,
						grupo.Count(),
						venta.Subtotal.ToString("C2"),
						venta.Total.ToString("C2"),
						grupo.Key.FechaPartido.ToString("dd/MMM/yyyy HH:mm", CultureInfo.GetCultureInfo("es-MX")),
						venta.MetodoPago);
					gridReporteInOper.Rows[fila].Tag = grupo.Key.IdVenta;
				}

				int boletosVendidos = boletos.Count;
				decimal ingresos = idsVenta.Where(ventas.ContainsKey).Sum(id => ventas[id].Total);
				lblBoletosVendidosReportesInOper.Text = $"BOLETOS VENDIDOS: {boletosVendidos}";
				lblIngresosReportesInOper.Text = $"INGRESOS: {ingresos:C2}";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudo filtrar el reporte", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//=======================================================
		// Limpia los filtros y resultados del reporte de ventas.
		//=======================================================
		private async void btnLimparGridReporteInOper_Click(object? sender, EventArgs e)
		{
			_cargandoFiltrosReporte = true;
			dateTimePicker1.Value = dateTimePicker1.MinDate;
			dateTimePicker2.Value = DateTime.Today;
			cboEstadioReporteInOper.SelectedIndex = 0;
			_cargandoFiltrosReporte = false;
			await CargarPartidosFiltroReporteAsync("Todos los estadios");
			await FiltrarReporteAsync();
		}

		//======================================================
		// Valida y guarda los cambios del cliente seleccionado.
		//======================================================
		private async void btnGuardarCambiosEditarClienteInOper_Click(object? sender, EventArgs e)
		{
			if (_idClienteEditando is null)
			{
				MessageBox.Show("Cliente: selecciona primero un registro del grid.", "Edición de cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				GridClientesInOper.Focus();
				return;
			}
			if (cboBoxEditarClienteInOper.SelectedItem is not string campo)
			{
				MessageBox.Show("Dato a editar: selecciona qué información del cliente quieres cambiar.", "Edición de cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				cboBoxEditarClienteInOper.Focus();
				return;
			}

			string valor;
			if (campo == "DOMICILIO")
			{
				string calle = txtCalleEditarClienteInOper.Text.Trim();
				string exterior = txtNoExteriorEditarClienteInOper.Text.Trim();
				string interior = txtNoInteriorEditarClienteInOper.Text.Trim();
				bool direccionEstructurada = exterior.Length > 0 ||
					txtCPEditarClienteInOper.Text.Trim().Length > 0 ||
					txtColoniaEditarClienteInOper.Text.Trim().Length > 0 ||
					txtCiudadEditarClienteInOper.Text.Trim().Length > 0 ||
					txtEstadoEditarClienteInOper.Text.Trim().Length > 0 ||
					txtPaisEditarClienteInOper.Text.Trim().Length > 0;

				if (direccionEstructurada)
				{
					var requeridos = new (Control Control, string Etiqueta)[]
					{
						(txtCalleEditarClienteInOper, "Calle"), (txtNoExteriorEditarClienteInOper, "Número exterior"),
						(txtCPEditarClienteInOper, "Código postal"), (txtColoniaEditarClienteInOper, "Colonia"),
						(txtCiudadEditarClienteInOper, "Ciudad"), (txtEstadoEditarClienteInOper, "Estado"),
						(txtPaisEditarClienteInOper, "País")
					};
					foreach (var (control, etiqueta) in requeridos)
						if (string.IsNullOrWhiteSpace(control.Text))
						{
							MessageBox.Show($"{etiqueta}: captura este dato del domicilio.", "Edición de cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							control.Focus();
							return;
						}
					valor = $"{calle} {exterior}" + (interior.Length > 0 ? $" Int. {interior}" : "") +
						$", C.P. {txtCPEditarClienteInOper.Text.Trim()}, {txtColoniaEditarClienteInOper.Text.Trim()}, " +
						$"{txtCiudadEditarClienteInOper.Text.Trim()}, {txtEstadoEditarClienteInOper.Text.Trim()}, {txtPaisEditarClienteInOper.Text.Trim()}";
					if (valor.Length > 250)
					{
						MessageBox.Show("Domicilio: la dirección completa no puede exceder 250 caracteres.", "Dato inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						txtPaisEditarClienteInOper.Focus();
						return;
					}
				}
				else
				{
					valor = calle;
				}
			}
			else
			{
				valor = txtNuevoDatoEditarClienteInOper.Text.Trim();
				if (string.IsNullOrWhiteSpace(valor))
				{
					MessageBox.Show($"{campo}: escribe el nuevo valor.", "Edición de cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					txtNuevoDatoEditarClienteInOper.Focus();
					return;
				}
			}

			if (campo == "CORREO ELECTRONICO" && (valor.Length > 100 || !PoliticaCliente.EsCorreoValido(valor)))
			{
				MessageBox.Show("Correo electrónico: escribe una dirección válida de hasta 100 caracteres.", "Dato inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				txtNuevoDatoEditarClienteInOper.Focus();
				return;
			}
			if (campo.StartsWith("TELEFONO", StringComparison.Ordinal) &&
				!PoliticaCliente.EsTelefonoValido(valor))
			{
				MessageBox.Show($"{campo}: captura de 7 a 15 dígitos; puedes usar +, espacios, paréntesis o guiones.", "Dato inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				txtNuevoDatoEditarClienteInOper.Focus();
				return;
			}

			btnGuardarCambiosEditarClienteInOper.Enabled = false;
			try
			{
				await new RepoCliente().ActualizarClienteAsync(_idClienteEditando.Value, campo, valor);
				MessageBox.Show("Cambios guardados correctamente.", "Éxito",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				_idClienteEditando = null;
				tabClientesInVentas.SelectedIndex = 0;
				await CargarGridClientesOperAsync();
				await CargarClientesAsync();
			}
			catch (Exception ex)
			{
				string detalle = ex.GetBaseException().Message;
				if (detalle.Contains("duplicate", StringComparison.OrdinalIgnoreCase) || detalle.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
					detalle = $"{campo}: ese valor ya está registrado en otro cliente.";
				MessageBox.Show($"{campo}: no se pudo actualizar.\n{detalle}",
					"Edición de cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
				if (campo == "DOMICILIO") txtPaisEditarClienteInOper.Focus();
				else txtNuevoDatoEditarClienteInOper.Focus();
			}
			finally
			{
				btnGuardarCambiosEditarClienteInOper.Enabled = true;
			}
		}

		//=================================================
		// Abre la pestaña para registrar un cliente nuevo.
		//=================================================
		private void button2_Click(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 2;
			tabClientesInVentas.SelectedIndex = 1;
		}

		//===============================================
		// Exporta el reporte de ventas operativas a PDF.
		//===============================================
		private void btnExportarReporteVentaInOper_Click(object? sender, EventArgs e)
		{
			ExportarReporteVentasPdf();
		}

		//===================================================================================
		// Función: abre el diálogo para guardar el boleto PDF de la venta recién registrada.
		//===================================================================================
		private void ExportarBoletoVentaPdf(Venta venta, decimal subtotal, decimal iva, decimal total, string metodoPago)
		{
			using var dialogo = new SaveFileDialog
			{
				Title = "Guardar boleto de la venta",
				Filter = "Archivo PDF (*.pdf)|*.pdf",
				FileName = $"Boleto_{venta.Folio:N}.pdf",
				AddExtension = true,
				DefaultExt = "pdf",
				OverwritePrompt = true
			};

			if (dialogo.ShowDialog(this) != DialogResult.OK)
				return;

			var lineas = new List<string>
			{
				"NEXO TICKET | BOLETO DE ACCESO",
				$"Folio de venta: {venta.Folio}",
				$"Fecha y hora: {venta.FechaHoraVenta:dd/MM/yyyy HH:mm}",
				$"Cliente: {_clienteParaVenta?.Nombres} {_clienteParaVenta?.ApPaterno} {_clienteParaVenta?.ApMaterno}".Trim(),
				$"Partido: {_partidoParaVenta?.EquipoLocal} vs. {_partidoParaVenta?.EquipoVisitante}",
				$"Fecha del partido: {_partidoParaVenta?.FechaHora:dd/MM/yyyy HH:mm}",
				$"Estadio: {_partidoParaVenta?.Estadio}",
				$"Zona: {_zonaParaVenta?.NombreZona}",
				$"Cantidad de boletos: {_asientosSeleccionados.Count}",
				"ASIENTOS:"
			};

			lineas.AddRange(_asientosSeleccionados.OrderBy(a => a).Select(a => $"  {a}"));
			lineas.Add(string.Empty);
			lineas.Add($"Precio unitario: {_zonaParaVenta?.PrecioBase.ToString("C2")}");
			lineas.Add($"Subtotal: {subtotal:C2}");
			lineas.Add($"IVA: {iva:C2}");
			lineas.Add($"Total pagado: {total:C2}");
			lineas.Add($"Método de pago: {metodoPago}");
			lineas.Add(string.Empty);
			lineas.Add("Conserva este boleto y preséntalo el día del evento.");

			try
			{
				GeneradorPdfBasico.Guardar(dialogo.FileName, "BOLETO DE ACCESO", lineas);
				MessageBox.Show("El boleto PDF se guardó correctamente.", "Boleto generado",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"La venta se registró, pero no se pudo guardar el PDF.\n{ex.Message}",
					"Error al generar el boleto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		//========================================================================================
		// Función: exporta a PDF las filas y los totales que están visibles en el reporte actual.
		//========================================================================================
		private void ExportarReporteVentasPdf()
		{
			if (gridReporteInOper.Rows.Count == 0)
			{
				MessageBox.Show("No hay ventas en el reporte para exportar.");
				return;
			}

			using var dialogo = new SaveFileDialog
			{
				Title = "Exportar reporte de ventas",
				Filter = "Archivo PDF (*.pdf)|*.pdf",
				FileName = $"ReporteVentas_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
				AddExtension = true,
				DefaultExt = "pdf",
				OverwritePrompt = true
			};

			if (dialogo.ShowDialog(this) != DialogResult.OK)
				return;

			var lineas = new List<string>
			{
				$"Periodo del reporte: {dateTimePicker1.Value:dd/MM/yyyy} al {dateTimePicker2.Value:dd/MM/yyyy}",
				$"Estadio: {cboEstadioReporteInOper.SelectedItem ?? "Todos los estadios"}",
				$"Partido: {(cboPartidoReporteInOper.SelectedItem as ReportePartidoFiltro)?.Descripcion ?? "Todos los partidos"}",
				$"{lblBoletosVendidosReportesInOper.Text}    {lblIngresosReportesInOper.Text}",
				string.Empty,
				"DETALLE DE VENTAS"
			};

			foreach (DataGridViewRow fila in gridReporteInOper.Rows)
			{
				if (fila.IsNewRow) continue;
				string valor(int indice) => Convert.ToString(fila.Cells[indice].Value) ?? string.Empty;
				lineas.Add($"ID de venta: {valor(0)} | Folio: {valor(1)} | Fecha: {valor(2)}");
				lineas.Add($"Estadio: {valor(3)} | Partido: {valor(4)}");
				lineas.Add($"Zona: {valor(5)} | Boletos: {valor(6)}");
				lineas.Add($"Subtotal: {valor(7)} | Total: {valor(8)}");
				lineas.Add($"Fecha del partido: {valor(9)} | Método de pago: {valor(10)}");
				lineas.Add(string.Empty);
			}

			try
			{
				GeneradorPdfBasico.Guardar(dialogo.FileName, "REPORTE DE VENTAS", lineas);
				MessageBox.Show("El reporte PDF se guardó correctamente.", "Reporte exportado",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"No se pudo exportar el reporte.\n{ex.Message}",
					"Error al exportar", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

	}
}
