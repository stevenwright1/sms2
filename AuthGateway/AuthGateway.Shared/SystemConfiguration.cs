using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AuthGateway.Shared.Serializer;

namespace AuthGateway.Shared
{
	public class SystemConfigurationParseError : Exception
	{
		protected string message = string.Empty;
		public SystemConfigurationParseError()
			: base()
		{

		}
		public SystemConfigurationParseError(string message)
			: base(message)
		{

		}
		public SystemConfigurationParseError(string message, Exception inner)
		{
			var innerException = inner;
			while (innerException.InnerException != null)
				innerException = innerException.InnerException;
			this.message = string.Format("{0} - {1}", message, innerException.Message);
		}
		public override string Message
		{
			get
			{
				return this.message;
			}
		}
	}

	public class SystemConfiguration
	{
		public bool OnOffFound { get; private set; }

		public bool ErrorParsingConfiguration { get; private set; }

		private String _startUpPath = String.Empty;

		private IPAddress _smsServerIp = IPAddress.Parse("127.0.0.1");

		private IPAddress _authEngineServerIp = IPAddress.Parse("127.0.0.1");

		private const string ADCONTAINER = "ADContainer";
		private const string ADBASEDN = "ADBaseDN";
		private const string ADFILTER = "ADFilter";
		const string AD_DISASTER_PERCENTAGE = "ADDisasterPercentage";

		const string AD_ADMIN_BASEDN_OVERRIDE = "ADAdminBaseDNOverride";
		const string AD_ADMIN_FILTER_OVERRIDE = "ADAdminFilterOverride";
		

		private const string AUTHENGINE_USERSPOLLINTERVAL = "AuthEngineUsersPollInterval";
        private const string AUTHENGINE_HEARTBEATINTERVAL = "AuthEngineHeartbeatInterval";
        private const string AUTHENGINE_WAITINGINTERVAL = "AuthEngineWaitingInterval";
        private const string AUTHENGINE_POLLINGPREFERENCE = "AuthEnginePollingPreference";
        private const string MUTUALAUTHIMAGES_POLLINGPREFERENCE = "MutualAuthImagesPollingPreference";
		
		const string AUTHENGINE_ASKMISSINGINFO = "AuthEngineAskMissingInfo";
		const string AUTHENGINE_ASKPIN = "AuthEngineAskPin";
		const string AUTHENGINE_ASKPROVIDERINFO = "AuthEngineAskProviderInfo";
		const string AUTHENGINE_ASKAGEDTIMEHOURS = "AuthEngineAskAgedTimeHours";
		
		const string AUTHENGINE_AUTOSETUP_AGED_TIMEHOURS = "AuthEngineAutoSetupAgedTimeHours";
		
		private const string AUTHENGINE_DEFAULT_ENABLED = "AuthEngineDefaultEnabled";
		private const string AUTHENGINE_PINCODE = "AuthEnginePinCode";
		private const string AUTHENGINE_PINCODE_LENGTH = "AuthEnginePinCodeLength";
		private const string AUTHENGINE_DEFAULT_EXCEPTION_GROUPS = "AuthEngineDefaultExceptionGroups";
		private const string AUTHENGINE_EXTRADCS = "ExtraDCs";
		private const string AUTHENGINE_MANUALDOMAINREPLACEMENTS = "ManualDomainReplacements";
		private const string AUTHENGINE_REMOVEUSERS_AFTER_X_HOURS = "AuthEngineRemoveUsersAfterXHours";
		private const string AUTHENGINE_DEFAULT_DOMAIN = "AuthEngineDefaultDomain";
		private const string AUTHENGINE_ALLOWEMERGENCYPASSCODE = "AuthEngineAllowEmergencyPasscode";
        private const string AUTHENGINE_OVERRIDEWITHADINFO = "AuthEngineOverrideWithADInfo";
		private const string AUTHENGINE_ALLOWUPNLOGIN = "AuthEngineAllowUPNLogin";
        private const string AUTHENGINE_ALLOWPRE2000LOGIN = "AuthEngineAllowPre2000Login";
        private const string AUTHENGINE_ALLOWEMAILLOGIN = "AuthEngineAllowEmailLogin";
        private const string AUTHENGINE_ALLOWMOBILENUMBERLOGIN = "AuthEngineAllowMobileNumberLogin";
        private const string AUTHENGINE_ALLOWALIASESINLOGIN = "AuthEngineAllowAliasesInLogin";
        private const string AUTHENGINE_SAMLOGINPREFERRED = "AuthEngineSAMLoginPreferred";
		
		private const string NOTIFYPINCODEINCORRECTONACCESS = "NotifyPinCodeIncorrectOnAccess";

		private const string AUTHENGINE_LOCKDOWN_MODE = "AuthEngineLockDownMode";
		
		const string AUTHENGINE_VAULT_PASSWORD = "AuthEngineVaultPassword";
		const string AUTHENGINE_PINCODE_PANIC = "AuthEnginePincodePanic";

        const string AUTHENGINE_ENCRYPTION_KEY = "AuthEngineEncryptionKey";

		private const string LOGMAXFILES = "LogMaxFiles";
		private const string LOGMAXSIZE = "LogMaxSize";

		private const string AUTHENGINE_LOG = "AuthEngineLog";
		private const string AUTHENGINE_FILELOG = "AuthEngineFileLog";
		private const string AUTHENGINE_LOGFLUSHONWRITE = "AuthEngineLogFlushOnWrite";
		private const string AUTHENGINE_EVENTLOG = "AuthEngineEventLog";
		private const string CLOUDSMS_LOG = "CloudSMSLog";
		private const string CLOUDSMS_FILELOG = "CloudSMSFileLog";
		private const string CLOUDSMS_EVENTLOG = "CloudSMSEventLog";

		private const string CLOUDSMS_LOGFLUSHONWRITE = "CloudSMSLogFlushOnWrite";

		private const string AUTHENGINE_KEYBASE = "AuthEngineKeyBase";

		private const string AUTHENGINE_USEENCRYPTION = "AuthEngineUseEncryption";

		private const string AUTHENGINE_PRIVATE_RSAKEY = "AuthEnginePrivateRSAKey";
		private const string AUTHENGINE_PUBLIC_RSAKEY = "AuthEnginePublicRSAKey";
		private string _authEnginePrivateRsaKey = string.Empty;
		private string _authEnginePublicRsaKey = @"<RSAKeyValue><Modulus>0704aTJ5fG5jYRPui9ml7OIx6s2cE6QkQTaXZsDd9/BplBwVMxEUjw2HIl1D7rYdbXpSlWcSKsSWcTOsOtD3QdmD3cHAFW36pKU7q7HV8QDf2y7Sys4ATp9O4v/mTyaJ1O36xdwW/+VHAal82QDBXdDdRMKDqgKfTwcciQCZpU4mnjX4Ejme8tXys5jcWuyl4eDjebSlCWtIZktJKLwfmEv0nsntJggXObthmHnr/rW8Se2p7D7qEUyAnL+eM6DlNtx0gMeczILRx3qb4HrVzGgjYcj3RKhisG9aSIijCN8UoATsxsKhXIz0nUPjYV6nI4jUVzmctgun1RqsBnpoOw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

		private const string CLOUDSMS_USEENCRYPTION = "CloudSMSUseEncryption";

		private const string CLOUDSMS_PRIVATE_RSAKEY = "CloudSMSPrivateRSAKey";
		private const string CLOUDSMS_PUBLIC_RSAKEY = "CloudSMSPublicRSAKey";
		private string _cloudSMSPrivateRsaKey = string.Empty;
		private string _cloudSMSPublicRsaKey = @"<RSAKeyValue><Modulus>6aAfvgvCoJHA/nopdwSoenUg8Bsi15ZLny2iLaTGlhvlZp/O9GDJYEWKsxSM8EjMScncjWQ/sJKBXr0MJdWJHAGydpxI90hj2wSu2lG1/vvxBlXj5l7nrPC0Wl3KHU0379wzZ3rznYZU9MuYNIwhkwjZtJ5DerEjmhSQGngQpFJggFW+hLI2yZDHs40T/fHpWxchgjCPMbx+ZeHu+XGGeqgzrFU61qXkKRdAzRVbwGaC/padyHUw15R+qgIpWASGzYAEzzfCQiSlwk30exDp5r+HTAlNSj8truCy+HsLnXx/iBFVUpY4ukA2mS34Ccjn3e6bVYLtjm4+nUyZHpzffw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

		private const string AUTHPROVIDERS = "AuthProviders";

		private const string OATHCALC_SERVERIP = "OATHCalcServer";
		private const string OATHCALC_SERVERPORT = "OATHCalcPort";
		private const string OATHCALC_USEENCRYPTION = "OATHCalcUseEncryption";
		private const string OATHCALC_PRIVATE_RSAKEY = "OATHCalcPrivateRSAKey";
		private const string OATHCALC_PUBLIC_RSAKEY = "OATHCalcPublicRSAKey";

		private const string OATHCALC_TOTP_WINDOW = "OATHCalcTotpWindow";
		private const string OATHCALC_HOTP_AFTERWINDOW = "OATHCalcHotpAfterWindow";

        private const string OATHCALC_DEFAULTCONFIG = "OATHCalcDefaultConfig";

		private IPAddress _oathCalcServerIp = IPAddress.Parse("127.0.0.1");
		private string _oathCalcPrivateRsaKey = string.Empty;
		private string _oathCalcPublicRsaKey = @"<RSAKeyValue><Modulus>2Z7mqSaJQCzdQwIyEDvYetGaZjC9+mRVyL/cSy2iD9RyL0QHVjuilY8goDNc9Vbqxg/w0dvkq2B3XpsXsd47ma+REN/nMNI+owmPs+RgP6bBy8aCOdViEHyUs3G2Jowz5zOmeBavnE6Hel9ANUyjdVlRlaE9B9s27xXXR+/YPOuAccD3/J0Vte9tjfQ6aOvw1VLs3REMeLI/eOk2XZswynszNq65UlzTQ7WGoOuhskuj4QT+B1pjO5qvf3bjGo6FbsaUenmRMqVBtqwv6Sy7moX1XipNveO9n9SpZy5F5B1jP0PAnxA4x/igZDB1zTdbPygwXR/3M76kc+xLGaa1xQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

