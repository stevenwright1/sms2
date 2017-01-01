using System;
using System.Windows.Forms;
using AuthGateway.Shared;
using System.Collections.Generic;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class OATHCalcConfig01 : UserControl, IWizardScreen
	{
		protected SystemConfiguration sc;
		protected Wizard wizard;
		public OATHCalcConfig01()
		{
			InitializeComponent();
		}
		public OATHCalcConfig01(Wizard wizard, SystemConfiguration sc)
			: this()
		{
			this.wizard = wizard;
			this.sc = sc;
		}

		public bool Store()
		{
			try
			{
				this.sc.OATHCalcTotpWindow = Int32.Parse(this.nudTotp.Value.ToString());
				this.sc.OATHCalcHotpAfterWindow = Int32.Parse(this.nudHotp.Value.ToString());
				return true;
			}
			catch (Exception ex)
			{
				this.wizard.ShowErrors(new List<string>() { ex.Message });
				return false;
			}
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

		private void OATHCalcConfig01_Load(object sender, EventArgs e)
		{
			this.nudTotp.Value = this.sc.OATHCalcTotpWindow;
			this.nudHotp.Value = this.sc.OATHCalcHotpAfterWindow;
		}
	}
}
