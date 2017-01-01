namespace AuthGateway.AdminGUI
{
	partial class ucOATH
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
			this.cbDevice = new System.Windows.Forms.ComboBox();
			this.Label8 = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnDel = new System.Windows.Forms.Button();
            this.btnResyncDefaults = new System.Windows.Forms.Button();
            this.btnSaveDefaults = new System.Windows.Forms.Button();
			this.panelDevice = new System.Windows.Forms.Panel();
			this.ucOATHCalc1 = new AuthGateway.AdminGUI.ucOATHCalc();
			this.panelDevice.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbDevice
			// 
			this.cbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbDevice.FormattingEnabled = true;
			this.cbDevice.Location = new System.Drawing.Point(50, 2);
			this.cbDevice.Name = "cbDevice";
			this.cbDevice.Size = new System.Drawing.Size(100, 20);
			this.cbDevice.TabIndex = 17;
			this.cbDevice.SelectedIndexChanged += new System.EventHandler(this.cbDevice_SelectedIndexChanged);
			// 
			// Label8
			// 
			this.Label8.AutoSize = true;
			this.Label8.Location = new System.Drawing.Point(4, 6);
			this.Label8.Name = "Label8";
			this.Label8.Size = new System.Drawing.Size(44, 13);
			this.Label8.TabIndex = 16;
			this.Label8.Text = "Device:";
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(156, 1);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(48, 23);
			this.btnAdd.TabIndex = 18;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnDel
			// 
			this.btnDel.Location = new System.Drawing.Point(210, 1);
			this.btnDel.Name = "btnDel";
			this.btnDel.Size = new System.Drawing.Size(59, 23);
			this.btnDel.TabIndex = 19;
			this.btnDel.Text = "Delete";
			this.btnDel.UseVisualStyleBackColor = true;
			this.btnDel.Click += new System.EventHandler(this.btnDel_Click);            
            // 
            // btnResyncDefaults
            // 
            this.btnResyncDefaults.Location = new System.Drawing.Point(275, 1);
            this.btnResyncDefaults.Name = "btnResyncDefaults";
            this.btnResyncDefaults.Size = new System.Drawing.Size(105, 23);
            this.btnResyncDefaults.TabIndex = 1;
            this.btnResyncDefaults.Text = "Resync Defaults";
            this.btnResyncDefaults.UseVisualStyleBackColor = true;
            this.btnResyncDefaults.Visible = false;
            this.btnResyncDefaults.Click += new System.EventHandler(this.btnResyncDefaults_Click);
            // 
            // btnSaveDefaults
            // 
            this.btnSaveDefaults.Location = new System.Drawing.Point(275, 1);
            this.btnSaveDefaults.Name = "btnSaveDefaults";
            this.btnSaveDefaults.Size = new System.Drawing.Size(105, 23);
            this.btnSaveDefaults.TabIndex = 1;
            this.btnSaveDefaults.Text = "Save Defaults";
            this.btnSaveDefaults.UseVisualStyleBackColor = true;
            this.btnSaveDefaults.Click += new System.EventHandler(this.btnSaveDefaults_Click);
			// 
			// panelDevice
			// 
			this.panelDevice.Controls.Add(this.ucOATHCalc1);
			this.panelDevice.Location = new System.Drawing.Point(0, 33);
			this.panelDevice.Name = "panelDevice";
			this.panelDevice.Size = new System.Drawing.Size(386, 179);
			this.panelDevice.TabIndex = 20;
			// 
			// ucOATHCalc1
			// 
			this.ucOATHCalc1.Location = new System.Drawing.Point(0, -3);
			this.ucOATHCalc1.Name = "ucOATHCalc1";
			this.ucOATHCalc1.Size = new System.Drawing.Size(386, 180);
			this.ucOATHCalc1.TabIndex = 0;
			// 
			// ucOATH
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelDevice);
			this.Controls.Add(this.btnDel);
			this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnResyncDefaults);
            this.Controls.Add(this.btnSaveDefaults);
			this.Controls.Add(this.cbDevice);
			this.Controls.Add(this.Label8);
			this.Name = "ucOATH";
			this.Size = new System.Drawing.Size(386, 212);
            this.Load += new System.EventHandler(this.ucOATH_Load);
			this.panelDevice.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cbDevice;
		private System.Windows.Forms.Label Label8;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnResyncDefaults;
        private System.Windows.Forms.Button btnSaveDefaults;
		private System.Windows.Forms.Panel panelDevice;
		private ucOATHCalc ucOATHCalc1;
	}
}
