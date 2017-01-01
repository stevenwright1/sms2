using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using Microsoft.VisualBasic;
using Mindscape.Raygun4Net;

namespace AuthGateway.AdminGUI
{

	public partial class Splash
	{
		[System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
		private static extern System.IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

		[System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		private static extern bool DeleteObject(System.IntPtr hObject);

		public Splash()
		{
			InitializeComponent();
			Paint += Splash_Paint;
			Load += Splash_Load;
		}

		private void Splash_Load(object sender, System.EventArgs e)
		{
			this.Show();

			this.Cursor = Cursors.AppStarting;

			//th = New Thread(AddressOf LoadData)
			//th.Start()

			LoadData();
		}

		private delegate void DoFromThreadDelegate();
		public new void Hide()
		{
			if (this.InvokeRequired) {
				this.Invoke(new DoFromThreadDelegate(Hide));
			} else {
				base.Hide();
			}
		}

		public void SetCursorDefault()
		{
			if (this.InvokeRequired) {
				this.Invoke(new DoFromThreadDelegate(SetCursorDefault));
			} else {
				this.Cursor = Cursors.Default;
			}
		}

		private void LoadData()
		{
			Logger.Instance.SetLogLevel(LogLevel.Debug);
			string startUpPath = Assembly.GetExecutingAssembly().Location;
			startUpPath = Directory.GetParent(Directory.GetParent(startUpPath).FullName).FullName;
			startUpPath = startUpPath.TrimEnd(Path.DirectorySeparatorChar);
			string logPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar.ToString() + "Wright.AdminGUI" + Path.DirectorySeparatorChar.ToString();
			Logger.Instance.AddLogger(new FileLogger(logPath, "AdminGui"), LogLevel.Debug);
			Logger.Instance.WriteToLog("LoadData", LogLevel.Debug);
			try {
				Logger.Instance.WriteToLog("LoadServerSettings:Start", LogLevel.Debug);
				Variables clientLogic = new Variables(Application.StartupPath);

				Logger.Instance.WriteToLog("LoadServerSettings:DomainOrWG", LogLevel.Debug);
				int Typ = clientLogic.DomainOrWG();

				if (Typ == NetworkInformation.WORKGROUP) {
					clientLogic.OrgNameP = Environment.MachineName;
				} else if (Typ == NetworkInformation.DOMAIN) {
					clientLogic.OrgNameP = Environment.UserDomainName;
				}

				Logger.Instance.WriteToLog("LoadServerSettings:FrmUpdateuser", LogLevel.Debug);
				using (frmMain adminGUI = new frmMain(clientLogic)) {
					this.Hide();
					SetCursorDefault();
					adminGUI.ShowDialog();
					Logger.Instance.Flush();
				}
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("Splash Error" + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("Splash StackTrace" + ex.StackTrace, LogLevel.Error);
				//Program.HandleException(ex);
				MessageBox.Show("An unexpected error occurred. A log file was written inside '" + logPath + "'" + Environment.NewLine + "Application will shutdown.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			} finally {
				Logger.Instance.Flush();
			}
			Application.Exit();
		}

		private void Splash_Paint(System.Object sender, System.Windows.Forms.PaintEventArgs e)
		{
			System.IntPtr ptr = CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20);
			// _BoarderRaduis can be adjusted to your needs, try 15 to start.
			this.Region = System.Drawing.Region.FromHrgn(ptr);
			DeleteObject(ptr);
		}
	}
}
