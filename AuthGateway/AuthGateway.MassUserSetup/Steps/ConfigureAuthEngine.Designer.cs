namespace AuthGateway.MassUserSetup.Steps
{
	partial class ConfigureAuthEngine
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
			this.label1 = new System.Windows.Forms.Label();
			this.tbPort = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbServer = new System.Windows.Forms.TextBox();
			this.btnTest = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(13, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(151, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "AuthEngine Configuration";
			// 
			// tbPort
			// 
			this.tbPort.Location = new System.Drawing.Point(82, 49);
			this.tbPort.Name = "tbPort";
			this.tbPort.Size = new System.Drawing.Size(157, 20);
			this.tbPort.TabIndex = 11;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 52);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Port:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 28);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "Server:";
			// 
			// tbServer
			// 
			this.tbServer.Location = new System.Drawing.Point(82, 25);
			this.tbServer.Name = "tbServer";
			this.tbServer.Size = new System.Drawing.Size(157, 20);
			this.tbServer.TabIndex = 8;
			this.tbServer.TextChanged += new System.EventHandler(this.tbServer_TextChanged);
			// 
			// btnTest
			// 
			this.btnTest.Location = new System.Drawing.Point(16, 75);
			this.btnTest.Name = "btnTest";
			this.btnTest.Size = new System.Drawing.Size(135, 23);
			this.btnTest.TabIndex = 12;
			this.btnTest.Text = "Test Configuration";
			this.btnTest.UseVisualStyleBackColor = true;
			this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// ConfigureAuthEngine
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.btnTest);
			this.Controls.Add(this.tbPort);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbServer);
			this.Controls.Add(this.label1);
			this.Name = "ConfigureAuthEngine";
			this.Size = new System.Drawing.Size(394, 224);
			this.Load += new System.EventHandler(this.ConfigureAuthEngine_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbPort;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbServer;
		private System.Windows.Forms.Button btnTest;
	}
}
