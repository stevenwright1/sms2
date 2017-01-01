using System;
using System.Diagnostics;
using System.Windows.Forms;

/*
 * Custom Action parameters: /productName="[ProductName]"
 * Dialog: form.ShowDialog(new WindowHandleWrapper(Context.Parameters["productName"]));
 */

namespace AuthGateway.Setup.CitrixIntegration.Helpers
{
    public class WindowHandleWrapper : IWin32Window
    {

        public IntPtr Handle { get; private set; }

        public WindowHandleWrapper(IntPtr handle)
        {
            this.Handle = handle;
        }

        public WindowHandleWrapper(string installerWindowTitle)
            : this(GetWindowHandle(installerWindowTitle)) { }

        public static IntPtr GetWindowHandle(string installerWindowTitle)
        {
            try
            {
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName.Equals("msiexec", StringComparison.OrdinalIgnoreCase)
                        && process.MainWindowTitle.Equals(installerWindowTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        return process.MainWindowHandle;
                    }
                }

                // If we made it this far, the process was not found.
                throw new ArgumentException("Unable to create WindowHandleWrapper.  Window '" + installerWindowTitle + "' not found.", "installerWindowTitle");
            }
            catch
            {
                throw;
            }
        }
    }
}
