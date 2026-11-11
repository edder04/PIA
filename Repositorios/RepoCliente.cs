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
	}

	//========================
	// Repositorio de Clientes
	//========================
	public class RepoCliente
	{
		public async Task AgregarAsync(Cliente cliente)
		{
			using var db = new AppDbContext();

			bool existeRFC = await db.Cliente
				.AnyAsync(c => c.RFC == cliente.RFC);

			if (existeRFC)
				throw new InvalidOperationException(
					"Ya existe un cliente registrado con ese RFC.");

			db.Cliente.Add(cliente);
			await db.SaveChangesAsync();
		}

		public async Task ActualizarClienteAsync(int idCliente, string campo,	string valor)
		{
			using var db = new AppDbContext();
			var cliente = await db.Cliente
				.SingleOrDefaultAsync(c => c.IdCliente == idCliente);

			if (cliente is null)
				throw new InvalidOperationException("No se encontró el cliente.");

			switch (campo)
			{
				case "CORREO ELECTRONICO":
					cliente.CorreoElectronico = valor.Trim();
					break;

				case "TELEFONO CELULAR":
					cliente.TelefonoCelular = valor.Trim();
					break;

				case "TELEFONO DE CASA":
					cliente.TelefonoCasa = valor.Trim();
					break;

				case "DOMICILIO":
					cliente.Domicilio = valor.Trim();
					break;

				default:
					throw new ArgumentException("Selecciona un campo válido.");
			}

			await db.SaveChangesAsync();
		}

	}

	//========================
	// Repositorio de Estadios
	//========================
	public class RepoEstadio
	{
		// Agrega un estadio a la base de datos
		public async Task<int> AgregarEstadioAsync(Estadio estadio)
		{
			using var db = new AppDbContext();

			bool existeNombre = await db.Estadio
				.AnyAsync(e => e.Nombre == estadio.Nombre);

			if (existeNombre)
				throw new InvalidOperationException(
					"Ya existe un estadio registrado con ese nombre.");

			db.Estadio.Add(estadio);
			await db.SaveChangesAsync();

			return estadio.IdEstadio;
		}

		// Obtiene todos los estadios de la base de datos
		public async Task<List<Estadio>> ObtenerTodosAsync()
		{
			using var db = new AppDbContext();

			return await db.Estadio
				.AsNoTracking()
				.OrderBy(e => e.Nombre)
				.ToListAsync();
		}

	}

	//=====================
	// Repositorio de Zonas
	//=====================
	public class RepoZona{
		public async Task AgregarZonaAsync(Zona zona)
		{
			using var db = new AppDbContext();

			db.Zona.Add(zona);
			await db.SaveChangesAsync();
		}
	}

	//========================
	// Repositorio de Partidos
	//========================
	public class RepoPartido{
		public async Task AgregarPartidoAsync(Partido partido)
		{
			using var db = new AppDbContext();
			db.Partido.Add(partido);
			await db.SaveChangesAsync();
		}

	}

	public class RepoBoleto
	{
		// Agrega los boletos al contexto compartido.
		// RepoVenta hará el SaveChanges dentro de la transacción.
		public void AgregarBoletos(
			AppDbContext db,
			IEnumerable<Boleto> boletos)
		{
			db.Boleto.AddRange(boletos);
		}
	}

	public class RepoVenta
	{
		private readonly RepoBoleto _repoBoleto = new();

		public async Task<int> RegistrarVentaAsync(
			Venta venta,
			DetalleVenta detalle,
			List<Boleto> boletos)
		{
			await using var db = new AppDbContext();
			await using var transaccion = await db.Database.BeginTransactionAsync();

			try
			{
				db.Venta.Add(venta);
				await db.SaveChangesAsync();

				detalle.IdVenta = venta.IdVenta;
				db.DetalleVenta.Add(detalle);
				await db.SaveChangesAsync();

				foreach (var boleto in boletos)
				{
					boleto.IdVenta = venta.IdVenta;
				}

				db.Boleto.AddRange(boletos);
				await db.SaveChangesAsync();

				await transaccion.CommitAsync();
				return venta.IdVenta;
			}
			catch
			{
				await transaccion.RollbackAsync();
				throw;
			}
		}
	}

	//=======================
	// REPOSITORIO PARA CANCELAR UNA VENTA
	//=======================
	public class RepoCancelacion
	{
		//=======================
		// REGISTRA LA CANCELACION Y DEJA QUE EL TRIGGER PROCESE LA VENTA
		//=======================
		public async Task RegistrarAsync(Cancelacion cancelacion)
		{
			await using var db = new AppDbContext();
			bool yaCancelada = await db.Cancelacion
				.AnyAsync(c => c.IdVenta == cancelacion.IdVenta);

			if (yaCancelada)
				throw new InvalidOperationException("Esta venta ya tiene una cancelación registrada.");

			db.Cancelacion.Add(cancelacion);
			await db.SaveChangesAsync();
		}
	}

}
