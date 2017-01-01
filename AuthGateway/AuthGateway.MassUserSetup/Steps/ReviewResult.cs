using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuthGateway.MassUserSetup.Steps
{
	public partial class ReviewResult : StepBase
	{
		public ReviewResult()
		{
			InitializeComponent();
		}
		
		public ReviewResult(Main main)
		{
			this.main = main;
		}
	}
}
