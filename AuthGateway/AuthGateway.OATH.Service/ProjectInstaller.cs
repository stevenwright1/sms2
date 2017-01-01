using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
namespace once.upon.a.vb.time.service
{

	public partial class ProjectInstaller
	{

		public ProjectInstaller() : base()
		{
			AfterInstall += onAfterInstallStartService;

			//This call is required by the Component Designer.
			InitializeComponent();

			//Add initialization code after the call to InitializeComponent
		}

		public void onAfterInstallStartService(object sender, InstallEventArgs e)
		{
			try {
				//Dim sc As ServiceController = New ServiceController("WrightOATHCalc")
				//sc.Start()
			} catch {
			}
		}
	}
}
