using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace AuthGateway.LicenseCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            var ls = new LicenseHandler();
            var dt = monthCalendar1.SelectionEnd;
						dt = new DateTime(dt.Year, dt.Month, dt.Day).ToUniversalTime();
            ls.Save(saveFileDialog1.FileName, dt);
            MessageBox.Show(string.Format("License Valid until: {0}-{1}-{2}", dt.Year, dt.Month, dt.Day));
        }
    }
}
