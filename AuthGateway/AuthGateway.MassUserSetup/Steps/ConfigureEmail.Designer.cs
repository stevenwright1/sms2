namespace AuthGateway.MassUserSetup.Steps
{
	partial class ConfigureEmail
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
			this.tbPort = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbServer = new System.Windows.Forms.TextBox();
			this.cbUseSSL = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tbFrom = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.cbUseAuth = new System.Windows.Forms.CheckBox();
			this.tbPassword = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.tbUsername = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(13, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(121, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "E-Mail Configuration";
			// 
			// tbPort
			// 
			this.tbPort.Location = new System.Drawing.Point(79, 52);
			this.tbPort.Name = "tbPort";
			this.tbPort.Size = new System.Drawing.Size(157, 20);
			this.tbPort.TabIndex = 15;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 55);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 13);
			this.label3.TabIndex = 14;
			this.label3.Text = "Port:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 13;
			this.label2.Text = "Server:";
			// 
			// tbServer
			// 
			this.tbServer.Location = new System.Drawing.Point(79, 28);
			this.tbServer.Name = "tbServer";
			this.tbServer.Size = new System.Drawing.Size(157, 20);
			this.tbServer.TabIndex = 12;
			// 
			// cbUseSSL
			// 
			this.cbUseSSL.AutoSize = true;
			this.cbUseSSL.Location = new System.Drawing.Point(79, 79);
			this.cbUseSSL.Name = "cbUseSSL";
			this.cbUseSSL.Size = new System.Drawing.Size(15, 14);
			this.cbUseSSL.TabIndex = 16;
			this.cbUseSSL.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 79);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(30, 13);
			this.label4.TabIndex = 17;
			this.label4.Text = "SSL:";
			// 
			// tbFrom
			// 
			this.tbFrom.Location = new System.Drawing.Point(79, 99);
			this.tbFrom.Name = "tbFrom";
			this.tbFrom.Size = new System.Drawing.Size(157, 20);
			this.tbFrom.TabIndex = 19;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(13, 102);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(33, 13);
			this.label5.TabIndex = 18;
			this.label5.Text = "From:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(13, 127);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(54, 13);
			this.label7.TabIndex = 23;
			this.label7.Text = "Use Auth:";
			// 
			// cbUseAuth
			// 
			this.cbUseAuth.AutoSize = true;
			this.cbUseAuth.Location = new System.Drawing.Point(79, 127);
			this.cbUseAuth.Name = "cbUseAuth";
			this.cbUseAuth.Size = new System.Drawing.Size(15, 14);
			this.cbUseAuth.TabIndex = 22;
			this.cbUseAuth.UseVisualStyleBackColor = true;
			this.cbUseAuth.CheckedChanged += new System.EventHandler(this.cbUseAuth_CheckedChanged);
			// 
			// tbPassword
			// 
			this.tbPassword.Location = new System.Drawing.Point(79, 173);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.Size = new System.Drawing.Size(157, 20);
			this.tbPassword.TabIndex = 27;
			this.tbPassword.UseSystemPasswordChar = true;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(13, 176);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(56, 13);
			this.label8.TabIndex = 26;
			this.label8.Text = "Password:";
			// 
			// tbUsername
			// 
			this.tbUsername.Location = new System.Drawing.Point(79, 147);
			this.tbUsername.Name = "tbUsername";
			this.tbUsername.Size = new System.Drawing.Size(157, 20);
			this.tbUsername.TabIndex = 25;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(13, 150);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(58, 13);
			this.label9.TabIndex = 24;
			this.label9.Text = "Username:";
			// 
			// ConfigureEmail
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.tbPassword);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.tbUsername);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.cbUseAuth);
			this.Controls.Add(this.tbFrom);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cbUseSSL);
			this.Controls.Add(this.tbPort);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbServer);
			this.Controls.Add(this.label1);
			this.Name = "ConfigureEmail";
			this.Size = new System.Drawing.Size(479, 241);
			this.Load += new System.EventHandler(this.ConfigureEmail_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbPort;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbServer;
		private System.Windows.Forms.CheckBox cbUseSSL;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbFrom;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox cbUseAuth;
		private System.Windows.Forms.TextBox tbPassword;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox tbUsername;
		private System.Windows.Forms.Label label9;
	}
}
