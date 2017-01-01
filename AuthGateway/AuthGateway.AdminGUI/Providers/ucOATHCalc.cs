using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using AuthGateway.AdminGUI.OATH;
using AuthGateway.Shared.Helper;
using AuthGateway.Shared.Tracking;
using Gma.QrCodeNet.Encoding.Windows.Forms;

namespace AuthGateway.AdminGUI
{

	public partial class ucOATHCalc : ProviderConfig
	{
		private QrCodeGraphicControl qrCodeGraphicControl1;
		
		public string OriginalDeviceName { get; private set; }        

		private ProviderConfigContainer pcContainer;

        public bool UseDefaults { get; set; }
		public ucOATHCalc(ProviderConfigContainer pcCont) : this()
		{
			this.Parent = pcCont.getControl();
			pcCont.getControl().Controls.Add(this);
			this.Dock = DockStyle.Fill;

			this.SetProviderConfigContainer(pcCont);
		}


		public ucOATHCalc()
		{
			// This call is required by the designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
			this.qrCodeGraphicControl1 = new Gma.QrCodeNet.Encoding.Windows.Forms.QrCodeGraphicControl();
			this.qrCodeGraphicControl1.Parent = panelQR;
			this.qrCodeGraphicControl1.Dock = DockStyle.Fill;
			this.qrCodeGraphicControl1.BackColor = System.Drawing.SystemColors.Control;
			this.qrCodeGraphicControl1.ErrorCorrectLevel = Gma.QrCodeNet.Encoding.ErrorCorrectionLevel.M;
			this.qrCodeGraphicControl1.Name = "qrCodeGraphicControl1";
			this.qrCodeGraphicControl1.QuietZoneModule = Gma.QrCodeNet.Encoding.Windows.Render.QuietZoneModules.Two;
			this.qrCodeGraphicControl1.Show();
		}

		public void SetProviderConfigContainer(ProviderConfigContainer pcCont)
		{
			this.pcContainer = pcCont;
			
			tbDevicename.Text = string.Empty;
			tbSharedSecret.Text = string.Empty;

			this.cbAuthenticator.Items.Clear();
			this.cbAuthenticator.Items.Add(AuthenticatorFactory.FeitianSerial);
			this.cbAuthenticator.Items.Add(AuthenticatorFactory.GoogleAuthenticator);
			this.cbAuthenticator.Items.Add(AuthenticatorFactory.FeitianToken);
			this.cbAuthenticator.Items.Add(AuthenticatorFactory.Other);
			this.cbAuthenticator.SelectedIndex = 0;

			var selectedText = (string)cbAuthenticator.SelectedItem;
			var authenticator = getAuthenticator();
			if (!string.IsNullOrEmpty(selectedText)) {
				qrCodeGraphicControl1.Text = authenticator.GetSecretFormattedForQR(tbSharedSecret.Text, this.pcContainer.getUser(), rbTOTP.Checked, getCounterOrWindow());
			}

			this.qrCodeGraphicControl1.Visible = authenticator.ShowQR();

            if (UseDefaults)
                btnGenerateToken.Visible = pcContainer.getClientLogic().IsAdmin && authenticator.ShowGenerate();
            else
                btnGenerateToken.Visible = authenticator.ShowGenerate();

			lblSharedSecret.Text = authenticator.SharedSecretLabel();
		}

		string getCounterOrWindow() {
			if (rbHOTP.Checked) {
				return "1";
			} 
			return Convert.ToInt32(nudTotpWindow.Value).ToString();
		}
		
		public string getName()
		{
			return "OATHCalc";
		}

		private string friendlyName = string.Empty;
		public string getFriendlyName()
		{
			if (string.IsNullOrEmpty(friendlyName))
				return this.getName();
			return friendlyName;
		}
		public void setFriendlyName(string name)
		{
			friendlyName = name;
		}

		public void loadConfig(string config)
		{
			if (string.IsNullOrEmpty(config))
				config = HotpTotpModel.Serialize(new HotpTotpModel());
			string[] cfg = config.Split(new char[] { ',' }, StringSplitOptions.None);

			if (cfg[0] == HotpTotpModel.HOTP) {
				rbHOTP.Checked = true;
			} else {
				rbTOTP.Checked = true;
				try {
					nudTotpWindow.Value = Convert.ToInt32(cfg[3]);
				} catch (Exception ex) {
					nudTotpWindow.Value = 30;                    
				}
			}            

			if (cfg.Length == 3)
				tbDevicename.Text = "Default";
			else {
				if (cfg[0] == HotpTotpModel.HOTP || cfg.Length < 5)
					tbDevicename.Text = cfg[3];
				else
					tbDevicename.Text = cfg[4];
			}
			OriginalDeviceName = tbDevicename.Text;
		}

		public string getConfig()
		{
			var deviceName = tbDevicename.Text.Trim().Replace(",", "_");
			string sharedSecret = string.Empty;
			if (!string.IsNullOrEmpty(tbSharedSecret.Text)) {
				IAuthenticator authenticator = getAuthenticator();
				sharedSecret = authenticator.GetEncodedSharedSecret(tbSharedSecret.Text, cbHexEncoded.Checked);
			}
			
			var model = new HotpTotpModel {
				Secret = sharedSecret,
				DeviceName = deviceName
			};
			if (rbHOTP.Checked) {
				model.Type = HotpTotpModel.HOTP;
				model.CounterSkew = (!string.IsNullOrEmpty(sharedSecret))
						? "1" 
					: string.Empty;
			} else {
				model.Type = HotpTotpModel.TOTP;
				model.CounterSkew = "0";
				model.Window = getCounterOrWindow();
			}
			
			return HotpTotpModel.Serialize(model);
		}

