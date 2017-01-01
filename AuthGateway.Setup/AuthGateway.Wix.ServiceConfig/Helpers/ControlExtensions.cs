using System.Windows.Forms;

namespace AuthGateway.Wix.ServiceConfig
{
	public static class ControlExtensions
	{
		public static void InvokeIfRequired(this Control control, MethodInvoker action)
		{
			if (control.InvokeRequired)
			{
				control.Invoke(action);
			}
			else
			{
				action();
			}
		}
	}
}
