using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AuthGateway.Shared;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class ViewConfigurationXml : UserControl, IWizardScreen
	{
		protected SystemConfiguration sc;
		protected Wizard wizard;
		public ViewConfigurationXml()
		{
			InitializeComponent();
		}
		public ViewConfigurationXml(Wizard wizard, SystemConfiguration sc)
			: this()
		{
			this.wizard = wizard;
			this.sc = sc;
		}

		public bool Store()
		{
			return true;
		}

		public bool SkipNext()
		{
			return false;
		}

		public bool SkipPrevious()
		{
			return false;
		}

		public IWizardScreen GetWizardScreen()
		{
			return this;
		}

		public UserControl GetControl()
		{
			return this;
		}

		private void ViewConfigurationXml_Load(object sender, EventArgs e)
		{
			textBox1.Text = this.sc.WriteXMLCredentialsToString();
		}
	}
}
