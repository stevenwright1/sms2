namespace AuthGateway.MassUserSetup.Steps
{
	partial class ConfigureAuth
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
			this.tlp = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.flp = new System.Windows.Forms.FlowLayoutPanel();
			this.rbCloudSMS = new System.Windows.Forms.RadioButton();
			this.rbOATHCalc = new System.Windows.Forms.RadioButton();
			this.rbEmail = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panelAuthenticator = new System.Windows.Forms.Panel();
			this.rbGoogleAuthenticator = new System.Windows.Forms.RadioButton();
			this.rbNoBase = new System.Windows.Forms.RadioButton();
			this.panelTokenGenType = new System.Windows.Forms.Panel();
			this.rbHOTP = new System.Windows.Forms.RadioButton();
			this.rbTOTP = new System.Windows.Forms.RadioButton();
			this.Label8 = new System.Windows.Forms.Label();
			this.Label5 = new System.Windows.Forms.Label();
			this.panelQR = new System.Windows.Forms.Panel();
			this.tlp.SuspendLayout();
			this.flp.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panelAuthenticator.SuspendLayout();
			this.panelTokenGenType.SuspendLayout();
			this.SuspendLayout();
			// 
			// tlp
			// 
			this.tlp.ColumnCount = 1;
			this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlp.Controls.Add(this.label1, 0, 0);
			this.tlp.Controls.Add(this.flp, 0, 1);
			this.tlp.Controls.Add(this.panel1, 0, 2);
			this.tlp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlp.Location = new System.Drawing.Point(0, 0);
			this.tlp.Name = "tlp";
			this.tlp.Padding = new System.Windows.Forms.Padding(13, 9, 13, 9);
			this.tlp.RowCount = 3;
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlp.Size = new System.Drawing.Size(462, 208);
			this.tlp.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.tlp.SetColumnSpan(this.label1, 3);
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(16, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(142, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "User Auth Configuration";
			// 
			// flp
			// 
			this.flp.AutoScroll = true;
			this.flp.Controls.Add(this.rbCloudSMS);
			this.flp.Controls.Add(this.rbOATHCalc);
			this.flp.Controls.Add(this.rbEmail);
			this.flp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flp.Location = new System.Drawing.Point(16, 32);
			this.flp.Name = "flp";
			this.flp.Size = new System.Drawing.Size(430, 25);
			this.flp.TabIndex = 0;
			// 
			// rbCloudSMS
			// 
			this.rbCloudSMS.AutoSize = true;
			this.rbCloudSMS.Location = new System.Drawing.Point(3, 3);
			this.rbCloudSMS.Name = "rbCloudSMS";
			this.rbCloudSMS.Size = new System.Drawing.Size(75, 17);
			this.rbCloudSMS.TabIndex = 0;
			this.rbCloudSMS.TabStop = true;
			this.rbCloudSMS.Text = "CloudSMS";
			this.rbCloudSMS.UseVisualStyleBackColor = true;
			// 
			// rbOATHCalc
			// 
			this.rbOATHCalc.AutoSize = true;
			this.rbOATHCalc.Checked = true;
			this.rbOATHCalc.Location = new System.Drawing.Point(84, 3);
			this.rbOATHCalc.Name = "rbOATHCalc";
			this.rbOATHCalc.Size = new System.Drawing.Size(76, 17);
			this.rbOATHCalc.TabIndex = 1;
			this.rbOATHCalc.TabStop = true;
			this.rbOATHCalc.Text = "OATHCalc";
			this.rbOATHCalc.UseVisualStyleBackColor = true;
			// 
			// rbEmail
			// 
			this.rbEmail.AutoSize = true;
			this.rbEmail.Location = new System.Drawing.Point(166, 3);
			this.rbEmail.Name = "rbEmail";
			this.rbEmail.Size = new System.Drawing.Size(54, 17);
			this.rbEmail.TabIndex = 2;
			this.rbEmail.TabStop = true;
			this.rbEmail.Text = "E-Mail";
			this.rbEmail.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panelAuthenticator);
			this.panel1.Controls.Add(this.panelTokenGenType);
			this.panel1.Controls.Add(this.Label8);
			this.panel1.Controls.Add(this.Label5);
			this.panel1.Controls.Add(this.panelQR);
			this.panel1.Location = new System.Drawing.Point(16, 63);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(424, 133);
			this.panel1.TabIndex = 1;
			// 
			// panelAuthenticator
			// 
			this.panelAuthenticator.Controls.Add(this.rbGoogleAuthenticator);
			this.panelAuthenticator.Controls.Add(this.rbNoBase);
			this.panelAuthenticator.Location = new System.Drawing.Point(127, 45);
			this.panelAuthenticator.Name = "panelAuthenticator";
			this.panelAuthenticator.Size = new System.Drawing.Size(127, 33);
			this.panelAuthenticator.TabIndex = 18;
			// 
			// rbGoogleAuthenticator
			// 
			this.rbGoogleAuthenticator.AutoSize = true;
			this.rbGoogleAuthenticator.Checked = true;
			this.rbGoogleAuthenticator.Location = new System.Drawing.Point(3, 3);
			this.rbGoogleAuthenticator.Name = "rbGoogleAuthenticator";
			this.rbGoogleAuthenticator.Size = new System.Drawing.Size(125, 17);
			this.rbGoogleAuthenticator.TabIndex = 4;
			this.rbGoogleAuthenticator.TabStop = true;
			this.rbGoogleAuthenticator.Text = "Google Authenticator";
			this.rbGoogleAuthenticator.UseVisualStyleBackColor = true;
			// 
			// rbNoBase
			// 
			this.rbNoBase.AutoSize = true;
			this.rbNoBase.Location = new System.Drawing.Point(3, 17);
			this.rbNoBase.Name = "rbNoBase";
			this.rbNoBase.Size = new System.Drawing.Size(51, 17);
			this.rbNoBase.TabIndex = 5;
			this.rbNoBase.Text = "Other";
			this.rbNoBase.UseVisualStyleBackColor = true;
			// 
			// panelTokenGenType
			// 
			this.panelTokenGenType.Controls.Add(this.rbHOTP);
			this.panelTokenGenType.Controls.Add(this.rbTOTP);
			this.panelTokenGenType.Location = new System.Drawing.Point(127, 1);
			this.panelTokenGenType.Name = "panelTokenGenType";
			this.panelTokenGenType.Size = new System.Drawing.Size(122, 36);
			this.panelTokenGenType.TabIndex = 17;
			// 
			// rbHOTP
			// 
			this.rbHOTP.AutoSize = true;
			this.rbHOTP.Checked = true;
			this.rbHOTP.Location = new System.Drawing.Point(3, 3);
			this.rbHOTP.Name = "rbHOTP";
			this.rbHOTP.Size = new System.Drawing.Size(55, 17);
			this.rbHOTP.TabIndex = 2;
			this.rbHOTP.TabStop = true;
			this.rbHOTP.Text = "HOTP";
			this.rbHOTP.UseVisualStyleBackColor = true;
			// 
			// rbTOTP
			// 
			this.rbTOTP.AutoSize = true;
			this.rbTOTP.Location = new System.Drawing.Point(3, 19);
			this.rbTOTP.Name = "rbTOTP";
			this.rbTOTP.Size = new System.Drawing.Size(118, 17);
			this.rbTOTP.TabIndex = 3;
			this.rbTOTP.Text = "TOTP (time-based)";
			this.rbTOTP.UseVisualStyleBackColor = true;
			// 
			// Label8
			// 
			this.Label8.AutoSize = true;
			this.Label8.Location = new System.Drawing.Point(44, 45);
			this.Label8.Name = "Label8";
			this.Label8.Size = new System.Drawing.Size(73, 13);
			this.Label8.TabIndex = 16;
			this.Label8.Text = "Authenticator:";
			// 
			// Label5
			// 
			this.Label5.AutoSize = true;
			this.Label5.Location = new System.Drawing.Point(3, 0);
			this.Label5.Name = "Label5";
			this.Label5.Size = new System.Drawing.Size(117, 13);
			this.Label5.TabIndex = 15;
			this.Label5.Text = "Token generation type:";
			// 
			// panelQR
			// 
			this.panelQR.Location = new System.Drawing.Point(263, -5);
			this.panelQR.Name = "panelQR";
			this.panelQR.Size = new System.Drawing.Size(118, 114);
			this.panelQR.TabIndex = 12;
			// 
			// ConfigureAuth
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.tlp);
			this.Name = "ConfigureAuth";
			this.Size = new System.Drawing.Size(462, 208);
			this.tlp.ResumeLayout(false);
			this.tlp.PerformLayout();
			this.flp.ResumeLayout(false);
			this.flp.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panelAuthenticator.ResumeLayout(false);
			this.panelAuthenticator.PerformLayout();
			this.panelTokenGenType.ResumeLayout(false);
			this.panelTokenGenType.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tlp;
		private System.Windows.Forms.FlowLayoutPanel flp;
		private System.Windows.Forms.RadioButton rbCloudSMS;
		private System.Windows.Forms.RadioButton rbOATHCalc;
		private System.Windows.Forms.RadioButton rbEmail;
		private System.Windows.Forms.Panel panel1;
		internal System.Windows.Forms.Panel panelAuthenticator;
		internal System.Windows.Forms.RadioButton rbGoogleAuthenticator;
		internal System.Windows.Forms.RadioButton rbNoBase;
		internal System.Windows.Forms.Panel panelTokenGenType;
		internal System.Windows.Forms.RadioButton rbHOTP;
		internal System.Windows.Forms.RadioButton rbTOTP;
		internal System.Windows.Forms.Label Label8;
		internal System.Windows.Forms.Label Label5;
		internal System.Windows.Forms.Panel panelQR;
		private System.Windows.Forms.Label label1;
	}
}
