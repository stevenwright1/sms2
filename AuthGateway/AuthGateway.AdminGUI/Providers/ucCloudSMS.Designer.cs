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
	partial class ucCloudSMS : System.Windows.Forms.UserControl
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
			this.Button1 = new System.Windows.Forms.Button();
			this.cbModules = new System.Windows.Forms.ComboBox();
			this.Label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// Button1
			// 
			this.Button1.Location = new System.Drawing.Point(10, 130);
			this.Button1.Name = "Button1";
			this.Button1.Size = new System.Drawing.Size(105, 23);
			this.Button1.TabIndex = 0;
			this.Button1.Text = "Send Token SMS";
			this.Button1.UseVisualStyleBackColor = true;
			this.Button1.Click += new System.EventHandler(this.Button1_Click);
			// 
			// cbModules
			// 
			this.cbModules.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbModules.FormattingEnabled = true;
			this.cbModules.Location = new System.Drawing.Point(106, 13);
			this.cbModules.Name = "cbModules";
			this.cbModules.Size = new System.Drawing.Size(160, 21);
			this.cbModules.TabIndex = 3;
			// 
			// Label1
			// 
			this.Label1.AutoSize = true;
			this.Label1.Location = new System.Drawing.Point(12, 16);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(88, 13);
			this.Label1.TabIndex = 2;
			this.Label1.Text = "Contact Module: ";
			this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ucCloudSMS
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbModules);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.Button1);
			this.Name = "ucCloudSMS";
			this.Size = new System.Drawing.Size(386, 163);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.Button Button1;
		private System.Windows.Forms.ComboBox cbModules;
		private System.Windows.Forms.Label Label1;
	}
}
