using poe.lib.ImageExtension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace poe.lib.Win
{
    public class ScreenCapture
    {
        public bool EnableDebug;

        public Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }
        public Image CaptureWindow(IntPtr handle)
        {
            Stopwatch sw = null;
            if (EnableDebug)
            {
                sw = new Stopwatch();
                sw.Start();
            }

            User32.RECT rect;
            User32.GetWindowRect(handle, out rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            using (var bitmap = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(rect.left, rect.top), Point.Empty, new Size(width, height));

                    if (EnableDebug)
                    {
                        sw.Stop();
                        var msec = sw.ElapsedMilliseconds;
                        var fps = (int)(1000 / msec);
                        bitmap.ShowFPS(fps);
                    }
                }

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    var png = new Bitmap(ms);
                    return png;
                }
            }
        }

        public class User32
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
            public static extern IntPtr GetWindowRect(IntPtr hWnd, out RECT rect);
            [DllImport("user32.dll")]
            private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            public static IntPtr FindWindowByCaption(string lpWindowName)
            {
                return FindWindow(default(string), lpWindowName);
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();
        }
    }
}
