using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AuthGateway.Shared;
using AuthGateway.AuthEngine.Logic.Helpers;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class AuthEngineConfig02 : UserControl, IWizardScreen
	{
		protected SystemConfiguration sc;
		protected Wizard wizard;
		public AuthEngineConfig02()
		{
			InitializeComponent();
		}
		public AuthEngineConfig02(Wizard wizard, SystemConfiguration sc)
			: this()
		{
			this.wizard = wizard;
			this.sc = sc;
		}

		private void AuthEngineConfig02_Load(object sender, EventArgs e)
		{
			tbFrom.Text = this.sc.EmailConfig.From;
			tbUsername.Text = this.sc.EmailConfig.Username;
			tbPassword.Text = this.sc.EmailConfig.Password;
			if (this.sc.EmailConfig.Server == "send.nhs.net")
				cbNHS.Checked = true;
			else
				cbCustom.Checked = true;
		}

		public bool Store()
		{
			var reasons = new List<string>();
			if (string.IsNullOrEmpty(tbServer.Text.Trim()))
				reasons.Add("Server name cannot be empty.");
			else
				this.sc.EmailConfig.Server = tbServer.Text.Trim();

			var port = 0;
			if (!Int32.TryParse(tbPort.Text, out port))
			{
				reasons.Add("Invalid port number.");
			}
			else
				this.sc.EmailConfig.Port = port;

			this.sc.EmailConfig.EnableSSL = cbUseSSL.Checked;
			this.sc.EmailConfig.From = tbFrom.Text;
			this.sc.EmailConfig.UseAuth = cbUseAuth.Checked;
			if (this.sc.EmailConfig.UseAuth)
			{
				if (tbUsername.Text.Trim() == string.Empty)
					reasons.Add("Username cannot be empty if Use Auth is enabled.");
				else
					this.sc.EmailConfig.Username = tbUsername.Text.Trim();
				if (tbPassword.Text.Trim() == string.Empty)
					reasons.Add("Password cannot be empty if Use Auth is enabled.");
				else
					this.sc.EmailConfig.Password = tbPassword.Text.Trim();
			}

			if (reasons.Count == 0)
				return true;
			this.wizard.ShowErrors(reasons);
			return false;
		}

		public bool SkipNext()
		{
			return false;
		}

		public bool SkipPrevious()
		{
			return false;
		}

		public IWizardScreen GetWizardScreen()
		{
			return this;
		}

		public UserControl GetControl()
		{
			return this;
		}

		private void cbNHS_CheckedChanged(object sender, EventArgs e)
		{
			if (cbNHS.Checked)
			{
				tbServer.Text = "send.nhs.net";
				tbPort.Text = "587";
				cbUseSSL.Checked = true;
				cbUseAuth.Checked = true;
				enableControls(false);
			}
		}

		private void cbCustom_CheckedChanged(object sender, EventArgs e)
		{
			if (cbCustom.Checked)
			{
				tbServer.Text = this.sc.EmailConfig.Server.ToString();
				tbPort.Text = this.sc.EmailConfig.Port.ToString();
				cbUseSSL.Checked = this.sc.EmailConfig.EnableSSL;
				cbUseAuth.Checked = this.sc.EmailConfig.UseAuth;
				enableControls(true);
			}
		}

		private void enableControls(bool enable)
		{
			tbServer.Enabled = enable;
			tbPort.Enabled = enable;
			cbUseSSL.Enabled = enable;
			cbUseAuth.Enabled = enable;
		}

		private void btnSendTestEmail_Click(object sender, EventArgs e)
		{
			var mailSender = new MailSender();
			if (string.IsNullOrWhiteSpace(tbTestDestination.Text) || !mailSender.IsValidAddress(tbTestDestination.Text.Trim()))
			{
				this.wizard.ShowError("Please enter a valid test destination e-mail address");
				return;
			}
			try
			{
				if (!Store())
					return;
				var password = CryptoHelper.DecryptSettingIfNecessary(sc.EmailConfig.Password, "EmailPassword");
				mailSender.Send(sc.EmailConfig, tbTestDestination.Text.Trim(), "WrightCCS - SMS2 Test Message", "This is a test message.", password);
				wizard.ShowInfo("A test e-mail has been sent");
			}
			catch (Exception ex)
			{
				this.wizard.ShowError(string.Format("{0}", ex.Message));
			}
		}
	}
}
