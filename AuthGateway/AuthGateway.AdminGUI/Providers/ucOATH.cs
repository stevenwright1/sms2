using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using AuthGateway.Shared.Helper;
using AuthGateway.Shared.XmlMessages;

namespace AuthGateway.AdminGUI
{
	public partial class ucOATH : UserControl, ProviderConfig
	{
		private ProviderConfigContainer pcContainer;
		private Dictionary<string, string> configs;

        private bool useDefaults;

        public ucOATH()
        {
            InitializeComponent();
            this.configs = new Dictionary<string, string>();
        }

		public ucOATH(ProviderConfigContainer pcCont)
			: this()
		{
			this.pcContainer = pcCont;                        
		}
		
		public ucOATH(Control parent, ProviderConfigContainer pcCont, bool useDefaults)
			: this(pcCont)
		{
			this.Parent = parent;
			parent.Controls.Add(this);
			this.Dock = DockStyle.Fill;
            this.useDefaults = useDefaults;
            this.ucOATHCalc1.UseDefaults = useDefaults;
		}

        private void ClearConfig()
        {
            configs.Clear();
            cbDevice.Items.Clear();
        }

		public void loadConfig(string config)
		{
			if (string.IsNullOrEmpty(config))
				config = HotpTotpModel.Serialize(new HotpTotpModel());
			var cfgs = config.Split(new string[] { "|%|" }, StringSplitOptions.None);
			foreach (var cfg in cfgs)
			{
				var model = HotpTotpModel.Unserialize(cfg);				
				configs.Add(model.DeviceName, HotpTotpModel.Serialize(model));
				this.cbDevice.Items.Add(model.DeviceName);
			}
			this.cbDevice.SelectedIndex = this.cbDevice.Items.Count - 1;
		}

		public string getConfig()
		{
			string selectedDevice = string.Empty;
			if (!string.IsNullOrWhiteSpace(this.ucOATHCalc1.OriginalDeviceName))
			{
				var cfg = this.ucOATHCalc1.getConfig();                
				
				var model = HotpTotpModel.Unserialize(cfg);
				var deviceName = model.DeviceName;
				
				if (this.ucOATHCalc1.OriginalDeviceName != deviceName)
				{
					this.configs.Remove(this.ucOATHCalc1.OriginalDeviceName);
					this.configs.Add(deviceName, cfg);
				} else {
					var originalCfg = this.configs[deviceName];
					var originalModel = HotpTotpModel.Unserialize(originalCfg);
					
					originalModel.Type = model.Type;
					if (!string.IsNullOrWhiteSpace(model.Secret) && string.IsNullOrWhiteSpace(originalModel.Secret)) {
						originalModel.Secret = model.Secret;
						originalModel.Window = model.Window;
					} else if (!string.IsNullOrWhiteSpace(model.Secret) && (originalModel.Secret != model.Secret)) {
						originalModel.Secret = model.Secret;
						originalModel.Window = model.Window;
						originalModel.CounterSkew = model.CounterSkew;
					}

					configs[deviceName] = HotpTotpModel.Serialize(originalModel);
				}

				selectedDevice = deviceName;
			}
			this.cbDevice.Items.Clear();
			this.cbDevice.SelectedIndexChanged -= this.cbDevice_SelectedIndexChanged;

			foreach (var c in this.configs.Keys)
				this.cbDevice.Items.Add(c);
			if (!string.IsNullOrWhiteSpace(selectedDevice))
				this.cbDevice.SelectedIndex = this.cbDevice.Items.IndexOf(selectedDevice);

			this.cbDevice.SelectedIndexChanged += this.cbDevice_SelectedIndexChanged;

			return string.Join("|%|", configs.Values.ToArray<string>());
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

		public void ShowConfig()
		{
			this.Show();
		}

		public void HideConfig()
		{
			this.Hide();
		}

		public void BeforeSave()
		{
			this.ucOATHCalc1.BeforeSave();
		}

		public string PostSaveMessage()
		{
			if (this.DoNotDoPostMessage)
			{
				this.DoNotDoPostMessage = false;
				this.ucOATHCalc1.ChangeQRControlsVisible(false);
				return string.Empty;
			} else
				return this.ucOATHCalc1.PostSaveMessage();
		}

		public void validateBeforeSave()
		{
			this.ucOATHCalc1.validateBeforeSave();
		}

		private void cbDevice_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.ucOATHCalc1.SetProviderConfigContainer(this.pcContainer);
			var selectedDevice = string.Empty;
			if (this.cbDevice.SelectedItem != null)
				selectedDevice = Convert.ToString(this.cbDevice.SelectedItem).Trim();

			if (this.configs.ContainsKey(selectedDevice))
			{
				this.ucOATHCalc1.loadConfig(this.configs[selectedDevice]);
			}
			this.ucOATHCalc1.ShowConfig();
		}

