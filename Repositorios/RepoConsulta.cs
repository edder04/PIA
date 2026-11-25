using Microsoft.EntityFrameworkCore;
using PIA.Datos;

namespace PIA.Repositorios;


public static class RepoConsulta
{
	//============================================================================
	// Busca un usuario por correo usando el procedimiento almacenado de consulta.
	//============================================================================
	public static async Task<Usuario?> UsuarioPorCorreoAsync(AppDbContext db, string correo)
	{
		// SQL Server no permite que EF componga SingleOrDefault sobre EXEC.
		var usuarios = await db.Usuario
			.FromSqlInterpolated($"EXEC dbo.sp_Usuario_ConsultarCorreo {correo}")
			.AsNoTracking()
			.ToListAsync();

		return usuarios.SingleOrDefault();
	}

	//=========================================================================================
	// Consulta los registros de usuarios mediante el procedimiento almacenado correspondiente.
	//=========================================================================================
	public static Task<List<vw_Usuarios>> UsuariosAsync(AppDbContext db) =>
		db.vw_Usuarios.FromSqlRaw("EXEC dbo.sp_Usuarios_Consultar").AsNoTracking().ToListAsync();

	//=========================================================================================
	// Consulta los registros de clientes mediante el procedimiento almacenado correspondiente.
	//=========================================================================================
	public static Task<List<vw_Clientes>> ClientesAsync(AppDbContext db) =>
		db.vw_Clientes.FromSqlRaw("EXEC dbo.sp_Clientes_Consultar").AsNoTracking().ToListAsync();

	//=========================================================================================
	// Consulta los registros de estadios mediante el procedimiento almacenado correspondiente.
	//=========================================================================================
	public static Task<List<vw_Estadios>> EstadiosAsync(AppDbContext db) =>
		db.vw_Estadios.FromSqlRaw("EXEC dbo.sp_Estadios_Consultar").AsNoTracking().ToListAsync();

	//=========================================================================================
	// Consulta los registros de partidos mediante el procedimiento almacenado correspondiente.
	//=========================================================================================
	public static Task<List<vw_Partidos>> PartidosAsync(AppDbContext db) =>
		db.vw_Partidos.FromSqlRaw("EXEC dbo.sp_Partidos_Consultar").AsNoTracking().ToListAsync();

	//================================================================================================================
	// Consulta los registros de disponibilidad de zonas y cupos mediante el procedimiento almacenado correspondiente.
	//================================================================================================================
	public static Task<List<vw_DisponibilidadZonas>> DisponibilidadZonasAsync(AppDbContext db) =>
		db.vw_DisponibilidadZonas.FromSqlRaw("EXEC dbo.sp_DisponibilidadZonas_Consultar").AsNoTracking().ToListAsync();

	//=======================================================================================
	// Consulta los registros de ventas mediante el procedimiento almacenado correspondiente.
	//=======================================================================================
	public static Task<List<vw_Ventas>> VentasAsync(AppDbContext db) =>
		db.vw_Ventas.FromSqlRaw("EXEC dbo.sp_Ventas_Consultar").AsNoTracking().ToListAsync();

	//=================================================================================================
	// Consulta los registros de boletos vendidos mediante el procedimiento almacenado correspondiente.
	//=================================================================================================
	public static Task<List<vw_BoletosVendidos>> BoletosVendidosAsync(AppDbContext db) =>
		db.vw_BoletosVendidos.FromSqlRaw("EXEC dbo.sp_BoletosVendidos_Consultar").AsNoTracking().ToListAsync();

	//=========================================================================================
	// Consulta los registros de facturas mediante el procedimiento almacenado correspondiente.
	//=========================================================================================
	public static Task<List<vw_Facturas>> FacturasAsync(AppDbContext db) =>
		db.vw_Facturas.FromSqlRaw("EXEC dbo.sp_Facturas_Consultar").AsNoTracking().ToListAsync();

	//==============================================================================================
	// Consulta los registros de cancelaciones mediante el procedimiento almacenado correspondiente.
	//==============================================================================================
	public static Task<List<vw_Cancelaciones>> CancelacionesAsync(AppDbContext db) =>
		db.vw_Cancelaciones.FromSqlRaw("EXEC dbo.sp_Cancelaciones_Consultar").AsNoTracking().ToListAsync();

	//===============================================================================================================
	// Consulta los registros de ingresos agrupados por partido mediante el procedimiento almacenado correspondiente.
	//===============================================================================================================
	public static Task<List<vw_IngresosPorPartido>> IngresosPorPartidoAsync(AppDbContext db) =>
		db.vw_IngresosPorPartido.FromSqlRaw("EXEC dbo.sp_IngresosPorPartido_Consultar").AsNoTracking().ToListAsync();

	//==================================================================================================
	// Consulta los registros de asientos ocupados mediante el procedimiento almacenado correspondiente.
	//==================================================================================================
	public static Task<List<vw_AsientosOcupados>> AsientosOcupadosAsync(AppDbContext db) =>
		db.vw_AsientosOcupados.FromSqlRaw("EXEC dbo.sp_AsientosOcupados_Consultar").AsNoTracking().ToListAsync();

	//====================================================================================================
	// Consulta los registros de cambios de usuarios mediante el procedimiento almacenado correspondiente.
	//====================================================================================================
	public static Task<List<vw_AuditoriaUsuarios>> AuditoriaUsuariosAsync(AppDbContext db) =>
		db.vw_AuditoriaUsuarios.FromSqlRaw("EXEC dbo.sp_AuditoriaUsuarios_Consultar").AsNoTracking().ToListAsync();

	//====================================================================================================
	// Consulta los registros de cambios de clientes mediante el procedimiento almacenado correspondiente.
	//====================================================================================================
	public static Task<List<vw_AuditoriaClientes>> AuditoriaClientesAsync(AppDbContext db) =>
		db.vw_AuditoriaClientes.FromSqlRaw("EXEC dbo.sp_AuditoriaClientes_Consultar").AsNoTracking().ToListAsync();
}
