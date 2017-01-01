using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Threading;

using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;
using AuthGateway.Shared.Tracking;

namespace AuthGateway.MutualAuthImages
{
    public partial class Service1 : ServiceBase
    {
        private Server server;
        private Thread svThread = null;

        private SystemConfiguration sc;

        public Service1()
        {
            InitializeComponent();

            this.ServiceName = "WrightMutualAuthImages";

            this.CanShutdown = true;
            this.CanStop = true;
        }

        private void Startup()
        {
            Logger.Instance.EmptyLoggers();
            string path = Assembly.GetExecutingAssembly().Location;
            path = Directory.GetParent(Directory.GetParent(path).FullName).FullName;
            path = path.TrimEnd(Path.DirectorySeparatorChar);

            LogLevel logLevel = LogLevel.Debug; // TO DO: add a setting
            Logger.Instance.SetLogLevel(logLevel);
            Logger.Instance.AddLogger(new FileLogger(
                    path + @"\Logs"
                    , this.ServiceName), logLevel);
            Logger.Instance.AddLogger(new EventLogLogger(this), logLevel);

            String startUpPath = Assembly.GetExecutingAssembly().Location;
            startUpPath = Directory.GetParent(startUpPath).FullName;

            try {                
                sc = new WatchedSystemConfiguration(
                    delegate()
                    {
                        Logger.Instance.WriteToLog("RELOADING SystemConfiguration", LogLevel.All);
                        this.OnStop();
                        this.OnStart(new string[] { });
                    },
                    startUpPath);

                if (sc.ConfigFileDoesNotExists()) {
                    try { sc.WriteXMLCredentials(); }
                    catch { }
                }

                sc.LoadSettings(true);

                Logger.Instance.WriteToLog("Settings Loaded", LogLevel.Debug);

                if (sc.SendTrackingInfo) {
                    Tracker.Instance.StartTracking("MutualAuthImagesEvent");
                }
            }
            catch (SystemConfigurationParseError ex) {
                Logger.Instance.SetLogLevel(LogLevel.All);
                Logger.Instance.AddLogger(new FileLogger(
                        Directory.GetParent(startUpPath).FullName + @"\Logs"
                        , this.ServiceName), LogLevel.All);
                Logger.Instance.AddLogger(new EventLogLogger(this), LogLevel.Error);
                Logger.Instance.WriteToLog(string.Format("LogLevel set to All in all Loggers due to Configuration error."), LogLevel.Error);
                Logger.Instance.WriteToLog(string.Format("{0}", ex.Message), LogLevel.Error);
                if (ex.InnerException != null)
                    Logger.Instance.WriteToLog(string.Format("Inner error parsing configuration: {0}", ex.InnerException.Message), LogLevel.Error);
                throw new Exception("Error parsing configuration file.");
            }
        }

        protected override void OnStart(string[] args)
        {            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Startup();
            base.OnStart(args);
            Logger.Instance.WriteToLog("Service started", LogLevel.Info);

            server = new Server();
            svThread = new Thread(new ParameterizedThreadStart(server.StartService));
            svThread.IsBackground = true;
            svThread.Start(new object[] { sc });
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Tracker.Instance.TrackException("MutualAuthImages.Service1", System.Reflection.MethodInfo.GetCurrentMethod().Name, (Exception)e.ExceptionObject);
        }

        protected override void OnStop()
        {
            Logger.Instance.WriteToLog("Service stopped", LogLevel.Info);
            Logger.Instance.Flush();
            Tracker.Instance.StopTracking();
            base.OnStop();
        }
    }
}
