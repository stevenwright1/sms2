using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuthGateway.AdminGUI
{
	public partial class frmConfirmPincode : Form
	{
		private const int WM_NCHITTEST = 0x84;
		private const int HTCLIENT = 0x1;
		private const int HTCAPTION = 0x2;

		private string Pincode { get; set; }

		public frmConfirmPincode()
		{
			InitializeComponent();
			this.Pincode = string.Empty;
		}

		public frmConfirmPincode(string pincode)
			: this()
		{
			this.Pincode = pincode;
		}

		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case WM_NCHITTEST:
					base.WndProc(ref m);
					if ((int)m.Result == HTCLIENT)
						m.Result = (IntPtr)HTCAPTION;
					return;
			}
			base.WndProc(ref m);
		}

		private void btnConfirm_Click(object sender, EventArgs e)
		{
			if (tbPincode.Text != this.Pincode)
			{
				ShowErrorMessage("Pincode does not match.");
				this.DialogResult = DialogResult.None;
			}
			else
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void ShowErrorMessage(string error)
		{
			MessageBox.Show(error, Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
