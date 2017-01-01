using AuthGateway.Shared;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Text.RegularExpressions;

using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;

namespace AuthGateway.AdminGUI
{    
	public partial class DlSettings : ProviderConfigContainer
	{
		private Variables clientLogic;
        private List<string> domainAliases = new List<string>();

		private UserProvidersRet providers;
        
        private string rightMutualAuthImageAddress;
        private string leftMutualAuthImageAddress;

		public UserProviders uProviderCmd;

		public UserProvider uProvider = new UserProvider();

        private List<string> preferences = new List<string>(){ "Most Preferred", "Preferred", "Default", "Not Preferred" };

        private List<GetAliveServersRet.PollingServer> pollingServers = new List<GetAliveServersRet.PollingServer>();

        private int heartbeatInterval; //  seconds
        private int waitingInterval; // seconds;

        private bool useOATHCalcDefaults;
		public DlSettings(Variables clientLogic, UserProviders userProvidersCmd, bool locked)
		{
			// This call is required by the designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.

			this.clientLogic = clientLogic;

			this.uProviderCmd = userProvidersCmd;
			this.providers = clientLogic.GetProviders(userProvidersCmd);           

			if (locked && !this.clientLogic.IsAdmin)
				disableControls();

            var userSettings = clientLogic.AeGetUserSettings();

            foreach (var setting in userSettings) {
                if (setting.Object == "OATHCALC") {
                    switch (setting.Name) {
                        case "OATHCalcUseDefaults":
                            useOATHCalcDefaults = SystemConfiguration.getBoolOrDef(setting.Value, false);
                            cbUseOATHDefaults.Checked = useOATHCalcDefaults;
                            break;
                    }
                }
            }

            var settings = clientLogic.AeGetSettings();
			
			if (clientLogic.IsAdmin) {
					try {
						var messages = clientLogic.getTemplateMessages();
						messages.ForEach(x => cbMsgLabel.Items.Add(x));
					} catch (RemoteException ex) {
						ShowWarning(ex.Message);
                        Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
					}
					string passcodeLenStr = clientLogic.LoadPassCodeLen().Length.ToString();
					int passcodeLen = 4;
					nudPasscodeLength.Value = passcodeLen;
					if (Int32.TryParse(passcodeLenStr, out passcodeLen)) {
						if (passcodeLen < nudPasscodeLength.Minimum) {
							passcodeLen = Convert.ToInt32(nudPasscodeLength.Minimum);
						} else if (passcodeLen > nudPasscodeLength.Maximum) {
							passcodeLen = Convert.ToInt32(nudPasscodeLength.Maximum);
						}
						nudPasscodeLength.Value = passcodeLen;
					}
								
					foreach(var item in Enum.GetValues(typeof(RKGBase))) {
						cbKeyBase.Items.Add(item);
					}
					
					foreach(var item in Enum.GetValues(typeof(PinCodeOption))) {
						cbPincode.Items.Add(item);
					}
					
                    foreach (var domain in clientLogic.GetDomains()) {
                        cbDomain.Items.Add(domain);
                    }                   
                    
					foreach(var setting in settings) {
						if (setting.Object == "AESETTING") {
							switch (setting.Name) {                                
								case "AuthEngineKeyBase":
									cbKeyBase.SelectedValue = SystemConfiguration.getEnumValue<RKGBase>(setting.Value);
									cbKeyBase.Text = setting.Value;
									break;
								case "AuthEnginePinCode":
									cbPincode.SelectedValue = SystemConfiguration.getEnumValue<PinCodeOption>(setting.Value);
									cbPincode.Text = setting.Value;
									break;
								case "AuthEnginePinCodeLength":
									nupPincodeLength.Value = SystemConfiguration.getInt32OrDef(setting.Value, 6);
									break;
                                case "AuthEnginePinCodePanic":
                                    bool panicModeOn = SystemConfiguration.getBoolOrDef(setting.Value, false);
                                    bool userInPanicState = clientLogic.GetPanicState(uProviderCmd.User, uProviderCmd.Org);
                                    cbPincodePanicMode.Checked = panicModeOn;
                                    btnResetPanic.Visible = panicModeOn || userInPanicState;
                                    btnResetPanic.Enabled = userInPanicState;
                                    break;
                                case "AuthEngineMutualAuth":
                                    cbMutualAuthMode.Checked = SystemConfiguration.getBoolOrDef(setting.Value, false);
                                    break;
                                case "OATHCalcUseDefaults":
                                    cbUseOATHDefaults.Checked = SystemConfiguration.getBoolOrDef(setting.Value, false);
                                    break;
								case "AuthEngineAskMissingInfo":
									cbAskMissing.Checked = SystemConfiguration.getBoolOrDef(setting.Value, false);
									break;
								case "AuthEngineAskPin":
									cbAskPin.Checked = SystemConfiguration.getBoolOrDef(setting.Value, false);
									break;
								case "AuthEngineAskProviderInfo":
									cbAskProviderInfo.Checked = SystemConfiguration.getBoolOrDef(setting.Value, false);
									break;
								case "FieldMissingErrorEmail":
									tbMissingEmail.Text = ConvertToMultilineText(setting.Value);
									break;
								case "FieldMissingErrorMobilePhone":
									tbMissingPhone.Text = ConvertToMultilineText(setting.Value);
									break;
                                case "AuthEngineAllowUPNLogin":
                                    cbUPNLogin.Checked = SystemConfiguration.getBoolOrDef(setting.Value, true);
                                    break;
                                case "AuthEngineAllowPre2000Login":
                                    cbPre2000Login.Checked = SystemConfiguration.getBoolOrDef(setting.Value, true);
                                    break;
                                case "AuthEngineAllowEmailLogin":
                                    cbEmailAddressLogin.Checked = SystemConfiguration.getBoolOrDef(setting.Value, false);
                                    break;
                                case "AuthEngineAllowMobileNumberLogin":
                                    cbMobileNumberLogin.Checked = SystemConfiguration.getBoolOrDef(setting.Value, false);
                                    break;
                                case "AuthEngineAllowAliasesInLogin":
                                    cbAllowAliasesInLogin.Checked = SystemConfiguration.getBoolOrDef(setting.Value, true);
                                    break;
                                case "AuthEngineSAMLoginPreferred":
                                    cbSAMLoginPreferred.Checked = SystemConfiguration.getBoolOrDef(setting.Value, true);
                                    break;
                                case "AuthEngineHeartbeatInterval":
                                    heartbeatInterval = SystemConfiguration.getInt32OrDef(setting.Value, (int)nudHeartbeatInterval.Minimum);
                                    nudHeartbeatInterval.Value = heartbeatInterval;
                                    break;
                                case "AuthEngineWaitingInterval":
                                    waitingInterval = SystemConfiguration.getInt32OrDef(setting.Value, (int)nudWaitingInterval.Minimum);
                                    nudWaitingInterval.Value = waitingInterval;
                                    break;
	#if DEBUG
								default:
	throw new NotImplementedException(string.Format("setting.Name '{0}' not expected in AESETTING", setting.Name));
	#endif
							}
						} else if ( setting.Object == "RADIUS" ) {
							switch(setting.Name) {
								case "PasswordVaulting":
									cbPasswordVaulting.Checked = SystemConfiguration.getBoolOrDef(setting.Value, false);
									break;
							}
						}
					}
					
					TokensRet tRet = clientLogic.GetTokens();
					lblTokens.Text = "Text Message Credits: " + tRet.Available.ToString();
					this.ttProvider.SetToolTip(lblTokens, "This count may not be accurate.");                    

                    UsersPollingPreferenceColumn.DataSource = preferences;
                    //ImagesPollingPreferenceColumn.DataSource = preferences;
                    
                    RefreshPollingServers();
                    
				} // IsAdmin
                else {
					this.tcOptions.Controls.Remove(this.tabMessage);
					this.tcOptions.Controls.Remove(this.tabEngineOptions);
					this.tcOptions.Controls.Remove(this.tabOtherFunctions);
                    this.tcOptions.Controls.Remove(this.tabAliases);
                    this.tcOptions.Controls.Remove(this.tabHANodes);
                    this.tcOptions.Controls.Remove(this.tabLoginOptions);                    

				}            

				bool providerEnabled = false;
				foreach (UserProvider p in providers.Providers) {
					try {
						ProviderConfig pc = GetProviderConfig(p.Name);
						pc.loadConfig(p.Config);
						pc.setFriendlyName(p.FName);
						providerEnabled = true;
						var rb = new RadioButton();
						rb.Text = pc.getFriendlyName();
						rb.Tag = pc;
						if (p.Selected) {
							rb.Checked = true;
							pc.ShowConfig();
						} else {
							rb.Checked = false;
							pc.HideConfig();
						}
						rb.AutoSize = true;
						rb.CheckedChanged += rb_CheckedChanged;
						flpRB.Controls.Add(rb);
					} catch (ProviderConfigNotFoundException ex) {
						#if DEBUG
						Console.WriteLine(ex.Message);
						#endif
                        Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
					}
				}
				btnSave.Enabled = providerEnabled;

                foreach (var setting in settings) {
                    if (setting.Name == "AuthEngineMutualAuth") {
                        if (!SystemConfiguration.getBoolOrDef(setting.Value, false)) {                            
                            //tcOptions.TabPages.Remove(tabMutualAuth);                            
                        }
                    }
                }

                GetUserAuthImagesRet ret = clientLogic.GetUserAuthImages(userProvidersCmd.User, userProvidersCmd.Org);

                if (ret.LeftImageBytes != null) {
                    Bitmap bitmap = ImagingHelper.GetImageFromBytes(ret.LeftImageBytes);                    
                    pbLeftImage.Image = ImagingHelper.ResizeImage(bitmap, pbLeftImage.Width, pbLeftImage.Height);
                }

                if (ret.RightImageBytes != null) {
                    Bitmap bitmap = ImagingHelper.GetImageFromBytes(ret.RightImageBytes);
                    pbRightImage.Image = ImagingHelper.ResizeImage(bitmap, pbRightImage.Width, pbRightImage.Height);
                }
		}

