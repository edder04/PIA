
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.AspNetCore.Identity;
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

namespace PIA{                                         
	public partial class Administracion : Form
	{                                                         
		private int? _idEstadioEnCaptura = null;
		private int _zonasAgregadas = 0;
		private int _capacidadEstadioEnCaptura = 0;
		private int _cupoZonasAgregado = 0;
		private vw_Partidos? _partidoParaVenta;
		private vw_Clientes? _clienteParaVenta;
		private vw_DisponibilidadZonas? _zonaParaVenta;
		private readonly HashSet<string> _asientosOcupados = new();
		private readonly HashSet<string> _asientosSeleccionados = new();
		private const int AsientosPorFila = 15;
		private const decimal TasaIva = 0.16m;
		private bool _cargandoVentasCancelables;
		private bool _recargarVentasCancelables;
		private bool _cargandoFiltrosOcupacion;
		private bool _cargandoFiltrosReporte;
		private bool _cargandoFiltrosListaPartidos;
		private int _versionCargaAsientos;
		private Label? _lblCargandoAsientos;
		private bool _cargandoAsientos;
		private Cursor? _cursorAnteriorCargaAsientos;
		private int _versionConsultaFactura;
		private HistorialClienteReporte? _historialClienteActual;
		private int? _usuarioSeleccionadoParaEditar;
		private static readonly PasswordHasher<Usuario> PasswordHasher = new();

