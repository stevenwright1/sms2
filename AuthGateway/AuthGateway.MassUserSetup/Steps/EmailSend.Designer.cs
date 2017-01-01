namespace AuthGateway.MassUserSetup.Steps
{
	partial class EmailSend
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
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.dgv = new System.Windows.Forms.DataGridView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnAbort = new System.Windows.Forms.Button();
			this.btnSend = new System.Windows.Forms.Button();
			this.tlp.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tlp
			// 
			this.tlp.ColumnCount = 1;
			this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlp.Controls.Add(this.label1, 0, 0);
			this.tlp.Controls.Add(this.progressBar1, 0, 2);
			this.tlp.Controls.Add(this.dgv, 0, 1);
			this.tlp.Controls.Add(this.panel1, 0, 3);
			this.tlp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlp.Location = new System.Drawing.Point(0, 0);
			this.tlp.Name = "tlp";
			this.tlp.Padding = new System.Windows.Forms.Padding(13, 9, 13, 9);
			this.tlp.RowCount = 4;
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tlp.Size = new System.Drawing.Size(456, 244);
			this.tlp.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.tlp.SetColumnSpan(this.label1, 3);
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(16, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "E-Mail Process";
			// 
			// progressBar1
			// 
			this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.progressBar1.Location = new System.Drawing.Point(16, 173);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(424, 24);
			this.progressBar1.TabIndex = 0;
			// 
			// dgv
			// 
			this.dgv.AllowUserToAddRows = false;
			this.dgv.AllowUserToDeleteRows = false;
			this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgv.Location = new System.Drawing.Point(16, 37);
			this.dgv.Name = "dgv";
			this.dgv.ReadOnly = true;
			this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgv.Size = new System.Drawing.Size(424, 130);
			this.dgv.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnAbort);
			this.panel1.Controls.Add(this.btnSend);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(16, 203);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(424, 29);
			this.panel1.TabIndex = 4;
			// 
			// btnAbort
			// 
			this.btnAbort.Enabled = false;
			this.btnAbort.Location = new System.Drawing.Point(247, 3);
			this.btnAbort.Name = "btnAbort";
			this.btnAbort.Size = new System.Drawing.Size(75, 23);
			this.btnAbort.TabIndex = 1;
			this.btnAbort.Text = "Abort";
			this.btnAbort.UseVisualStyleBackColor = true;
			// 
			// btnSend
			// 
			this.btnSend.Location = new System.Drawing.Point(328, 3);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(87, 23);
			this.btnSend.TabIndex = 0;
			this.btnSend.Text = "Send e-mails";
			this.btnSend.UseVisualStyleBackColor = true;
			// 
			// EmailSend
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.tlp);
			this.Name = "EmailSend";
			this.Size = new System.Drawing.Size(456, 244);
			this.Load += new System.EventHandler(this.EmailSend_Load);
			this.tlp.ResumeLayout(false);
			this.tlp.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tlp;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.DataGridView dgv;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnAbort;
		private System.Windows.Forms.Button btnSend;
	}
}
