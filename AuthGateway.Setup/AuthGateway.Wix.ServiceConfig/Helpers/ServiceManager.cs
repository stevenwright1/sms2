using System;
using System.ServiceProcess;

namespace AuthGateway.Setup.Shared
{
	public class ServiceManager
	{
		private string serviceName;
		private string friendlyName;
		private ServiceController service;

		private int millisecs;

		public ServiceManager(string serviceName, int millisecs)
		{
			this.serviceName = serviceName;
			try
			{
				this.service = new ServiceController(serviceName);
				this.friendlyName = this.service.DisplayName;
			}
			catch (Exception ex)
			{
				this.service = null;
				this.friendlyName = ex.Message;
			}
			this.millisecs = millisecs;
		}

		public ServiceManager(string serviceName)
			: this(serviceName, 5000)
		{

		}

		public bool ValidService()
		{
			return this.service != null;
		}

		public void Start()
		{
			if (!this.ValidService()) return;
			TimeSpan timeout = TimeSpan.FromMilliseconds(millisecs);
			service.Refresh();

			if (service.Status != ServiceControllerStatus.Stopped)
				service.Stop();
			service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

			service.Refresh();
			service.Start();
			service.WaitForStatus(ServiceControllerStatus.Running, timeout);
		}

		public void Stop()
		{
			if (!this.ValidService()) return;
			service.Refresh();
			TimeSpan timeout = TimeSpan.FromMilliseconds(millisecs);

			if (service.Status != ServiceControllerStatus.Stopped)
				service.Stop();
			service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
		}

		public void Restart()
		{
			if (!this.ValidService()) return;
			service.Refresh();

			int millisec1 = Environment.TickCount;
			TimeSpan timeout = TimeSpan.FromMilliseconds(millisecs);
			if (service.Status != ServiceControllerStatus.Stopped)
				service.Stop();
			service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

			service.Refresh();
			int millisec2 = Environment.TickCount;
			timeout = TimeSpan.FromMilliseconds(millisecs - (millisec2 - millisec1));
			if (service.Status != ServiceControllerStatus.Running)
				service.Start();
			service.WaitForStatus(ServiceControllerStatus.Running, timeout);
		}

		public bool IsRunning()
		{
			if (!this.ValidService()) return false;
			service.Refresh();
			return (service.Status == ServiceControllerStatus.Running);
		}

		public string GetDisplayName()
		{
			if (!this.ValidService()) return string.Format("Invalid service '{0}'", this.serviceName);
			return service.DisplayName;
		}
	}
}
