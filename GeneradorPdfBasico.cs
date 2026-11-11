using System.Text;

namespace PIA
{
    /// <summary>Genera documentos PDF sencillos sin dependencias externas.</summary>
    internal static class GeneradorPdfBasico
    {
        // Función: crea un PDF de una o más páginas con texto en formato imprimible.
        public static void Guardar(string ruta, string titulo, IEnumerable<string> lineas)
        {
            var renglones = new List<string>();
            foreach (string linea in lineas)
            {
                string texto = linea ?? string.Empty;
                if (texto.Length == 0)
                {
                    renglones.Add(string.Empty);
                    continue;
                }

                const int anchoMaximo = 92;
                while (texto.Length > anchoMaximo)
                {
                    int corte = texto.LastIndexOf(' ', anchoMaximo);
                    if (corte < 1) corte = anchoMaximo;
                    renglones.Add(texto[..corte]);
                    texto = texto[(corte)..].TrimStart();
                }
                renglones.Add(texto);
            }

            const int lineasPorPagina = 48;
            var paginas = new List<List<string>>();
            for (int inicio = 0; inicio < Math.Max(1, renglones.Count); inicio += lineasPorPagina)
                paginas.Add(renglones.Skip(inicio).Take(lineasPorPagina).ToList());

            var objetos = new List<string> { string.Empty };
            objetos.Add("<< /Type /Catalog /Pages 2 0 R >>");
            objetos.Add(string.Empty); // Se completa con la lista de páginas.
            objetos.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>");

            var referenciasPaginas = new List<int>();
            foreach (var paginaLineas in paginas)
            {
                int idPagina = objetos.Count;
                referenciasPaginas.Add(idPagina);
                int idContenido = idPagina + 1;

                var contenido = new StringBuilder();
                contenido.AppendLine("BT /F1 18 Tf 48 790 Td <" + AHex(titulo) + "> Tj ET");
                contenido.AppendLine("0.24 w 48 778 m 547 778 l S");
                int y = 756;
                foreach (string linea in paginaLineas)
                {
                    contenido.AppendLine($"BT /F1 10 Tf 48 {y} Td <{AHex(linea)}> Tj ET");
                    y -= 14;
                }

                objetos.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 3 0 R >> >> /Contents {idContenido} 0 R >>");
                string stream = contenido.ToString();
                objetos.Add($"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}endstream");
            }

            objetos[2] = $"<< /Type /Pages /Kids [{string.Join(" ", referenciasPaginas.Select(id => $"{id} 0 R"))}] /Count {referenciasPaginas.Count} >>";

            var pdf = new StringBuilder("%PDF-1.4\n");
            var offsets = new List<int> { 0 };
            for (int id = 1; id < objetos.Count; id++)
            {
                offsets.Add(pdf.Length);
                pdf.Append($"{id} 0 obj\n{objetos[id]}\nendobj\n");
            }

            int inicioXref = pdf.Length;
            pdf.Append($"xref\n0 {objetos.Count}\n0000000000 65535 f \n");
            for (int id = 1; id < objetos.Count; id++)
                pdf.Append($"{offsets[id]:D10} 00000 n \n");
            pdf.Append($"trailer\n<< /Size {objetos.Count} /Root 1 0 R >>\nstartxref\n{inicioXref}\n%%EOF");

            File.WriteAllBytes(ruta, Encoding.ASCII.GetBytes(pdf.ToString()));
        }

        // Función: convierte texto español a bytes WinAnsi representados en hexadecimal PDF.
        private static string AHex(string texto)
        {
            var hex = new StringBuilder(texto.Length * 2);
            foreach (char caracter in texto)
            {
                byte valor = caracter switch
                {
                    '€' => 0x80,
                    '‚' => 0x82,
                    'ƒ' => 0x83,
                    '„' => 0x84,
                    '…' => 0x85,
                    '†' => 0x86,
                    '‡' => 0x87,
                    'ˆ' => 0x88,
                    '‰' => 0x89,
                    'Š' => 0x8A,
                    '‹' => 0x8B,
                    'Œ' => 0x8C,
                    'Ž' => 0x8E,
                    '‘' => 0x91,
                    '’' => 0x92,
                    '“' => 0x93,
                    '”' => 0x94,
                    '•' => 0x95,
                    '–' => 0x96,
                    '—' => 0x97,
                    '˜' => 0x98,
                    '™' => 0x99,
                    'š' => 0x9A,
                    '›' => 0x9B,
                    'œ' => 0x9C,
                    'ž' => 0x9E,
                    'Ÿ' => 0x9F,
                    _ when caracter <= 0xFF => (byte)caracter,
                    _ => (byte)'?'
                };
                hex.Append(valor.ToString("X2"));
            }
            return hex.ToString();
        }
    }
}
