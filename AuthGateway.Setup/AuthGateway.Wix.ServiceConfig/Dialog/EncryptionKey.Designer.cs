﻿namespace AuthGateway.Wix.ServiceConfig.Dialog
{
    partial class EncryptionKey
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EncryptionKey));
            this.tbKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGenKey = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbKey
            // 
            this.tbKey.Location = new System.Drawing.Point(16, 35);
            this.tbKey.Multiline = true;
            this.tbKey.Name = "tbKey";
            this.tbKey.Size = new System.Drawing.Size(367, 24);
            this.tbKey.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Base64 Encoded AES-256 key:";
            // 
            // btnGenKey
            // 
            this.btnGenKey.Location = new System.Drawing.Point(16, 65);
            this.btnGenKey.Name = "btnGenKey";
            this.btnGenKey.Size = new System.Drawing.Size(117, 23);
            this.btnGenKey.TabIndex = 2;
            this.btnGenKey.Text = "Generate New Key";
            this.btnGenKey.UseVisualStyleBackColor = true;
            this.btnGenKey.Click += new System.EventHandler(this.btnGenKey_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 107);
            this.label2.MaximumSize = new System.Drawing.Size(320, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(318, 91);
            this.label2.TabIndex = 3;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // EncryptionKey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnGenKey);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbKey);
            this.Name = "EncryptionKey";
            this.Size = new System.Drawing.Size(401, 298);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbKey;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnGenKey;
        private System.Windows.Forms.Label label2;
	}
}
