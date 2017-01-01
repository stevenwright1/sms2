using System;
using System.IO;
using System.Windows.Forms;
using AuthGateway.Shared;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class SelectOrInputLicense : UserControl, IWizardScreen
	{
		protected SystemConfiguration sc;
		protected Wizard wizard;

		private string licenseXml;

		public SelectOrInputLicense()
		{
			InitializeComponent();
		}
		public SelectOrInputLicense(Wizard wizard, SystemConfiguration sc)
			: this()
		{
			this.wizard = wizard;
			this.sc = sc;


			licenseXml = wizard.SessionValues["LICENSEXML"];
			try
			{
				if (string.IsNullOrEmpty(licenseXml) || licenseXml == "0")
					licenseXml = File.ReadAllText(Path.Combine(wizard.InstallDir, "Settings", "License.xml"));
			}
			catch
			{
				//var lh = new LicenseCreator.LicenseHandler();
				//licenseXml = lh.SaveToString(DateTime.Now.AddYears(5));
				//licenseXml = string.Empty;
                licenseXml = "If you have already registered, please paste the license you received via email here.\r\n\r\nYou can obtain a new license by registering at https://www.wrightccs.com/get/";
			}
			tbLicense.Text = licenseXml;
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

		public bool Store()
		{
            // BUGFIX BEGIN: 2016-10-16 - Validate license before allowing installation to continue
            if (string.IsNullOrEmpty(tbLicense.Text.Trim()) || tbLicense.Text.Trim() == "0") {
                this.wizard.ShowError("The license code entered does not appear to be valid.\r\nPlease check and try again.");
                return false;
            }
            try {
                var lh = new LicenseCreator.LicenseHandler();
                var lxml = lh.Load(tbLicense.Text);
            }
            catch {
                this.wizard.ShowError("The license code entered does not appear to be valid.\r\nPlease check and try again.");
                return false;
            }
            // BUGFIX END
			wizard.SessionValues["LICENSEXML"] = tbLicense.Text.Trim();
			return true;
		}

		public IWizardScreen GetWizardScreen()
		{
			return this;
		}

		private void btnCheck_Click(object sender, EventArgs e)
		{
			try
			{
				var lh = new LicenseCreator.LicenseHandler();
				var lxml = lh.Load(tbLicense.Text);
				this.wizard.ShowInfo("License expires: " + lh.getDecryptedTime(lxml.AuthEngine));
			}
			catch
			{
                this.wizard.ShowError("The license code entered does not appear to be valid.\r\nPlease check and try again.");
			}
		}

        private void tbLicense_Enter(object sender, EventArgs e)
        {
            try {
                var lh = new LicenseCreator.LicenseHandler();
                var lxml = lh.Load(tbLicense.Text);
            }
            catch {
                tbLicense.Text = string.Empty;
            }
        }
	}
}
