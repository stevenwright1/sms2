using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace SMS2.PremiumAccess
{
	public class EntryPoint
	{
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetForegroundWindow(IntPtr hWnd);

		[STAThread]
		public static void Main (string[] args)
		{
			bool createdNew = true;
			using (Mutex mutex = new Mutex(true, "SMS2PremiumAccessInstance", out createdNew)) {
				if (createdNew) {
					SMS2.PremiumAccess.App app = new SMS2.PremiumAccess.App();
					app.InitializeComponent();
					app.Run();
				} else {
					try {
						Process current = Process.GetCurrentProcess ();
						foreach (Process process in Process.GetProcessesByName(current.ProcessName)) {
							if (process.Id != current.Id) {
								SetForegroundWindow (process.MainWindowHandle);
								break;
							}
						}
					} catch { 
					}
				}
			}
		}
	}
}
