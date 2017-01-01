using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
namespace AuthGateway.AdminGUI
{

	public partial class frmResyncToken
	{

		private ProviderConfigContainer pcContainer;
		private string device;

		public frmResyncToken(ProviderConfigContainer pcConfig, string device, bool isTotp)
		{
			// This call is required by the designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
			this.pcContainer = pcConfig;
			this.device = device;
			lbDevice.Text = device;
			lbToken2.Visible = !isTotp;
			tbToken2.Visible = !isTotp;
		}

		private void btnResync_Click(System.Object sender, System.EventArgs e)
		{

			if (string.IsNullOrEmpty(tbToken1.Text) || string.IsNullOrEmpty(tbToken2.Text)) {
			}
			ResyncHotpRet ret = pcContainer.getClientLogic().ResyncProvider(pcContainer.getUser(), pcContainer.getDomain(), string.Empty, device, tbToken1.Text, tbToken2.Text);
			if (!string.IsNullOrEmpty(ret.Error)) {
				MessageBox.Show(ret.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.DialogResult = System.Windows.Forms.DialogResult.Abort;
			} else if (ret.Out == 0) {
				MessageBox.Show("No record was synchronized", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				this.DialogResult = System.Windows.Forms.DialogResult.Abort;
			} else {
				MessageBox.Show("Counter synchronized.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
			}
			this.Close();
		}
	}
}
