using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuthGateway.MassUserSetup.Steps
{
	public partial class ConfigureEmail : StepBase
	{
		public ConfigureEmail() : base()
		{
			InitializeComponent();
		}

		private void ConfigureEmail_Load(object sender, EventArgs e)
		{

		}	

		public ConfigureEmail(Main main) : this()
		{
			this.main = main;
			tbServer.Text = this.main.Config.EmailConfig.Server.ToString();
			tbPort.Text = this.main.Config.EmailConfig.Port.ToString();
			cbUseSSL.Checked = this.main.Config.EmailConfig.EnableSSL;
			tbFrom.Text = this.main.Config.EmailConfig.From;
			//this.main.Config.EmailConfig.MessageTitle
			cbUseAuth.Checked = this.main.Config.EmailConfig.UseAuth;
			tbUsername.Text = this.main.Config.EmailConfig.Username;
			tbPassword.Text = this.main.Config.EmailConfig.Password;
		}

		public override bool CanDoNext()
		{
			var reasons = new List<string>();
			if (string.IsNullOrEmpty(tbServer.Text.Trim()))
				reasons.Add("Server name cannot be empty.");
			
			var port = 0;
			if (!Int32.TryParse(tbPort.Text, out port))
				reasons.Add("Invalid port number.");

			
			if (this.main.Config.EmailConfig.UseAuth)
			{
				if (tbUsername.Text.Trim() == string.Empty)
					reasons.Add("Username cannot be empty if Use Auth is enabled.");
				if (tbPassword.Text.Trim() == string.Empty)
					reasons.Add("Password cannot be empty if Use Auth is enabled.");
			}

			if (reasons.Count == 0)
				return true;
			this.main.ShowErrors(reasons);
			return false;
		}

		private void EnableUserData(bool enabled)
		{
			tbUsername.ReadOnly = enabled;
			tbPassword.ReadOnly = enabled;
		}

		private void cbUseAuth_CheckedChanged(object sender, EventArgs e)
		{
			EnableUserData(!cbUseAuth.Checked);
		}

		public override void DoNext()
		{
			this.main.Config.EmailConfig.EnableSSL = cbUseSSL.Checked;
			this.main.Config.EmailConfig.From = tbFrom.Text;
			this.main.Config.EmailConfig.UseAuth = cbUseAuth.Checked;
			this.main.Config.EmailConfig.Server = tbServer.Text.Trim();
			this.main.Config.EmailConfig.Port = Convert.ToInt32(tbPort.Text);
			this.main.Config.EmailConfig.Username = tbUsername.Text.Trim();
			this.main.Config.EmailConfig.Password = tbPassword.Text.Trim();
		}
	}
}
