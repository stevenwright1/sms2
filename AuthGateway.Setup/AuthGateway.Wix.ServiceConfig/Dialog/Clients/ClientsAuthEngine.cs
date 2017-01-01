using System;
using System.Net;
using System.Windows.Forms;
using AuthGateway.Shared;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.DirectoryServices.ActiveDirectory;
using System.Data.SqlClient;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class ClientsAuthEngine : UserControl, IWizardScreen
	{
		protected SystemConfiguration scClient;
		protected SystemConfiguration scServer;
		protected Wizard wizard;
		public ClientsAuthEngine()
		{
			InitializeComponent();
		}
		public ClientsAuthEngine(Wizard wizard, SystemConfiguration scClient, SystemConfiguration scServer)
			: this()
		{
			this.wizard = wizard;
			this.scClient = scClient;
			this.scServer = scServer;
		}

		private void ClientsAuthEngine_Load(object sender, EventArgs e)
		{
			IPHostEntry iphe = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress[] ips = iphe.AddressList;
			var currentAuthEngineIp = scServer.getAuthEngineServerAddress().ToString();
			foreach (IPAddress ip in ips)
				cbAuthEngineAddress.Items.Add(ip.ToString());
			if (scClient.getAuthEngineServerAddress().ToString() == "127.0.0.1")
				cbAuthEngineAddress.Text = currentAuthEngineIp;
			if (scClient.AuthEngineServerPort == 9060)
				tbAuthEnginePort.Text = scClient.AuthEngineServerPort.ToString();
		}

		public bool Store()
		{
			try
			{
				scClient.setServicesData(cbAuthEngineAddress.Text, tbAuthEnginePort.Text,
					scClient.getSMSServerAddress().ToString(), scClient.CloudSMSServerPort.ToString());
				return true;
			}
			catch (Exception ex)
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
	}
}
