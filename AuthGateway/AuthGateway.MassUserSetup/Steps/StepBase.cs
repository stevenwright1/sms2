using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuthGateway.MassUserSetup.Steps
{
	public class StepBase : UserControl
	{
		protected Main main;

		public StepBase()
		{
			
		}

		public StepBase(Main main)
			: this()
		{
			this.main = main;
		}

		protected void showErrors(List<string> errors)
		{
			var errorsAppend = new StringBuilder();
			errorsAppend.AppendLine("An error ocurred:");
			foreach (var error in errors)
			{
				errorsAppend.AppendFormat("{0}" + Environment.NewLine, error);
			}
			MessageBox.Show(this.main, errorsAppend.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public virtual bool CanDoNext()
		{
			return true;
		}

		public virtual void DoNext()
		{
		}
	}
}
