using poe.lib.Win;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace keyboard_detect
{
    public partial class Form1 : Form
    {
        KeyboardCapture kb;
        Thread JobKeepRunning;
        bool EnableKeepRunning = false;
        bool Stopping = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            kb = new KeyboardCapture();
            KeyboardCapture.OnKeyDown += KeyboardCapture_OnKeyDown;
            KeyboardCapture.OnKeyUp += KeyboardCapture_OnKeyUp;

            JobKeepRunning = new Thread(() =>
            {
                while (!Stopping)
                {
                    if (!EnableKeepRunning && label1.Text == "Stopped")
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    label1.Invoke((MethodInvoker)delegate
                    {
                        if (EnableKeepRunning)
                            label1.Text = "Running...";
                        else
                            label1.Text = "Stopped";
                        Thread.Sleep(100);
                    });
                }
            });
            JobKeepRunning.Start();
        }

        private void KeyboardCapture_OnKeyDown(poe.lib.Win.KeyEventArgs e)
        {
            var keyInfo = e.KeyCode;
            listBox1.Items.Add($"{keyInfo} keydown");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            if (listBox1.Items.Count > 30)
                listBox1.Items.RemoveAt(0);

            if (keyInfo == poe.lib.Win.Keys.Oemtilde)
                EnableKeepRunning = true;
        }
        private void KeyboardCapture_OnKeyUp(poe.lib.Win.KeyEventArgs e)
        {
            var keyInfo = e.KeyCode;
            listBox1.Items.Add($"{keyInfo} keyup");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            if (listBox1.Items.Count > 30)
                listBox1.Items.RemoveAt(0);

            if (keyInfo == poe.lib.Win.Keys.Oemtilde)
                EnableKeepRunning = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stopping = true;
            try
            {
                JobKeepRunning.Join(500);
                if (JobKeepRunning.IsAlive)
                    JobKeepRunning.Abort();
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
