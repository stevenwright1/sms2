using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AuthGateway.Shared;
using AuthGateway.Shared.Identity;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{

	class ConfigException : Exception
	{
		public ConfigException(string message)
			: base(message)
		{
		}
	}

	public abstract partial class BaseConfigServiceUser : UserControl, IWizardScreen
	{
		protected SystemConfiguration sc;
		protected Wizard wizard;

		private string username;
		private string password;

		protected abstract string ServiceFriendlyName { get; }
		protected abstract string UsernameConfigName { get; }
		protected abstract string PasswordeConfigName { get; }

		private Dictionary<string, string> PresetUsernames = new Dictionary<string, string>()
		{
		};

		public BaseConfigServiceUser()
		{
			InitializeComponent();
		}

		public BaseConfigServiceUser(Wizard wizard, SystemConfiguration sc)
			: this()
		{
			this.wizard = wizard;
			this.sc = sc;
			this.PresetUsernames.Add("Local System", wizard.SessionValues["WIX_ACCOUNT_LOCALSYSTEM"]);
			this.PresetUsernames.Add("Local Service", wizard.SessionValues["WIX_ACCOUNT_LOCALSERVICE"]);
			this.PresetUsernames.Add("Network Service", wizard.SessionValues["WIX_ACCOUNT_NETWORKSERVICE"]);
		}

		private void getUsernameAndPasswordFromGUI()
		{
			var username = cbUsername.Text.Trim();
			var password = tbAdPassword.Text.Trim();

			if (string.IsNullOrWhiteSpace(username))
				throw new ConfigException("Username cannot be empty.");

			if (this.PresetUsernames.ContainsKey(username))
				this.username = this.PresetUsernames[username];
			else
			{
				var parser = new IdentityParser();
				var domainUsername = parser.GetDomainUserNameOrNull(username);
				if (domainUsername == null 
					|| string.IsNullOrWhiteSpace(domainUsername.Domain)
					|| domainUsername.Domain == ".")
					throw new ConfigException("Please specify a domain for the username. If it's a local account, use local machine name.");

				this.username = username;
			}
			if (this.tbAdPassword.Enabled)
				this.password = password;
			else
				this.password = string.Empty;
		}

		private void setUsernameAndPasswordFromSession()
		{
			var username = wizard.SessionValues[this.UsernameConfigName];
			var password = wizard.SessionValues[this.PasswordeConfigName];

			if (string.IsNullOrWhiteSpace(username))
				username = this.PresetUsernames.Values.First();

			foreach (var i in this.PresetUsernames)
			{
				this.username = username;
				if (i.Value == username)
				{
					this.username = i.Key;
					break;
				}
			}
			this.password = password;
		}

		private void ConfigServiceUser_Load(object sender, EventArgs e)
		{
			setUsernameAndPasswordFromSession();
			this.groupBox2.Text = this.ServiceFriendlyName + " Service User";
			this.cbUsername.Text = this.username;
			this.tbAdPassword.Text = this.password;
		}

		public bool Store()
		{
			try
			{
				getUsernameAndPasswordFromGUI();
				wizard.SessionValues[this.UsernameConfigName] = this.username;
				wizard.SessionValues[this.PasswordeConfigName] = this.password;
				return true;
			}
			catch (ConfigException ex)
			{
				wizard.ShowError(ex.Message);
			}
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

		public UserControl GetControl()
		{
			return this;
		}

		public IWizardScreen GetWizardScreen()
		{
			return this;
		}

		private void btnTestConfig_Click(object sender, EventArgs e)
		{
			try
			{
				getUsernameAndPasswordFromGUI();
				if (this.PresetUsernames.ContainsValue(this.username))
				{
					wizard.ShowInfo("Internal users cannot be tested.");
					return;
				}

				var parser = new IdentityParser();
				var domainUsername = parser.GetDomainUserNameOrNull(this.username);

				using (var impersonated = new ImpersonatedUser(domainUsername.Username, domainUsername.Domain, this.password))
				{

				}
				wizard.ShowInfo("User logged in successfully.");
			}
			catch (ConfigException ex)
			{
				wizard.ShowError(ex.Message);
			}
			catch (NotImpersonableException ex)
			{
				wizard.ShowError(ex.Message);
			}
			catch
			{
				wizard.ShowError("There was an error trying to impersonate this user.");
			}
			finally
			{
			}
		}

		private void cbUsername_TextChanged(object sender, EventArgs e)
		{
			if (this.PresetUsernames.ContainsKey(cbUsername.Text.Trim()))
				this.tbAdPassword.Enabled = false;
			else 
				this.tbAdPassword.Enabled = true;
		}
	}
}
