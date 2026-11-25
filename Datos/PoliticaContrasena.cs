namespace PIA.Datos;

public static class PoliticaContrasena
{
	//=========================================================
	// Comprueba que la contraseña cumpla la política definida.
	//=========================================================
	public static bool EsValida(string? valor)
	{
		if (string.IsNullOrEmpty(valor) || valor.Length < 8)
			return false;

		return valor.Any(char.IsUpper)
			&& valor.Any(char.IsLower)
			&& valor.Any(c => !char.IsLetterOrDigit(c));
	}

	public const string Mensaje = "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula y un carácter especial.";
}
