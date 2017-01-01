namespace AuthGateway.MassUserSetup.Steps
{
	partial class ConfigureTemplate
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
			this.tbTemplate = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbFrom = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.rbHtml = new System.Windows.Forms.RadioButton();
			this.rbPlain = new System.Windows.Forms.RadioButton();
			this.tlp.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tlp
			// 
			this.tlp.ColumnCount = 3;
			this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
			this.tlp.Controls.Add(this.label1, 0, 0);
			this.tlp.Controls.Add(this.tbTemplate, 1, 2);
			this.tlp.Controls.Add(this.label2, 0, 2);
			this.tlp.Controls.Add(this.tbFrom, 1, 1);
			this.tlp.Controls.Add(this.label5, 0, 1);
			this.tlp.Controls.Add(this.panel1, 2, 2);
			this.tlp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlp.Location = new System.Drawing.Point(0, 0);
			this.tlp.Name = "tlp";
			this.tlp.Padding = new System.Windows.Forms.Padding(13, 9, 13, 9);
			this.tlp.RowCount = 3;
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlp.Size = new System.Drawing.Size(463, 298);
			this.tlp.TabIndex = 24;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.tlp.SetColumnSpan(this.label1, 3);
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(16, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(177, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "E-Mail Template Configuration";
			// 
			// tbTemplate
			// 
			this.tbTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbTemplate.Location = new System.Drawing.Point(76, 62);
			this.tbTemplate.Multiline = true;
			this.tbTemplate.Name = "tbTemplate";
			this.tbTemplate.Size = new System.Drawing.Size(289, 224);
			this.tbTemplate.TabIndex = 22;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(47, 25);
			this.label2.TabIndex = 23;
			this.label2.Text = "Content:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbFrom
			// 
			this.tbFrom.Location = new System.Drawing.Point(76, 37);
			this.tbFrom.Name = "tbFrom";
			this.tbFrom.Size = new System.Drawing.Size(209, 20);
			this.tbFrom.TabIndex = 21;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 34);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(54, 25);
			this.label5.TabIndex = 20;
			this.label5.Text = "Subject:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.rbHtml);
			this.panel1.Controls.Add(this.rbPlain);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(371, 62);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(76, 224);
			this.panel1.TabIndex = 24;
			// 
			// rbHtml
			// 
			this.rbHtml.AutoSize = true;
			this.rbHtml.Location = new System.Drawing.Point(1, 28);
			this.rbHtml.Name = "rbHtml";
			this.rbHtml.Size = new System.Drawing.Size(55, 17);
			this.rbHtml.TabIndex = 26;
			this.rbHtml.Text = "HTML";
			this.rbHtml.UseVisualStyleBackColor = true;
			// 
			// rbPlain
			// 
			this.rbPlain.AutoSize = true;
			this.rbPlain.Checked = true;
			this.rbPlain.Location = new System.Drawing.Point(1, 5);
			this.rbPlain.Name = "rbPlain";
			this.rbPlain.Size = new System.Drawing.Size(72, 17);
			this.rbPlain.TabIndex = 25;
			this.rbPlain.TabStop = true;
			this.rbPlain.Text = "Plain Text";
			this.rbPlain.UseVisualStyleBackColor = true;
			// 
			// ConfigureTemplate
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.tlp);
			this.Name = "ConfigureTemplate";
			this.Size = new System.Drawing.Size(463, 298);
			this.tlp.ResumeLayout(false);
			this.tlp.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbFrom;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbTemplate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TableLayoutPanel tlp;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton rbPlain;
		private System.Windows.Forms.RadioButton rbHtml;
	}
}
