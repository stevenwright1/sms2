using System;
using System.Collections.Generic;
using System.IO;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;
using AuthGateway.Shared.Tracking;
using Mindscape.Raygun4Net;

namespace Ias.RADIUS.CSharp
{
	

	public class RadiusLogic
	{
		private static RadiusLogic instance = new RadiusLogic();

		private bool didInit;
		private RadiusLogicBackend backend;
		private RaygunClient raygun = new RaygunClient("5erFvyETNM51PW+NoGvxrA==");

		public static RadiusLogic Instance()
		{
			return instance;
		}

		private RadiusLogic()
		{
			didInit = false;
			Logger.Instance.SetFlushOnWrite(true);
			Logger.Instance.SetLogLevel(LogLevel.All);
		}

		public void Init(string path)
		{
			if (didInit)
				return;
			try
			{
				Logger.Instance.EmptyLoggers();
				this.backend = new RadiusLogicBackend(
					new WatchedSystemConfiguration(delegate()
					                               {
					                               	Logger.Instance.WriteToLog("RELOADING SystemConfiguration", LogLevel.All);
					                               	didInit = false;
					                               	this.Init(path);
					                               },
					                               path, "SettingsPublic"), raygun);

				String parentPath = Directory.GetParent(path).FullName;
				parentPath = parentPath.TrimEnd(Path.DirectorySeparatorChar);
				parentPath = parentPath + @"\Logs";

				Logger.Instance.AddLogger(new FileLogger(parentPath, "iasradius"), LogLevel.Debug);
				didInit = true;
			}
			catch(Exception ex)
			{
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				try
				{
					raygun.Send(ex, new List<string>() { "Ias.RADIUS.CSharp.RadiusLogic", "Init" });
				}
				catch { }
				//using (StreamWriter sw = File.AppendText(@"c:\\radius\\tmp\\managed.txt"))
				//{
				//    sw.WriteLine(ex.Message);
				//    sw.WriteLine(ex.StackTrace);
				//}
			}
		}

		public ActOnRet ActOn(string username, string password, string state)
		{
			return backend.ActOn(username, password, state);
		}
	}
}
