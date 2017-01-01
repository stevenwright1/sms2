using System;
using System.Windows.Forms;
using System.Linq;
using AuthGateway.Shared;
namespace AuthGateway.AdminGUI
{

	public partial class ucOneTime : ProviderConfig
	{
		ProviderConfigContainer pcContainer;
		string currentConfig = string.Empty;
		
		public ucOneTime(ProviderConfigContainer pcCont) : this()
		{
			this.pcContainer = pcCont;
			this.Parent = this.pcContainer.getControl();
			this.pcContainer.getControl().Controls.Add(this);
			this.Dock = DockStyle.Fill;
		}

		public ucOneTime()
		{
			// This call is required by the designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
		}

		public string getConfig()
		{
			return currentConfig;
		}


		public void loadConfig(string config)
		{
			currentConfig = config;
		}

		public string getName()
		{
			return "OneTime";
		}

		string friendlyName = string.Empty;
		public string getFriendlyName()
		{
			if (string.IsNullOrEmpty(friendlyName))
				return getName();
			return friendlyName;
		}
		public void setFriendlyName(string name)
		{
			friendlyName = name;
		}

		public void ShowConfig()
		{
			Show();
		}

		public void HideConfig()
		{
			Hide();
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
			
		}
	}
}
