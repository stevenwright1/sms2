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
	public partial class EmailSend : StepBase
	{
		private List<ToUser> users;
		public EmailSend()
		{
			InitializeComponent();
		}
		public EmailSend(Main main)
			: this()
		{
			this.main = main;
			users = new List<ToUser>();
		}

		private void bindGrid()
		{
			this.dgv.DataSource = null;
			this.dgv.ReadOnly = true;
			this.dgv.AutoGenerateColumns = false;
			this.dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			this.dgv.Columns.Add(
				new DataGridViewTextBoxColumn()
				{
					DataPropertyName = "Username",
					HeaderText = "Username",
					Name = "Username",
				});
			this.dgv.Columns.Add(
				new DataGridViewTextBoxColumn()
				{
					DataPropertyName = "Fullname",
					HeaderText = "Fullname",
					Name = "Fullname",
				});
			this.dgv.Columns.Add(
				new DataGridViewTextBoxColumn()
				{
					DataPropertyName = "Email",
					HeaderText = "Email",
					Name = "Email",
				});
			this.dgv.Columns.Add(
				new DataGridViewComboBoxColumn()
				{
					DataSource = Enum.GetValues(typeof(SendStatus)),
					ValueType = typeof(SendStatus),
					DataPropertyName = "Status",
					HeaderText = "Status",
					Name = "Status",
				});
			this.dgv.DataSource = users;
		}
		private void fillTestData()
		{
			users.Add(
				new ToUser()
				{
					Username = "jdoe",
					Fullname = "John Doe",
					Email = "jdoe@wrightccs.com",
					Status = SendStatus.NotSent
				});

		}

		private void EmailSend_Load(object sender, EventArgs e)
		{
			fillTestData();
			bindGrid();
		}
	}
}
