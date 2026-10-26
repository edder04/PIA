using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PIA.Diseños
{
    /// <summary>Estilo de fondo translúcido.</summary>
    public enum GlassStyle
    {
        /// <summary>Desenfoque tipo acrílico (Windows 10 1803+ / Windows 11). Permite tinte personalizado.</summary>
        Acrylic,
        /// <summary>Material Mica del sistema (solo Windows 11 22H2+). Si no existe, usa Acrylic.</summary>
        Mica,
        /// <summary>Desenfoque simple sin ruido (Windows 10 / 11).</summary>
        Blur
    }

    /// <summary>
    /// Aplica un efecto de vidrio (glassmorphism) a un Form con una sola llamada:
    ///     GlassEffect.Apply(this);
    /// </summary>
    public static class GlassEffect
    {
        // ---------- API pública ----------

        /// <param name="form">Formulario al que se aplica el efecto.</param>
        /// <param name="style">Tipo de efecto.</param>
        /// <param name="tint">Color del tinte (por defecto, negro azulado).</param>
        /// <param name="tintOpacity">0 = muy transparente, 255 = tinte sólido. Solo Acrylic/Blur.</param>
        /// <param name="darkMode">Barra de título y controles del sistema en modo oscuro.</param>
        /// <param name="roundedCorners">Esquinas redondeadas (Windows 11).</param>
        public static void Apply(
            Form form,
            GlassStyle style = GlassStyle.Acrylic,
            Color? tint = null,
            byte tintOpacity = 120,
            bool darkMode = true,
            bool roundedCorners = true)
        {
            ArgumentNullException.ThrowIfNull(form);

            // Los píxeles negros de GDI se dibujan transparentes sobre el efecto.
            // Por eso el fondo del form debe ser negro y el texto claro.
            form.BackColor = Color.Black;
            form.ForeColor = Color.White;

            void Enable()
            {
                IntPtr hwnd = form.Handle;
                Color t = tint ?? Color.FromArgb(18, 24, 38);

                if (darkMode) SetInt(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, 1);
                if (roundedCorners) SetInt(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, DWMWCP_ROUND);

                bool canMica = Environment.OSVersion.Version.Build >= 22621;

                if (style == GlassStyle.Mica && canMica)
                {
                    var margins = new MARGINS { Left = -1, Right = -1, Top = -1, Bottom = -1 };
                    DwmExtendFrameIntoClientArea(hwnd, ref margins);
                    SetInt(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, DWMSBT_MAINWINDOW);
                    return;
                }

                SetAccent(hwnd,
                    style == GlassStyle.Blur ? ACCENT_ENABLE_BLURBEHIND : ACCENT_ENABLE_ACRYLICBLURBEHIND,
                    t, tintOpacity);
            }

            if (form.IsHandleCreated) Enable();
            else form.HandleCreated += (_, _) => Enable();
        }

        /// <summary>Quita el efecto y deja el fondo normal.</summary>
        public static void Remove(Form form)
        {
            if (!form.IsHandleCreated) return;
            SetAccent(form.Handle, ACCENT_DISABLED, Color.Black, 0);
            SetInt(form.Handle, DWMWA_SYSTEMBACKDROP_TYPE, 1); // none
            form.BackColor = SystemColors.Control;
            form.ForeColor = SystemColors.ControlText;
        }

        // ---------- Implementación (Win32) ----------

        private const int ACCENT_DISABLED = 0;
        private const int ACCENT_ENABLE_BLURBEHIND = 3;
        private const int ACCENT_ENABLE_ACRYLICBLURBEHIND = 4;
        private const int WCA_ACCENT_POLICY = 19;

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const int DWMWCP_ROUND = 2;
        private const int DWMSBT_MAINWINDOW = 2; // Mica

        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy
        {
            public int AccentState;
            public int AccentFlags;
            public int GradientColor; // 0xAABBGGRR
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            public int Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS
        {
            public int Left, Right, Top, Bottom;
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        private static void SetInt(IntPtr hwnd, int attribute, int value)
        {
            try { DwmSetWindowAttribute(hwnd, attribute, ref value, sizeof(int)); }
            catch { /* sistema sin soporte: se ignora */ }
        }

        private static void SetAccent(IntPtr hwnd, int state, Color tint, byte opacity)
        {
            var accent = new AccentPolicy
            {
                AccentState = state,
                AccentFlags = 2,
                GradientColor = (opacity << 24) | (tint.B << 16) | (tint.G << 8) | tint.R
            };

            int size = Marshal.SizeOf(accent);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(accent, ptr, false);
                var data = new WindowCompositionAttributeData
                {
                    Attribute = WCA_ACCENT_POLICY,
                    Data = ptr,
                    SizeOfData = size
                };
                SetWindowCompositionAttribute(hwnd, ref data);
            }
            catch { /* sistema sin soporte: se ignora */ }
            finally { Marshal.FreeHGlobal(ptr); }
        }
    }

}
