using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.ServiceProcess;
namespace AuthGateway.OATH.Service
{

	partial class winservice
	{

		//UserService overrides dispose to clean up the component list.
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

		// The main entry point for the process
		[MTAThread()]
		[System.Diagnostics.DebuggerNonUserCode()]
		public static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun = null;

			// More than one NT Service may run within the same process. To add
			// another service to this process, change the following line to
			// create a second service object. For example,
			//
			//   ServicesToRun = New System.ServiceProcess.ServiceBase () {New Service1, New MySecondUserService}
			//
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new winservice() };

			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		//Required by the Component Designer

		private System.ComponentModel.IContainer components = null;
		// NOTE: The following procedure is required by the Component Designer
		// It can be modified using the Component Designer.  
		// Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			//
			//winservice
			//
			this.ServiceName = "WrightOATHCalc";

		}

	}
}
