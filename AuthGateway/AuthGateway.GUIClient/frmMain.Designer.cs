namespace AuthGateway.GUIClient
{
	partial class frmClient
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmClient));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tbUsername = new System.Windows.Forms.TextBox();
			this.tbPin = new System.Windows.Forms.TextBox();
			this.tbPincode = new System.Windows.Forms.TextBox();
			this.tbResponse = new System.Windows.Forms.TextBox();
			this.btnValidateUser = new System.Windows.Forms.Button();
			this.btnValidatePin = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tbState = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 25);
			this.label1.TabIndex = 0;
			this.label1.Text = "Username:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Location = new System.Drawing.Point(3, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 25);
			this.label2.TabIndex = 1;
			this.label2.Text = "PIN2:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Location = new System.Drawing.Point(3, 135);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(94, 20);
			this.label3.TabIndex = 2;
			this.label3.Text = "Response";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label4.Location = new System.Drawing.Point(3, 75);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(94, 25);
			this.label4.TabIndex = 3;
			this.label4.Text = "PIN:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbUsername
			// 
			this.tbUsername.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbUsername.Location = new System.Drawing.Point(103, 3);
			this.tbUsername.Name = "tbUsername";
			this.tbUsername.Size = new System.Drawing.Size(222, 20);
			this.tbUsername.TabIndex = 4;
			// 
			// tbPin
			// 
			this.tbPin.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbPin.Location = new System.Drawing.Point(103, 78);
			this.tbPin.Name = "tbPin";
			this.tbPin.Size = new System.Drawing.Size(222, 20);
			this.tbPin.TabIndex = 5;
			// 
			// tbPincode
			// 
			this.tbPincode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbPincode.Location = new System.Drawing.Point(103, 28);
			this.tbPincode.Name = "tbPincode";
			this.tbPincode.Size = new System.Drawing.Size(222, 20);
			this.tbPincode.TabIndex = 6;
			// 
			// tbResponse
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.tbResponse, 2);
			this.tbResponse.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbResponse.Location = new System.Drawing.Point(3, 158);
			this.tbResponse.Multiline = true;
			this.tbResponse.Name = "tbResponse";
			this.tbResponse.ReadOnly = true;
			this.tbResponse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbResponse.Size = new System.Drawing.Size(322, 225);
			this.tbResponse.TabIndex = 7;
			// 
			// btnValidateUser
			// 
			this.btnValidateUser.Location = new System.Drawing.Point(3, 3);
			this.btnValidateUser.Name = "btnValidateUser";
			this.btnValidateUser.Size = new System.Drawing.Size(94, 23);
			this.btnValidateUser.TabIndex = 8;
			this.btnValidateUser.Text = "Validate User";
			this.btnValidateUser.UseVisualStyleBackColor = true;
			this.btnValidateUser.Click += new System.EventHandler(this.btnValidateUser_Click);
			// 
			// btnValidatePin
			// 
			this.btnValidatePin.Location = new System.Drawing.Point(114, 3);
			this.btnValidatePin.Name = "btnValidatePin";
			this.btnValidatePin.Size = new System.Drawing.Size(94, 23);
			this.btnValidatePin.TabIndex = 9;
			this.btnValidatePin.Text = "Validate Pin";
			this.btnValidatePin.UseVisualStyleBackColor = true;
			this.btnValidatePin.Click += new System.EventHandler(this.btnValidatePin_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.tbState, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.label5, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.tbPin, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label3, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this.tbUsername, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.tbResponse, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.tbPincode, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 4);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 7;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(328, 386);
			this.tableLayoutPanel1.TabIndex = 10;
			// 
			// tbState
			// 
			this.tbState.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbState.Location = new System.Drawing.Point(103, 53);
			this.tbState.Name = "tbState";
			this.tbState.Size = new System.Drawing.Size(222, 20);
			this.tbState.TabIndex = 12;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label5.Location = new System.Drawing.Point(3, 50);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(94, 25);
			this.label5.TabIndex = 11;
			this.label5.Text = "State:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnValidateUser);
			this.panel1.Controls.Add(this.btnValidatePin);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(103, 103);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(222, 29);
			this.panel1.TabIndex = 10;
			// 
			// frmClient
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(328, 386);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmClient";
			this.Text = "SMS2 - Test Client";
			this.Load += new System.EventHandler(this.frmClient_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbUsername;
		private System.Windows.Forms.TextBox tbPin;
		private System.Windows.Forms.TextBox tbPincode;
		private System.Windows.Forms.TextBox tbResponse;
		private System.Windows.Forms.Button btnValidateUser;
		private System.Windows.Forms.Button btnValidatePin;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox tbState;
		private System.Windows.Forms.Label label5;
	}
}

