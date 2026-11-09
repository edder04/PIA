
using System; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIA.Datos{                                                                                    // Indico que este código pertenece al proyecto PIA.

	// Tabla de Usuario
	public class Usuario
	{
		public int IdUsuario { get; set; }
		[Column("Nombre(s)")]
		public string Nombres { get; set; } = string.Empty;
		public string ApPaterno { get; set; } = string.Empty;
		public string? ApMaterno { get; set; }
		public DateTime FechaNacimiento { get; set; }
		public string NumeroNomina { get; set; } = string.Empty;
		public string CorreoElectronico { get; set; } = string.Empty;
		public string? Rol { get; set; }
		public string NombreUsuario { get; set; } = string.Empty;
		public string Contraseña { get; set; } = string.Empty;
		public bool Activo { get; set; } = true;
		public int? CreadoPor { get; set; }
		public DateTime FechaRegistro { get; set; }
	}

	public class AuditoriaUsuario
	{
		public int IdAuditoria { get; set; }
		public string? IdUsuario { get; set; }
		public string? DatoACambiar { get; set; }
		public string? DatoAnterior { get; set; }
		public string? DatoNuevo { get; set; }
		public int ModificadoPor { get; set; }
		public DateTime FechaModificacion { get; set; }
	}

	//====================
	// Tabla de Cliente
	public class Cliente
	{
		public int IdCliente { get; set; }
		[Column("Nombre(s)")]
		public string Nombres { get; set; } = string.Empty;
		public string ApPaterno { get; set; } = string.Empty;
		public string? ApMaterno { get; set; }
		public DateTime FechaNacimiento { get; set; }
		public string Domicilio { get; set; } = string.Empty;
		public string RFC { get; set; } = string.Empty;
		public string CorreoElectronico { get; set; } = string.Empty;
		public string? TelefonoCasa { get; set; }
		public string TelefonoCelular { get; set; } = string.Empty;
		public bool Activo { get; set; } = true;
		public int CreadoPor { get; set; }
		public DateTime FechaHoraRegistro { get; set; }
	}


	public class AuditoriaCliente
	{
		public int IdAuditoria { get; set; }
		public string? IdCliente { get; set; }
		public string? DatoACambiar { get; set; }
		public string? DatoAnterior { get; set; }
		public string? DatoNuevo { get; set; }
		public int ModificadoPor { get; set; }
		public DateTime FechaModificacion { get; set; }
    }

	public class Estadio
	{
		public int IdEstadio { get; set; }
		public string Nombre { get; set; } = string.Empty;
		public string Ciudad { get; set; } = string.Empty;
		public int CapacidadTotal { get; set; }
		public string? MapaEstadio { get; set; }
		public int CreadoPor { get; set; }
		public DateTime FechaHoraRegistro { get; set; }
	}

	public class Zona
	{
		public int IdZona { get; set; }
		public int IdEstadio { get; set; }
		public string NombreZona { get; set; } = string.Empty;
		public decimal PrecioBase { get; set; }
		public int CupoZona { get; set; }
		public int CreadoPor { get; set; }
		public DateTime FechaHoraRegistro { get; set; }
	}

	public class Partido
	{
		public int IdPartido { get; set; }
		public int IdEstadio { get; set; }
		public DateTime FechaHora { get; set; }
		public string EquipoLocal { get; set; } = string.Empty;
		public string EquipoVisitante { get; set; } = string.Empty;
		public string Estado { get; set; } = "Programado";
		public int CreadoPor { get; set; }
		public DateTime FechaHoraRegistro { get; set; }
	}

	public class Venta
	{
		public int IdVenta { get; set; }
		public Guid Folio { get; set; }
		public int IdCliente { get; set; }
		public int IdUsuario { get; set; }
		public DateTime FechaHoraVenta { get; set; }
		public decimal Subtotal { get; set; }
		public decimal IVA { get; set; }
		public decimal Total { get; set; }
		public string MetodoPago { get; set; } = string.Empty;
		public string EstadoVenta { get; set; } = "Activa";
		public string? TarjetaTitular { get; set; }
		public string? TarjetaNumero { get; set; }
		public string? TarjetaExpiracion { get; set; }
		public string? TarjetaCVV { get; set; }
	}

	public class DetalleVenta
	{
		public int IdDetalleVenta { get; set; }
		public int IdVenta { get; set; }
		public int IdPartido { get; set; }
		public int IdZona { get; set; }
		public int Cantidad { get; set; }
		public decimal PrecioUnitario { get; set; }
		public decimal Subtotal { get; set; }
		public decimal IVA { get; set; }
		public decimal Total { get; set; }
	}

	public class Boleto
	{
		public int IdBoleto { get; set; }
		public int IdVenta { get; set; }
		public int IdPartido { get; set; }
		public int IdZona { get; set; }
		public string Fila { get; set; } = string.Empty;
		public string Asiento { get; set; } = string.Empty;
		public decimal PrecioUnitario { get; set; }
		public string EstadoBoleto { get; set; } = "Vendido";
	}

	public class Factura
	{
		public int IdFactura { get; set; }
		public int IdVenta { get; set; }
		public string FolioFactura { get; set; } = string.Empty;
		public DateTime FechaHoraEmision { get; set; }
		public string EmisorNombre { get; set; } = string.Empty;
		public string EmisorRFC { get; set; } = string.Empty;
		public string EmisorDomicilio { get; set; } = string.Empty;
		public string ReceptorNombre { get; set; } = string.Empty;
		public string ReceptorRFC { get; set; } = string.Empty;
		public string ReceptorDomicilio { get; set; } = string.Empty;
		public string UsoCFDI { get; set; } = string.Empty;
		public string MetodoPago { get; set; } = string.Empty;
		public decimal Subtotal { get; set; }
		public decimal IVA { get; set; }
		public decimal Total { get; set; }
	}
	public class Cancelacion
	{
		public int IdCancelacion { get; set; }
		public int IdVenta { get; set; }
		public int IdUsuario { get; set; }
		public DateTime FechaHoraCancelacion { get; set; }
		public string Motivo { get; set; } = string.Empty;
	}

	
	// Vistas para mostrar informacion de Usuarios
	public class vw_Usuarios
	{
		public int IdUsuario { get; set; }
		public string Nombres { get; set; } = string.Empty;
		public string ApPaterno { get; set; } = string.Empty;
		public string? ApMaterno { get; set; }
		public DateTime FechaNacimiento { get; set; }
		public int Edad { get; set; }
		public string NumeroNomina { get; set; } = string.Empty;
		public string CorreoElectronico { get; set; } = string.Empty;
		public string Rol { get; set; } = string.Empty;
		public string NombreUsuario { get; set; } = string.Empty;
		public bool Activo { get; set; }
		public int? CreadoPor { get; set; }
		public DateTime FechaRegistro { get; set; }
	}

	// Vistas para mostrar informacion de Clientes
	public class vw_Clientes
	{
		public int IdCliente { get; set; }
		public string Nombres { get; set; } = string.Empty;
		public string ApPaterno { get; set; } = string.Empty;
		public string? ApMaterno { get; set; }
		public DateTime FechaNacimiento { get; set; }
		public int Edad { get; set; }
		public string Domicilio { get; set; } = string.Empty;
		public string RFC { get; set; } = string.Empty;
		public string CorreoElectronico { get; set; } = string.Empty;
		public string? TelefonoCasa { get; set; }
		public string TelefonoCelular { get; set; } = string.Empty;
		public bool Activo { get; set; }
		public DateTime FechaHoraRegistro { get; set; }
		public string CreadoPorNombre { get; set; } = string.Empty;
		public string CreadoPorRol { get; set; } = string.Empty;
	}

	// Vistas para mostrar informacion de Estadios
	public class vw_Estadios
	{
		public int IdEstadio { get; set; }
		public string Nombre { get; set; } = string.Empty;
		public string Ciudad { get; set; } = string.Empty;
		public int CapacidadTotal { get; set; }
		public string? MapaEstadio { get; set; }
		public int TotalZonas { get; set; }
		public int CupoAsignado { get; set; }
		public int CapacidadDisponible { get; set; }
		public DateTime FechaHoraRegistro { get; set; }
	}

	// Vistas para mostrar informacion de Partidos
	public class vw_Partidos
	{
		public int IdPartido { get; set; }
		public DateTime FechaHora { get; set; }
		public string EquipoLocal { get; set; } = string.Empty;
		public string EquipoVisitante { get; set; } = string.Empty;
		public string Estado { get; set; } = string.Empty;
		public int IdEstadio { get; set; }
		public string Estadio { get; set; } = string.Empty;
		public string Ciudad { get; set; } = string.Empty;
		public int CapacidadTotal { get; set; }
		public int BoletosVendidos { get; set; }
		public int BoletosDisponibles { get; set; }
		public DateTime FechaHoraRegistro { get; set; }
		public string CreadoPor { get; set; } = string.Empty;
	}

	// Vistas para disponibilidad por zona
	public class vw_DisponibilidadZonas
	{
		public int IdPartido { get; set; }
		public string EquipoLocal { get; set; } = string.Empty;
		public string EquipoVisitante { get; set; } = string.Empty;
		public DateTime FechaHora { get; set; }
		public string EstadoPartido { get; set; } = string.Empty;
		public string Estadio { get; set; } = string.Empty;
		public int IdZona { get; set; }
		public string NombreZona { get; set; } = string.Empty;
		public decimal PrecioBase { get; set; }
		public int CupoZona { get; set; }
		public int BoletosVendidos { get; set; }
		public int BoletosDisponibles { get; set; }
	}

	// Vistas para el registro de Ventas
	public class vw_Ventas
	{
		public int IdVenta { get; set; }
		public Guid Folio { get; set; } // UNIQUEIDENTIFIER en SQL
		public DateTime FechaHoraVenta { get; set; }
		public int IdCliente { get; set; }
		public string Cliente { get; set; } = string.Empty;
		public string RFC { get; set; } = string.Empty;
		public string CorreoElectronico { get; set; } = string.Empty;
		public int IdVendedor { get; set; }
		public string Vendedor { get; set; } = string.Empty;
		public decimal Subtotal { get; set; }
		public decimal IVA { get; set; }
		public decimal Total { get; set; }
		public string MetodoPago { get; set; } = string.Empty;
		public string EstadoVenta { get; set; } = string.Empty;
		public int TotalBoletos { get; set; }
		public int BoletosCancelados { get; set; }
		public int Facturada { get; set; } // SQL devuelve 1 o 0
		public string? FolioFactura { get; set; }
	}

	// Vistas para Boletos Vendidos
	public class vw_BoletosVendidos
	{
		public int IdBoleto { get; set; }
		public string Fila { get; set; } = string.Empty;
		public string Asiento { get; set; } = string.Empty;
		public decimal PrecioUnitario { get; set; }
		public string EstadoBoleto { get; set; } = string.Empty;
		public int IdPartido { get; set; }
		public DateTime FechaPartido { get; set; }
		public string EquipoLocal { get; set; } = string.Empty;
		public string EquipoVisitante { get; set; } = string.Empty;
		public string Estadio { get; set; } = string.Empty;
		public string Ciudad { get; set; } = string.Empty;
		public int IdZona { get; set; }
		public string NombreZona { get; set; } = string.Empty;
		public int IdVenta { get; set; }
		public Guid FolioVenta { get; set; }
		public DateTime FechaHoraVenta { get; set; }
		public string EstadoVenta { get; set; } = string.Empty;
		public int IdCliente { get; set; }
		public string Cliente { get; set; } = string.Empty;
		public string RFC { get; set; } = string.Empty;
	}

	// Vistas para Facturas
	public class vw_Facturas
	{
		public int IdFactura { get; set; }
		public string FolioFactura { get; set; } = string.Empty;
		public DateTime FechaHoraEmision { get; set; }
		public int IdVenta { get; set; }
		public Guid FolioVenta { get; set; }
		public DateTime FechaHoraVenta { get; set; }
		public string EmisorNombre { get; set; } = string.Empty;
		public string EmisorRFC { get; set; } = string.Empty;
		public string EmisorDomicilio { get; set; } = string.Empty;
		public string ReceptorNombre { get; set; } = string.Empty;
		public string ReceptorRFC { get; set; } = string.Empty;
		public string ReceptorDomicilio { get; set; } = string.Empty;
		public string UsoCFDI { get; set; } = string.Empty;
		public string MetodoPago { get; set; } = string.Empty;
		public decimal Subtotal { get; set; }
		public decimal IVA { get; set; }
		public decimal Total { get; set; }
		public string Cliente { get; set; } = string.Empty;
		public string EstadoVenta { get; set; } = string.Empty;
	}

	// Vistas para Cancelaciones
	public class vw_Cancelaciones
	{
		public int IdCancelacion { get; set; }
		public DateTime FechaHoraCancelacion { get; set; }
		public string Motivo { get; set; } = string.Empty;
		public int IdVenta { get; set; }
		public Guid FolioVenta { get; set; }
		public DateTime FechaHoraVenta { get; set; }
		public decimal Total { get; set; }
		public int IdCliente { get; set; }
		public string Cliente { get; set; } = string.Empty;
		public string RFC { get; set; } = string.Empty;
		public int IdUsuarioCancelo { get; set; }
		public string UsuarioCancelo { get; set; } = string.Empty;
	}

	// Vistas para Ingresos por Partido
	public class vw_IngresosPorPartido
	{
		public int IdPartido { get; set; }
		public DateTime FechaHora { get; set; }
		public string EquipoLocal { get; set; } = string.Empty;
		public string EquipoVisitante { get; set; } = string.Empty;
		public string Estado { get; set; } = string.Empty;
		public string Estadio { get; set; } = string.Empty;
		public int BoletosVendidos { get; set; }
		public decimal Subtotal { get; set; }
		public decimal IVA { get; set; }
		public decimal Total { get; set; }
	}

	// Vistas para Asientos Ocupados
	public class vw_AsientosOcupados
	{
		public int IdPartido { get; set; }
		public int IdZona { get; set; }
		public string NombreZona { get; set; } = string.Empty;
		public string Fila { get; set; } = string.Empty;
		public string Asiento { get; set; } = string.Empty;
		public int IdVenta { get; set; }
	}

	// Vistas para Auditoria de Usuarios
	public class vw_AuditoriaUsuarios
	{
		public int IdAuditoria { get; set; }
		public string IdUsuario { get; set; } = string.Empty;
		public string DatoACambiar { get; set; } = string.Empty;
		public string? DatoAnterior { get; set; }
		public string? DatoNuevo { get; set; }
		public int ModificadoPor { get; set; }
		public string ModificadoPorNombre { get; set; } = string.Empty;
		public DateTime FechaModificacion { get; set; }
	}

	// Vistas para Auditoria de Clientes
	public class vw_AuditoriaClientes
	{
		public int IdAuditoria { get; set; }
		public string IdCliente { get; set; } = string.Empty;
		public string DatoACambiar { get; set; } = string.Empty;
		public string? DatoAnterior { get; set; }
		public string? DatoNuevo { get; set; }
		public int ModificadoPor { get; set; }
		public string ModificadoPorNombre { get; set; } = string.Empty;
		public DateTime FechaModificacion { get; set; }
	}
}
