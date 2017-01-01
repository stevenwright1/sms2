using System;
using System.Windows.Forms;
using AuthGateway.Shared;
using System.Collections.Generic;
using AuthGateway.Shared.Serializer;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class CommonPropertiesConfig01 : UserControl, IWizardScreen
	{
		protected SystemConfiguration sc;
		protected Wizard wizard;		

		public CommonPropertiesConfig01()
		{
			InitializeComponent();
		}

        public CommonPropertiesConfig01(Wizard wizard, SystemConfiguration sc)
			: this()
		{
			this.wizard = wizard;
			this.sc = sc;

            LoadCommonProperties();
		}

        private void CommonPropertiesConfig01_Load(object sender, EventArgs e)
		{            
		}

        private void LoadCommonProperties()
        {
            this.cbSendTrackingInfo.Checked = sc.SendTrackingInfo;
        }        		

		public bool Store()
		{
			try
			{                
                sc.SendTrackingInfo = this.cbSendTrackingInfo.Checked;
				return true;
			}
			catch (Exception ex)
			{
				var reasons = new List<string>();
				reasons.Add(ex.Message);
				if (reasons.Count == 0)
					return true;
				this.wizard.ShowErrors(reasons);
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

		private void btnLoadSample_Click(object sender, EventArgs e)
		{			
		}
	}
}
