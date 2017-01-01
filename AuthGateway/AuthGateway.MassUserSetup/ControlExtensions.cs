using System.Windows.Forms;

namespace AuthGateway.MassUserSetup
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