		private void disableControls()
		{
			foreach(TabPage tabPage in this.tcOptions.TabPages)
				foreach (Control ctl in tabPage.Controls) ctl.Enabled = false;
		}        

		static string ConvertToMultilineText(string text) {
			if (text == null)
				return text;
			return text
				.Replace("\r\n", "\n")
				.Replace("\n","\r\n");
		}

		private ProviderConfig GetProviderConfig(string name)
		{
			switch (name)
			{
				case "CloudSMS":
					return new ucCloudSMS(this);
				case "OATHCalc":
					return new ucOATH(getControl(), this, useOATHCalcDefaults);
				case "PINTAN":
					return new ucPINTAN(this);
				case "Email":
					return new ucEmail(this);
				case "NHS":
					return new ucNHS(this);
				case "Static":
					return new ucStatic(this);
				case "OneTime":
					return new ucOneTime(this);
				case "Passthrough":
					return new ucPassthrough(this);
			}
			throw new ProviderConfigNotFoundException();
		}

		void BtnClose_Click(Object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
		void BtnOkSMS_Click(Object sender, EventArgs e)
		{
			if ( string.IsNullOrWhiteSpace(cbMsgLabel.Text) || cbMsgLabel.SelectedIndex == -1)
				return;
			var update = clientLogic.updateMessage(cbMsgLabel.Text, tbTitle.Text, txtMessage.Text);
			if (string.IsNullOrEmpty(update.Error)) {
				var message = (TemplateMessage)cbMsgLabel.Items[cbMsgLabel.SelectedIndex];
				message.Title = tbTitle.Text;
				message.Message = txtMessage.Text;
				MessageBox.Show("Record Updated.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
			} else {
				MessageBox.Show("Update Failed.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		
		List<Setting> buildSettings() {
			var settings = new List<Setting>();
			settings.Add(new Setting {
			             	Name ="AuthEngineKeyBase",
							Value = cbKeyBase.Text,
							Object = "AESETTING"
			             });
			settings.Add(new Setting {
			             	Name ="AuthEnginePinCode",
							Value = cbPincode.Text,
							Object = "AESETTING"
			             });
			settings.Add(new Setting {
			             	Name ="AuthEnginePinCodeLength",
			             	Value = Convert.ToInt32(nupPincodeLength.Value).ToString(),
							Object = "AESETTING"
			             });
            settings.Add(new Setting
            {
                Name = "AuthEnginePinCodePanic",
                Value = cbPincodePanicMode.Checked ? "True" : "False",
                Object = "AESETTING"
            });
            settings.Add(new Setting
            {
                Name = "AuthEngineMutualAuth",
                Value = cbMutualAuthMode.Checked ? "True" : "False",
                Object = "AESETTING"
            });
            settings.Add(new Setting {
                Name = "OATHCalcUseDefaults",
                Value = cbUseOATHDefaults.Checked ? "True" : "False",
                Object = "OATHCALC"
            });
			settings.Add(new Setting {
			             	Name ="AuthEngineAskMissingInfo",
			             	Value = cbAskMissing.Checked ? "True" : "False",
							Object = "AESETTING"
			             });
			settings.Add(new Setting {
			             	Name ="AuthEngineAskPin",
			             	Value = cbAskPin.Checked ? "True" : "False",
							Object = "AESETTING"
			             });
			settings.Add(new Setting {
			             	Name ="AuthEngineAskProviderInfo",
			             	Value = cbAskProviderInfo.Checked ? "True" : "False",
							Object = "AESETTING"
			             });
			settings.Add(new Setting {
		             	Name ="FieldMissingErrorEmail",
						Value = tbMissingEmail.Text,
						Object = "AESETTING"
		             });
			settings.Add(new Setting {
		             	Name ="FieldMissingErrorMobilePhone",
						Value = tbMissingPhone.Text,
						Object = "AESETTING"
		             });
			
			settings.Add(new Setting {
			             	Name ="PasswordVaulting",
			             	Value = cbPasswordVaulting.Checked ? "True" : "False",
							Object = "RADIUS"
			             });

            settings.Add(new Setting
            {
                Name = "AuthEngineAskPin",
                Value = cbAskPin.Checked ? "True" : "False",
                Object = "AESETTING"
            });

            settings.Add(new Setting
            {
                Name = "AuthEngineAllowUPNLogin",
                Value = cbUPNLogin.Checked ? "True" : "False",
                Object = "AESETTING"
            });

            settings.Add(new Setting
            {
                Name = "AuthEngineAllowPre2000Login",
                Value = cbPre2000Login.Checked ? "True" : "False",
                Object = "AESETTING"
            });

            settings.Add(new Setting
            {
                Name = "AuthEngineAllowMobileNumberLogin",
                Value = cbMobileNumberLogin.Checked ? "True" : "False",
                Object = "AESETTING"
            });

            settings.Add(new Setting
            {
                Name = "AuthEngineAllowEmailLogin",
                Value = cbEmailAddressLogin.Checked ? "True" : "False",
                Object = "AESETTING"
            });

            settings.Add(new Setting
            {
                Name = "AuthEngineAllowAliasesInLogin",
                Value = cbAllowAliasesInLogin.Checked ? "True" : "False",
                Object = "AESETTING"
            });

            settings.Add(new Setting
            {
                Name = "AuthEngineSAMLoginPreferred",
                Value = cbSAMLoginPreferred.Checked ? "True" : "False",
                Object = "AESETTING"
            });

            settings.Add(new Setting {
                Name = "AuthEngineHeartbeatInterval",
                Value = nudHeartbeatInterval.Value.ToString(),
                Object = "AESETTING"
            });

            settings.Add(new Setting {
                Name = "AuthEngineWaitingInterval",
                Value = nudWaitingInterval.Value.ToString(),
                Object = "AESETTING"
            });

			return settings;
		}
		
		void BtnOKPassCode_Click(Object sender, EventArgs e)
		{
			var error = false;
			try {
				if ( ! string.IsNullOrEmpty(clientLogic.UpdatePassCodeLen(nudPasscodeLength.Value.ToString()).Error) )
					error = true;
				
				var ret = clientLogic.SaveSettings(buildSettings());
				if ( ! string.IsNullOrEmpty( ret.Error ) )
					error = true;
				if (error)
					throw new Exception();
				MessageBox.Show("Record Updated.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                /*if (cbMutualAuthMode.Checked && !tcOptions.TabPages.Contains(tabMutualAuth))
                    tcOptions.TabPages.Add(tabMutualAuth);
                if (!cbMutualAuthMode.Checked && tcOptions.TabPages.Contains(tabMutualAuth))
                    tcOptions.TabPages.Remove(tabMutualAuth);*/
			} catch(Exception ex) {
				MessageBox.Show("Update Failed.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}

		void rb_CheckedChanged(Object sender, EventArgs e)
		{
			RadioButton rb = (RadioButton)sender;
			ProviderConfig pc = (ProviderConfig)rb.Tag;
			if (rb.Checked) {
				pc.ShowConfig();
			} else {
				pc.HideConfig();
			}
		}

		public void SaveSelectedProvider(bool validate)
		{
			ProviderConfig pc = null;
			try {
				foreach (RadioButton rb in flpRB.Controls) {
					if (rb.Checked) {
						pc = (ProviderConfig)rb.Tag;
						if (validate)
							pc.validateBeforeSave();
						uProvider.Name = pc.getName();
						uProvider.Config = pc.getConfig();
					}
				}
				if (pc == null || string.IsNullOrEmpty(uProvider.Name)) {
					MessageBox.Show("Provider is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				pc.BeforeSave();
				SetUserProviderRet ret = clientLogic.SetProvider(uProviderCmd.User, uProviderCmd.Org, uProvider);
				if (!string.IsNullOrEmpty((ret.Error))) {
					MessageBox.Show(ret.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				} else if (ret.Out == 0) {
					MessageBox.Show("No setting was updated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				} else {
					string message = "Record Updated.";
					if (!string.IsNullOrEmpty(pc.PostSaveMessage())) {
						message = message + Environment.NewLine + pc.PostSaveMessage();
					}

					foreach (UserProvider up in providers.Providers) {
						up.Selected = (up.Name == uProvider.Name);
					}

					if (ret.Locked && !clientLogic.IsAdmin)
					{
						disableControls();
					}

					MessageBox.Show(message, Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			} catch (ArgumentException ex) {
				MessageBox.Show(ex.Message, Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}
		
		private void btnSave_Click(System.Object sender, System.EventArgs e)
		{
			SaveSelectedProvider(true);
		}

		private void btnPollUsers_Click(System.Object sender, System.EventArgs e)
		{
			MessageBox.Show("Polling users may take a while and will be done in background by AuthEngine.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
			string err = clientLogic.PollUsers().Error;
			if (!string.IsNullOrEmpty(err)) {
				MessageBox.Show(err, Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public string getUser()
		{
			return uProviderCmd.User;
		}

		public string getDomain()
		{
			return uProviderCmd.Org;
		}

		public System.Windows.Forms.Control getControl()
		{
			return this.gbConfig;
		}

		public Variables getClientLogic()
		{
			return this.clientLogic;
		}

		public List<UserProvider> getProviders()
		{
			if (this.providers == null) {
				return new List<UserProvider>();
			}
			return this.providers.Providers;
		}

		public bool isSelectedProvider(string name)
		{
			if (this.providers == null || this.providers.Providers == null) {
				return false;
			}
			foreach (UserProvider p in this.providers.Providers) {
				if (p.Selected && p.Name == name) {
					return true;
				}
			}
			return false;
		}

		public void ShowError(string message)
		{
			MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public void ShowWarning(string message)
		{
			MessageBox.Show(message, "WrightCCS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		
		public bool ShowConfirm(string message)
		{
			return MessageBox.Show(message, "WrightCCS", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
		}
		
		void CbMsgLabelSelectedIndexChanged(object sender, EventArgs e)
		{
			var message = (TemplateMessage)cbMsgLabel.Items[cbMsgLabel.SelectedIndex];
			tbTitle.Text = message.Title;
			txtMessage.Text = message.Message;
			lblLegend.Text = message.Legend;
		}

        private void btnSaveLoginOptions_Click(object sender, EventArgs e)
        {
            try
            {
                var ret = clientLogic.SaveSettings(buildSettings());
                if (!string.IsNullOrEmpty(ret.Error))
                    throw new Exception();
                MessageBox.Show("Record Updated.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update Failed.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        private void ShowDomainAliases(string domain)
        {            
            lbAliases.Items.Clear();
            domainAliases.Clear();
            
            if (!String.IsNullOrEmpty(domain))
            {
                foreach (var alias in clientLogic.GetAliases(domain))
                {                
                    lbAliases.Items.Add(alias);                  
                }

                domainAliases = lbAliases.Items.Cast<string>().ToList();
                
                btnAddAlias.Enabled = true;
            }
            else
            {
                btnAddAlias.Enabled = false;
            }

        }

        private void cbDomain_SelectedIndexChanging(object sender, EventArgs e)
        {
            List<string> currentAliases = lbAliases.Items.Cast<string>().ToList();
            var added = currentAliases.Except(domainAliases);
            var removed = domainAliases.Except(currentAliases);
            
            if (added.ToList().Count > 0 || removed.ToList().Count > 0)
            {
                DialogResult res = MessageBox.Show(string.Format("Save aliases for domain '{0}'?", cbDomain.Items[cbDomain.LastAcceptedSelectedIndex].ToString()), "Save Aliases", MessageBoxButtons.YesNoCancel);                
                switch (res)
                {
                    case DialogResult.Yes:
                        SaveAliases(cbDomain.Items[cbDomain.LastAcceptedSelectedIndex].ToString());
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        ((System.ComponentModel.CancelEventArgs)e).Cancel = true;
                        break;
                }
            }
        }

        private void cbDomain_SelectedIndexChanged(object sender, EventArgs e)
        {            
            ComboBox cb = (ComboBox)sender;
            string selectedDomain = cb.SelectedItem.ToString();                        
            ShowDomainAliases(selectedDomain);
            btnSaveAliases.Enabled = false;
            btnRemoveAlias.Enabled = false;
            btnEditAlias.Enabled = false;
        }

        private void btnAddAlias_Click(object sender, EventArgs e)
        {
            frmNewAlias newAliasDlg = new frmNewAlias(cbDomain.SelectedItem.ToString(), lbAliases.Items.Cast<String>().ToList());
            if (newAliasDlg.ShowDialog((sender as Button).Parent) == System.Windows.Forms.DialogResult.OK)
            {
                lbAliases.Items.Add(newAliasDlg.NewAlias);
                btnSaveAliases.Enabled = true;
            }
        }

        private void SaveAliases(string domain)
        {
            try
            {
                List<string> currentAliases = lbAliases.Items.Cast<String>().ToList();
                clientLogic.UpdateAliases(domain, currentAliases);
                btnSaveAliases.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update Failed.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        private void btnSaveAliases_Click(object sender, EventArgs e)
        {
            SaveAliases(cbDomain.SelectedItem.ToString());
        }

        private void btnRemoveAlias_Click(object sender, EventArgs e)
        {
            if (lbAliases.SelectedIndex != -1)
            {
                string alias = lbAliases.SelectedItem.ToString();
                if (MessageBox.Show(string.Format("Remove alias '{0}'?", alias), "Remove Alias", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    lbAliases.Items.Remove(lbAliases.SelectedItem);
                    btnSaveAliases.Enabled = true;
                }
            }
        }

        private void lbAliases_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEditAlias.Enabled = (sender as ListBox).SelectedIndex != -1;
            btnRemoveAlias.Enabled = (sender as ListBox).SelectedIndex != -1;
        }

        private void btnEditAlias_Click(object sender, EventArgs e)
        {
            if (lbAliases.SelectedIndex != -1)
            {
                frmEditAlias editAliasDlg = new frmEditAlias(cbDomain.SelectedItem.ToString(), lbAliases.SelectedItem.ToString(), lbAliases.Items.Cast<String>().ToList());
                if (editAliasDlg.ShowDialog((sender as Button).Parent) == System.Windows.Forms.DialogResult.OK)
                {
                    lbAliases.Items[lbAliases.SelectedIndex] = editAliasDlg.EditedAlias;
                    btnSaveAliases.Enabled = true;
                }
            }
        }

        private void btnResetPanic_Click(object sender, EventArgs e)
        {
            try
            {                
                clientLogic.ResetPanicState(uProviderCmd.User, uProviderCmd.Org);
                MessageBox.Show("Panic state is reset.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnResetPanic.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error resetting panic state.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        private void btnLeftImage_Click(object sender, EventArgs e)
        {   
            frmChooseImage selectImageDialog = new frmChooseImage(clientLogic);
            if (selectImageDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                leftMutualAuthImageAddress = selectImageDialog.SelectedImageUrl;                
                pbLeftImage.Image = ImagingHelper.ResizeImage(selectImageDialog.SelectedImage, pbLeftImage.Width, pbLeftImage.Height);
                if (pbRightImage.Image != Properties.Resources.ImageNotSet)
                    btnSaveImages.Enabled = true;
            }
        }

        private void btnRightImage_Click(object sender, EventArgs e)
        {
            frmChooseImage selectImageDialog = new frmChooseImage(clientLogic);
            if (selectImageDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                rightMutualAuthImageAddress = selectImageDialog.SelectedImageUrl;                
                pbRightImage.Image = ImagingHelper.ResizeImage(selectImageDialog.SelectedImage, pbRightImage.Width, pbRightImage.Height);
                if (pbLeftImage.Image != Properties.Resources.ImageNotSet)
                    btnSaveImages.Enabled = true;
            }
        }

        private void btnSaveImages_Click(object sender, EventArgs e)
        {            
            try {                
                var setImagesRet = clientLogic.SetUserAuthImages(uProviderCmd.User, uProviderCmd.Org, leftMutualAuthImageAddress, rightMutualAuthImageAddress);
                if (!string.IsNullOrEmpty(setImagesRet.Error)) 
                    throw new Exception();
                MessageBox.Show("Record Updated.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);                    
            }
            catch (Exception ex) {
                MessageBox.Show("Update Failed.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }         
        }        

        private bool HANodesTabSettingsChanged()
        {
            return PollingServersPreferencesChanged() || HeartbeatSettingChanged() || WaitingSettingChanged();
        }

        private void RefreshPollingServers()
        {
            try {
                GetAliveServersRet serversRet = clientLogic.GetAliveServers();
                pollingServers = serversRet.Servers;

                dgvServers.Rows.Clear();
                for (int i = 0; i < pollingServers.Count; i++) {
                    dgvServers.Rows.Add();
                    dgvServers.Rows[i].Cells[HostnameColumn.Index].Value = pollingServers[i].Hostname;
                    dgvServers.Rows[i].Cells[MacAddressColumn.Index].Value = pollingServers[i].MACAddress;
                    dgvServers.Rows[i].Cells[UsersPollingPreferenceColumn.Index].Value = preferences[(int)pollingServers[i].UsersPollingPreference];
                    //dgvServers.Rows[i].Cells[ImagesPollingPreferenceColumn.Index].Value = preferences[(int)pollingServers[i].ImagesPollingPreference];
                }
                
            }
            catch (Exception ex){
                MessageBox.Show("Error updating polling servers.");
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
        }        

        private void btnRefreshPollingServers_Click(object sender, EventArgs e)
        {
            RefreshPollingServers();
            btnSavePollingConfig.Enabled = HANodesTabSettingsChanged();
        }

        private void btnSavePollingConfig_Click(object sender, EventArgs e)
        {
            try {
                string message = "Settings saved.";
                if (HeartbeatSettingChanged())
                    message += " Heartbeat interval change will take effect after AuthEngine restart.";
                if (PollingServersPreferencesChanged())
                    message += string.Format(" Preferences update will take effect within {0} seconds.", heartbeatInterval);

                if (HeartbeatSettingChanged() || WaitingSettingChanged()) {                    
                    var settingsRet = clientLogic.SaveSettings(buildSettings());
                    heartbeatInterval = (int)nudHeartbeatInterval.Value;
                    waitingInterval = (int)nudWaitingInterval.Value;
                    if (!string.IsNullOrEmpty(settingsRet.Error))
                        throw new Exception();
                }

                if (PollingServersPreferencesChanged()) {
                    for (int i = 0; i < pollingServers.Count; i++) {
                        if (pollingServers[i].UsersPollingPreference != (PollingPreference)preferences.IndexOf(dgvServers.Rows[i].Cells[UsersPollingPreferenceColumn.Index].Value.ToString())
                           /* || pollingServers[i].ImagesPollingPreference != (PollingPreference)preferences.IndexOf(dgvServers.Rows[i].Cells[ImagesPollingPreferenceColumn.Index].Value.ToString())*/) {                        
                            string hostname = dgvServers.Rows[i].Cells[HostnameColumn.Index].Value.ToString();
                            string macAddress = dgvServers.Rows[i].Cells[MacAddressColumn.Index].Value.ToString();
                            PollingPreference usersPollingPreference = (PollingPreference)preferences.IndexOf(dgvServers.Rows[i].Cells[UsersPollingPreferenceColumn.Index].Value.ToString());
                           // PollingPreference imagesPollingPreference = (PollingPreference)preferences.IndexOf(dgvServers.Rows[i].Cells[ImagesPollingPreferenceColumn.Index].Value.ToString());
                            SetServerPreferencesRet ret = clientLogic.SetServerPreferences(hostname, macAddress, usersPollingPreference/*, imagesPollingPreference*/);
                            if (!string.IsNullOrEmpty(ret.Error))
                                throw new Exception();
                        }
                    }
                }                

                MessageBox.Show(message, Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSavePollingConfig.Enabled = false;
            }
            catch (Exception ex) {
                MessageBox.Show("Error saving settings.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            } 
        }

        private bool HeartbeatSettingChanged()
        {
            return heartbeatInterval != nudHeartbeatInterval.Value;
        }

        private bool WaitingSettingChanged()
        {
            return waitingInterval != nudWaitingInterval.Value;
        }               

        private bool PollingServersPreferencesChanged()
        {
            bool changed = false;
            for (int i = 0; i < pollingServers.Count; i++) {
                if (pollingServers[i].UsersPollingPreference != (PollingPreference)preferences.IndexOf(dgvServers.Rows[i].Cells[UsersPollingPreferenceColumn.Index].Value.ToString())
                    /*|| pollingServers[i].ImagesPollingPreference != (PollingPreference)preferences.IndexOf(dgvServers.Rows[i].Cells[ImagesPollingPreferenceColumn.Index].Value.ToString())*/) {
                    changed = true;
                    break;
                }
            }

            return changed;
        }        

        private void dgvServers_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == UsersPollingPreferenceColumn.Index /*|| e.ColumnIndex == ImagesPollingPreferenceColumn.Index*/) && e.RowIndex >= 0) {
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dgvServers.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.Value != null) {
                    btnSavePollingConfig.Enabled = HANodesTabSettingsChanged();
                }
            }      
        }

        private void nudHeartbeatInterval_ValueChanged(object sender, EventArgs e)
        {
            btnSavePollingConfig.Enabled = HANodesTabSettingsChanged();
        }

        private void nudWaitingInterval_ValueChanged(object sender, EventArgs e)
        {
            btnSavePollingConfig.Enabled = HANodesTabSettingsChanged();
        }

	}
}
