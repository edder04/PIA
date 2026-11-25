using Microsoft.EntityFrameworkCore;
using PIA.Datos;
using PIA.Repositorios;
using System.Globalization;

namespace PIA;

internal sealed class HistorialClienteReporte
{
    public required vw_Clientes Cliente { get; init; }
    public required List<vw_Ventas> Ventas { get; init; }
    public required List<vw_BoletosVendidos> Boletos { get; init; }

    public int TotalBoletosValidos => Ventas.Sum(v => Math.Max(0, v.TotalBoletos - v.BoletosCancelados));
    public decimal TotalGastado => Ventas
        .Where(v => string.Equals(v.EstadoVenta, "Activa", StringComparison.OrdinalIgnoreCase))
        .Sum(v => v.Total);

    //==========================================================
    // Obtiene los partidos asociados a una venta del historial.
    //==========================================================
    public string ObtenerPartidos(int idVenta)
    {
        var partidos = Boletos
            .Where(b => b.IdVenta == idVenta)
            .Select(b => $"{b.EquipoLocal} vs. {b.EquipoVisitante}")
            .Distinct(StringComparer.OrdinalIgnoreCase);
        string resultado = string.Join("; ", partidos);
        return string.IsNullOrWhiteSpace(resultado) ? "Sin boletos registrados" : resultado;
    }

    //================================================================================
    // Prepara las líneas de texto con los datos del historial para exportarlas a PDF.
    //================================================================================
    public IEnumerable<string> CrearLineasPdf()
    {
        var cultura = CultureInfo.GetCultureInfo("es-MX");
        yield return $"Cliente: {Cliente.Nombres} {Cliente.ApPaterno} {Cliente.ApMaterno}".Trim();
        yield return $"ID de cliente: {Cliente.IdCliente}";
        yield return $"RFC: {Cliente.RFC}";
        yield return $"Correo: {Cliente.CorreoElectronico}";
        yield return $"Teléfono celular: {Cliente.TelefonoCelular}";
        yield return $"Fecha del reporte: {DateTime.Now:dd/MM/yyyy HH:mm}";
        yield return string.Empty;
        yield return $"Compras registradas: {Ventas.Count}";
        yield return $"Total gastado en ventas activas: {TotalGastado.ToString("C2", cultura)}";
        yield return $"Boletos vigentes: {TotalBoletosValidos}";
        yield return string.Empty;
        yield return "DETALLE DE COMPRAS";

        if (Ventas.Count == 0)
        {
            yield return "El cliente aún no tiene compras registradas.";
            yield break;
        }

        foreach (var venta in Ventas)
        {
            yield return string.Empty;
            yield return $"Fecha: {venta.FechaHoraVenta:dd/MM/yyyy HH:mm} | Folio: {venta.Folio}";
            yield return $"Partido(s): {ObtenerPartidos(venta.IdVenta)}";
            yield return $"Boletos: {Math.Max(0, venta.TotalBoletos - venta.BoletosCancelados)} vigentes de {venta.TotalBoletos}; cancelados: {venta.BoletosCancelados}";
            yield return $"Subtotal: {venta.Subtotal.ToString("C2", cultura)} | IVA: {venta.IVA.ToString("C2", cultura)} | Total: {venta.Total.ToString("C2", cultura)}";
            yield return $"Estado: {venta.EstadoVenta} | Pago: {venta.MetodoPago} | Vendedor: {venta.Vendedor}";
            yield return venta.Facturada == 1
                ? $"Factura: Sí{(string.IsNullOrWhiteSpace(venta.FolioFactura) ? string.Empty : $" | Folio: {venta.FolioFactura}")}"
                : "Factura: No emitida";

            var boletosVenta = Boletos.Where(b => b.IdVenta == venta.IdVenta).ToList();
            foreach (var grupo in boletosVenta.GroupBy(b => new { b.EquipoLocal, b.EquipoVisitante, b.Estadio, b.NombreZona }))
            {
                string asientos = string.Join(", ", grupo.Select(b => $"{b.Fila}-{b.Asiento} ({b.EstadoBoleto})"));
                yield return $"  {grupo.Key.EquipoLocal} vs. {grupo.Key.EquipoVisitante} | {grupo.Key.Estadio} | {grupo.Key.NombreZona} | Asientos: {asientos}";
            }
        }
    }
}

internal static class HistorialClienteRepositorio
{
    //==============================================================
    // Consulta el historial de ventas, boletos y datos del cliente.
    //==============================================================
    public static async Task<HistorialClienteReporte?> ObtenerAsync(int idCliente)
    {
        using var db = new AppDbContext();
        var cliente = (await RepoConsulta.ClientesAsync(db))
            .SingleOrDefault(c => c.IdCliente == idCliente);
        if (cliente is null)
            return null;

        var ventas = (await RepoConsulta.VentasAsync(db))
            .Where(v => v.IdCliente == idCliente)
            .OrderByDescending(v => v.FechaHoraVenta)
            .ToList();

        var boletos = (await RepoConsulta.BoletosVendidosAsync(db))
            .Where(b => b.IdCliente == idCliente)
            .OrderBy(b => b.FechaHoraVenta)
            .ThenBy(b => b.Fila)
            .ThenBy(b => b.Asiento)
            .ToList();

        return new HistorialClienteReporte
        {
            Cliente = cliente,
            Ventas = ventas,
            Boletos = boletos
        };
    }
}
