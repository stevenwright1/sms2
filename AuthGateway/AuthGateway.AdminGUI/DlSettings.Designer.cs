using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using AuthGateway.AdminGUI.Controls;
namespace AuthGateway.AdminGUI
{
    partial class DlSettings : System.Windows.Forms.Form
    {

        //Form overrides dispose to clean up the component list.
        [System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        //Required by the Windows Form Designer

        private System.ComponentModel.IContainer components = null;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tcOptions = new System.Windows.Forms.TabControl();
            this.tabMessage = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.lblLegend = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.BtnOkSMS = new System.Windows.Forms.Button();
            this.cbMsgLabel = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.tabEngineOptions = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.Label2 = new System.Windows.Forms.Label();
            this.nudPasscodeLength = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.cbKeyBase = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cbUseOATHDefaults = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbMissingEmail = new System.Windows.Forms.TextBox();
            this.tbMissingPhone = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.cbPasswordVaulting = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.cbMutualAuthMode = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.cbAskMissing = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cbAskPin = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cbAskProviderInfo = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbPincode = new System.Windows.Forms.ComboBox();
            this.nupPincodeLength = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.cbPincodePanicMode = new System.Windows.Forms.CheckBox();
            this.BtnOKPassCode = new System.Windows.Forms.Button();
            this.tabOtherFunctions = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel13 = new System.Windows.Forms.TableLayoutPanel();
            this.lblTokens = new System.Windows.Forms.Label();
            this.btnPollUsers = new System.Windows.Forms.Button();
            this.tabAuthOptions = new System.Windows.Forms.TabPage();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.flpRB = new System.Windows.Forms.FlowLayoutPanel();
            this.gbConfig = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnResetPanic = new System.Windows.Forms.Button();
            this.tabLoginOptions = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.cbSAMLoginPreferred = new System.Windows.Forms.CheckBox();
            this.cbAllowAliasesInLogin = new System.Windows.Forms.CheckBox();
            this.cbMobileNumberLogin = new System.Windows.Forms.CheckBox();
            this.cbUsernameLogin = new System.Windows.Forms.CheckBox();
            this.cbEmailAddressLogin = new System.Windows.Forms.CheckBox();
            this.cbUPNLogin = new System.Windows.Forms.CheckBox();
            this.cbPre2000Login = new System.Windows.Forms.CheckBox();
            this.btnSaveLoginOptions = new System.Windows.Forms.Button();
            this.tabAliases = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label14 = new System.Windows.Forms.Label();
            this.cbDomain = new AuthGateway.AdminGUI.Controls.ComboBoxEx();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label15 = new System.Windows.Forms.Label();
            this.lbAliases = new System.Windows.Forms.ListBox();
            this.btnSaveAliases = new System.Windows.Forms.Button();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddAlias = new System.Windows.Forms.Button();
            this.btnEditAlias = new System.Windows.Forms.Button();
            this.btnRemoveAlias = new System.Windows.Forms.Button();
            this.tabMutualAuth = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label18 = new System.Windows.Forms.Label();
            this.btnSelectLeftImage = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label19 = new System.Windows.Forms.Label();
            this.btnRightImage = new System.Windows.Forms.Button();
            this.pbLeftImage = new System.Windows.Forms.PictureBox();
            this.pbRightImage = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSaveImages = new System.Windows.Forms.Button();
            this.tabHANodes = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSavePollingConfig = new System.Windows.Forms.Button();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.lblServers = new System.Windows.Forms.Label();
            this.btnRefreshPollingServers = new System.Windows.Forms.Button();
            this.dgvServers = new System.Windows.Forms.DataGridView();
            this.HostnameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MacAddressColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UsersPollingPreferenceColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ImagesPollingPreferenceColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.nudWaitingInterval = new System.Windows.Forms.NumericUpDown();
            this.nudHeartbeatInterval = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.BtnClose = new System.Windows.Forms.Button();
            this.ttProvider = new System.Windows.Forms.ToolTip(this.components);            
            this.tcOptions.SuspendLayout();
            this.tabMessage.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.tabEngineOptions.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPasscodeLength)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupPincodeLength)).BeginInit();
            this.tabOtherFunctions.SuspendLayout();
            this.tableLayoutPanel13.SuspendLayout();
            this.tabAuthOptions.SuspendLayout();
            this.TableLayoutPanel1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.tabLoginOptions.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tabAliases.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.tabMutualAuth.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightImage)).BeginInit();
            this.flowLayoutPanel5.SuspendLayout();
            this.tabHANodes.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServers)).BeginInit();
            this.tableLayoutPanel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWaitingInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeartbeatInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // tcOptions
            // 
            this.tcOptions.Controls.Add(this.tabMessage);
            this.tcOptions.Controls.Add(this.tabEngineOptions);
            this.tcOptions.Controls.Add(this.tabOtherFunctions);
            this.tcOptions.Controls.Add(this.tabAuthOptions);
            this.tcOptions.Controls.Add(this.tabLoginOptions);
            this.tcOptions.Controls.Add(this.tabAliases);
            //this.tcOptions.Controls.Add(this.tabMutualAuth);
            this.tcOptions.Controls.Add(this.tabHANodes);
            this.tcOptions.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcOptions.Location = new System.Drawing.Point(12, 12);
            this.tcOptions.Name = "tcOptions";
            this.tcOptions.SelectedIndex = 0;
            this.tcOptions.Size = new System.Drawing.Size(411, 349);
            this.tcOptions.TabIndex = 1;
            // 
            // tabMessage
            // 
            this.tabMessage.Controls.Add(this.tableLayoutPanel2);
            this.tabMessage.Location = new System.Drawing.Point(4, 22);
            this.tabMessage.Name = "tabMessage";
            this.tabMessage.Padding = new System.Windows.Forms.Padding(3);
            this.tabMessage.Size = new System.Drawing.Size(403, 323);
            this.tabMessage.TabIndex = 0;
            this.tabMessage.Text = "Message";
            this.tabMessage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.66247F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.33753F));
            this.tableLayoutPanel2.Controls.Add(this.GroupBox1, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.Label1, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtMessage, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.BtnOkSMS, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.cbMsgLabel, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tbTitle, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(397, 317);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.lblLegend);
            this.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBox1.Location = new System.Drawing.Point(89, 185);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(305, 129);
            this.GroupBox1.TabIndex = 4;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Labels";
            // 
            // lblLegend
            // 
            this.lblLegend.AutoSize = true;
            this.lblLegend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLegend.Location = new System.Drawing.Point(3, 17);
            this.lblLegend.MaximumSize = new System.Drawing.Size(285, 0);
            this.lblLegend.Name = "lblLegend";
            this.lblLegend.Size = new System.Drawing.Size(0, 13);
            this.lblLegend.TabIndex = 1;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(3, 53);
            this.Label1.Margin = new System.Windows.Forms.Padding(3);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(53, 13);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Message:";
            // 
            // txtMessage
            // 
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Location = new System.Drawing.Point(89, 53);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(305, 126);
            this.txtMessage.TabIndex = 1;
            // 
            // BtnOkSMS
            // 
            this.BtnOkSMS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnOkSMS.BackColor = System.Drawing.SystemColors.Control;
            this.BtnOkSMS.Location = new System.Drawing.Point(10, 284);
            this.BtnOkSMS.Margin = new System.Windows.Forms.Padding(10, 3, 3, 10);
            this.BtnOkSMS.Name = "BtnOkSMS";
            this.BtnOkSMS.Size = new System.Drawing.Size(67, 23);
            this.BtnOkSMS.TabIndex = 3;
            this.BtnOkSMS.Text = "Save";
            this.BtnOkSMS.UseVisualStyleBackColor = false;
            this.BtnOkSMS.Click += new System.EventHandler(this.BtnOkSMS_Click);
            // 
            // cbMsgLabel
            // 
            this.cbMsgLabel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMsgLabel.FormattingEnabled = true;
            this.cbMsgLabel.Location = new System.Drawing.Point(89, 3);
            this.cbMsgLabel.Name = "cbMsgLabel";
            this.cbMsgLabel.Size = new System.Drawing.Size(157, 21);
            this.cbMsgLabel.TabIndex = 6;
            this.cbMsgLabel.SelectedIndexChanged += new System.EventHandler(this.CbMsgLabelSelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 3);
            this.label6.Margin = new System.Windows.Forms.Padding(3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Message Type:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 19);
            this.label3.TabIndex = 7;
            this.label3.Text = "Title:";
            // 
            // tbTitle
            // 
            this.tbTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbTitle.Location = new System.Drawing.Point(89, 28);
            this.tbTitle.Name = "tbTitle";
            this.tbTitle.Size = new System.Drawing.Size(305, 21);
            this.tbTitle.TabIndex = 8;
            // 
            // tabEngineOptions
            // 
            this.tabEngineOptions.Controls.Add(this.tableLayoutPanel5);
            this.tabEngineOptions.Controls.Add(this.tableLayoutPanel4);
            this.tabEngineOptions.Controls.Add(this.tableLayoutPanel3);
            this.tabEngineOptions.Controls.Add(this.BtnOKPassCode);
            this.tabEngineOptions.Location = new System.Drawing.Point(4, 22);
            this.tabEngineOptions.Name = "tabEngineOptions";
            this.tabEngineOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabEngineOptions.Size = new System.Drawing.Size(403, 323);
            this.tabEngineOptions.TabIndex = 1;
            this.tabEngineOptions.Text = "Engine Options";
            this.tabEngineOptions.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.63107F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54.36893F));
            this.tableLayoutPanel5.Controls.Add(this.Label2, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.nudPasscodeLength, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label10, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.cbKeyBase, 1, 1);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(10, 6);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 53.7037F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 46.2963F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(309, 51);
            this.tableLayoutPanel5.TabIndex = 9;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(4, 0);
            this.Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(88, 13);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "Passcode Length";
            // 
            // nudPasscodeLength
            // 
            this.nudPasscodeLength.Location = new System.Drawing.Point(144, 3);
            this.nudPasscodeLength.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudPasscodeLength.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudPasscodeLength.Name = "nudPasscodeLength";
            this.nudPasscodeLength.Size = new System.Drawing.Size(121, 21);
            this.nudPasscodeLength.TabIndex = 6;
            this.nudPasscodeLength.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(3, 27);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 23);
            this.label10.TabIndex = 7;
            this.label10.Text = "Passcode KeyBase";
            // 
            // cbKeyBase
            // 
            this.cbKeyBase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbKeyBase.FormattingEnabled = true;
            this.cbKeyBase.Location = new System.Drawing.Point(144, 30);
            this.cbKeyBase.Name = "cbKeyBase";
            this.cbKeyBase.Size = new System.Drawing.Size(121, 21);
            this.cbKeyBase.TabIndex = 8;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.92308F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.07692F));
            this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel1, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.label8, 0, 4);
            this.tableLayoutPanel4.Controls.Add(this.label9, 0, 5);
            this.tableLayoutPanel4.Controls.Add(this.tbMissingEmail, 1, 4);
            this.tableLayoutPanel4.Controls.Add(this.tbMissingPhone, 1, 5);
            this.tableLayoutPanel4.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label13, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.cbPasswordVaulting, 1, 3);
            this.tableLayoutPanel4.Controls.Add(this.label17, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.cbMutualAuthMode, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel6, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.label20, 0, 1);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(8, 134);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 6;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(390, 147);
            this.tableLayoutPanel4.TabIndex = 8;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.cbUseOATHDefaults);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(147, 33);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(240, 25);
            this.flowLayoutPanel1.TabIndex = 11;
            // 
            // cbUseOATHDefaults
            // 
            this.cbUseOATHDefaults.AutoSize = true;
            this.cbUseOATHDefaults.Location = new System.Drawing.Point(3, 3);
            this.cbUseOATHDefaults.Name = "cbUseOATHDefaults";
            this.cbUseOATHDefaults.Size = new System.Drawing.Size(15, 14);
            this.cbUseOATHDefaults.TabIndex = 0;
            this.cbUseOATHDefaults.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(2, 104);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(140, 21);
            this.label8.TabIndex = 1;
            this.label8.Text = "Missing E-mail Error";
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(2, 125);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 22);
            this.label9.TabIndex = 5;
            this.label9.Text = "Missing Phone error";
            // 
            // tbMissingEmail
            // 
            this.tbMissingEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbMissingEmail.Location = new System.Drawing.Point(146, 106);
            this.tbMissingEmail.Margin = new System.Windows.Forms.Padding(2);
            this.tbMissingEmail.Multiline = true;
            this.tbMissingEmail.Name = "tbMissingEmail";
            this.tbMissingEmail.Size = new System.Drawing.Size(242, 17);
            this.tbMissingEmail.TabIndex = 6;
            // 
            // tbMissingPhone
            // 
            this.tbMissingPhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbMissingPhone.Location = new System.Drawing.Point(146, 127);
            this.tbMissingPhone.Margin = new System.Windows.Forms.Padding(2);
            this.tbMissingPhone.Multiline = true;
            this.tbMissingPhone.Name = "tbMissingPhone";
            this.tbMissingPhone.Size = new System.Drawing.Size(242, 18);
            this.tbMissingPhone.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(2, 0);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(140, 30);
            this.label7.TabIndex = 0;
            this.label7.Text = "Ask missing info";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(3, 81);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(100, 23);
            this.label13.TabIndex = 12;
            this.label13.Text = "Password Vault";
            // 
            // cbPasswordVaulting
            // 
            this.cbPasswordVaulting.AutoSize = true;
            this.cbPasswordVaulting.Location = new System.Drawing.Point(147, 84);
            this.cbPasswordVaulting.Name = "cbPasswordVaulting";
            this.cbPasswordVaulting.Size = new System.Drawing.Size(15, 14);
            this.cbPasswordVaulting.TabIndex = 13;
            this.cbPasswordVaulting.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 61);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(135, 13);
            this.label17.TabIndex = 14;
            this.label17.Text = "Mutual Authorization Mode";
            this.label17.Visible = false;
            // 
            // cbMutualAuthMode
            // 
            this.cbMutualAuthMode.AutoSize = true;
            this.cbMutualAuthMode.Location = new System.Drawing.Point(147, 64);
            this.cbMutualAuthMode.Name = "cbMutualAuthMode";
            this.cbMutualAuthMode.Size = new System.Drawing.Size(15, 14);
            this.cbMutualAuthMode.TabIndex = 15;
            this.cbMutualAuthMode.UseVisualStyleBackColor = true;
            this.cbMutualAuthMode.Visible = false;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.Controls.Add(this.cbAskMissing);
            this.flowLayoutPanel6.Controls.Add(this.label11);
            this.flowLayoutPanel6.Controls.Add(this.cbAskPin);
            this.flowLayoutPanel6.Controls.Add(this.label12);
            this.flowLayoutPanel6.Controls.Add(this.cbAskProviderInfo);
            this.flowLayoutPanel6.Location = new System.Drawing.Point(147, 3);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(240, 24);
            this.flowLayoutPanel6.TabIndex = 16;
            // 
            // cbAskMissing
            // 
            this.cbAskMissing.Location = new System.Drawing.Point(2, 2);
            this.cbAskMissing.Margin = new System.Windows.Forms.Padding(2);
            this.cbAskMissing.Name = "cbAskMissing";
            this.cbAskMissing.Size = new System.Drawing.Size(25, 16);
            this.cbAskMissing.TabIndex = 4;
            this.cbAskMissing.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(32, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 13);
            this.label11.TabIndex = 8;
            this.label11.Text = "Ask PIN";
            // 
            // cbAskPin
            // 
            this.cbAskPin.AutoSize = true;
            this.cbAskPin.Location = new System.Drawing.Point(82, 3);
            this.cbAskPin.Name = "cbAskPin";
            this.cbAskPin.Size = new System.Drawing.Size(15, 14);
            this.cbAskPin.TabIndex = 10;
            this.cbAskPin.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(103, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(90, 13);
            this.label12.TabIndex = 9;
            this.label12.Text = "Ask Provider Info";
            // 
            // cbAskProviderInfo
            // 
            this.cbAskProviderInfo.AutoSize = true;
            this.cbAskProviderInfo.Location = new System.Drawing.Point(199, 3);
            this.cbAskProviderInfo.Name = "cbAskProviderInfo";
            this.cbAskProviderInfo.Size = new System.Drawing.Size(15, 14);
            this.cbAskProviderInfo.TabIndex = 11;
            this.cbAskProviderInfo.UseVisualStyleBackColor = true;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 30);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(119, 13);
            this.label20.TabIndex = 17;
            this.label20.Text = "Use OATHCalc Defaults";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.cbPincode, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.nupPincodeLength, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label16, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.cbPincodePanicMode, 1, 2);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(10, 62);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(285, 68);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(2, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(138, 22);
            this.label4.TabIndex = 0;
            this.label4.Text = "Pincode";
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(2, 22);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 22);
            this.label5.TabIndex = 1;
            this.label5.Text = "Pincode Length";
            // 
            // cbPincode
            // 
            this.cbPincode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPincode.FormattingEnabled = true;
            this.cbPincode.Location = new System.Drawing.Point(144, 2);
            this.cbPincode.Margin = new System.Windows.Forms.Padding(2);
            this.cbPincode.Name = "cbPincode";
            this.cbPincode.Size = new System.Drawing.Size(92, 21);
            this.cbPincode.TabIndex = 2;
            // 
            // nupPincodeLength
            // 
            this.nupPincodeLength.Location = new System.Drawing.Point(144, 24);
            this.nupPincodeLength.Margin = new System.Windows.Forms.Padding(2);
            this.nupPincodeLength.Name = "nupPincodeLength";
            this.nupPincodeLength.Size = new System.Drawing.Size(91, 21);
            this.nupPincodeLength.TabIndex = 3;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Location = new System.Drawing.Point(3, 44);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(136, 24);
            this.label16.TabIndex = 4;
            this.label16.Text = "Pincode Panic Mode";
            // 
            // cbPincodePanicMode
            // 
            this.cbPincodePanicMode.AutoSize = true;
            this.cbPincodePanicMode.Location = new System.Drawing.Point(145, 47);
            this.cbPincodePanicMode.Name = "cbPincodePanicMode";
            this.cbPincodePanicMode.Size = new System.Drawing.Size(15, 14);
            this.cbPincodePanicMode.TabIndex = 5;
            this.cbPincodePanicMode.UseVisualStyleBackColor = true;
            // 
            // BtnOKPassCode
            // 
            this.BtnOKPassCode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnOKPassCode.BackColor = System.Drawing.SystemColors.Control;
            this.BtnOKPassCode.Location = new System.Drawing.Point(330, 294);
            this.BtnOKPassCode.Name = "BtnOKPassCode";
            this.BtnOKPassCode.Size = new System.Drawing.Size(67, 23);
            this.BtnOKPassCode.TabIndex = 5;
            this.BtnOKPassCode.Text = "Save";
            this.BtnOKPassCode.UseVisualStyleBackColor = false;
            this.BtnOKPassCode.Click += new System.EventHandler(this.BtnOKPassCode_Click);
            // 
            // tabOtherFunctions
            // 
            this.tabOtherFunctions.Controls.Add(this.tableLayoutPanel13);
            this.tabOtherFunctions.Location = new System.Drawing.Point(4, 22);
            this.tabOtherFunctions.Name = "tabOtherFunctions";
            this.tabOtherFunctions.Padding = new System.Windows.Forms.Padding(3);
            this.tabOtherFunctions.Size = new System.Drawing.Size(403, 323);
            this.tabOtherFunctions.TabIndex = 7;
            this.tabOtherFunctions.Text = "Other Functions";
            this.tabOtherFunctions.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel13
            // 
            this.tableLayoutPanel13.ColumnCount = 1;
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel13.Controls.Add(this.lblTokens, 0, 1);
            this.tableLayoutPanel13.Controls.Add(this.btnPollUsers, 0, 0);
            this.tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel13.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            this.tableLayoutPanel13.RowCount = 2;
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel13.Size = new System.Drawing.Size(397, 317);
            this.tableLayoutPanel13.TabIndex = 2;
            // 
            // lblTokens
            // 
            this.lblTokens.AutoSize = true;
            this.lblTokens.Location = new System.Drawing.Point(3, 30);
            this.lblTokens.Name = "lblTokens";
            this.lblTokens.Size = new System.Drawing.Size(118, 13);
            this.lblTokens.TabIndex = 1;
            this.lblTokens.Text = "Text Message Credits: ";
            // 
            // btnPollUsers
            // 
            this.btnPollUsers.Location = new System.Drawing.Point(3, 3);
            this.btnPollUsers.Name = "btnPollUsers";
            this.btnPollUsers.Size = new System.Drawing.Size(75, 23);
            this.btnPollUsers.TabIndex = 0;
            this.btnPollUsers.Text = "Poll Users";
            this.btnPollUsers.UseVisualStyleBackColor = true;
            this.btnPollUsers.Click += new System.EventHandler(this.btnPollUsers_Click);
            // 
            // tabAuthOptions
            // 
            this.tabAuthOptions.Controls.Add(this.TableLayoutPanel1);
            this.tabAuthOptions.Location = new System.Drawing.Point(4, 22);
            this.tabAuthOptions.Name = "tabAuthOptions";
            this.tabAuthOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabAuthOptions.Size = new System.Drawing.Size(403, 323);
            this.tabAuthOptions.TabIndex = 2;
            this.tabAuthOptions.Text = "Auth Options";
            this.tabAuthOptions.UseVisualStyleBackColor = true;
            // 
            // TableLayoutPanel1
            // 
            this.TableLayoutPanel1.ColumnCount = 2;
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 154F));
            this.TableLayoutPanel1.Controls.Add(this.GroupBox2, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.gbConfig, 0, 1);
            this.TableLayoutPanel1.Controls.Add(this.btnSave, 1, 2);
            this.TableLayoutPanel1.Controls.Add(this.btnResetPanic, 0, 2);
            this.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 3;
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.TableLayoutPanel1.Size = new System.Drawing.Size(397, 317);
            this.TableLayoutPanel1.TabIndex = 4;
            // 
            // GroupBox2
            // 
            this.TableLayoutPanel1.SetColumnSpan(this.GroupBox2, 2);
            this.GroupBox2.Controls.Add(this.flpRB);
            this.GroupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBox2.Location = new System.Drawing.Point(3, 3);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(391, 44);
            this.GroupBox2.TabIndex = 2;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Provider";
            // 
            // flpRB
            // 
            this.flpRB.AutoScroll = true;
            this.flpRB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpRB.Location = new System.Drawing.Point(3, 17);
            this.flpRB.Name = "flpRB";
            this.flpRB.Size = new System.Drawing.Size(385, 24);
            this.flpRB.TabIndex = 0;
            // 
            // gbConfig
            // 
            this.TableLayoutPanel1.SetColumnSpan(this.gbConfig, 2);
            this.gbConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbConfig.Location = new System.Drawing.Point(3, 53);
            this.gbConfig.Name = "gbConfig";
            this.gbConfig.Size = new System.Drawing.Size(391, 228);
            this.gbConfig.TabIndex = 1;
            this.gbConfig.TabStop = false;
            this.gbConfig.Text = "Config";
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.Location = new System.Drawing.Point(273, 287);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(121, 27);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save Configuration";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnResetPanic
            // 
            this.btnResetPanic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnResetPanic.Enabled = false;
            this.btnResetPanic.Location = new System.Drawing.Point(3, 287);
            this.btnResetPanic.Name = "btnResetPanic";
            this.btnResetPanic.Size = new System.Drawing.Size(108, 27);
            this.btnResetPanic.TabIndex = 8;
            this.btnResetPanic.Text = "Reset Panic State";
            this.btnResetPanic.UseVisualStyleBackColor = true;
            this.btnResetPanic.Visible = false;
            this.btnResetPanic.Click += new System.EventHandler(this.btnResetPanic_Click);
            // 
            // tabLoginOptions
            // 
            this.tabLoginOptions.Controls.Add(this.tableLayoutPanel6);
            this.tabLoginOptions.Location = new System.Drawing.Point(4, 22);
            this.tabLoginOptions.Name = "tabLoginOptions";
            this.tabLoginOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabLoginOptions.Size = new System.Drawing.Size(403, 323);
            this.tabLoginOptions.TabIndex = 4;
            this.tabLoginOptions.Text = "Login Options";
            this.tabLoginOptions.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Controls.Add(this.cbSAMLoginPreferred, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.cbAllowAliasesInLogin, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.cbMobileNumberLogin, 0, 4);
            this.tableLayoutPanel6.Controls.Add(this.cbUsernameLogin, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.cbEmailAddressLogin, 0, 3);
            this.tableLayoutPanel6.Controls.Add(this.cbUPNLogin, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.cbPre2000Login, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.btnSaveLoginOptions, 1, 5);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 6;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.Size = new System.Drawing.Size(397, 317);
            this.tableLayoutPanel6.TabIndex = 8;
            // 
            // cbSAMLoginPreferred
            // 
            this.cbSAMLoginPreferred.AutoSize = true;
            this.cbSAMLoginPreferred.Location = new System.Drawing.Point(201, 3);
            this.cbSAMLoginPreferred.Name = "cbSAMLoginPreferred";
            this.cbSAMLoginPreferred.Size = new System.Drawing.Size(147, 17);
            this.cbSAMLoginPreferred.TabIndex = 7;
            this.cbSAMLoginPreferred.Text = "Prefer SAM to UPN prefix";
            this.cbSAMLoginPreferred.UseVisualStyleBackColor = true;
            // 
            // cbAllowAliasesInLogin
            // 
            this.cbAllowAliasesInLogin.AutoSize = true;
            this.cbAllowAliasesInLogin.Location = new System.Drawing.Point(201, 33);
            this.cbAllowAliasesInLogin.Name = "cbAllowAliasesInLogin";
            this.cbAllowAliasesInLogin.Size = new System.Drawing.Size(86, 17);
            this.cbAllowAliasesInLogin.TabIndex = 6;
            this.cbAllowAliasesInLogin.Text = "Allow aliases";
            this.cbAllowAliasesInLogin.UseVisualStyleBackColor = true;
            // 
            // cbMobileNumberLogin
            // 
            this.cbMobileNumberLogin.AutoSize = true;
            this.cbMobileNumberLogin.Location = new System.Drawing.Point(3, 123);
            this.cbMobileNumberLogin.Name = "cbMobileNumberLogin";
            this.cbMobileNumberLogin.Size = new System.Drawing.Size(96, 17);
            this.cbMobileNumberLogin.TabIndex = 4;
            this.cbMobileNumberLogin.Text = "Mobile Number";
            this.cbMobileNumberLogin.UseVisualStyleBackColor = true;
            // 
            // cbUsernameLogin
            // 
            this.cbUsernameLogin.AutoSize = true;
            this.cbUsernameLogin.BackColor = System.Drawing.SystemColors.Control;
            this.cbUsernameLogin.Checked = true;
            this.cbUsernameLogin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUsernameLogin.Enabled = false;
            this.cbUsernameLogin.Location = new System.Drawing.Point(3, 3);
            this.cbUsernameLogin.Name = "cbUsernameLogin";
            this.cbUsernameLogin.Size = new System.Drawing.Size(74, 17);
            this.cbUsernameLogin.TabIndex = 0;
            this.cbUsernameLogin.Text = "Username";
            this.cbUsernameLogin.UseVisualStyleBackColor = false;
            // 
            // cbEmailAddressLogin
            // 
            this.cbEmailAddressLogin.AutoSize = true;
            this.cbEmailAddressLogin.Location = new System.Drawing.Point(3, 93);
            this.cbEmailAddressLogin.Name = "cbEmailAddressLogin";
            this.cbEmailAddressLogin.Size = new System.Drawing.Size(96, 17);
            this.cbEmailAddressLogin.TabIndex = 3;
            this.cbEmailAddressLogin.Text = "E-mail Address";
            this.cbEmailAddressLogin.UseVisualStyleBackColor = true;
            // 
            // cbUPNLogin
            // 
            this.cbUPNLogin.AutoSize = true;
            this.cbUPNLogin.Location = new System.Drawing.Point(3, 33);
            this.cbUPNLogin.Name = "cbUPNLogin";
            this.cbUPNLogin.Size = new System.Drawing.Size(46, 17);
            this.cbUPNLogin.TabIndex = 1;
            this.cbUPNLogin.Text = "UPN";
            this.cbUPNLogin.UseVisualStyleBackColor = true;
            // 
            // cbPre2000Login
            // 
            this.cbPre2000Login.AutoSize = true;
            this.cbPre2000Login.Location = new System.Drawing.Point(3, 63);
            this.cbPre2000Login.Name = "cbPre2000Login";
            this.cbPre2000Login.Size = new System.Drawing.Size(119, 17);
            this.cbPre2000Login.TabIndex = 2;
            this.cbPre2000Login.Text = "Pre-Windows 2000 ";
            this.cbPre2000Login.UseVisualStyleBackColor = true;
            // 
            // btnSaveLoginOptions
            // 
            this.btnSaveLoginOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSaveLoginOptions.Location = new System.Drawing.Point(201, 291);
            this.btnSaveLoginOptions.Name = "btnSaveLoginOptions";
            this.btnSaveLoginOptions.Size = new System.Drawing.Size(193, 23);
            this.btnSaveLoginOptions.TabIndex = 5;
            this.btnSaveLoginOptions.Text = "Save Configuration";
            this.btnSaveLoginOptions.UseVisualStyleBackColor = true;
            this.btnSaveLoginOptions.Click += new System.EventHandler(this.btnSaveLoginOptions_Click);
            // 
            // tabAliases
            // 
            this.tabAliases.Controls.Add(this.tableLayoutPanel7);
            this.tabAliases.Location = new System.Drawing.Point(4, 22);
            this.tabAliases.Name = "tabAliases";
            this.tabAliases.Padding = new System.Windows.Forms.Padding(3);
            this.tabAliases.Size = new System.Drawing.Size(403, 323);
            this.tabAliases.TabIndex = 5;
            this.tabAliases.Text = "Domain Aliases";
            this.tabAliases.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel7.Controls.Add(this.flowLayoutPanel2, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.flowLayoutPanel3, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.btnSaveAliases, 1, 2);
            this.tableLayoutPanel7.Controls.Add(this.flowLayoutPanel4, 1, 1);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 3;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(397, 317);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.label14);
            this.flowLayoutPanel2.Controls.Add(this.cbDomain);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(232, 24);
            this.flowLayoutPanel2.TabIndex = 2;
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 7);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(42, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Domain";
            // 
            // cbDomain
            // 
            this.cbDomain.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbDomain.FormattingEnabled = true;
            this.cbDomain.Location = new System.Drawing.Point(51, 3);
            this.cbDomain.Name = "cbDomain";
            this.cbDomain.Size = new System.Drawing.Size(160, 21);
			
            this.cbDomain.TabIndex = 1;
            this.cbDomain.SelectedIndexChanging += new System.ComponentModel.CancelEventHandler(this.cbDomain_SelectedIndexChanging);
            this.cbDomain.SelectedIndexChanged += new System.EventHandler(this.cbDomain_SelectedIndexChanged);            
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.label15);
            this.flowLayoutPanel3.Controls.Add(this.lbAliases);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 33);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(232, 251);
            this.flowLayoutPanel3.TabIndex = 3;
            // 
            // label15
            // 
            this.label15.Dock = System.Windows.Forms.DockStyle.Left;
            this.label15.Location = new System.Drawing.Point(3, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(229, 23);
            this.label15.TabIndex = 0;
            this.label15.Text = "Aliases";
            // 
            // lbAliases
            // 
            this.lbAliases.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbAliases.FormattingEnabled = true;
            this.lbAliases.Location = new System.Drawing.Point(3, 26);
            this.lbAliases.Name = "lbAliases";
            this.lbAliases.Size = new System.Drawing.Size(229, 212);
            this.lbAliases.TabIndex = 1;
            this.lbAliases.SelectedIndexChanged += new System.EventHandler(this.lbAliases_SelectedIndexChanged);
            // 
            // btnSaveAliases
            // 
            this.btnSaveAliases.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSaveAliases.Enabled = false;
            this.btnSaveAliases.Location = new System.Drawing.Point(319, 290);
            this.btnSaveAliases.Name = "btnSaveAliases";
            this.btnSaveAliases.Size = new System.Drawing.Size(75, 24);
            this.btnSaveAliases.TabIndex = 4;
            this.btnSaveAliases.Text = "Save";
            this.btnSaveAliases.UseVisualStyleBackColor = true;
            this.btnSaveAliases.Click += new System.EventHandler(this.btnSaveAliases_Click);
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.btnAddAlias);
            this.flowLayoutPanel4.Controls.Add(this.btnEditAlias);
            this.flowLayoutPanel4.Controls.Add(this.btnRemoveAlias);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(241, 199);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(153, 85);
            this.flowLayoutPanel4.TabIndex = 5;
            // 
            // btnAddAlias
            // 
            this.btnAddAlias.Enabled = false;
            this.btnAddAlias.Location = new System.Drawing.Point(3, 3);
            this.btnAddAlias.Name = "btnAddAlias";
            this.btnAddAlias.Size = new System.Drawing.Size(75, 23);
            this.btnAddAlias.TabIndex = 0;
            this.btnAddAlias.Text = "Add";
            this.btnAddAlias.UseVisualStyleBackColor = true;
            this.btnAddAlias.Click += new System.EventHandler(this.btnAddAlias_Click);
            // 
            // btnEditAlias
            // 
            this.btnEditAlias.Enabled = false;
            this.btnEditAlias.Location = new System.Drawing.Point(3, 32);
            this.btnEditAlias.Name = "btnEditAlias";
            this.btnEditAlias.Size = new System.Drawing.Size(75, 23);
            this.btnEditAlias.TabIndex = 1;
            this.btnEditAlias.Text = "Edit";
            this.btnEditAlias.UseVisualStyleBackColor = true;
            this.btnEditAlias.Click += new System.EventHandler(this.btnEditAlias_Click);
            // 
            // btnRemoveAlias
            // 
            this.btnRemoveAlias.Enabled = false;
            this.btnRemoveAlias.Location = new System.Drawing.Point(3, 61);
            this.btnRemoveAlias.Name = "btnRemoveAlias";
            this.btnRemoveAlias.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveAlias.TabIndex = 2;
            this.btnRemoveAlias.Text = "Remove";
            this.btnRemoveAlias.UseVisualStyleBackColor = true;
            this.btnRemoveAlias.Click += new System.EventHandler(this.btnRemoveAlias_Click);
            // 
            // tabMutualAuth
            // 
            this.tabMutualAuth.Controls.Add(this.tableLayoutPanel8);
            this.tabMutualAuth.Location = new System.Drawing.Point(4, 22);
            this.tabMutualAuth.Name = "tabMutualAuth";
            this.tabMutualAuth.Padding = new System.Windows.Forms.Padding(3);
            this.tabMutualAuth.Size = new System.Drawing.Size(403, 323);
            this.tabMutualAuth.TabIndex = 6;
            this.tabMutualAuth.Text = "Mutual Auth";
            this.tabMutualAuth.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 1;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Controls.Add(this.tableLayoutPanel9, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.flowLayoutPanel5, 0, 1);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 2;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(397, 317);
            this.tableLayoutPanel8.TabIndex = 0;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Controls.Add(this.splitContainer1, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.splitContainer2, 1, 0);
            this.tableLayoutPanel9.Controls.Add(this.pbLeftImage, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.pbRightImage, 1, 1);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 2;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(391, 278);
            this.tableLayoutPanel9.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label18);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnSelectLeftImage);
            this.splitContainer1.Size = new System.Drawing.Size(189, 29);
            this.splitContainer1.SplitterDistance = 63;
            this.splitContainer1.TabIndex = 0;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(4, 13);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(59, 13);
            this.label18.TabIndex = 0;
            this.label18.Text = "Left Image";
            // 
            // btnSelectLeftImage
            // 
            this.btnSelectLeftImage.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSelectLeftImage.Location = new System.Drawing.Point(0, 0);
            this.btnSelectLeftImage.Name = "btnSelectLeftImage";
            this.btnSelectLeftImage.Size = new System.Drawing.Size(75, 29);
            this.btnSelectLeftImage.TabIndex = 0;
            this.btnSelectLeftImage.Text = "Choose...";
            this.btnSelectLeftImage.UseVisualStyleBackColor = true;
            this.btnSelectLeftImage.Click += new System.EventHandler(this.btnLeftImage_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(198, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.label19);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.btnRightImage);
            this.splitContainer2.Size = new System.Drawing.Size(190, 29);
            this.splitContainer2.SplitterDistance = 63;
            this.splitContainer2.TabIndex = 1;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(3, 13);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(65, 13);
            this.label19.TabIndex = 0;
            this.label19.Text = "Right Image";
            // 
            // btnRightImage
            // 
            this.btnRightImage.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnRightImage.Location = new System.Drawing.Point(0, 0);
            this.btnRightImage.Name = "btnRightImage";
            this.btnRightImage.Size = new System.Drawing.Size(75, 29);
            this.btnRightImage.TabIndex = 1;
            this.btnRightImage.Text = "Choose...";
            this.btnRightImage.UseVisualStyleBackColor = true;
            this.btnRightImage.Click += new System.EventHandler(this.btnRightImage_Click);
            // 
            // pbLeftImage
            // 
            this.pbLeftImage.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pbLeftImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbLeftImage.Image = global::AuthGateway.AdminGUI.Properties.Resources.ImageNotSet;
            this.pbLeftImage.Location = new System.Drawing.Point(5, 38);
            this.pbLeftImage.Name = "pbLeftImage";
            this.pbLeftImage.Size = new System.Drawing.Size(184, 131);
            this.pbLeftImage.TabIndex = 2;
            this.pbLeftImage.TabStop = false;
            // 
            // pbRightImage
            // 
            this.pbRightImage.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pbRightImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbRightImage.Image = global::AuthGateway.AdminGUI.Properties.Resources.ImageNotSet;
            this.pbRightImage.InitialImage = null;
            this.pbRightImage.Location = new System.Drawing.Point(201, 38);
            this.pbRightImage.Name = "pbRightImage";
            this.pbRightImage.Size = new System.Drawing.Size(184, 131);
            this.pbRightImage.TabIndex = 3;
            this.pbRightImage.TabStop = false;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.Controls.Add(this.btnSaveImages);
            this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel5.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(3, 287);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(391, 27);
            this.flowLayoutPanel5.TabIndex = 2;
            // 
            // btnSaveImages
            // 
            this.btnSaveImages.Enabled = false;
            this.btnSaveImages.Location = new System.Drawing.Point(313, 3);
            this.btnSaveImages.Name = "btnSaveImages";
            this.btnSaveImages.Size = new System.Drawing.Size(75, 23);
            this.btnSaveImages.TabIndex = 2;
            this.btnSaveImages.Text = "Save";
            this.btnSaveImages.UseVisualStyleBackColor = true;
            this.btnSaveImages.Click += new System.EventHandler(this.btnSaveImages_Click);
            // 
            // tabHANodes
            // 
            this.tabHANodes.Controls.Add(this.tableLayoutPanel10);
            this.tabHANodes.Location = new System.Drawing.Point(4, 22);
            this.tabHANodes.Name = "tabHANodes";
            this.tabHANodes.Padding = new System.Windows.Forms.Padding(3);
            this.tabHANodes.Size = new System.Drawing.Size(403, 323);
            this.tabHANodes.TabIndex = 3;
            this.tabHANodes.Text = "HA Nodes";
            this.tabHANodes.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 1;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.Controls.Add(this.btnSavePollingConfig, 0, 3);
            this.tableLayoutPanel10.Controls.Add(this.tableLayoutPanel11, 0, 1);
            this.tableLayoutPanel10.Controls.Add(this.dgvServers, 0, 2);
            this.tableLayoutPanel10.Controls.Add(this.tableLayoutPanel12, 0, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 4;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(397, 317);
            this.tableLayoutPanel10.TabIndex = 2;
            // 
            // btnSavePollingConfig
            // 
            this.btnSavePollingConfig.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSavePollingConfig.Enabled = false;
            this.btnSavePollingConfig.Location = new System.Drawing.Point(319, 290);
            this.btnSavePollingConfig.Name = "btnSavePollingConfig";
            this.btnSavePollingConfig.Size = new System.Drawing.Size(75, 24);
            this.btnSavePollingConfig.TabIndex = 1;
            this.btnSavePollingConfig.Text = "Save";
            this.btnSavePollingConfig.UseVisualStyleBackColor = true;
            this.btnSavePollingConfig.Click += new System.EventHandler(this.btnSavePollingConfig_Click);
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.ColumnCount = 2;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.96164F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.03836F));
            this.tableLayoutPanel11.Controls.Add(this.lblServers, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.btnRefreshPollingServers, 1, 0);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(3, 71);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 1;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.Size = new System.Drawing.Size(391, 26);
            this.tableLayoutPanel11.TabIndex = 2;
            // 
            // lblServers
            // 
            this.lblServers.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblServers.AutoSize = true;
            this.lblServers.Location = new System.Drawing.Point(3, 8);
            this.lblServers.Name = "lblServers";
            this.lblServers.Size = new System.Drawing.Size(89, 13);
            this.lblServers.TabIndex = 0;
            this.lblServers.Text = "Running servers:";
            // 
            // btnRefreshPollingServers
            // 
            this.btnRefreshPollingServers.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRefreshPollingServers.Location = new System.Drawing.Point(313, 3);
            this.btnRefreshPollingServers.Name = "btnRefreshPollingServers";
            this.btnRefreshPollingServers.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshPollingServers.TabIndex = 1;
            this.btnRefreshPollingServers.Text = "Refresh";
            this.btnRefreshPollingServers.UseVisualStyleBackColor = true;
            this.btnRefreshPollingServers.Click += new System.EventHandler(this.btnRefreshPollingServers_Click);
            // 
            // dgvServers
            // 
            this.dgvServers.AllowUserToAddRows = false;
            this.dgvServers.AllowUserToDeleteRows = false;
            this.dgvServers.AllowUserToResizeRows = false;
            this.dgvServers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.HostnameColumn,
            this.MacAddressColumn,
            this.UsersPollingPreferenceColumn,
            //this.ImagesPollingPreferenceColumn
            });
            this.dgvServers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvServers.Location = new System.Drawing.Point(3, 103);
            this.dgvServers.Name = "dgvServers";
            this.dgvServers.RowHeadersVisible = false;
            this.dgvServers.Size = new System.Drawing.Size(391, 181);
            this.dgvServers.TabIndex = 3;
            this.dgvServers.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvServers_CellEndEdit);
            // 
            // HostnameColumn
            // 
            this.HostnameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.HostnameColumn.HeaderText = "Host Name";
            this.HostnameColumn.Name = "HostnameColumn";
            this.HostnameColumn.ReadOnly = true;
            this.HostnameColumn.Width = 84;
            // 
            // MacAddressColumn
            // 
            this.MacAddressColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.MacAddressColumn.HeaderText = "MAC Address";
            this.MacAddressColumn.Name = "MacAddressColumn";
            this.MacAddressColumn.ReadOnly = true;
            this.MacAddressColumn.Width = 96;
            // 
            // UsersPollingPreferenceColumn
            // 
            this.UsersPollingPreferenceColumn.HeaderText = "Users Polling";
            this.UsersPollingPreferenceColumn.Name = "UsersPollingPreferenceColumn";
            // 
            // ImagesPollingPreferenceColumn
            // 
            this.ImagesPollingPreferenceColumn.HeaderText = "Images Polling";
            this.ImagesPollingPreferenceColumn.Name = "ImagesPollingPreferenceColumn";
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 3;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 223F));
            this.tableLayoutPanel12.Controls.Add(this.label21, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.label22, 0, 1);
            this.tableLayoutPanel12.Controls.Add(this.nudWaitingInterval, 1, 1);
            this.tableLayoutPanel12.Controls.Add(this.nudHeartbeatInterval, 1, 0);
            this.tableLayoutPanel12.Controls.Add(this.label23, 2, 0);
            this.tableLayoutPanel12.Controls.Add(this.label24, 2, 1);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 2;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(391, 62);
            this.tableLayoutPanel12.TabIndex = 4;
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(3, 9);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(101, 13);
            this.label21.TabIndex = 0;
            this.label21.Text = "Heartbeat Interval:";
            // 
            // label22
            // 
            this.label22.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(3, 40);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(88, 13);
            this.label22.TabIndex = 1;
            this.label22.Text = "Waiting Interval:";
            // 
            // nudWaitingInterval
            // 
            this.nudWaitingInterval.Location = new System.Drawing.Point(110, 34);
            this.nudWaitingInterval.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.nudWaitingInterval.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudWaitingInterval.Name = "nudWaitingInterval";
            this.nudWaitingInterval.Size = new System.Drawing.Size(55, 21);
            this.nudWaitingInterval.TabIndex = 2;
            this.nudWaitingInterval.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudWaitingInterval.ValueChanged += new System.EventHandler(this.nudWaitingInterval_ValueChanged);
            // 
            // nudHeartbeatInterval
            // 
            this.nudHeartbeatInterval.Location = new System.Drawing.Point(110, 3);
            this.nudHeartbeatInterval.Maximum = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            this.nudHeartbeatInterval.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudHeartbeatInterval.Name = "nudHeartbeatInterval";
            this.nudHeartbeatInterval.Size = new System.Drawing.Size(55, 21);
            this.nudHeartbeatInterval.TabIndex = 3;
            this.nudHeartbeatInterval.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudHeartbeatInterval.ValueChanged += new System.EventHandler(this.nudHeartbeatInterval_ValueChanged);
            // 
            // label23
            // 
            this.label23.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(171, 9);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(23, 13);
            this.label23.TabIndex = 4;
            this.label23.Text = "sec";
            // 
            // label24
            // 
            this.label24.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(171, 40);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(23, 13);
            this.label24.TabIndex = 5;
            this.label24.Text = "sec";
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnClose.BackColor = System.Drawing.SystemColors.Control;
            this.BtnClose.Location = new System.Drawing.Point(356, 367);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(67, 23);
            this.BtnClose.TabIndex = 4;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = false;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);            
            // 
            // DlSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(435, 402);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.tcOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DlSettings";
            this.tcOptions.ResumeLayout(false);
            this.tabMessage.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.tabEngineOptions.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPasscodeLength)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupPincodeLength)).EndInit();
            this.tabOtherFunctions.ResumeLayout(false);
            this.tableLayoutPanel13.ResumeLayout(false);
            this.tableLayoutPanel13.PerformLayout();
            this.tabAuthOptions.ResumeLayout(false);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.tabLoginOptions.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tabAliases.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel4.ResumeLayout(false);
            this.tabMutualAuth.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel9.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightImage)).EndInit();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.tabHANodes.ResumeLayout(false);
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServers)).EndInit();
            this.tableLayoutPanel12.ResumeLayout(false);
            this.tableLayoutPanel12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWaitingInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeartbeatInterval)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.TabControl tcOptions;
        private System.Windows.Forms.TabPage tabMessage;
        private System.Windows.Forms.TabPage tabEngineOptions;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Button BtnOkSMS;
        private System.Windows.Forms.Button BtnOKPassCode;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.NumericUpDown nudPasscodeLength;
        private System.Windows.Forms.GroupBox GroupBox1;
        private System.Windows.Forms.Label lblLegend;
        private System.Windows.Forms.TabPage tabAuthOptions;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        private System.Windows.Forms.GroupBox GroupBox2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox gbConfig;
        private System.Windows.Forms.TabPage tabHANodes;
        private System.Windows.Forms.FlowLayoutPanel flpRB;        
        private System.Windows.Forms.ToolTip ttProvider;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ComboBox cbMsgLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox cbAskMissing;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbPincode;
        private System.Windows.Forms.NumericUpDown nupPincodeLength;
        private System.Windows.Forms.TextBox tbMissingEmail;
        private System.Windows.Forms.TextBox tbMissingPhone;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cbKeyBase;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox cbAskPin;
        private System.Windows.Forms.CheckBox cbAskProviderInfo;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox cbPasswordVaulting;
        private TabPage tabLoginOptions;
        private CheckBox cbAllowAliasesInLogin;
        private Button btnSaveLoginOptions;
        private CheckBox cbMobileNumberLogin;
        private CheckBox cbEmailAddressLogin;
        private CheckBox cbPre2000Login;
        private CheckBox cbUPNLogin;
        private CheckBox cbUsernameLogin;
        private TableLayoutPanel tableLayoutPanel6;
        private CheckBox cbSAMLoginPreferred;
        private TabPage tabAliases;
        private TableLayoutPanel tableLayoutPanel7;
        private FlowLayoutPanel flowLayoutPanel2;
        private Label label14;
        private ComboBoxEx cbDomain;
        private FlowLayoutPanel flowLayoutPanel3;
        private Label label15;
        private ListBox lbAliases;
        private Button btnSaveAliases;
        private FlowLayoutPanel flowLayoutPanel4;
        private Button btnAddAlias;
        private Button btnEditAlias;
        private Button btnRemoveAlias;
        private Label label16;
        private CheckBox cbPincodePanicMode;
        private Button btnResetPanic;
        private Label label17;
        private CheckBox cbMutualAuthMode;
        private TabPage tabMutualAuth;
        private TableLayoutPanel tableLayoutPanel8;
        private TableLayoutPanel tableLayoutPanel9;
        private FlowLayoutPanel flowLayoutPanel5;
        private Button btnSelectLeftImage;
        private Button btnRightImage;
        private Button btnSaveImages;
        private SplitContainer splitContainer1;
        private Label label18;
        private SplitContainer splitContainer2;
        private Label label19;
        private PictureBox pbLeftImage;
        private PictureBox pbRightImage;
        private TableLayoutPanel tableLayoutPanel10;
        private Button btnSavePollingConfig;
        private TableLayoutPanel tableLayoutPanel11;
        private Label lblServers;
        private Button btnRefreshPollingServers;
        private DataGridView dgvServers;
        private TableLayoutPanel tableLayoutPanel12;
        private Label label21;
        private Label label22;
        private NumericUpDown nudWaitingInterval;
        private NumericUpDown nudHeartbeatInterval;
        private Label label23;
        private Label label24;
        private TabPage tabOtherFunctions;
        private TableLayoutPanel tableLayoutPanel13;
        private Label lblTokens;
        private Button btnPollUsers;
        private DataGridViewTextBoxColumn HostnameColumn;
        private DataGridViewTextBoxColumn MacAddressColumn;
        private DataGridViewComboBoxColumn UsersPollingPreferenceColumn;
        private DataGridViewComboBoxColumn ImagesPollingPreferenceColumn;
        private CheckBox cbUseOATHDefaults;
        private FlowLayoutPanel flowLayoutPanel6;
        private Label label20;        
    }
}
