namespace AuthGateway.Wix.ServiceConfig.Test
{
	partial class Form1
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnConfigCloudSMS = new System.Windows.Forms.Button();
			this.btnConfigOATHCalc = new System.Windows.Forms.Button();
			this.btnConfigAuthEngine = new System.Windows.Forms.Button();
			this.btnTestRemoveRegistry = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnConfigCloudSMS
			// 
			this.btnConfigCloudSMS.Location = new System.Drawing.Point(95, 60);
			this.btnConfigCloudSMS.Name = "btnConfigCloudSMS";
			this.btnConfigCloudSMS.Size = new System.Drawing.Size(75, 23);
			this.btnConfigCloudSMS.TabIndex = 0;
			this.btnConfigCloudSMS.Text = "CloudSMS";
			this.btnConfigCloudSMS.UseVisualStyleBackColor = true;
			this.btnConfigCloudSMS.Click += new System.EventHandler(this.btnConfigCloudSMS_Click);
			// 
			// btnConfigOATHCalc
			// 
			this.btnConfigOATHCalc.Location = new System.Drawing.Point(95, 89);
			this.btnConfigOATHCalc.Name = "btnConfigOATHCalc";
			this.btnConfigOATHCalc.Size = new System.Drawing.Size(75, 23);
			this.btnConfigOATHCalc.TabIndex = 1;
			this.btnConfigOATHCalc.Text = "OATHCalc";
			this.btnConfigOATHCalc.UseVisualStyleBackColor = true;
			this.btnConfigOATHCalc.Click += new System.EventHandler(this.btnConfigOATHCalc_Click);
			// 
			// btnConfigAuthEngine
			// 
			this.btnConfigAuthEngine.Location = new System.Drawing.Point(95, 31);
			this.btnConfigAuthEngine.Name = "btnConfigAuthEngine";
			this.btnConfigAuthEngine.Size = new System.Drawing.Size(75, 23);
			this.btnConfigAuthEngine.TabIndex = 2;
			this.btnConfigAuthEngine.Text = "AuthEngine";
			this.btnConfigAuthEngine.UseVisualStyleBackColor = true;
			this.btnConfigAuthEngine.Click += new System.EventHandler(this.btnConfigAuthEngine_Click);
			// 
			// btnTestRemoveRegistry
			// 
			this.btnTestRemoveRegistry.Location = new System.Drawing.Point(95, 119);
			this.btnTestRemoveRegistry.Name = "btnTestRemoveRegistry";
			this.btnTestRemoveRegistry.Size = new System.Drawing.Size(75, 37);
			this.btnTestRemoveRegistry.TabIndex = 3;
			this.btnTestRemoveRegistry.Text = "Remove Registry";
			this.btnTestRemoveRegistry.UseVisualStyleBackColor = true;
			this.btnTestRemoveRegistry.Click += new System.EventHandler(this.btnTestRemoveRegistry_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(95, 163);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "db handler";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(103, 209);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 5;
			this.button2.Text = "clients";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnTestRemoveRegistry);
			this.Controls.Add(this.btnConfigAuthEngine);
			this.Controls.Add(this.btnConfigOATHCalc);
			this.Controls.Add(this.btnConfigCloudSMS);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnConfigCloudSMS;
		private System.Windows.Forms.Button btnConfigOATHCalc;
		private System.Windows.Forms.Button btnConfigAuthEngine;
		private System.Windows.Forms.Button btnTestRemoveRegistry;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
	}
}

