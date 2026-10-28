using System;
using System.Collections.Generic;
using System.Text;

namespace PIA.Datos
{

	public static class Sesion
	{
		public static int IdUsuario { get; set; }
		public static string NombreUsuario { get; set; } = string.Empty;
		public static string? Rol { get; set; }

		public static void Cerrar()
		{
			IdUsuario = 0;
			NombreUsuario = string.Empty;
			Rol = null;
		}
	}
}
