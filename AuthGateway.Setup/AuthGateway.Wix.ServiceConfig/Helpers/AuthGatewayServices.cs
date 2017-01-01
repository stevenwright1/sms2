using System.Collections.Generic;
using AuthGateway.Setup.Shared;
using System.IO;
using ServiceTools;

namespace AuthGateway.Setup.Shared
{
	public class AuthGatewayServices
	{
		private class ServiceInfo
		{
			public string Name { get; set; }
			public string Path { get; set; }
			public string DisplayName { get; set; }
		}

		private List<ServiceManager> servicesToCheck;
		private List<ServiceInfo> servicesInfo;

		public AuthGatewayServices()
		{
			servicesToCheck = new List<ServiceManager>();

			servicesInfo = new List<ServiceInfo>();
			servicesInfo.Add(new ServiceInfo() { Name = "WrightAuthEngine", DisplayName = "Wright AuthEngine", Path = "Service\\Wright.AuthEngine.exe" });
			servicesInfo.Add(new ServiceInfo() { Name = "WrightCloudSMS", DisplayName = "Wright CloudSMS", Path = "Service\\Wright.CloudSMS.exe" });
            servicesInfo.Add(new ServiceInfo() { Name = "WrightMutualAuthImages", DisplayName = "Wright MutualAuthImages", Path = "Service\\Wright.MutualAuthImages.exe" });
			servicesInfo.Add(new ServiceInfo() { Name = "WrightOATHCalc", DisplayName = "Wright OATHCalc", Path = "Service\\Wright.OATHCalc.Service.exe" });

			servicesToCheck.Add(new ServiceManager("WrightAuthEngine", 7500));
			servicesToCheck.Add(new ServiceManager("WrightCloudSMS", 7500));
            servicesToCheck.Add(new ServiceManager("WrightMutualAuthImages", 7500));
			servicesToCheck.Add(new ServiceManager("WrightOATHCalc", 7500));
			servicesToCheck.Add(new ServiceManager("IAS", 7500));
		}

		public List<ServiceManager> GetServices()
		{
			return this.servicesToCheck;
		}

		private void ChangeServicesStatus(bool stop)
		{
			foreach (var manager in this.servicesToCheck)
			{
				try
				{
					if (stop)
						manager.Stop();
					else
						manager.Start();
				}
				catch
				{

				}
			}
		}

		public void StopAll()
		{
			ChangeServicesStatus(true);
		}

		public void StartAll()
		{
			ChangeServicesStatus(false);
		}

		public void FixIfNecessary(string rootPath)
		{
			if (string.IsNullOrEmpty(rootPath))
				return;
			foreach (var si in servicesInfo)
			{
				var filePath = Path.Combine(rootPath, si.Path);
				if (!ServiceInstaller.ServiceIsInstalled(si.Name) && File.Exists(filePath))
				{
					ServiceInstaller.Install(si.Name, si.DisplayName, filePath);
				}
			}
		}
	}
}