		private const string PINTAN_MAX_SHEETS = "PinTanMaximumSheets";
		private const string PINTAN_COLS = "PinTanColumns";
		private const string PINTAN_ROWS = "PinTanRows";

		private const string MINTIMEBETWEENRADIUSREQUESTSPERUSER = "MinTimeBetweenRadiusRequestsPerUser";
		private const string AUTHENGINE_CHALLENGERESPONSE = "AuthEngineChallengeResponse";

		private const string CLOUDSMS_TOKEN_EXPIRE_TIME_MINUTES = "CloudSMSTokenExpireTimeMinutes";
		const string EMERGENCY_TOKEN_EXPIRE_TIME_MINUTES = "EmergencyTokenExpireTimeMinutes";
		const string ONETIME_TOKEN_EXPIRE_TIME_MINUTES = "OneTimeTokenExpireTimeMinutes";

		private const string EMAILCONFIG = "EmailConfig";

		private const string CLOUDSMS_MODULECONFIG = "CloudSMSModuleConfig";

		private const string RADIUS_CHALLENGE_MESSAGE = "RadiusChallengeMessage";

		private const string RADIUS_SHOW_PIN = "RadiusShowPin";
		private const string ADMINGUI_CONFIRM_PINCODE = "AdminGUIConfirmPincode";
		private const string CITRIX_WI_TITLE = "CitrixWITitle";
		
		const string FIELD_MISSING_ERROR_EMAIL = "FieldMissingErrorEmail";
		const string FIELD_MISSING_ERROR_MOBILE_PHONE = "FieldMissingErrorMobilePhone";

        private const string SEND_TRACKING_INFO = "SendTrackingInfo";

		private bool configFileDoesNotExists;

		public SystemConfiguration()
		{
			this.StopServiceOnException = false;
			this.OnOffFound = false;
			this.ErrorParsingConfiguration = false;

			this.ADUsername = "Administrator";
			this.ADPassword = "Password$";
			this.ADServerAddress = "localhost";
			this.ADContainer = "";
			this.ADBaseDN = "";
			this.ADFilter = "(&(objectClass=person))";
			this.ADDisasterPercentage = 0;
			this.ADAdminBaseDNOverride = "";
			this.ADAdminFilterOverride = "";
			this.DbServer = "(local)";
			this.DatabaseName = "SMS_DB";
			this.DbUsername = "sa";
			this.DbPassword = "";
			
			this.VaultPassword = "Wr1gh7_SMS2";

			this.AuthEnginePinCode = PinCodeOption.True;
			this.CloudSMSServerPort = 9070;
			this.AuthEngineServerPort = 9060;
			this.OATHCalcServerPort = 9991;

			this.AuthEngineUsersPollInterval = 40;
            this.AuthEngineHeartbeatInterval = 2 * 60;
            this.AuthEngineWaitingInterval = 15 * 60;
            this.AuthEnginePollingPreference = PollingPreference.Default;
            this.MutualAuthImagesPollingPreference = PollingPreference.Default;
			this.AuthEngineUseEncryption = true;
			this.CloudSMSUseEncryption = true;
			this.OATHCalcUseEncryption = true;
			this.CloudSMSFlushOnWrite = false;
			this.AuthEngineDefaultEnabled = true;
			this.AuthEngineAllowEmergencyPasscode = false;
			this.AuthEngineOverrideWithADInfo = false;
			this.AuthEngineLockDownMode = false;
			
			this.AuthEngineAskMissingInfo = false;
			this.AuthEngineAskPin = false;
			this.AuthEngineAskProviderInfo = false;
			this.AuthEngineAskAgedTimeHours = 168;
			
			AuthEngineAutoSetupAgedTimeHours = 168;
			
			this.BaseSendTokenTestMode = false;
			this.AuthEngineDisableUserVerification = false;
			this.DbUseIntegratedSecurity = false;
			this.DbPort = 1433;
			this.DbUsePipes = false;
            this.DbPipeName = @"\sql\query";
			this.OATHCalcTotpWindow = 10; // Accept 10 tokens before and after current token
			this.OATHCalcHotpAfterWindow = 15; // Accept 10 tokens before and after current token
			this.configFileDoesNotExists = false;
			this.AuthEngineDefaultDomain = string.Empty;
			this.MinTimeBetweenRadiusRequestsPerUser = 2000;
			this.NotifyPinCodeIncorrectOnAccess = true;
			this.AuthEngineChallengeResponse = true;
			this.PinTanMaxSheets = 5;
			this.PinTanColumns = 5;
			this.PinTanRows = 10;
			this.PintanPasscodeLength = 6;
			this.CloudSMSTokenExpireTime = 15;
			this.OneTimeTokenExpireTimeMinutes = 10 * 24 * 60;
			this.EmergencyTokenExpireTimeMinutes = 15;

			this.AuthEnginePinCodeLength = 6;
			this.AuthEngineRemoveUsersAfterXHours = 96;

			this.LogMaxFiles = 5;
			this.LogMaxSize = 5;

			this.AuthEngineLogLevel = Log.LogLevel.All;
			this.AuthEngineFileLogLevel = Log.LogLevel.Info;
			this.AuthEngineEventLogLevel = Log.LogLevel.Error;
			this.AuthEngineFlushOnWrite = false;
			this.CloudSMSLogLevel = Log.LogLevel.All;
			this.CloudSMSFileLogLevel = Log.LogLevel.Info;
			this.CloudSMSEventLogLevel = Log.LogLevel.Error;

			this.AuthEngineDefaultExceptionGroups = new AuthEngineDefaultExceptionGroups() { new Group() { Name = "WrightCCS" } };
			this.ExtraDCs = new ExtraDCs() { new DC() { Name = "" } };
			this.ManualDomainReplacements = new ManualDomainReplacements() { new DomainReplacement() { Name = "" } };

            

			this.Providers = new AuthProviders() {
						new Provider() { Name="CloudSMS", AdGroup="", Enabled=true, Default=true }
						,new Provider() { Name="OATHCalc", AdGroup="", Enabled=true, Config="HOTP,,1,Default", AutoSetup = false }
						,new Provider() { Name="PINTAN", AdGroup="", Enabled=true, Config="", AutoSetup = false }
						,new Provider() { Name="Email", AdGroup="", Enabled=true, Config="" }
						,new Provider() { Name="NHS", AdGroup="", Enabled=false, Config="" }
						,new Provider() { Name="Static", AdGroup="", Enabled=false, Config="" }
						,new Provider() { Name="OneTime", FriendlyName="XenMobile-Enrolment", AdGroup="", Enabled=false, Config="" }
						,new Provider() { Name="Passthrough", FriendlyName="Passthrough", AdGroup="", Enabled=false, Config="" }
				};

			this.EmailConfig = new EmailConfig()
			{
				UseAuth = true,
				EnableSSL = true,
				From = "wrightcss@stub.com",
				Password = "password",
				Port = 25,
				Server = "emailserver.com",
				Username = "wrightcss@stub.com",
				MessageTitle = "Access Token"
			};

			this.CloudSMSConfiguration = new CloudSMSConfiguration();

			this.CloudSMSConfiguration.CloudSMSModules = new CloudSMSModules();

			var cloudSMSModuleConfig = new CloudSMSModuleConfig()
			{
				TypeName = "Regexp",
				ModuleParameters = new ModuleParameters() {
					new ModuleParameter() { Name = "Url", Value = @"http://www.txtlocal.com/sendsmspost.php" },
					new ModuleParameter() { Name = "Regex", Value = @"CreditsRemaining=(?'CreditsRemaining'[0-9\.]+)" },
					new ModuleParameter() { Name = "CreditsRemaining", Value = "1", Output = true},
					new ModuleParameter() { Name = "selectednums", Value = @"{destination}" },
					new ModuleParameter() { Name = "message", Value = @"{message}" },
					new ModuleParameter() { Name = "uname", Value = @"textlocal@username.com" },
					new ModuleParameter() { Name = "pword", Value = @"textlocalpassword", Encrypt = true },
					//new ModuleParameter() { Name = "uname", Value = @"steven@stevenwright.co.uk" },
					//new ModuleParameter() { Name = "pword", Value = @"password", Encrypt = true },
					new ModuleParameter() { Name = "from", Value = @"SMS_Validation" },
					new ModuleParameter() { Name = "info", Value = @"1" },
				},
			};

			this.CloudSMSConfiguration.CloudSMSModules.Add(cloudSMSModuleConfig);

            this.RadiusChallengeMessage = "Message challenge";
			this.RadiusShowPin = false;
			this.AdminGUIConfirmPincode = true;
			this.CitrixWITitle = "Please enter your token code";
			this.FieldMissingErrorEmail = "Enter your e-mail address:";
			this.FieldMissingErrorMobilePhone = "Enter your mobile phone number:";

            this.AuthEngineAllowAliasesInLogin = true;
            this.AuthEngineAllowEmailLogin = false;
            this.AuthEngineAllowMobileNumberLogin = false;
            this.AuthEngineAllowPre2000Login = true;
            this.AuthEngineAllowUPNLogin = true;
            this.AuthEngineSAMLoginPreferred = true;

            this.OATHCalcDefaultConfig = string.Empty;
            this.SendTrackingInfo = true;
		}

		public SystemConfiguration(String startUpPath, String settingsPath = "Settings")
			: this()
		{
			if (!string.IsNullOrEmpty(startUpPath))
			{
				String path = Directory.GetParent(startUpPath).FullName;
				path = path.TrimEnd(Path.DirectorySeparatorChar);

				this._startUpPath = path + @"\" + settingsPath;
			}
			if (string.IsNullOrEmpty(this._startUpPath) || !File.Exists(GetConfigFullname()))
			{
				this.configFileDoesNotExists = true;
			}
		}

		public string GetConfigFullname()
		{
			return Path.Combine(this.GetSettingsPath(), "Configuration.xml");
		}

		public string GetSettingsPath()
		{
			return _startUpPath;
		}

		public string GetWrightPath()
		{
			return Directory.GetParent(_startUpPath).FullName + Path.DirectorySeparatorChar;
		}

		public bool ConfigFileDoesNotExists()
		{
			return this.configFileDoesNotExists;
		}

