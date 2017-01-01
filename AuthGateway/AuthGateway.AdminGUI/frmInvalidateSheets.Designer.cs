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
	partial class frmInvalidateSheets : System.Windows.Forms.Form
	{

//Form overrides dispose to clean up the component list.
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
			this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.Label2 = new System.Windows.Forms.Label();
			this.btnInvalidate = new System.Windows.Forms.Button();
			this.clbSheets = new System.Windows.Forms.CheckedListBox();
			this.TableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			//
			//TableLayoutPanel1
			//
			this.TableLayoutPanel1.ColumnCount = 2;
			this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
			this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
			this.TableLayoutPanel1.Controls.Add(this.Label2, 0, 0);
			this.TableLayoutPanel1.Controls.Add(this.btnInvalidate, 1, 2);
			this.TableLayoutPanel1.Controls.Add(this.clbSheets, 0, 1);
			this.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.TableLayoutPanel1.Name = "TableLayoutPanel1";
			this.TableLayoutPanel1.RowCount = 3;
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31f));
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31f));
			this.TableLayoutPanel1.Size = new System.Drawing.Size(211, 209);
			this.TableLayoutPanel1.TabIndex = 0;
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.TableLayoutPanel1.SetColumnSpan(this.Label2, 2);
			this.Label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Label2.Location = new System.Drawing.Point(3, 0);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(205, 31);
			this.Label2.TabIndex = 1;
			this.Label2.Text = "Select sheet IDs to invalidate:";
			this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//btnInvalidate
			//
			this.btnInvalidate.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnInvalidate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.btnInvalidate.Location = new System.Drawing.Point(133, 181);
			this.btnInvalidate.Name = "btnInvalidate";
			this.btnInvalidate.Size = new System.Drawing.Size(75, 25);
			this.btnInvalidate.TabIndex = 3;
			this.btnInvalidate.Text = "Invalidate";
			this.btnInvalidate.UseVisualStyleBackColor = true;
			this.btnInvalidate.Click += new System.EventHandler(btnResync_Click);
			//
			//clbSheets
			//
			this.TableLayoutPanel1.SetColumnSpan(this.clbSheets, 2);
			this.clbSheets.Dock = System.Windows.Forms.DockStyle.Fill;
			this.clbSheets.FormattingEnabled = true;
			this.clbSheets.Location = new System.Drawing.Point(3, 34);
			this.clbSheets.Name = "clbSheets";
			this.clbSheets.Size = new System.Drawing.Size(205, 141);
			this.clbSheets.TabIndex = 4;
			//
			//frmInvalidateSheets
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(211, 209);
			this.Controls.Add(this.TableLayoutPanel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmInvalidateSheets";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "PIN/TAN - Invalidate Sheets";
			this.TableLayoutPanel1.ResumeLayout(false);
			this.TableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Button btnInvalidate;
		private System.Windows.Forms.CheckedListBox clbSheets;
	}
}
