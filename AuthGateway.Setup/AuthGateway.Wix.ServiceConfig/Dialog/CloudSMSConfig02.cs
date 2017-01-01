using System;
using System.Windows.Forms;
using AuthGateway.Shared;
using System.Collections.Generic;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class CloudSMSConfig02 : UserControl, IWizardScreen
	{
		protected SystemConfiguration sc;
		protected Wizard wizard;

		public CloudSMSConfig02()
		{
			InitializeComponent();
		}

		public CloudSMSConfig02(Wizard wizard, SystemConfiguration sc)
			: this()
		{
			this.wizard = wizard;
			this.sc = sc;
		}

		private void CloudSMSConfig02_Load(object sender, EventArgs e)
		{
			if (sc.CloudSMSConfiguration.CloudSMSModules[0].TypeName == "Textlocal")
			{
				var username = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("Username");
				if (username != null)
					tbUsername.Text = username.Value;

				var password = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("Password");
				if (password != null)
					tbPassword.Text = password.Value;
			}
			else
			{
				if (sc.CloudSMSConfiguration.CloudSMSModules[0].TypeName == "Regexp")
				{
					var url = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("url");
					if (url != null && url.Value.Contains("txtlocal"))
					{
						var username = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("uname");
						if (username != null)
							tbUsername.Text = username.Value;

						var password = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("pword");
						if (password != null)
							tbPassword.Text = password.Value;
					}
				}
			}
		}

		public bool Store()
		{
			try
			{
				if (sc.CloudSMSConfiguration.CloudSMSModules[0].TypeName == "Textlocal")
				{
					var username = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("Username");
					if (username != null)
						username.Value = tbUsername.Text;

					var password = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("Password");
					if (password != null)
						password.Value = tbPassword.Text;
				}
				else
				{
					if (sc.CloudSMSConfiguration.CloudSMSModules[0].TypeName == "Regexp")
					{
						var url = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("Url");
						if (url != null && url.Value.Contains("txtlocal"))
						{
							var username = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("uname");
							if (username != null)
								username.Value = tbUsername.Text;

							var password = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("pword");
							if (password != null)
								password.Value = tbPassword.Text;
						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				var reasons = new List<string>();
				reasons.Add(ex.Message);
				if (reasons.Count == 0)
					return true;
				this.wizard.ShowErrors(reasons);
				return false;
			}
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
	}
}
