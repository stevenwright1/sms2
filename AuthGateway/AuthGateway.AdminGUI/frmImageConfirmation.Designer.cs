namespace AuthGateway.AdminGUI
{
    partial class frmImageConfirmation
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
            if (disposing && (components != null)) {
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnImageDlgOK = new System.Windows.Forms.Button();
            this.btnImageDlgCancel = new System.Windows.Forms.Button();
            this.pbLeft = new System.Windows.Forms.PictureBox();
            this.pbRight = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRight)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.pbLeft, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pbRight, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 205F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(572, 268);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(129, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label1.Size = new System.Drawing.Size(28, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Left:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(411, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label2.Size = new System.Drawing.Size(35, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Right:";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.btnImageDlgOK);
            this.flowLayoutPanel1.Controls.Add(this.btnImageDlgCancel);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(404, 237);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(165, 28);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // btnImageDlgOK
            // 
            this.btnImageDlgOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnImageDlgOK.Location = new System.Drawing.Point(3, 3);
            this.btnImageDlgOK.Name = "btnImageDlgOK";
            this.btnImageDlgOK.Size = new System.Drawing.Size(75, 23);
            this.btnImageDlgOK.TabIndex = 0;
            this.btnImageDlgOK.Text = "OK";
            this.btnImageDlgOK.UseVisualStyleBackColor = true;
            // 
            // btnImageDlgCancel
            // 
            this.btnImageDlgCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnImageDlgCancel.Location = new System.Drawing.Point(84, 3);
            this.btnImageDlgCancel.Name = "btnImageDlgCancel";
            this.btnImageDlgCancel.Size = new System.Drawing.Size(75, 23);
            this.btnImageDlgCancel.TabIndex = 1;
            this.btnImageDlgCancel.Text = "Cancel";
            this.btnImageDlgCancel.UseVisualStyleBackColor = true;
            // 
            // pbLeft
            // 
            this.pbLeft.Location = new System.Drawing.Point(3, 32);
            this.pbLeft.Name = "pbLeft";
            this.pbLeft.Padding = new System.Windows.Forms.Padding(5);
            this.pbLeft.Size = new System.Drawing.Size(280, 199);
            this.pbLeft.TabIndex = 3;
            this.pbLeft.TabStop = false;
            // 
            // pbRight
            // 
            this.pbRight.Location = new System.Drawing.Point(289, 32);
            this.pbRight.Name = "pbRight";
            this.pbRight.Padding = new System.Windows.Forms.Padding(5);
            this.pbRight.Size = new System.Drawing.Size(280, 199);
            this.pbRight.TabIndex = 4;
            this.pbRight.TabStop = false;
            // 
            // frmImageConfirmation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 268);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "frmImageConfirmation";
            this.Text = "Confirm Image Selection";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnImageDlgOK;
        private System.Windows.Forms.Button btnImageDlgCancel;
        private System.Windows.Forms.PictureBox pbLeft;
        private System.Windows.Forms.PictureBox pbRight;
    }
}