using System;
using System.Windows.Forms;
using AuthGateway.Shared;
using System.Data.SqlClient;
using AuthGateway.Shared.Identity;
using AuthGateway.Setup.SQLDB;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class AuthEngineConfig03 : UserControl, IWizardScreen
	{
		protected SystemConfiguration sc;
		protected Wizard wizard;
		public AuthEngineConfig03()
		{
			InitializeComponent();
		}
		public AuthEngineConfig03(Wizard wizard, SystemConfiguration sc)
			: this()
		{
			this.wizard = wizard;
			this.sc = sc;
		}

		private void AuthEngineConfig03_Load(object sender, EventArgs e)
		{
			tbIp.Text = sc.DbServer;
			tbPort.Text = sc.DbPort.ToString();
			tbDatabaseName.Text = sc.DatabaseName;
			tbUsername.Text = sc.DbUsername;
			tbPassword.Text = sc.DbPassword;
			cbUseIntegratedSecurity.Checked = sc.DbUseIntegratedSecurity;
			cbUseNamedPipes.Checked = sc.DbUsePipes;
            tbPipeName.Text = sc.DbPipeName;
		}

		public bool Store()
		{
			try
			{
				var databaseName = tbDatabaseName.Text.Trim();
				if (string.IsNullOrEmpty(databaseName))
				{
					throw new Exception(@"Please enter the name of a database. If it does not exist and it will be created for you.");
				}
				if (string.Compare(databaseName, "msdb", true)==0) {
					throw new Exception(@"Stop. The msdb database is used by SQL Server Agent for scheduling alerts and jobs and by other features such as SQL Server Management Studio, Service Broker and Database Mail. 
Please enter the name of a database that does not exist and it will be created for you.");
				}
				sc.SetDBData(
					tbIp.Text.Trim(), 
					tbUsername.Text.Trim(), 
					tbPassword.Text.Trim(),
					tbPort.Text.Trim(),
                    cbUseNamedPipes.Checked,
                    tbPipeName.Text.Trim(),
                    cbUseIntegratedSecurity.Checked,
					databaseName);

				return true;
			}
			catch (Exception ex)
			{
				wizard.ShowError(ex.Message);
			}
			return false;
		}

		public bool SkipNext()
		{
			return false;
		}

		public bool SkipPrevious()
		{
			return false;
		}

		public UserControl GetControl()
		{
			return this;
		}

		public IWizardScreen GetWizardScreen()
		{
			return this;
		}

		private void cbUseIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
		{
			if (cbUseIntegratedSecurity.Checked)
			{
				tbUsername.Enabled = false;
				tbPassword.Enabled = false;
			}
			else
			{
				tbUsername.Enabled = true;
				tbPassword.Enabled = true;
			}
		}

		private void cbUseNamedPipes_CheckedChanged(object sender, EventArgs e)
		{
            if (cbUseNamedPipes.Checked)
            {
                tbIp.Text = "(local)";
                tbIp.Enabled = false;
                tbPort.Enabled = false;
                tbPipeName.Enabled = true;
            }
            else
            {
                tbIp.Enabled = true;
                tbPort.Enabled = true;
                tbPipeName.Enabled = false;
            }
		}

		private void btnCheck_Click(object sender, EventArgs e)
		{
			ImpersonatedUser impersonatedUser = null;
			try
			{
				sc.SetDBData(tbIp.Text, tbUsername.Text, tbPassword.Text
						, tbPort.Text, cbUseNamedPipes.Checked, tbPipeName.Text, cbUseIntegratedSecurity.Checked, tbDatabaseName.Text);
				if (cbUseIntegratedSecurity.Checked)
				{
					var parser = new IdentityParser();
					var username = wizard.SessionValues["AEUSERNAME"];
					var password = wizard.SessionValues["AEPASSWORD"];
					var domainUsername = parser.GetDomainUserNameOrNull(username);
					try
					{
						impersonatedUser = new ImpersonatedUser(domainUsername.Username, domainUsername.Domain, password);
					}
					catch (NotImpersonableException ex)
					{
						throw new Exception(string.Format("Service accounts ({0}) cannot be tested - {1}", username, ex.Message));
					}
					catch
					{
						throw new Exception(string.Format("Error impersonating user '{0}' domain '{1}'", domainUsername.Username, domainUsername.Domain));
					}
				}
				SqlConnection conn;
				try
				{
					try
					{
						conn = new SqlConnection(sc.GetSQLConnectionString(false));
						conn.Open();
						conn.Close();
					}
					catch (Exception ex)
					{
						throw new Exception(string.Format("Could not connect with the user. - {0}", ex.Message), ex);
					}
					try
					{
						conn = new SqlConnection(sc.GetSQLConnectionString(true));
						conn.Open();
						conn.Close();
					}
					catch(SqlException sqlException)
					{
						var errorCount = sqlException.Errors.Count;
						var cb = new DatabaseHandler();
						var dbStatus = cb.DoStatus(sc.GetSQLConnectionString(false), sc.GetSQLConnectionString(true), sc.DatabaseName);
						try
						{
							if (!dbStatus.Exists)
							{
								cb.DoCreate(sc.GetSQLConnectionString(false), sc.DatabaseName);
								cb.DoDrop(sc.GetSQLConnectionString(false), sc.DatabaseName);
							}
							else
							{
								throw new Exception(string.Format(
								@"{0}", sqlException.Message), sqlException);
							}
						}
						catch(Exception ex)
						{
							throw new Exception(string.Format(
						@"Could not USE database or it does not exists and this user cannot create a database.
{0}", ex.Message), ex);
						}
					}
					if (impersonatedUser != null)
					{
						impersonatedUser.Dispose();
						impersonatedUser = null;
					}
					wizard.ShowInfo("Test SQL connection successful");
				}
				catch (Exception ex)
				{
					if (impersonatedUser != null)
					{
						impersonatedUser.Dispose();
						impersonatedUser = null;
					}
					wizard.ShowError(ex.Message);
				}
			}
			catch (Exception ex)
			{
				if (impersonatedUser != null)
				{
					impersonatedUser.Dispose();
					impersonatedUser = null;
				}
				wizard.ShowError(ex.Message);
			}
			finally
			{
				if (impersonatedUser != null)
				{
					impersonatedUser.Dispose();
					impersonatedUser = null;
				}
			}
		}

        private void tbPipeName_Enter(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int visibleTime = 15000; 

            ToolTip tooltip = new ToolTip();
            tooltip.Show("If you use a named instance of SQL Server Pipe Name format should be\n'\\\\.\\pipe\\MSSQL$[instanceName]\\sql\\query' unless the pipe name was changed.\nFor default instance keep it blank or default.", textBox, textBox.Width/2, textBox.Height, visibleTime);
        }
	}
}
