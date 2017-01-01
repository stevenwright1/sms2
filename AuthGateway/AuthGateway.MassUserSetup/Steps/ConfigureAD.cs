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
	public partial class ConfigureAD : StepBase
	{
		private List<AdUser> users;
		public ConfigureAD() : base()
		{
			InitializeComponent();
		}

		public ConfigureAD(Main main)
			: this()
		{
			this.main = main;
			this.users = new List<AdUser>();
		}

		private void EnableTextboxes(bool enable)
		{
			tbServer.ReadOnly = enable;
			tbUsername.ReadOnly = enable;
			tbPassword.ReadOnly = enable;
		}

		private void rbCustomConfig_CheckedChanged(object sender, EventArgs e)
		{
			if (!rbCustomConfig.Checked)
			{
				EnableTextboxes(true);
			}
			else
				EnableTextboxes(false);

		}

		private void ConfigureAD_Load(object sender, EventArgs e)
		{
			this.main.EnableNext(false);
		}

		private void btnTest_Click(object sender, EventArgs e)
		{
			List<string> groups = null;
			if (rbCustomConfig.Checked)
				groups = AdHelper.GetGroups(tbServer.Text, tbUsername.Text, tbPassword.Text);
			else
				groups = AdHelper.GetGroups();
			cbGroups.Items.AddRange(groups.ToArray());
		}

		private void btnTestGroup_Click(object sender, EventArgs e)
		{
			lblADUsers.Visible = true;
			if (rbCustomConfig.Checked)
				users = AdHelper.GetUsers(tbServer.Text, cbGroups.Text, tbUsername.Text, tbPassword.Text);
			else
				users = AdHelper.GetUsers(cbGroups.Text);
			lblADUsers.Text = string.Format("Found {0} users.", users.Count);
			
			this.main.EnableNext(true);
		}

		public override void DoNext()
		{
			this.main.Users.Clear();
			this.main.Users.AddRange(users);
		}
	}
}
