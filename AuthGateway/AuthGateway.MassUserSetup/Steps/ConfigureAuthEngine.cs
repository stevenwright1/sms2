using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace AuthGateway.MassUserSetup.Steps
{
	public partial class ConfigureAuthEngine : StepBase
	{
		private IPAddress authEngineServerAddress;
		private int port;

		public ConfigureAuthEngine() : base()
		{
			InitializeComponent();
		}

		public ConfigureAuthEngine(Main main) : this()
		{
			this.main = main;
			authEngineServerAddress = IPAddress.Parse("127.0.0.1");
			tbServer.Text = this.main.Config.AuthEngineServerAddress.ToString();
			tbPort.Text = this.main.Config.AuthEngineServerPort.ToString();
		}

		private void ConfigureAuthEngine_Load(object sender, EventArgs e)
		{
			this.main.EnableNext(false);
		}

		private void btnTest_Click(object sender, EventArgs e)
		{
			var errors = new List<string>();
			if (!IPAddress.TryParse(tbServer.Text, out authEngineServerAddress))
				errors.Add("Enter a valid IPv4/IPv6 Address.");
			if (!Int32.TryParse(tbPort.Text, out port))
				errors.Add("Enter a valid port number.");
			if (errors.Count > 0)
			{
				this.main.ShowErrors(errors);
			}
			else
			{
				this.main.EnableNext(true);
			}
		}

		private void tbServer_TextChanged(object sender, EventArgs e)
		{

		}

		public override void DoNext()
		{
			this.main.Config.AuthEngineServerAddress = authEngineServerAddress;
			this.main.Config.AuthEngineServerPort = port;
		}
	}
}
