using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AuthGateway.Shared;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class Wizard : Form
	{
		private int index = 0;
		private IWizardScreen[] screens;
		private string configurationXml;
		public string InstallDir { get; private set; }
		public SystemConfiguration sc { get; private set; }
		public Dictionary<string, string> SessionValues { get; set; }

		public Wizard()
		{
			InitializeComponent();
			this.SessionValues = new Dictionary<string, string>();
		}

		public Wizard(string configurationXml, string installDir)
			: this()
		{
			// TODO: Complete member initialization
			this.configurationXml = configurationXml;
			this.InstallDir = installDir;
			try
			{
				if (string.IsNullOrEmpty(this.configurationXml) || this.configurationXml == "0")
				{
					sc = new SystemConfiguration();
					//this.ShowInfo("file: " + Path.Combine(installDir, "Settings", "Configuration.xml"));
					sc.LoadSettingsFromFile(Path.Combine(installDir, "Settings", "Configuration.xml"), false);
					this.configurationXml = sc.WriteXMLCredentialsToString();
				}
				else
				{
					sc = new SystemConfiguration();
					//this.ShowInfo("configurationXml: " + this.configurationXml);
					sc.LoadSettings(this.configurationXml);
					this.configurationXml = sc.WriteXMLCredentialsToString();
				}
			}
			catch //(Exception ex)
			{
				//this.ShowError(ex.Message);
				sc = new SystemConfiguration();
				this.configurationXml = sc.WriteXMLCredentialsToString();
			}
		}

		internal Control GetScreensContainer()
		{
			return this.tableLayoutPanel1;
		}

		internal void SetScreens(List<IWizardScreen> screens)
		{
			this.screens = screens.ToArray();
		}

		internal void SetCurrentScreenIndex(int p)
		{
			this.index = p;
		}

		private void Wizard_Load(object sender, EventArgs e)
		{
			//this.tableLayoutPanel1.Controls.Add(this.screens[this.index].GetControl(), 0, 0);
			ShowCurrentScreen();
		}

		internal void ShowCurrentScreen()
		{
			var currentScreen = this.tableLayoutPanel1.GetControlFromPosition(0, 0);
			if (currentScreen != null)
				this.tableLayoutPanel1.Controls.Remove(currentScreen);
			this.tableLayoutPanel1.Controls.Add(this.screens[this.index].GetControl(), 0, 0);
			this.screens[this.index].GetControl().Dock = DockStyle.Fill;
			if (this.index == 0)
				btnBack.Enabled = false;
			else
				btnBack.Enabled = true;
			btnNext.Enabled = true;
			if (this.index < this.screens.Length - 1)
			{
				btnNext.Text = "Next";
			}
			else if (this.index == this.screens.Length - 1)
			{
				btnNext.Text = "Finish";
			}
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			var wizardscreen = this.screens[this.index];
			if (!wizardscreen.Store())
				return;
			this.index++;
			if (this.index >= this.screens.Length)
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			else
			{
				ShowCurrentScreen();
			}
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			this.index--;
			if (this.index < 0)
				this.index = 0;
			ShowCurrentScreen();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		public void ShowError(string error)
		{
			ShowErrors(new List<string>() { error });
		}

		public void ShowErrors(List<string> errors)
		{
			this.InvokeIfRequired(() =>
			{
				var errorsAppend = new StringBuilder();
				errorsAppend.AppendLine("An error occurred:" + Environment.NewLine);
				foreach (var error in errors)
				{
					errorsAppend.AppendFormat("{0}" + Environment.NewLine, error);
				}
				MessageBox.Show(this, errorsAppend.ToString(), "WrightCCS - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)8192);
			});
		}

		internal void ShowInfo(string message, string title = "WrightCCS - Info")
		{
			this.InvokeIfRequired(() =>
			{
				MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, (MessageBoxOptions)8192);
			});
		}

		internal void ShowWarning(string message, string title = "WrightCCS - Warning")
		{
			this.InvokeIfRequired(() =>
			{
				MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, (MessageBoxOptions)8192);
			});
		}
	}
}
