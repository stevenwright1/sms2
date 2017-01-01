using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using AuthGateway.Shared.XmlMessages;
namespace AuthGateway.AdminGUI
{

	public interface ProviderConfigContainer
	{

		string getDomain();

		string getUser();

		Control getControl();

		Variables getClientLogic();

		List<UserProvider> getProviders();

		bool isSelectedProvider(string name);

		void SaveSelectedProvider(bool validate);

		void ShowError(string message);

		void ShowWarning(string message);
		
		bool ShowConfirm(string message);
	}
}
