using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuthGateway.MassUserSetup.Steps
{
	public partial class ConfigureAuth : StepBase
	{
		public ConfigureAuth()
		{
			InitializeComponent();
		}

		public ConfigureAuth(Main main) : this()
		{
			this.main = main;
		}

		public override void DoNext()
		{
			this.main.Configurator = getConfigurator();
		}

		private IConfigurator getConfigurator()
		{
			if (rbCloudSMS.Checked)
			{
				return new ConfigCloudSMS();
			}
			else if (rbEmail.Checked)
			{
				return new ConfigEmail();
			}
			else if (rbOATHCalc.Checked)
			{
				return new ConfigOATHCalc();
			}
			throw new NotImplementedException("getConfigurator()");
		}
	}
}
