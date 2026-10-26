using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PIA
{
    public static class RedondearBoton
    {
        public static void Aplicar(Button boton, int radio = 45)
        {
            void Actualizar(object sender, EventArgs e)
            {
                if (boton.Width <= 0 || boton.Height <= 0)
                    return;

                int r = Math.Max(1, Math.Min(
                    radio, Math.Min(boton.Width, boton.Height) / 2));

                int d = r * 2;

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(0, 0, d, d, 180, 90);
                    path.AddArc(boton.Width - d - 1, 0,
                        d, d, 270, 90);
                    path.AddArc(boton.Width - d - 1,
                        boton.Height - d - 1, d, d, 0, 90);
                    path.AddArc(0, boton.Height - d - 1,
                        d, d, 90, 90);

                    path.CloseFigure();
                    boton.Region?.Dispose();
                    boton.Region = new Region(path);
                }
            }

            boton.Resize += Actualizar;
            boton.HandleCreated += Actualizar;
            Actualizar(boton, EventArgs.Empty);
        }
    }
}