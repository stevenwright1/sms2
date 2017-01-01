/*
 * Created by SharpDevelop.
 * User: GPB
 * Date: 11/12/2014
 * Time: 00:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using AuthGateway.AuthEngine.Logic;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;

namespace AuthGateway.AuthEngine.Cmd
{
	public class Service1
	{
		ServerAccess server;
		Thread svThread;

		string path = string.Empty;
		SystemConfiguration sc;
		
		const string ServiceName = "WrightCCS.AuthEngine.Cmd";

		public void OnStart(string[] args)
		{
			Logger.Instance.EmptyLoggers();

			Assembly thisAssembly = Assembly.GetExecutingAssembly();
			path = Directory.GetParent(thisAssembly.Location).FullName.TrimEnd(Path.DirectorySeparatorChar);

			var startUpPath = Assembly.GetExecutingAssembly().Location;
			startUpPath = Directory.GetParent(startUpPath).FullName;

			try
			{
				sc = new SystemConfiguration(Path.Combine(startUpPath, "Dummy"));
				
				if (sc.ConfigFileDoesNotExists())
				{
					try { 
						sc.WriteXMLCredentials(); 
					}
					catch { }
				}
				
				sc.LoadSettings(true);
				Logger.Instance.SetLogLevel(LogLevel.All);
				Logger.Instance.AddLogger(new ConsoleLogger(), LogLevel.All);
				Logger.Instance.AddLogger(new FileLogger(
						path
						, ServiceName, sc.LogMaxFiles, sc.LogMaxSize), sc.AuthEngineFileLogLevel);

				DBQueriesProvider.SetQueriesProvider(new DBQueriesProviderNew(sc.GetSQLConnectionString()));
			}
			catch (SystemConfigurationParseError ex)
			{
				Logger.Instance.SetLogLevel(LogLevel.All);
				Logger.Instance.EmptyLoggers();
				Logger.Instance.SetFlushOnWrite(true);
				Logger.Instance.AddLogger(new FileLogger(
						path
						, ServiceName), LogLevel.All);
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
				Logger.Instance.AddLogger(new ConsoleLogger(), LogLevel.All);
				Logger.Instance.AddLogger(new FileLogger(
						path
						, ServiceName), LogLevel.All);
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
				throw;
			}
		}

		public void OnStop()
		{
			try
			{
				Logger.Instance.WriteToLog("Service stopped", LogLevel.Info);
				Logger.Instance.Flush();
				server.StopService();
			}
			catch
			{

			}
		}
	}
	
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Starting service");
			
			var service = new Service1();
			try {
				service.OnStart(null);
				Console.ReadKey();
				Console.ReadKey();
				Console.ReadKey();
			} finally {
				service.OnStop();
			}
		}
	}
}