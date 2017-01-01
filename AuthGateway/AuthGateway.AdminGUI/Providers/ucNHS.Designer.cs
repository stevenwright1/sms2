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
	partial class ucNHS : System.Windows.Forms.UserControl
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
			this.SuspendLayout();
			//
			//Button1
			//
			this.Button1.Location = new System.Drawing.Point(3, 3);
			this.Button1.Name = "Button1";
			this.Button1.Size = new System.Drawing.Size(105, 23);
			this.Button1.TabIndex = 0;
			this.Button1.Text = "Send Token Email";
			this.Button1.UseVisualStyleBackColor = true;
			this.Button1.Click += new System.EventHandler(Button1_Click);
			//
			//ucCloudSMS
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.Button1);
			this.Name = "ucCloudSMS";
			this.Size = new System.Drawing.Size(386, 163);
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.Button Button1;
	}
}
