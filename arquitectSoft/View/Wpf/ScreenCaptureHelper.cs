using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Captura del escritorio virtual con GDI/Win32 (BitBlt). Se usa como fondo del
    /// efecto liquid glass: se toma UNA foto antes de mostrar la ventana y luego se
    /// recorta a la posición de la ventana. Basado en la técnica del repo MIT
    /// "WPF-Liquid-Glass-Effect"; reescrito para .NET Framework.
    /// OJO: sin ajuste fino de DPI (asume que GetSystemMetrics y PointToScreen van
    /// en la misma escala de píxeles físicos, válido en monitor único).
    /// </summary>
    internal static class ScreenCaptureHelper
    {
        private const int SrcCopy = 0x00CC0020;
        private const int SmXVirtualScreen = 76;
        private const int SmYVirtualScreen = 77;
        private const int SmCxVirtualScreen = 78;
        private const int SmCyVirtualScreen = 79;

        public static BitmapSource FullScreenSnapshot { get; private set; }
        public static int VirtualScreenX { get; private set; }
        public static int VirtualScreenY { get; private set; }
        public static int VirtualScreenWidth { get; private set; }
        public static int VirtualScreenHeight { get; private set; }

        public static void CaptureFullScreen()
        {
            VirtualScreenX = GetSystemMetrics(SmXVirtualScreen);
            VirtualScreenY = GetSystemMetrics(SmYVirtualScreen);
            VirtualScreenWidth = GetSystemMetrics(SmCxVirtualScreen);
            VirtualScreenHeight = GetSystemMetrics(SmCyVirtualScreen);

            BitmapSource bmp = CaptureScreen(VirtualScreenX, VirtualScreenY, VirtualScreenWidth, VirtualScreenHeight);
            if (bmp != null) FullScreenSnapshot = bmp;
        }

        private static BitmapSource CaptureScreen(int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0) return null;

            IntPtr screenDc = IntPtr.Zero;
            IntPtr memDc = IntPtr.Zero;
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                screenDc = GetDC(IntPtr.Zero);
                if (screenDc == IntPtr.Zero) return null;
                memDc = CreateCompatibleDC(screenDc);
                if (memDc == IntPtr.Zero) return null;
                hBitmap = CreateCompatibleBitmap(screenDc, width, height);
                if (hBitmap == IntPtr.Zero) return null;

                oldBitmap = SelectObject(memDc, hBitmap);
                BitBlt(memDc, 0, 0, width, height, screenDc, x, y, SrcCopy);

                BitmapSource bitmap = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(width, height));
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (oldBitmap != IntPtr.Zero && memDc != IntPtr.Zero) SelectObject(memDc, oldBitmap);
                if (hBitmap != IntPtr.Zero) DeleteObject(hBitmap);
                if (memDc != IntPtr.Zero) DeleteDC(memDc);
                if (screenDc != IntPtr.Zero) ReleaseDC(IntPtr.Zero, screenDc);
            }
        }

        [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")] private static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);
        [DllImport("gdi32.dll")] private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("gdi32.dll")] private static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int width, int height, IntPtr hdcSrc, int xSrc, int ySrc, int rop);
        [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr hObject);
        [DllImport("user32.dll")] private static extern int GetSystemMetrics(int nIndex);
    }
}
