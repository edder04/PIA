
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

	// Vistas para mostrar información de Cliente
	public class vw_ConsultaCliente
	{
    public int IdCliente { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Domicilio { get; set; } = string.Empty;
    public string RFC { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string? TelefonoCasa { get; set; }
    public string? Celular { get; set; }
    public bool Activo { get; set; }
    }

	// Vistas para mostrar información de Usuario
	public class vw_ConsultaUsuario{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string NumeroNomina { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string rol { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public bool Activo{get; set;}
    }
}
