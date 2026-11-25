using PIA.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PIA.Repositorios
{
	//=======================
	// Repositorio de Usuario
	//=======================
	public class RepoUsuario{
		//=================================================
		// Agrega el registro capturado a la base de datos.
		//=================================================
		public async Task AgregarAsync(Usuario usuario)
		{
			using var db = new AppDbContext();
			await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.sp_Usuario_Agregar {usuario.Nombres}, {usuario.ApPaterno}, {usuario.ApMaterno}, {usuario.FechaNacimiento}, {usuario.NumeroNomina}, {usuario.CorreoElectronico}, {usuario.Rol}, {usuario.NombreUsuario}, {usuario.Contraseña}, {usuario.CreadoPor}");
		}

		//========================================
		// Actualiza async con los datos actuales.
		//========================================
		public async Task ActualizarAsync(int idUsuario, string campo, string valor, int modificadoPor)
		{
			using var db = new AppDbContext();
			await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.sp_Usuario_Actualizar {idUsuario}, {campo}, {valor}, {modificadoPor}");
		}
	}

	//========================
	// Repositorio de Clientes
	//========================
	public class RepoCliente
	{
		//=================================================
		// Agrega el registro capturado a la base de datos.
		//=================================================
		public async Task AgregarAsync(Cliente cliente)
		{
			using var db = new AppDbContext();
			await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.sp_Cliente_Agregar {cliente.Nombres}, {cliente.ApPaterno}, {cliente.ApMaterno}, {cliente.FechaNacimiento}, {cliente.Domicilio}, {cliente.RFC}, {cliente.CorreoElectronico}, {cliente.TelefonoCasa}, {cliente.TelefonoCelular}, {cliente.CreadoPor}");
		}

		//================================================
		// Actualiza cliente async con los datos actuales.
		//================================================
		public async Task ActualizarClienteAsync(int idCliente, string campo,	string valor)
		{
			using var db = new AppDbContext();
			await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.sp_Cliente_Actualizar {idCliente}, {campo}, {valor.Trim()}, {Sesion.IdUsuario}");
		}

	}

	//========================
	// Repositorio de Estadios
	//========================
	public class RepoEstadio
	{
		//======================================
		// Agrega un estadio a la base de datos.
		//======================================
		public async Task<int> AgregarEstadioAsync(Estadio estadio)
		{
			using var db = new AppDbContext();
			var resultado = await db.Database.SqlQuery<int>(
				$"EXEC dbo.sp_Estadio_Agregar {estadio.Nombre}, {estadio.Ciudad}, {estadio.CapacidadTotal}, {estadio.MapaEstadio}, {estadio.CreadoPor}")
				.ToListAsync();
			return resultado.Single();
		}

		//================================================
		// Obtiene todos los estadios de la base de datos.
		//================================================
		public async Task<List<Estadio>> ObtenerTodosAsync()
		{
			using var db = new AppDbContext();

			return await db.Estadio.FromSqlRaw("EXEC dbo.sp_Estadios_ListadoCaptura").AsNoTracking().ToListAsync();
		}

	}

	//=====================
	// Repositorio de Zonas
	//=====================
	public class RepoZona{
		//=================================================
		// Agrega el registro capturado a la base de datos.
		//=================================================
		public async Task AgregarZonaAsync(Zona zona)
		{
			using var db = new AppDbContext();

			await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.sp_Zona_Agregar {zona.IdEstadio}, {zona.NombreZona}, {zona.PrecioBase}, {zona.CupoZona}, {zona.CreadoPor}");
		}
	}

	//========================
	// Repositorio de Partidos
	//========================
	public class RepoPartido{
		//=================================================
		// Agrega el registro capturado a la base de datos.
		//=================================================
		public async Task AgregarPartidoAsync(Partido partido)
		{
			using var db = new AppDbContext();
			await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.sp_Partido_Agregar {partido.IdEstadio}, {partido.FechaHora}, {partido.EquipoLocal}, {partido.EquipoVisitante}, {partido.Estado}, {partido.CreadoPor}");
		}

	}

	public class RepoVenta
	{
		//=============================================================
		// Registra la venta, sus boletos y los asientos seleccionados.
		//=============================================================
		public async Task<int> RegistrarVentaAsync(
			Venta venta,
			DetalleVenta detalle,
			List<Boleto> boletos)
		{
			await using var db = new AppDbContext();
			if (venta.Folio == Guid.Empty) venta.Folio = Guid.NewGuid();
			string asientosJson = System.Text.Json.JsonSerializer.Serialize(
				boletos.Select(b => new { Fila = b.Fila, Asiento = b.Asiento }));
			var resultados = await db.Database.SqlQuery<int>(
				$"EXEC dbo.sp_Venta_Registrar {venta.IdCliente}, {venta.IdUsuario}, {detalle.IdPartido}, {detalle.IdZona}, {venta.MetodoPago}, {asientosJson}, {venta.Folio}")
				.ToListAsync();
			int idVenta = resultados.Single();
			venta.IdVenta = idVenta;
			return idVenta;
		}
	}

	//====================================
	// REPOSITORIO PARA CANCELAR UNA VENTA
	//====================================
	public class RepoCancelacion
	{
		//=======================================================
		// Ejecuta una operación de registro en la base de datos.
		//=======================================================
		public async Task RegistrarAsync(Cancelacion cancelacion)
		{
			await using var db = new AppDbContext();
			await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.sp_Venta_CancelarId {cancelacion.IdVenta}, {cancelacion.IdUsuario}, {cancelacion.Motivo}");
		}
	}

}