		private void writeCredentials(string outputPath, Dictionary<string, string> data)
		{
			DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(outputPath));
			if (!dir.Exists)
				dir.Create();
			if (File.Exists(outputPath))
			{
				try
				{
					File.Delete(outputPath);
				}
				catch { }
			}
			using (XmlTextWriter txtWriter = new XmlTextWriter(outputPath, System.Text.Encoding.UTF8))
			{
				txtWriter.WriteStartDocument();
				txtWriter.Formatting = Formatting.Indented;
				txtWriter.Indentation = 2;
				txtWriter.WriteStartElement("User");

				foreach (KeyValuePair<string, string> entry in data)
					this.writeSetting(txtWriter, entry.Key, entry.Value);

				txtWriter.WriteEndDocument();
			}
		}

		public void WriteClientCredentials(string outputPath)
		{
			this.writeCredentials(outputPath, new Dictionary<string, string>() {
								{ "AuthEngineServerIP", _authEngineServerIp.ToString() }
								,{ "AuthEngineServerPort", this.AuthEngineServerPort.ToString() }
								,{AUTHENGINE_USEENCRYPTION, this.AuthEngineUseEncryption.ToString() }
								,{RADIUS_CHALLENGE_MESSAGE, this.RadiusChallengeMessage }
								,{RADIUS_SHOW_PIN, this.RadiusShowPin.ToString() }
								,{ADMINGUI_CONFIRM_PINCODE, this.AdminGUIConfirmPincode.ToString() }
								,{CITRIX_WI_TITLE, this.CitrixWITitle}
								});
		}
		public void WriteCloudSMSCredentials(string outputPath)
		{
			this.writeCredentials(outputPath, new Dictionary<string, string>() {
								{ "AuthEngineServerIP", _authEngineServerIp.ToString() }
								,{ "AuthEngineServerPort", this.AuthEngineServerPort.ToString() }
								,{AUTHENGINE_USEENCRYPTION, this.AuthEngineUseEncryption.ToString() }

								,{"SMSServerIP",_smsServerIp.ToString()}
								,{"SMSPort", this.CloudSMSServerPort.ToString()}

								//,{CLOUDSMS_LOG, this.CloudSMSLogLevel.ToString()}
								,{CLOUDSMS_FILELOG, this.CloudSMSFileLogLevel.ToString()}
								,{CLOUDSMS_EVENTLOG, this.CloudSMSEventLogLevel.ToString()}
								,{CLOUDSMS_USEENCRYPTION, this.CloudSMSUseEncryption.ToString()}
								,{CLOUDSMS_LOGFLUSHONWRITE, this.CloudSMSFlushOnWrite.ToString()}
								,{CLOUDSMS_TOKEN_EXPIRE_TIME_MINUTES, this.CloudSMSTokenExpireTime.ToString()}
								//,{CLOUDSMS_MODULECONFIG, this.CloudSMSModuleConfig}
								});
		}

		private string TryEncrypted(string password, string salt)
		{
			var currentPassword = password;
			try
			{
				return CryptoHelper.EncryptSettingIfNecessary(password, salt);
			}
			catch { }
			return currentPassword;
		}

		public void WriteAuthEngineCredentials(string outputPath)
		{
			var thisDbPassword = TryEncrypted(this.DbPassword, "DBPassword");
			var thisADPassword = TryEncrypted(this.ADPassword, "ADPassword");
			var vaultPassword = TryEncrypted(VaultPassword, AUTHENGINE_VAULT_PASSWORD);

			this.writeCredentials(outputPath, new Dictionary<string, string>() {
								{ "AuthEngineServerIP", _authEngineServerIp.ToString() }
								,{ "AuthEngineServerPort",this.AuthEngineServerPort.ToString() }
								,{AUTHENGINE_USEENCRYPTION, this.AuthEngineUseEncryption.ToString() }

								,{"SQLServer", this.DbServer}
								,{"SQLPort", this.DbPort.ToString()}

								,{"DBName", this.DatabaseName}
								,{"DBUser", this.DbUsername}
								,{"DBPassword", thisDbPassword}
								,{"DBUsePipes", this.DbUsePipes.ToString()}
                                ,{"DBPipeName", this.DbPipeName}
								,{"DBUseIntegratedSecurity",this.DbUseIntegratedSecurity.ToString()}

								,{"SMSServerIP",_smsServerIp.ToString()}
								,{"SMSPort", this.CloudSMSServerPort.ToString()}

								,{"ADUsername", this.ADUsername}
								,{"ADPassword", thisADPassword}
								,{"LDAPServer", this.ADServerAddress}
								,{ADCONTAINER, this.ADContainer}
								,{ADBASEDN, this.ADBaseDN}
								,{ADFILTER, this.ADFilter}
								,{AD_DISASTER_PERCENTAGE, ADDisasterPercentage.ToString()}
								,{AD_ADMIN_BASEDN_OVERRIDE, ADAdminBaseDNOverride }
								,{AD_ADMIN_FILTER_OVERRIDE, ADAdminFilterOverride }

								,{AUTHENGINE_USERSPOLLINTERVAL, this.AuthEngineUsersPollInterval.ToString()}
                                ,{AUTHENGINE_HEARTBEATINTERVAL, this.AuthEngineHeartbeatInterval.ToString()}
                                ,{AUTHENGINE_WAITINGINTERVAL, this.AuthEngineWaitingInterval.ToString()}
                                ,{AUTHENGINE_POLLINGPREFERENCE, this.AuthEnginePollingPreference.ToString()}
                                ,{MUTUALAUTHIMAGES_POLLINGPREFERENCE, this.MutualAuthImagesPollingPreference.ToString()}
								,{AUTHENGINE_DEFAULT_ENABLED, this.AuthEngineDefaultEnabled.ToString()}
								,{AUTHENGINE_ALLOWEMERGENCYPASSCODE, this.AuthEngineAllowEmergencyPasscode.ToString()}
								,{AUTHENGINE_OVERRIDEWITHADINFO, this.AuthEngineOverrideWithADInfo.ToString()}
								,{AUTHENGINE_REMOVEUSERS_AFTER_X_HOURS, this.AuthEngineRemoveUsersAfterXHours.ToString()}
								,{AUTHENGINE_DEFAULT_DOMAIN, this.AuthEngineDefaultDomain}
								,{NOTIFYPINCODEINCORRECTONACCESS, this.NotifyPinCodeIncorrectOnAccess.ToString()}
								,{AUTHENGINE_LOCKDOWN_MODE, this.AuthEngineLockDownMode.ToString()}
								
								,{AUTHENGINE_VAULT_PASSWORD, vaultPassword}
								,{AUTHENGINE_PINCODE_PANIC, AuthEnginePinCodePanic.ToString()}
                                ,{AUTHENGINE_ENCRYPTION_KEY, AuthEngineEncryptionKey}
								
								,{AUTHENGINE_ASKAGEDTIMEHOURS, AuthEngineAskAgedTimeHours.ToString()}

                                ,{AUTHENGINE_ALLOWUPNLOGIN, AuthEngineAllowUPNLogin.ToString()}
                                ,{AUTHENGINE_ALLOWPRE2000LOGIN, AuthEngineAllowPre2000Login.ToString()}
                                ,{AUTHENGINE_ALLOWMOBILENUMBERLOGIN, AuthEngineAllowMobileNumberLogin.ToString()}
                                ,{AUTHENGINE_ALLOWEMAILLOGIN, AuthEngineAllowEmailLogin.ToString()}
                                ,{AUTHENGINE_SAMLOGINPREFERRED, AuthEngineSAMLoginPreferred.ToString()}
                                ,{AUTHENGINE_ALLOWALIASESINLOGIN, AuthEngineAllowAliasesInLogin.ToString()}
								
								,{AUTHENGINE_AUTOSETUP_AGED_TIMEHOURS, AuthEngineAutoSetupAgedTimeHours.ToString()}

								,{LOGMAXFILES, this.LogMaxFiles.ToString()}
								,{LOGMAXSIZE, this.LogMaxSize.ToString()}

								//,{AUTHENGINE_LOG, this.AuthEngineLogLevel.ToString()}
								,{AUTHENGINE_FILELOG, this.AuthEngineFileLogLevel.ToString()}
								,{AUTHENGINE_EVENTLOG, this.AuthEngineEventLogLevel.ToString()}

								,{OATHCALC_SERVERIP, _oathCalcServerIp.ToString()}
								,{OATHCALC_SERVERPORT, this.OATHCalcServerPort.ToString()}
								,{OATHCALC_USEENCRYPTION, this.OATHCalcUseEncryption.ToString()}

								,{OATHCALC_TOTP_WINDOW, this.OATHCalcTotpWindow.ToString()}
								,{OATHCALC_HOTP_AFTERWINDOW, this.OATHCalcHotpAfterWindow.ToString()}

								,{CLOUDSMS_TOKEN_EXPIRE_TIME_MINUTES, this.CloudSMSTokenExpireTime.ToString()}
								,{ONETIME_TOKEN_EXPIRE_TIME_MINUTES, OneTimeTokenExpireTimeMinutes.ToString()}
								,{EMERGENCY_TOKEN_EXPIRE_TIME_MINUTES, EmergencyTokenExpireTimeMinutes.ToString()}

								,{PINTAN_MAX_SHEETS, this.PinTanMaxSheets.ToString()}
								,{MINTIMEBETWEENRADIUSREQUESTSPERUSER, this.MinTimeBetweenRadiusRequestsPerUser.ToString()}
								,{AUTHENGINE_CHALLENGERESPONSE, this.AuthEngineChallengeResponse.ToString()}
								,{SEND_TRACKING_INFO, this.SendTrackingInfo.ToString()}
								});
		}

