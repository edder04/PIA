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

}