		//==============================================================
		// Este es el constructor que se ejecuta cuando abro la ventana.
		//==============================================================
		public Administracion()
		{                                                                       

			InitializeComponent();
			ConfigurarEstiloGrids(this);
			btnHistorialClienteInAdmin.Click += btnHistorialClienteInAdmin_Click;
			btnExportarInHistorialCliente.Click += btnExportarHistorialClienteInAdmin_Click;
			btnRegresarInHistorialCliente.Click += btnRegresarHistorialClienteInAdmin_Click;
			ConfigurarGridCancelaciones();
			btnCancelacion.Click += btnCancelacion_Click;
			txtBuscarCancelarInAdmin.TextChanged += txtBuscarCancelarInAdmin_TextChanged;
			button1.Click += button1_Click;
			cboZonaVentaPaso3.SelectedIndexChanged += cboZonaVentaPaso3_SelectedIndexChanged;
			pnlMenuInAdmin.Visible = false;  // Oculto el menú de administración al iniciar.

			ConfigurarGridReporteVentas();
			button6.Click += button6_Click;
			btnExportarReporteVentaInOper.Click += button6_Click;
			btnReporteVentaInReportes.Click += (_, _) => tabReportes.SelectedIndex = 0;
			btnOcupacionInReportes.Click += (_, _) => tabReportes.SelectedIndex = 1;
			cboEstadioInOcupacion.SelectedIndexChanged += cboEstadioInOcupacion_SelectedIndexChanged;
			cboPartidoInOcupacion.SelectedIndexChanged += cboPartidoInOcupacion_SelectedIndexChanged;
			dgvDisponibilidadInOcupacion.CellClick += dgvDisponibilidadInOcupacion_CellClick;
			ConfigurarGridsOcupacion();
			button6.Visible = false;
			lblIngresosReporte.Visible = false;
			lblBoletosVendidosReporte.Visible = false;
			btnLimparGridReporteInOper.Click += button4_Click;
			btnFiltrarReporteInOper.Click += btnFiltrarReporte_Click_1;
			cboEstadioReporteInOper.SelectedIndexChanged += cboEstadioReporteInOper_SelectedIndexChanged;
			comboBox1.SelectedIndexChanged += filtrosListaPartidos_SelectedIndexChanged;
			comboBox2.SelectedIndexChanged += filtrosListaPartidos_SelectedIndexChanged;
			btnTodosInPartidos.Click += (_, _) => SeleccionarFiltroFechaPartidos(0);
			btnHoyInPartidos.Click += (_, _) => SeleccionarFiltroFechaPartidos(1);
			btnEstaSemanaInPartidos.Click += (_, _) => SeleccionarFiltroFechaPartidos(2);
			btnProximosInPartidos.Click += (_, _) => SeleccionarFiltroFechaPartidos(3);
			textBox29.TextChanged += textBox29_TextChanged;
			dataGridView3.SelectionChanged += dataGridView3_SelectionChanged;
			btnCancelarInFactura.Click += btnCancelarInFactura_Click;
			btnGenerarFacturaInFactura.Click += btnGenerarFacturaInFactura_Click;
			txtCodigoVentaInFactura.TextChanged += txtCodigoVentaInFactura_TextChanged;
			foreach (Control control in new Control[] { txtRFCInFactura, txtRazonInFactura, txtCPInFactura, cboRegimenInFactura, cboCFDIInFactura,
				lblRFCInFactura, lblRazonInFactura, lblCPInFactura, lblRegimenInFactura, lblCFDIInFactura, lblDatosFiscalesInFactura })
				control.Visible = false;
			btnGenerarFacturaInFactura.Text = "EXPORTAR FACTURA PDF";
			GlassEffect.Apply(this);                                                                  // Aplico el efecto visual de vidrio a la ventana.

			RedondearBoton.Aplicar(btnAjustesInAdmin, 20);                                              // Redondeo las esquinas del botón de ajustes.
			RedondearBoton.Aplicar(btnHoyInPartidos, 20);                                               // Redondeo las esquinas del botón de partidos de hoy.
			RedondearBoton.Aplicar(btnProximosInPartidos, 20);                                          // Redondeo las esquinas del botón de próximos partidos.
			RedondearBoton.Aplicar(btnTodosInPartidos, 20);                                             // Redondeo las esquinas del botón para ver todos los partidos.
			RedondearBoton.Aplicar(btnEstaSemanaInPartidos, 20);                                        // Redondeo las esquinas del botón de partidos de esta semana.

			tabCtrlInAdmin.Show();                                                                             // Muestro el control de pestañas de la ventana.
			tabCtrlInAdmin.SelectedIndexChanged += async (_, _) =>
			{
				if (tabCtrlInAdmin.SelectedIndex == 1)
					await CargarGridClientesInVentasAsync();
			};
			tabCtrlVenta.SelectedIndexChanged += async (_, _) =>
			{
				if (tabCtrlVenta.SelectedIndex == 1)
					await CargarGridPartidosAsync();
			};
			txtNombreCliente.TextChanged += async (_, _) => await CargarGridClientesInVentasAsync();

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
			ConfigurarGridHistorialClienteInAdmin();
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
			_ = CargarGridClientesInVentasAsync();
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
			_ = CargarGridPartidosAsync();
		}
		//=========================================================
		// Consulta y carga la información necesaria para partidos.
		//=========================================================
		private async Task CargarPartidos()
		{
			pnlPartidos.Controls.Clear();

			using var db = new AppDbContext();

			var consulta = (await RepoConsulta.PartidosAsync(db))
				.Where(p => p.Estado == "Programado" && p.FechaHora >= DateTime.Now);
			if (comboBox2.SelectedItem is FiltroEstadioPartidos { IdEstadio: int idEstadio })
				consulta = consulta.Where(p => p.IdEstadio == idEstadio);

			DateTime hoy = DateTime.Today;
			DateTime inicioSemana = hoy.AddDays(-(((int)hoy.DayOfWeek + 6) % 7));
			switch (comboBox1.SelectedIndex)
			{
				case 1:
					consulta = consulta.Where(p => p.FechaHora.Date == hoy);
					break;
				case 2:
					consulta = consulta.Where(p => p.FechaHora.Date >= inicioSemana && p.FechaHora.Date < inicioSemana.AddDays(7));
					break;
				case 3:
					consulta = consulta.Where(p => p.FechaHora.Date >= hoy && p.FechaHora.Date < hoy.AddDays(31));
					break;
			}

			var partidos = consulta.OrderBy(p => p.FechaHora).ToList();

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

		private sealed class FiltroEstadioPartidos
		{
			public int? IdEstadio { get; init; }
			public string Nombre { get; init; } = string.Empty;
		}

		//===============================================================================
		// Consulta y carga la información necesaria para filtros estadio lista partidos.
		//===============================================================================
		private async Task CargarFiltrosEstadioListaPartidosAsync()
		{
			_cargandoFiltrosListaPartidos = true;
			try
			{
				using var db = new AppDbContext();
				var estadios = (await RepoConsulta.EstadiosAsync(db))
					.OrderBy(e => e.Nombre)
					.Select(e => new FiltroEstadioPartidos { IdEstadio = e.IdEstadio, Nombre = e.Nombre })
					.ToList();
				estadios.Insert(0, new FiltroEstadioPartidos { IdEstadio = null, Nombre = "Todos los estadios" });

				comboBox2.DataSource = null;
				comboBox2.DisplayMember = nameof(FiltroEstadioPartidos.Nombre);
				comboBox2.ValueMember = nameof(FiltroEstadioPartidos.IdEstadio);
				comboBox2.DataSource = estadios;
				comboBox2.SelectedIndex = 0;

				comboBox1.DataSource = null;
				comboBox1.Items.Clear();
				comboBox1.Items.AddRange(new object[]
				{
					"🗓️ Todas las Fechas",
					"Hoy",
					"Esta semana",
					"Próximos 30 días"
				});
				comboBox1.SelectedIndex = 0;
			}
			finally
			{
				_cargandoFiltrosListaPartidos = false;
			}
		}

		//===================================================================
		// Aplica la selección indicada y actualiza la vista correspondiente.
		//===================================================================
		private void SeleccionarFiltroFechaPartidos(int indice)
		{
			if (indice >= 0 && indice < comboBox1.Items.Count)
				comboBox1.SelectedIndex = indice;
		}

		//=======================================================================
		// Actualiza los datos dependientes cuando cambia la opción seleccionada.
		//=======================================================================
		private async void filtrosListaPartidos_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (_cargandoFiltrosListaPartidos)
				return;
			try
			{
				await CargarPartidos();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "No se pudo actualizar la lista de partidos",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		//=====================================================================
		// Carga los datos iniciales y prepara las secciones de administración.
		//=====================================================================
		private async void Administracion_Load(object sender, EventArgs e)
		{   // Este método se ejecuta cuando se carga la ventana de administración.

			//===========================================================
			// MUESTRA EL USUARIO ACTIVO EN EL ENCABEZADO Y EN BIENVENIDA
			//===========================================================
			string nombreUsuario = string.IsNullOrWhiteSpace(Sesion.NombreUsuario)
				? "USUARIO"
				: Sesion.NombreUsuario.Trim().ToUpper(new CultureInfo("es-MX"));
			lblNombreUsuario.Text = nombreUsuario;
			lblBienvenidaInicio.Text = $"BUENOS DÍAS, {nombreUsuario}";

			string rutaCheck = @"C:\Users\edcru\Downloads\UANL\Agosto-Diciembre 2026\checkMark.gif";
			if (File.Exists(rutaCheck))
			{
				using var imagen = Image.FromFile(rutaCheck);
				pictureBox1.Image = new Bitmap(imagen);
				pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			}
			await CargarFiltrosReporteAsync();
			await CargarResumenInicioAsync();
			await CargarFiltrosOcupacionAsync();
			await CargarFiltrosEstadioListaPartidosAsync();
			await CargarUsuariosAsync();
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

		//===============================================================
		// Consulta y carga la información necesaria para resumen inicio.
		//===============================================================
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
			lblInfoIngresosInicio.Text = ventasHoy.Sum(v => v.Total).ToString("C2", CultureInfo.GetCultureInfo("es-MX"));
			lblInfoBoletosVendidosInicio.Text =
				ventasHoy.Sum(v => Math.Max(0, v.TotalBoletos - v.BoletosCancelados)).ToString();
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
					.Select(e => new { e.IdEstadio, e.Nombre })
					.ToList();

				cboEstadioInOcupacion.DataSource = null;
				cboEstadioInOcupacion.DisplayMember = "Nombre";
				cboEstadioInOcupacion.ValueMember = "IdEstadio";
				cboEstadioInOcupacion.DataSource = estadios;
				cboEstadioInOcupacion.SelectedIndex = -1;
				cboPartidoInOcupacion.DataSource = null;
				LimpiarGridsOcupacion();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudieron cargar los estadios para ocupación",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			foreach (var grid in new[] { dgvDisponibilidadInOcupacion, dgvAsientosInOcupacion })
			{
				grid.ReadOnly = true;
				grid.AllowUserToAddRows = false;
				grid.AllowUserToDeleteRows = false;
				grid.MultiSelect = false;
				grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			}
			dgvDisponibilidadInOcupacion.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgvAsientosInOcupacion.SelectionMode = DataGridViewSelectionMode.CellSelect;
		}

		//=====================================================
		// Restablece los controles y datos de grids ocupacion.
		//=====================================================
		private void LimpiarGridsOcupacion()
		{
			dgvDisponibilidadInOcupacion.Rows.Clear();
			dgvAsientosInOcupacion.Rows.Clear();
			dgvAsientosInOcupacion.Columns.Clear();
		}

		//=======================================================================
		// Actualiza los datos dependientes cuando cambia la opción seleccionada.
		//=======================================================================
		private async void cboEstadioInOcupacion_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (_cargandoFiltrosOcupacion || cboEstadioInOcupacion.SelectedValue is not int idEstadio)
				return;

			_cargandoFiltrosOcupacion = true;
			try
			{
				using var db = new AppDbContext();
				var partidos = (await RepoConsulta.PartidosAsync(db))
					.Where(p => p.IdEstadio == idEstadio)
					.OrderByDescending(p => p.FechaHora)
					.Select(p => new
					{
						p.IdPartido,
						Descripcion = $"{p.EquipoLocal} vs. {p.EquipoVisitante} | {p.FechaHora:dd/MM/yyyy HH:mm} | {p.Estado}"
					})
					.ToList();

				cboPartidoInOcupacion.DataSource = null;
				cboPartidoInOcupacion.DisplayMember = "Descripcion";
				cboPartidoInOcupacion.ValueMember = "IdPartido";
				cboPartidoInOcupacion.DataSource = partidos;
				cboPartidoInOcupacion.SelectedIndex = -1;
				LimpiarGridsOcupacion();
				lblEstadioInOcupacion.Text = "ESTADIO:";
				lblPartidoInOcupacion.Text = "PARTIDO:";
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
		private async void cboPartidoInOcupacion_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (_cargandoFiltrosOcupacion || cboPartidoInOcupacion.SelectedValue is not int idPartido)
				return;

			try
			{
				using var db = new AppDbContext();
				var zonas = (await RepoConsulta.DisponibilidadZonasAsync(db))
					.Where(z => z.IdPartido == idPartido)
					.OrderBy(z => z.NombreZona)
					.ToList();

				dgvDisponibilidadInOcupacion.Rows.Clear();
				dgvAsientosInOcupacion.Rows.Clear();
				dgvAsientosInOcupacion.Columns.Clear();
				foreach (var zona in zonas)
				{
					decimal porcentaje = zona.CupoZona <= 0 ? 0m : zona.BoletosVendidos * 100m / zona.CupoZona;
					int fila = dgvDisponibilidadInOcupacion.Rows.Add(
						zona.NombreZona, zona.CupoZona, zona.BoletosDisponibles, zona.BoletosVendidos, $"{porcentaje:N1}%");
					dgvDisponibilidadInOcupacion.Rows[fila].Tag = zona;
				}
				lblPartidoInOcupacion.Text = "PARTIDO:";
				lblEstadioInOcupacion.Text = "ESTADIO:";
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
		private async void dgvDisponibilidadInOcupacion_CellClick(object? sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= dgvDisponibilidadInOcupacion.Rows.Count ||
				dgvDisponibilidadInOcupacion.Rows[e.RowIndex].Tag is not vw_DisponibilidadZonas zona ||
				cboPartidoInOcupacion.SelectedValue is not int idPartido || idPartido != zona.IdPartido)
				return;

			const int asientosPorFila = 15;
			dgvAsientosInOcupacion.Rows.Clear();
			dgvAsientosInOcupacion.Columns.Clear();
			for (int n = 1; n <= asientosPorFila; n++)
				dgvAsientosInOcupacion.Columns.Add($"Asiento{n:00}", n.ToString("00"));

			try
			{
				using var db = new AppDbContext();
				var ocupados = (await RepoConsulta.AsientosOcupadosAsync(db))
					.Where(a => a.IdPartido == idPartido && a.IdZona == zona.IdZona)
					.Select(a => $"{a.Fila.Trim()}-{a.Asiento.Trim()}")
					.ToHashSet(StringComparer.OrdinalIgnoreCase);

				int totalFilas = (zona.CupoZona + asientosPorFila - 1) / asientosPorFila;
				for (int filaIndex = 0; filaIndex < totalFilas; filaIndex++)
				{
					int rowIndex = dgvAsientosInOcupacion.Rows.Add();
					var fila = dgvAsientosInOcupacion.Rows[rowIndex];
					fila.HeaderCell.Value = $"F{filaIndex + 1:D3}";
					for (int columna = 0; columna < asientosPorFila; columna++)
					{
						int numeroAbsoluto = filaIndex * asientosPorFila + columna;
						var celda = fila.Cells[columna];
						if (numeroAbsoluto >= zona.CupoZona)
						{
							celda.Value = string.Empty;
							continue;
						}

						string codigo = $"{filaIndex + 1:D3}-{columna + 1:D2}";
						bool ocupado = ocupados.Contains(codigo);
						celda.Value = $"{columna + 1:D2}";
						celda.ToolTipText = codigo + (ocupado ? " | Ocupado" : " | Disponible");
						celda.Style.BackColor = ocupado ? Color.IndianRed : Color.FromArgb(60, 60, 60);
						celda.Style.ForeColor = Color.White;
						celda.Style.SelectionBackColor = celda.Style.BackColor;
						celda.Style.SelectionForeColor = Color.White;
					}
				}
				dgvAsientosInOcupacion.ClearSelection();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
					"No se pudo consultar la ocupación", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		//==========================================
		// METODO CARGAR GRID CLIENTES
		//==========================================
		//========================================================================
		// Consulta y carga la información necesaria para grid clientes in ventas.
		//========================================================================
		private async Task CargarGridClientesInVentasAsync()
		{
			string texto = txtNombreCliente.Text.Trim();
			using var db = new AppDbContext();
			var clientes = (await RepoConsulta.ClientesAsync(db))
				.Where(c => string.IsNullOrEmpty(texto) || c.ApPaterno.Contains(texto, StringComparison.OrdinalIgnoreCase) || c.CorreoElectronico.Contains(texto, StringComparison.OrdinalIgnoreCase) || c.RFC.Contains(texto, StringComparison.OrdinalIgnoreCase));

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
		//==============================================
		// Muestra el panel de inicio de administración.
		//==============================================
		private void btnInicio_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 0;
			pnlMenuInAdmin.Visible = false;
		}
		//=====================================
		// BOTON DE VENTAS EN EL MENU PRINCIPAL
		//=====================================
		//===================================================
		// Abre la sección para consultar y registrar ventas.
		//===================================================
		private void btnVentasVentas_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 1;
			pnlMenuInAdmin.Visible = false;
		}

		//====================================================
		// Abre la sección para buscar y administrar clientes.
		//====================================================
		private void btnClientesVentas_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 2;
			pnlMenuInAdmin.Visible = false;
		}

		//=======================================================
		// Abre la sección para consultar y administrar partidos.
		//=======================================================
		private void btnPartidosVentas_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 3;
			pnlMenuInAdmin.Visible = false;
		}

		//=======================================================
		// Abre la sección para buscar ventas y generar facturas.
		//=======================================================
		private void btnFacturasVentas_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 4;
			pnlMenuInAdmin.Visible = false;
		}

		//=============================
		// Abre la sección de reportes.
		//=============================
		private void btnReportesVentas_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 5;
			pnlMenuInAdmin.Visible = false;
		}

