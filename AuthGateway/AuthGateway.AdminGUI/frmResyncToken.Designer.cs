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
	partial class frmResyncToken : System.Windows.Forms.Form
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
			this.tbToken2 = new System.Windows.Forms.TextBox();
			this.Label2 = new System.Windows.Forms.Label();
			this.Label1 = new System.Windows.Forms.Label();
			this.lbToken2 = new System.Windows.Forms.Label();
			this.btnResync = new System.Windows.Forms.Button();
			this.tbToken1 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.lbDevice = new System.Windows.Forms.Label();
			this.TableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// TableLayoutPanel1
			// 
			this.TableLayoutPanel1.ColumnCount = 2;
			this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.TableLayoutPanel1.Controls.Add(this.tbToken2, 1, 3);
			this.TableLayoutPanel1.Controls.Add(this.Label2, 0, 0);
			this.TableLayoutPanel1.Controls.Add(this.Label1, 0, 2);
			this.TableLayoutPanel1.Controls.Add(this.lbToken2, 0, 3);
			this.TableLayoutPanel1.Controls.Add(this.btnResync, 1, 4);
			this.TableLayoutPanel1.Controls.Add(this.tbToken1, 1, 2);
			this.TableLayoutPanel1.Controls.Add(this.label4, 0, 1);
			this.TableLayoutPanel1.Controls.Add(this.lbDevice, 1, 1);
			this.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.TableLayoutPanel1.Name = "TableLayoutPanel1";
			this.TableLayoutPanel1.RowCount = 5;
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 17F));
			this.TableLayoutPanel1.Size = new System.Drawing.Size(291, 164);
			this.TableLayoutPanel1.TabIndex = 0;
			// 
			// tbToken2
			// 
			this.tbToken2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbToken2.Location = new System.Drawing.Point(148, 101);
			this.tbToken2.Name = "tbToken2";
			this.tbToken2.Size = new System.Drawing.Size(140, 20);
			this.tbToken2.TabIndex = 5;
			// 
			// Label2
			// 
			this.Label2.AutoSize = true;
			this.TableLayoutPanel1.SetColumnSpan(this.Label2, 2);
			this.Label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Label2.Location = new System.Drawing.Point(3, 0);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(285, 40);
			this.Label2.TabIndex = 1;
			this.Label2.Text = "Generate two tokens and write them here to resync the counter:";
			this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// Label1
			// 
			this.Label1.AutoSize = true;
			this.Label1.Dock = System.Windows.Forms.DockStyle.Right;
			this.Label1.Location = new System.Drawing.Point(92, 69);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(50, 29);
			this.Label1.TabIndex = 0;
			this.Label1.Text = "Token 1:";
			this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lbToken2
			// 
			this.lbToken2.AutoSize = true;
			this.lbToken2.Dock = System.Windows.Forms.DockStyle.Right;
			this.lbToken2.Location = new System.Drawing.Point(92, 98);
			this.lbToken2.Name = "lbToken2";
			this.lbToken2.Size = new System.Drawing.Size(50, 30);
			this.lbToken2.TabIndex = 2;
			this.lbToken2.Text = "Token 2:";
			this.lbToken2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnResync
			// 
			this.btnResync.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnResync.Location = new System.Drawing.Point(213, 131);
			this.btnResync.Name = "btnResync";
			this.btnResync.Size = new System.Drawing.Size(75, 30);
			this.btnResync.TabIndex = 3;
			this.btnResync.Text = "Resync";
			this.btnResync.UseVisualStyleBackColor = true;
			this.btnResync.Click += new System.EventHandler(this.btnResync_Click);
			// 
			// tbToken1
			// 
			this.tbToken1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbToken1.Location = new System.Drawing.Point(148, 72);
			this.tbToken1.Name = "tbToken1";
			this.tbToken1.Size = new System.Drawing.Size(140, 20);
			this.tbToken1.TabIndex = 4;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Dock = System.Windows.Forms.DockStyle.Right;
			this.label4.Location = new System.Drawing.Point(98, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(44, 29);
			this.label4.TabIndex = 6;
			this.label4.Text = "Device:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lbDevice
			// 
			this.lbDevice.AutoSize = true;
			this.lbDevice.Dock = System.Windows.Forms.DockStyle.Left;
			this.lbDevice.Location = new System.Drawing.Point(148, 40);
			this.lbDevice.Name = "lbDevice";
			this.lbDevice.Size = new System.Drawing.Size(0, 29);
			this.lbDevice.TabIndex = 7;
			this.lbDevice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// frmResyncToken
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(291, 164);
			this.Controls.Add(this.TableLayoutPanel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmResyncToken";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Resync Existing Token";
			this.TableLayoutPanel1.ResumeLayout(false);
			this.TableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label lbToken2;
		private System.Windows.Forms.Button btnResync;
		private System.Windows.Forms.TextBox tbToken2;
		private System.Windows.Forms.TextBox tbToken1;
		private Label label4;
		private Label lbDevice;
	}
}
