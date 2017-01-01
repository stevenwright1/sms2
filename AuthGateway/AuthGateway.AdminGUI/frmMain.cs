using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using AuthGateway.Shared;
using AuthGateway.Shared.Identity;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking; 
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using AuthGateway.AdminGUI.Helpers;

namespace AuthGateway.AdminGUI
{
	public partial class frmMain
	{
        public const string VERSION = "16101701";

		public bool tbPincodeChanged { get; set; }

		private Variables clientLogic;        
		
		public frmMain()
		{
			InitializeComponent();
			
			MouseDown += DragWindow_MouseDown;
			Load += FrmUpdateuser_Load;
			
			tbPincode.KeyDown += tbPincode_KeyDown;
			
			LstUsers.DisplayMember = "FullName";
			LstUsers.ValueMember = "u";            
		}        

		public frmMain(Variables clientLogic) : this()
		{            
			this.clientLogic = clientLogic;

            if (clientLogic.SC.SendTrackingInfo) {
                Tracker.Instance.StartTracking("AdminGuiEvent"); 
            }
			
			ShowAdminControls(false);
			this.lblVersion.Text = "Version: " + VERSION;
			this.CboUserType.Items.Clear();
			this.CboUserType.Items.Add(UserType.Administrator);
			this.CboUserType.Items.Add(UserType.User);
			
			cbFilterByUserType.SelectedIndexChanged -= CbFilterProviderSelectedIndexChanged;
			cbFilterByUserType.Items.Clear();
			cbFilterByUserType.Items.Add("All");
			cbFilterByUserType.Items.Add("Users");
			cbFilterByUserType.Items.Add("Administrators");
			cbFilterByUserType.SelectedIndex = 0;
			cbFilterByUserType.SelectedIndexChanged += CbFilterProviderSelectedIndexChanged;			

			lblMobileOverride.Visible = false;
			lblPhoneOverride.Visible = false;

			try {
				WindowsIdentity ident = WindowsIdentity.GetCurrent();
				IPrincipal principal = new WindowsPrincipal(ident);
				var identName = principal.Identity.GetLogin();

				validatePermissions();
				
				if (clientLogic.IsAdmin)
					loadProvidersList();

				LoadUserDetails(identName, clientLogic.OrgNameP);

				loadUsersThreaded();

				TxtUsername.ReadOnly = true;
                txtUPN.ReadOnly = true;

				//Me.tooltipMain.SetToolTip(tbPincode, "Leave empty if you don't want to change your PinCode.")
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("frmMain Error" + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("frmMain StackTrace" + ex.StackTrace, LogLevel.Error);
				Logger.Instance.Flush();
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw;
			}                        
		}

		private void BtnInsert_Click(System.Object sender, System.EventArgs e)
		{
			try {
				if (clientLogic.SC.AdminGUIConfirmPincode && tbPincodeChanged)
				{
					using (var frmConfirm = new frmConfirmPincode(tbPincode.Text))
					{
						var dgResult = frmConfirm.ShowDialog();
						if (dgResult != DialogResult.OK)
							return;
					}
				}
	
				var updateDetailsError = clientLogic
					.UpdateDetails(
						getSelectedUsername(), TxtPhone.Text, TxtMobile.Text
						, txtLastName.Text, txtFirstName.Text, CboUserType.Text, clientLogic.OrgNameP
						, cbAuthEnabled.Checked, tbPincode.Text, tbPincodeChanged, tbEmail.Text
						, cbLockedDown.Checked
                        , txtUPN.Text
						)
					.Error;
	
	
				if (string.IsNullOrEmpty(updateDetailsError)) {
					ShowInfoMessage("User information updated.");
	
	
					if (Environment.UserName == getSelectedUsername()) {
						validatePermissions();
						LoadUserDetails(Environment.UserName, clientLogic.OrgNameP);
						loadUsersThreaded();
					}
				} else {
					ShowErrorMessage("User information update failed." + Environment.NewLine + updateDetailsError);
				}
			} catch( NoUserSelectedException ex ) {
				ShowErrorMessage(ex.Message);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}


		public void ClearBoxes()
		{
			foreach (Control ctr in this.Controls) {
				if (ctr.Name.Contains("Txt")) {
					ctr.Text = "";
				}
			}

		}


		public void BlockBoxes()
		{
			foreach (Control ctr in this.Controls) {
				if (ctr.Name.Contains("Txt")) {
					ctr.Enabled = false;
				}
			}

		}

		public void AllowBoxes()
		{
			foreach (Control ctr in this.Controls) {
				if (ctr.Name.Contains("Txt")) {
					ctr.Enabled = true;
				}
			}
		}
		
		void ShowAdminControls(bool show) {
			cbFilterProvider.Visible = show;
			lblFilterProvider.Visible = show;
		}

		public static string Nuller(object value)
		{
			if (value == null | DBNull.Value == value) {
				value = "";
			}
			return Convert.ToString(value);
		}

		void ClearUserDetails()
		{
			CboUserType.Enabled = false;
			cbAuthEnabled.Enabled = false;
			cbLockedDown.Enabled = false;
			TxtUsername.Text = string.Empty;
			txtLastName.Text = string.Empty;
			txtFirstName.Text = string.Empty;
			TxtPhone.Text = string.Empty;
			TxtMobile.Text = string.Empty;
			tbPincode.Text = string.Empty;
			tbPincodeChanged = false;
			CboUserType.Text = string.Empty;
			cbAuthEnabled.Checked = false;
			cbLockedDown.Checked = false;
			BlockBoxes();
			BtnInsert.Enabled = false;
		}

		private void LoadUserDetails(string user, string domain)
		{
			if ((string.IsNullOrEmpty(user))) {
				return;
			}
			DetailsRet userDetailFields = clientLogic.GetDetails(user, domain);

			if (!string.IsNullOrEmpty(userDetailFields.Error)) {
				ShowErrorMessage("Error getting user details.");
				ClearUserDetails();
				return;
			}

			ShowAdminControls(clientLogic.IsAdmin);
			if (clientLogic.IsAdmin || user == Environment.UserName) {
				CboUserType.Enabled = clientLogic.IsAdmin;
				cbLockedDown.Enabled = clientLogic.IsAdmin;
				TxtUsername.Text = user;               
                txtUPN.Text = userDetailFields.UPN;
				txtLastName.Text = userDetailFields.LastName;
				txtFirstName.Text = userDetailFields.FirstName;
				TxtPhone.Text = userDetailFields.Phone;
				TxtMobile.Text = userDetailFields.Mobile;
				CboUserType.Text = userDetailFields.UserType;
				tbEmail.Text = userDetailFields.Email;
				AllowBoxes();
				BtnInsert.Enabled = true;

				tbPincode.Enabled = userDetailFields.PinCodeEnabled;

				if ((userDetailFields.HasPinCodeEntered)) {
					tbPincode.Text = new string(Convert.ToChar(" "), userDetailFields.PinCodeLength);
				}
				tbPincodeChanged = false;

				cbAuthEnabled.Enabled = false;
				cbAuthEnabled.Checked = userDetailFields.AuthEnabled;

				cbLockedDown.Enabled = false;
				cbLockedDown.Checked = userDetailFields.Locked;
				if (clientLogic.IsAdmin) {
					//cbAuthEnabled.Enabled = True
					cbLockedDown.Enabled = true;
				}

				lblMobileOverride.Visible = userDetailFields.MobileOverrided;
				lblEmailOverride.Visible = userDetailFields.EmailOverrided;
				lblPhoneOverride.Visible = userDetailFields.PhoneOverrided;

				if (!userDetailFields.LockdownMode)
				{
					cbLockedDown.Visible = false;
					lblLockedDown.Visible = false;
					lblLockedDownYesNo.Visible = false;
				} else if(!clientLogic.IsAdmin)
					cbLockedDown.Visible = false;
			} else {
				ClearUserDetails();
			}
		}

		private void LstUsers_SelectedIndexChanged(System.Object sender, System.EventArgs e)
		{
			loadSelectedUserDetails();
		}

		string getSelectedUsername() {
			string username = string.Empty;
			if ((LstUsers == null || LstUsers.SelectedItem == null))
			{
				if (string.IsNullOrEmpty(TxtUsername.Text))
					throw new NoUserSelectedException();
				username = TxtUsername.Text;
			}
			else
				username = ((UserRet)LstUsers.SelectedItem).u;
			const string m = "M: ";
			if (username.StartsWith(m))
				username = username.Substring(m.Length);
			return username;
		}
		private void loadSelectedUserDetails()
		{
			try {
				LoadUserDetails(getSelectedUsername(), clientLogic.OrgNameP);
			} catch( NoUserSelectedException ex ) {
				ShowErrorMessage(ex.Message);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}

		private void BtnUsers_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				string user = getSelectedUsername();
				string org = clientLogic.OrgNameP;
				var userProvidersCmd = new UserProviders()
				{
					User = user,
					Org = org
				};
				DlSettings fDlSettings = new DlSettings(clientLogic, userProvidersCmd, cbLockedDown.Checked);
				fDlSettings.ShowDialog();
				validatePermissions();
				loadSelectedUserDetails();
			} catch (NoUserSelectedException ex) {
				ShowErrorMessage(ex.Message);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
#if DEBUG
			catch(Exception ex) {
				ShowErrorMessage(ex.Message);
			}
#endif
		}

		private void FrmUpdateuser_Load(System.Object sender, System.EventArgs e)
		{
			Show();
		}

		void loadProvidersList()
		{
			var plist = clientLogic.GetProvidersList();
			cbFilterProvider.Items.Clear();
			cbFilterProvider.Items.Add(string.Empty);
			plist.ForEach(x => cbFilterProvider.Items.Add(x));
		}
		
		private void validatePermissions()
		{
			PermissionsRet perms = null;
			Logger.Instance.WriteToLog("LoadServerSettings:validatePermissions", LogLevel.Debug);
			var userName = Environment.UserName;
			var userDomain = Environment.MachineName;
			int Typ = clientLogic.DomainOrWG();
			if (Typ == NetworkInformation.DOMAIN)
			{
				clientLogic.OrgNameP = Environment.UserDomainName;
				userDomain = Environment.UserDomainName;
			}
			perms = clientLogic.validatePermissions(userName, userDomain);

			if (string.IsNullOrEmpty(perms.Error))
			{
				if (perms.Status)
				{
					var isAdmin = false;
					switch (perms.UserType)
					{
						case UserType.Administrator:
							isAdmin = true;
							//cbAuthEnabled.Enabled = True
							AllowBoxes();
							break;
						case UserType.User:
							BtnInsert.Enabled = false;
							cbAuthEnabled.Enabled = false;
							BlockBoxes();
							break;
					}
					CboUserType.Enabled = isAdmin;
					cbLockedDown.Enabled = isAdmin;
					btnEmergencyToken.Visible = isAdmin && perms.AllowEmergency;
					btnEmergencyToken.Enabled = isAdmin && perms.AllowEmergency;
					btnClearPin.Visible = isAdmin && perms.PinCode;
					btnClearPin.Enabled = isAdmin && perms.PinCode;
					clientLogic.IsAdmin = isAdmin;
					ShowAdminControls(isAdmin);
				}
				else
				{
					var errorMsg = "User is disabled."
						+ "\r\n\r\n"
						+ string.Format("Sent credentials: Domain '{0}' User '{1}'", userDomain, userName);
					Logger.Instance.WriteToLog(errorMsg, LogLevel.Error);
					ShowErrorMessage(errorMsg);
					System.Environment.Exit(0);
				}
			}
			else
			{
				var errorMsg = perms.Error
					+ "\r\n\r\n"
					+ string.Format("Sent credentials Domain '{0}' User '{1}'", userDomain, userName);
				Logger.Instance.WriteToLog(errorMsg, LogLevel.Error);
				ShowErrorMessage(errorMsg);
				System.Environment.Exit(0);
			}
		}

		public delegate void btnUsersCountSetCount_Delegate(int users);
		private void btnUsersCountSetCount(int users)
		{
			if (this.InvokeRequired) {
				btnUsersCountSetCount_Delegate del = new btnUsersCountSetCount_Delegate(btnUsersCountSetCount);
				this.Invoke(del, new object[] { users });
			} else {
				btnUsersCount.Text = "Users: " + users.ToString();
			}
		}

		class UserRetConcat : UserRet {
			public string FullName { 
				get {
					var name = string.Empty;
					if (!string.IsNullOrEmpty(l)) {
						name = l;
						if (!string.IsNullOrEmpty(f)) {
						if (name == string.Empty)
							return string.Format("{0} ({1})", u, f);
						return string.Format("{0} ({1}, {2})", u, l, f);
					}
					}
					if (name != string.Empty)
						return string.Format("{0} ({1})", u, name);
					return string.Format("{0}", u);
				}
			}
		}
		
		class NoUserSelectedException : Exception {
			public NoUserSelectedException(): base("No user selected.") {
				
			}
		}
		
		public delegate void loadUserAdd_Delegate(List<UserRet> texts);
		private void loadUserAdd(IList<UserRet> texts)
		{
			if (this.InvokeRequired) {
				loadUserAdd_Delegate del = new loadUserAdd_Delegate(loadUserAdd);
				this.Invoke(del, new object[] { texts });
			} else {
				LstUsers.Items.AddRange(texts.Select(x => new UserRetConcat { u = x.u, d = x.d, f = x.f, l = x.l }).ToArray());
				btnUsersCountSetCount(LstUsers.Items.Count);
			}
		}

		public delegate void loadUserClear_Delegate();
		private void loadUserClear()
		{
			if (this.InvokeRequired) {
				loadUserClear_Delegate del = new loadUserClear_Delegate(loadUserClear);
				this.Invoke(del, new object[] { });
			} else {
				LstUsers.Items.Clear();
				btnUsersCountSetCount(LstUsers.Items.Count);
			}
		}

		private Task loadUsersTask = null;
		private CancellationTokenSource cts = null;
		private void loadUsersThreaded()
		{
			if (loadUsersTask != null)
			{
				if (cts != null)
					cts.Cancel();
			}

			cts = new CancellationTokenSource();
			CancellationToken ct = cts.Token;
			loadUsersTask = Task.Factory.StartNew(() =>
			{
				try
				{
					loadUserClear();
					var cbOnlyOverridenValue = false;
					bool? cbShowAdminsValue = null;
					var cbFilterProviderText = string.Empty;
					var cbText = string.Empty;
					cbOnlyOverriden.PerformSafely(() => cbOnlyOverridenValue = cbOnlyOverriden.Checked );
					cbFilterProvider.PerformSafely(() => cbFilterProviderText = cbFilterProvider.Text );
					TxtSearch.PerformSafely(() => cbText = TxtSearch.Text.Trim() );
					cbFilterByUserType.PerformSafely(() => { 
					                           	switch (cbFilterByUserType.Text) {
					                           		case "All":
					                           			cbShowAdminsValue = null;
					                           			break;
					                           		case "Administrators":
					                           			cbShowAdminsValue = true;
					                           			break;
					                           		case "Users":
					                           			cbShowAdminsValue = false;
					                           			break;
					                           	}
					                           } );
					clientLogic.loadUsersUntilClear(
						loadUserAdd,
						ct.ThrowIfCancellationRequested
						, cbOnlyOverridenValue
						, cbFilterProviderText
						, cbShowAdminsValue
						, cbText
					);
				} catch (OperationCanceledException) {
				} catch (AggregateException) {
				} catch (Exception ex) {
					Logger.Instance.WriteToLog("loadUsers Error" + ex.Message, LogLevel.Error);
					Logger.Instance.WriteToLog("loadUsers StackTrace" + ex.StackTrace, LogLevel.Error);
					Logger.Instance.Flush();
					ShowErrorMessage("An error occurred retrieving the list of users.");
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				}
			}
			, cts.Token);
		}

		private void BtnClose_Click(System.Object sender, System.EventArgs e)
		{
			this.Close();            
		}

		private void BtnMin_Click(System.Object sender, System.EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
		}

		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		public const int HT_BOTTOMRIGHT = 17;
		
		[DllImportAttribute("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();
		
		void DragWindow_MouseDown(object sender, MouseEventArgs e)
		{     
		    if (e.Button == MouseButtons.Left)
		    {
		        ReleaseCapture();
		        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
		    }
		}

		private delegate void DoFromThreadDelegate();
		private delegate void DoFromThreadDelegateWithInt(int @int);

		private Thread searchThread;
		private void StartSearch(string searchText)
		{
			searchText = searchText.ToUpper();

			if ((searchThread != null)) {
				if (searchThread.ThreadState == System.Threading.ThreadState.Running) {
					searchThread.Abort();
					searchThread = null;
				}
			}

			if (string.IsNullOrWhiteSpace(searchText)) {
				LstUsers.ClearSelected();
			}

			searchThread = new Thread(new ParameterizedThreadStart(DoSearch));
			searchThread.Start(searchText);
		}

		private void DoSearch(object searchObj)
		{
			string searchText = (string)searchObj;
			int x = 0;
			for (x = 0; x <= LstUsers.Items.Count - 1; x++) {
				if (LstUsers.Items[x].ToString().ToUpper().Contains(searchText)) {
					LstUsersChangeSelectedIndex(x);
					break; // TODO: might not be correct. Was : Exit For
				}
			}
		}
		
		private void LstUsersChangeSelectedIndex(int idx)
		{
			object[] parms = new object[] { idx };
			if (this.InvokeRequired) {
				this.Invoke(new DoFromThreadDelegateWithInt(LstUsersChangeSelectedIndex), parms);
			} else {
				LstUsers.SelectedIndex = idx;
			}
		}

		private void cbAuthEnabled_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			if (cbAuthEnabled.Checked)
				lblAuthEnabled.Text = "Yes";
			else
				lblAuthEnabled.Text = "No";
		}

		[System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
		private static extern System.IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

		[System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		private static extern bool DeleteObject(System.IntPtr hObject);

		private void TxtSearch_TextChanged(System.Object sender, System.EventArgs e)
		{
			loadUsersThreaded();
		}

		private void cbLockedDown_CheckedChanged(object sender, EventArgs e)
		{
			if (cbLockedDown.Checked)
				lblLockedDownYesNo.Text = "Yes";
			else
				lblLockedDownYesNo.Text = "No";
		}

		void btnEmergencyToken_Click(object sender, EventArgs e)
		{
			try {
				var user = getSelectedUsername();
				var org = clientLogic.OrgNameP;
				var ret = clientLogic.SendToken(user, org, true);
				if (!string.IsNullOrEmpty(ret.Error))
					ShowErrorMessage(ret.Error);
				else
					ShowInfoMessage(string.Format(@"Emergency token generated: {0}", ret.Pin));
			} catch( NoUserSelectedException ex ) {
				ShowErrorMessage(ex.Message);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}

		private void ShowErrorMessage(string error)
		{
			MessageBox.Show(error, Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void ShowInfoMessage(string info)
		{
			MessageBox.Show(info, Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void cbOnlyOverriden_CheckedChanged(object sender, EventArgs e)
		{
			loadUsersThreaded();
		}

		private void tbPincode_KeyDown(object sender, KeyEventArgs e)
		{
			tbPincodeChanged = true;
		}
		
		void CbFilterProviderSelectedIndexChanged(object sender, EventArgs e)
		{
			loadUsersThreaded();
		}
		
		void CbFilterByUserTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			loadUsersThreaded();
		}
		void btnMaximize_Click(object sender, EventArgs e)
		{
			if ( WindowState == FormWindowState.Maximized )
				WindowState = FormWindowState.Normal;
			else
				WindowState = FormWindowState.Maximized;
		}
		
		void PnlResizeWindowMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
		    {
		        ReleaseCapture();
		        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_BOTTOMRIGHT, 0);
		    }
		}
		void BtnClearPinClick(object sender, EventArgs e) {
			try {
				var user = getSelectedUsername();
				var org = clientLogic.OrgNameP;
				var ret = clientLogic.ClearPin(user, org);
				if (!string.IsNullOrEmpty(ret.Error))
					ShowErrorMessage(ret.Error);
				else
					ShowInfoMessage(@"Pincode was cleared on the database.");
			} catch( NoUserSelectedException ex ) {
				ShowErrorMessage(ex.Message);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}
	}
}
