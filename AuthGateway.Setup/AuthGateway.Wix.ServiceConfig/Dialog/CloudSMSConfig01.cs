using System;
using System.Windows.Forms;
using AuthGateway.Shared;
using System.Collections.Generic;
using AuthGateway.Shared.Serializer;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class CloudSMSConfig01 : UserControl, IWizardScreen
	{
		protected SystemConfiguration sc;
		protected Wizard wizard;
		protected CloudSMSModuleConfig modConfig;

		public CloudSMSConfig01()
		{
			InitializeComponent();
		}

		public CloudSMSConfig01(Wizard wizard, SystemConfiguration sc)
			: this()
		{
			this.wizard = wizard;
			this.sc = sc;

			LoadValidModules();
			LoadSamples();

			LoadModConfig(sc.CloudSMSConfiguration.CloudSMSModules[0]);
		}

		private void CloudSMSConfig01_Load(object sender, EventArgs e)
		{

		}

		private void LoadValidModules()
		{
			this.cbModule.DataSource = CloudSMSModuleConfig.Modules;
		}

		private void LoadModConfig(CloudSMSModuleConfig modConfig)
		{
			this.dataGridView1.DataSource = null;
			var modConfigClone = Generic.Deserialize<CloudSMSModuleConfig>(Generic.Serialize<CloudSMSModuleConfig>(modConfig));

			this.cbModule.Text = modConfigClone.TypeName;
			this.dataGridView1.DataSource = new BindingSource(modConfigClone.ModuleParameters, null);
			this.dataGridView1.AllowUserToAddRows = true;

			for (var i = 0; i < this.dataGridView1.ColumnCount; i++ )
			{
				var col = this.dataGridView1.Columns[i];
				//if (i == 0)
				//  col.ReadOnly = true;
				if (i != 1)
					col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
				else
					col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			}

			this.modConfig = modConfigClone;
		}

		private void LoadSamples()
		{
			Dictionary<string, CloudSMSModuleConfig> dic = new Dictionary<string, CloudSMSModuleConfig>();

			CloudSMSModuleConfig modConfigToAdd;

			modConfigToAdd = new CloudSMSModuleConfig()
				{
					TypeName = CloudSMSModuleConfig.REGEXP,
					ModuleParameters = new ModuleParameters() 
					{ 
						//new ModuleParameter() { Name = "", Value = "" },
						new ModuleParameter() { Name = "Url", Value = @"http://www.txtlocal.com/sendsmspost.php" },
						new ModuleParameter() { Name = "Regex", Value = @"CreditsRemaining=([0-9\.]+)" },
						new ModuleParameter() { Name = "CreditsRemaining", Value = "1", Output = true},
						new ModuleParameter() { Name = "selectednums", Value = @"{destination}" },
						new ModuleParameter() { Name = "message", Value = @"{message}" },
						new ModuleParameter() { Name = "uname", Value = @"textlocal@username.com" },
						new ModuleParameter() { Name = "pword", Value = @"textlocalpassword", Encrypt = true },
						new ModuleParameter() { Name = "from", Value = @"SMS_Validation" },
						new ModuleParameter() { Name = "info", Value = @"1" },
					}
				};
			dic.Add(CloudSMSModuleConfig.REGEXP, modConfigToAdd);

			modConfigToAdd = new CloudSMSModuleConfig()
			{
				TypeName = CloudSMSModuleConfig.REGEXP,
				ModuleParameters = new ModuleParameters() 
					{ 
						//new ModuleParameter() { Name = "", Value = "" },
						new ModuleParameter() { Name = "Url", Value = @"http://www.textapp.net/webservice/httpservice.aspx" },
						new ModuleParameter() { Name = "Regex", Value = @"<Code>1</Code>" },
						new ModuleParameter() { Name = "CreditsRemaining", Value = "1", Output = true},
						new ModuleParameter() { Name = "method", Value = @"sendsms" },
						
						new ModuleParameter() { Name = "externalLogin", Value = @"CHANGE THIS CLIENT_ID" },
						new ModuleParameter() { Name = "password", Value = @"CHANGE THIS CLIENT_PASS", Encrypt = true },

						new ModuleParameter() { Name = "originator", Value = @"SMS2" },

						new ModuleParameter() { Name = "clientBillingReference", Value = @"1" },
						new ModuleParameter() { Name = "validity", Value = @"72" },
						new ModuleParameter() { Name = "characterSetID", Value = @"2" },
						new ModuleParameter() { Name = "replyMethodID", Value = @"1" },
						new ModuleParameter() { Name = "replyData", Value = @"" },
						new ModuleParameter() { Name = "statusNotificationUrl", Value = @"" },

						new ModuleParameter() { Name = "clientMessageReference", Value = @"{guid}" },
						new ModuleParameter() { Name = "destinations", Value = @"{destination}" },
						new ModuleParameter() { Name = "body", Value = @"{message}" },
					}
			};
			dic.Add("TextAnywhere.net (RegExp)", modConfigToAdd);

			modConfigToAdd = new CloudSMSModuleConfig()
				{
					TypeName = CloudSMSModuleConfig.TEXTLOCAL,
					ModuleParameters = new ModuleParameters() 
					{ 
					new ModuleParameter() { Name = "Username", Value = "TEXTLOCAL USERNAME" },
					new ModuleParameter() { Name = "Password", Value = "TEXTLOCAL PASSWORD", Encrypt = true },
					}
				};
			dic.Add(CloudSMSModuleConfig.TEXTLOCAL, modConfigToAdd);

			modConfigToAdd = new CloudSMSModuleConfig()
			{
				TypeName = CloudSMSModuleConfig.CMDIRECT,
				ModuleParameters = new ModuleParameters() 
					{ 
					new ModuleParameter() { Name = "ProductToken", Value = "PRODUCT TOKEN" },
					new ModuleParameter() { Name = "Sender", Value = "SENDER PHONE" },
					}
			};
			dic.Add(CloudSMSModuleConfig.CMDIRECT, modConfigToAdd);

			modConfigToAdd = new CloudSMSModuleConfig()
				{
					TypeName = CloudSMSModuleConfig.TWILIO,
					ModuleParameters = new ModuleParameters() 
					{ 
						new ModuleParameter() { Name = "AccountSid", Value = "TWILIO ACCOUNT SID" },
						new ModuleParameter() { Name = "AuthToken", Value = "TWILIO AUTH TOKEN" },
						new ModuleParameter() { Name = "From", Value = "TWILIO PHONE NUMBER" },
						new ModuleParameter() { Name = "voice", Value = "alice" },
						new ModuleParameter() { Name = "language", Value = "en-GB" },
					}
				};
			dic.Add("Twilio (Voice)", modConfigToAdd);

			modConfigToAdd = new CloudSMSModuleConfig()
				{
					TypeName = CloudSMSModuleConfig.TWILIO,
					ModuleParameters = new ModuleParameters() 
					{ 
						new ModuleParameter() { Name = "AccountSid", Value = "TWILIO ACCOUNT SID" },
						new ModuleParameter() { Name = "AuthToken", Value = "TWILIO AUTH TOKEN" },
						new ModuleParameter() { Name = "TwilioService", Value = "SMS" },
						new ModuleParameter() { Name = "From", Value = "+441158241527" },
					}
				};
			dic.Add("Twilio (SMS)", modConfigToAdd);

			modConfigToAdd = new CloudSMSModuleConfig()
			{
				TypeName = CloudSMSModuleConfig.REGEXP,
				ModuleParameters = new ModuleParameters() 
					{ 
						//new ModuleParameter() { Name = "", Value = "" },
						new ModuleParameter() { Name = "Url", Value = @"http://api.clickatell.com/http/sendmsg" },
						new ModuleParameter() { Name = "Regex", Value = @"ID: .+" },
						new ModuleParameter() { Name = "to", Value = @"{destination}" },
						new ModuleParameter() { Name = "text", Value = @"{message}" },
						new ModuleParameter() { Name = "api_id", Value = @"CONFIGURED IN CLICKATELL" },
						new ModuleParameter() { Name = "user", Value = @"CONFIGURED IN CLICKATELL" },
						new ModuleParameter() { Name = "password", Value = @"CONFIGURED IN CLICKATELL", Encrypt = true },
					}
			};
			dic.Add("Click A Tell (RegExp)", modConfigToAdd);

			modConfigToAdd = new CloudSMSModuleConfig()
			{
				TypeName = CloudSMSModuleConfig.REGEXP,
				ModuleParameters = new ModuleParameters() 
					{ 
						new ModuleParameter() { Name = "Url", Value = @"https://rest.nexmo.com/sms/json" },
						new ModuleParameter() { Name = "Regex", Value = @"""status"":""0""" },
						new ModuleParameter() { Name = "to", Value = @"{destination}" },
						new ModuleParameter() { Name = "text", Value = @"{message}" },
						new ModuleParameter() { Name = "api_key", Value = @"PROVIDED BY NEXMO" },
						new ModuleParameter() { Name = "api_secret", Value = @"PROVIDED BY NEXMO", Encrypt=true },
						new ModuleParameter() { Name = "from", Value = @"SMS2" },
					}
			};
			dic.Add("Nexmo (RegExp)", modConfigToAdd);

			modConfigToAdd = new CloudSMSModuleConfig()
			{
				TypeName = CloudSMSModuleConfig.REGEXP,
				ModuleParameters = new ModuleParameters() 
					{ 
						new ModuleParameter() { Name = "Url", Value = @"http://api.directsms.com.au/s3/http/send_message" },
						new ModuleParameter() { Name = "Regex", Value = @"id:" },
						new ModuleParameter() { Name = "username", Value = @"CHANGE THIS CLIENT_ID" },
						new ModuleParameter() { Name = "password", Value = @"CHANGE THIS CLIENT_PASS", Encrypt = true },
						new ModuleParameter() { Name = "message", Value = @"{message}" },
						new ModuleParameter() { Name = "senderid", Value = @"SMS2" },
						new ModuleParameter() { Name = "to", Value = @"{destination}" },
						new ModuleParameter() { Name = "type", Value = @"1-way" },
					}
			};
			dic.Add("DirectSMS - Australia (RegExp)", modConfigToAdd);
			
			
			modConfigToAdd = new CloudSMSModuleConfig()
			{
				TypeName = CloudSMSModuleConfig.REGEXP,
				ModuleParameters = new ModuleParameters() 
					{ 
						new ModuleParameter() { Name = "Url", Value = @"http://playsms.id/index.php?app=ws" },
						new ModuleParameter() { Name = "u", Value = @"CHANGE THIS" },
						new ModuleParameter() { Name = "h", Value = @"CHANGE THIS", Encrypt = true },
						new ModuleParameter() { Name = "op", Value = @"pv" },
						new ModuleParameter() { Name = "to", Value = @"{destination}" },
						new ModuleParameter() { Name = "msg", Value = @"{message}" },
						new ModuleParameter() { Name = "Regex", Value = @"(.*)OK" },
					}
			};
			dic.Add("PlaySMS (RegExp)", modConfigToAdd);
			
			modConfigToAdd = new CloudSMSModuleConfig()
			{
				TypeName = CloudSMSModuleConfig.GSM,
				ModuleParameters = new ModuleParameters() 
					{ 
						new ModuleParameter() {	Name = "input1", Value = "atz", },
						new ModuleParameter() { Name = "expected1", Value = "OK" },

						new ModuleParameter() { Name = "input2", Value = "at^curc=0" },
						new ModuleParameter() { Name = "expected2", Value = "OK" },

						new ModuleParameter() { Name = "input3", Value = "at+cfun=1" },
						new ModuleParameter() { Name = "expected3", Value = "OK" },

						new ModuleParameter() { Name = "input4", Value = "at+cops?" },
						new ModuleParameter() { Name = "expected4", Value = "OK" },

						new ModuleParameter() { Name = "input5", Value = "at+cmgf=1" },
						new ModuleParameter() { Name = "expected5", Value = "OK" },

						new ModuleParameter() { Name = "input6", Value = "at+cmgs=\"{destination}\"" },
						new ModuleParameter() { Name = "expected6", Value = ">" },
						new ModuleParameter() { Name = "delay6", Value = "200" },

						new ModuleParameter() { Name = "input7", Value = "{message}" },
						new ModuleParameter() { Name = "expected7", Value = @"\+CMG?S" },
						new ModuleParameter() { Name = "delay7", Value = "500" },
						new ModuleParameter() { Name = "receiveTries7", Value = "120" },
					}
			};
			dic.Add(CloudSMSModuleConfig.GSM, modConfigToAdd);

			modConfigToAdd = new CloudSMSModuleConfig()
			{
				TypeName = CloudSMSModuleConfig.GSM,
				ModuleParameters = new ModuleParameters() 
					{ 
						new ModuleParameter() {	Name = "input1", Value = "atz", },
						new ModuleParameter() { Name = "expected1", Value = "OK" },

						new ModuleParameter() { Name = "input2", Value = "at^curc=0" },
						new ModuleParameter() { Name = "expected2", Value = "OK" },

						new ModuleParameter() { Name = "input3", Value = "at+cfun=1" },
						new ModuleParameter() { Name = "expected3", Value = "OK" },

						new ModuleParameter() { Name = "input4", Value = "at+cops?" },
						new ModuleParameter() { Name = "expected4", Value = "OK" },

						new ModuleParameter() { Name = "input5", Value = "at+cmgf=1" },
						new ModuleParameter() { Name = "expected5", Value = "OK" },

						new ModuleParameter() { Name = "input6", Value = "at+csmp=17,167,0,16" },
						new ModuleParameter() { Name = "expected6", Value = "OK" },

						new ModuleParameter() { Name = "input7", Value = "at+cmgs=\"{destination}\"" },
						new ModuleParameter() { Name = "expected7", Value = ">" },
						new ModuleParameter() { Name = "delay7", Value = "200" },

						new ModuleParameter() { Name = "input8", Value = "{message}" },
						new ModuleParameter() { Name = "expected8", Value = @"\+CMG?S" },
						new ModuleParameter() { Name = "delay8", Value = "500" },
						new ModuleParameter() { Name = "receiveTries8", Value = "120" },
					}
			};
			dic.Add(CloudSMSModuleConfig.GSM + " with Flash", modConfigToAdd);

			this.cbSamples.DisplayMember = "Key";
			this.cbSamples.ValueMember = "Value";
			this.cbSamples.DataSource = new BindingSource(dic, null);
		}

		public bool Store()
		{
			try
			{
				this.modConfig.TypeName = this.cbModule.Text;
				sc.CloudSMSConfiguration.CloudSMSModules[0] = this.modConfig;
				return true;
			}
			catch (Exception ex)
			{
				var reasons = new List<string>();
				reasons.Add(ex.Message);
				if (reasons.Count == 0)
					return true;
				this.wizard.ShowErrors(reasons);
				return false;
			}
		}

		public bool SkipNext()
		{
			return false;
		}

		public bool SkipPrevious()
		{
			return false;
		}

		public IWizardScreen GetWizardScreen()
		{
			return this;
		}

		public UserControl GetControl()
		{
			return this;
		}

		private void btnLoadSample_Click(object sender, EventArgs e)
		{
			LoadModConfig((CloudSMSModuleConfig)cbSamples.SelectedValue);
		}
	}
}
