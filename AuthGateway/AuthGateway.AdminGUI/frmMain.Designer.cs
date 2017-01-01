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
	public partial class frmMain : System.Windows.Forms.Form
	{

		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.  
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tooltipMain = new System.Windows.Forms.ToolTip(this.components);
            this.cbFilterByUserType = new System.Windows.Forms.ComboBox();
            this.lblEmailOverride = new System.Windows.Forms.Label();
            this.cbOnlyOverriden = new System.Windows.Forms.CheckBox();
            this.lblMobileOverride = new System.Windows.Forms.Label();
            this.lblPhoneOverride = new System.Windows.Forms.Label();
            this.btnEmergencyToken = new System.Windows.Forms.Button();
            this.lblLockedDown = new System.Windows.Forms.Label();
            this.btnClearPin = new System.Windows.Forms.Button();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.pnlControlsContainer = new System.Windows.Forms.Panel();
            this.tlpAccountDetailsMain = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.Label1 = new System.Windows.Forms.Label();
            this.TxtSearch = new AuthGateway.AdminGUI.Controls.DelayedTextBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblFilterProvider = new System.Windows.Forms.Label();
            this.cbFilterProvider = new System.Windows.Forms.ComboBox();
            this.LstUsers = new System.Windows.Forms.ListBox();
            this.tlpAccountDetailsFields = new System.Windows.Forms.TableLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.TxtUsername = new System.Windows.Forms.TextBox();
            this.BtnInsert = new System.Windows.Forms.Button();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.TxtPhone = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.TxtMobile = new System.Windows.Forms.TextBox();
            this.CboUserType = new System.Windows.Forms.ComboBox();
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.lblPincode = new System.Windows.Forms.Label();
            this.tbPincode = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblAuthEnabled = new System.Windows.Forms.Label();
            this.cbAuthEnabled = new System.Windows.Forms.CheckBox();
            this.cbLockedDown = new System.Windows.Forms.CheckBox();
            this.lblLockedDownYesNo = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.pnlResizeWindow = new System.Windows.Forms.Panel();
            this.pnlButtonsUp = new System.Windows.Forms.Panel();
            this.btnAccountDetails = new System.Windows.Forms.Button();
            this.BtnUsers = new System.Windows.Forms.Button();
            this.btnUsersCount = new System.Windows.Forms.Button();
            this.pnlLogoAndButtons = new System.Windows.Forms.Panel();
            this.btnMaximize = new System.Windows.Forms.Button();
            this.pnlLogo = new System.Windows.Forms.Panel();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnMin = new System.Windows.Forms.Button();
            this.txtUPN = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.pnlContainer.SuspendLayout();
            this.pnlControlsContainer.SuspendLayout();
            this.tlpAccountDetailsMain.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.tlpAccountDetailsFields.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlButtonsUp.SuspendLayout();
            this.pnlLogoAndButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbFilterByUserType
            // 
            this.cbFilterByUserType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilterByUserType.FormattingEnabled = true;
            this.cbFilterByUserType.Items.AddRange(new object[] {
            "All",
            "Users",
            "Administrators"});
            this.cbFilterByUserType.Location = new System.Drawing.Point(93, 3);
            this.cbFilterByUserType.Name = "cbFilterByUserType";
            this.cbFilterByUserType.Size = new System.Drawing.Size(53, 21);
            this.cbFilterByUserType.TabIndex = 170;
            this.tooltipMain.SetToolTip(this.cbFilterByUserType, "Select user type to be shown on the list");
            this.cbFilterByUserType.SelectedIndexChanged += new System.EventHandler(this.CbFilterByUserTypeSelectedIndexChanged);
            // 
            // lblEmailOverride
            // 
            this.lblEmailOverride.AutoSize = true;
            this.lblEmailOverride.BackColor = System.Drawing.Color.Transparent;
            this.lblEmailOverride.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmailOverride.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.lblEmailOverride.Location = new System.Drawing.Point(309, 180);
            this.lblEmailOverride.Name = "lblEmailOverride";
            this.lblEmailOverride.Size = new System.Drawing.Size(19, 17);
            this.lblEmailOverride.TabIndex = 79;
            this.lblEmailOverride.Text = "M";
            this.lblEmailOverride.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tooltipMain.SetToolTip(this.lblEmailOverride, "User has manually entered a number.");
            // 
            // cbOnlyOverriden
            // 
            this.cbOnlyOverriden.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbOnlyOverriden.AutoSize = true;
            this.cbOnlyOverriden.BackColor = System.Drawing.Color.Transparent;
            this.cbOnlyOverriden.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbOnlyOverriden.Font = new System.Drawing.Font("Segoe UI Semibold", 7.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbOnlyOverriden.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.cbOnlyOverriden.Location = new System.Drawing.Point(60, 3);
            this.cbOnlyOverriden.Name = "cbOnlyOverriden";
            this.cbOnlyOverriden.Size = new System.Drawing.Size(27, 23);
            this.cbOnlyOverriden.TabIndex = 160;
            this.cbOnlyOverriden.Text = "M";
            this.tooltipMain.SetToolTip(this.cbOnlyOverriden, "When checked, only users which manually\r\nentered a mobile or phone number will be" +
        "\r\nshown in the list.");
            this.cbOnlyOverriden.UseVisualStyleBackColor = false;
            this.cbOnlyOverriden.CheckedChanged += new System.EventHandler(this.cbOnlyOverriden_CheckedChanged);
            // 
            // lblMobileOverride
            // 
            this.lblMobileOverride.AutoSize = true;
            this.lblMobileOverride.BackColor = System.Drawing.Color.Transparent;
            this.lblMobileOverride.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMobileOverride.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.lblMobileOverride.Location = new System.Drawing.Point(309, 150);
            this.lblMobileOverride.Name = "lblMobileOverride";
            this.lblMobileOverride.Size = new System.Drawing.Size(19, 17);
            this.lblMobileOverride.TabIndex = 77;
            this.lblMobileOverride.Text = "M";
            this.lblMobileOverride.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tooltipMain.SetToolTip(this.lblMobileOverride, "User has manually entered a number.");
            // 
            // lblPhoneOverride
            // 
            this.lblPhoneOverride.AutoSize = true;
            this.lblPhoneOverride.BackColor = System.Drawing.Color.Transparent;
            this.lblPhoneOverride.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPhoneOverride.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.lblPhoneOverride.Location = new System.Drawing.Point(309, 120);
            this.lblPhoneOverride.Name = "lblPhoneOverride";
            this.lblPhoneOverride.Size = new System.Drawing.Size(19, 17);
            this.lblPhoneOverride.TabIndex = 60;
            this.lblPhoneOverride.Text = "M";
            this.lblPhoneOverride.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tooltipMain.SetToolTip(this.lblPhoneOverride, "User has manually entered a number.");
            // 
            // btnEmergencyToken
            // 
            this.btnEmergencyToken.BackColor = System.Drawing.Color.Transparent;
            this.btnEmergencyToken.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEmergencyToken.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEmergencyToken.Font = new System.Drawing.Font("Segoe UI Semibold", 7.75F, System.Drawing.FontStyle.Bold);
            this.btnEmergencyToken.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.btnEmergencyToken.Location = new System.Drawing.Point(69, 3);
            this.btnEmergencyToken.Name = "btnEmergencyToken";
            this.btnEmergencyToken.Size = new System.Drawing.Size(110, 23);
            this.btnEmergencyToken.TabIndex = 130;
            this.btnEmergencyToken.Text = "Emergency Token";
            this.tooltipMain.SetToolTip(this.btnEmergencyToken, "Generate a token for this user.");
            this.btnEmergencyToken.UseVisualStyleBackColor = false;
            this.btnEmergencyToken.Click += new System.EventHandler(this.btnEmergencyToken_Click);
            // 
            // lblLockedDown
            // 
            this.lblLockedDown.BackColor = System.Drawing.Color.Transparent;
            this.lblLockedDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLockedDown.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLockedDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.lblLockedDown.Location = new System.Drawing.Point(3, 300);
            this.lblLockedDown.Name = "lblLockedDown";
            this.lblLockedDown.Size = new System.Drawing.Size(118, 30);
            this.lblLockedDown.TabIndex = 72;
            this.lblLockedDown.Text = "Locked Down:";
            this.lblLockedDown.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tooltipMain.SetToolTip(this.lblLockedDown, "When locked down a user won\'t be able\r\nto change it\'s auth settings.");
            // 
            // btnClearPin
            // 
            this.btnClearPin.BackColor = System.Drawing.Color.Transparent;
            this.btnClearPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClearPin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearPin.Font = new System.Drawing.Font("Segoe UI Semibold", 7.75F, System.Drawing.FontStyle.Bold);
            this.btnClearPin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.btnClearPin.Location = new System.Drawing.Point(69, 30);
            this.btnClearPin.Name = "btnClearPin";
            this.btnClearPin.Size = new System.Drawing.Size(110, 23);
            this.btnClearPin.TabIndex = 131;
            this.btnClearPin.Text = "Clear Pin Code";
            this.tooltipMain.SetToolTip(this.btnClearPin, "Clear the Pincode of this user");
            this.btnClearPin.UseVisualStyleBackColor = false;
            this.btnClearPin.Click += new System.EventHandler(this.BtnClearPinClick);
            // 
            // pnlContainer
            // 
            this.pnlContainer.BackColor = System.Drawing.Color.White;
            this.pnlContainer.Controls.Add(this.pnlControlsContainer);
            this.pnlContainer.Controls.Add(this.pnlLogoAndButtons);
            this.pnlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContainer.Location = new System.Drawing.Point(2, 2);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Padding = new System.Windows.Forms.Padding(3);
            this.pnlContainer.Size = new System.Drawing.Size(667, 530);
            this.pnlContainer.TabIndex = 46;
            this.pnlContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindow_MouseDown);
            // 
            // pnlControlsContainer
            // 
            this.pnlControlsContainer.Controls.Add(this.tlpAccountDetailsMain);
            this.pnlControlsContainer.Controls.Add(this.pnlBottom);
            this.pnlControlsContainer.Controls.Add(this.pnlButtonsUp);
            this.pnlControlsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlControlsContainer.Location = new System.Drawing.Point(3, 55);
            this.pnlControlsContainer.Name = "pnlControlsContainer";
            this.pnlControlsContainer.Size = new System.Drawing.Size(661, 472);
            this.pnlControlsContainer.TabIndex = 86;
            this.pnlControlsContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindow_MouseDown);
            // 
            // tlpAccountDetailsMain
            // 
            this.tlpAccountDetailsMain.ColumnCount = 2;
            this.tlpAccountDetailsMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 337F));
            this.tlpAccountDetailsMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAccountDetailsMain.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tlpAccountDetailsMain.Controls.Add(this.flowLayoutPanel2, 1, 2);
            this.tlpAccountDetailsMain.Controls.Add(this.LstUsers, 1, 1);
            this.tlpAccountDetailsMain.Controls.Add(this.tlpAccountDetailsFields, 0, 0);
            this.tlpAccountDetailsMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAccountDetailsMain.Location = new System.Drawing.Point(0, 33);
            this.tlpAccountDetailsMain.Margin = new System.Windows.Forms.Padding(0);
            this.tlpAccountDetailsMain.Name = "tlpAccountDetailsMain";
            this.tlpAccountDetailsMain.RowCount = 3;
            this.tlpAccountDetailsMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpAccountDetailsMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAccountDetailsMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tlpAccountDetailsMain.Size = new System.Drawing.Size(661, 406);
            this.tlpAccountDetailsMain.TabIndex = 191;
            this.tlpAccountDetailsMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindow_MouseDown);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.Label1);
            this.flowLayoutPanel1.Controls.Add(this.TxtSearch);
            this.flowLayoutPanel1.Controls.Add(this.cbOnlyOverriden);
            this.flowLayoutPanel1.Controls.Add(this.cbFilterByUserType);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(337, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(324, 30);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.BackColor = System.Drawing.Color.Transparent;
            this.Label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.Label1.Location = new System.Drawing.Point(3, 0);
            this.Label1.MinimumSize = new System.Drawing.Size(0, 25);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(51, 25);
            this.Label1.TabIndex = 55;
            this.Label1.Text = "Search:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TxtSearch
            // 
            this.TxtSearch.DelayedTextChangedTimeout = 500;
            this.TxtSearch.Location = new System.Drawing.Point(60, 3);
            this.TxtSearch.Name = "TxtSearch";
            this.TxtSearch.Size = new System.Drawing.Size(123, 21);
            this.TxtSearch.TabIndex = 150;
            this.TxtSearch.DelayedTextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.lblFilterProvider);
            this.flowLayoutPanel2.Controls.Add(this.cbFilterProvider);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(337, 372);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(324, 34);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // lblFilterProvider
            // 
            this.lblFilterProvider.BackColor = System.Drawing.Color.Transparent;
            this.lblFilterProvider.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterProvider.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.lblFilterProvider.Location = new System.Drawing.Point(3, 0);
            this.lblFilterProvider.MinimumSize = new System.Drawing.Size(0, 25);
            this.lblFilterProvider.Name = "lblFilterProvider";
            this.lblFilterProvider.Size = new System.Drawing.Size(117, 25);
            this.lblFilterProvider.TabIndex = 81;
            this.lblFilterProvider.Text = "Filter by Provider:";
            this.lblFilterProvider.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbFilterProvider
            // 
            this.cbFilterProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilterProvider.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFilterProvider.FormattingEnabled = true;
            this.cbFilterProvider.ItemHeight = 14;
            this.cbFilterProvider.Location = new System.Drawing.Point(126, 3);
            this.cbFilterProvider.Name = "cbFilterProvider";
            this.cbFilterProvider.Size = new System.Drawing.Size(128, 22);
            this.cbFilterProvider.TabIndex = 190;
            this.cbFilterProvider.SelectedIndexChanged += new System.EventHandler(this.CbFilterProviderSelectedIndexChanged);
            // 
            // LstUsers
            // 
            this.LstUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LstUsers.FormattingEnabled = true;
            this.LstUsers.IntegralHeight = false;
            this.LstUsers.Location = new System.Drawing.Point(340, 33);
            this.LstUsers.Name = "LstUsers";
            this.LstUsers.Size = new System.Drawing.Size(318, 336);
            this.LstUsers.TabIndex = 180;
            this.LstUsers.SelectedIndexChanged += new System.EventHandler(this.LstUsers_SelectedIndexChanged);
            // 
            // tlpAccountDetailsFields
            // 
            this.tlpAccountDetailsFields.ColumnCount = 3;
            this.tlpAccountDetailsFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 124F));
            this.tlpAccountDetailsFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAccountDetailsFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpAccountDetailsFields.Controls.Add(this.label11, 0, 1);
            this.tlpAccountDetailsFields.Controls.Add(this.label8, 0, 3);
            this.tlpAccountDetailsFields.Controls.Add(this.Label2, 0, 0);
            this.tlpAccountDetailsFields.Controls.Add(this.Label3, 0, 2);
            this.tlpAccountDetailsFields.Controls.Add(this.TxtUsername, 1, 0);
            this.tlpAccountDetailsFields.Controls.Add(this.BtnInsert, 1, 11);
            this.tlpAccountDetailsFields.Controls.Add(this.txtLastName, 1, 2);
            this.tlpAccountDetailsFields.Controls.Add(this.Label4, 0, 4);
            this.tlpAccountDetailsFields.Controls.Add(this.TxtPhone, 1, 4);
            this.tlpAccountDetailsFields.Controls.Add(this.Label5, 0, 5);
            this.tlpAccountDetailsFields.Controls.Add(this.TxtMobile, 1, 5);
            this.tlpAccountDetailsFields.Controls.Add(this.lblPhoneOverride, 2, 4);
            this.tlpAccountDetailsFields.Controls.Add(this.CboUserType, 1, 7);
            this.tlpAccountDetailsFields.Controls.Add(this.lblMobileOverride, 2, 5);
            this.tlpAccountDetailsFields.Controls.Add(this.lblEmailOverride, 2, 6);
            this.tlpAccountDetailsFields.Controls.Add(this.lblLockedDown, 0, 10);
            this.tlpAccountDetailsFields.Controls.Add(this.tbEmail, 1, 6);
            this.tlpAccountDetailsFields.Controls.Add(this.Label9, 0, 6);
            this.tlpAccountDetailsFields.Controls.Add(this.Label6, 0, 7);
            this.tlpAccountDetailsFields.Controls.Add(this.Label7, 0, 9);
            this.tlpAccountDetailsFields.Controls.Add(this.lblPincode, 0, 8);
            this.tlpAccountDetailsFields.Controls.Add(this.tbPincode, 1, 8);
            this.tlpAccountDetailsFields.Controls.Add(this.panel1, 1, 9);
            this.tlpAccountDetailsFields.Controls.Add(this.txtFirstName, 1, 3);
            this.tlpAccountDetailsFields.Controls.Add(this.txtUPN, 1, 1);
            this.tlpAccountDetailsFields.Location = new System.Drawing.Point(3, 3);
            this.tlpAccountDetailsFields.Name = "tlpAccountDetailsFields";
            this.tlpAccountDetailsFields.RowCount = 12;
            this.tlpAccountDetailsMain.SetRowSpan(this.tlpAccountDetailsFields, 2);
            this.tlpAccountDetailsFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpAccountDetailsFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpAccountDetailsFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpAccountDetailsFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpAccountDetailsFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpAccountDetailsFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpAccountDetailsFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpAccountDetailsFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpAccountDetailsFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpAccountDetailsFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpAccountDetailsFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpAccountDetailsFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpAccountDetailsFields.Size = new System.Drawing.Size(331, 366);
            this.tlpAccountDetailsFields.TabIndex = 86;
            this.tlpAccountDetailsFields.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindow_MouseDown);
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.label8.Location = new System.Drawing.Point(3, 90);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(118, 30);
            this.label8.TabIndex = 87;
            this.label8.Text = "First Name:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.Color.Transparent;
            this.Label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.Label2.Location = new System.Drawing.Point(3, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(118, 30);
            this.Label2.TabIndex = 56;
            this.Label2.Text = "Username:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label3
            // 
            this.Label3.BackColor = System.Drawing.Color.Transparent;
            this.Label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label3.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.Label3.Location = new System.Drawing.Point(3, 60);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(118, 30);
            this.Label3.TabIndex = 57;
            this.Label3.Text = "Last Name:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TxtUsername
            // 
            this.TxtUsername.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtUsername.Location = new System.Drawing.Point(127, 3);
            this.TxtUsername.Name = "TxtUsername";
            this.TxtUsername.Size = new System.Drawing.Size(176, 21);
            this.TxtUsername.TabIndex = 30;
            // 
            // BtnInsert
            // 
            this.BtnInsert.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnInsert.BackColor = System.Drawing.Color.Green;
            this.BtnInsert.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnInsert.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtnInsert.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BtnInsert.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnInsert.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.BtnInsert.ForeColor = System.Drawing.Color.White;
            this.BtnInsert.Location = new System.Drawing.Point(137, 333);
            this.BtnInsert.Name = "BtnInsert";
            this.BtnInsert.Size = new System.Drawing.Size(166, 30);
            this.BtnInsert.TabIndex = 140;
            this.BtnInsert.Text = "Update Account Details";
            this.BtnInsert.UseVisualStyleBackColor = false;
            this.BtnInsert.Click += new System.EventHandler(this.BtnInsert_Click);
            // 
            // txtLastName
            // 
            this.txtLastName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLastName.Location = new System.Drawing.Point(127, 63);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(176, 21);
            this.txtLastName.TabIndex = 40;
            // 
            // Label4
            // 
            this.Label4.BackColor = System.Drawing.Color.Transparent;
            this.Label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label4.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.Label4.Location = new System.Drawing.Point(3, 120);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(118, 30);
            this.Label4.TabIndex = 58;
            this.Label4.Text = "Phone Number:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TxtPhone
            // 
            this.TxtPhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtPhone.Location = new System.Drawing.Point(127, 123);
            this.TxtPhone.Name = "TxtPhone";
            this.TxtPhone.Size = new System.Drawing.Size(176, 21);
            this.TxtPhone.TabIndex = 60;
            // 
            // Label5
            // 
            this.Label5.BackColor = System.Drawing.Color.Transparent;
            this.Label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label5.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.Label5.Location = new System.Drawing.Point(3, 150);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(118, 30);
            this.Label5.TabIndex = 59;
            this.Label5.Text = "Mobile Number:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TxtMobile
            // 
            this.TxtMobile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtMobile.Location = new System.Drawing.Point(127, 153);
            this.TxtMobile.Name = "TxtMobile";
            this.TxtMobile.Size = new System.Drawing.Size(176, 21);
            this.TxtMobile.TabIndex = 70;
            // 
            // CboUserType
            // 
            this.CboUserType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CboUserType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CboUserType.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CboUserType.FormattingEnabled = true;
            this.CboUserType.ItemHeight = 14;
            this.CboUserType.Items.AddRange(new object[] {
            "Administrator",
            "User"});
            this.CboUserType.Location = new System.Drawing.Point(127, 213);
            this.CboUserType.Name = "CboUserType";
            this.CboUserType.Size = new System.Drawing.Size(176, 22);
            this.CboUserType.TabIndex = 90;
            // 
            // tbEmail
            // 
            this.tbEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbEmail.Location = new System.Drawing.Point(127, 183);
            this.tbEmail.Name = "tbEmail";
            this.tbEmail.Size = new System.Drawing.Size(176, 21);
            this.tbEmail.TabIndex = 80;
            // 
            // Label9
            // 
            this.Label9.BackColor = System.Drawing.Color.Transparent;
            this.Label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label9.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.Label9.Location = new System.Drawing.Point(3, 180);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(118, 30);
            this.Label9.TabIndex = 70;
            this.Label9.Text = "E-mail:";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label6
            // 
            this.Label6.BackColor = System.Drawing.Color.Transparent;
            this.Label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label6.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.Label6.Location = new System.Drawing.Point(3, 210);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(118, 30);
            this.Label6.TabIndex = 60;
            this.Label6.Text = "User Type:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label7
            // 
            this.Label7.BackColor = System.Drawing.Color.Transparent;
            this.Label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label7.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.Label7.Location = new System.Drawing.Point(3, 270);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(118, 30);
            this.Label7.TabIndex = 63;
            this.Label7.Text = "SMS2 Enabled:";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPincode
            // 
            this.lblPincode.BackColor = System.Drawing.Color.Transparent;
            this.lblPincode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPincode.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPincode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.lblPincode.Location = new System.Drawing.Point(3, 240);
            this.lblPincode.Name = "lblPincode";
            this.lblPincode.Size = new System.Drawing.Size(118, 30);
            this.lblPincode.TabIndex = 66;
            this.lblPincode.Text = "Pin Code:";
            this.lblPincode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbPincode
            // 
            this.tbPincode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbPincode.Location = new System.Drawing.Point(127, 243);
            this.tbPincode.Name = "tbPincode";
            this.tbPincode.Size = new System.Drawing.Size(176, 21);
            this.tbPincode.TabIndex = 100;
            this.tbPincode.UseSystemPasswordChar = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnClearPin);
            this.panel1.Controls.Add(this.btnEmergencyToken);
            this.panel1.Controls.Add(this.lblAuthEnabled);
            this.panel1.Controls.Add(this.cbAuthEnabled);
            this.panel1.Controls.Add(this.cbLockedDown);
            this.panel1.Controls.Add(this.lblLockedDownYesNo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(124, 270);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.tlpAccountDetailsFields.SetRowSpan(this.panel1, 2);
            this.panel1.Size = new System.Drawing.Size(182, 60);
            this.panel1.TabIndex = 80;
            // 
            // lblAuthEnabled
            // 
            this.lblAuthEnabled.AutoSize = true;
            this.lblAuthEnabled.BackColor = System.Drawing.Color.Transparent;
            this.lblAuthEnabled.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuthEnabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.lblAuthEnabled.Location = new System.Drawing.Point(3, 7);
            this.lblAuthEnabled.Name = "lblAuthEnabled";
            this.lblAuthEnabled.Size = new System.Drawing.Size(26, 17);
            this.lblAuthEnabled.TabIndex = 64;
            this.lblAuthEnabled.Text = "No";
            this.lblAuthEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAuthEnabled
            // 
            this.cbAuthEnabled.AutoSize = true;
            this.cbAuthEnabled.Enabled = false;
            this.cbAuthEnabled.Location = new System.Drawing.Point(39, 10);
            this.cbAuthEnabled.Name = "cbAuthEnabled";
            this.cbAuthEnabled.Size = new System.Drawing.Size(15, 14);
            this.cbAuthEnabled.TabIndex = 110;
            this.cbAuthEnabled.UseVisualStyleBackColor = true;
            this.cbAuthEnabled.Visible = false;
            this.cbAuthEnabled.CheckedChanged += new System.EventHandler(this.cbAuthEnabled_CheckedChanged);
            // 
            // cbLockedDown
            // 
            this.cbLockedDown.AutoSize = true;
            this.cbLockedDown.Enabled = false;
            this.cbLockedDown.Location = new System.Drawing.Point(39, 40);
            this.cbLockedDown.Name = "cbLockedDown";
            this.cbLockedDown.Size = new System.Drawing.Size(15, 14);
            this.cbLockedDown.TabIndex = 120;
            this.cbLockedDown.UseVisualStyleBackColor = true;
            this.cbLockedDown.CheckedChanged += new System.EventHandler(this.cbLockedDown_CheckedChanged);
            // 
            // lblLockedDownYesNo
            // 
            this.lblLockedDownYesNo.AutoSize = true;
            this.lblLockedDownYesNo.BackColor = System.Drawing.Color.Transparent;
            this.lblLockedDownYesNo.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLockedDownYesNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.lblLockedDownYesNo.Location = new System.Drawing.Point(3, 37);
            this.lblLockedDownYesNo.Name = "lblLockedDownYesNo";
            this.lblLockedDownYesNo.Size = new System.Drawing.Size(26, 17);
            this.lblLockedDownYesNo.TabIndex = 73;
            this.lblLockedDownYesNo.Text = "No";
            this.lblLockedDownYesNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFirstName.Location = new System.Drawing.Point(127, 93);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(176, 21);
            this.txtFirstName.TabIndex = 50;
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.Black;
            this.pnlBottom.Controls.Add(this.lblVersion);
            this.pnlBottom.Controls.Add(this.pnlResizeWindow);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 439);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(661, 33);
            this.pnlBottom.TabIndex = 85;
            this.pnlBottom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindow_MouseDown);
            // 
            // lblVersion
            // 
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.LightGray;
            this.lblVersion.Location = new System.Drawing.Point(454, 0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(177, 33);
            this.lblVersion.TabIndex = 75;
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblVersion.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindow_MouseDown);
            // 
            // pnlResizeWindow
            // 
            this.pnlResizeWindow.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.pnlResizeWindow.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlResizeWindow.Location = new System.Drawing.Point(631, 0);
            this.pnlResizeWindow.Name = "pnlResizeWindow";
            this.pnlResizeWindow.Size = new System.Drawing.Size(30, 33);
            this.pnlResizeWindow.TabIndex = 191;
            this.pnlResizeWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnlResizeWindowMouseDown);
            // 
            // pnlButtonsUp
            // 
            this.pnlButtonsUp.BackColor = System.Drawing.Color.Black;
            this.pnlButtonsUp.Controls.Add(this.btnAccountDetails);
            this.pnlButtonsUp.Controls.Add(this.BtnUsers);
            this.pnlButtonsUp.Controls.Add(this.btnUsersCount);
            this.pnlButtonsUp.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtonsUp.Location = new System.Drawing.Point(0, 0);
            this.pnlButtonsUp.Name = "pnlButtonsUp";
            this.pnlButtonsUp.Size = new System.Drawing.Size(661, 33);
            this.pnlButtonsUp.TabIndex = 84;
            this.pnlButtonsUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindow_MouseDown);
            // 
            // btnAccountDetails
            // 
            this.btnAccountDetails.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAccountDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnAccountDetails.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAccountDetails.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAccountDetails.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnAccountDetails.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(94)))));
            this.btnAccountDetails.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(94)))));
            this.btnAccountDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAccountDetails.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.btnAccountDetails.ForeColor = System.Drawing.Color.White;
            this.btnAccountDetails.Location = new System.Drawing.Point(0, 0);
            this.btnAccountDetails.Name = "btnAccountDetails";
            this.btnAccountDetails.Size = new System.Drawing.Size(144, 33);
            this.btnAccountDetails.TabIndex = 10;
            this.btnAccountDetails.Text = "Account Details";
            this.btnAccountDetails.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnAccountDetails.UseVisualStyleBackColor = false;
            // 
            // BtnUsers
            // 
            this.BtnUsers.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnUsers.BackColor = System.Drawing.Color.Transparent;
            this.BtnUsers.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnUsers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnUsers.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BtnUsers.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(94)))));
            this.BtnUsers.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(94)))));
            this.BtnUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnUsers.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.BtnUsers.ForeColor = System.Drawing.Color.White;
            this.BtnUsers.Location = new System.Drawing.Point(144, 0);
            this.BtnUsers.Name = "BtnUsers";
            this.BtnUsers.Size = new System.Drawing.Size(183, 33);
            this.BtnUsers.TabIndex = 20;
            this.BtnUsers.Text = "Authentication Options";
            this.BtnUsers.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnUsers.UseVisualStyleBackColor = false;
            this.BtnUsers.Click += new System.EventHandler(this.BtnUsers_Click);
            // 
            // btnUsersCount
            // 
            this.btnUsersCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUsersCount.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUsersCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnUsersCount.FlatAppearance.BorderSize = 0;
            this.btnUsersCount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsersCount.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUsersCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btnUsersCount.Location = new System.Drawing.Point(536, 5);
            this.btnUsersCount.Name = "btnUsersCount";
            this.btnUsersCount.Size = new System.Drawing.Size(120, 23);
            this.btnUsersCount.TabIndex = 68;
            this.btnUsersCount.TabStop = false;
            this.btnUsersCount.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnUsersCount.UseVisualStyleBackColor = false;
            this.btnUsersCount.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindow_MouseDown);
            // 
            // pnlLogoAndButtons
            // 
            this.pnlLogoAndButtons.Controls.Add(this.btnMaximize);
            this.pnlLogoAndButtons.Controls.Add(this.pnlLogo);
            this.pnlLogoAndButtons.Controls.Add(this.BtnClose);
            this.pnlLogoAndButtons.Controls.Add(this.BtnMin);
            this.pnlLogoAndButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLogoAndButtons.Location = new System.Drawing.Point(3, 3);
            this.pnlLogoAndButtons.Name = "pnlLogoAndButtons";
            this.pnlLogoAndButtons.Size = new System.Drawing.Size(661, 52);
            this.pnlLogoAndButtons.TabIndex = 87;
            this.pnlLogoAndButtons.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindow_MouseDown);
            // 
            // btnMaximize
            // 
            this.btnMaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMaximize.AutoSize = true;
            this.btnMaximize.BackColor = System.Drawing.Color.Transparent;
            this.btnMaximize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnMaximize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMaximize.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnMaximize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaximize.Location = new System.Drawing.Point(596, 3);
            this.btnMaximize.Name = "btnMaximize";
            this.btnMaximize.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.btnMaximize.Size = new System.Drawing.Size(29, 25);
            this.btnMaximize.TabIndex = 1100;
            this.btnMaximize.Text = "+";
            this.btnMaximize.UseVisualStyleBackColor = false;
            this.btnMaximize.Click += new System.EventHandler(this.btnMaximize_Click);
            // 
            // pnlLogo
            // 
            this.pnlLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlLogo.BackgroundImage")));
            this.pnlLogo.Location = new System.Drawing.Point(162, 3);
            this.pnlLogo.Name = "pnlLogo";
            this.pnlLogo.Size = new System.Drawing.Size(291, 43);
            this.pnlLogo.TabIndex = 83;
            this.pnlLogo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindow_MouseDown);
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.AutoSize = true;
            this.BtnClose.BackColor = System.Drawing.Color.Transparent;
            this.BtnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnClose.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnClose.Location = new System.Drawing.Point(631, 3);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.BtnClose.Size = new System.Drawing.Size(27, 25);
            this.BtnClose.TabIndex = 1200;
            this.BtnClose.Text = "x";
            this.BtnClose.UseVisualStyleBackColor = false;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnMin
            // 
            this.BtnMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnMin.AutoSize = true;
            this.BtnMin.BackColor = System.Drawing.Color.Transparent;
            this.BtnMin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BtnMin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnMin.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BtnMin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnMin.Location = new System.Drawing.Point(565, 3);
            this.BtnMin.Name = "BtnMin";
            this.BtnMin.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.BtnMin.Size = new System.Drawing.Size(25, 25);
            this.BtnMin.TabIndex = 1000;
            this.BtnMin.Text = "-";
            this.BtnMin.UseVisualStyleBackColor = false;
            this.BtnMin.Click += new System.EventHandler(this.BtnMin_Click);
            // 
            // txtUPN
            // 
            this.txtUPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUPN.Location = new System.Drawing.Point(127, 33);
            this.txtUPN.Name = "txtUPN";
            this.txtUPN.Size = new System.Drawing.Size(176, 21);
            this.txtUPN.TabIndex = 35;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.label11.Location = new System.Drawing.Point(3, 30);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(118, 30);
            this.label11.TabIndex = 143;
            this.label11.Text = "UPN:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(671, 534);
            this.Controls.Add(this.pnlContainer);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(620, 490);
            this.Name = "frmMain";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SMS2 Administration Console";
            this.pnlContainer.ResumeLayout(false);
            this.pnlControlsContainer.ResumeLayout(false);
            this.tlpAccountDetailsMain.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.tlpAccountDetailsFields.ResumeLayout(false);
            this.tlpAccountDetailsFields.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            this.pnlButtonsUp.ResumeLayout(false);
            this.pnlLogoAndButtons.ResumeLayout(false);
            this.pnlLogoAndButtons.PerformLayout();
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.ToolTip tooltipMain;
		private Panel pnlContainer;
		private Panel pnlLogo;
		private ComboBox cbFilterByUserType;
		private Label lblFilterProvider;
		private ComboBox cbFilterProvider;
		private Label lblEmailOverride;
		private CheckBox cbOnlyOverriden;
		private Label lblMobileOverride;
		private Label lblPhoneOverride;
		private Label lblVersion;
		private Button btnEmergencyToken;
		private Label lblLockedDownYesNo;
		private Label lblLockedDown;
		private CheckBox cbLockedDown;
		private Label Label9;
		private TextBox tbEmail;
		private Label lblPincode;
		private TextBox tbPincode;
		private Label lblAuthEnabled;
		private Label Label7;
		private CheckBox cbAuthEnabled;
		private Label Label6;
		private Label Label5;
		private Label Label4;
		private Label Label3;
		private Label Label2;
		private Label Label1;
		private Button btnUsersCount;
		private Button BtnClose;
		private Button BtnMin;
		private Button BtnInsert;
		private Button BtnUsers;
		private TextBox TxtUsername;
		private TextBox txtLastName;
		private TextBox TxtPhone;
		private TextBox TxtMobile;
		private ComboBox CboUserType;
		private DelayedTextBox TxtSearch;
		private ListBox LstUsers;
		private Panel pnlButtonsUp;
		private Button btnAccountDetails;
		private Panel pnlBottom;
		private System.Windows.Forms.Panel pnlControlsContainer;
		private System.Windows.Forms.Panel pnlLogoAndButtons;
		private System.Windows.Forms.Button btnMaximize;
		private System.Windows.Forms.TableLayoutPanel tlpAccountDetailsFields;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtFirstName;
		private System.Windows.Forms.TableLayoutPanel tlpAccountDetailsMain;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
		private System.Windows.Forms.Panel pnlResizeWindow;
		private System.Windows.Forms.Button btnClearPin;
        private Label label11;
        private TextBox txtUPN;
	}
}
