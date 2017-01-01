
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using Mindscape.Raygun4Net;

namespace AuthGateway.AdminGUI
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ApplicationExit += Application_ApplicationExit;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
                       
			Application.Run(new Splash());
		}

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Tracker.Instance.StopTracking();
        }

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			//HandleException(e.Exception);
            Tracker.Instance.TrackException("AdminGui.Program", System.Reflection.MethodInfo.GetCurrentMethod().Name, e.Exception);
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			//HandleException((Exception)e.ExceptionObject);
            Tracker.Instance.TrackException("AdminGui.Program", System.Reflection.MethodInfo.GetCurrentMethod().Name, (Exception)e.ExceptionObject);
		}

		public static void HandleException(Exception ex)
		{
			var raygun = new RaygunClient("6LxCWfxnE/HQJi+bN1E5DQ==");
			try
			{
				raygun.Send(ex, new List<string>(), frmMain.VERSION);
			}
			catch { 
                Logger.Instance.WriteToLog(ex, new List<string>() { "Raygun" }); 
            }
		}
	}
}
