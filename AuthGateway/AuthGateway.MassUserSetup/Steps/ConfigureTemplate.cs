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
	public partial class ConfigureTemplate : StepBase
	{
		public ConfigureTemplate() : base()
		{
			InitializeComponent();
		}

		public ConfigureTemplate(Main main) : this()
		{
			this.main = main;
			this.tbFrom.Text = main.EmailTemplate.Subject;
			this.tbTemplate.Text = main.EmailTemplate.Content;
			this.rbPlain.Checked = !main.EmailTemplate.IsHtml;
		}

		public override void DoNext()
		{
			main.EmailTemplate.Subject = this.tbFrom.Text;
			main.EmailTemplate.Content = this.tbTemplate.Text;
			main.EmailTemplate.IsHtml = !this.rbPlain.Checked;
		}
	}
}