		//===============================================
		// Abre la sección de administración de usuarios.
		//===============================================
		private void btnUsuarios_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 6;
			pnlMenuInAdmin.Visible = false;
			tabCtrlUsauriosInAdmin.SelectedIndex = 0;
		}

		//=============================================================
		// Configura los controles y el comportamiento de estilo grids.
		//=============================================================
		private static void ConfigurarEstiloGrids(Control padre)
		{
			var fondo = Color.FromArgb(60, 60, 60);
			foreach (Control control in padre.Controls)
			{
				if (control is DataGridView grid)
				{
					grid.BackgroundColor = fondo;
					grid.GridColor = Color.FromArgb(90, 90, 90);
					grid.EnableHeadersVisualStyles = false;
					grid.DefaultCellStyle.BackColor = fondo;
					grid.DefaultCellStyle.ForeColor = Color.White;
					grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 90, 160);
					grid.DefaultCellStyle.SelectionForeColor = Color.White;
					grid.RowsDefaultCellStyle.BackColor = fondo;
					grid.RowsDefaultCellStyle.ForeColor = Color.White;
					grid.AlternatingRowsDefaultCellStyle.BackColor = fondo;
					grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.White;
					grid.ColumnHeadersDefaultCellStyle.BackColor = fondo;
					grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
					grid.RowHeadersDefaultCellStyle.BackColor = fondo;
					grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
				}

				if (control.HasChildren)
					ConfigurarEstiloGrids(control);
			}
		}

		//===================================================================
		// Configura los controles y el comportamiento de grid cancelaciones.
		//===================================================================
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

		//============================================
		// Abre el formulario para cancelar una venta.
		//============================================
		private async void btnCancelacion_Click(object? sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 7;
			pnlMenuInAdmin.Visible = false;
			await CargarVentasCancelablesAsync();
		}

		//=================================================================================
		// FILTRA LAS VENTAS POR FOLIO, RFC, CLIENTE O ESTADO
		// Actualiza la búsqueda o los datos relacionados cuando cambia el texto del campo.
		//=================================================================================
		private async void txtBuscarCancelarInAdmin_TextChanged(object? sender, EventArgs e)
		{
			await CargarVentasCancelablesAsync();
		}

		//===================================================================
		// Consulta y carga la información necesaria para ventas cancelables.
		//===================================================================
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
				var ventas = (await RepoConsulta.VentasAsync(db)).OrderByDescending(v => v.FechaHoraVenta).ToList();
				var cancelaciones = await RepoConsulta.CancelacionesAsync(db);
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

		//=======================================================================================
		// Registra la cancelación de la venta seleccionada y actualiza la lista de cancelaciones.
		//=======================================================================================
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

		//========================================================
		// Cierra la sesión del administrador y regresa al acceso.
		//========================================================
		private void btnSalir_Click(object sender, EventArgs e)
		{
			pnlMenuInAdmin.Visible = false;
			InicioSesion ventanaActual = new InicioSesion();
			ventanaActual.Show();
			this.Close();
		}

		//==============================================
		// Abre la sección de ajustes de administración.
		//==============================================
		private void btnAjustesInAdmin_Click(object sender, EventArgs e)
		{
			pnlMenuInAdmin.Visible = !pnlMenuInAdmin.Visible;
			if (pnlMenuInAdmin.Visible)
			{
				pnlMenuInAdmin.BringToFront();
			}
		}

		//===================================================
		// Abre el formulario para capturar un cliente nuevo.
		//===================================================
		private void btnNuevoClienteInCliente_Click(object sender, EventArgs e)
		{
			tabClientesInAdmin.SelectedIndex = 1;
		}

		//==========================================
		// Abre la edición del cliente seleccionado.
		//==========================================
		private void btnEditarCliente_Click(object sender, EventArgs e)
		{
			tabClientesInAdmin.SelectedIndex = 2;
		}

		//=================================================
		// Abre la pestaña para registrar un cliente nuevo.
		//=================================================
		private void button2_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 2;
			tabClientesInAdmin.SelectedIndex = 1;
		}

		//=======================================================
		// Valida el cliente y avanza a la selección del partido.
		//=======================================================
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

			_clienteParaVenta = (await RepoConsulta.ClientesAsync(db)).SingleOrDefault(c => c.IdCliente == idCliente);

			if (_clienteParaVenta is null)
			{
				MessageBox.Show("No se encontró el cliente.");
				return;
			}

			await CargarGridPartidosAsync();
			tabCtrlVenta.SelectedIndex = 1;
		}

		//==========================================
		// Regresa al paso de selección del cliente.
		//==========================================
		private void btnAtrasVentaPaso2_Click(object sender, EventArgs e)
		{
			tabCtrlVenta.SelectedIndex = 0;
		}

		//===================================================
		// Valida el partido y avanza a la selección de zona.
		//===================================================
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

			_partidoParaVenta = (await RepoConsulta.PartidosAsync(db)).SingleOrDefault(p => p.IdPartido == idPartido);

			if (_partidoParaVenta is null)
			{
				MessageBox.Show("No se encontró el partido.");
				return;
			}

			lblInfoPartidoVentaPaso3.Text =
				$"{_partidoParaVenta.EquipoLocal} vs. {_partidoParaVenta.EquipoVisitante}";
			lblInfoEstadioVentaPaso3.Text = _partidoParaVenta.Estadio;
			lblFechaVentaPaso3.Text = _partidoParaVenta.FechaHora.ToString("dd/MM/yyyy HH:mm");

			var zonas = (await RepoConsulta.DisponibilidadZonasAsync(db))
				.Where(z => z.IdPartido == idPartido && z.BoletosDisponibles > 0)
				.OrderBy(z => z.NombreZona)
				.ToList();
			cboZonaVentaPaso3.DataSource = null;
			_zonaParaVenta = null;
			_asientosSeleccionados.Clear();
			_asientosOcupados.Clear();
			dgvAsientosVentaPaso3.Rows.Clear();

			cboZonaVentaPaso3.DataSource = zonas;
			cboZonaVentaPaso3.DisplayMember = "NombreZona";
			cboZonaVentaPaso3.ValueMember = "IdZona";
			if (zonas.Count == 0)
			{
				ActualizarTotalesVenta();
				MessageBox.Show("Este partido no tiene zonas con boletos disponibles.");
				return;
			}

			cboZonaVentaPaso3.SelectedIndex = 0;

			tabCtrlVenta.SelectedIndex = 2;
		}

		//==========================================
		// Regresa al paso de selección del partido.
		//==========================================
		private void btnAtrasVentaPaso3_Click(object sender, EventArgs e)
		{
			tabCtrlVenta.SelectedIndex = 1;
		}

		//====================================================
		// Valida los asientos seleccionados y avanza al pago.
		//====================================================
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

		//==========================================
		// Regresa al paso de selección de asientos.
		//==========================================
		private void btnAtrasVentaPaso4_Click(object sender, EventArgs e)
		{
			tabCtrlVenta.SelectedIndex = 2;
		}

		//==========================================================
		// Registra la venta confirmada y genera los boletos en PDF.
		//==========================================================
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

				// No se conservan datos sensibles en el formulario.
				txtTitularVentaPaso4.Clear();
				txtNumeroTarjetaVentaPaso4.Clear();
				txtVencimientoTarjetaVentaPaso4.Clear();
				txtCVVVentaPaso4.Clear();
			}
		}

		//====================================================
		// Genera el PDF con el boleto de la venta confirmada.
		//====================================================
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

		//=======================================================
		// Identifica la marca de tarjeta a partir de su prefijo.
		//=======================================================
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

		//===========================================================
		// Valida el número de tarjeta mediante el algoritmo de Luhn.
		//===========================================================
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

		//==================================================
		// Restablece el formulario para iniciar otra venta.
		//==================================================
		private void btnNuevaVentaVentaRealizada_Click(object sender, EventArgs e)
		{
			tabCtrlVenta.SelectedIndex = 0;
		}

		//==============================================
		// Abre la factura de la venta recién realizada.
		//==============================================
		private void btnFacturarVentaRealizada_Click(object sender, EventArgs e)
		{
			tabCtrlInAdmin.SelectedIndex = 4;                                                                  // Cambio a la quinta pestaña, donde se encuentra la sección de facturas.	
		}

		//============================================
		// Abre el formulario para agregar un usuario.
		//============================================
		private void button11_Click(object sender, EventArgs e)
		{
			tabCtrlUsauriosInAdmin.SelectedIndex = 1;                                                              // Cambio a la segunda pestaña de la sección de usuarios.
		}

		//=========================================================
		// Consulta y carga la información necesaria para usuarios.
		//=========================================================
		private async Task CargarUsuariosAsync()
		{
			string filtro = textBox29.Text.Trim();
			using var db = new AppDbContext();
			var usuarios = (await RepoConsulta.UsuariosAsync(db))
				.Where(u => filtro.Length == 0 || u.ApPaterno.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
					u.CorreoElectronico.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
					u.NumeroNomina.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
					u.NombreUsuario.Contains(filtro, StringComparison.OrdinalIgnoreCase))
				.OrderBy(u => u.ApPaterno).ThenBy(u => u.Nombres).ToList();
			dataGridView3.Rows.Clear();
			foreach (var u in usuarios)
				dataGridView3.Rows.Add(u.IdUsuario, $"{u.Nombres} {u.ApPaterno} {u.ApMaterno}".Trim(), u.NombreUsuario,
					u.CorreoElectronico, u.Activo ? "Activo" : "Inactivo");
		}

		//======================================================================
		// Vuelve a consultar los usuarios mientras cambia el texto de búsqueda.
		//======================================================================
		private async void textBox29_TextChanged(object? sender, EventArgs e)
		{
			try { await CargarUsuariosAsync(); }
			catch (Exception ex) { MessageBox.Show("No se pudo buscar usuarios.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
		}

		//===================================================
		// Cancela la captura de factura y limpia los campos.
		//===================================================
		private void btnCancelarInFactura_Click(object? sender, EventArgs e)
		{
			_versionConsultaFactura++;
			txtCodigoVentaInFactura.Clear();
			LimpiarInformacionFactura();
		}

		//===========================================================
		// Limpia los datos de la factura y restablece el formulario.
		//===========================================================
		private void LimpiarInformacionFactura(string mensajeDetalle = "DETALLE DE COMPRA")
		{
			lblClienteInFactura.Text = "CLIENTE";
			lblFechaInFactura.Text = "DD/MM/AA";
			lblTotalInFactura.Text = "TOTAL";
			lblSubtotalInFactura.Text = "SUBTOTAL";
			lblIVAInFactura.Text = "IVA";
			lblMetodoPagoInFactura.Text = "MÉTODO DE PAGO";
			lblDetallesInFactura.Text = mensajeDetalle;
			lblPartidoInFactura.Text = "PARTIDO";
			lblInfoEstadioInFactura.Text = "◻️ ESTADIO";
			lblZonaInFactura.Text = "◻️ ZONA";
			lblAsientosInFactura.Text = "ASIENTOS";
			lblPrecioInFactura.Text = "PRECIO";
		}

		//==========================================================
		// Busca la venta mientras se captura o actualiza su código.
		//==========================================================
		private async void txtCodigoVentaInFactura_TextChanged(object? sender, EventArgs e)
		{
			int version = ++_versionConsultaFactura;
			LimpiarInformacionFactura();
			if (!int.TryParse(txtCodigoVentaInFactura.Text.Trim(), out int idVenta) || idVenta <= 0)
				return;

			try
			{
				await Task.Delay(300);
				if (version == _versionConsultaFactura)
					await CargarInformacionFacturaAsync(idVenta, version);
			}
			catch (Exception ex)
			{
				if (version == _versionConsultaFactura)
					LimpiarInformacionFactura($"NO SE PUDO CONSULTAR: {ex.Message}");
			}
		}

		//===============================================================
		// Busca la venta y carga su información para generar la factura.
		//===============================================================
		private async Task<bool> CargarInformacionFacturaAsync(int idVenta, int version)
		{
			try
			{
				using var db = new AppDbContext();
				var factura = (await RepoConsulta.FacturasAsync(db)).SingleOrDefault(f => f.IdVenta == idVenta);
				if (version != _versionConsultaFactura)
					return false;
				if (factura is null)
				{
					LimpiarInformacionFactura("NO EXISTE FACTURA PARA ESA VENTA");
					return false;
				}
				var boletos = (await RepoConsulta.BoletosVendidosAsync(db)).Where(b => b.IdVenta == idVenta && b.EstadoBoleto == "Vendido").ToList();
				if (version != _versionConsultaFactura)
					return false;
				lblClienteInFactura.Text = $"CLIENTE: {factura.Cliente}";
				lblFechaInFactura.Text = factura.FechaHoraEmision.ToString("dd/MM/yyyy HH:mm");
				lblTotalInFactura.Text = $"TOTAL: {factura.Total:C2}";
				lblSubtotalInFactura.Text = $"SUBTOTAL: {factura.Subtotal:C2}";
				lblIVAInFactura.Text = $"IVA: {factura.IVA:C2}";
				lblMetodoPagoInFactura.Text = $"MÉTODO DE PAGO: {factura.MetodoPago}";
				lblDetallesInFactura.Text = $"VENTA {factura.IdVenta} | {boletos.Count} BOLETO(S) | FOLIO {factura.FolioFactura}";
				if (boletos.Count > 0)
				{
					var b = boletos[0];
					lblPartidoInFactura.Text = $"{b.EquipoLocal} vs. {b.EquipoVisitante} | {b.FechaPartido:dd/MM/yyyy HH:mm}";
					lblInfoEstadioInFactura.Text = b.Estadio;
					lblZonaInFactura.Text = b.NombreZona;
					lblAsientosInFactura.Text = string.Join(", ", boletos.Select(x => $"{x.Fila}-{x.Asiento}"));
					lblPrecioInFactura.Text = b.PrecioUnitario.ToString("C2");
				}
				return true;
			}
			catch (Exception ex)
			{
				if (version == _versionConsultaFactura)
					LimpiarInformacionFactura($"NO SE PUDO CONSULTAR: {ex.Message}");
				return false;
			}
		}

		//=========================================================
		// Valida los datos fiscales y genera el PDF de la factura.
		//=========================================================
		private async void btnGenerarFacturaInFactura_Click(object? sender, EventArgs e)
		{
			if (!int.TryParse(txtCodigoVentaInFactura.Text.Trim(), out int idVenta) || idVenta <= 0)
			{ MessageBox.Show("Captura el número de venta para exportar su factura.", "Factura", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
			int version = ++_versionConsultaFactura;
			if (!await CargarInformacionFacturaAsync(idVenta, version))
			{
				MessageBox.Show("No se encontró una factura asociada a ese número de venta.", "Factura", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			using var dialogo = new SaveFileDialog { Title = "Guardar factura de venta", Filter = "Archivo PDF (*.pdf)|*.pdf", FileName = $"Factura_Venta_{idVenta}.pdf", DefaultExt = "pdf", AddExtension = true, OverwritePrompt = true };
			if (dialogo.ShowDialog(this) != DialogResult.OK) return;
			try
			{
				using var db = new AppDbContext();
				var factura = (await RepoConsulta.FacturasAsync(db)).SingleOrDefault(f => f.IdVenta == idVenta);
				if (factura is null) throw new InvalidOperationException("No se encontró la factura asociada a esa venta.");
				var boletos = (await RepoConsulta.BoletosVendidosAsync(db)).Where(b => b.IdVenta == idVenta).ToList();
				var lineas = new List<string> {
					"FACTURA DE VENTA", $"Folio de factura: {factura.FolioFactura}",
					$"Folio de compra: {factura.FolioVenta}", $"Fecha y hora de emisión: {factura.FechaHoraEmision:dd/MM/yyyy HH:mm}",
					$"Emisor: {factura.EmisorNombre} | RFC: {factura.EmisorRFC} | Domicilio: {factura.EmisorDomicilio}",
					$"Receptor: {factura.ReceptorNombre} | RFC: {factura.ReceptorRFC} | Domicilio: {factura.ReceptorDomicilio}",
					$"Método de pago: {factura.MetodoPago}", "DETALLE DE BOLETOS:" };
				lineas.AddRange(boletos.Select(b => $"{b.EquipoLocal} vs. {b.EquipoVisitante} | {b.FechaPartido:dd/MM/yyyy HH:mm} | {b.Estadio} | {b.NombreZona} | Asiento {b.Fila}-{b.Asiento} | {b.PrecioUnitario:C2}"));
				lineas.Add($"Cantidad: {boletos.Count}"); lineas.Add($"Subtotal: {factura.Subtotal:C2}");
				lineas.Add($"IVA: {factura.IVA:C2}"); lineas.Add($"Total: {factura.Total:C2}");
				GeneradorPdfBasico.Guardar(dialogo.FileName, "FACTURA DE VENTA", lineas);
				MessageBox.Show("Factura exportada correctamente.", "Factura exportada", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex) { MessageBox.Show("No se pudo exportar la factura.\n" + ex.Message, "Factura", MessageBoxButtons.OK, MessageBoxIcon.Error); }
			finally
			{
				_versionConsultaFactura++;
				txtCodigoVentaInFactura.Clear();
				LimpiarInformacionFactura();
			}
		}

		//======================================================================
		// Actualiza la información de factura al cambiar la venta seleccionada.
		//======================================================================
		private void dataGridView3_SelectionChanged(object? sender, EventArgs e)
		{
			if (dataGridView3.CurrentRow?.Cells["IDUSUARIO"].Value is int id)
				_usuarioSeleccionadoParaEditar = id;
		}

		//======================================================================
		// Carga los datos del usuario seleccionado en el formulario de edición.
		//======================================================================
		private void button9_Click(object sender, EventArgs e)
		{
			if (dataGridView3.CurrentRow?.Cells["IDUSUARIO"].Value is not int idUsuario)
			{
				MessageBox.Show("Selecciona un usuario para editar.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			_usuarioSeleccionadoParaEditar = idUsuario;
			label67.Text = Convert.ToString(dataGridView3.CurrentRow.Cells["NOMBRECOMPLETO"].Value) ?? string.Empty;
			label55.Text = Convert.ToString(dataGridView3.CurrentRow.Cells["NOMBREUSUARIO"].Value) ?? string.Empty;
			textBox1.Clear();
			comboBox3.SelectedIndex = -1;
			tabCtrlUsauriosInAdmin.SelectedIndex = 2;                                                              // Cambio a la tercera pestaña de la sección de usuarios.
		}

		//====================================
		// Valida y registra un usuario nuevo.
		//====================================
		private async void button13_Click(object sender, EventArgs e)
		{
			string nombres = textBox43.Text.Trim();
			string paterno = textBox42.Text.Trim();
			string materno = textBox41.Text.Trim();
			string nomina = textBox40.Text.Trim();
			string correo = textBox39.Text.Trim();
			string rol = textBox38.Text.Trim();
			string usuarioNombre = textBox37.Text.Trim();
			string password = textBox30.Text;
			if (new[] { nombres, paterno, nomina, correo, rol, usuarioNombre, password }.Any(string.IsNullOrWhiteSpace) || !PoliticaContrasena.EsValida(password) ||
				!DateTime.TryParse(textBox36.Text.Trim(), CultureInfo.GetCultureInfo("es-MX"), DateTimeStyles.None, out var fechaNacimiento) ||
				fechaNacimiento.Date >= DateTime.Today || !Regex.IsMatch(correo, @"^[^\s@]+@[^\s@]+\.[^\s@]+$") ||
				(!rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase) && !rol.Equals("Operativo", StringComparison.OrdinalIgnoreCase)))
			{
				MessageBox.Show($"Completa los campos; {PoliticaContrasena.Mensaje} Verifica también la fecha, el correo y el rol.", "Datos inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			try
			{
				rol = rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ? "Administrador" : "Operativo";
				var nuevo = new Usuario { Nombres = nombres, ApPaterno = paterno, ApMaterno = string.IsNullOrWhiteSpace(materno) ? null : materno,
					FechaNacimiento = fechaNacimiento.Date, NumeroNomina = nomina, CorreoElectronico = correo, Rol = rol,
					NombreUsuario = usuarioNombre, Activo = true, CreadoPor = Sesion.IdUsuario, FechaRegistro = DateTime.Now };
				nuevo.Contraseña = PasswordHasher.HashPassword(nuevo, password);
				await new RepoUsuario().AgregarAsync(nuevo);
				foreach (var control in new Control[] { textBox43, textBox42, textBox41, textBox40, textBox39, textBox38, textBox37, textBox36, textBox30 }) control.Text = string.Empty;
				await CargarUsuariosAsync();
				MessageBox.Show("Usuario agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
				tabCtrlUsauriosInAdmin.SelectedIndex = 0;
			}
			catch (Exception ex) { MessageBox.Show("No se pudo guardar el usuario.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
		}

		//====================================================
		// Valida y guarda el cambio del usuario seleccionado.
		//====================================================
		private async void button14_Click(object sender, EventArgs e)
		{
			if (_usuarioSeleccionadoParaEditar is not int id || comboBox3.SelectedItem is not string campo || string.IsNullOrWhiteSpace(textBox1.Text))
			{ MessageBox.Show("Selecciona un usuario, un campo y captura el nuevo valor.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
			string valor = textBox1.Text.Trim();
			try
			{
				string campoDb = campo;
				switch (campo)
				{
					case "CORREO ELECTRONICO": if (!Regex.IsMatch(valor, @"^[^\s@]+@[^\s@]+\.[^\s@]+$")) throw new InvalidOperationException("Captura un correo válido."); break;
					case "ROL": if (!valor.Equals("Administrador", StringComparison.OrdinalIgnoreCase) && !valor.Equals("Operativo", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("El rol debe ser Administrador u Operativo."); valor = valor.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ? "Administrador" : "Operativo"; break;
					case "NOMBRE DE USUARIO": break;
					case "CONTRASEÑA": if (!PoliticaContrasena.EsValida(valor)) throw new InvalidOperationException(PoliticaContrasena.Mensaje); var usuarioTemporal = new Usuario { IdUsuario = id }; valor = PasswordHasher.HashPassword(usuarioTemporal, valor); break;
					case "ACTIVO": if (!bool.TryParse(valor, out bool activo)) throw new InvalidOperationException("Para Activo captura true o false."); valor = activo ? "1" : "0"; break;
					default: throw new InvalidOperationException("Campo no válido.");
				}
				await new RepoUsuario().ActualizarAsync(id, campoDb, valor, Sesion.IdUsuario);
				textBox1.Clear(); await CargarUsuariosAsync();
				MessageBox.Show("Usuario editado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
				tabCtrlUsauriosInAdmin.SelectedIndex = 0;
			}
			catch (Exception ex) { MessageBox.Show("No se pudo editar el usuario.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
		}

		//=============================================
		// Abre el formulario para capturar un cliente.
		//=============================================
		private async void btnAgregarClienteInAdmin_Click(object sender, EventArgs e)
		{
			string rfc = txtRFCClienteInAdmin.Text.Trim().ToUpperInvariant();
			string correo = txtCorreoClienteInAdmin.Text.Trim();
			string celular = txtCelularClienteInAdmin.Text.Trim();
			if (string.IsNullOrWhiteSpace(txtNombreClienteInAdmin.Text) ||
				string.IsNullOrWhiteSpace(txtApPaternoClienteInAdmin.Text) ||
				dateFechaNacimientoClienteinAdmin.Value.Date >= DateTime.Today ||
				!PoliticaCliente.EsRFCValido(rfc) ||
				!PoliticaCliente.EsCorreoValido(correo) ||
				!PoliticaCliente.EsTelefonoValido(celular) ||
				new[] { txtCalleClienteInAdmin.Text, txtNoExteriorCleinteInAdmin.Text, txtCPClienteInAdmin.Text,
					txtColoniaClienteInAdmin.Text, txtCiudadClienteInAdmin.Text, txtEstadoClienteInAdmin.Text,
					txtPaisClienteInAdmin.Text }.Any(string.IsNullOrWhiteSpace))
			{
				MessageBox.Show("Completa los datos obligatorios y verifica fecha, RFC, correo, celular y domicilio.");
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
						RFC = rfc,
						CorreoElectronico = correo,
						TelefonoCasa = txtTelefonoCasaInAdmin.Text,
						TelefonoCelular = celular,
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

		//========================================
		// Valida y registra el cliente capturado.
		//========================================
		private void btnGuardarClienteInAdmin_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Cliente editado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.None);
			tabClientesInAdmin.SelectedIndex = 0;
		}

		//=================================================================================
		// Actualiza la búsqueda o los datos relacionados cuando cambia el texto del campo.
		//=================================================================================
		private void txtBuscarCliente_TextChanged(object sender, EventArgs e)
		{
			CargarGridClientes();
		}

		//==============================================================
		// Consulta y carga la información necesaria para grid clientes.
		//==============================================================
		private async void CargarGridClientes()
		{
			string texto = txtBuscarCliente.Text.Trim();
			using var db = new AppDbContext();
			var clientes = (await RepoConsulta.ClientesAsync(db))
				.Where(c => string.IsNullOrEmpty(texto) || c.ApPaterno.Contains(texto, StringComparison.OrdinalIgnoreCase) || c.CorreoElectronico.Contains(texto, StringComparison.OrdinalIgnoreCase) || c.RFC.Contains(texto, StringComparison.OrdinalIgnoreCase));

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

		//==========================================================
		// Restablece los controles y datos de campos nuevo cliente.
		//==========================================================
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

		//=======================================================================
		// Actualiza los datos dependientes cuando cambia la opción seleccionada.
		//=======================================================================
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

		//=============================================
		// Guarda los cambios del cliente seleccionado.
		//=============================================
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


		//================================================================================
		// Configura los controles y el comportamiento de grid historial cliente in admin.
		//================================================================================
		private void ConfigurarGridHistorialClienteInAdmin()
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

		//==============================================================
		// Abre el historial del cliente seleccionado en administración.
		//==============================================================
		private async void btnHistorialClienteInAdmin_Click(object? sender, EventArgs e)
		{
			var fila = GridClientesInClientes.CurrentRow;
			if (fila is null || fila.IsNewRow ||
				fila.Cells["IdCliente"].Value is null)
			{
				MessageBox.Show("Selecciona un cliente del grid para consultar su historial.",
					"Historial del cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			try
			{
				int idCliente = Convert.ToInt32(fila.Cells["IdCliente"].Value);
				await MostrarHistorialClienteInAdminAsync(idCliente);
				tabClientesInAdmin.SelectedTab = tabHistorialCliente;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"No se pudo cargar el historial del cliente.\n{ex.Message}",
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//=============================================================================
		// Consulta y presenta el historial del cliente seleccionado en administración.
		//=============================================================================
		private async Task MostrarHistorialClienteInAdminAsync(int idCliente)
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

		//========================================================================
		// Regresa desde el historial a la búsqueda de clientes en administración.
		//========================================================================
		private void btnRegresarHistorialClienteInAdmin_Click(object? sender, EventArgs e)
		{
			tabClientesInAdmin.SelectedTab = BuscarCliente;
		}

		//===================================================================
		// Exporta a PDF el historial del cliente mostrado en administración.
		//===================================================================
		private void btnExportarHistorialClienteInAdmin_Click(object? sender, EventArgs e)
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
		//===================================================================
		// Maneja el clic de EditarClienteAdmin y ejecuta la acción asociada.
		//===================================================================
		private void btnEditarClienteAdmin_Click(object sender, EventArgs e)
		{
			tabClientesInAdmin.SelectedIndex = 2;
		}

		//===================================================================
		// Actualiza los datos relacionados al cambiar la selección del grid.
		//===================================================================
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


		//==============================================
		// Abre el formulario para registrar un estadio.
		//==============================================
		private async void btnAgregarEstadioInAdmin_Click(object sender, EventArgs e)
		{
			string nombre = txtNombreEstadioInAdmin.Text.Trim();
			string ciudad = txtCiudadEstadioInAdmin.Text.Trim();
			if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(ciudad) ||
				!int.TryParse(txtCapacidadEstadioInAdmin.Text.Trim(), out int capacidad) || capacidad <= 0)
			{
				MessageBox.Show("Captura el nombre, la ciudad y una capacidad total mayor que cero.",
					"Datos del estadio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				var estadio = new Estadio
				{
					Nombre = nombre,
					Ciudad = ciudad,
					CapacidadTotal = capacidad,
					CreadoPor = Sesion.IdUsuario
				};

				_idEstadioEnCaptura = await new RepoEstadio().AgregarEstadioAsync(estadio);
				_zonasAgregadas = 0;
				_capacidadEstadioEnCaptura = capacidad;
				_cupoZonasAgregado = 0;

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
	
		//==========================================================
		// Abre la captura de una zona para el estadio seleccionado.
		//==========================================================
		private async void btnAgregarZonaInAdmin_Click(object sender, EventArgs e)
		{
			if (_idEstadioEnCaptura is null)
			{
				MessageBox.Show("Primero agrega un estadio.");
				return;
			}

			if (string.IsNullOrWhiteSpace(txtNombreZonaInAdmin.Text) ||
				!decimal.TryParse(txtPrecioZonaInAdmin.Text, out decimal precio) || precio < 0 ||
				!int.TryParse(txtAsientosZonaInAdmin.Text, out int cupo) || cupo <= 0)
			{
				MessageBox.Show("Captura el nombre de la zona, un precio no negativo y un cupo mayor que cero.");
				return;
			}
			if ((long)_cupoZonasAgregado + cupo > _capacidadEstadioEnCaptura)
			{
				MessageBox.Show($"El cupo acumulado de las zonas no puede superar la capacidad del estadio ({_capacidadEstadioEnCaptura:N0}).");
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
				_cupoZonasAgregado += cupo;

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

	
		//=========================================
		// Guarda el partido y finaliza su captura.
		//=========================================
		private async void btnFinalizarAgregarPartidoInAdmin_Click(object sender, EventArgs e)
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
			_capacidadEstadioEnCaptura = 0;
			_cupoZonasAgregado = 0;

			MessageBox.Show("Registro del estadio y sus zonas finalizado.");

			tabCtrlPartidoEstadio.Visible = false;
			pnlPartidos.Size = new Size(1390, 690);
			try
			{
				using var db = new AppDbContext();
				var estadios = await new RepoEstadio().ObtenerTodosAsync();
				cboEstadioInAdmin.DataSource = null;
				cboEstadioInAdmin.DisplayMember = nameof(Estadio.Nombre);
				cboEstadioInAdmin.ValueMember = nameof(Estadio.IdEstadio);
				cboEstadioInAdmin.DataSource = estadios;
				cboEstadioInAdmin.SelectedIndex = -1;
				await CargarFiltrosReporteAsync();
				await CargarFiltrosOcupacionAsync();
				await CargarPartidos();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "No se actualizaron los filtros del estadio",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		//==============================================
		// Abre el formulario para registrar un partido.
		//==============================================
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
			if (string.Equals(txtEquipoLocalPartidoInAdmin.Text.Trim(), txtEquipoVisitantePartidoInAdmin.Text.Trim(), StringComparison.OrdinalIgnoreCase))
			{
				MessageBox.Show("El equipo local y el visitante deben ser distintos.");
				return;
			}
			DateTime fechaHoraPartido = dteFechaPartidoInAdmin.Value.Date.Add(hora);
			if (fechaHoraPartido <= DateTime.Now)
			{
				MessageBox.Show("La fecha y hora del partido deben ser futuras.");
				return;
			}

			try
			{
				var partido = new Partido
				{
					IdEstadio = idEstadio,
					FechaHora = fechaHoraPartido,
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
				await CargarFiltrosReporteAsync();
				await CargarFiltrosOcupacionAsync();
				await CargarPartidos();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"No se pudo agregar el partido.\n{ex.Message}");
			}
		}


		//======================================
		// Valida y guarda el partido capturado.
		//======================================
		private void btnFinalizarPartidoInAdmin_Click(object sender, EventArgs e)
		{
			tabCtrlPartidoEstadio.Visible = false;
			pnlPartidos.Size = new Size(1390, 690);
		}

		//==================================
		// Confirma el registro del partido.
		//==================================
		private void btnAgregarPartido_Click(object sender, EventArgs e)
		{
			tabCtrlPartidoEstadio.Visible = true;
			tabCtrlPartidoEstadio.SelectedIndex = 0;
			pnlPartidos.Size = new Size(700, 690);
		}

		//=============================
		// Guarda el estadio capturado.
		//=============================
		private void btnAgregarEstadio_Click(object sender, EventArgs e)
		{
			tabCtrlPartidoEstadio.Visible = true;
			tabCtrlPartidoEstadio.SelectedIndex = 1;
			pnlPartidos.Size = new Size(700, 690);
		}

		//========================================================
		// Regresa de la captura de zona a la sección de estadios.
		//========================================================
		private void btnRegresarEstadioZonaInAdmin_Click(object sender, EventArgs e)
		{
			tabCtrlPartidoEstadio.Visible = false;
			pnlPartidos.Size = new Size(1390, 690);
		}


		//=================================================================================
		// Actualiza la búsqueda o los datos relacionados cuando cambia el texto del campo.
		//=================================================================================
		private async void txtBuscarEquipoVentaPaso2_TextChanged(object sender, EventArgs e)
		{
					await CargarGridPartidosAsync();
		}


		//==============================================================
		// Consulta y carga la información necesaria para grid partidos.
		//==============================================================
		private async Task CargarGridPartidosAsync()
		{
			string texto = txtBuscarEquipoVentaPaso2.Text.Trim();

			using var db = new AppDbContext();
			var partidos = (await RepoConsulta.PartidosAsync(db))
				.Where(p => (string.IsNullOrWhiteSpace(texto) || p.EquipoLocal.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
					p.EquipoVisitante.Contains(texto, StringComparison.OrdinalIgnoreCase) || p.Estadio.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
					$"{p.EquipoLocal} vs. {p.EquipoVisitante}".Contains(texto, StringComparison.OrdinalIgnoreCase)) &&
					p.Estado == "Programado" && p.FechaHora >= DateTime.Now && p.BoletosDisponibles > 0)
				.OrderBy(p => p.FechaHora).ToList();

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

		//=======================================================================
		// Actualiza los datos dependientes cuando cambia la opción seleccionada.
		//=======================================================================
		private async void cboZonaVentaPaso3_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboZonaVentaPaso3.SelectedItem is not vw_DisponibilidadZonas zona)
				return;

			_zonaParaVenta = zona;

			lblInfoPrecioVentaPaso3.Text = zona.PrecioBase.ToString("C");
			lblInfoDisponiblesVentaPaso3.Text = zona.BoletosDisponibles.ToString();
			await CargarGridAsientosAsync();

		}

		//=====================================================================
		// Configura el grid virtual y su apariencia para mostrar los asientos.
		//=====================================================================
		private void ConfigurarGridAsientos()
		{
			dgvAsientosVentaPaso3.Columns.Clear();
			dgvAsientosVentaPaso3.Rows.Clear();

			dgvAsientosVentaPaso3.AllowUserToAddRows = false;
			dgvAsientosVentaPaso3.ReadOnly = true;
			dgvAsientosVentaPaso3.MultiSelect = false;
			dgvAsientosVentaPaso3.VirtualMode = true;
			dgvAsientosVentaPaso3.SelectionMode = DataGridViewSelectionMode.CellSelect;
			dgvAsientosVentaPaso3.RowHeadersVisible = true;
			dgvAsientosVentaPaso3.RowHeadersWidth = 70;
			dgvAsientosVentaPaso3.RowHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
			dgvAsientosVentaPaso3.RowHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
			dgvAsientosVentaPaso3.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dgvAsientosVentaPaso3.AutoSizeColumnsMode =
				DataGridViewAutoSizeColumnsMode.Fill;

			for (int asiento = 1; asiento <= AsientosPorFila; asiento++)
			{
				dgvAsientosVentaPaso3.Columns.Add(
					$"Asiento{asiento:00}",
					asiento.ToString("00"));
			}

			dgvAsientosVentaPaso3.CellClick -= dgvAsientosVentaPaso3_CellClick;
			dgvAsientosVentaPaso3.CellClick += dgvAsientosVentaPaso3_CellClick;
			dgvAsientosVentaPaso3.CellValueNeeded -= dgvAsientosVentaPaso3_CellValueNeeded;
			dgvAsientosVentaPaso3.CellValueNeeded += dgvAsientosVentaPaso3_CellValueNeeded;
			dgvAsientosVentaPaso3.CellFormatting -= dgvAsientosVentaPaso3_CellFormatting;
			dgvAsientosVentaPaso3.CellFormatting += dgvAsientosVentaPaso3_CellFormatting;
			dgvAsientosVentaPaso3.RowPostPaint -= dgvAsientosVentaPaso3_RowPostPaint;
			dgvAsientosVentaPaso3.RowPostPaint += dgvAsientosVentaPaso3_RowPostPaint;
		}

		//=============================================================================
		// Carga de forma asíncrona la disponibilidad de asientos para la zona elegida.
		//=============================================================================
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
				// Permite que WinForms pinte el mensaje antes de iniciar la consulta.
				await Task.Yield();
				_asientosSeleccionados.Clear();
				_asientosOcupados.Clear();
				ConfigurarGridAsientos();

				using var db = new AppDbContext();
				var ocupados = (await RepoConsulta.AsientosOcupadosAsync(db))
					.Where(a => a.IdPartido == idPartido && a.IdZona == zona.IdZona)
					.Select(a => new { a.Fila, a.Asiento }).ToList();

				if (version != _versionCargaAsientos || _partidoParaVenta?.IdPartido != idPartido || _zonaParaVenta?.IdZona != zona.IdZona)
					return;

				foreach (var asiento in ocupados)
					_asientosOcupados.Add($"{asiento.Fila.Trim()}-{asiento.Asiento.Trim()}");

				int cantidadFilas = (zona.CupoZona + AsientosPorFila - 1) / AsientosPorFila;
				dgvAsientosVentaPaso3.RowCount = cantidadFilas;
				dgvAsientosVentaPaso3.Invalidate();
				dgvAsientosVentaPaso3.Refresh();
				lblBoletosSeleccionadosVentaPaso3.Text = "0";
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
				dgvAsientosVentaPaso3.Controls.Add(_lblCargandoAsientos);
			}

			_lblCargandoAsientos.Visible = visible;
			if (visible)
				_lblCargandoAsientos.BringToFront();
		}

		//==============================================================
		// Proporciona al grid virtual el valor de cada asiento visible.
		//==============================================================
		private void dgvAsientosVentaPaso3_CellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
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
		private void dgvAsientosVentaPaso3_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
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
		private void dgvAsientosVentaPaso3_RowPostPaint(object? sender, DataGridViewRowPostPaintEventArgs e)
		{
			string encabezado = $"F{e.RowIndex + 1:D3}";
			var bounds = new System.Drawing.Rectangle(e.RowBounds.Left, e.RowBounds.Top,
				dgvAsientosVentaPaso3.RowHeadersWidth, e.RowBounds.Height);
			TextRenderer.DrawText(e.Graphics, encabezado, dgvAsientosVentaPaso3.Font, bounds,
				System.Drawing.Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
		}

		//======================================================================
		// Actualiza la selección de asientos cuando el usuario pulsa una celda.
		//======================================================================
		private void dgvAsientosVentaPaso3_CellClick(
	object? sender,
	DataGridViewCellEventArgs e)
		{
			// Ignora encabezados y clics fuera de las celdas
			if (e.RowIndex < 0 || e.ColumnIndex < 0)
				return;

			if (_zonaParaVenta is null || e.RowIndex * AsientosPorFila + e.ColumnIndex >= _zonaParaVenta.CupoZona)
				return;
			string clave = $"{e.RowIndex + 1:D3}-{e.ColumnIndex + 1:D2}";

			// Los asientos ocupados no se pueden seleccionar
			if (_asientosOcupados.Contains(clave))
				return;

			if (_asientosSeleccionados.Contains(clave))
			{
				_asientosSeleccionados.Remove(clave);
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
			}

			dgvAsientosVentaPaso3.InvalidateCell(e.ColumnIndex, e.RowIndex);
			ActualizarTotalesVenta();
		}

		//====================================================
		// Recalcula subtotal, IVA y total de la venta actual.
		//====================================================
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

		//=============================================================
		// Prepara el resumen y los controles del último paso de venta.
		//=============================================================
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

		//=========================================
		// CLASE PARA FILTRO DE PARTIDOS EN REPORTE
		//=========================================
		private sealed class FiltroPartidoReporte
		{
			public int? IdPartido { get; set; }
			public string Descripcion { get; set; } = "";
		}

		//================================================================
		// Consulta y carga la información necesaria para filtros reporte.
		//================================================================
		private async Task CargarFiltrosReporteAsync()
		{
			_cargandoFiltrosReporte = true;
			try
			{
				using var db = new AppDbContext();
				var estadios = (await RepoConsulta.EstadiosAsync(db))
					.Select(e => e.Nombre).Distinct().OrderBy(e => e).ToList();
				estadios.Insert(0, "Todos");
				cboEstadioReporteInOper.DataSource = null;
				cboEstadioReporteInOper.DataSource = estadios;
				cboEstadioReporteInOper.SelectedIndex = 0;
				dateTimePicker1.Value = DateTime.Today.AddDays(-30);
				dateTimePicker2.Value = DateTime.Today;
			}
			finally
			{
				_cargandoFiltrosReporte = false;
			}

			await CargarPartidosFiltroReporteAsync("Todos");
		}


		//=======================================================================
		// Actualiza los datos dependientes cuando cambia la opción seleccionada.
		//=======================================================================
		private async void cboEstadioReporteInOper_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (_cargandoFiltrosReporte)
				return;
			await CargarPartidosFiltroReporteAsync(cboEstadioReporteInOper.SelectedItem?.ToString());
		}

		//========================================================================
		// Consulta y carga la información necesaria para partidos filtro reporte.
		//========================================================================
		private async Task CargarPartidosFiltroReporteAsync(string? estadio)
		{
			_cargandoFiltrosReporte = true;
			try
			{
				using var db = new AppDbContext();
				var partidos = (await RepoConsulta.PartidosAsync(db))
					.Where(p => string.IsNullOrWhiteSpace(estadio) || estadio == "Todos" || p.Estadio == estadio)
					.OrderBy(p => p.FechaHora)
					.Select(p => new FiltroPartidoReporte
					{
						IdPartido = p.IdPartido,
						Descripcion = $"{p.EquipoLocal} vs. {p.EquipoVisitante} | {p.FechaHora:dd/MM/yyyy HH:mm}"
					})
					.ToList();
				partidos.Insert(0, new FiltroPartidoReporte { IdPartido = null, Descripcion = "Todos" });
				cboPartidoReporteInOper.DataSource = null;
				cboPartidoReporteInOper.DisplayMember = nameof(FiltroPartidoReporte.Descripcion);
				cboPartidoReporteInOper.ValueMember = nameof(FiltroPartidoReporte.IdPartido);
				cboPartidoReporteInOper.DataSource = partidos;
				cboPartidoReporteInOper.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "No se pudieron cargar los partidos del filtro",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				_cargandoFiltrosReporte = false;
			}
		}

		//============================================================
		// Aplica los filtros de fecha y estadio al reporte de ventas.
		//============================================================
		private async void btnFiltrarReporte_Click_1(object sender, EventArgs e)
		{
			DateTime desde = dateTimePicker1.Value.Date;
			DateTime hasta = dateTimePicker2.Value.Date;

			if (desde > hasta)
			{
				MessageBox.Show("La fecha Desde no puede ser posterior a Hasta.");
				return;
			}

			try
			{
				using var db = new AppDbContext();

				var consulta = (await RepoConsulta.BoletosVendidosAsync(db))
					.Where(b =>
						b.FechaHoraVenta >= desde &&
						b.FechaHoraVenta < hasta.AddDays(1) &&
						b.EstadoVenta == "Activa" &&
						b.EstadoBoleto == "Vendido");

				if (cboEstadioReporteInOper.SelectedItem is string estadio &&
					estadio != "Todos")
				{
					consulta = consulta.Where(b => b.Estadio == estadio);
				}

				if (cboPartidoReporteInOper.SelectedValue is int idPartido)
				{
					consulta = consulta.Where(b => b.IdPartido == idPartido);
				}

				var boletos = consulta.ToList();

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
							FechaVenta = g.Min(b => b.FechaHoraVenta),
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

				gridReporteInOper.Rows.Clear();

				foreach (var fila in filas)
				{
					gridReporteInOper.Rows.Add(
						fila.Folio.ToString(),
						fila.FechaVenta.ToString("dd/MM/yyyy HH:mm"),
						fila.Estadio,
						fila.Partido,
						fila.Zona,
						fila.Cantidad,
						fila.Subtotal,
						fila.Total);
				}

				int cantidadBoletos = filas.Sum(f => f.Cantidad);
				decimal ingresos = filas.Sum(f => f.Total);

				lblBoletosVendidosReportesInOper.Text =
					$"BOLETOS VENDIDOS: {cantidadBoletos}";

				lblIngresosReportesInOper.Text =
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

		//====================================================================
		// Configura los controles y el comportamiento de grid reporte ventas.
		//====================================================================
		private void ConfigurarGridReporteVentas()
		{
			gridReporteInOper.Columns.Clear();
			gridReporteInOper.AutoGenerateColumns = false;
			gridReporteInOper.AllowUserToAddRows = false;
			gridReporteInOper.ReadOnly = true;
			gridReporteInOper.RowHeadersVisible = false;
			gridReporteInOper.AutoSizeColumnsMode =
				DataGridViewAutoSizeColumnsMode.Fill;

			gridReporteInOper.Columns.Add("Folio", "FOLIO");
			gridReporteInOper.Columns.Add("FechaVenta", "FECHA DE VENTA");
			gridReporteInOper.Columns.Add("Estadio", "ESTADIO");
			gridReporteInOper.Columns.Add("Partido", "PARTIDO");
			gridReporteInOper.Columns.Add("Zona", "ZONA");
			gridReporteInOper.Columns.Add("Cantidad", "CANTIDAD");
			gridReporteInOper.Columns.Add("Subtotal", "SUBTOTAL");
			gridReporteInOper.Columns.Add("Total", "TOTAL");

			gridReporteInOper.Columns["Subtotal"]!
				.DefaultCellStyle.Format = "C2";

			gridReporteInOper.Columns["Total"]!
				.DefaultCellStyle.Format = "C2";
		}

		//==============================================
		// Restablece los filtros del reporte de ventas.
		//==============================================
		private void button4_Click(object sender, EventArgs e)
		{
			dateTimePicker1.Value = DateTime.Today;
			dateTimePicker2.Value = DateTime.Today;

			if (cboEstadioReporteInOper.Items.Count > 0)
				cboEstadioReporteInOper.SelectedIndex = 0; // "Todos"

			if (cboPartidoReporteInOper.Items.Count > 0)
				cboPartidoReporteInOper.SelectedIndex = 0; // "Todos"

			gridReporteInOper.Rows.Clear();

			lblBoletosVendidosReportesInOper.Text = "BOLETOS VENDIDOS: 0";
			lblIngresosReportesInOper.Text = "INGRESOS: $0.00";
		}

		//============================================
		// Exporta a PDF el reporte de ventas visible.
		//============================================
		private void button6_Click(object? sender, EventArgs e)
		{
			if (gridReporteInOper.Rows.Count == 0)
			{
				MessageBox.Show("No hay datos en el reporte para exportar.", "Reporte vacío",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			using var dialogo = new SaveFileDialog
			{
				Title = "Exportar reporte de ventas",
				Filter = "Archivo PDF (*.pdf)|*.pdf",
				FileName = $"ReporteVentas_Admin_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
				AddExtension = true,
				DefaultExt = "pdf",
				OverwritePrompt = true
			};

			if (dialogo.ShowDialog(this) != DialogResult.OK)
				return;

			var lineas = new List<string>
			{
				$"Periodo: {dateTimePicker1.Value:dd/MM/yyyy} al {dateTimePicker2.Value:dd/MM/yyyy}",
				$"Estadio: {cboEstadioReporteInOper.SelectedItem ?? "Todos"}",
				$"Partido: {(cboPartidoReporteInOper.SelectedItem as FiltroPartidoReporte)?.Descripcion ?? "Todos"}",
				lblBoletosVendidosReportesInOper.Text,
				lblIngresosReportesInOper.Text,
				string.Empty,
				"DETALLE DEL REPORTE DE VENTAS"
			};

			foreach (DataGridViewRow fila in gridReporteInOper.Rows)
			{
				if (fila.IsNewRow)
					continue;

				string Valor(int indice) => Convert.ToString(fila.Cells[indice].Value) ?? string.Empty;
				lineas.Add($"Folio: {Valor(0)} | Fecha: {Valor(1)}");
				lineas.Add($"Estadio: {Valor(2)} | Partido: {Valor(3)}");
				lineas.Add($"Zona: {Valor(4)} | Boletos: {Valor(5)} | Subtotal: {Valor(6)} | Total: {Valor(7)}");
				lineas.Add(string.Empty);
			}

			try
			{
				GeneradorPdfBasico.Guardar(dialogo.FileName, "REPORTE DE VENTAS - ADMINISTRACION", lineas);
				MessageBox.Show("El reporte PDF se guardó correctamente.", "Reporte exportado",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"No se pudo guardar el reporte PDF.\n{ex.Message}",
					"Error al exportar", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

	}

}