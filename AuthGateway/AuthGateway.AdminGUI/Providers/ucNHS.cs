using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
namespace AuthGateway.AdminGUI
{

	public partial class ucNHS : ProviderConfig
	{


		private ProviderConfigContainer pcContainer;
		public ucNHS(ProviderConfigContainer pcCont) : this()
		{
			this.pcContainer = pcCont;
			this.Parent = this.pcContainer.getControl();
			this.pcContainer.getControl().Controls.Add(this);
			this.Dock = DockStyle.Fill;
		}

		public ucNHS()
		{
			// This call is required by the designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
		}

		public string getConfig()
		{
			return string.Empty;
		}


		public void loadConfig(string config)
		{
		}

		public string getName()
		{
			return "NHS";
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
