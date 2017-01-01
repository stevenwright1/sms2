using System;
using System.Windows.Forms;
using System.Linq;
using AuthGateway.Shared;
namespace AuthGateway.AdminGUI
{

	public partial class ucStatic : ProviderConfig
	{


		private ProviderConfigContainer pcContainer;
		public ucStatic(ProviderConfigContainer pcCont) : this()
		{
			this.pcContainer = pcCont;
			this.Parent = this.pcContainer.getControl();
			this.pcContainer.getControl().Controls.Add(this);
			this.Dock = DockStyle.Fill;
		}

		public ucStatic()
		{
			// This call is required by the designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
		}

		public string getConfig()
		{
			return CryptoHelper.HashPincode(getPassword());
		}


		public void loadConfig(string config)
		{
		}

		public string getName()
		{
			return "Static";
		}

		private string friendlyName = string.Empty;
		public string getFriendlyName()
		{
			if (string.IsNullOrEmpty(friendlyName))
				return this.getName();
			return friendlyName;
		}
		public void setFriendlyName(string name)
		{
			friendlyName = name;
		}

		public void ShowConfig()
		{
			this.Show();
		}

		public void HideConfig()
		{
			this.Hide();
		}

		public void BeforeSave()
		{
		}

		public string PostSaveMessage()
		{
			return string.Empty;
		}

		public void validateBeforeSave()
		{
			if (string.IsNullOrEmpty(getPassword()))
			{
				throw new ArgumentException("Static token cannot be empty.");
			}
		}

		private string getPassword()
		{
			if (string.IsNullOrEmpty(tbPassword.Text))
				return string.Empty;
			return tbPassword.Text.Trim();
		}
	}
}
