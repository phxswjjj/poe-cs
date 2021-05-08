using poe.lib.Win;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stream_viewer
{
    public partial class Form1 : Form
    {
        Thread JobRefreshStream = null;
        bool Stopping = false;
        bool IsPause = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var missAndRetryTime = 10 * 1000;
            var findWindowTitle = "Path of Exile";
            var targetHandle = ScreenCapture.User32.FindWindowByCaption(findWindowTitle);
            var capture = new ScreenCapture() { EnableDebug = true };

            JobRefreshStream = new Thread(() =>
            {
                while (!Stopping)
                {
                    if (targetHandle == IntPtr.Zero)
                    {
                        //wait retry
                        Thread.Sleep(missAndRetryTime);
                        targetHandle = ScreenCapture.User32.FindWindowByCaption(findWindowTitle);
                    }
                    if (!IsPause && targetHandle == ScreenCapture.User32.GetForegroundWindow())
                    {
                        try
                        {
                            var img = capture.CaptureWindow(targetHandle);
                            if (pictureBox1.Image != null)
                                pictureBox1.Image.Dispose();
                            pictureBox1.Image = img;
                        }
                        catch (System.Runtime.InteropServices.ExternalException)
                        {
                            targetHandle = IntPtr.Zero;
                        }
                    }
                }
            });

            JobRefreshStream.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (JobRefreshStream != null)
            {
                Stopping = true;
                if (!JobRefreshStream.Join(1000))
                {
                    try
                    {
                        JobRefreshStream.Abort();
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }

            }
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            IsPause = true;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            IsPause = false;
        }
    }
}
