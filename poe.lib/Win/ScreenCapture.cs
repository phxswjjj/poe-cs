using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace poe.lib.Win
{
    public class ScreenCapture
    {
        public Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }
        public Image CaptureWindow(IntPtr handle)
        {
            IntPtr hscrdc = User32.GetWindowDC(handle);

            User32.RECT rect;
            User32.GetWindowRect(handle, out rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            IntPtr hbitmap = Gdi32.CreateCompatibleBitmap(hscrdc, width, height);
            IntPtr hmemdc = Gdi32.CreateCompatibleDC(hscrdc);
            Gdi32.SelectObject(hmemdc, hbitmap);
            User32.PrintWindow(handle, hmemdc, 0);

            var bmp = Bitmap.FromHbitmap(hbitmap);

            //刪除用過的對象  
            Gdi32.DeleteDC(hscrdc);
            Gdi32.DeleteDC(hmemdc);

            return bmp;
        }
        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            img.Save(filename, format);
        }
        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }

        private class Gdi32
        {
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
            [DllImport("gdi32.dll")]
            public static extern int DeleteDC(IntPtr hdc);
        }
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, out RECT rect);

            [DllImport("user32.dll")]
            public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, UInt32 nFlags);
        }
    }
}
