using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuthGateway.Wix.ServiceConfig
{
	public interface IWizardScreen
	{
		bool Store();
		bool SkipNext();
		bool SkipPrevious();
		IWizardScreen GetWizardScreen();
		UserControl GetControl();
	}
}
