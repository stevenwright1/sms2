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
	public partial class ClientsConfigAuthEngine : UserControl, IWizardScreen
	{
		protected SystemConfiguration sc;
		protected Wizard wizard;
		public ClientsConfigAuthEngine()
		{
			InitializeComponent();
		}
		public ClientsConfigAuthEngine(Wizard wizard, SystemConfiguration sc)
			: this()
		{
			this.wizard = wizard;
			this.sc = sc;
		}

		private void ClientsConfigAuthEngine_Load(object sender, EventArgs e)
		{
			IPHostEntry iphe = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress[] ips = iphe.AddressList;
			var currentAuthEngineIp = sc.getAuthEngineServerAddress().ToString();
			foreach (IPAddress ip in ips)
				cbAuthEngineAddress.Items.Add(ip.ToString());
			cbAuthEngineAddress.Text = currentAuthEngineIp;
			tbAuthEnginePort.Text = sc.AuthEngineServerPort.ToString();
		}

		public bool Store()
		{
			try
			{
				sc.setServicesData(cbAuthEngineAddress.Text, tbAuthEnginePort.Text,
					sc.getSMSServerAddress().ToString(), sc.CloudSMSServerPort.ToString());
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