		protected void btnAdd_Click(object sender, EventArgs e)
		{
			var newName = "Default " + (this.configs.Count + 1) + (this.configs.Count + 1);
			var model = new HotpTotpModel { DeviceName = newName };

            if (useDefaults && !pcContainer.getClientLogic().IsAdmin) {
                var defaultModel = HotpTotpModel.Unserialize(GetDefaultConfig());
                defaultModel.DeviceName = newName;
                model = defaultModel;                
            }
             
			addConfigDevice(newName, HotpTotpModel.Serialize(model));

            if (useDefaults && !pcContainer.getClientLogic().IsAdmin) {
                ucOATHCalc1.LoadDefaultConfig(HotpTotpModel.Serialize(model));            
            }
		}
		
		protected void addConfigDevice(string name, string config) {
			this.configs.Add(name, config);
			this.cbDevice.Items.Add(name);
			this.cbDevice.SelectedIndex = this.cbDevice.Items.Count - 1;
		}
		
		protected void btnDel_Click(object sender, EventArgs e)
		{
			if (this.cbDevice.Items.Count == 1) {
				this.pcContainer.ShowError("You cannot delete all devices.");
				return;
			}
			if (!this.pcContainer.ShowConfirm("Deleting the device will save the configuration, do you want to continue?"))
				return;
			var selectedDevice = string.Empty;
			if (this.cbDevice.SelectedItem != null)
				selectedDevice = Convert.ToString(this.cbDevice.SelectedItem).Trim();

			if (this.configs.ContainsKey(selectedDevice))
			{
				this.DoNotDoPostMessage = true;
				this.configs.Remove(selectedDevice);
				this.cbDevice.Items.RemoveAt(this.cbDevice.SelectedIndex);
				this.cbDevice.SelectedIndex = this.cbDevice.Items.Count - 1;
				this.pcContainer.SaveSelectedProvider(false);
			}
		}
		
		protected void setSharedSecret(string text)
		{
			this.ucOATHCalc1.tbSharedSecret.Text = text;
		}
		protected string getSharedSecret()
		{
			return this.ucOATHCalc1.tbSharedSecret.Text;
		}

		public bool DoNotDoPostMessage { get; set; }

        private void ucOATH_Load(object sender, EventArgs e)
        {
            if (useDefaults) {
                if (!pcContainer.getClientLogic().IsAdmin) {
                    btnSaveDefaults.Visible = false;
                }
                else {
                    btnResyncDefaults.Visible = false;
                }
            }
            else {
                btnSaveDefaults.Visible = false;
                btnResyncDefaults.Visible = false;
            }
        }

        private string GetDefaultConfig()
        {
            var settings = pcContainer.getClientLogic().AeGetUserSettings();

            string defaultConfig = string.Empty;

            foreach (var setting in settings) {
                if (setting.Object == "OATHCALC") {
                    switch (setting.Name) {
                        case "OATHCalcDefaultConfig":
                            defaultConfig = setting.Value;
                            break;
                    }
                }
            }

            return defaultConfig;
        }

        private void btnResyncDefaults_Click(object sender, EventArgs e)
        {
            try {
                string defaultConfig = GetDefaultConfig();
                
                if (string.IsNullOrEmpty(defaultConfig)) {
                    MessageBox.Show("Cannot resync defaults: defaults are not set.");
                }
                else {
                    ClearConfig();
                    LoadDefaultConfig(defaultConfig);                    
                }
            }
            catch (Exception ex) {                
                MessageBox.Show("Error resyncing.");
                throw ex;
            }
        }

        private void LoadDefaultConfig(string defaultConfig)
        {
            loadConfig(defaultConfig);
            ucOATHCalc1.LoadDefaultConfig(defaultConfig);
        }

        private void btnSaveDefaults_Click(object sender, EventArgs e)
        {
            bool settingsValid = false;

            try {
                validateBeforeSave();
                settingsValid = true;
            }
            catch (Exception ex) {
                MessageBox.Show("Cannot save invalid settings as default: " + ex.Message);
            }            

            if (settingsValid) {

                string defaultConfig = ucOATHCalc1.GetSelectedConfig();

                var settings = new List<Setting>();

                settings.Add(new Setting {
                    Name = "OATHCalcDefaultConfig",
                    Value = defaultConfig,
                    Object = "OATHCALC"
                });

                try {
                    var ret = pcContainer.getClientLogic().SaveSettings(settings);
                    if (!string.IsNullOrEmpty(ret.Error)) {
                        throw new Exception("Error saving defaults.");
                    }

                    var applyRet = pcContainer.getClientLogic().ApplyOATHCalcDefaults(defaultConfig);
                    if (!string.IsNullOrEmpty(applyRet.Error)) {
                        throw new Exception("Error applying defaults.");
                    }

                    MessageBox.Show("Defaults updated and applied.");
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
                
            }
        }
	}
}
