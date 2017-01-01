using System;
using System.Windows.Forms;
using System.Linq;
namespace AuthGateway.AdminGUI
{

	public partial class ucPassthrough : ProviderConfig
	{
		ProviderConfigContainer pcContainer;
		string currentConfig = string.Empty;
		
		public ucPassthrough(ProviderConfigContainer pcCont) : this()
		{
			this.pcContainer = pcCont;
			this.Parent = this.pcContainer.getControl();
			this.pcContainer.getControl().Controls.Add(this);
			this.Dock = DockStyle.Fill;
		}

		public ucPassthrough()
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
			return "Passthrough";
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
