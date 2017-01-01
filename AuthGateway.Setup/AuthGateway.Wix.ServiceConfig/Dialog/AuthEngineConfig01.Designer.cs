namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	partial class AuthEngineConfig01
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tbAuthEnginePort = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.cbAuthEngineAddress = new System.Windows.Forms.ComboBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tbAdUsername = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tbAdServer = new System.Windows.Forms.TextBox();
			this.tbAdPassword = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.tbLdapBaseDN = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.btnTestADConfig = new System.Windows.Forms.Button();
			this.tbLdapContainer = new System.Windows.Forms.TextBox();
			this.tbLdapFilter = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.ttTip = new System.Windows.Forms.ToolTip(this.components);
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbAuthEnginePort
			// 
			this.tbAuthEnginePort.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbAuthEnginePort.Location = new System.Drawing.Point(186, 32);
			this.tbAuthEnginePort.Name = "tbAuthEnginePort";
			this.tbAuthEnginePort.Size = new System.Drawing.Size(178, 20);
			this.tbAuthEnginePort.TabIndex = 2;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.tableLayoutPanel1);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(373, 78);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Network Bindings";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.tbAuthEnginePort, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.cbAuthEngineAddress, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(367, 59);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Location = new System.Drawing.Point(3, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(177, 30);
			this.label2.TabIndex = 21;
			this.label2.Text = "AuthEngine Port";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(177, 29);
			this.label1.TabIndex = 20;
			this.label1.Text = "AuthEngine Address";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cbAuthEngineAddress
			// 
			this.cbAuthEngineAddress.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cbAuthEngineAddress.FormattingEnabled = true;
			this.cbAuthEngineAddress.Location = new System.Drawing.Point(186, 3);
			this.cbAuthEngineAddress.Name = "cbAuthEngineAddress";
			this.cbAuthEngineAddress.Size = new System.Drawing.Size(178, 21);
			this.cbAuthEngineAddress.TabIndex = 1;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.tableLayoutPanel2);
			this.groupBox2.Location = new System.Drawing.Point(8, 92);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(373, 213);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Active Directory";
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this.label5, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.tbAdUsername, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.label4, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.tbAdServer, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.tbAdPassword, 1, 2);
			this.tableLayoutPanel2.Controls.Add(this.label6, 0, 3);
			this.tableLayoutPanel2.Controls.Add(this.tbLdapBaseDN, 1, 3);
			this.tableLayoutPanel2.Controls.Add(this.label7, 0, 4);
			this.tableLayoutPanel2.Controls.Add(this.btnTestADConfig, 1, 6);
			this.tableLayoutPanel2.Controls.Add(this.tbLdapContainer, 1, 4);
			this.tableLayoutPanel2.Controls.Add(this.tbLdapFilter, 1, 5);
			this.tableLayoutPanel2.Controls.Add(this.label8, 0, 5);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 7;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(367, 194);
			this.tableLayoutPanel2.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label5.Location = new System.Drawing.Point(3, 54);
			this.label5.Name = "label5";
			this.label5.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.label5.Size = new System.Drawing.Size(177, 27);
			this.label5.TabIndex = 23;
			this.label5.Text = "AD/LDAP Password";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Location = new System.Drawing.Point(3, 27);
			this.label3.Name = "label3";
			this.label3.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.label3.Size = new System.Drawing.Size(177, 27);
			this.label3.TabIndex = 21;
			this.label3.Text = " AD/LDAP Query Account";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tbAdUsername
			// 
			this.tbAdUsername.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbAdUsername.Location = new System.Drawing.Point(186, 30);
			this.tbAdUsername.Name = "tbAdUsername";
			this.tbAdUsername.Size = new System.Drawing.Size(178, 20);
			this.tbAdUsername.TabIndex = 4;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label4.Location = new System.Drawing.Point(3, 0);
			this.label4.Name = "label4";
			this.label4.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.label4.Size = new System.Drawing.Size(177, 27);
			this.label4.TabIndex = 20;
			this.label4.Text = "AD/LDAP Server";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tbAdServer
			// 
			this.tbAdServer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbAdServer.Location = new System.Drawing.Point(186, 3);
			this.tbAdServer.Name = "tbAdServer";
			this.tbAdServer.Size = new System.Drawing.Size(178, 20);
			this.tbAdServer.TabIndex = 3;
			// 
			// tbAdPassword
			// 
			this.tbAdPassword.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbAdPassword.Location = new System.Drawing.Point(186, 57);
			this.tbAdPassword.Name = "tbAdPassword";
			this.tbAdPassword.Size = new System.Drawing.Size(178, 20);
			this.tbAdPassword.TabIndex = 5;
			this.tbAdPassword.UseSystemPasswordChar = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label6.Location = new System.Drawing.Point(3, 81);
			this.label6.Name = "label6";
			this.label6.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.label6.Size = new System.Drawing.Size(177, 27);
			this.label6.TabIndex = 26;
			this.label6.Text = "AD/LDAP BaseDN";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tbLdapBaseDN
			// 
			this.tbLdapBaseDN.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbLdapBaseDN.Location = new System.Drawing.Point(186, 84);
			this.tbLdapBaseDN.Name = "tbLdapBaseDN";
			this.tbLdapBaseDN.Size = new System.Drawing.Size(178, 20);
			this.tbLdapBaseDN.TabIndex = 6;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label7.Location = new System.Drawing.Point(3, 108);
			this.label7.Name = "label7";
			this.label7.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.label7.Size = new System.Drawing.Size(177, 27);
			this.label7.TabIndex = 28;
			this.label7.Text = "AD/LDAP Container (optional)";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// btnTestADConfig
			// 
			this.btnTestADConfig.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnTestADConfig.Location = new System.Drawing.Point(205, 165);
			this.btnTestADConfig.Name = "btnTestADConfig";
			this.btnTestADConfig.Size = new System.Drawing.Size(159, 26);
			this.btnTestADConfig.TabIndex = 9;
			this.btnTestADConfig.Text = "Test AD/LDAP Config";
			this.btnTestADConfig.UseVisualStyleBackColor = true;
			this.btnTestADConfig.Click += new System.EventHandler(this.btnTestADConfig_Click);
			// 
			// tbLdapContainer
			// 
			this.tbLdapContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbLdapContainer.Location = new System.Drawing.Point(186, 111);
			this.tbLdapContainer.Name = "tbLdapContainer";
			this.tbLdapContainer.Size = new System.Drawing.Size(178, 20);
			this.tbLdapContainer.TabIndex = 7;
			// 
			// tbLdapFilter
			// 
			this.tbLdapFilter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbLdapFilter.Location = new System.Drawing.Point(186, 138);
			this.tbLdapFilter.Name = "tbLdapFilter";
			this.tbLdapFilter.Size = new System.Drawing.Size(178, 20);
			this.tbLdapFilter.TabIndex = 8;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label8.Location = new System.Drawing.Point(3, 135);
			this.label8.Name = "label8";
			this.label8.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.label8.Size = new System.Drawing.Size(177, 27);
			this.label8.TabIndex = 31;
			this.label8.Text = "AD/LDAP Filter (optional)";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// AuthEngineConfig01
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "AuthEngineConfig01";
			this.Size = new System.Drawing.Size(449, 311);
			this.Load += new System.EventHandler(this.AuthEngineConfig01_Load);
			this.groupBox1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox tbAuthEnginePort;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbAuthEngineAddress;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbAdUsername;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbAdServer;
		private System.Windows.Forms.TextBox tbAdPassword;
		private System.Windows.Forms.Button btnTestADConfig;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbLdapBaseDN;
		private System.Windows.Forms.ToolTip ttTip;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox tbLdapContainer;
		private System.Windows.Forms.TextBox tbLdapFilter;
		private System.Windows.Forms.Label label8;
	}
}
