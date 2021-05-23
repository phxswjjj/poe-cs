using poe.lib.Win;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace keyboard_detect
{
    public partial class Form1 : Form
    {
        KeyboardCapture kb;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            kb = new KeyboardCapture();
            KeyboardCapture.OnKeyDown += KeyboardCapture_OnKeyDown;
            KeyboardCapture.OnKeyUp += KeyboardCapture_OnKeyUp;
        }

        private void KeyboardCapture_OnKeyDown(poe.lib.Win.KeyEventArgs e)
        {
            var keyInfo = e.KeyCode;
            listBox1.Items.Add($"{keyInfo} keydown");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            if (listBox1.Items.Count > 30)
                listBox1.Items.RemoveAt(0);
        }
        private void KeyboardCapture_OnKeyUp(poe.lib.Win.KeyEventArgs e)
        {
            var keyInfo = e.KeyCode;
            listBox1.Items.Add($"{keyInfo} keyup");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            if (listBox1.Items.Count > 30)
                listBox1.Items.RemoveAt(0);
        }
    }
}
