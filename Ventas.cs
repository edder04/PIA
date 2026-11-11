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
		private bool _ventaRegistrada;
		private int? _idClienteEditando;

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
			cboEstadioODInOper.SelectedIndexChanged += cboEstadioODInOper_SelectedIndexChanged;
			cboPartidoODInOper.SelectedIndexChanged += cboPartidoODInOper_SelectedIndexChanged;
			dgvDisponibilidadODInOper.CellClick += dgvDisponibilidadODInOper_CellClick;
			btnReporteVentaInOper.Click += btnReporteVentaInOper_Click;
			btnOcupacionDisponibilidadInOper.Click += btnOcupacionDisponibilidadInOper_Click;
			radioBtnEfectivoVentaPaso4.CheckedChanged += radioBtnMetodoPago_CheckedChanged;
			radioBtnTarjetaVentaPaso4.CheckedChanged += radioBtnMetodoPago_CheckedChanged;
			ActualizarVisibilidadCamposEdicion();
			ActualizarCamposPago();
		}


		// Método: Ventas_Load.
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

		// Método: ConfigurarGridClientes.
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

		// Método: ConfigurarEstiloGrids.
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

		// Método: ConfigurarGridAsientos.
		private void ConfigurarGridAsientos()
		{
			dgvAsientosInOper.AllowUserToAddRows = false;
			dgvAsientosInOper.AllowUserToDeleteRows = false;
			dgvAsientosInOper.AllowUserToResizeRows = false;
			dgvAsientosInOper.ReadOnly = true;
			dgvAsientosInOper.MultiSelect = false;
			dgvAsientosInOper.RowHeadersVisible = true;
			dgvAsientosInOper.RowHeadersWidth = 70;
			dgvAsientosInOper.SelectionMode = DataGridViewSelectionMode.CellSelect;
			dgvAsientosInOper.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dgvAsientosInOper.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dgvAsientosInOper.CellClick -= gridAsientos_CellClick;
			dgvAsientosInOper.CellClick += gridAsientos_CellClick;
		}

		// Método: ConfigurarTarjeta.
		private void ConfigurarTarjeta()
		{
			radioBtnEfectivoVentaPaso4.Checked = true;
		}
		// Método: CargarResumenInicioAsync.
		private async Task CargarResumenInicioAsync()
		{
			using var db = new AppDbContext();
			DateTime hoy = DateTime.Today;

			var ventasHoy = await db.vw_Ventas
				.AsNoTracking()
				.Where(v => v.FechaHoraVenta >= hoy &&
							v.FechaHoraVenta < hoy.AddDays(1) &&
							v.EstadoVenta == "Activa")
				.ToListAsync();

			lblInfoVentasDelDiaInicio.Text = ventasHoy.Count.ToString();
			lblInfoIngresosInicio.Text = ventasHoy.Sum(v => v.Total).ToString("C2");
			lblInfoBoletosVendidosInicio.Text =
				ventasHoy.Sum(v => v.TotalBoletos - v.BoletosCancelados).ToString();
		}

		// Método: CargarClientesAsync.
		private async Task CargarClientesAsync()
		{
			string texto = txtNombreCliente.Text.Trim();
			if (texto.Equals("NOMBRE CLIENTE", StringComparison.OrdinalIgnoreCase))
				texto = string.Empty;

			_cargandoClientes = true;
			try
			{
				using var db = new AppDbContext();
				var consulta = db.vw_Clientes.AsNoTracking().Where(c => c.Activo);

				if (!string.IsNullOrWhiteSpace(texto))
				{
					consulta = consulta.Where(c =>
						c.Nombres.Contains(texto) ||
						c.ApPaterno.Contains(texto) ||
						(c.ApMaterno != null && c.ApMaterno.Contains(texto)) ||
						c.RFC.Contains(texto) ||
						c.CorreoElectronico.Contains(texto));
				}

				var clientes = await consulta
					.OrderBy(c => c.ApPaterno)
					.ThenBy(c => c.Nombres)
					.ToListAsync();

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

		// Método: txtNombreCliente_TextChanged.
		private async void txtNombreCliente_TextChanged(object? sender, EventArgs e)
		{
			if (!_cargandoClientes)
				await CargarClientesAsync();
		}

		// Método: txtBuscarClienteInOper_TextChanged.
		private async void txtBuscarClienteInOper_TextChanged(object? sender, EventArgs e)
		{
			await CargarGridClientesOperAsync();
		}

		// Método: CargarGridClientesOperAsync.
		private async Task CargarGridClientesOperAsync()
		{
			string texto = txtBuscarClienteInOper.Text.Trim();
			using var db = new AppDbContext();
			var consulta = db.vw_Clientes.AsNoTracking().Where(c => c.Activo);

			if (!string.IsNullOrWhiteSpace(texto))
			{
				consulta = consulta.Where(c =>
					c.Nombres.Contains(texto) || c.ApPaterno.Contains(texto) ||
					(c.ApMaterno != null && c.ApMaterno.Contains(texto)) ||
					c.RFC.Contains(texto) || c.CorreoElectronico.Contains(texto));
			}

			var clientes = await consulta.OrderBy(c => c.ApPaterno)
				.ThenBy(c => c.Nombres).ToListAsync();

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

		// Método: btnAgregarNuevoClienteInOper_Click.
		private async void btnAgregarNuevoClienteInOper_Click(object? sender, EventArgs e)
		{
			string nombres = txtNombreNuevoClienteInOper.Text.Trim();
			string paterno = txtApPaternoNuevoClienteInOper.Text.Trim();
			string rfc = txtRFCNuevoClienteInOper.Text.Trim().ToUpperInvariant();
			string correo = txtCorreoNuevoClienteInOper.Text.Trim();
			string celular = txtTelefonoCelularNuevoClienteInOper.Text.Trim();

			if (string.IsNullOrWhiteSpace(nombres) || string.IsNullOrWhiteSpace(paterno) ||
				string.IsNullOrWhiteSpace(rfc) || string.IsNullOrWhiteSpace(correo) ||
				string.IsNullOrWhiteSpace(celular) ||
				!DateTime.TryParse(txtFechaNacimientoNuevoClienteInOper.Text.Trim(),
					CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime nacimiento) ||
				string.IsNullOrWhiteSpace(txtCalleNuevoClienteInOper.Text) ||
				string.IsNullOrWhiteSpace(txtNoExteriorNuevoClienteInOper.Text) ||
				string.IsNullOrWhiteSpace(txtCPNuevoClienteInOper.Text) ||
				string.IsNullOrWhiteSpace(txtColoniaNuevoClienteInOper.Text) ||
				string.IsNullOrWhiteSpace(txtCiudadNuevoClienteInOper.Text) ||
				string.IsNullOrWhiteSpace(txtEstadoNuevoClienteInOper.Text) ||
				string.IsNullOrWhiteSpace(txtPaisNuevoClienteInOper.Text))
			{
				MessageBox.Show("Completa los campos obligatorios y verifica la fecha (dd/MM/aaaa).");
				return;
			}
			if (rfc.Length is not (12 or 13))
			{
				MessageBox.Show("El RFC debe tener 12 o 13 caracteres.");
				txtRFCNuevoClienteInOper.Focus();
				return;
			}

			string interior = txtNoInteriorNuevoClienteInOper.Text.Trim();
			string domicilio = $"{txtCalleNuevoClienteInOper.Text.Trim()} {txtNoExteriorNuevoClienteInOper.Text.Trim()}" +
				(interior.Length > 0 ? $" Int. {interior}" : "") +
				$", C.P. {txtCPNuevoClienteInOper.Text.Trim()}, {txtColoniaNuevoClienteInOper.Text.Trim()}, " +
				$"{txtCiudadNuevoClienteInOper.Text.Trim()}, {txtEstadoNuevoClienteInOper.Text.Trim()}, {txtPaisNuevoClienteInOper.Text.Trim()}";

			try
			{
				var cliente = new Cliente
				{
					Nombres = nombres,
					ApPaterno = paterno,
					ApMaterno = string.IsNullOrWhiteSpace(txtApMaternoNuevoClienteInOper.Text)
						? null : txtApMaternoNuevoClienteInOper.Text.Trim(),
					FechaNacimiento = nacimiento.Date,
					RFC = rfc,
					CorreoElectronico = correo,
					TelefonoCasa = string.IsNullOrWhiteSpace(txtTelefonoCasaNuevoClienteInOper.Text)
						? null : txtTelefonoCasaNuevoClienteInOper.Text.Trim(),
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
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudo agregar el cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		// Método: LimpiarFormularioNuevoClienteOper.
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

		// Método: btnContinuarVentaPaso1_Click.
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

		// Método: CargarPartidosAsync.
		private async Task CargarPartidosAsync()
		{
			using var db = new AppDbContext();
			var partidos = await db.vw_Partidos
				.AsNoTracking()
				.Where(p => p.Estado == "Programado" && p.FechaHora >= DateTime.Now)
				.OrderBy(p => p.FechaHora)
				.ToListAsync();

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

		// Método: PartidoElegidoEnPaso2.
		private void PartidoElegidoEnPaso2(vw_Partidos partido)
		{
			_partidoParaVenta = partido;
			txtEquipoVentaPaso2.Text =
				$"{partido.EquipoLocal} vs. {partido.EquipoVisitante}";
			txtFechaVentaPaso2.Text = partido.FechaHora.ToString("dd/MM/yyyy HH:mm");
			txtEstadioVentaPaso2.Text = partido.Estadio;
		}

		// Método: btnContinuarVentaPaso2_Click.
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

				var zonas = await db.vw_DisponibilidadZonas
					.AsNoTracking()
					.Where(z => z.IdPartido == _partidoParaVenta.IdPartido &&
								z.BoletosDisponibles > 0)
					.OrderBy(z => z.NombreZona)
					.ToListAsync();

				cboZonaVentaPaso3.DataSource = null;
				_zonaParaVenta = null;
				_asientosSeleccionados.Clear();
				_asientosOcupados.Clear();
				dgvAsientosInOper.Rows.Clear();

				cboZonaVentaPaso3.DisplayMember = nameof(vw_DisponibilidadZonas.NombreZona);
				cboZonaVentaPaso3.ValueMember = nameof(vw_DisponibilidadZonas.IdZona);
				cboZonaVentaPaso3.DataSource = zonas;
				cboZonaVentaPaso3.SelectedIndex = -1;
				ActualizarTotalesVenta();

				if (zonas.Count == 0)
				{
					MessageBox.Show("Este partido no tiene zonas con boletos disponibles.");
					return;
				}

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

		// Método: cboZonaVentaPaso3_SelectedIndexChanged.
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

		// Método: CargarGridAsientosAsync.
		private async Task CargarGridAsientosAsync()
		{
			if (_partidoParaVenta is null || _zonaParaVenta is null)
				return;

			dgvAsientosInOper.Rows.Clear();
			dgvAsientosInOper.Columns.Clear();
			_asientosSeleccionados.Clear();
			_asientosOcupados.Clear();

			using var db = new AppDbContext();
			var ocupados = await db.vw_AsientosOcupados
				.AsNoTracking()
				.Where(a => a.IdPartido == _partidoParaVenta.IdPartido &&
							a.IdZona == _zonaParaVenta.IdZona)
				.Select(a => new { a.Fila, a.Asiento })
				.ToListAsync();

			foreach (var asiento in ocupados)
				_asientosOcupados.Add($"{asiento.Fila.Trim()}-{asiento.Asiento.Trim()}");

			for (int n = 1; n <= AsientosPorFila; n++)
				dgvAsientosInOper.Columns.Add($"Asiento{n:00}", n.ToString("00"));

			int filas = (_zonaParaVenta.CupoZona + AsientosPorFila - 1) / AsientosPorFila;

			dgvAsientosInOper.SuspendLayout();
			try
			{
				for (int filaIndex = 0; filaIndex < filas; filaIndex++)
				{
					int rowIndex = dgvAsientosInOper.Rows.Add();
					var row = dgvAsientosInOper.Rows[rowIndex];
					row.HeaderCell.Value = "F" + (filaIndex + 1).ToString("D3");

					for (int columna = 0; columna < AsientosPorFila; columna++)
					{
						int indice = filaIndex * AsientosPorFila + columna;
						var cell = row.Cells[columna];

						if (indice >= _zonaParaVenta.CupoZona)
						{
							cell.Value = string.Empty;
							cell.Tag = null;
							continue;
						}

						string fila = (filaIndex + 1).ToString("D3");
						string asiento = (columna + 1).ToString("D2");
						string clave = $"{fila}-{asiento}";
						bool ocupado = _asientosOcupados.Contains(clave);

						cell.Value = asiento;
						cell.Tag = clave;
						cell.Style.BackColor = ocupado
							? System.Drawing.Color.IndianRed
							: System.Drawing.Color.FromArgb(60, 60, 60);
						cell.Style.ForeColor = System.Drawing.Color.White;
						cell.Style.SelectionBackColor = cell.Style.BackColor;
						cell.Style.SelectionForeColor = System.Drawing.Color.White;
						cell.ToolTipText = ocupado ? "Ocupado" : "Disponible";
					}
				}
			}
			finally
			{
				dgvAsientosInOper.ResumeLayout();
			}

			dgvAsientosInOper.ClearSelection();
			ActualizarTotalesVenta();
		}

		// Método: gridAsientos_CellClick.
		private void gridAsientos_CellClick(object? sender, DataGridViewCellEventArgs e)
		{
			if
 (e.RowIndex < 0 || e.ColumnIndex < 0)
				return;

			var cell = dgvAsientosInOper.Rows[e.RowIndex].Cells[e.ColumnIndex];
			if (cell.Tag is not string clave || _asientosOcupados.Contains(clave))
				return;

			if (_asientosSeleccionados.Contains(clave))
			{
				_asientosSeleccionados.Remove(clave);
				cell.Style.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
			}
			else
			{
				if (_asientosSeleccionados.Count >= MaximoAsientos)

				{
					MessageBox.Show($"Solo puedes seleccionar hasta {MaximoAsientos} asientos.");
					return;
				}

				_asientosSeleccionados.Add(clave);
				cell.Style.BackColor = System.Drawing.Color.SeaGreen;
			}

			cell.Style.SelectionBackColor = cell.Style.BackColor;
			dgvAsientosInOper.ClearSelection();
			ActualizarTotalesVenta();
		}

		// Método: ActualizarTotalesVenta.
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

		// Método: PrepararPaso4.
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

		// Método: btnContinuarVentaPaso3_Click.
		private void btnContinuarVentaPaso3_Click(object sender, EventArgs e)
		{
			if (PrepararPaso4())
				tabCtrlVenta.SelectedIndex = 3;
		}

		// Método: btnConfirmarVentaPaso4_Click.
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
			else if (radioBtnTarjetaVentaPaso4.Checked)
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

				// La tarjeta se registra como Crédito; no se guardan PAN ni CVV.
				metodoPago = "Crédito";
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

		// Método: radioBtnMetodoPago_CheckedChanged.
		private void radioBtnMetodoPago_CheckedChanged(object? sender, EventArgs e)
		{
			ActualizarCamposPago();
		}

		// Método: ActualizarCamposPago.
		private void ActualizarCamposPago()
		{
			bool tarjeta = radioBtnTarjetaVentaPaso4.Checked;

			lblTitularVentaPaso4.Visible = tarjeta;
			txtTitularVentaPaso4.Visible = tarjeta;
			lblNumeroTarjetaVentaPaso4.Visible = tarjeta;
			txtNumeroTarjetaVentaPaso4.Visible = tarjeta;
			lblVencimientoTarjetaVentaPaso4.Visible = tarjeta;
			txtVencimientoTarjetaVentaPaso4.Visible = tarjeta;

			lblCVVVentaPaso4.Visible = tarjeta;
			txtCVVVentaPaso4.Visible = tarjeta;
		}

		// Método: DetectarMarcaTarjeta.
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

		// Método: PasaLuhn.
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

		// Método: btnAtrasVentaPaso2_Click.
		private void btnAtrasVentaPaso2_Click(object sender, EventArgs e)
		{
			tabCtrlVenta.SelectedIndex = 0;
		}

		// Método: btnAtrasVentaPaso3_Click.
		private void btnAtrasVentaPaso3_Click(object sender, EventArgs e)
		{

			tabCtrlVenta.SelectedIndex = 1;
		}

		// Método: btnAtrasVentaPaso4_Click.
		private void btnAtrasVentaPaso4_Click(object sender, EventArgs e)
		{
			tabCtrlVenta.SelectedIndex = 2;
		}

		// Método: btnNuevaVentaVentaRealizada_Click.
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

		// Método: btnInicio_Click.
		private void btnInicio_Click(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 0;
			pnlMenu.Visible = false;
		}

		// Método: btnVentasVentas_Click_1.
		private void btnVentasVentas_Click_1(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 1;
			pnlMenu.Visible = false;
		}

		// Método: btnClientesVentas_Click.
		private void btnClientesVentas_Click(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 2;
			pnlMenu.Visible = false;
		}

		// Método: btnPartidosVentas_Click.
		private void btnPartidosVentas_Click(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 3;
			pnlMenu.Visible = false;
		}

		// Método: btnFacturasVentas_Click.
		private void btnFacturasVentas_Click(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 4;
			pnlMenu.Visible = false;
		}

		// Método: btnReportesVentas_Click.
		private void btnReportesVentas_Click(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 5;

			pnlMenu.Visible = false;
		}

		// Método: btnSalir_Click.
		private void btnSalir_Click(object sender, EventArgs e)
		{
			pnlMenu.Visible = false;
			new InicioSesion().Show();
			Close();
		}

		// Método: btnAjustes_Click.
		private void btnAjustes_Click(object sender, EventArgs e)
		{
			pnlMenu.Visible = !pnlMenu.Visible;
			if (pnlMenu.Visible)
				pnlMenu.BringToFront();
		}

		// Método: btnNuevoClienteInCliente_Click.
		private void btnNuevoClienteInCliente_Click(object sender, EventArgs e)
		{
			tabClientesInVentas.SelectedIndex = 1;
		}

		// Método: btnEditarCliente_Click.
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

		// Método: cboBoxEditarClienteInOper_SelectedIndexChanged.
		private void cboBoxEditarClienteInOper_SelectedIndexChanged(object? sender, EventArgs e)
		{
			ActualizarVisibilidadCamposEdicion();
		}

		// Método: ActualizarVisibilidadCamposEdicion.
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

		// Método: CargarFiltrosReporteAsync.
		private async Task CargarFiltrosReporteAsync()
		{
			_cargandoFiltrosReporte = true;
			try
			{
				using var db = new AppDbContext();
				var estadios = await db.vw_Estadios.AsNoTracking()
					.OrderBy(e => e.Nombre)
					.Select(e => e.Nombre)
					.Distinct()
					.ToListAsync();

				cboEstadioReporteInOper.Items.Clear();
				cboEstadioReporteInOper.Items.Add("Todos los estadios");
				foreach (string estadio in estadios)
					cboEstadioReporteInOper.Items.Add(estadio);
				cboEstadioReporteInOper.SelectedIndex = 0;

				dateTimePicker1.Value = DateTime.Today.AddDays(-30);
				dateTimePicker2.Value = DateTime.Today;
			}
			finally
			{
				_cargandoFiltrosReporte = false;
			}

			await CargarPartidosFiltroReporteAsync("Todos los estadios");
		}

		// Método: cboEstadioReporteInOper_SelectedIndexChanged.
		private async void cboEstadioReporteInOper_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (_cargandoFiltrosReporte)
				return;



			await CargarPartidosFiltroReporteAsync(cboEstadioReporteInOper.SelectedItem?.ToString());
		}

		//=======================
		// CARGA LOS ESTADIOS DEL FILTRO DE OCUPACION
		//=======================
		private async Task CargarFiltrosOcupacionAsync()
		{
			_cargandoFiltrosOcupacion = true;
			try
			{
				using var db = new AppDbContext();
				var estadios = await db.vw_Estadios.AsNoTracking()
					.OrderBy(e => e.Nombre)
					.Select(e => new FiltroEstadioOcupacion
					{
						IdEstadio = e.IdEstadio,
						Nombre = e.Nombre
					})
					.ToListAsync();

				cboEstadioODInOper.DataSource = null;
				cboEstadioODInOper.DisplayMember = nameof(FiltroEstadioOcupacion.Nombre);
				cboEstadioODInOper.ValueMember = nameof(FiltroEstadioOcupacion.IdEstadio);
				cboEstadioODInOper.DataSource = estadios;
				cboEstadioODInOper.SelectedIndex = -1;
				cboPartidoODInOper.DataSource = null;
				dgvDisponibilidadODInOper.Rows.Clear();
				dgvAsientosODInOper.Rows.Clear();
				dgvAsientosODInOper.Columns.Clear();
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

		//=======================
		// CONFIGURA LOS GRIDS DE OCUPACION Y DISPONIBILIDAD
		//=======================
		private void ConfigurarGridsOcupacion()
		{
			dgvDisponibilidadODInOper.ReadOnly = true;
			dgvDisponibilidadODInOper.AllowUserToAddRows = false;
			dgvDisponibilidadODInOper.AllowUserToDeleteRows = false;
			dgvDisponibilidadODInOper.MultiSelect = false;
			dgvDisponibilidadODInOper.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgvDisponibilidadODInOper.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

			dgvAsientosODInOper.ReadOnly = true;
			dgvAsientosODInOper.AllowUserToAddRows = false;
			dgvAsientosODInOper.AllowUserToDeleteRows = false;
			dgvAsientosODInOper.MultiSelect = false;
			dgvAsientosODInOper.SelectionMode = DataGridViewSelectionMode.CellSelect;
			dgvAsientosODInOper.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		}

		//=======================
		// CARGA LOS PARTIDOS DEL ESTADIO SELECCIONADO
		//=======================
		private async void cboEstadioODInOper_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (_cargandoFiltrosOcupacion || cboEstadioODInOper.SelectedValue is not int idEstadio)
				return;

			_cargandoFiltrosOcupacion = true;
			try
			{
				using var db = new AppDbContext();
				var partidosDb = await db.vw_Partidos.AsNoTracking()
					.Where(p => p.IdEstadio == idEstadio)
					.OrderByDescending(p => p.FechaHora)
					.Select(p => new
					{
						p.IdPartido,
						p.EquipoLocal,
						p.EquipoVisitante,
						p.FechaHora,
						p.Estado
					})
					.ToListAsync();

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
				dgvDisponibilidadODInOper.Rows.Clear();
				dgvAsientosODInOper.Rows.Clear();
				dgvAsientosODInOper.Columns.Clear();
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

		//=======================
		// CARGA LA DISPONIBILIDAD POR ZONA DEL PARTIDO
		//=======================
		private async void cboPartidoODInOper_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (_cargandoFiltrosOcupacion || cboPartidoODInOper.SelectedValue is not int idPartido)
				return;

			try
			{
				using var db = new AppDbContext();
				var zonas = await db.vw_DisponibilidadZonas.AsNoTracking()
					.Where(z => z.IdPartido == idPartido)
					.OrderBy(z => z.NombreZona)
					.ToListAsync();

				dgvDisponibilidadODInOper.Rows.Clear();
				dgvAsientosODInOper.Rows.Clear();
				dgvAsientosODInOper.Columns.Clear();
				foreach (var zona in zonas)
				{
					decimal porcentaje = zona.CupoZona <= 0
						? 0m
						: zona.BoletosVendidos * 100m / zona.CupoZona;
					int fila = dgvDisponibilidadODInOper.Rows.Add(
						zona.NombreZona,
						zona.CupoZona,
						zona.BoletosDisponibles,
						zona.BoletosVendidos,
						$"{porcentaje:N1}%");
					dgvDisponibilidadODInOper.Rows[fila].Tag = zona;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudo cargar la disponibilidad", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//=======================
		// DIBUJA LOS ASIENTOS OCUPADOS Y DISPONIBLES DE LA ZONA ELEGIDA
		//=======================
		private async void dgvDisponibilidadODInOper_CellClick(object? sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || dgvDisponibilidadODInOper.Rows[e.RowIndex].Tag is not vw_DisponibilidadZonas zona)
				return;

			if (cboPartidoODInOper.SelectedValue is not int idPartido || idPartido != zona.IdPartido)
				return;

			dgvAsientosODInOper.Rows.Clear();
			dgvAsientosODInOper.Columns.Clear();
			const int asientosPorFila = 15;
			for (int n = 1; n <= asientosPorFila; n++)
				dgvAsientosODInOper.Columns.Add($"Asiento{n:00}", n.ToString("00"));

			HashSet<string> ocupados;
			try
			{
				using var db = new AppDbContext();
				var ocupadosDb = await db.vw_AsientosOcupados.AsNoTracking()
					.Where(a => a.IdPartido == idPartido && a.IdZona == zona.IdZona)
					.Select(a => new { a.Fila, a.Asiento })
					.ToListAsync();
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
				int indexFila = dgvAsientosODInOper.Rows.Add();
				DataGridViewRow fila = dgvAsientosODInOper.Rows[indexFila];
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
			dgvAsientosODInOper.ClearSelection();
		}

		//=======================
		// CAMBIA A LA PESTAÑA DE REPORTE DE VENTAS
		//=======================
		private void btnReporteVentaInOper_Click(object? sender, EventArgs e)
		{
			tabReporteVenta.SelectedIndex = 0;
		}

		//=======================
		// CAMBIA A LA PESTAÑA DE OCUPACION Y DISPONIBILIDAD
		//=======================
		private void btnOcupacionDisponibilidadInOper_Click(object? sender, EventArgs e)
		{
			tabReporteVenta.SelectedIndex = 1;
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

		// Método: CargarPartidosFiltroReporteAsync.
		private async Task CargarPartidosFiltroReporteAsync(string? estadio)
		{
			_cargandoFiltrosReporte = true;
			try
			{
				using var db = new AppDbContext();
				var consulta = db.vw_Partidos.AsNoTracking();
				if (!string.IsNullOrWhiteSpace(estadio) && estadio != "Todos los estadios")
					consulta = consulta.Where(p => p.Estadio == estadio);

				var partidosDb = await consulta.OrderByDescending(p => p.FechaHora)
					.Select(p => new
					{
						p.IdPartido,
						p.EquipoLocal,
						p.EquipoVisitante,
						p.FechaHora
					}).ToListAsync();
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
				cboPartidoReporteInOper.SelectedIndex = 0;
			}
			finally
			{
				_cargandoFiltrosReporte = false;
			}
		}

		// Método: btnFiltrarReporteInOper_Click.
		private async void btnFiltrarReporteInOper_Click(object? sender, EventArgs e)
		{
			await FiltrarReporteAsync();
		}

		// Método: FiltrarReporteAsync.
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
				var boletosQuery = db.vw_BoletosVendidos.AsNoTracking()
					.Where(b => b.FechaHoraVenta >= desde && b.FechaHoraVenta < hastaExclusivo &&
								b.EstadoBoleto == "Vendido" && b.EstadoVenta == "Activa");

				if (!string.IsNullOrWhiteSpace(estadio) && estadio != "Todos los estadios")
					boletosQuery = boletosQuery.Where(b => b.Estadio == estadio);
				if (idPartido.HasValue)
					boletosQuery = boletosQuery.Where(b => b.IdPartido == idPartido.Value);

				var boletos
 = await boletosQuery.ToListAsync();
				var idsVenta = boletos.Select(b => b.IdVenta).Distinct().ToList();
				var ventas = await db.vw_Ventas.AsNoTracking()
					.Where(v => idsVenta.Contains(v.IdVenta) && v.EstadoVenta == "Activa")
					.ToDictionaryAsync(v => v.IdVenta);

				gridReporteInOper.Rows.Clear();
				var grupos = boletos.GroupBy(b => new
				{
					b.IdVenta,
					b.FolioVenta,
					b.FechaHoraVenta,
					b.Estadio,
					b.IdPartido,
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
						grupo.Key.FolioVenta.ToString(),
						grupo.Key.FechaHoraVenta.ToString("dd/MM/yyyy HH:mm"),
						grupo.Key.Estadio,
						$"{grupo.Key.EquipoLocal} vs. {grupo.Key.EquipoVisitante}",
						grupo.Key.NombreZona,
						grupo.Count(),
						venta.Subtotal.ToString("C2"),
						venta.Total.ToString("C2"));
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

		// Método: btnLimparGridReporteInOper_Click.
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

		// Método: btnGuardarCambiosEditarClienteInOper_Click.
		private async void btnGuardarCambiosEditarClienteInOper_Click(object? sender, EventArgs e)
		{
			if (_idClienteEditando is null)
			{
				MessageBox.Show("Selecciona un cliente desde el grid antes de editar.");
				return;
			}
			if (cboBoxEditarClienteInOper.SelectedItem is not string campo)
			{
				MessageBox.Show("Selecciona qué dato deseas cambiar.");
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
					if (string.IsNullOrWhiteSpace(calle) || string.IsNullOrWhiteSpace(exterior) ||
						string.IsNullOrWhiteSpace(txtCPEditarClienteInOper.Text) ||
						string.IsNullOrWhiteSpace(txtColoniaEditarClienteInOper.Text) ||
						string.IsNullOrWhiteSpace(txtCiudadEditarClienteInOper.Text) ||
						string.IsNullOrWhiteSpace(txtEstadoEditarClienteInOper.Text) ||
						string.IsNullOrWhiteSpace(txtPaisEditarClienteInOper.Text))
					{
						MessageBox.Show("Completa calle, número exterior, C.P., colonia, ciudad, estado y país.");
						return;
					}
					valor = $"{calle} {exterior}" + (interior.Length > 0 ? $" Int. {interior}" : "") +
						$", C.P. {txtCPEditarClienteInOper.Text.Trim()}, {txtColoniaEditarClienteInOper.Text.Trim()}, " +
						$"{txtCiudadEditarClienteInOper.Text.Trim()}, {txtEstadoEditarClienteInOper.Text.Trim()}, {txtPaisEditarClienteInOper.Text.Trim()}";
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
					MessageBox.Show("Escribe el nuevo valor.");
					txtNuevoDatoEditarClienteInOper.Focus();
					return;
				}
			}

			if (campo == "CORREO ELECTRONICO" && !Regex.IsMatch(valor, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
			{
				MessageBox.Show("Escribe un correo electrónico válido.");
				return;
			}
			if (campo.StartsWith("TELEFONO", StringComparison.Ordinal) &&
				!Regex.IsMatch(valor, @"^\+?[0-9\s()-]{7,15}$"))
			{
				MessageBox.Show("Escribe un teléfono válido.");
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
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudieron guardar los cambios", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				btnGuardarCambiosEditarClienteInOper.Enabled = true;
			}
		}

		// Método: button2_Click.
		private void button2_Click(object sender, EventArgs e)
		{
			tabCtrlVentas.SelectedIndex = 2;
			tabClientesInVentas.SelectedIndex = 1;
		}

		// Método: button6_Click.
		private void button6_Click(object sender, EventArgs e)
		{
			ExportarReporteVentasPdf();
		}

		// Función: abre el diálogo para guardar el boleto PDF de la venta recién registrada.
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

		// Función: exporta a PDF las filas y los totales que están visibles en el reporte actual.
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
				lineas.Add($"Folio: {valor(0)} | Fecha: {valor(1)}");
				lineas.Add($"Estadio: {valor(2)} | Partido: {valor(3)}");
				lineas.Add($"Zona: {valor(4)} | Boletos: {valor(5)}");
				lineas.Add($"Subtotal: {valor(6)} | Total: {valor(7)}");
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

		private void tabPgReportes_Click(object sender, EventArgs e)
		{

		}
	}
}
