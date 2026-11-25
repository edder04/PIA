using System.Text.RegularExpressions;

namespace PIA.Datos;

public static class PoliticaCliente
{
    private static readonly Regex FormatoRFC = new(@"^[A-Z&Ñ]{3,4}\d{6}[A-Z0-9]{3}$", RegexOptions.Compiled);
    private static readonly Regex FormatoCorreo = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);
    private static readonly Regex FormatoTelefono = new(@"^\+?[0-9\s()\-]{7,15}$", RegexOptions.Compiled);

    //============================================================
    // Valida que el RFC tenga estructura y caracteres permitidos.
    //============================================================
    public static bool EsRFCValido(string? valor) =>
        !string.IsNullOrWhiteSpace(valor) && FormatoRFC.IsMatch(valor.Trim().ToUpperInvariant());

    //==========================================
    // Valida el formato del correo electrónico.
    //==========================================
    public static bool EsCorreoValido(string? valor) =>
        !string.IsNullOrWhiteSpace(valor) && FormatoCorreo.IsMatch(valor.Trim());

    //==============================================
    // Valida el formato y la longitud del teléfono.
    //==============================================
    public static bool EsTelefonoValido(string? valor) =>
        !string.IsNullOrWhiteSpace(valor) && FormatoTelefono.IsMatch(valor.Trim());
}
