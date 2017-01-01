namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	partial class AuthEngineConfig02
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tbTestDestination = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSendTestEmail = new System.Windows.Forms.Button();
			this.cbNHS = new System.Windows.Forms.RadioButton();
			this.cbCustom = new System.Windows.Forms.RadioButton();
			this.tbPassword = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.tbUsername = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.cbUseAuth = new System.Windows.Forms.CheckBox();
			this.tbFrom = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.cbUseSSL = new System.Windows.Forms.CheckBox();
			this.tbPort = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbServer = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
			this.groupBox1.Controls.Add(this.tbTestDestination);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.btnSendTestEmail);
			this.groupBox1.Controls.Add(this.cbNHS);
			this.groupBox1.Controls.Add(this.cbCustom);
			this.groupBox1.Controls.Add(this.tbPassword);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.tbUsername);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.cbUseAuth);
			this.groupBox1.Controls.Add(this.tbFrom);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.cbUseSSL);
			this.groupBox1.Controls.Add(this.tbPort);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.tbServer);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(395, 259);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "E-Mail Configuration";
			// 
			// tbTestDestination
			// 
			this.tbTestDestination.Location = new System.Drawing.Point(97, 233);
			this.tbTestDestination.Name = "tbTestDestination";
			this.tbTestDestination.Size = new System.Drawing.Size(157, 20);
			this.tbTestDestination.TabIndex = 10;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 236);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 13);
			this.label1.TabIndex = 45;
			this.label1.Text = "Test destination:";
			// 
			// btnSendTestEmail
			// 
			this.btnSendTestEmail.Location = new System.Drawing.Point(260, 231);
			this.btnSendTestEmail.Name = "btnSendTestEmail";
			this.btnSendTestEmail.Size = new System.Drawing.Size(115, 23);
			this.btnSendTestEmail.TabIndex = 11;
			this.btnSendTestEmail.Text = "Send test e-mail";
			this.btnSendTestEmail.UseVisualStyleBackColor = true;
			this.btnSendTestEmail.Click += new System.EventHandler(this.btnSendTestEmail_Click);
			// 
			// cbNHS
			// 
			this.cbNHS.AutoSize = true;
			this.cbNHS.Location = new System.Drawing.Point(100, 20);
			this.cbNHS.Name = "cbNHS";
			this.cbNHS.Size = new System.Drawing.Size(81, 17);
			this.cbNHS.TabIndex = 2;
			this.cbNHS.TabStop = true;
			this.cbNHS.Text = "NHS Preset";
			this.cbNHS.UseVisualStyleBackColor = true;
			this.cbNHS.CheckedChanged += new System.EventHandler(this.cbNHS_CheckedChanged);
			// 
			// cbCustom
			// 
			this.cbCustom.AutoSize = true;
			this.cbCustom.Location = new System.Drawing.Point(9, 20);
			this.cbCustom.Name = "cbCustom";
			this.cbCustom.Size = new System.Drawing.Size(60, 17);
			this.cbCustom.TabIndex = 1;
			this.cbCustom.TabStop = true;
			this.cbCustom.Text = "Manual";
			this.cbCustom.UseVisualStyleBackColor = true;
			this.cbCustom.CheckedChanged += new System.EventHandler(this.cbCustom_CheckedChanged);
			// 
			// tbPassword
			// 
			this.tbPassword.Location = new System.Drawing.Point(72, 190);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.Size = new System.Drawing.Size(157, 20);
			this.tbPassword.TabIndex = 9;
			this.tbPassword.UseSystemPasswordChar = true;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(6, 193);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(56, 13);
			this.label8.TabIndex = 40;
			this.label8.Text = "Password:";
			// 
			// tbUsername
			// 
			this.tbUsername.Location = new System.Drawing.Point(72, 164);
			this.tbUsername.Name = "tbUsername";
			this.tbUsername.Size = new System.Drawing.Size(157, 20);
			this.tbUsername.TabIndex = 8;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(6, 167);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(58, 13);
			this.label9.TabIndex = 38;
			this.label9.Text = "Username:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 144);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(54, 13);
			this.label7.TabIndex = 37;
			this.label7.Text = "Use Auth:";
			// 
			// cbUseAuth
			// 
			this.cbUseAuth.AutoSize = true;
			this.cbUseAuth.Location = new System.Drawing.Point(72, 144);
			this.cbUseAuth.Name = "cbUseAuth";
			this.cbUseAuth.Size = new System.Drawing.Size(15, 14);
			this.cbUseAuth.TabIndex = 7;
			this.cbUseAuth.UseVisualStyleBackColor = true;
			// 
			// tbFrom
			// 
			this.tbFrom.Location = new System.Drawing.Point(72, 116);
			this.tbFrom.Name = "tbFrom";
			this.tbFrom.Size = new System.Drawing.Size(157, 20);
			this.tbFrom.TabIndex = 6;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 119);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(33, 13);
			this.label5.TabIndex = 34;
			this.label5.Text = "From:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(30, 13);
			this.label4.TabIndex = 33;
			this.label4.Text = "SSL:";
			// 
			// cbUseSSL
			// 
			this.cbUseSSL.AutoSize = true;
			this.cbUseSSL.Location = new System.Drawing.Point(72, 96);
			this.cbUseSSL.Name = "cbUseSSL";
			this.cbUseSSL.Size = new System.Drawing.Size(15, 14);
			this.cbUseSSL.TabIndex = 5;
			this.cbUseSSL.UseVisualStyleBackColor = true;
			// 
			// tbPort
			// 
			this.tbPort.Location = new System.Drawing.Point(72, 69);
			this.tbPort.Name = "tbPort";
			this.tbPort.Size = new System.Drawing.Size(157, 20);
			this.tbPort.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 13);
			this.label3.TabIndex = 30;
			this.label3.Text = "Port:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 29;
			this.label2.Text = "Server:";
			// 
			// tbServer
			// 
			this.tbServer.Location = new System.Drawing.Point(72, 45);
			this.tbServer.Name = "tbServer";
			this.tbServer.Size = new System.Drawing.Size(157, 20);
			this.tbServer.TabIndex = 3;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 136F));
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 265F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(401, 363);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// AuthEngineConfig02
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "AuthEngineConfig02";
			this.Size = new System.Drawing.Size(401, 363);
			this.Load += new System.EventHandler(this.AuthEngineConfig02_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TextBox tbPassword;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox tbUsername;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox cbUseAuth;
		private System.Windows.Forms.TextBox tbFrom;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox cbUseSSL;
		private System.Windows.Forms.TextBox tbPort;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbServer;
		private System.Windows.Forms.RadioButton cbNHS;
		private System.Windows.Forms.RadioButton cbCustom;
		private System.Windows.Forms.Button btnSendTestEmail;
		private System.Windows.Forms.TextBox tbTestDestination;
		private System.Windows.Forms.Label label1;
	}
}
