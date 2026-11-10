using PIA.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PIA.Repositorios
{
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

	


}