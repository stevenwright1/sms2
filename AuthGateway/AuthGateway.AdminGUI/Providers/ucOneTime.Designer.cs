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
	partial class ucOneTime : System.Windows.Forms.UserControl
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
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(350, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "OneTime token is generated automatically when this provider is selected.";
			// 
			// ucOneTime
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label1);
			this.Name = "ucOneTime";
			this.Size = new System.Drawing.Size(386, 163);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private Label label1;
	}
}
