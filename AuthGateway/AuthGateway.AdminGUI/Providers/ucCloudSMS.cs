using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using Microsoft.VisualBasic;

namespace AuthGateway.AdminGUI
{

	public partial class ucCloudSMS : ProviderConfig
	{


		private ProviderConfigContainer pcContainer;
		public ucCloudSMS(ProviderConfigContainer pcCont) : this()
		{
			this.pcContainer = pcCont;
			this.Parent = this.pcContainer.getControl();
			this.pcContainer.getControl().Controls.Add(this);
			this.Dock = DockStyle.Fill;
		}

		public ucCloudSMS()
		{
			// This call is required by the designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
		}

		public string getConfig()
		{
			return (string)this.cbModules.SelectedItem;
		}

		public void loadConfig(string config)
		{
			try
			{
				List<string> modules = this.pcContainer.getClientLogic().GetModules(this.pcContainer.getUser(), this.pcContainer.getDomain());
				this.cbModules.Items.AddRange(modules.ToArray());
				if (modules.Contains(config))
				{
					this.cbModules.SelectedItem = config;
				}
				else
				{
					this.cbModules.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An error occured loading CloudSMS configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}

		public string getName()
		{
			return "CloudSMS";
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

		private void Button1_Click(System.Object sender, System.EventArgs e)
		{
			if (!this.pcContainer.isSelectedProvider(this.getName())) {
				this.pcContainer.ShowError("Please activate this provider by clicking Save Configuration before using this functionality");
				return;
			}

			Variables clientLogic = this.pcContainer.getClientLogic();
			SendTokenRet ret = clientLogic.SendToken(this.pcContainer.getUser(), this.pcContainer.getDomain());
			if (!string.IsNullOrEmpty(ret.Error)) {
				MessageBox.Show(ret.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
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
