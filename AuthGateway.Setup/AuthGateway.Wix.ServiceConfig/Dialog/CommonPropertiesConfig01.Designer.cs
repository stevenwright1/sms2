namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	partial class CommonPropertiesConfig01
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbSendTrackingInfo = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbSendTrackingInfo);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(401, 471);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Common Properties";
            // 
            // cbSendTrackingInfo
            // 
            this.cbSendTrackingInfo.AutoSize = true;
            this.cbSendTrackingInfo.Checked = true;
            this.cbSendTrackingInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSendTrackingInfo.Location = new System.Drawing.Point(7, 20);
            this.cbSendTrackingInfo.Name = "cbSendTrackingInfo";
            this.cbSendTrackingInfo.Size = new System.Drawing.Size(236, 17);
            this.cbSendTrackingInfo.TabIndex = 0;
            this.cbSendTrackingInfo.Text = "Allow to send errors and statistics information";
            this.cbSendTrackingInfo.UseVisualStyleBackColor = true;
            // 
            // CommonPropertiesConfig01
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Name = "CommonPropertiesConfig01";
            this.Size = new System.Drawing.Size(401, 471);
            this.Load += new System.EventHandler(this.CommonPropertiesConfig01_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbSendTrackingInfo;
	}
}
