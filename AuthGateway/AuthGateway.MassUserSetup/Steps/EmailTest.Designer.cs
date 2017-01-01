namespace AuthGateway.MassUserSetup.Steps
{
	partial class EmailTest
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
			this.tlp = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tbEmail = new System.Windows.Forms.TextBox();
			this.btnTestEmail = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tbFullname = new System.Windows.Forms.TextBox();
			this.cbUsername = new System.Windows.Forms.ComboBox();
			this.tlp.SuspendLayout();
			this.SuspendLayout();
			// 
			// tlp
			// 
			this.tlp.ColumnCount = 2;
			this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.47597F));
			this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 83.52403F));
			this.tlp.Controls.Add(this.label1, 0, 0);
			this.tlp.Controls.Add(this.label5, 0, 3);
			this.tlp.Controls.Add(this.tbEmail, 1, 3);
			this.tlp.Controls.Add(this.btnTestEmail, 1, 4);
			this.tlp.Controls.Add(this.label2, 0, 2);
			this.tlp.Controls.Add(this.label3, 0, 1);
			this.tlp.Controls.Add(this.tbFullname, 1, 2);
			this.tlp.Controls.Add(this.cbUsername, 1, 1);
			this.tlp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlp.Location = new System.Drawing.Point(0, 0);
			this.tlp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.tlp.Name = "tlp";
			this.tlp.Padding = new System.Windows.Forms.Padding(20, 14, 20, 14);
			this.tlp.RowCount = 6;
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
			this.tlp.Size = new System.Drawing.Size(694, 428);
			this.tlp.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.tlp.SetColumnSpan(this.label1, 3);
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(24, 14);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 20);
			this.label1.TabIndex = 3;
			this.label1.Text = "E-Mail Test";
			// 
			// label5
			// 
			this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label5.Location = new System.Drawing.Point(24, 145);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(99, 48);
			this.label5.TabIndex = 22;
			this.label5.Text = "To:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbEmail
			// 
			this.tbEmail.Location = new System.Drawing.Point(131, 150);
			this.tbEmail.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.tbEmail.Name = "tbEmail";
			this.tbEmail.Size = new System.Drawing.Size(312, 26);
			this.tbEmail.TabIndex = 23;
			// 
			// btnTestEmail
			// 
			this.btnTestEmail.Location = new System.Drawing.Point(131, 198);
			this.btnTestEmail.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnTestEmail.Name = "btnTestEmail";
			this.btnTestEmail.Size = new System.Drawing.Size(178, 35);
			this.btnTestEmail.TabIndex = 24;
			this.btnTestEmail.Text = "Send test e-mail";
			this.btnTestEmail.UseVisualStyleBackColor = true;
			this.btnTestEmail.Click += new System.EventHandler(this.btnTestEmail_Click);
			// 
			// label2
			// 
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Location = new System.Drawing.Point(24, 97);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(99, 48);
			this.label2.TabIndex = 25;
			this.label2.Text = "Fullname:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Location = new System.Drawing.Point(24, 49);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(99, 48);
			this.label3.TabIndex = 26;
			this.label3.Text = "Username:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbFullname
			// 
			this.tbFullname.Location = new System.Drawing.Point(131, 102);
			this.tbFullname.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.tbFullname.Name = "tbFullname";
			this.tbFullname.Size = new System.Drawing.Size(312, 26);
			this.tbFullname.TabIndex = 28;
			// 
			// cbUsername
			// 
			this.cbUsername.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbUsername.FormattingEnabled = true;
			this.cbUsername.Location = new System.Drawing.Point(130, 52);
			this.cbUsername.Name = "cbUsername";
			this.cbUsername.Size = new System.Drawing.Size(313, 28);
			this.cbUsername.TabIndex = 29;
			// 
			// EmailTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.tlp);
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "EmailTest";
			this.Size = new System.Drawing.Size(694, 428);
			this.tlp.ResumeLayout(false);
			this.tlp.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tlp;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbEmail;
		private System.Windows.Forms.Button btnTestEmail;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbFullname;
		private System.Windows.Forms.ComboBox cbUsername;
	}
}