		private void rbHOTP_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			if (rbHOTP.Checked) {
				nudTotpWindow.Enabled = false;
			} else {
				nudTotpWindow.Enabled = true;
			}
		}

		private void tbSharedSecret_TextChanged(System.Object sender, System.EventArgs e)
		{
			IAuthenticator authenticator = getAuthenticator();
			qrCodeGraphicControl1.Text = authenticator.GetSecretFormattedForQR(tbSharedSecret.Text, this.pcContainer.getUser(), rbTOTP.Checked, getCounterOrWindow());
			ChangeQRControlsVisible(false);
		}

		public void ShowConfig()
		{
			ChangeQRControlsVisible(false);
			this.Show();
		}

		public void HideConfig()
		{
			ChangeQRControlsVisible(false);
			this.Hide();
		}

		private void btnResync_Click(System.Object sender, System.EventArgs e)
		{
			if (!this.pcContainer.isSelectedProvider(this.getName())) {
				this.pcContainer.ShowError("Please activate this provider by clicking Save Configuration before using this functionality");
				return;
			}
			if (OriginalDeviceName != tbDevicename.Text.Trim())
			{
				this.pcContainer.ShowError("Please save this provider by clicking Save Configuration before using this functionality");
				return;
			}

			frmResyncToken frm = new frmResyncToken(this.pcContainer, tbDevicename.Text.Trim(), rbTOTP.Checked);
			frm.ShowDialog();
		}

		public void BeforeSave()
		{
			IAuthenticator authenticator = getAuthenticator();
			ChangeQRControlsVisible(authenticator.ShowQR());

			this.qrCodeGraphicControl1.Text = authenticator.GetSecretFormattedForQR(tbSharedSecret.Text, this.pcContainer.getUser(), rbTOTP.Checked, getCounterOrWindow());
		}

		public void ChangeQRControlsVisible(bool visible)
		{
			this.qrCodeGraphicControl1.Visible = visible;
			this.btnCopyQR.Visible = visible;
		}

		public string PostSaveMessage()
		{
			IAuthenticator authenticator = getAuthenticator();
			if (authenticator.ShowQR()) {
				return "You can now sync your device using QR code above.";
			}
			else
			{
				if (authenticator.IsSerial())
				{
					return "Your config has been saved, you can start using your device.";
				}
				else
				{
					return "You can now sync your device.";
				}
			}
		}

		private void btnCopyQR_Click(System.Object sender, System.EventArgs e)
		{
			try {
				using (Bitmap b = new Bitmap(this.qrCodeGraphicControl1.Size.Width, this.qrCodeGraphicControl1.Size.Height)) {
					this.qrCodeGraphicControl1.DrawToBitmap(b, new Rectangle(new Point(0, 0), b.Size));
					Clipboard.SetImage(b);
				}
				//MessageBox.Show("QR Code copied to clipboard.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
			} catch (Exception ex) {
				MessageBox.Show("Could not copy QR Code to clipboard.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);                
			}
		}

		public void validateBeforeSave()
		{
			IAuthenticator authenticator = getAuthenticator();
			authenticator.Validate(tbSharedSecret.Text, cbHexEncoded.Checked);
		}

		private void cbAuthenticator_SelectedIndexChanged(object sender, EventArgs e)
		{
			IAuthenticator authenticator = getAuthenticator();
			tbSharedSecret.Text = string.Empty;
			cbHexEncoded.Enabled = authenticator.HexEncodedEnabled();
			cbHexEncoded.Checked = authenticator.HexEncodedChecked();
            if (UseDefaults)
                btnGenerateToken.Visible = pcContainer.getClientLogic().IsAdmin && authenticator.ShowGenerate();
            else
                btnGenerateToken.Visible = authenticator.ShowGenerate();
			lblSharedSecret.Text = authenticator.SharedSecretLabel();
			ChangeQRControlsVisible(false);
		}

		private IAuthenticator getAuthenticator()
		{
			string selectedText = (string)cbAuthenticator.SelectedItem;
			return AuthenticatorFactory.Build(selectedText);
		}

		private void btnGenerateToken_Click(object sender, EventArgs e)
		{
			IAuthenticator authenticator = getAuthenticator();
			tbSharedSecret.Text = authenticator.GetGeneratedToken();
		}
		void Label2Click(object sender, EventArgs e)
		{

        }

        private void ucOATHCalc_Load(object sender, EventArgs e)
        {
            if (UseDefaults && !pcContainer.getClientLogic().IsAdmin) {
                cbAuthenticator.Visible = false;                
                tbSharedSecret.Visible = false;                                                                              
                rbHOTP.Visible = false;
                rbTOTP.Visible = false;                
                nudTotpWindow.Visible = false;
                cbHexEncoded.Visible = false;
                
                label2.Visible = false;
                label3.Visible = false;
                Label5.Visible = false;
                Label8.Visible = false;                
                lblSharedSecret.Visible = false;               
            }
        }
        public string GetSelectedConfig()
        {
            string config = string.Empty;

            var configValues = getConfig().Split(',').ToList();
            configValues.Add(cbAuthenticator.SelectedItem.ToString());
            config = string.Join(",", configValues);

            return config;
        }

        public void LoadDefaultConfig(string defaultConfig)
        {            
            string[] defaultConfigParams = defaultConfig.Split(',');
            cbAuthenticator.SelectedIndex = cbAuthenticator.Items.IndexOf(defaultConfigParams.Last());
            string sharedSecret = defaultConfigParams[1];
            if (!string.IsNullOrEmpty(sharedSecret)) {
                IAuthenticator authenticator = getAuthenticator();
                sharedSecret = authenticator.DecodeSharedSecret(sharedSecret, cbHexEncoded.Checked);
            }
            tbSharedSecret.Text = sharedSecret;
        }
	}
}
