using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Xml.Linq;

using AuthGateway.OATH;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;
using AuthGateway.Shared.Tracking;

namespace AuthGateway.OATH.Service
{

	public partial class winservice : ServiceBase
	{

		private AsyncServer server;

		Thread mWorker;

		public winservice()
		{
			// This call is required by the designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
		}

		protected override void OnStart(string[] args)
		{            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			if ((server != null)) {
				server.StopServer();
				if ((mWorker != null) && mWorker.ThreadState ==  System.Threading.ThreadState.WaitSleepJoin) {
					mWorker.Join();
				}
			}
			server = new AsyncServer();
			mWorker = new Thread(new ThreadStart(DoWork));
			mWorker.Start();
		}

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Tracker.Instance.TrackException("OATH.Service.winservice", System.Reflection.MethodInfo.GetCurrentMethod().Name, (Exception)e.ExceptionObject);
        }

		protected override void OnStop()
		{
			Logger.Instance.WriteToLog("Service stopping.", LogLevel.Info);
			server.StopServer();
			Logger.Instance.WriteToLog("Service stopped.", LogLevel.Info);
			Logger.Instance.Flush();
            Tracker.Instance.StopTracking();
			server = null;
		}

		private void RestartService()
		{
			Logger.Instance.WriteToLog("RELOADING SystemConfiguration", LogLevel.All);
			this.OnStop();
			string[] args = {
				
			};
			this.OnStart(args);
		}

		private void DoWork()
		{
			Logger.Instance.EmptyLoggers();

			string path = Assembly.GetExecutingAssembly().Location;
			path = Directory.GetParent(Directory.GetParent(path).FullName).FullName;
			path = path.TrimEnd(System.IO.Path.DirectorySeparatorChar);

			string startUpPath = Assembly.GetExecutingAssembly().Location;
			startUpPath = Directory.GetParent(startUpPath).FullName;

			SystemConfiguration sc = new WatchedSystemConfiguration(this.RestartService, startUpPath);
			try {
				if ((sc.ConfigFileDoesNotExists())) {
					try {
						sc.WriteXMLCredentials();

					} catch {
					}
				}

				sc.LoadSettings(true);
			} catch (SystemConfigurationParseError ex) {
				Logger.Instance.SetLogLevel(LogLevel.All);
				Logger.Instance.AddLogger(new FileLogger(Directory.GetParent(startUpPath).FullName + "\\Logs", this.ServiceName), LogLevel.All);
				Logger.Instance.AddLogger(new EventLogLogger(this), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("LogLevel set to All in all Loggers due to Configuration error."), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("{0}", ex.Message), LogLevel.Error);
				if ((ex.InnerException != null)) {
					Logger.Instance.WriteToLog(string.Format("Inner error parsing configuration: {0}", ex.InnerException.Message), LogLevel.Error);
				}
				throw new Exception("Error parsing configuration file.");
			}

			Logger.Instance.SetLogLevel(sc.AuthEngineLogLevel);
			Logger.Instance.AddLogger(new FileLogger(path + "\\Logs", this.ServiceName), sc.AuthEngineFileLogLevel);
			Logger.Instance.AddLogger(new EventLogLogger(this), sc.AuthEngineEventLogLevel);

			Logger.Instance.WriteToLog("Do work started.", LogLevel.Info);

            if (sc.SendTrackingInfo) {
                Tracker.Instance.StartTracking("OATHCalcEvent");
            }

			int maxconnections = Convert.ToInt32(ConfigurationManager.AppSettings["MaxConnections"]);
			Logger.Instance.WriteToLog(string.Format("Server starting: {0}:{1}", sc.OATHCalcServerIp, sc.OATHCalcServerPort), LogLevel.Info);
			Logger.Instance.Flush();
			server.StartServer(sc, maxconnections);
			Logger.Instance.WriteToLog("Do work stopping.", LogLevel.Info);
			Logger.Instance.Flush();
		}

	}
}
