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
	public partial class EmailTest : StepBase
	{
		public EmailTest()
		{
			InitializeComponent();
		}
		public EmailTest(Main main)
			: this()
		{
			this.main = main;
			this.cbUsername.DataSource = this.main.Users;
			this.cbUsername.DisplayMember = "Username";
			this.cbUsername.ValueMember = "Username";
		}

		private void btnTestEmail_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(
				"This will set the specified username settings as a part of the test. Do you want to continue?",
				"SMS2", 
				MessageBoxButtons.OK, 
				MessageBoxIcon.Warning
				) != DialogResult.OK)
				return;

			if (cbUsername.SelectedValue == null)
				return;

			var toUser = new ToUser()
			{
				Username = cbUsername.SelectedValue.ToString(),
				Fullname = tbFullname.Text,
				Email = tbEmail.Text,
				Status = SendStatus.NotSent,
			};

			/*
			 * $fullname
			 * $username
			 * $email
			 * 
			 */
			this.main.Configurator.Configurate(toUser);
			var emailSender = new SmtpMailSender(this.main.Config.EmailConfig, this.main.EmailTemplate);
			emailSender.Send(toUser);
		}
	}
}
