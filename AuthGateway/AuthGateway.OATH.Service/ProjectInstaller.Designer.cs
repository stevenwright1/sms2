using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
namespace once.upon.a.vb.time.service
{
	[System.ComponentModel.RunInstaller(true)]
	partial class ProjectInstaller : System.Configuration.Install.Installer
	{

		//Installer overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]
		protected override void Dispose(bool disposing)
		{
			try {
				if (disposing && components != null) {
					components.Dispose();
				}
			} finally {
				base.Dispose(disposing);
			}
		}

		//Required by the Component Designer

		private System.ComponentModel.IContainer components = null;
		//NOTE: The following procedure is required by the Component Designer
		//It can be modified using the Component Designer.  
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.ServiceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
			this.ServiceInstaller1 = new System.ServiceProcess.ServiceInstaller();
			//
			//ServiceProcessInstaller1
			//
			this.ServiceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.ServiceProcessInstaller1.Password = null;
			this.ServiceProcessInstaller1.Username = null;
			//
			//ServiceInstaller1
			//
			this.ServiceInstaller1.Description = "Wright OATHCalc is a service that computes OTP";
			this.ServiceInstaller1.DisplayName = "Wright OATHCalc";
			this.ServiceInstaller1.ServiceName = "WrightOATHCalc";
			this.ServiceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			//
			//ProjectInstaller
			//
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
				this.ServiceProcessInstaller1,
				this.ServiceInstaller1
			});

		}
		internal System.ServiceProcess.ServiceProcessInstaller ServiceProcessInstaller1;

		internal System.ServiceProcess.ServiceInstaller ServiceInstaller1;
	}
}
