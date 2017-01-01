using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using AuthGateway.AuthEngine.Logic;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;
using AuthGateway.Shared.Tracking;
using AuthGateway.AuthEngine.Logic.DAL;
using System.Collections.Generic;

namespace AuthGateway.AuthEngine
{
	public partial class Service1 : ServiceBase
	{
		private ServerAccess server;
		private Thread svThread = null;

		private string path = string.Empty;
		private SystemConfiguration sc;

		public Service1()
		{
			InitializeComponent();

			this.ServiceName = "WrightAuthEngine";

			this.CanShutdown = true;
			this.CanStop = true;
		}

		protected override void OnStart(string[] args)
		{            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			base.OnStart(args);            

			Logger.Instance.EmptyLoggers();

			Assembly thisAssembly = Assembly.GetExecutingAssembly();
			this.path = Directory.GetParent(Directory.GetParent(thisAssembly.Location).FullName).FullName.TrimEnd(Path.DirectorySeparatorChar);

			String startUpPath = Assembly.GetExecutingAssembly().Location;
			startUpPath = Directory.GetParent(startUpPath).FullName;

			try
			{
				sc = new WatchedSystemConfiguration(
					delegate() {
						Logger.Instance.WriteToLog("RELOADING SystemConfiguration", LogLevel.All);
						this.OnStop();
						this.OnStart(new string[] { });
					},
					startUpPath);

				if (sc.ConfigFileDoesNotExists())
				{
					try { sc.WriteXMLCredentials(); }
					catch { }
				}

				sc.LoadSettings(true);                

				Logger.Instance.SetLogLevel(sc.AuthEngineLogLevel);
				Logger.Instance.AddLogger(new FileLogger(
						this.path + @"\Logs"
						, this.ServiceName, sc.LogMaxFiles, sc.LogMaxSize), sc.AuthEngineFileLogLevel);
				Logger.Instance.AddLogger(new EventLogLogger(this), sc.AuthEngineEventLogLevel);
				Logger.Instance.SetFlushOnWrite(sc.AuthEngineFlushOnWrite);

                if (sc.SendTrackingInfo) {
                    Tracker.Instance.StartTracking("AuthEngineEvent");
                }

				DBQueriesProvider.SetQueriesProvider(new DBQueriesProviderNew(sc.GetSQLConnectionString()));
			}
			catch (SystemConfigurationParseError ex)
			{
				Logger.Instance.SetLogLevel(LogLevel.All);
				Logger.Instance.EmptyLoggers();
				Logger.Instance.SetFlushOnWrite(true);
				Logger.Instance.AddLogger(new FileLogger(
						this.path + @"\Logs"
						, this.ServiceName), LogLevel.All);
				Logger.Instance.AddLogger(new EventLogLogger(this), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("LogLevel set to All in all Loggers due to Configuration error."), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("SystemConfigurationParseError Error: {0}", ex.Message), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("SystemConfigurationParseError Stack: {0}", ex.StackTrace), LogLevel.Debug);
				if (ex.InnerException != null)
					Logger.Instance.WriteToLog(string.Format("Inner error parsing configuration: {0}", ex.InnerException.Message), LogLevel.Error);
				throw new Exception("Error parsing configuration file.");
			}
			catch (Exception ex)
			{
				Logger.Instance.SetLogLevel(LogLevel.All);
				Logger.Instance.EmptyLoggers();
				Logger.Instance.SetFlushOnWrite(true);
				Logger.Instance.AddLogger(new FileLogger(
						this.path + @"\Logs"
						, this.ServiceName), LogLevel.All);
				Logger.Instance.WriteToLog(string.Format("Exception Error: {0}", ex.Message), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("Exception Stack: {0}", ex.StackTrace), LogLevel.Error);
				if (ex.InnerException != null)
				{
					Logger.Instance.WriteToLog(string.Format("Inner error: {0}", ex.InnerException.Message), LogLevel.Error);
					Logger.Instance.WriteToLog(string.Format("Inner stack: {0}", ex.InnerException.StackTrace), LogLevel.Error);
				}
				throw new Exception("Error parsing configuration file.");
			}

			Logger.Instance.WriteToLog("Service started", LogLevel.Info);
			server = new ServerAccess();
			try
			{
				svThread = new Thread(new ParameterizedThreadStart(server.StartService));
				svThread.IsBackground = true;
				svThread.Start(new object[] { sc, path, new Registry() });
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(string.Format("OnStart Error: {0}", ex.Message), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("OnStart Stack: {0}", ex.StackTrace), LogLevel.Error);
				Logger.Instance.Flush();
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw;
			}
		}

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Tracker.Instance.TrackException("AuthEngine.Service1", System.Reflection.MethodInfo.GetCurrentMethod().Name, (Exception)e.ExceptionObject);
        }

		protected override void OnStop()
		{
			try
			{                
				Logger.Instance.WriteToLog("Service stopped", LogLevel.Info);
				Logger.Instance.Flush();
				server.StopService();
                Tracker.Instance.StopTracking();
			}
			catch (Exception ex)
			{
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			base.OnStop();
		}
	}
}
