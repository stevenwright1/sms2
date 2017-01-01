using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
namespace AuthGateway.AdminGUI
{
	public interface ProviderConfig
	{

		void loadConfig(string config);
		string getConfig();

		string getName();

		string getFriendlyName();
		void setFriendlyName(string name);

		void ShowConfig();

		void HideConfig();

		void BeforeSave();
		string PostSaveMessage();

		void validateBeforeSave();
	}
}
namespace AuthGateway.AdminGUI
{

	public class ProviderConfigNotFoundException : Exception
	{
	}
}
