using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Threading;

namespace AuthGateway.AuthEngine
{
		[RunInstaller(true)]
		public partial class Installer1 : Installer
		{
				private ServiceProcessInstaller _serviceProcessInstaller = new ServiceProcessInstaller();
				private ServiceInstaller _serviceInstaller = new ServiceInstaller();


				public Installer1()
				{
						InitializeComponent();

						this.AfterInstall += new InstallEventHandler(Installer1AfterInstall);
															
						_serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
						_serviceProcessInstaller.Username = null;
						_serviceProcessInstaller.Password = null;

						//# Service Information
						_serviceInstaller.DisplayName = "Wright AuthEngine";
						_serviceInstaller.StartType = ServiceStartMode.Automatic;


						//# This must be identical to the WindowsService.ServiceBase name
						_serviceInstaller.ServiceName = "WrightAuthEngine";
						_serviceInstaller.Description = "Wright AuthEngine is the controller application for AuthGateway. This service handles database and interprocess communication";

						this.Installers.Add(_serviceProcessInstaller);
						this.Installers.Add(_serviceInstaller);         
				}

				private void Installer1AfterInstall(object sender, InstallEventArgs e)
				{
					try
					{
						//ServiceController sc = new ServiceController("WrightAuthEngine");
						//sc.Start();
					}
					catch { }
				}//-----------------------

				protected override void OnBeforeUninstall(IDictionary savedState)
				{
						base.OnBeforeUninstall(savedState);

						// Set the service name based on the command line
						String name = "WrightAuthEngine";

						if (name != "")
						{
								_serviceInstaller.ServiceName = name;
						}
				}//--------------------

				////event will get context paramenter
				//public string GetContextParameter(string key)
				//{
				//    string sValue = "";
				//    try
				//    {
				//        sValue = this.Context.Parameters[key].ToString();
				//    }
				//    catch
				//    {
				//        sValue = "";
				//    }

				//    return sValue;
				//}//---------------------------

				//protected override void OnBeforeInstall(IDictionary savedState)
				//{
				//    base.OnBeforeInstall(savedState);

				//    Boolean isUserAccount = false;

				//    // Decode the command line switches
				//    String name = GetContextParameter("name").Trim();
				//    if (name != "")
				//    {
				//        _serviceInstaller.ServiceName = name;
				//    }

				//    String desc = GetContextParameter("desc").Trim();

				//    if (desc != "")
				//    {
				//        _serviceInstaller.Description = desc;
				//    }
						
				//    // What type of credentials to use to run the service
				//    String acct = GetContextParameter("account");

				//    switch (acct.ToLower())
				//    {
				//        case "user":
				//            _serviceProcessInstaller.Account = ServiceAccount.User;
				//            isUserAccount = true;
				//            break;
				//        case "localservice":
				//            _serviceProcessInstaller.Account = ServiceAccount.LocalService;
				//            break;
				//        case "localsystem":
				//            _serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
				//            break;
				//        case "networkservice":
				//            _serviceProcessInstaller.Account = ServiceAccount.NetworkService;
				//            break;
				//    }

				//    // User name and password
				//    String username = GetContextParameter("user").Trim();
				//    String password = GetContextParameter("password").Trim();

				//    // Should I use a user account?
				//    if (isUserAccount)
				//    {
				//        // If we need to use a user account,
				//        // set the user name and password
				//        if (username != "")
				//        {
				//            _serviceProcessInstaller.Username = username;
				//        }
				//        if (password != "")
				//        {
				//            _serviceProcessInstaller.Password = password;
				//        }
				//    }
				//}//------------------------


			
		}
}
