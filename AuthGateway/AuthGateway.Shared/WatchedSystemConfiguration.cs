using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AuthGateway.Shared
{
	public class WatchedSystemConfiguration : SystemConfiguration
	{
		private Action handler;
		private FileSystemWatcher watcher;
		public WatchedSystemConfiguration(Action onConfigChange, string startUpPath, string settingsPath = "Settings")
			: base(startUpPath, settingsPath)
		{
			this.handler = onConfigChange;
			setupWatch();
		}

		private void setupWatch()
		{
			watcher = new FileSystemWatcher();
			watcher.Path = GetSettingsPath();
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Filter = "Configuration.xml";
			watcher.Changed += new FileSystemEventHandler(handleChange);
			watcher.EnableRaisingEvents = true;
		}

		private void handleChange(object sender, FileSystemEventArgs e)
		{
			watcher.EnableRaisingEvents = false;
			this.handler();
		}
	}
}