        public void SavePollingPreferences()
        {
            string configFile = GetConfigFullname();

            if (File.Exists(configFile)) {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFile);
                
                XmlNode userNode = doc.SelectSingleNode("User");

                XmlNode aePreferenceNode = userNode.SelectSingleNode("descendant::" + AUTHENGINE_POLLINGPREFERENCE);
                if (aePreferenceNode.InnerText != this.AuthEnginePollingPreference.ToString()) {
                    aePreferenceNode.InnerText = this.AuthEnginePollingPreference.ToString();
                    using (var writer = new StreamWriter(configFile)) {
                        doc.Save(writer);
                    }
                }

                XmlNode maiPreferenceNode = userNode.SelectSingleNode("descendant::" + MUTUALAUTHIMAGES_POLLINGPREFERENCE);
                if (maiPreferenceNode.InnerText != this.MutualAuthImagesPollingPreference.ToString()) {
                    maiPreferenceNode.InnerText = this.MutualAuthImagesPollingPreference.ToString();
                    using (var writer = new StreamWriter(configFile)) {
                        doc.Save(writer);
                    }
                }

            }
        }

		public void WriteXMLCredentials()
		{
			if (string.IsNullOrEmpty(_startUpPath))
				return;

			DirectoryInfo dir = new DirectoryInfo(_startUpPath);
			if (!dir.Exists)
				dir.Create();

			WriteXMLCredentials(GetConfigFullname());
		}

		public void WriteXMLCredentials(string file)
		{
			XmlWriterSettings writerSettings = new XmlWriterSettings();
			writerSettings.Indent = true;
			writerSettings.NewLineHandling = NewLineHandling.None;
			var utf8nobom = new UTF8Encoding(false);
			writerSettings.Encoding = utf8nobom;
            writerSettings.CloseOutput = true;

			using (var txtWriter = XmlWriter.Create(file, writerSettings))
			{
				WriteXMLCredentials(txtWriter);                
				txtWriter.Close();
			}
		}

		public string WriteXMLCredentialsToString()
		{
			using (var ms = new MemoryStream())
			{
				XmlWriterSettings writerSettings = new XmlWriterSettings();
				writerSettings.Indent = true;
				writerSettings.NewLineHandling = NewLineHandling.None;
				var utf8nobom = new UTF8Encoding(false);
				writerSettings.Encoding = utf8nobom;

				using (var writer = XmlWriter.Create(ms, writerSettings))
				{
					WriteXMLCredentials(writer);
				}
				ms.Position = 0;
				using (var sr = new StreamReader(ms))
				{
					return sr.ReadToEnd();
				}
			}
		}

		public void WriteXMLCredentials(XmlWriter txtWriter)
		{
			txtWriter.WriteStartDocument();

			//-------------- Used For Client Solution
			txtWriter.WriteStartElement("User");

			writeSetting(txtWriter, "AuthEngineServerIP", _authEngineServerIp.ToString());
			writeSetting(txtWriter, "AuthEngineServerPort", this.AuthEngineServerPort.ToString());
			writeSetting(txtWriter, AUTHENGINE_USEENCRYPTION, this.AuthEngineUseEncryption.ToString());
			//------------------

			//----------- Used For Server Solution

			writeSetting(txtWriter, AUTHENGINE_USERSPOLLINTERVAL, this.AuthEngineUsersPollInterval.ToString());
            writeSetting(txtWriter, AUTHENGINE_HEARTBEATINTERVAL, this.AuthEngineHeartbeatInterval.ToString());
            writeSetting(txtWriter, AUTHENGINE_WAITINGINTERVAL, this.AuthEngineWaitingInterval.ToString());
            writeSetting(txtWriter, AUTHENGINE_POLLINGPREFERENCE, this.AuthEnginePollingPreference.ToString());
            writeSetting(txtWriter, MUTUALAUTHIMAGES_POLLINGPREFERENCE, this.MutualAuthImagesPollingPreference.ToString());
			writeSetting(txtWriter, AUTHENGINE_DEFAULT_ENABLED, this.AuthEngineDefaultEnabled.ToString());
			writeSetting(txtWriter, AUTHENGINE_ALLOWEMERGENCYPASSCODE, this.AuthEngineAllowEmergencyPasscode.ToString());
			writeSetting(txtWriter, AUTHENGINE_OVERRIDEWITHADINFO, this.AuthEngineOverrideWithADInfo.ToString());
			writeSetting(txtWriter, AUTHENGINE_LOCKDOWN_MODE, this.AuthEngineLockDownMode.ToString());
			writeSetting(txtWriter, AUTHENGINE_REMOVEUSERS_AFTER_X_HOURS, this.AuthEngineRemoveUsersAfterXHours.ToString());
			writeSetting(txtWriter, AUTHENGINE_DEFAULT_DOMAIN, this.AuthEngineDefaultDomain);
			writeSetting(txtWriter, NOTIFYPINCODEINCORRECTONACCESS, this.NotifyPinCodeIncorrectOnAccess.ToString());
			
			writeSetting(txtWriter, AUTHENGINE_ASKAGEDTIMEHOURS, AuthEngineAskAgedTimeHours.ToString());
			
			writeSetting(txtWriter, AUTHENGINE_AUTOSETUP_AGED_TIMEHOURS, AuthEngineAutoSetupAgedTimeHours.ToString());

			writeSetting(txtWriter, "SQLServer", this.DbServer);
			writeSetting(txtWriter, "SQLPort", this.DbPort.ToString());

			writeSetting(txtWriter, "DBName", this.DatabaseName);
			writeSetting(txtWriter, "DBUser", this.DbUsername);
			writeSetting(txtWriter, "DBPassword", TryEncrypted(this.DbPassword, "DBPassword"));
			writeSetting(txtWriter, "DBUsePipes", this.DbUsePipes.ToString());
            writeSetting(txtWriter, "DBPipeName", this.DbPipeName);
			writeSetting(txtWriter, "DBUseIntegratedSecurity", this.DbUseIntegratedSecurity.ToString());

			writeSetting(txtWriter, "SMSServerIP", _smsServerIp.ToString());
			writeSetting(txtWriter, "SMSPort", this.CloudSMSServerPort.ToString());

			writeSetting(txtWriter, "ADUsername", this.ADUsername);
			writeSetting(txtWriter, "ADPassword", TryEncrypted(this.ADPassword, "ADPassword"));
			writeSetting(txtWriter, "LDAPServer", this.ADServerAddress);
			writeSetting(txtWriter, ADCONTAINER, this.ADContainer);
			writeSetting(txtWriter, ADBASEDN, this.ADBaseDN);
			writeSetting(txtWriter, ADFILTER, this.ADFilter);
			writeSetting(txtWriter, AD_DISASTER_PERCENTAGE, ADDisasterPercentage.ToString());
			writeSetting(txtWriter, AD_ADMIN_BASEDN_OVERRIDE, ADAdminBaseDNOverride);
			writeSetting(txtWriter, AD_ADMIN_FILTER_OVERRIDE, ADAdminFilterOverride);

			writeSetting(txtWriter, LOGMAXFILES, this.LogMaxFiles.ToString());
			writeSetting(txtWriter, LOGMAXSIZE, this.LogMaxSize.ToString());

			//writeSetting(txtWriter, AUTHENGINE_LOG, this.AuthEngineLogLevel.ToString());
			writeSetting(txtWriter, AUTHENGINE_FILELOG, this.AuthEngineFileLogLevel.ToString());
			writeSetting(txtWriter, AUTHENGINE_EVENTLOG, this.AuthEngineEventLogLevel.ToString());
			writeSetting(txtWriter, AUTHENGINE_LOGFLUSHONWRITE, AuthEngineFlushOnWrite.ToString());
			
			//writeSetting(txtWriter, CLOUDSMS_LOG, this.CloudSMSLogLevel.ToString());
			writeSetting(txtWriter, CLOUDSMS_FILELOG, this.CloudSMSFileLogLevel.ToString());
			writeSetting(txtWriter, CLOUDSMS_EVENTLOG, this.CloudSMSEventLogLevel.ToString());
			writeSetting(txtWriter, CLOUDSMS_USEENCRYPTION, this.CloudSMSUseEncryption.ToString());

			writeSetting(txtWriter, CLOUDSMS_LOGFLUSHONWRITE, this.CloudSMSFlushOnWrite.ToString());

			writeSetting(txtWriter, CLOUDSMS_TOKEN_EXPIRE_TIME_MINUTES, this.CloudSMSTokenExpireTime.ToString());
			writeSetting(txtWriter, ONETIME_TOKEN_EXPIRE_TIME_MINUTES, OneTimeTokenExpireTimeMinutes.ToString());
			writeSetting(txtWriter, EMERGENCY_TOKEN_EXPIRE_TIME_MINUTES, EmergencyTokenExpireTimeMinutes.ToString());

			writeSetting(txtWriter, OATHCALC_SERVERIP, _oathCalcServerIp.ToString());
			writeSetting(txtWriter, OATHCALC_SERVERPORT, this.OATHCalcServerPort.ToString());
			writeSetting(txtWriter, OATHCALC_USEENCRYPTION, this.OATHCalcUseEncryption.ToString());

			writeSetting(txtWriter, OATHCALC_TOTP_WINDOW, this.OATHCalcTotpWindow.ToString());
			writeSetting(txtWriter, OATHCALC_HOTP_AFTERWINDOW, this.OATHCalcHotpAfterWindow.ToString());

			writeSetting(txtWriter, PINTAN_MAX_SHEETS, this.PinTanMaxSheets.ToString());
			writeSetting(txtWriter, MINTIMEBETWEENRADIUSREQUESTSPERUSER, this.MinTimeBetweenRadiusRequestsPerUser.ToString());
			writeSetting(txtWriter, AUTHENGINE_CHALLENGERESPONSE, this.AuthEngineChallengeResponse.ToString());
			
			writeSetting(txtWriter, AUTHENGINE_VAULT_PASSWORD, TryEncrypted(VaultPassword, AUTHENGINE_VAULT_PASSWORD));
			writeSetting(txtWriter, AUTHENGINE_PINCODE_PANIC, AuthEnginePinCodePanic.ToString());
            writeSetting(txtWriter, AUTHENGINE_ENCRYPTION_KEY, AuthEngineEncryptionKey);

            writeSetting(txtWriter, SEND_TRACKING_INFO, SendTrackingInfo.ToString());

			string providers = string.Empty;
			writeRaw(txtWriter, Environment.NewLine);

			foreach (Provider p in this.Providers)
				providers += AuthGateway.Shared.Serializer.Generic.SerializeIndented<Provider>(p) + Environment.NewLine;
			writeRawSetting(txtWriter, AUTHPROVIDERS, providers);
			writeRaw(txtWriter, Environment.NewLine);

			writeRaw(txtWriter,
					Generic.SerializeIndented<AuthEngineDefaultExceptionGroups>(this.AuthEngineDefaultExceptionGroups)
			);
			writeRaw(txtWriter, Environment.NewLine);

			if (this.ExtraDCs.Count == 0)
				this.ExtraDCs.Add(new DC() { Name = "" });

			writeRaw(txtWriter,
					Generic.SerializeIndented<ExtraDCs>(this.ExtraDCs)
			);
			writeRaw(txtWriter, Environment.NewLine);

			if (this.ManualDomainReplacements.Count == 0)
			{
				this.ManualDomainReplacements = new ManualDomainReplacements() { new DomainReplacement() { Name = "" } };
			}

			writeRaw(txtWriter,
					Generic.SerializeIndented<ManualDomainReplacements>(this.ManualDomainReplacements)
			);
			writeRaw(txtWriter, Environment.NewLine);

			writeRaw(txtWriter,
					Generic.SerializeIndented<EmailConfig>(this.EmailConfig)
			);
			writeRaw(txtWriter, Environment.NewLine);

			writeRaw(txtWriter,
				Generic.SerializeIndented<CloudSMSConfiguration>(this.CloudSMSConfiguration)
			);
			writeRaw(txtWriter, Environment.NewLine);

			txtWriter.WriteEndElement();
			txtWriter.WriteEndDocument();
		}

		public void LoadSettings(bool rewriteForEncryption = false)
		{
			if (string.IsNullOrEmpty(_startUpPath))
			{
				return;
			}
			LoadSettingsFromFile(GetConfigFullname(), rewriteForEncryption);
		}

		public void LoadSettingsFromFile(string file, bool rewriteForEncryption)
		{
			if (string.IsNullOrEmpty(file))
			{
				return;
			}
			try
			{
				var txt = File.ReadAllText(file, Encoding.UTF8);
				txt = txt.Trim();
				var needsRewritingToEncrypt = LoadSettings(txt);
				if (rewriteForEncryption && needsRewritingToEncrypt)
				{
					try
					{
						File.Delete(file);
					}
					catch { }
					try
					{
						WriteXMLCredentials(file);
					}
					catch { }
				}
			}
			catch (SystemConfigurationParseError)
			{
				throw;
			}
			catch (Exception ex)
			{
				this.ErrorParsingConfiguration = true;
				throw new SystemConfigurationParseError("WARNING: Error parsing configuration file, some values may be set to default value.", ex);
			}
		}

		public bool LoadSettings(string xmlContents)
		{
			try
			{
				var rewriteSystemConfiguration = false;
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xmlContents);
				string tmpString;

				_authEngineServerIp = IPAddress.Parse(getItemFromXML(doc, "AuthEngineServerIP"));
				this.AuthEngineServerPort = getInt32OrDef(getItemFromXML(doc, "AuthEngineServerPort"), this.AuthEngineServerPort);
				this.AuthEngineUseEncryption = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_USEENCRYPTION), this.AuthEngineUseEncryption);

				this.DbServer = getItemFromXML(doc, "SQLServer");
				this.DbPort = getInt32OrDef(getItemFromXML(doc, "SQLPort"), this.DbPort);

				this.DatabaseName = getItemFromXML(doc, "DBName");
				this.DbUsername = getItemFromXML(doc, "DBUser");
				
				var dbPassword = getItemFromXML(doc, "DBPassword");
				this.DbPassword = decryptIfNecessary(dbPassword, "DBPassword");
				rewriteSystemConfiguration = rewriteSystemConfiguration || (!string.IsNullOrEmpty(dbPassword) && dbPassword == DbPassword);

				this.DbUsePipes = getThisBoolOrDef(getItemFromXML(doc, "DBUsePipes"), this.DbUsePipes);
                this.DbPipeName = getItemFromXML(doc, "DBPipeName");
				this.DbUseIntegratedSecurity = getThisBoolOrDef(getItemFromXML(doc, "DBUseIntegratedSecurity"), this.DbUseIntegratedSecurity);

				this.NotifyPinCodeIncorrectOnAccess = getThisBoolOrDef(getItemFromXML(doc, NOTIFYPINCODEINCORRECTONACCESS), this.NotifyPinCodeIncorrectOnAccess);

				tmpString = getItemFromXML(doc, "SMSServerIP");
				if (!string.IsNullOrEmpty(tmpString))
					_smsServerIp = IPAddress.Parse(tmpString);

				this.CloudSMSServerPort = getInt32OrDef(getItemFromXML(doc, "SMSPort"), this.CloudSMSServerPort);

				this.ADUsername = getItemFromXML(doc, "ADUsername");

				var adPassword = getItemFromXML(doc, "ADPassword");
				ADPassword = decryptIfNecessary(adPassword, "ADPassword");
				rewriteSystemConfiguration = rewriteSystemConfiguration || (!string.IsNullOrEmpty(adPassword) && adPassword == ADPassword);

				this.ADServerAddress = getItemFromXML(doc, "LDAPServer");
				this.ADContainer = getItemFromXML(doc, ADCONTAINER);
				this.ADBaseDN = getItemFromXML(doc, ADBASEDN);
				this.ADFilter = getItemFromXML(doc, ADFILTER);
				ADDisasterPercentage = getInt32OrDef(getItemFromXML(doc, AD_DISASTER_PERCENTAGE), ADDisasterPercentage);
				ADAdminBaseDNOverride = getStringOrDef(getItemFromXML(doc, AD_ADMIN_BASEDN_OVERRIDE), ADAdminBaseDNOverride);
				ADAdminFilterOverride = getStringOrDef(getItemFromXML(doc, AD_ADMIN_FILTER_OVERRIDE), ADAdminFilterOverride);

				this.AuthEngineUsersPollInterval = getInt32OrDef(getItemFromXML(doc, AUTHENGINE_USERSPOLLINTERVAL), this.AuthEngineUsersPollInterval);
                this.AuthEngineHeartbeatInterval = getInt32OrDef(getItemFromXML(doc, AUTHENGINE_HEARTBEATINTERVAL), this.AuthEngineHeartbeatInterval);
                this.AuthEngineWaitingInterval = getInt32OrDef(getItemFromXML(doc, AUTHENGINE_WAITINGINTERVAL), this.AuthEngineWaitingInterval);
                this.AuthEnginePollingPreference = (PollingPreference)getEnumIntOrDef(typeof(PollingPreference), getItemFromXML(doc, AUTHENGINE_POLLINGPREFERENCE), (int)this.AuthEnginePollingPreference);
                this.MutualAuthImagesPollingPreference = (PollingPreference)getEnumIntOrDef(typeof(PollingPreference), getItemFromXML(doc, MUTUALAUTHIMAGES_POLLINGPREFERENCE), (int)this.MutualAuthImagesPollingPreference);

				this.AuthEngineDefaultEnabled = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_DEFAULT_ENABLED), this.AuthEngineDefaultEnabled);
				this.AuthEngineAllowEmergencyPasscode = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_ALLOWEMERGENCYPASSCODE), this.AuthEngineAllowEmergencyPasscode);
				this.AuthEngineOverrideWithADInfo = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_OVERRIDEWITHADINFO), this.AuthEngineOverrideWithADInfo);
				this.AuthEngineLockDownMode = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_LOCKDOWN_MODE), this.AuthEngineLockDownMode);

				var vaultPassword = getItemFromXML(doc, AUTHENGINE_VAULT_PASSWORD);
				VaultPassword = decryptIfNecessary(vaultPassword, AUTHENGINE_VAULT_PASSWORD);
				rewriteSystemConfiguration = rewriteSystemConfiguration || (!string.IsNullOrEmpty(vaultPassword) && vaultPassword == VaultPassword);
				if ( string.IsNullOrEmpty(vaultPassword)) {
					var rand = new byte[32];
					CryptoRandom.Instance().NextBytes(rand);
					VaultPassword = Convert.ToBase64String(rand);
					rewriteSystemConfiguration = true;
				}
				AuthEnginePinCodePanic = getBoolOrDef(getItemFromXML(doc, AUTHENGINE_PINCODE_PANIC), AuthEnginePinCodePanic);
                AuthEngineEncryptionKey = getItemFromXML(doc, AUTHENGINE_ENCRYPTION_KEY);
				
				this.AuthEnginePinCode = getThisPinCodeOption(getItemFromXML(doc, AUTHENGINE_PINCODE));

				this.AuthEnginePinCodeLength = getInt32OrDef(getItemFromXML(doc, AUTHENGINE_PINCODE_LENGTH), this.AuthEnginePinCodeLength);
				
				AuthEngineAskMissingInfo = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_ASKMISSINGINFO), AuthEngineAskMissingInfo);
				AuthEngineAskPin = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_ASKPIN), AuthEngineAskPin);
				AuthEngineAskProviderInfo = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_ASKPROVIDERINFO), AuthEngineAskProviderInfo);
				AuthEngineAskAgedTimeHours = getInt32OrDef(getItemFromXML(doc, AUTHENGINE_ASKAGEDTIMEHOURS), AuthEngineAskAgedTimeHours);
				
				AuthEngineAutoSetupAgedTimeHours = getInt32OrDef(getItemFromXML(doc, AUTHENGINE_AUTOSETUP_AGED_TIMEHOURS), AuthEngineAutoSetupAgedTimeHours);

                AuthEngineAllowUPNLogin = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_ALLOWUPNLOGIN), AuthEngineAllowUPNLogin);
                AuthEngineAllowPre2000Login = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_ALLOWPRE2000LOGIN), AuthEngineAllowPre2000Login);
                AuthEngineAllowMobileNumberLogin = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_ALLOWMOBILENUMBERLOGIN), AuthEngineAllowMobileNumberLogin);
                AuthEngineAllowEmailLogin = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_ALLOWEMAILLOGIN), AuthEngineAllowEmailLogin);
                AuthEngineAllowAliasesInLogin = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_ALLOWALIASESINLOGIN), AuthEngineAllowAliasesInLogin);
                AuthEngineSAMLoginPreferred = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_SAMLOGINPREFERRED), AuthEngineSAMLoginPreferred);

				string _groups = getXmlFromXML(doc, AUTHENGINE_DEFAULT_EXCEPTION_GROUPS);
				if (!string.IsNullOrEmpty(_groups))
					this.AuthEngineDefaultExceptionGroups = (AuthGateway.Shared.Serializer.Generic.Deserialize<AuthEngineDefaultExceptionGroups>(_groups));
				else
					this.AuthEngineDefaultExceptionGroups = new AuthEngineDefaultExceptionGroups();

				tmpString = getXmlFromXML(doc, AUTHENGINE_EXTRADCS);
				if (!string.IsNullOrEmpty(tmpString))
					this.ExtraDCs = (AuthGateway.Shared.Serializer.Generic.Deserialize<ExtraDCs>(tmpString));
				else
					this.ExtraDCs = new ExtraDCs();

				tmpString = getXmlFromXML(doc, AUTHENGINE_MANUALDOMAINREPLACEMENTS);
				if (!string.IsNullOrEmpty(tmpString))
					this.ManualDomainReplacements = (AuthGateway.Shared.Serializer.Generic.Deserialize<ManualDomainReplacements>(tmpString));
				else
					this.ManualDomainReplacements = new ManualDomainReplacements() { new DomainReplacement() { Name = "" } };

				this.AuthEngineRemoveUsersAfterXHours = getInt32OrDef(getItemFromXML(doc, AUTHENGINE_REMOVEUSERS_AFTER_X_HOURS), this.AuthEngineRemoveUsersAfterXHours);

				tmpString = getItemFromXML(doc, AUTHENGINE_DEFAULT_DOMAIN);
				if (!string.IsNullOrEmpty(tmpString))
					this.AuthEngineDefaultDomain = tmpString;

				this.NotifyPinCodeIncorrectOnAccess = getThisBoolOrDef(getItemFromXML(doc, NOTIFYPINCODEINCORRECTONACCESS), this.NotifyPinCodeIncorrectOnAccess);

				this.PinTanMaxSheets = getInt32OrDef(getItemFromXML(doc, PINTAN_MAX_SHEETS), this.PinTanMaxSheets);

				this.MinTimeBetweenRadiusRequestsPerUser = getInt32OrDef(getItemFromXML(doc, MINTIMEBETWEENRADIUSREQUESTSPERUSER), this.MinTimeBetweenRadiusRequestsPerUser);

				this.AuthEngineChallengeResponse = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_CHALLENGERESPONSE), this.AuthEngineChallengeResponse);

				FieldMissingErrorEmail = getStringOrDef(getItemFromXML(doc, FIELD_MISSING_ERROR_EMAIL), FieldMissingErrorEmail);
				FieldMissingErrorMobilePhone = getStringOrDef(getItemFromXML(doc, FIELD_MISSING_ERROR_MOBILE_PHONE), FieldMissingErrorMobilePhone);
				
				this.LogMaxFiles = getInt32OrDef(getItemFromXML(doc, LOGMAXFILES), this.LogMaxFiles);
				this.LogMaxSize = getInt32OrDef(getItemFromXML(doc, LOGMAXSIZE), this.LogMaxSize);

				//this.AuthEngineLogLevel = getEnumValue<Log.LogLevel>(getItemFromXML(doc, AUTHENGINE_LOG));
				this.AuthEngineEventLogLevel = getEnumValue<Log.LogLevel>(getItemFromXML(doc, AUTHENGINE_EVENTLOG));
				this.AuthEngineFileLogLevel = getEnumValue<Log.LogLevel>(getItemFromXML(doc, AUTHENGINE_FILELOG));
				AuthEngineFlushOnWrite = getThisBoolOrDef(getItemFromXML(doc, AUTHENGINE_LOGFLUSHONWRITE), AuthEngineFlushOnWrite);
				//this.CloudSMSLogLevel = getEnumValue<Log.LogLevel>(getItemFromXML(doc, CLOUDSMS_LOG));
				this.CloudSMSEventLogLevel = getEnumValue<Log.LogLevel>(getItemFromXML(doc, CLOUDSMS_EVENTLOG));
				this.CloudSMSFileLogLevel = getEnumValue<Log.LogLevel>(getItemFromXML(doc, CLOUDSMS_FILELOG));

				this.CloudSMSFlushOnWrite = getThisBoolOrDef(getItemFromXML(doc, CLOUDSMS_LOGFLUSHONWRITE), this.CloudSMSFlushOnWrite);

				this.AuthEngineKeyBase = getEnumValue<RKGBase>(getItemFromXML(doc, AUTHENGINE_KEYBASE));

				this.CloudSMSUseEncryption = getThisBoolOrDef(getItemFromXML(doc, CLOUDSMS_USEENCRYPTION), this.CloudSMSUseEncryption);

				tmpString = getItemFromXML(doc, OATHCALC_SERVERIP);
				if (!string.IsNullOrEmpty(tmpString))
					_oathCalcServerIp = IPAddress.Parse(tmpString);

				this.OATHCalcServerPort = getInt32OrDef(getItemFromXML(doc, OATHCALC_SERVERPORT), this.OATHCalcServerPort);

				this.OATHCalcUseEncryption = getThisBoolOrDef(getItemFromXML(doc, OATHCALC_USEENCRYPTION), this.OATHCalcUseEncryption);

				this.OATHCalcTotpWindow = getInt32OrDef(getItemFromXML(doc, OATHCALC_TOTP_WINDOW), this.OATHCalcTotpWindow);
				this.OATHCalcHotpAfterWindow = getInt32OrDef(getItemFromXML(doc, OATHCALC_HOTP_AFTERWINDOW), this.OATHCalcHotpAfterWindow);

				this.CloudSMSTokenExpireTime = getInt32OrDef(getItemFromXML(doc, CLOUDSMS_TOKEN_EXPIRE_TIME_MINUTES), this.CloudSMSTokenExpireTime);
				OneTimeTokenExpireTimeMinutes = getInt32OrDef(getItemFromXML(doc, ONETIME_TOKEN_EXPIRE_TIME_MINUTES), OneTimeTokenExpireTimeMinutes);
				EmergencyTokenExpireTimeMinutes = getInt32OrDef(getItemFromXML(doc, EMERGENCY_TOKEN_EXPIRE_TIME_MINUTES), EmergencyTokenExpireTimeMinutes);

				this.RadiusChallengeMessage = getItemFromXML(doc, RADIUS_CHALLENGE_MESSAGE);
				this.RadiusShowPin = getThisBoolOrDef(getItemFromXML(doc, RADIUS_SHOW_PIN), this.RadiusShowPin);
				this.AdminGUIConfirmPincode = getThisBoolOrDef(getItemFromXML(doc, ADMINGUI_CONFIRM_PINCODE), this.AdminGUIConfirmPincode);
				this.CitrixWITitle = getStringOrDef(getItemFromXML(doc, CITRIX_WI_TITLE), this.CitrixWITitle);

                this.SendTrackingInfo = getBoolOrDef(getItemFromXML(doc, SEND_TRACKING_INFO), this.SendTrackingInfo);

				tmpString = getXmlFromXML(doc, AUTHPROVIDERS);
				if (!string.IsNullOrEmpty(tmpString))
					try
					{
						this.Providers = (AuthGateway.Shared.Serializer.Generic.Deserialize<AuthProviders>(tmpString));
					}
					catch //(Exception ex)
					{
#if DEBUG
						//MessageBox.Show("AuthProviders: " + ex.Message + ex.StackTrace);
#endif
					}

				tmpString = getXmlFromXML(doc, EMAILCONFIG);
				if (!string.IsNullOrEmpty(tmpString))
					try
					{
						this.EmailConfig = (Generic.Deserialize<EmailConfig>(tmpString));
						if (this.EmailConfig.NeedsEncryption)
							rewriteSystemConfiguration = true;
					}
					catch //(Exception ex)
					{
#if DEBUG
						//MessageBox.Show("EmailConfig: " + ex.Message + ex.StackTrace);
#endif
					}

				tmpString = getXmlFromXML(doc, "CloudSMSConfiguration");
				if (!string.IsNullOrEmpty(tmpString))
					this.CloudSMSConfiguration = (Generic.Deserialize<CloudSMSConfiguration>(tmpString));

				if (this.CloudSMSConfiguration.NeedsEncryption)
					rewriteSystemConfiguration = true;

				string k;
				// Allow overriding of keys
				k = getItemFromXML(doc, AUTHENGINE_PRIVATE_RSAKEY).Trim();
				if (!string.IsNullOrEmpty(k))
					_authEnginePrivateRsaKey = k;
				k = getItemFromXML(doc, AUTHENGINE_PUBLIC_RSAKEY).Trim();
				if (!string.IsNullOrEmpty(k))
					_authEnginePublicRsaKey = k;

				k = getItemFromXML(doc, CLOUDSMS_PRIVATE_RSAKEY).Trim();
				if (!string.IsNullOrEmpty(k))
					_cloudSMSPrivateRsaKey = k;
				k = getItemFromXML(doc, CLOUDSMS_PUBLIC_RSAKEY).Trim();
				if (!string.IsNullOrEmpty(k))
					_cloudSMSPublicRsaKey = k;

				k = getItemFromXML(doc, OATHCALC_PRIVATE_RSAKEY).Trim();
				if (!string.IsNullOrEmpty(k))
					_oathCalcPrivateRsaKey = k;
				k = getItemFromXML(doc, OATHCALC_PUBLIC_RSAKEY).Trim();
				if (!string.IsNullOrEmpty(k))
					_oathCalcPublicRsaKey = k;

				this.ErrorParsingConfiguration = false;

				return rewriteSystemConfiguration;
			}
			catch (Exception ex)
			{
				this.ErrorParsingConfiguration = true;
				throw new SystemConfigurationParseError("WARNING: Error parsing configuration file, some values may be set to default value.", ex);
			}
		}

		private string getStringOrDef(string toParse, string defVal)
		{
			if (string.IsNullOrEmpty(toParse))
				return defVal;
			return toParse;
		}

		private string decryptIfNecessary(string text, string salt)
		{
			return CryptoHelper.DecryptSettingIfNecessary(text, salt);
		}

		public bool getThisBoolOrDef(string toParse, bool defVal) {
			return getBoolOrDef(toParse, defVal, this);
		}
		
		public static bool getBoolOrDef(string toParse, bool defVal, SystemConfiguration sc = null)
		{
			var ret = defVal;
			if (string.IsNullOrEmpty(toParse))
				return ret;
			var toParseOnOff = toParse.ToLower();
			if (toParseOnOff == "on")
			{
				if (sc != null)
					sc.OnOffFound = true;
				return true;
			}
			if (toParseOnOff == "off")
			{
				if (sc != null)
					sc.OnOffFound = true;
				return false;
			}
			if (Boolean.TryParse(toParse, out ret))
				return ret;
			return defVal;
		}

		public static int getInt32OrDef(string toParse, Int32 defVal)
		{
			var ret = defVal;
			if (string.IsNullOrEmpty(toParse))
				return ret;
			if (Int32.TryParse(toParse, out ret))
				return ret;
			return defVal;
		}
        
        public static int getEnumIntOrDef(Type type, string toParse, Int32 defVal)
		{
			var ret = defVal;
            if (!string.IsNullOrEmpty(toParse)) {
                try {
                    ret = (int)Enum.Parse(type, toParse, false);
                }
                catch {

                }
            }
            return ret;
		}

		public PinCodeOption getThisPinCodeOption(string toParse)
		{
			return getPinCodeOption(toParse, this);
		}
		
		public static PinCodeOption getPinCodeOption(string toParse, SystemConfiguration sc = null)
		{
			var toParseOnOff = toParse.ToLower();
			if (toParseOnOff == "on")
			{
				if (sc != null)
					sc.OnOffFound = true;
				return PinCodeOption.True;
			}
			if (toParseOnOff == "off")
			{
				if (sc != null)
					sc.OnOffFound = true;
				return PinCodeOption.False;
			}
			return getEnumValue<PinCodeOption>(toParse);
		}

		public void SetDBData(string dbServer, string dbUsername
				, string dbPassword, string dbPort, bool dbUseNamedPipes, string dbPipeName, bool dbUseIS, string dbName)
		{
			string error = string.Empty;
			if (string.IsNullOrEmpty(dbServer))
				error += "Invalid server\n";
			else
				this.DbServer = dbServer;
			this.DbUsername = dbUsername;
			this.DbPassword = dbPassword;
			this.DbPort = getInt32OrDef(dbPort, this.DbPort);
			if (string.IsNullOrEmpty(dbName))
				error += "Invalid database name\n";
			else
				this.DatabaseName = dbName;
			this.DbUseIntegratedSecurity = dbUseIS;
            this.DbPipeName = dbPipeName;
			this.DbUsePipes = dbUseNamedPipes;

			if (!string.IsNullOrEmpty(error))
				throw new Exception(error);
		}

		public void setServicesData(string authEngineAddress, string authEnginePort
				, string smsAddress, string smsPort)
		{
			string error = string.Empty;
			if (!IPAddress.TryParse(authEngineAddress, out this._authEngineServerIp))
				error += "Invalid AuthEngine Address\n";

			this.AuthEngineServerPort = getInt32OrDef(authEnginePort, this.AuthEngineServerPort);
			if (!IPAddress.TryParse(smsAddress, out this._smsServerIp))
				error += "Invalid CloudSMS Address\n";
			this.CloudSMSServerPort = getInt32OrDef(smsPort, this.CloudSMSServerPort);
			if (!string.IsNullOrEmpty(error))
				throw new Exception(error);
		}
		
		public bool StopServiceOnException { get; set; }

		public string DbServer { get; set; }
		public int DbPort
		{
			get;
			set;
		}
		public string DbUsername { get; set; }
		public string DbPassword
		{
			get;
			set;
		}

		public bool DbUseIntegratedSecurity
		{
			get;
			set;
		}

		public bool DbUsePipes
		{
			get;
			set;
		}

        public string DbPipeName
        {
            get;
            set; 
        }

		public string DatabaseName
		{
			get;
			set;
		}

		public string GetSQLConnectionString(bool useInitialCatalog = true)
		{
			string dataSource = this.DbServer + "," + this.DbPort;
            if (this.DbUsePipes)
            {               
                if (!String.IsNullOrEmpty(this.DbPipeName) && this.DbPipeName != @"\sql\query")
                    dataSource = this.DbPipeName;
                else
                    dataSource = this.DbServer; // local
            }

			string stringConnection = string.Empty;
			if (this.DbUseIntegratedSecurity)
				stringConnection = "Integrated Security = SSPI; Data Source = " + dataSource + ";";
			else
				stringConnection = "Data Source=" + dataSource + ";User Id=" + this.DbUsername + ";Password=" + this.DbPassword + ";";
			if (useInitialCatalog)
				stringConnection += "Initial Catalog = " + this.DatabaseName + ";";
			return stringConnection;
		}

		public IPAddress getAuthEngineServerAddress() { return _authEngineServerIp; }
		public IPAddress AuthEngineServerAddress
		{
			get { return _authEngineServerIp; }
			set { _authEngineServerIp = value; }
		}

		public int AuthEngineServerPort
		{
			get;
			set;
		}

		public IPAddress getSMSServerAddress() { return _smsServerIp; }
		public int CloudSMSServerPort
		{
			get;
			set;
		}

		public string ADServerAddress
		{
			get;
			set;
		}
		public string ADUsername
		{
			get;
			set;
		}
		public string ADPassword
		{
			get;
			set;
		}

		public int ADDisasterPercentage {
			get;
			set;
		}
		
		public int LogMaxFiles { get; set; }
		public int LogMaxSize { get; set; }

		public Log.LogLevel AuthEngineLogLevel
		{
			get;
			set;
		}
		public Log.LogLevel AuthEngineEventLogLevel
		{
			get;
			set;
		}
		public Log.LogLevel AuthEngineFileLogLevel
		{
			get;
			set;
		}

		public Log.LogLevel CloudSMSLogLevel
		{
			get;
			set;
		}
		public Log.LogLevel CloudSMSEventLogLevel
		{
			get;
			set;
		}
		public Log.LogLevel CloudSMSFileLogLevel
		{
			get;
			set;
		}

		public bool BaseSendTokenTestMode
		{
			get;
			set;
		}

		public RKGBase AuthEngineKeyBase
		{
			get;
			set;
		}

		public bool AuthEngineUseEncryption
		{
			get;
			set;
		}
		public bool CloudSMSUseEncryption
		{
			get;
			set;
		}

		public string getAuthEnginePublicKey() { return _authEnginePublicRsaKey; }
		public string getAuthEnginePrivateKey() { return _authEnginePrivateRsaKey; }
		public void setAuthEnginePublicKey(string key) { _authEnginePublicRsaKey = key; }
		public void setAuthEnginePrivateKey(string key) { _authEnginePrivateRsaKey = key; }

		public string getCloudSMSPublicKey() { return _cloudSMSPublicRsaKey; }
		public string getCloudSMSPrivateKey() { return _cloudSMSPrivateRsaKey; }
		public void setCloudSMSPublicKey(string key) { _cloudSMSPublicRsaKey = key; }
		public void setCloudSMSPrivateKey(string key) { _cloudSMSPrivateRsaKey = key; }

		public IPAddress OATHCalcServerIp
		{
			get { return _oathCalcServerIp; }
			set { _oathCalcServerIp = value; }
		}
		public int OATHCalcServerPort
		{
			get;
			set;
		}
		public bool OATHCalcUseEncryption
		{
			get;
			set;
		}
		public String OATHCalcPublicKey
		{
			get { return _oathCalcPublicRsaKey; }
			set { _oathCalcPublicRsaKey = value; }
		}
		public String OATHCalcPrivateKey
		{
			get { return _oathCalcPrivateRsaKey; }
			set { _oathCalcPrivateRsaKey = value; }
		}

		public AuthProviders Providers
		{
			get;
			set;
		}

		public int OATHCalcTotpWindow
		{
			get;
			set;
		}

		public int OATHCalcHotpAfterWindow
		{
			get;
			set;
		}

		public bool AuthEngineDefaultEnabled
		{
			get;
			set;
		}

		public bool AuthEngineAllowEmergencyPasscode
		{
			get;
			set;
		}

		public bool AuthEngineOverrideWithADInfo
		{
			get;
			set;
		}

		public bool AuthEngineLockDownMode
		{
			get;
			set;
		}

		public bool AuthEngineFlushOnWrite { get; set; }
		
		public bool CloudSMSFlushOnWrite
		{
			get;
			set;
		}

		public PinCodeOption AuthEnginePinCode
		{
			get;
			set;
		}

		public int AuthEnginePinCodeLength
		{
			get;
			set;
		}

		public AuthEngineDefaultExceptionGroups AuthEngineDefaultExceptionGroups
		{
			get;
			set;
		}

		public int AuthEngineRemoveUsersAfterXHours
		{
			get;
			set;
		}

		public string AuthEngineDefaultDomain
		{
			get;
			set;
		}

		public bool NotifyPinCodeIncorrectOnAccess
		{
			get;
			set;
		}

		public int PinTanMaxSheets
		{
			get;
			set;
		}

		public int MinTimeBetweenRadiusRequestsPerUser
		{
			get;
			set;
		}

		public int PinTanColumns
		{
			get;
			set;
		}

		public int PinTanRows
		{
			get;
			set;
		}

		//public PinCodeTokenSeparated AuthEnginePinCodeTokenSeparated
		private bool _AuthEngineChallengeResponse;
		public bool AuthEngineChallengeResponse
		{
			get
			{
				return _AuthEngineChallengeResponse;
			}
			set
			{
				_AuthEngineChallengeResponse = value;
				AuthEngineStateless = !_AuthEngineChallengeResponse;
			}
		}

		public bool AuthEngineStateless
		{
			get;
			private set;
		}

		public int CloudSMSTokenExpireTime
		{
			get; 
			set;
		}
		
		public int OneTimeTokenExpireTimeMinutes { get; set; }
		public int EmergencyTokenExpireTimeMinutes { get; set; }

		public int PintanPasscodeLength
		{
			get;
			set;
		}

		#region Helpers

		private void writeSetting(XmlWriter writer, string element, string value)
		{
			writer.WriteStartElement(element);
			writer.WriteValue(value);
			writer.WriteFullEndElement();
		}

		private void writeRawSetting(XmlWriter writer, string element, string value)
		{
			writer.WriteStartElement(element);
			writer.WriteRaw(value);
			writer.WriteFullEndElement();
		}

		private void writeRaw(XmlWriter writer, string value)
		{
			writer.WriteRaw(value);
		}

		private string getItemFromXML(XmlDocument doc, string itemname)
		{
			XmlNodeList list = doc.GetElementsByTagName(itemname);
			foreach (XmlElement item in list)
			{
				return item.InnerText;
			}
			return string.Empty;
		}

		private string getXmlFromXML(XmlDocument doc, string itemname)
		{
			XmlNodeList list = doc.GetElementsByTagName(itemname);
			foreach (XmlElement item in list)
				return item.OuterXml;
			return string.Empty;
		}

		/*
		 * Use [DefaultValue(ENUMVALUE)] when using this function
		 */
		public static T getEnumValue<T>(string value)
		{
			if (!Enum.IsDefined(typeof(T), value))
				return default(T);
			try
			{
				return (T)Enum.Parse(typeof(T), value, true);
			}
			catch (Exception ex)
			{
				Log.Logger.Instance.WriteToLog("Invalid enum value: " + ex.Message, Log.LogLevel.Error);
			}
			return default(T);
		}

		#endregion

		public EmailConfig EmailConfig
		{
			get;
			set;
		}

		public string RadiusChallengeMessage { get; set; }

		public string ADContainer { get; set; }
		public string ADBaseDN { get; set; }
		public string ADFilter { get; set; }
		public string ADAdminBaseDNOverride { get; set; }
		public string ADAdminFilterOverride { get; set; }

		public int AuthEngineUsersPollInterval { get; set; }
        public int AuthEngineHeartbeatInterval { get; set; }
        public int AuthEngineWaitingInterval { get; set; }

        public PollingPreference AuthEnginePollingPreference { get; set; }
		
		public bool AuthEngineAskMissingInfo { get; set; }
		public bool AuthEngineAskPin { get; set; }
		public bool AuthEngineAskProviderInfo { get; set; }
		public int AuthEngineAskAgedTimeHours { get; set; }
		
		public int AuthEngineAutoSetupAgedTimeHours { get; set; }
		
		public string VaultPassword { get; set; }
		
		public bool AuthEnginePinCodePanic { get; set; }

        public bool AuthEngineMutualAuth { get; set; }

        public string AuthEngineEncryptionKey { get; set; }

        public bool OATHCalcUseDefaults { get; set; }

		public CloudSMSConfiguration CloudSMSConfiguration { get; set; }

		public bool AuthEngineDisableUserVerification { get; set; }

		public ExtraDCs ExtraDCs { get; set; }

		public ManualDomainReplacements ManualDomainReplacements { get; set; }

		public bool RadiusShowPin { get; set; }

		public bool AdminGUIConfirmPincode { get; set; }

		public string CitrixWITitle { get; set; }

		public string FieldMissingErrorMobilePhone { get; set; }

		public string FieldMissingErrorEmail { get; set; }
		
		public string GetChecksum() {
			return CryptoHelper.GetSHA1String(WriteXMLCredentialsToString());
		}
		
		public bool PasswordVaulting { get; set; }

        public bool AuthEngineAllowUPNLogin { get; set; }

        public bool AuthEngineAllowPre2000Login { get; set; }

        public bool AuthEngineAllowEmailLogin { get; set; }

        public bool AuthEngineAllowMobileNumberLogin { get; set; }

        public bool AuthEngineAllowAliasesInLogin { get; set; }

        public bool AuthEngineSAMLoginPreferred { get; set; }

        public PollingPreference MutualAuthImagesPollingPreference { get; set; }

        public string OATHCalcDefaultConfig { get; set; }
        public bool SendTrackingInfo { get; set; }
	}

    [DefaultValue(Default)]
    public enum PollingPreference
    {
        MostPreferred = 0,
        Preferred = 1,
        Default = 2,
        NotPreferred = 3
    }

	[DefaultValue(False)]
	public enum PinCodeOption
	{
		False,
		True,
		Enforced,
		AD
	}

	[DefaultValue(Base10)]
	public enum RKGBase
	{
		Base10,
		Base32
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class ExtraDCs : List<DC>
	{
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	[DefaultProperty("Name")]
	public class DC
	{
		public DC() { this.Name = string.Empty; }
		public DC(string name) { this.Name = name; }
		public string Name { get; set; }
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class ManualDomainReplacements : List<DomainReplacement>
	{
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	[DefaultProperty("Name")]
	public class DomainReplacement
	{
		public DomainReplacement() { this.Name = string.Empty; }
		public DomainReplacement(string name) { this.Name = name; }
		public string Name { get; set; }
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class AuthProviders : List<Provider>
	{
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class AuthEngineDefaultExceptionGroups : List<Group>
	{
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class CloudSMSModules : List<CloudSMSModuleConfig>
	{
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class Group
	{
		public string Name { get; set; }
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class Provider
	{
		public Provider() { 
			FriendlyName = string.Empty;
			AutoSetup = false;
		}
		[XmlIgnore]
		public int Id { get; set; }
		public string FriendlyName { get; set; }
		public string Name { get; set; }
		public string AdGroup { get; set; }
		public bool Enabled { get; set; }
		public bool Default { get; set; }
		public bool AutoSetup { get; set; }
		public string Config { get; set; }
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class EmailConfig : IXmlSerializationCallback
	{
		[XmlIgnore]
		public bool NeedsEncryption = false;

		public string Server { get; set; }
		public int Port { get; set; }
		public bool UseAuth { get; set; }
		public bool EnableSSL { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string From { get; set; }
		public string MessageTitle { get; set; }

		public void OnXmlDeserialized()
		{
			try
			{
				var currentValue = Password;
				Password = CryptoHelper.DecryptSettingIfNecessary(Password, "EmailPassword");
				if (Password == currentValue)
					NeedsEncryption = true;
			}
			catch { }
		}

		public void OnXmlSerializing()
		{
			var currentPassword = Password;
			try
			{
				Password = CryptoHelper.EncryptSettingIfNecessary(Password, "EmailPassword");
			}
			catch
			{
				Password = currentPassword;
			}
		}

		public void OnXmlSerialized()
		{
			try
			{
				Password = CryptoHelper.DecryptSettingIfNecessary(Password, "EmailPassword");
			}
			catch { }
		}
	}

	public enum UrlMethod
	{
		POST,
		GET
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class ModuleParameters : Collection<ModuleParameter>
	{
		public ModuleParameter GetByName(string name)
		{
			foreach (var parameter in this.Items)
			{
				if (parameter.Name == name)
					return parameter;
			}
			return null;
		}
		public List<ModuleParameter> GetInputParameters()
		{
			return GetInputParameters(new List<string>());
		}
		public List<ModuleParameter> GetInputParameters(List<string> skipParameters)
		{
			var ret = new List<ModuleParameter>();
			foreach (var parameter in this.Items)
			{
				if (!skipParameters.Contains(parameter.Name) && !parameter.Output)
					ret.Add(parameter);
			}
			return ret;
		}
		public List<ModuleParameter> GetOutputParameters()
		{
			var ret = new List<ModuleParameter>();
			foreach (var parameter in this.Items)
			{
				if (parameter.Output)
					ret.Add(parameter);
			}
			return ret;
		}
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class ModuleParameter
	{
		[XmlAttribute("name")]
		public string Name { get; set; }
		[XmlAttribute("value")]
		public string Value { get; set; }
		[DefaultValue(false)]
		[XmlAttribute("encrypt")]
		public bool Encrypt { get; set; }
		[DefaultValue(false)]
		[XmlAttribute("output")]
		public bool Output { get; set; }
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class CloudSMSModuleConfig : IXmlSerializationCallback
	{
		[XmlIgnore]
		public bool NeedsEncryption = false;
		
		public const string TWILIO = "Twilio";
		public const string REGEXP = "Regexp";
		public const string TEXTLOCAL = "Textlocal";
		public const string CMDIRECT = "CMDirect";
		//public const string CLICKATELL = "Clickatell";
		public const string GSM = "GSM";
		
		public static List<string> Modules = new List<String>() { 
			TWILIO, 
			REGEXP, 
			TEXTLOCAL,
			CMDIRECT,
			GSM,
		};

		public string TypeName { get; set; }

		private string moduleName;
		public string ModuleName {
			get {
				if (string.IsNullOrEmpty(this.moduleName))
					this.moduleName = this.TypeName;
				return this.moduleName;
			}
			set { this.moduleName = value; }
		}

		[XmlArrayItem(Type = typeof(ModuleParameter))]
		public ModuleParameters ModuleParameters { get; set; }

		public void OnXmlDeserialized()
		{
			try
			{
				if (this.ModuleParameters == null)
					return;
				foreach (var parameter in this.ModuleParameters)
				{
					if (!string.IsNullOrEmpty(parameter.Value))
					{
						var currentValue = parameter.Value;
						parameter.Value = CryptoHelper.DecryptSettingIfNecessary(parameter.Value, "ModulePassword");
						if (parameter.Encrypt && parameter.Value == currentValue)
							NeedsEncryption = true;
					}
				}
			}
			catch { }
		}

		public void OnXmlSerializing()
		{
			if (this.ModuleParameters == null)
				return;
			foreach (var parameter in this.ModuleParameters)
			{
				if (parameter.Encrypt && !string.IsNullOrEmpty(parameter.Value))
				{
					var currentValue = parameter.Value;
					try
					{
						parameter.Value = CryptoHelper.EncryptSettingIfNecessary(parameter.Value, "ModulePassword");
					}
					catch
					{
						parameter.Value = currentValue;
					}
				}
			}
		}

		public void OnXmlSerialized()
		{
			if (this.ModuleParameters == null)
				return;
			foreach (var parameter in this.ModuleParameters)
			{
				if (parameter.Encrypt && !string.IsNullOrEmpty(parameter.Value))
				{
					try
					{
							parameter.Value = CryptoHelper.DecryptSettingIfNecessary(parameter.Value, "ModulePassword");
						}
					catch { }
				}
			}
		}
	}

	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class CloudSMSConfiguration : IXmlSerializationCallback
	{
		[XmlIgnore]
		public bool NeedsEncryption = false;

		public CloudSMSModules CloudSMSModules { get; set; }

		public void OnXmlDeserialized()
		{
			if (this.CloudSMSModules == null || this.CloudSMSModules.Count == 0)
				return;
			foreach (var module in this.CloudSMSModules)
			{
				module.OnXmlDeserialized();
				if (module.NeedsEncryption)
					NeedsEncryption = true;
			}
		}

		public void OnXmlSerializing()
		{
			if (this.CloudSMSModules == null || this.CloudSMSModules.Count == 0)
				return;
			foreach (var module in this.CloudSMSModules)
			{
				module.OnXmlSerializing();
			}
		}

		public void OnXmlSerialized()
		{
			if (this.CloudSMSModules == null || this.CloudSMSModules.Count == 0)
				return;
			foreach (var module in this.CloudSMSModules)
			{
				module.OnXmlSerialized();
			}
		}
	}
}
