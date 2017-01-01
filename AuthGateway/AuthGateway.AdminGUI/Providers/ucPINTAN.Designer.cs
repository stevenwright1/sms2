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
	partial class ucPINTAN : System.Windows.Forms.UserControl
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
			this.btnGenerateSheets = new System.Windows.Forms.Button();
			this.btnInvalidateAllSheets = new System.Windows.Forms.Button();
			this.MigraDocPrintDocument1 = new MigraDoc.Rendering.Printing.MigraDocPrintDocument();
			this.btnInvalidateSheets = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			//btnGenerateSheets
			//
			this.btnGenerateSheets.Location = new System.Drawing.Point(14, 46);
			this.btnGenerateSheets.Name = "btnGenerateSheets";
			this.btnGenerateSheets.Size = new System.Drawing.Size(121, 23);
			this.btnGenerateSheets.TabIndex = 0;
			this.btnGenerateSheets.Text = "Generate Sheets";
			this.btnGenerateSheets.UseVisualStyleBackColor = true;
			this.btnGenerateSheets.Click += new System.EventHandler(btnGenerateSheets_Click);
			//
			//btnInvalidateAllSheets
			//
			this.btnInvalidateAllSheets.Location = new System.Drawing.Point(14, 75);
			this.btnInvalidateAllSheets.Name = "btnInvalidateAllSheets";
			this.btnInvalidateAllSheets.Size = new System.Drawing.Size(121, 23);
			this.btnInvalidateAllSheets.TabIndex = 1;
			this.btnInvalidateAllSheets.Text = "Invalidate All Sheets";
			this.btnInvalidateAllSheets.UseVisualStyleBackColor = true;
			this.btnInvalidateAllSheets.Click += new System.EventHandler(btnInvalidateAllSheets_Click);
			//
			//MigraDocPrintDocument1
			//
			this.MigraDocPrintDocument1.Renderer = null;
			this.MigraDocPrintDocument1.SelectedPage = 0;
			//
			//btnInvalidateSheets
			//
			this.btnInvalidateSheets.Location = new System.Drawing.Point(14, 104);
			this.btnInvalidateSheets.Name = "btnInvalidateSheets";
			this.btnInvalidateSheets.Size = new System.Drawing.Size(121, 23);
			this.btnInvalidateSheets.TabIndex = 2;
			this.btnInvalidateSheets.Text = "Invalidate Sheets";
			this.btnInvalidateSheets.UseVisualStyleBackColor = true;
			this.btnInvalidateSheets.Click += new System.EventHandler(btnInvalidateSheets_Click);
			//
			//ucPINTAN
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnInvalidateSheets);
			this.Controls.Add(this.btnInvalidateAllSheets);
			this.Controls.Add(this.btnGenerateSheets);
			this.Name = "ucPINTAN";
			this.Size = new System.Drawing.Size(386, 163);
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.Button btnGenerateSheets;
		private System.Windows.Forms.Button btnInvalidateAllSheets;
		private MigraDoc.Rendering.Printing.MigraDocPrintDocument MigraDocPrintDocument1;
		private System.Windows.Forms.Button btnInvalidateSheets;
	}
}
