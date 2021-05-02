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
            var findWindowTitle = "LINE";
            var process = Process.GetProcesses().FirstOrDefault(p => p.MainWindowTitle == findWindowTitle);
            var capture = new ScreenCapture();

            JobRefreshStream = new Thread(() =>
            {
                while (!Stopping)
                {
                    if (process == null)
                    {
                        //wait retry
                        Thread.Sleep(missAndRetryTime);
                        process = Process.GetProcesses().FirstOrDefault(p => p.MainWindowTitle == findWindowTitle);
                    }
                    if (!IsPause && process != null)
                    {
                        try
                        {
                            var img = capture.CaptureWindow(process.MainWindowHandle);
                            if (pictureBox1.Image != null)
                                pictureBox1.Image.Dispose();
                            pictureBox1.Image = img;
                        }
                        catch (System.Runtime.InteropServices.ExternalException)
                        {
                            process = null;
                            Thread.Sleep(missAndRetryTime);
                        }
                    }
                    Thread.Sleep(100);
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
