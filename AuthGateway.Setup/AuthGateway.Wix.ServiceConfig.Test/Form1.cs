using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AuthGateway.Shared;
using Microsoft.Deployment.WindowsInstaller;

namespace AuthGateway.Wix.ServiceConfig.Test
{
	public partial class Form1 : Form
	{
		protected CASession session;
		public Form1()
		{
			InitializeComponent();
			//this.session = Session.FromHandle(this.Handle, true);
			this.session = new CASessionTest();
		}

		private void btnConfigCloudSMS_Click(object sender, EventArgs e)
		{
			var casa = new CASessionTest();
			var cah = new CustomActionsHandler();
			casa["CONFIGURATIONXML"] = @"";
			casa["INSTALLLOCATION"] = @"C:\Test";
			cah.WrightCCS_CA_ConfigureCloudSMS(casa);
		}

		private void btnConfigOATHCalc_Click(object sender, EventArgs e)
		{
			var casa = new CASessionTest();
			var cah = new CustomActionsHandler();
			casa["CONFIGURATIONXML"] = @"";
			casa["INSTALLLOCATION"] = @"C:\Test";
			cah.WrightCCS_CA_ConfigureOATHCalc(casa);
		}

		private void btnConfigAuthEngine_Click(object sender, EventArgs e)
		{
			var casa = new CASessionTest();
			var cah = new CustomActionsHandler();
			casa["WIX_ACCOUNT_LOCALSYSTEM"] = @"NT AUTHORITY\SYSTEM";
			casa["WIX_ACCOUNT_LOCALSERVICE"] = @"NT AUTHORITY\LOCAL SERVICE";
			casa["WIX_ACCOUNT_NETWORKSERVICE"] = @"NT AUTHORITY\NETWORK SERVICE";
			casa["CONFIGURATIONXML"] = @"";
			casa["INSTALLLOCATION"] = @"C:\Test";
			cah.WrightCCS_CA_ConfigureAuthEngine(casa);
		}

		private void btnTestRemoveRegistry_Click(object sender, EventArgs e)
		{
			var casa = new CASessionTest();
			var cah = new CustomActionsHandler();
			casa["CONFIGURATIONXML"] = @"";
			casa["INSTALLLOCATION"] = @"C:\Test";
			cah.WrightCCS_CA_RemoveOldResidual(casa);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var casa = new CASessionTest();
			var cah = new CustomActionsHandler();
			casa["CONFIGURATIONXML"] = @"";
			casa["INSTALLLOCATION"] = @"C:\Test";
			var sc = new SystemConfiguration();
			sc.SetDBData("(local)\\SQLEXPRESS", "pa", "pa", "1433", false, @"\sql\query", false, "SMS_ASDASD1");
			CustomActionsHandler.executeDatabaseHandler(casa, sc);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			var casa = new CASessionTest();
			var cah = new CustomActionsHandler();
			casa["WIX_ACCOUNT_LOCALSYSTEM"] = @"NT AUTHORITY\SYSTEM";
			casa["WIX_ACCOUNT_LOCALSERVICE"] = @"NT AUTHORITY\LOCAL SERVICE";
			casa["WIX_ACCOUNT_NETWORKSERVICE"] = @"NT AUTHORITY\NETWORK SERVICE";
			casa["CONFIGURATIONXML"] = @"";
			casa["INSTALLLOCATION"] = @"C:\Test";
			cah.WrightCCS_CA_ConfigureAuthEngine(casa);
			cah.WrightCCS_CA_ConfigureClients(casa);
		}
	}
}
