using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
namespace AuthGateway.AdminGUI
{
	partial class ucOATHCalc : System.Windows.Forms.UserControl
	{

		//UserControl overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]
		protected override void Dispose(bool disposing)
		{
			try {
				if (disposing && components != null) {
					components.Dispose();
				}
			} finally {
				base.Dispose(disposing);
			}
		}

		//Required by the Windows Form Designer

		private System.ComponentModel.IContainer components = null;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.  
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
            this.panelOATHCalc = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.nudTotpWindow = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbDevicename = new System.Windows.Forms.TextBox();
            this.btnGenerateToken = new System.Windows.Forms.Button();
            this.cbAuthenticator = new System.Windows.Forms.ComboBox();
            this.cbHexEncoded = new System.Windows.Forms.CheckBox();
            this.btnCopyQR = new System.Windows.Forms.Button();
            this.btnResync = new System.Windows.Forms.Button();
            this.panelTokenGenType = new System.Windows.Forms.Panel();
            this.rbHOTP = new System.Windows.Forms.RadioButton();
            this.rbTOTP = new System.Windows.Forms.RadioButton();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.lblSharedSecret = new System.Windows.Forms.Label();
            this.panelQR = new System.Windows.Forms.Panel();
            this.tbSharedSecret = new System.Windows.Forms.TextBox();
            this.panelOATHCalc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTotpWindow)).BeginInit();
            this.panelTokenGenType.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelOATHCalc
            // 
            this.panelOATHCalc.Controls.Add(this.label3);
            this.panelOATHCalc.Controls.Add(this.nudTotpWindow);
            this.panelOATHCalc.Controls.Add(this.label2);
            this.panelOATHCalc.Controls.Add(this.label1);
            this.panelOATHCalc.Controls.Add(this.tbDevicename);
            this.panelOATHCalc.Controls.Add(this.btnGenerateToken);
            this.panelOATHCalc.Controls.Add(this.cbAuthenticator);
            this.panelOATHCalc.Controls.Add(this.cbHexEncoded);
            this.panelOATHCalc.Controls.Add(this.btnCopyQR);
            this.panelOATHCalc.Controls.Add(this.btnResync);
            this.panelOATHCalc.Controls.Add(this.panelTokenGenType);
            this.panelOATHCalc.Controls.Add(this.Label8);
            this.panelOATHCalc.Controls.Add(this.Label5);
            this.panelOATHCalc.Controls.Add(this.lblSharedSecret);
            this.panelOATHCalc.Controls.Add(this.panelQR);
            this.panelOATHCalc.Controls.Add(this.tbSharedSecret);
            this.panelOATHCalc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelOATHCalc.Location = new System.Drawing.Point(0, 0);
            this.panelOATHCalc.Name = "panelOATHCalc";
            this.panelOATHCalc.Size = new System.Drawing.Size(386, 180);
            this.panelOATHCalc.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(135, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Hex (Base 16) secret";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudTotpWindow
            // 
            this.nudTotpWindow.Location = new System.Drawing.Point(90, 115);
            this.nudTotpWindow.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.nudTotpWindow.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTotpWindow.Name = "nudTotpWindow";
            this.nudTotpWindow.Size = new System.Drawing.Size(43, 20);
            this.nudTotpWindow.TabIndex = 6;
            this.nudTotpWindow.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "TOTP Window:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label2.Click += new System.EventHandler(this.Label2Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Device Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbDevicename
            // 
            this.tbDevicename.Location = new System.Drawing.Point(89, 3);
            this.tbDevicename.Name = "tbDevicename";
            this.tbDevicename.Size = new System.Drawing.Size(100, 20);
            this.tbDevicename.TabIndex = 1;
            // 
            // btnGenerateToken
            // 
            this.btnGenerateToken.Location = new System.Drawing.Point(140, 150);
            this.btnGenerateToken.Name = "btnGenerateToken";
            this.btnGenerateToken.Size = new System.Drawing.Size(135, 27);
            this.btnGenerateToken.TabIndex = 9;
            this.btnGenerateToken.Text = "Generate Shared Secret";
            this.btnGenerateToken.UseVisualStyleBackColor = true;
            this.btnGenerateToken.Click += new System.EventHandler(this.btnGenerateToken_Click);
            // 
            // cbAuthenticator
            // 
            this.cbAuthenticator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAuthenticator.FormattingEnabled = true;
            this.cbAuthenticator.Location = new System.Drawing.Point(89, 64);
            this.cbAuthenticator.Name = "cbAuthenticator";
            this.cbAuthenticator.Size = new System.Drawing.Size(168, 21);
            this.cbAuthenticator.TabIndex = 4;
            this.cbAuthenticator.SelectedIndexChanged += new System.EventHandler(this.cbAuthenticator_SelectedIndexChanged);
            // 
            // cbHexEncoded
            // 
            this.cbHexEncoded.AutoSize = true;
            this.cbHexEncoded.Location = new System.Drawing.Point(244, 118);
            this.cbHexEncoded.Name = "cbHexEncoded";
            this.cbHexEncoded.Size = new System.Drawing.Size(15, 14);
            this.cbHexEncoded.TabIndex = 7;
            this.cbHexEncoded.UseVisualStyleBackColor = true;
            // 
            // btnCopyQR
            // 
            this.btnCopyQR.Location = new System.Drawing.Point(314, 142);
            this.btnCopyQR.Name = "btnCopyQR";
            this.btnCopyQR.Size = new System.Drawing.Size(69, 35);
            this.btnCopyQR.TabIndex = 10;
            this.btnCopyQR.Text = "Copy to Clipboard";
            this.btnCopyQR.UseVisualStyleBackColor = true;
            this.btnCopyQR.Click += new System.EventHandler(this.btnCopyQR_Click);
            // 
            // btnResync
            // 
            this.btnResync.Location = new System.Drawing.Point(4, 150);
            this.btnResync.Name = "btnResync";
            this.btnResync.Size = new System.Drawing.Size(133, 27);
            this.btnResync.TabIndex = 8;
            this.btnResync.Text = "Resync existing token";
            this.btnResync.UseVisualStyleBackColor = true;
            this.btnResync.Click += new System.EventHandler(this.btnResync_Click);
            // 
            // panelTokenGenType
            // 
            this.panelTokenGenType.Controls.Add(this.rbHOTP);
            this.panelTokenGenType.Controls.Add(this.rbTOTP);
            this.panelTokenGenType.Location = new System.Drawing.Point(122, 24);
            this.panelTokenGenType.Name = "panelTokenGenType";
            this.panelTokenGenType.Size = new System.Drawing.Size(135, 38);
            this.panelTokenGenType.TabIndex = 10;
            // 
            // rbHOTP
            // 
            this.rbHOTP.AutoSize = true;
            this.rbHOTP.Location = new System.Drawing.Point(3, 3);
            this.rbHOTP.Name = "rbHOTP";
            this.rbHOTP.Size = new System.Drawing.Size(55, 17);
            this.rbHOTP.TabIndex = 2;
            this.rbHOTP.TabStop = true;
            this.rbHOTP.Text = "HOTP";
            this.rbHOTP.UseVisualStyleBackColor = true;
            this.rbHOTP.CheckedChanged += new System.EventHandler(this.rbHOTP_CheckedChanged);
            // 
            // rbTOTP
            // 
            this.rbTOTP.AutoSize = true;
            this.rbTOTP.Location = new System.Drawing.Point(3, 20);
            this.rbTOTP.Name = "rbTOTP";
            this.rbTOTP.Size = new System.Drawing.Size(114, 17);
            this.rbTOTP.TabIndex = 3;
            this.rbTOTP.TabStop = true;
            this.rbTOTP.Text = "TOTP (time-based)";
            this.rbTOTP.UseVisualStyleBackColor = true;
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Location = new System.Drawing.Point(6, 68);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(73, 13);
            this.Label8.TabIndex = 7;
            this.Label8.Text = "Authenticator:";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(4, 26);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(117, 13);
            this.Label5.TabIndex = 6;
            this.Label5.Text = "Token generation type:";
            // 
            // lblSharedSecret
            // 
            this.lblSharedSecret.AutoSize = true;
            this.lblSharedSecret.Location = new System.Drawing.Point(4, 92);
            this.lblSharedSecret.Name = "lblSharedSecret";
            this.lblSharedSecret.Size = new System.Drawing.Size(78, 13);
            this.lblSharedSecret.TabIndex = 5;
            this.lblSharedSecret.Text = "Shared Secret:";
            this.lblSharedSecret.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelQR
            // 
            this.panelQR.Location = new System.Drawing.Point(265, 21);
            this.panelQR.Name = "panelQR";
            this.panelQR.Size = new System.Drawing.Size(118, 114);
            this.panelQR.TabIndex = 4;
            // 
            // tbSharedSecret
            // 
            this.tbSharedSecret.Location = new System.Drawing.Point(89, 89);
            this.tbSharedSecret.Name = "tbSharedSecret";
            this.tbSharedSecret.Size = new System.Drawing.Size(168, 20);
            this.tbSharedSecret.TabIndex = 5;
            this.tbSharedSecret.TextChanged += new System.EventHandler(this.tbSharedSecret_TextChanged);
            // 
            // ucOATHCalc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelOATHCalc);
            this.Name = "ucOATHCalc";
            this.Size = new System.Drawing.Size(386, 180);
            this.Load += new System.EventHandler(this.ucOATHCalc_Load);
            this.panelOATHCalc.ResumeLayout(false);
            this.panelOATHCalc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTotpWindow)).EndInit();
            this.panelTokenGenType.ResumeLayout(false);
            this.panelTokenGenType.PerformLayout();
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.Panel panelOATHCalc;
		private System.Windows.Forms.Panel panelTokenGenType;
		private System.Windows.Forms.RadioButton rbHOTP;
		private System.Windows.Forms.RadioButton rbTOTP;
		private System.Windows.Forms.Label Label8;
		private System.Windows.Forms.Label Label5;
		private System.Windows.Forms.Label lblSharedSecret;
		private System.Windows.Forms.Panel panelQR;
		public System.Windows.Forms.TextBox tbSharedSecret;
		private System.Windows.Forms.Button btnResync;
		private System.Windows.Forms.Button btnCopyQR;
		private System.Windows.Forms.CheckBox cbHexEncoded;
		private System.Windows.Forms.ComboBox cbAuthenticator;
		private System.Windows.Forms.Button btnGenerateToken;
		private Label label1;
		private TextBox tbDevicename;
		private System.Windows.Forms.NumericUpDown nudTotpWindow;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
	}
}
