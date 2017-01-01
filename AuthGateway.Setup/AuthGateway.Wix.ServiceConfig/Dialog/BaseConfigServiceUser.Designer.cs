namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	partial class BaseConfigServiceUser
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
			this.components = new System.ComponentModel.Container();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tbAdPassword = new System.Windows.Forms.TextBox();
			this.btnTestConfig = new System.Windows.Forms.Button();
			this.ttTip = new System.Windows.Forms.ToolTip(this.components);
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.cbUsername = new System.Windows.Forms.ComboBox();
			this.groupBox2.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.tableLayoutPanel2);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(3, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(443, 254);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "CloudSMS Service User";
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 3;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 211F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
			this.tableLayoutPanel2.Controls.Add(this.label5, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.label3, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.tbAdPassword, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.btnTestConfig, 1, 2);
			this.tableLayoutPanel2.Controls.Add(this.cbUsername, 1, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 3;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(437, 235);
			this.tableLayoutPanel2.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label5.Location = new System.Drawing.Point(3, 30);
			this.label5.Name = "label5";
			this.label5.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.label5.Size = new System.Drawing.Size(143, 29);
			this.label5.TabIndex = 23;
			this.label5.Text = "Password";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Location = new System.Drawing.Point(3, 0);
			this.label3.Name = "label3";
			this.label3.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.label3.Size = new System.Drawing.Size(143, 30);
			this.label3.TabIndex = 21;
			this.label3.Text = "Account";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tbAdPassword
			// 
			this.tbAdPassword.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbAdPassword.Location = new System.Drawing.Point(152, 33);
			this.tbAdPassword.Name = "tbAdPassword";
			this.tbAdPassword.Size = new System.Drawing.Size(205, 20);
			this.tbAdPassword.TabIndex = 27;
			this.tbAdPassword.UseSystemPasswordChar = true;
			// 
			// btnTestConfig
			// 
			this.btnTestConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTestConfig.Location = new System.Drawing.Point(261, 62);
			this.btnTestConfig.Name = "btnTestConfig";
			this.btnTestConfig.Size = new System.Drawing.Size(96, 25);
			this.btnTestConfig.TabIndex = 28;
			this.btnTestConfig.Text = "Test Login";
			this.btnTestConfig.UseVisualStyleBackColor = true;
			this.btnTestConfig.Click += new System.EventHandler(this.btnTestConfig_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 77.85714F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.14286F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(449, 334);
			this.tableLayoutPanel1.TabIndex = 5;
			// 
			// cbUsername
			// 
			this.cbUsername.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cbUsername.FormattingEnabled = true;
			this.cbUsername.Items.AddRange(new object[] {
            "Local System",
            "Local Service",
            "Network Service"});
			this.cbUsername.Location = new System.Drawing.Point(152, 3);
			this.cbUsername.Name = "cbUsername";
			this.cbUsername.Size = new System.Drawing.Size(205, 21);
			this.cbUsername.TabIndex = 26;
			this.cbUsername.TextChanged += new System.EventHandler(this.cbUsername_TextChanged);
			// 
			// CloudSMSConfigServiceUser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "CloudSMSConfigServiceUser";
			this.Size = new System.Drawing.Size(449, 334);
			this.Load += new System.EventHandler(this.ConfigServiceUser_Load);
			this.groupBox2.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnTestConfig;
		private System.Windows.Forms.ToolTip ttTip;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbAdPassword;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.ComboBox cbUsername;
	}
}
