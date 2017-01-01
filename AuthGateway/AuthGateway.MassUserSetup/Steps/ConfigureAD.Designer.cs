namespace AuthGateway.MassUserSetup.Steps
{
	partial class ConfigureAD
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
			this.label1 = new System.Windows.Forms.Label();
			this.rbCurrentUser = new System.Windows.Forms.RadioButton();
			this.rbCustomConfig = new System.Windows.Forms.RadioButton();
			this.tbServer = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tbUsername = new System.Windows.Forms.TextBox();
			this.tbPassword = new System.Windows.Forms.TextBox();
			this.btnTest = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.cbGroups = new System.Windows.Forms.ComboBox();
			this.btnTestGroup = new System.Windows.Forms.Button();
			this.lblADUsers = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(13, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(177, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Active Directory Configuration";
			// 
			// rbCurrentUser
			// 
			this.rbCurrentUser.AutoSize = true;
			this.rbCurrentUser.Checked = true;
			this.rbCurrentUser.Location = new System.Drawing.Point(16, 26);
			this.rbCurrentUser.Name = "rbCurrentUser";
			this.rbCurrentUser.Size = new System.Drawing.Size(167, 17);
			this.rbCurrentUser.TabIndex = 1;
			this.rbCurrentUser.TabStop = true;
			this.rbCurrentUser.Text = "Use current user configuration";
			this.rbCurrentUser.UseVisualStyleBackColor = true;
			// 
			// rbCustomConfig
			// 
			this.rbCustomConfig.AutoSize = true;
			this.rbCustomConfig.Location = new System.Drawing.Point(16, 50);
			this.rbCustomConfig.Name = "rbCustomConfig";
			this.rbCustomConfig.Size = new System.Drawing.Size(125, 17);
			this.rbCustomConfig.TabIndex = 2;
			this.rbCustomConfig.Text = "Custom Configuration";
			this.rbCustomConfig.UseVisualStyleBackColor = true;
			this.rbCustomConfig.CheckedChanged += new System.EventHandler(this.rbCustomConfig_CheckedChanged);
			// 
			// tbServer
			// 
			this.tbServer.Location = new System.Drawing.Point(226, 50);
			this.tbServer.Name = "tbServer";
			this.tbServer.ReadOnly = true;
			this.tbServer.Size = new System.Drawing.Size(157, 20);
			this.tbServer.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(160, 53);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Server:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(160, 77);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(58, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Username:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(160, 101);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Password:";
			// 
			// tbUsername
			// 
			this.tbUsername.Location = new System.Drawing.Point(226, 74);
			this.tbUsername.Name = "tbUsername";
			this.tbUsername.ReadOnly = true;
			this.tbUsername.Size = new System.Drawing.Size(157, 20);
			this.tbUsername.TabIndex = 7;
			// 
			// tbPassword
			// 
			this.tbPassword.Location = new System.Drawing.Point(226, 99);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.ReadOnly = true;
			this.tbPassword.Size = new System.Drawing.Size(157, 20);
			this.tbPassword.TabIndex = 8;
			this.tbPassword.UseSystemPasswordChar = true;
			// 
			// btnTest
			// 
			this.btnTest.Location = new System.Drawing.Point(16, 129);
			this.btnTest.Name = "btnTest";
			this.btnTest.Size = new System.Drawing.Size(174, 23);
			this.btnTest.TabIndex = 9;
			this.btnTest.Text = "Test Configuration";
			this.btnTest.UseVisualStyleBackColor = true;
			this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(13, 160);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(177, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "Active Directory Target Group";
			// 
			// cbGroups
			// 
			this.cbGroups.FormattingEnabled = true;
			this.cbGroups.Location = new System.Drawing.Point(16, 177);
			this.cbGroups.Name = "cbGroups";
			this.cbGroups.Size = new System.Drawing.Size(174, 21);
			this.cbGroups.TabIndex = 11;
			// 
			// btnTestGroup
			// 
			this.btnTestGroup.Location = new System.Drawing.Point(16, 204);
			this.btnTestGroup.Name = "btnTestGroup";
			this.btnTestGroup.Size = new System.Drawing.Size(174, 23);
			this.btnTestGroup.TabIndex = 12;
			this.btnTestGroup.Text = "Test Configuration";
			this.btnTestGroup.UseVisualStyleBackColor = true;
			this.btnTestGroup.Click += new System.EventHandler(this.btnTestGroup_Click);
			// 
			// lblADUsers
			// 
			this.lblADUsers.AutoSize = true;
			this.lblADUsers.Location = new System.Drawing.Point(223, 209);
			this.lblADUsers.Name = "lblADUsers";
			this.lblADUsers.Size = new System.Drawing.Size(95, 13);
			this.lblADUsers.TabIndex = 13;
			this.lblADUsers.Text = "Found 1234 users.";
			this.lblADUsers.Visible = false;
			// 
			// ConfigureAD
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.lblADUsers);
			this.Controls.Add(this.btnTestGroup);
			this.Controls.Add(this.cbGroups);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.btnTest);
			this.Controls.Add(this.tbPassword);
			this.Controls.Add(this.tbUsername);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbServer);
			this.Controls.Add(this.rbCustomConfig);
			this.Controls.Add(this.rbCurrentUser);
			this.Controls.Add(this.label1);
			this.Name = "ConfigureAD";
			this.Size = new System.Drawing.Size(398, 231);
			this.Load += new System.EventHandler(this.ConfigureAD_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton rbCurrentUser;
		private System.Windows.Forms.RadioButton rbCustomConfig;
		private System.Windows.Forms.TextBox tbServer;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbUsername;
		private System.Windows.Forms.TextBox tbPassword;
		private System.Windows.Forms.Button btnTest;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cbGroups;
		private System.Windows.Forms.Button btnTestGroup;
		private System.Windows.Forms.Label lblADUsers;
	}
}
