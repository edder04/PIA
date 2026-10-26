using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace PIA.Diseños
{
    internal class TansparentPictureBox
    {
    }

        public class TransparentPictureBox : PictureBox
        {
            public TransparentPictureBox()
            {
                SetStyle(
                    ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true
                );

                BackColor = Color.Transparent;
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                // No dibujar fondo.
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (Image == null)
                    return;

                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Rectangle destino;

                if (SizeMode == PictureBoxSizeMode.Zoom)
                {
                    float ratioX = (float)Width / Image.Width;
                    float ratioY = (float)Height / Image.Height;

                    float ratio = Math.Min(ratioX, ratioY);

                    int width = (int)(Image.Width * ratio);
                    int height = (int)(Image.Height * ratio);

                    int x = (Width - width) / 2;
                    int y = (Height - height) / 2;

                    destino = new Rectangle(x, y, width, height);
                }
                else
                {
                    destino = new Rectangle(0, 0, Width, Height);
                }

                e.Graphics.DrawImage(Image, destino);
            }
        }
}

