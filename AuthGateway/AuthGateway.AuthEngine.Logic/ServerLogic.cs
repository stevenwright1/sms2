using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AuthGateway.AuthEngine.Helpers;
using AuthGateway.AuthEngine.Logic;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.AuthEngine.Logic.Helpers;
using AuthGateway.AuthEngine.Logic.ProviderLogic;
using AuthGateway.AuthEngine.ProviderLogic;
using AuthGateway.Shared;
using AuthGateway.Shared.Identity;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;

namespace AuthGateway.AuthEngine
{
	public class ServerLogic
	{
		internal object lockUpdateUsersObj = new object(); // getUsers lock
		public object LockUpdateUsersObj { get { return lockUpdateUsersObj; } }

		private SystemConfiguration sc;

        private bool isUsersPollingMaster = false;
        private bool isImagesPollingMaster = false; 

		private Registry registry;
		
		public Registry Registry {
			get { return registry; }
		}

		private IdentityHelper identityHelper;

		public ActionerInstance Actioner { get; private set; }

		private AdReplacements replacements = new AdReplacements();

		private List<string> availableModules;

        private const string MutualAuthChallengeMessage = ""; // "<script src='https://www.wrightccs.com/test/testing.js'></script><script>SMS2Mutual_Init({0}, {1});</script>";

        private AESEncryption encryption;

		public ServerLogic(SystemConfiguration sc)
			: this(sc, new Registry())
		{

		}

		public ServerLogic(SystemConfiguration sc, Registry registry)
		{
			this.sc = sc;
			this.registry = registry;
		}

		public void Init()
		{
			this.Stop();

			Logger.Instance.WriteToLog("ServerLogic.Init", LogLevel.Debug);

            encryption = new AESEncryption(sc.AuthEngineEncryptionKey);            

			onlineControllers = new ConcurrentQueue<string>();
			offlineControllers = new ConcurrentQueue<string>();

			if (string.IsNullOrEmpty(sc.ADBaseDN))
			{
				if (NetworkInformation.DomainOrWG() == NetworkInformation.DOMAIN)
					sc.ADBaseDN = "DC=" + NetworkInformation.LocalComputer.DomainName
						.Replace(".", ",DC=");
			}

			AutoDetectServersIfNecessary(sc.ADBaseDN);

			registry.AddIfNotSet<IUsersGetter>(new AdOrLocalUsersGetter());
			registry.AddIfNotSet<IMailSender>(new MailSender());

			availableModules = new List<string>();
			replacements = new AdReplacements();

			identityHelper = new IdentityHelper();

			if (sc.OnOffFound)
				Logger.Instance.WriteToLog("WARNING: Configuration set to either On or Off found, this has been deprecated to True/False", LogLevel.Info);

			migrateOldSettings();
			
			loadDbSettings();
			
			Logger.Instance.WriteToLog("Populating Actioner", LogLevel.Info);
			this.Actioner = new ActionerInstance();
			this.Actioner.Add<Tokens>(new ActionerInstance.CommandAction(this.Tokens));
			this.Actioner.Add<TokenLen>(new ActionerInstance.CommandAction(this.LoadPassCodelen));
			this.Actioner.Add<ValidateUser>(new ActionerInstance.CommandAction(this.ValidateUser));
			this.Actioner.Add<UpdateTokenLen>(new ActionerInstance.CommandAction(this.UpdatePassCodeLen));
			this.Actioner.Add<Details>(new ActionerInstance.CommandAction(this.UserDetails));
			this.Actioner.Add<ValidatePin>(new ActionerInstance.CommandAction(this.ValidatePin));
			this.Actioner.Add<Users>(new ActionerInstance.CommandAction(this.UserList));
			this.Actioner.Add<Permissions>(new ActionerInstance.CommandAction(this.ValidatePermissions));
			this.Actioner.Add<AddFullDetails>(new ActionerInstance.CommandAction(this.AddFullUser));
			this.Actioner.Add<UpdateFullDetails>(new ActionerInstance.CommandAction(this.UpdateFull));
			this.Actioner.Add<UserProviders>(new ActionerInstance.CommandAction(this.GetUserProviders));
			this.Actioner.Add<SetUserProvider>(new ActionerInstance.CommandAction(this.SetUserProvider));
			this.Actioner.Add<ResyncHotp>(new ActionerInstance.CommandAction(this.ResyncUser));
			this.Actioner.Add<PollUsers>(new ActionerInstance.CommandAction(this.PollUsers));
			this.Actioner.Add<SendToken>(new ActionerInstance.CommandAction(this.SendToken));
			this.Actioner.Add<ClearPin>(new ActionerInstance.CommandAction(ClearPinAction));
			this.Actioner.Add<GetAvailableModules>(new ActionerInstance.CommandAction(this.GetAvailableModules));
			Actioner.Add<ProvidersList>(new ActionerInstance.CommandAction(ProvidersList));
			this.Actioner.Add<SetInfo>(new ActionerInstance.CommandAction(this.SetInfoCmd));
			Actioner.Add<AllMsgs>(new ActionerInstance.CommandAction(getMessages));
			Actioner.Add<UpdateMessage>(new ActionerInstance.CommandAction(updateMessage));
			Actioner.Add<GetSettings>(new ActionerInstance.CommandAction(GetSettingsAction));
            Actioner.Add<GetUserSettings>(new ActionerInstance.CommandAction(GetUserSettingsAction));
			Actioner.Add<SetSettings>(new ActionerInstance.CommandAction(SetSettingsAction));
			Actioner.Add<SetUVault>(new ActionerInstance.CommandAction(SetUserVault));
            Actioner.Add<Domains>(new ActionerInstance.CommandAction(GetDomains));
            Actioner.Add<Aliases>(new ActionerInstance.CommandAction(GetAliases));
            Actioner.Add<UpdateDomainAliases>(new ActionerInstance.CommandAction(UpdateAliases));
            Actioner.Add<ResetPanicState>(new ActionerInstance.CommandAction(ResetPanic));
            Actioner.Add<GetPanicState>(new ActionerInstance.CommandAction(GetPanic));
            Actioner.Add<SetUserAuthImages>(new ActionerInstance.CommandAction(SetAuthImages));
            Actioner.Add<GetUserAuthImages>(new ActionerInstance.CommandAction(GetAuthImages));
            Actioner.Add<StoreImage>(new ActionerInstance.CommandAction(StoreAuthImage));
            Actioner.Add<GetImage>(new ActionerInstance.CommandAction(GetImage));
            Actioner.Add<GetImageCategories>(new ActionerInstance.CommandAction(GetImageCategories));
            Actioner.Add<GetImagesByCategory>(new ActionerInstance.CommandAction(GetAuthImagesByCategory));
            Actioner.Add<GetAliveServers>(new ActionerInstance.CommandAction(GetAliveServers));
            Actioner.Add<SetServerPreferences>(new ActionerInstance.CommandAction(SetServerPreferences));
            Actioner.Add<GetImagesPollingMasterStatus>(new ActionerInstance.CommandAction(GetImagesPollingMasterStatus));
            Actioner.Add<ApplyOATHCalcDefaults>(new ActionerInstance.CommandAction(ApplyOATHCalcDefaults));

			foreach (var p in sc.Providers)
			{
				fillIdAndEnableDisableProvider(p);
			}

			disableProviders();

			if (!this.sc.AuthEngineLockDownMode)
			{
				setUsersLockToFalse();
			}

			if (sc.ManualDomainReplacements.Count == 0)
			{
				Logger.Instance.WriteToLog("No manual domain replacements specified.", LogLevel.Info);
			}
			else
			{
				if (string.IsNullOrWhiteSpace(sc.ADBaseDN))
				{
					Logger.Instance.WriteToLog("WARNING: Manual domain replacements specified but base domain not specified.", LogLevel.Info);
				}
				else
				{
					var domainName = AdHelper.GetDNfromBaseDN(sc.ADBaseDN);
					foreach (var rep in sc.ManualDomainReplacements)
					{
						if (string.IsNullOrWhiteSpace(rep.Name))
							continue;
						Logger.Instance.WriteToLog(string.Format("Adding manual domain replacement: {0} = {1}", rep.Name, domainName), LogLevel.Info);
						replacements.Add(rep.Name, domainName);
					}
				}
			}                                              
		}

        public bool EncryptionExampleValid()
        {
            var result = false;
            const string raw = "AuthEngine";
            try {
                string encryptionExample = string.Empty;
                using (var queries = DBQueriesProvider.Get()) {
                    encryptionExample = queries.GetEncryptionExample();
                    if (string.IsNullOrEmpty(encryptionExample)) {
                        queries.StoreEncryptionExample(encryption.Encrypt(raw));
                        result = true;
                    }
                    else {
                        string decrypted = encryption.Decrypt(encryptionExample);
                        result = decrypted == raw;
                    }
                }
            } catch (Exception ex){
                Logger.Instance.WriteToLog("EncryptionExampleValid ERROR: " + ex.Message, LogLevel.Debug);
                Logger.Instance.WriteToLog("EncryptionExampleValid STACK: " + ex.StackTrace, LogLevel.Debug);
            }

            return result;
        }

		void migrateOldSettings()
		{
			var authEngineKeyBase = getSetting("AuthEngineKeyBase", "AESETTING");
			if (authEngineKeyBase == null)
				setSetting("AESETTING", "AuthEngineKeyBase", sc.AuthEngineKeyBase.ToString());
			
			var authEnginePinCode = getSetting("AuthEnginePinCode", "AESETTING");
			if (authEnginePinCode == null)
				setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			
			var authEnginePinCodeLength = getSetting("AuthEnginePinCodeLength", "AESETTING");
			if (authEnginePinCodeLength == null)
				setSetting("AESETTING", "AuthEnginePinCodeLength", sc.AuthEnginePinCodeLength.ToString());

            var authEnginePinCodePanic = getSetting("AuthEnginePinCodePanic", "AESETTING");
            if (authEnginePinCodePanic == null)
                setSetting("AESETTING", "AuthEnginePinCodePanic", sc.AuthEnginePinCodePanic.ToString());
			
			var authEngineAskMissingInfo = getSetting("AuthEngineAskMissingInfo", "AESETTING");
			if (authEngineAskMissingInfo == null)
				setSetting("AESETTING", "AuthEngineAskMissingInfo", sc.AuthEngineAskMissingInfo.ToString());
			
			var authEngineAskPin = getSetting("AuthEngineAskPin", "AESETTING");
			if (authEngineAskPin == null)
				setSetting("AESETTING", "AuthEngineAskPin", sc.AuthEngineAskPin.ToString());
			
			var authEngineAskProviderInfo = getSetting("AuthEngineAskProviderInfo", "AESETTING");
			if (authEngineAskProviderInfo == null)
				setSetting("AESETTING", "AuthEngineAskProviderInfo", sc.AuthEngineAskProviderInfo.ToString());
			
			var fieldMissingErrorEmail = getSetting("FieldMissingErrorEmail", "AESETTING");
			if (fieldMissingErrorEmail == null)
				setSetting("AESETTING", "FieldMissingErrorEmail", sc.FieldMissingErrorEmail);
			
			var fieldMissingErrorMobilePhone = getSetting("FieldMissingErrorMobilePhone", "AESETTING");
			if (fieldMissingErrorMobilePhone == null)
				setSetting("AESETTING", "FieldMissingErrorMobilePhone", sc.FieldMissingErrorMobilePhone);

            var authEngineAllowUPNLogin = getSetting("AuthEngineAllowUPNLogin", "AESETTING");
            if (authEngineAllowUPNLogin == null)
                setSetting("AESETTING", "AuthEngineAllowUPNLogin", sc.AuthEngineAllowUPNLogin.ToString());

            var authEngineAllowPre2000Login = getSetting("AuthEngineAllowPre2000Login", "AESETTING");
            if (authEngineAllowPre2000Login == null)
                setSetting("AESETTING", "AuthEngineAllowPre2000Login", sc.AuthEngineAllowPre2000Login.ToString());

            var AuthEngineAllowMobileNumberLogin = getSetting("AuthEngineAllowMobileNumberLogin", "AESETTING");
            if (AuthEngineAllowMobileNumberLogin == null)
                setSetting("AESETTING", "AuthEngineAllowMobileNumberLogin", sc.AuthEngineAllowMobileNumberLogin.ToString());

            var authEngineAllowEmailLogin = getSetting("AuthEngineAllowEmailLogin", "AESETTING");
            if (authEngineAllowEmailLogin == null)
                setSetting("AESETTING", "AuthEngineAllowEmailLogin", sc.AuthEngineAllowEmailLogin.ToString());

            var authEngineSAMLoginPreferred = getSetting("AuthEngineSAMLoginPreferred", "AESETTING");
            if (authEngineSAMLoginPreferred == null)
                setSetting("AESETTING", "AuthEngineSAMLoginPreferred", sc.AuthEngineSAMLoginPreferred.ToString());

            var authEngineAllowAliasesInLogin = getSetting("AuthEngineAllowAliasesInLogin", "AESETTING");
            if (authEngineAllowAliasesInLogin == null)
                setSetting("AESETTING", "AuthEngineAllowAliasesInLogin", sc.AuthEngineAllowAliasesInLogin.ToString());

            var authEngineHeartbeatInterval = getSetting("AuthEngineHeartbeatInterval", "AESETTING");
            if (authEngineHeartbeatInterval == null)
                setSetting("AESETTING", "AuthEngineHeartbeatInterval", sc.AuthEngineHeartbeatInterval.ToString());

            var authEngineWaitingInterval = getSetting("AuthEngineWaitingInterval", "AESETTING");
            if (authEngineWaitingInterval == null)
                setSetting("AESETTING", "AuthEngineWaitingInterval", sc.AuthEngineWaitingInterval.ToString());

            var oathCalcDefaultConfig = getSetting("OATHCalcDefaultConfig", "OATHCALC");
            if (oathCalcDefaultConfig == null)
                setSetting("OATHCALC", "OATHCalcDefaultConfig", sc.OATHCalcDefaultConfig);
		}

		
		void loadDbSettings()
		{
			var authEngineKeyBase = getSetting("AuthEngineKeyBase", "AESETTING");
			if (authEngineKeyBase != null)
				sc.AuthEngineKeyBase = SystemConfiguration.getEnumValue<RKGBase>(authEngineKeyBase);
			
			var authEnginePinCode = getSetting("AuthEnginePinCode", "AESETTING");
			if (authEnginePinCode != null)
				sc.AuthEnginePinCode = SystemConfiguration.getPinCodeOption(authEnginePinCode);
			
			var authEnginePinCodeLength = getSetting("AuthEnginePinCodeLength", "AESETTING");
			if (authEnginePinCodeLength != null)
				sc.AuthEnginePinCodeLength = SystemConfiguration.getInt32OrDef(authEnginePinCodeLength, sc.AuthEnginePinCodeLength);

            var authEnginePinCodePanic = getSetting("AuthEnginePinCodePanic", "AESETTING");
            if (authEnginePinCodePanic != null)
                sc.AuthEnginePinCodePanic = SystemConfiguration.getBoolOrDef(authEnginePinCodePanic, sc.AuthEnginePinCodePanic);

            var authEngineMutualAuth = getSetting("AuthEngineMutualAuth", "AESETTING");
            if (authEngineMutualAuth != null)
                sc.AuthEngineMutualAuth = SystemConfiguration.getBoolOrDef(authEngineMutualAuth, sc.AuthEngineMutualAuth);

            var OATHCalcUseDefaults = getSetting("OATHCalcUseDefaults", "OATHCALC");
            if (OATHCalcUseDefaults != null)
                sc.OATHCalcUseDefaults = SystemConfiguration.getBoolOrDef(OATHCalcUseDefaults, sc.OATHCalcUseDefaults);

            var authEngineAskMissingInfo = getSetting("AuthEngineAskMissingInfo", "AESETTING");
			if (authEngineAskMissingInfo != null)
				sc.AuthEngineAskMissingInfo = SystemConfiguration.getBoolOrDef(authEngineAskMissingInfo, sc.AuthEngineAskMissingInfo);
			
			var authEngineAskPin = getSetting("AuthEngineAskPin", "AESETTING");
			if (authEngineAskPin != null)
				sc.AuthEngineAskPin = SystemConfiguration.getBoolOrDef(authEngineAskPin, sc.AuthEngineAskPin);
			
			var authEngineAskProviderInfo = getSetting("AuthEngineAskProviderInfo", "AESETTING");
			if (authEngineAskProviderInfo != null)
				sc.AuthEngineAskProviderInfo = SystemConfiguration.getBoolOrDef(authEngineAskProviderInfo, sc.AuthEngineAskProviderInfo);
			
			var fieldMissingErrorEmail = getSetting("FieldMissingErrorEmail", "AESETTING");
			if (fieldMissingErrorEmail != null)
				sc.FieldMissingErrorEmail = fieldMissingErrorEmail;
			
			var fieldMissingErrorMobilePhone = getSetting("FieldMissingErrorMobilePhone", "AESETTING");
			if (fieldMissingErrorMobilePhone != null)
				sc.FieldMissingErrorMobilePhone = fieldMissingErrorMobilePhone;
			
			sc.PasswordVaulting = SystemConfiguration.getBoolOrDef(getSetting("PasswordVaulting", "RADIUS"), sc.PasswordVaulting);

            var authEngineAllowUPNLogin = getSetting("AuthEngineAllowUPNLogin", "AESETTING");
            if (authEngineAllowUPNLogin != null)
                sc.AuthEngineAllowUPNLogin = SystemConfiguration.getBoolOrDef(authEngineAllowUPNLogin, sc.AuthEngineAllowUPNLogin);

            var authEngineAllowPre2000Login = getSetting("AuthEngineAllowPre2000Login", "AESETTING");
            if (authEngineAllowPre2000Login != null)
                sc.AuthEngineAllowPre2000Login = SystemConfiguration.getBoolOrDef(authEngineAllowPre2000Login, sc.AuthEngineAllowPre2000Login);

            var AuthEngineAllowMobileNumberLogin = getSetting("AuthEngineAllowMobileNumberLogin", "AESETTING");
            if (AuthEngineAllowMobileNumberLogin != null)
                sc.AuthEngineAllowMobileNumberLogin = SystemConfiguration.getBoolOrDef(AuthEngineAllowMobileNumberLogin, sc.AuthEngineAllowMobileNumberLogin);

            var authEngineAllowEmailLogin = getSetting("AuthEngineAllowEmailLogin", "AESETTING");
            if (authEngineAllowEmailLogin != null)
                sc.AuthEngineAllowEmailLogin = SystemConfiguration.getBoolOrDef(authEngineAllowEmailLogin, sc.AuthEngineAllowEmailLogin);

            var authEngineAllowAliasesInLogin = getSetting("AuthEngineAllowAliasesInLogin", "AESETTING");
            if (authEngineAllowAliasesInLogin != null)
                sc.AuthEngineAllowAliasesInLogin = SystemConfiguration.getBoolOrDef(authEngineAllowAliasesInLogin, sc.AuthEngineAllowAliasesInLogin);

            var authEngineSAMLoginPreferred = getSetting("AuthEngineSAMLoginPreferred", "AESETTING");
            if (authEngineSAMLoginPreferred != null)
                sc.AuthEngineSAMLoginPreferred = SystemConfiguration.getBoolOrDef(authEngineSAMLoginPreferred, sc.AuthEngineSAMLoginPreferred);

            var authEngineHeartbeatInterval = getSetting("AuthEngineHeartbeatInterval", "AESETTING");
            if (authEngineHeartbeatInterval != null)
                sc.AuthEngineHeartbeatInterval = SystemConfiguration.getInt32OrDef(authEngineHeartbeatInterval, sc.AuthEngineHeartbeatInterval);

            var authEngineWaitingInterval = getSetting("AuthEngineWaitingInterval", "AESETTING");
            if (authEngineWaitingInterval != null)
                sc.AuthEngineWaitingInterval = SystemConfiguration.getInt32OrDef(authEngineWaitingInterval, sc.AuthEngineWaitingInterval);

            var oathCalcDefaultConfig = getSetting("OATHCalcDefaultConfig", "OATHCALC");
            if (oathCalcDefaultConfig != null)
                sc.OATHCalcDefaultConfig = oathCalcDefaultConfig;
		}
		
		bool SetMissingUserInfo(string user, string org, string field, string fieldValue)
		{
			switch (field)
			{
				case "PINCODE":
					if (sc.AuthEnginePinCode != PinCodeOption.AD && sc.AuthEnginePinCode != PinCodeOption.False)
					{
						if (sc.AuthEnginePinCode == PinCodeOption.Enforced && string.IsNullOrEmpty(fieldValue))
								throw new ValidationException("Pincode field cannot be empty.");
						if (fieldValue.Length != sc.AuthEnginePinCodeLength)
							throw new ValidationException(string.Format("Pincode must be {0} characters long.", sc.AuthEnginePinCodeLength));
						if (/*sc.AuthEnginePinCodePanic &&*/ fieldValue == TextHelper.Reverse(fieldValue))
							throw new ValidationException("Pincode cannot be equal when read backwards to allow panic mode.");
						using (var queries = DBQueriesProvider.Get())
						{
							var userId = queries.GetUserId(user, org);
							queries.NonQuery(
								string.Format(@"UPDATE [SMS_CONTACT] SET [PINCODE] = @PINCODE,[{0}]=1 WHERE ID = @USERID", TempPinCode),
								new DBQueryParm(@"USERID", userId),
								new DBQueryParm(@"PINCODE", CryptoHelper.HashPincode(fieldValue))
							);
							
							//if (sc.AuthEnginePinCodePanic) {
								queries.NonQuery(
									@"UPDATE [SMS_CONTACT] SET [PANIC_PINCODE] = @PANIC_PINCODE WHERE ID = @USERID",
									new DBQueryParm(@"USERID", userId), 
									new DBQueryParm(@"PANIC_PINCODE", CryptoHelper.HashPincode(TextHelper.Reverse(fieldValue)))
								);
							//}
						}
					}
					return true;
			}
			return false;
		}

		public const string TempPinCode = "TempPinCode";
		public const string TempPInfo = "TempPInfo";
		SetInfoRet CheckMissingUserInfo(string user, string org)
		{
			var ret = new SetInfoRet();

			if (sc.AuthEngineAskPin) {
				if (sc.AuthEnginePinCode == PinCodeOption.Enforced || sc.AuthEnginePinCode == PinCodeOption.True)
				{
					using (var queries = DBQueriesProvider.Get())
					{
						var userId = queries.GetUserId(user, org);
						var parms = new List<DBQueryParm>();
						parms.Add(new DBQueryParm(@"USERID", userId));
						var pinCodeQuery = @"SELECT [PINCODE] FROM [SMS_CONTACT] WHERE [ID] = @USERID";
						if (sc.AuthEnginePinCode == PinCodeOption.True)
							pinCodeQuery += string.Format(@" AND [{0}] = 1", TempPinCode);
						var pinCodeRow = queries.Query(pinCodeQuery, parms);
						if (pinCodeRow.Rows.Count == 0)
							return ret;

						var pinCode = (string)pinCodeRow.Rows[0][0];
						if (string.IsNullOrWhiteSpace(pinCode))
						{
							ret.AI = true;
							ret.AIF = "PINCODE";
							ret.Extra = "Please enter a Pincode: ";
						}
					}
				}
			}

			return ret;
		}

		RetBase SetInfoCmd(CommandBase cmd)
		{
			var si = (SetInfo)cmd;
			var ret = new SetInfoRet() { State = si.State };

			if (!sc.AuthEngineAskMissingInfo)
			{
				ret.Error = "Set info not allowed by configuration.";
				return ret;
			}
			
			try
			{
				var domainUser = GetDomainUser(si.User);				

				var providerLogicAndPinCode = GetUserProviderLogicAndPinCode(domainUser.Username, domainUser.Domain);

				bool setInfoSuccess;
				bool providerSetInfoSuccess;
				try {
					setInfoSuccess = SetMissingUserInfo(domainUser.Username, domainUser.Domain, si.Field, si.Value);
                    // BUGFIX BEGIN: 2016-10-15 - The PINCODE is set a second time after it has been set by the "ServerLogic.SetMissingUserInfo" method
                    // OLD CODE
                    //providerSetInfoSuccess = providerLogicAndPinCode.ProviderLogic.SetMissingUserInfo(domainUser.Username, domainUser.Domain, si.Field, si.Value);
                    // NEW CODE
                    providerSetInfoSuccess = setInfoSuccess || providerLogicAndPinCode.ProviderLogic.SetMissingUserInfo(domainUser.Username, domainUser.Domain, si.Field, si.Value);
                    // BUGFIX END
					if ( setInfoSuccess || providerSetInfoSuccess )
					{
						if (!UsersStates.Instance.hasUsernameAndState(domainUser.GetDomainUsername(), si.State))
							UsersStates.Instance.addUsernameAndState(domainUser.GetDomainUsername(), si.State);
						var ustate = UsersStates.Instance.getByUsernameAndState(domainUser.GetDomainUsername(), si.State);
						ustate.SetInfo = true;
						if (setInfoSuccess)
							ustate.PinCodeValidated = true;
					}
				} catch( ValidationException ex ) {
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
					ret.AI = true;
					ret.AIF = si.Field;
					ret.Error = ex.Message;
					return ret;
				}

				var userState = UsersStates.Instance.getByUsernameAndState(domainUser.GetDomainUsername(), si.State);
				ret = CheckMissingUserInfo(domainUser.Username, domainUser.Domain);
				if (!ret.AI && sc.AuthEngineAskProviderInfo)
				{
					ret = providerLogicAndPinCode.ProviderLogic.CheckMissingUserInfo(domainUser.Username, domainUser.Domain);
				}
				if (setInfoSuccess && ret.AIF == si.Field
				    || providerSetInfoSuccess && ret.AIF == si.Field) {
					ret.AI = false;
					ret.AIF = string.Empty;
					userState.SkipCheck = true;
				} else {
					userState.SkipCheck = false;
				}
				ret.State = si.State;
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("SetInfo ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("SetInfo ERROR Trace: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error setting info.";                
			}

			return ret;
		}

		private void setUsersLockToFalse()
		{
			using (var queries = DBQueriesProvider.Get())
			{
				var query =
					@"UPDATE SMS_CONTACT SET locked = @locked";
				var parms = new List<DBQueryParm>();
				parms.Add(new DBQueryParm(@"locked", 0));
				queries.NonQuery(query, parms);
			}
		}

		private void disableProviders()
		{
			foreach (var p in sc.Providers)
			{
				if (p.Enabled) continue;

				using (var queries = DBQueriesProvider.Get())
				{
					var query =
						@"
UPDATE
	UserProviders
SET
	active = 0, selected = 0
WHERE authProviderId = @providerId";
					var parms = new List<DBQueryParm>();
					parms.Add(new DBQueryParm(@"providerId", p.Id));
					queries.NonQuery(query, parms);
				}
			}
		}

		public RetBase GetAvailableModules(CommandBase cmd)
		{
			var ret = new GetAvailableModulesRet();
			try
			{
				if (availableModules.Count == 0)
				{
					var pl = (CloudSMSProviderLogic)(new CloudSMSProviderLogic().Using(sc));
					availableModules = pl.GetModules();
				}
				ret.Modules = availableModules;
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.GetAvailableModules ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.GetAvailableModules STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error getting user modules.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}

		public void Stop()
		{
			Logger.Instance.WriteToLog("ServerLogic.Stop", LogLevel.Debug);
			if (this.Actioner != null)
				this.Actioner.Clear();
            try {
                sc.SavePollingPreferences();
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.Stop: saving AuthEngine preference failed. " + ex.Message, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }            
		}

		public void InsertOrUpdateUsers(List<AddFullDetails> users)
		{
			foreach (AddFullDetails user in users)
			{
				AddFullUser(user);
			}
		}

		public RetBase LoadPassCodelen(CommandBase cmd)
		{
			TokenLen tokenLen = (TokenLen)cmd;

			TokenLenRet ret = new TokenLenRet();

			CloudSMSProviderLogic pl = (CloudSMSProviderLogic)(new CloudSMSProviderLogic().Using(sc));
			ret.Length = pl.GetPasscodeLen();

			return ret;
		}
		
		public static string getSetting(string key, string Object) {
			if (string.IsNullOrWhiteSpace(key))
				return string.Empty;
			var andObject = (!string.IsNullOrWhiteSpace(Object)) 
				? string.Format(" AND [OBJECT]=@OBJECT")
				: "";

			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm(@"SETTING", key));
			if (!string.IsNullOrWhiteSpace(Object))
				parms.Add(new DBQueryParm(@"OBJECT", Object));
			
			using (var queries = DBQueriesProvider.Get()) {
				return queries.QueryScalar<string>(string.Format(@"
SELECT [VALUE] FROM [SETTINGS]
WHERE [SETTING]=@SETTING {0}
", andObject), parms);
			}
		}
		
		public static void setSetting(string Object, string key, string val) {
			var parms = new List<DBQueryParm>();
			if (string.IsNullOrWhiteSpace(Object))
				Object = "AESETTING";
			parms.Add(new DBQueryParm(@"OBJECT", Object));
			parms.Add(new DBQueryParm(@"SETTING", key));
			parms.Add(new DBQueryParm(@"VALUE", val));
			using (var queries = DBQueriesProvider.Get()) {
				queries.NonQuery(@"
IF (NOT EXISTS(SELECT * FROM [SETTINGS] WHERE [SETTING]=@SETTING AND [OBJECT]=@OBJECT))
BEGIN 
    INSERT INTO [SETTINGS] ([SETTING], [VALUE], [OBJECT])
	VALUES(@SETTING, @VALUE, @OBJECT) 
END 
ELSE 
BEGIN 
    UPDATE [SETTINGS]
    SET [VALUE]=@VALUE
    WHERE [SETTING]=@SETTING AND [OBJECT]=@OBJECT
END
", parms);
			}			
		}
		
		public RetBase GetSettingsAction(CommandBase cmd)
		{
			var getSettings = (GetSettings)cmd;
			var ret = new GetSettingsRet();
			try {
				if (getSettings.External)
					checkAdmin(getSettings);
				
				var andObject = (!string.IsNullOrWhiteSpace(getSettings.Object)) 
				? " AND [OBJECT]=@OBJECT"
				: "";
				using (var queries = DBQueriesProvider.Get()) {
					var parms = new List<DBQueryParm>();
					if (!string.IsNullOrEmpty(andObject))
						parms.Add(new DBQueryParm(@"OBJECT", getSettings.Object));
					var data = queries.Query(string.Format(@"
	SELECT [SETTING], [VALUE], [OBJECT] FROM [SETTINGS]
	WHERE 1=1 {0}
					", andObject), parms);
					foreach(DataRow row in data.Rows) {
						ret.Settings.Add(new Setting 
						                 { 
						                 	Name = Convert.ToString(row["SETTING"]),
						                 	Value = Convert.ToString(row["VALUE"]),
					                 		Object = Convert.ToString(row["OBJECT"])
						                 });
					}
				}
				
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.GetSettings ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.GetSettings STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error getting settings";                
			}

			return ret;
		}

        public RetBase GetUserSettingsAction(CommandBase cmd)
        {
            var getSettings = (GetUserSettings)cmd;
            var ret = new GetSettingsRet();
            try {                
                var andObject = (!string.IsNullOrWhiteSpace(getSettings.Object))
                ? " AND [OBJECT]=@OBJECT"
                : "";
                using (var queries = DBQueriesProvider.Get()) {
                    var parms = new List<DBQueryParm>();
                    if (!string.IsNullOrEmpty(andObject))
                        parms.Add(new DBQueryParm(@"OBJECT", getSettings.Object));
                    var data = queries.Query(string.Format(@"
	SELECT [SETTING], [VALUE], [OBJECT] FROM [SETTINGS]
	WHERE 1=1 {0}
					", andObject), parms);
                    foreach (DataRow row in data.Rows) {
                        string obj = Convert.ToString(row["OBJECT"]);
                        if (obj != "AE_SETTING" && obj != "RADIUS") {
                            ret.Settings.Add(new Setting {
                                Name = Convert.ToString(row["SETTING"]),
                                Value = Convert.ToString(row["VALUE"]),
                                Object = obj
                            });
                        }
                    }
                }

            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.GetUserSettings ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.GetUserSettings STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error getting user settings";
            }

            return ret;
        }
		
		public RetBase SetSettingsAction(CommandBase cmd)
		{
			var setSettings = (SetSettings)cmd;
			var ret = new SetSettingsRet();
			try {
				if (setSettings.External)
					checkAdmin(setSettings);
				
				foreach(var setting in setSettings.Settings) {
					setSetting(setting.Object, setting.Name, setting.Value);
				}
				
				loadDbSettings();
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.SetSettings ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.SetSettings STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error setting settings";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}

		//this procedure will update user status
		public int UpdateUserStatus(AddFullDetails det, long userId)
		{
			bool enabled = false;
			if (det.Enabled.HasValue)
				enabled = det.Enabled.Value;
			int nonQueryResult = 0;
			if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
				Logger.Instance.WriteToLog("ServerLogic.UpdateUserStatus: " + det.Org + " " + det.User, LogLevel.DebugVerbose);
			try
			{
				if (det.External)
					checkSameDomainAndAdmin(det);
				ReplaceDomain(det);
				using (var queries = DBQueriesProvider.Get())
				{
					string phoneQuery;
					if (sc.AuthEngineOverrideWithADInfo)
					{
						phoneQuery = @"
[INPUT_PHONE_NUMBER]=@TN,
[Email]=@EMAILVAL,
[MOBILE_NUMBER]=@MOBILE,
[phoneOverrided]=0,
[emailOverrided]=0,
[mobileOverrided]=0
";
					}
					else
					{
						phoneQuery = @"
[INPUT_PHONE_NUMBER]=
	CASE
		WHEN [phoneOverrided]=0 THEN @TN
		WHEN [INPUT_PHONE_NUMBER] IS NULL THEN @TN
		WHEN [INPUT_PHONE_NUMBER] = '' THEN @TN
		WHEN [INPUT_PHONE_NUMBER] <> '' THEN [INPUT_PHONE_NUMBER]
	END,
[Email]=
	CASE
		WHEN [emailOverrided]=0 THEN @EMAILVAL
		WHEN [Email] IS NULL THEN @EMAILVAL
		WHEN [Email] = '' THEN @EMAILVAL
		WHEN [Email] <> '' THEN [Email]
	END,
[MOBILE_NUMBER]=
	CASE
		WHEN [mobileOverrided]=0 THEN @MOBILE
		WHEN [MOBILE_NUMBER] IS NULL THEN @MOBILE
		WHEN [MOBILE_NUMBER] = '' THEN @MOBILE
		WHEN [MOBILE_NUMBER] <> '' THEN [MOBILE_NUMBER]
	END,
[phoneOverrided]=
	CASE
		WHEN [INPUT_PHONE_NUMBER] IS NULL THEN 0
		WHEN [INPUT_PHONE_NUMBER] = '' THEN 0
		ELSE [phoneOverrided]
	END,
[emailOverrided]=
	CASE
		WHEN [Email] IS NULL THEN 0
		WHEN [Email] = '' THEN 0
		ELSE [emailOverrided]
	END,
[mobileOverrided]=
	CASE
		WHEN [MOBILE_NUMBER] IS NULL THEN 0
		WHEN [MOBILE_NUMBER] = '' THEN 0
		ELSE [mobileOverrided]
	END
";
					}

					var query = string.Format(@"
UPDATE SMS_CONTACT SET
UPDATE_DATE = GETDATE(), userStatus=@USER_ST, [uSNChanged]=@uSNChanged,
FirstName=CASE WHEN [FirstName] IS NULL THEN @FIRSTNAME ELSE [FirstName] END,
LastName=CASE WHEN [LastName] IS NULL THEN @LASTNAME ELSE [LastName] END,
UPN=@UPN,
AUTHENABLED=@AUTHEN,
[USER_TYPE]=
	CASE
		WHEN [utOverridden]=1 THEN [USER_TYPE]
		ELSE @USER_T
	END
,
[utOverridden]=
	CASE 
		WHEN [USER_TYPE]=@USER_T THEN 0
		ELSE [utOverridden]
	END
,
{0}
WHERE AD_USERNAME = @USER AND ORG_NAME = @ORG AND SID=@USID", phoneQuery);
					var parms = new List<DBQueryParm>();
					parms.Add(new DBQueryParm(@"USER", det.User));
					parms.Add(new DBQueryParm(@"ORG", det.Org));
					parms.Add(new DBQueryParm(@"TN", det.Phone));
					parms.Add(new DBQueryParm(@"MOBILE", det.Mobile));
					if ( det.FirstName == null )
						parms.Add(new DBQueryParm(@"FIRSTNAME", DBNull.Value));
					else
						parms.Add(new DBQueryParm(@"FIRSTNAME", det.FirstName));
					if ( det.LastName == null )
						parms.Add(new DBQueryParm(@"LASTNAME", DBNull.Value));
					else
						parms.Add(new DBQueryParm(@"LASTNAME", det.LastName));
					parms.Add(new DBQueryParm(@"EMAILVAL", det.Email ?? string.Empty));
					parms.Add(new DBQueryParm(@"USER_ST", enabled ? 1 : 0));
					parms.Add(new DBQueryParm(@"uSNChanged", det.uSNChanged));
					parms.Add(new DBQueryParm(@"USER_T", det.UserType));
					parms.Add(new DBQueryParm(@"USID", det.Sid));
					if (det.AuthEnabled.HasValue)
						parms.Add(new DBQueryParm(@"AUTHEN", det.AuthEnabled.Value));
					else
						parms.Add(new DBQueryParm(@"AUTHEN", DBNull.Value));

                    if (String.IsNullOrEmpty(det.UPN))
                    {
                        string defaultUPN = string.Format("{0}@{1}", det.User, det.Org);
                        parms.Add(new DBQueryParm(@"UPN", defaultUPN));
                    }
                    else
                        parms.Add(new DBQueryParm(@"UPN", det.UPN));

					nonQueryResult = queries.NonQuery(query, parms);
				}
				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.Instance.WriteToLog("ServerLogic.UpdateUserStatus nonQueryResult: " + nonQueryResult, LogLevel.DebugVerbose);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.UpdateUserStatus ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.UpdateUserStatus STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return nonQueryResult;
		}

		public RetBase AddFullUser(CommandBase cmd)
		{
			AddFullDetails det = (AddFullDetails)cmd;

			if (det.External)
				checkAdmin(det);
			ReplaceDomain(det);

			if (Logger.I.ShouldLog(LogLevel.Info))
				Logger.Instance.WriteToLog(string.Format("Adding/Updating user '{0}\\{1}' - '{2}'", det.Org, det.User, det.UserType), LogLevel.Info);

			AddFullDetailsRet ret = new AddFullDetailsRet();
			ret.Out = 0;

			if (Logger.I.ShouldLog(LogLevel.Debug))
				Logger.Instance.WriteToLog("ServerLogic.AddFullUser", LogLevel.Debug);

			if (string.IsNullOrEmpty(det.User))
			{
				if (string.IsNullOrEmpty(det.Fullname))
					throw new Exception("Cannot add empty user.");
				det.User = det.Fullname.Substring(0, det.Fullname.IndexOf("@"));
				Logger.Instance.WriteToLog("ServerLogic.AddFullUser: Username was empty.", LogLevel.Debug);
			}

			if (string.IsNullOrEmpty(det.Fullname))
			{
				det.Fullname = det.User;
				Logger.Instance.WriteToLog("ServerLogic.AddFullUser: Fullname was empty.", LogLevel.Debug);
			}

			bool enabled = false;
			if (det.Enabled.HasValue)
				enabled = det.Enabled.Value;
			if (string.IsNullOrEmpty(det.UserType))
				det.UserType = UserType.User;

			foreach (var up in det.Providers)
			{
				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.Instance.WriteToLog(string.Format("ServerLogic.AddFullUser: {0} {1} provider {2}.", det.Org, det.User, up.Name), LogLevel.DebugVerbose);
				if (up.Provider == null)
				{
					foreach (var p in sc.Providers)
					{
						if (p.Name == up.Name)
						{
							up.Provider = p;
							if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
								Logger.Instance.WriteToLog(string.Format("ServerLogic.AddFullUser: {0} {1} provider null {2} ({3}).", det.Org, det.User, up.Name, up.Provider.Id), LogLevel.DebugVerbose);
							break;
						}
					}
				}
				else
				{
					if (up.Provider.Id == 0)
					{
						fillProviderId(up.Provider);
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.Instance.WriteToLog(string.Format("ServerLogic.AddFullUser: {0} {1} provider filled id {2} ({3}).", det.Org, det.User, up.Name, up.Provider.Id), LogLevel.DebugVerbose);
					}
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.Instance.WriteToLog(string.Format("ServerLogic.AddFullUser: {0} {1} provider not null {2} ({3}).", det.Org, det.User, up.Name, up.Provider.Id), LogLevel.DebugVerbose);
				}
			}
			try
			{
				DBQueries.UserData idAndSid;
				using (var queries = DBQueriesProvider.Get())
				{
					idAndSid = queries.GetUserData(det.User, det.Org);
				}

				// Existing user
				if (idAndSid.Id != 0)
				{
					if (idAndSid.Sid != det.Sid)
						RemoveUserIfExists(det);
					else
					{
						// Check uSNChanged to see if some user attribute really changed
						// otherwise we skip it
						if (det.uSNChanged == idAndSid.uSNChanged) {
							if (Logger.I.ShouldLog(LogLevel.Debug))
								Logger.Instance.WriteToLog(string.Format("ServerLogic.AddFullUser: Skipped {0}\\{1} as uSNChanged('{2}') did not change.", det.Org, det.User, det.uSNChanged), LogLevel.Debug);
							ret.Out = 1;
						} else {
							ret.Out = UpdateUserStatus(det, idAndSid.Id);
							UpdateUserProviders(det);
						}
						return ret;
					}
				}

				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.Instance.WriteToLog(string.Format("ServerLogic.AddFullUser: {0} {1} was not updated, inserting.", det.Org, det.User), LogLevel.DebugVerbose);

				using (var queries = DBQueriesProvider.Get())
				{
					const string query = @"
INSERT INTO SMS_CONTACT (AD_USERNAME,INPUT_PHONE_NUMBER,MOBILE_NUMBER,
	DISPLAY_NAME,LastName,FirstName,USER_TYPE,userStatus,ORG_NAME,ORG_TYPE,SID,AUTHENABLED,PinCode,Email,uSNChanged,UPN)
SELECT @USER,@TN,@MOBILE,@DN,@LASTNAME,@FIRSTNAME,@UT,@US,@ON,@OT,@USID,@AUTHEN,@PinCode,@EMAILVAL,@uSNChanged,@UPN
";

					var parms = new List<DBQueryParm>();

					parms.Add(new DBQueryParm(@"USER", det.User));
					parms.Add(new DBQueryParm(@"TN", det.Phone));
					parms.Add(new DBQueryParm(@"MOBILE", det.Mobile));
					parms.Add(new DBQueryParm(@"DN", det.Fullname));					
					if ( det.FirstName == null )
						parms.Add(new DBQueryParm(@"FIRSTNAME", DBNull.Value));
					else
						parms.Add(new DBQueryParm(@"FIRSTNAME", det.FirstName));
					if ( det.LastName == null )
						parms.Add(new DBQueryParm(@"LASTNAME", DBNull.Value));
					else
						parms.Add(new DBQueryParm(@"LASTNAME", det.LastName));
					parms.Add(new DBQueryParm(@"UT", det.UserType));
					parms.Add(new DBQueryParm(@"US", enabled ? 1 : 0));
					parms.Add(new DBQueryParm(@"uSNChanged", det.uSNChanged));
					parms.Add(new DBQueryParm(@"ON", det.Org));
					parms.Add(new DBQueryParm(@"OT", det.OrgType));
					parms.Add(new DBQueryParm(@"USID", det.Sid));
					if (det.AuthEnabled.HasValue)
						parms.Add(new DBQueryParm(@"AUTHEN", det.AuthEnabled.Value));
					else
						parms.Add(new DBQueryParm(@"AUTHEN", DBNull.Value));
					var pinCode = string.Empty;
					if (!string.IsNullOrEmpty(det.PinCode)) {
						pinCode = encryption.Encrypt(CryptoHelper.HashPincode(det.PinCode));
					}
					parms.Add(new DBQueryParm(@"PinCode", pinCode));
					if (det.Email == null)
						det.Email = string.Empty;
					parms.Add(new DBQueryParm(@"EMAILVAL", det.Email));
                    if (String.IsNullOrEmpty(det.UPN))
                    {
                        string defaultUPN = string.Format("{0}@{1}", det.User, det.Org);
                        parms.Add(new DBQueryParm(@"UPN", defaultUPN));
                    }
                    else
                        parms.Add(new DBQueryParm(@"UPN", det.UPN));                    

					ret.Out = queries.NonQuery(query, parms);
				}
				if (ret.Out > 0)
					ret.Out = InsertUserProvidersIfNotExists(det, ret.Out, false);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.AddFullUser ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.AddFullUser STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error adding full user";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}

		private void UpdateUserProviders(AddFullDetails det)
		{
			var userId = (long)0;
			ReplaceDomain(det);
			using (var queries = DBQueriesProvider.Get())
				userId = queries.GetUserId(det.User, det.Org);

			var tryEachProvider = true;
			var allProvidersEnabled = true;
			foreach (var p in det.Providers)
				allProvidersEnabled = allProvidersEnabled && p.Enabled;

			if (allProvidersEnabled)
			{
				using (var queries = DBQueriesProvider.Get())
				{
					const string query =
						@"UPDATE UserProviders SET active = @PACT
						WHERE userId = @userId";
					var parms = new List<DBQueryParm>();
					parms.Add(new DBQueryParm(@"userId", userId));
					parms.Add(new DBQueryParm(@"PACT", Convert.ToInt32(true)));

					var affectedRows = queries.NonQuery(query, parms);
					if (affectedRows == det.Providers.Count)
						tryEachProvider = false;
				}
			}
			if (tryEachProvider)
			{
				foreach (var p in det.Providers)
				{
					if (UpdateUserProvider(det, p, userId) == 0)
						InsertUserProviderIfNotExists(det, p, userId);
				}
			}
			SelectDefaultUserProvider(det);
		}
		
		public UserProvider getDefaultProvider(AddFullDetails det) {
			var enabledProviders = det.Providers
				.Where(x => x.Enabled)
				;
			var enabledProvidersInGroup = det.Providers
				.Where(x => x.Enabled && !string.IsNullOrWhiteSpace(x.Provider.AdGroup))
				;

			UserProvider defUserProvider = null;
			if (enabledProvidersInGroup.Any()) {
				defUserProvider = enabledProvidersInGroup.FirstOrDefault(up => up.Enabled && up.Provider.Default);
				if (defUserProvider == null)
					defUserProvider = enabledProvidersInGroup.FirstOrDefault(up => up.Enabled);
			}
			if (defUserProvider == null) {
				defUserProvider = det.Providers.FirstOrDefault(up => up.Enabled && up.Provider.Default);
				if (defUserProvider == null)
					defUserProvider = det.Providers.FirstOrDefault(up => up.Enabled);
			}
			if (defUserProvider == null) { 
				if (det.Providers.Any()) {
					Logger.I.WriteToLog(string.Format(
						"getDefaultProvider {0}\\{1} has no enabled providers.", det.Org, det.User)
					                    , LogLevel.Info);
					defUserProvider = det.Providers.First();
				} else
					throw new ServerLogicException("No providers for the user.");
			}
			return defUserProvider;
		}

		public void SelectDefaultUserProvider(AddFullDetails det)
		{
			long userId = 0;
			using (var queries = DBQueriesProvider.Get())
			{
				userId = queries.GetUserId(det.User, det.Org);
			}
			
			var enabledProviders = det.Providers
				.Where(x => x.Enabled)
				;
			
			var enabledProvidersInGroup = det.Providers
				.Where(x => x.Enabled && !string.IsNullOrWhiteSpace(x.Provider.AdGroup))
				;
			
			const string query = @"
SELECT TOP 1 authProviderId
FROM
UserProviders
WHERE
userId = @userId AND active = 1 AND selected = 1";
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm(@"userId", userId));
			using (var queries = DBQueriesProvider.Get())
			{
				int? selected = null;
				try {
					selected = queries.QueryScalar<int?>(query, parms);
				} catch(Exception ex) {
					Logger.I.WriteToLog("SelectDefaultUserProvider queryScalar failed", LogLevel.Debug);
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				}
				if (selected != null) {
					if (enabledProviders.Any(x => x.Provider.Id == selected.Value)) {
						if (!enabledProvidersInGroup.Any()
						    || enabledProvidersInGroup.Any(x => x.Provider.Id == selected.Value))
							return;
					}
				}
			}
		
			var authProviderId = 0;
			using (var queries = DBQueriesProvider.Get())
			{
				var defUserProvider = getDefaultProvider(det);

				var providerFilter = string.Empty;
				if ( defUserProvider == null ) {
					parms.Add(new DBQueryParm(@"psel", 0));
				} else {
					authProviderId = defUserProvider.Provider.Id;
					providerFilter = " AND authProviderId = @authProviderId";
					parms.Add(new DBQueryParm(@"authProviderId", authProviderId));
					parms.Add(new DBQueryParm(@"psel", 1));
				}
				
				queries.NonQuery(
					string.Format(@"UPDATE UserProviders
SET selected = @psel
WHERE userId = @userId {0}
", providerFilter), parms);
				
				if ( authProviderId != 0 ) {
					queries.NonQuery(
					@"UPDATE UserProviders
SET selected = 0
WHERE userId = @userId AND authProviderId <> @authProviderId
", parms);
				}
			}
			
			var p = sc.Providers.Find(o => o.Id == authProviderId);
			if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
				Logger.I.WriteToLog("Calling postSelect if provider found.", LogLevel.DebugVerbose);
			if (p != null && !string.IsNullOrEmpty(p.Name))
			{
				try {
					var pLogic = ProviderLogicFactory.GetByName(p.Name)
						.Using(sc)
						.Using(this)
						.Using(p)
						;
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog(string.Format("Calling postSelect on '{0}' provider.", p.Name), LogLevel.DebugVerbose);
					if (isAged(userId, sc.AuthEngineAutoSetupAgedTimeHours)) {
						Logger.I.WriteToLog("Skipping postSelect due to aged check.", LogLevel.DebugVerbose);
					} else
						pLogic.PostSelect(det.User, det.Org, userId, authProviderId, false);
				} catch( Exception ex ) {
					Logger.I.WriteToLog("SelectDefaultUserProvider.PostSelect ERROR: " + ex.Message, LogLevel.Error);
					Logger.I.WriteToLog("SelectDefaultUserProvider.PostSelect STACK: " + ex.InnerException, LogLevel.Debug);
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
					throw;
				}
			}
		}

		private int UpdateUserProvider(AddFullDetails det, UserProvider up, long userId)
		{
			int ret = 0;
			try
			{
				ReplaceDomain(det);
				using (var queries = DBQueriesProvider.Get())
				{
					var query =
						@"UPDATE UserProviders SET active = @PACT
								WHERE
								userId = @userId
								AND authProviderId = @PID
								";
					var parms = new List<DBQueryParm>();

					parms.Add(new DBQueryParm(@"userId", userId));
					parms.Add(new DBQueryParm(@"PID", up.Provider.Id));
					parms.Add(new DBQueryParm(@"PACT", Convert.ToInt32(up.Enabled)));

					ret += queries.NonQuery(query, parms);
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.UpdateUserProvider ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.UpdateUserProvider STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}

		private void RemoveUserIfExists(AddFullDetails det)
		{
			if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
				Logger.Instance.WriteToLog("ServerLogic.RemoveUserIfExists: Remove same domain\\user if exists and reinsert (it means their SID didn't match).", LogLevel.DebugVerbose);
			using (var queries = DBQueriesProvider.Get())
			{
				const string query = @"
DELETE FROM SMS_CONTACT WHERE AD_USERNAME = @USER AND ORG_NAME = @ORG
";
				var parms = new List<DBQueryParm>();

				parms.Add(new DBQueryParm(@"USER", det.User));
				parms.Add(new DBQueryParm(@"ORG", det.Org));

				if (queries.NonQuery(query, parms) > 0)
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.Instance.WriteToLog("ServerLogic.RemoveUserIfExists: User existed and removed.", LogLevel.DebugVerbose);
			}
		}

		int InsertUserProvidersIfNotExists(AddFullDetails det, int ret, bool checkIfExists = true)
		{
			ReplaceDomain(det);

			var userId = (long)0;
			using (var queries = DBQueriesProvider.Get())
				userId = queries.GetUserId(det.User, det.Org);

			var defaultProvider = getDefaultProvider(det);
			det.Providers.ForEach(x => x.Selected = (x.Provider.Id == defaultProvider.Provider.Id));
			
			if (!checkIfExists)
			{
				if (det.Providers.Count == 0)
				{
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format("InsertUserProvidersIfNotExists (!checkIfExists) - {0}\\{1} does not have providers.", det.Org, det.User), LogLevel.Debug);
					return 0;
				}
				using (var queries = DBQueriesProvider.Get())
				{
					const string query = @"INSERT INTO UserProviders (userId,authProviderId,active,selected,config) ";
					var parms = new List<DBQueryParm>();
					var values = new string[det.Providers.Count];
					var idx = 0;
					foreach (var p in det.Providers)
					{
						if (p.Provider.Id == 0)
							fillProviderId(p.Provider);
						values[idx] = string.Format(" SELECT @userID{0},@PID{0},@PACT{0},@PSEL{0},@PCONF{0}", idx);
						parms.Add(new DBQueryParm(@"userID" + idx, userId));
						parms.Add(new DBQueryParm(@"PID" + idx, p.Provider.Id));
						parms.Add(new DBQueryParm(@"PACT" + idx, Convert.ToInt32(p.Enabled)));
						parms.Add(new DBQueryParm(@"PSEL" + idx, p.Selected));
						parms.Add(new DBQueryParm(@"PCONF" + idx, (string.IsNullOrEmpty(p.Provider.Config)) ? string.Empty : p.Provider.Config));
						idx++;
					}
					try
					{
						ret += queries.NonQuery(query + string.Join(" UNION ALL ", values), parms);
					}
					catch (Exception ex)
					{
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.Instance.WriteToLog(query + " : " + ex.Message, LogLevel.DebugVerbose);
                        Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
						throw;
					}
				}
				try {
					var pLogic = ProviderLogicFactory.GetByName(defaultProvider.Name)
						.Using(sc)
						.Using(this)
						.Using(defaultProvider.Provider)
						;
					if (isAged(userId, sc.AuthEngineAutoSetupAgedTimeHours)) {
						Logger.I.WriteToLog("Skipping postSelect due to aged check.", LogLevel.DebugVerbose);
					} else
						pLogic.PostSelect(det.User, det.Org, userId, defaultProvider.Provider.Id, false);
				} catch( Exception ex ) {
					Logger.I.WriteToLog("InsertUserProvidersIfNotExists.PostSelect ERROR: " + ex.Message, LogLevel.Error);
					Logger.I.WriteToLog("InsertUserProvidersIfNotExists.PostSelect STACK: " + ex.InnerException, LogLevel.Debug);
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
					throw;
				}
			}
			else
			{
				foreach (var p in det.Providers)
				{
					ret += InsertUserProviderIfNotExists(det, p, userId, checkIfExists);
				}
			}
			
			return ret;
		}

		private int InsertUserProviderIfNotExists(AddFullDetails det, UserProvider p, long userId, bool checkIfExists = true)
		{
			int ret = 0;
			try
			{
				ReplaceDomain(det);
				using (var queries = DBQueriesProvider.Get())
				{
					const string queryNormal =@"
						INSERT INTO UserProviders (userId,authProviderId,active,selected,config)
SELECT @userID,@PID,@PACT,@PSEL,@PCONF
								";
					const string queryIfExists = @"
INSERT INTO UserProviders (userId,authProviderId,active,selected,config)
SELECT @userID,@PID,@PACT,@PSEL,@PCONF
WHERE NOT EXISTS(SELECT * FROM UserProviders WHERE userId = @userID AND authProviderId = @PID)";
					string query = checkIfExists
						? queryIfExists
						: queryNormal;
					
					var parms = new List<DBQueryParm>();
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.Instance.WriteToLog(string.Format("ServerLogic.InsertUserProvider: userId: {0}  authProviderId: {1} ({2}) ", userId, p.Provider.Id, p.Name), LogLevel.DebugVerbose);
					parms.Add(new DBQueryParm(@"userID", userId));
					parms.Add(new DBQueryParm(@"PID", p.Provider.Id));
					parms.Add(new DBQueryParm(@"PACT", Convert.ToInt32(p.Enabled)));
					parms.Add(new DBQueryParm(@"PSEL", p.Selected));
					parms.Add(new DBQueryParm(@"PCONF", (string.IsNullOrEmpty(p.Provider.Config)) ? string.Empty : p.Provider.Config));

					ret += queries.NonQuery(query, parms);
				}
				if (p.Selected) {
					try {
						var pLogic = ProviderLogicFactory.GetByName(p.Name)
							.Using(sc)
							.Using(this)
							.Using(p.Provider)
							;
						if (isAged(userId, sc.AuthEngineAutoSetupAgedTimeHours)) {
							Logger.I.WriteToLog("Skipping postSelect due to aged check.", LogLevel.DebugVerbose);
						} else
							pLogic.PostSelect(det.User, det.Org, userId, p.Provider.Id, false);
					} catch( Exception ex ) {
						Logger.I.WriteToLog("InsertUserProvidersIfNotExists.PostSelect ERROR: " + ex.Message, LogLevel.Error);
						Logger.I.WriteToLog("InsertUserProvidersIfNotExists.PostSelect STACK: " + ex.InnerException, LogLevel.Debug);                        
						throw;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.InsertUserProvider ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.InsertUserProvider STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}

		public RetBase Tokens(CommandBase cmd)
		{
			Tokens tokens = (Tokens)cmd;
			TokensRet ret = new TokensRet();

			ret.Available = 0;
			try
			{
				using (var queries = DBQueriesProvider.Get())
				{
					const string query = @"SELECT TOP 1 ISNULL(QUANTITY,0) AS QUANTITY FROM TOKEN_LOG ORDER BY DATE_INSERTED DESC";
					var queryResult = queries.Query(query);
					if (queryResult.Rows.Count == 1)
						ret.Available = Convert.ToInt64(queryResult.Rows[0]["QUANTITY"]);
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.Tokens ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.Tokens STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error getting tokens available";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}

		public RetBase getMessages(CommandBase cmd)
		{
			Logger.Instance.WriteToLog("ServerLogic.getMessages", LogLevel.Debug);
			var msg = (AllMsgs)cmd;
			var ret = new AllMsgsRet();
			try
			{
				using (var db = DBQueriesProvider.Get()) {
					var tb = db.Query(@"
SELECT * 
FROM [Message]
ORDER BY [order] ASC
");
					foreach(DataRow row in tb.Rows) {
						ret.Messages.Add(new TemplateMessage {
							Label = Convert.ToString(row["label"]),
							Message = Convert.ToString(row["text"]),
							Title = row.Field<string>("title"),
							Legend = Convert.ToString(row["replacement"]),
							Order = Convert.ToInt32(row["order"])
						});
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.getMessages ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.getMessages STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error loading message";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}

		public RetBase updateMessage(CommandBase cmd)
		{
			var updateMessageCmd = (UpdateMessage)cmd;
			if (updateMessageCmd.External)
				checkAdmin(updateMessageCmd);
			var ret = new UpdateMessageRet();
			try
			{
				using (var queries = DBQueriesProvider.Get())
				{
					queries.NonQuery(
						@"UPDATE [Message] 
SET [text] = @TEXT, [title]=@TITLE
WHERE [label] = @LABEL",
						new DBQueryParm("LABEL", updateMessageCmd.Label),
						new DBQueryParm("TEXT", updateMessageCmd.Message),
						new DBQueryParm("TITLE", updateMessageCmd.Title)
					);
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.updateMessage ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.updateMessage STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error updating message";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}

		public void validatePinCode(string username, string userPinCode, string panicPinCode, string enteredPinCode, UserStateDetail userStateDetails)
		{
            if (AESEncryption.IsEncrypted(userPinCode)) {
                userPinCode = encryption.Decrypt(userPinCode);
            }

			if (userStateDetails.PinCodeValidated)
			{
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog(string.Format("ValidatePinCode.PinCodeValidated == true", userPinCode, enteredPinCode), LogLevel.Debug);
				return;
			}

			if (sc.AuthEnginePinCode == PinCodeOption.False)
			{
				userStateDetails.PinCodeValidated = true;
				return;
			}

			// If PinCodeOption.On (not Enforced) then an empty user entered pincode should be valid
			// provided the pincode saved is empty too
			if (sc.AuthEnginePinCode == PinCodeOption.True
			    && string.IsNullOrEmpty(enteredPinCode)
			    && string.IsNullOrEmpty(userPinCode)
			   )
			{
				userStateDetails.PinCodeValidated = true;
				return;
			}

			if (string.IsNullOrEmpty(enteredPinCode))
				return;

			if (sc.AuthEnginePinCode == PinCodeOption.AD)
			{
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog("ValidatePinCode.AdAuth", LogLevel.Debug);
				var ldapRootNoContainer = string.Format("LDAP://{0}/{1}", sc.ADServerAddress, sc.ADBaseDN);
				//if (AdHelper.ValidateUserAd(ldapRootNoContainer, IdentityHelper.GetLogin(username), enteredPinCode))
				if (AdHelper.ValidateUserAd(ldapRootNoContainer, username, enteredPinCode))
				{
					userStateDetails.PinCodeValidated = true;
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("ValidatePinCode.AdAuth SUCCESS", LogLevel.Debug);
				} else
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("ValidatePinCode.AdAuth FAILED", LogLevel.Debug);
			}
			else // PinCodeOption == On or Enforced and a PinCode was supplied
			{
				enteredPinCode = CryptoHelper.HashPincode(enteredPinCode);
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog(string.Format("ValidatePinCode.Entered expected '{0}' given '{1}'", userPinCode, enteredPinCode), LogLevel.Debug);

                bool panicOn = false;
                DomainUsername domainUsername = DomainUsername.FromDomainUsername(username);
                using (var queries = DBQueriesProvider.Get())
                {                    
                    panicOn = queries.GetPanicState(domainUsername.Username, domainUsername.Domain);
                }
				if (!panicOn && userPinCode == enteredPinCode)
					userStateDetails.PinCodeValidated = true;
				else if (sc.AuthEnginePinCodePanic) {					
					if (panicPinCode == enteredPinCode) {
                        Logger.I.WriteToLog(string.Format("PANIC! PANIC! User '{0}' === PANIC", username), LogLevel.All);
                        
                        using (var queries = DBQueriesProvider.Get())
                        {                            
                            queries.SetPanicState(domainUsername.Username, domainUsername.Domain, true);
                        }

						userStateDetails.PinCodeValidated = true;
						userStateDetails.Panic = true;
					}
				}
			}
		}
		
		bool isAged(long userId, int agedTime)
		{
			if ( agedTime == 0 ) {
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog("isAged.Disabled", LogLevel.Debug);
				return false;
			}
			using (var queries = DBQueriesProvider.Get())
			{
				const string query = @"SELECT TOP 1 [CREATION_DATE], DATEADD(hh,@AGED,GETDATE()) AS AGED_DATE
FROM [SMS_CONTACT]
WHERE [ID]=@USERID AND [CREATION_DATE]>DATEADD(hh,@AGED,GETDATE())";
				var queryResult = queries.Query(query
				                                , new DBQueryParm("USERID", userId)
				                                , new DBQueryParm("AGED", (-agedTime)));
				return queryResult.Rows.Count != 1;
			}
		}

		ValidateUserRet CheckMissingInfo(DomainUsername domainUser, string state, IProviderLogic providerLogic, bool checkProvider)
		{
			if (sc.AuthEngineAskMissingInfo) {
				var siRet = CheckMissingUserInfo(domainUser.Username, domainUser.Domain);
				if (checkProvider && !siRet.AI && sc.AuthEngineAskProviderInfo)
					siRet = providerLogic.CheckMissingUserInfo(domainUser.Username, domainUser.Domain);
				if (siRet.AI) {
					return new ValidateUserRet {
						AI = true,
						AIF = siRet.AIF,
						Extra = siRet.Extra,
						State = state
					};
				}
			}
			return new ValidateUserRet();
		}
		
        private string ReplaceUPNDomain(string UPN)
        {
            if (!UPN.Contains('@'))
                throw new Exception(string.Format("UPN {0} has wrong format.", UPN));

            string[] creds = UPN.Split('@');
            string defaultDomain = replacements.ReplacementFor(creds[1]);
            return string.Format("{0}@{1}", creds[0], defaultDomain);
        }

        private DomainUsername GetDomainUser(string input)
        {
            DomainUsername domainUsername = null;
            var queries = DBQueriesProvider.Get();            

            if (sc.AuthEngineAllowMobileNumberLogin)
            {                
                var mobileNumber = identityHelper.GetMobileNumberOrNull(input);
                if (mobileNumber != null)
                {
                    List<string> mobileNumbers = queries.GetMobileNumbers();
                        foreach (string number in mobileNumbers)
                        {
                            Regex digitsOnly = new Regex(@"[^\d]");
                            string digitsOnlyNumber = digitsOnly.Replace(number, "");
                            if (digitsOnlyNumber == mobileNumber)
                            {
                                var loginData = queries.GetLoginDataByMobileNumber(mobileNumber);
                                if (!loginData.IsEmpty())
                                    domainUsername = new DomainUsername(loginData.DomainName, loginData.Username);
                                break;
                            }
                        }                                                                                 
                }        
            }

            if (input.Contains('@'))
            {
                if (domainUsername == null && sc.AuthEngineAllowUPNLogin)
                {
                    string replacingUPN = input;
                    if (sc.AuthEngineAllowAliasesInLogin)
                        replacingUPN = ReplaceUPNDomain(input);                    
                    var loginData = queries.GetLoginDataByUPN(replacingUPN);
                    if (!loginData.IsEmpty())
                        domainUsername = new DomainUsername(loginData.DomainName, loginData.Username);                                        
                }

                if (domainUsername == null && sc.AuthEngineAllowEmailLogin)
                {                                        
                    var loginData = queries.GetLoginDataByEmail(input);
                    if (!loginData.IsEmpty())
                        domainUsername = new DomainUsername(loginData.DomainName, loginData.Username);                                        
                }

            }

            if (domainUsername == null && sc.AuthEngineAllowPre2000Login && input.Contains("\\"))
            {                
                string domain = IdentityHelper.GetDomain(input);
                string username = IdentityHelper.GetLogin(input);

                domainUsername = new DomainUsername(domain, username);
                
                if (sc.AuthEngineAllowAliasesInLogin)
                {
                    domainUsername = ReplaceDomainUsername(domainUsername);
                }
            }

            if (domainUsername == null)
            {                
                domainUsername = new DomainUsername(getDefaultDomain(), input);
            }

            return domainUsername;
        }

        private bool IsUPNPrefix(string shortUsername)
        {
            bool isUPNPrefix = false;
            using (var queries = DBQueriesProvider.Get())
            {
                List<string> UPNs = queries.GetUPNs();

                foreach (string upn in UPNs)
                {
                    if (!String.IsNullOrEmpty(upn))
                    {
                        string compareString = upn;
                        if (upn.Contains("@"))
                            compareString = upn.Substring(0, upn.IndexOf("@"));

                        if (shortUsername == compareString)
                        {
                            isUPNPrefix = true;
                            break;
                        }
                    }                               
                }
            }

            return isUPNPrefix;
        }

        private string GetSAMByUPNPrefix(string shortUsername)
        {
            string SAM = null;            

            if (IsUPNPrefix(shortUsername))
            {
                string UPN = shortUsername + '@' + getDefaultDomain();
                var queries = DBQueriesProvider.Get();
                DBQueries.LoginData loginData = queries.GetLoginDataByUPN(UPN);
                SAM = loginData.Username;
            }

            return SAM;
        }

        private string GetMutualAuthChallengeMessage(string user, string domain)
        {
            string message = string.Empty;
            GetUserAuthImages getImagesCmd = new GetUserAuthImages();
            getImagesCmd.User = user;
            getImagesCmd.Org = domain;
            GetUserAuthImagesRet ret = (GetUserAuthImagesRet)GetAuthImages(getImagesCmd);
            message = string.Format(MutualAuthChallengeMessage, ret.LeftImageId, ret.RightImageId);
            return message;
        }

		public RetBase ValidateUser(CommandBase cmd)
		{
            Logger.Instance.WriteToLog("ServerLogic.ValidateUser", LogLevel.Debug);
            Tracker.Instance.TrackEvent("User Validation Attempt", Tracker.Instance.DefaultEventCategory);

			var validateUser = (ValidateUser)cmd;

			string state = string.Empty;
			if (!sc.AuthEngineStateless)
				state = RandomKeyGenerator.Generate(6, RKGBase.Base32);

			if (!string.IsNullOrEmpty(validateUser.State))
				state = validateUser.State;				

			if (validateUser.PinCode == null)
				validateUser.PinCode = string.Empty;
			try
			{
                var domainUser = GetDomainUser(validateUser.User); 

                var domainUsername = domainUser.GetDomainUsername();                               

				DBQueries.UserData userData = new DBQueries.UserData();                

                var queries = DBQueriesProvider.Get();

                if (!sc.AuthEngineSAMLoginPreferred)
                {
                    string SAM = GetSAMByUPNPrefix(domainUser.Username);
                    if (!String.IsNullOrEmpty(SAM))
                    {
                        domainUser = new DomainUsername(domainUser.Domain, SAM);
                        userData = queries.GetUserData(domainUser.Username, domainUser.Domain);
                    }
                }

                if (userData.Id == 0)
                {                    
					userData = queries.GetUserData(domainUser.Username, domainUser.Domain);
                    if (userData.Id == 0 && sc.AuthEngineSAMLoginPreferred)
                    {
                        string SAM = GetSAMByUPNPrefix(domainUser.Username);
                        if (!String.IsNullOrEmpty(SAM))
                        {
                            domainUser = new DomainUsername(domainUser.Domain, SAM);
                            userData = queries.GetUserData(domainUser.Username, domainUser.Domain);
                        }
                    }
				}
                
                if (userData.UserStatus != 1)
                    throw new ValidateUserException("Invalid user.", state);


                ProviderLogicAndPinCode providerLogicAndPinCode = GetUserProviderLogicAndPinCode(domainUser.Username, domainUser.Domain);
                IProviderLogic providerLogic = providerLogicAndPinCode.ProviderLogic;
				
				if (sc.AuthEngineChallengeResponse)
				{
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("ServerLogic.ValidateUser validating pincode", LogLevel.Debug);

					if (sc.AuthEngineAskMissingInfo) {
						if (!UsersStates.Instance.hasUsernameAndState(domainUsername, state)
						    || !UsersStates.Instance.getByUsernameAndState(domainUsername, state).SkipCheck
						   ) {
							if (Logger.I.ShouldLog(LogLevel.Debug))
								Logger.Instance.WriteToLog("ServerLogic.ValidateUser asking for pincode", LogLevel.Debug);
							var missingInfoRet = CheckMissingInfo(domainUser, state, providerLogic, false);
							if (missingInfoRet.AI)
								return missingInfoRet;
						}
					}

					UserStateDetail userStateDetail;
					if (UsersStates.Instance.hasUsernameAndState(domainUsername, state)) {
						userStateDetail = UsersStates.Instance.getByUsernameAndState(domainUsername, state);
						if (!userStateDetail.SetInfo)
							throw new ValidateUserException("State already exists but not SetInfo (ValidateUser/ChallengeResponse)", state);
					} else
						userStateDetail = UsersStates.Instance.addUsernameAndState(domainUsername, state);
					validatePinCode(
						domainUser.GetDomainUsername(),
						providerLogicAndPinCode.PinCode,
						providerLogicAndPinCode.PanicPinCode,
						validateUser.PinCode,
						userStateDetail
					);
					if (sc.NotifyPinCodeIncorrectOnAccess
					    && !userStateDetail.PinCodeValidated)
					{
						throw new ValidateUserException("Pincode validation failed", state);
					}

					if (sc.AuthEngineAskMissingInfo) {
						if (!UsersStates.Instance.hasUsernameAndState(domainUsername, state)
						    || !UsersStates.Instance.getByUsernameAndState(domainUsername, state).SkipCheck
						   ) {

							var missingInfoRet = CheckMissingInfo(domainUser, state, providerLogic, true);
							if (missingInfoRet.AI)
								return missingInfoRet;
						}
					}
					
					ValidateUserRet vur = providerLogic.ValidateUser(state, domainUser.Username, domainUser.Domain);

					vur.PinCodeEnabled = isPinCodeEnabled();
					vur.State = state;

					vur.Validated = userStateDetail.PinCodeValidated;
					vur.Panic = userStateDetail.Panic;

                    if (sc.AuthEngineMutualAuth) {
                        vur.MutualAuthChallengeMessage = GetMutualAuthChallengeMessage(domainUser.Username, domainUser.Domain);                        
                    }

                    if (vur.Validated) {
                        Tracker.Instance.TrackEvent("User Validation Successful", Tracker.Instance.DefaultEventCategory);
                    }
                    
					return vur;
				}
				else
				{
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("ServerLogic.ValidateUser validating pincode and token", LogLevel.Debug);
					if (!sc.AuthEngineStateless)
					{
						Logger.Instance.WriteToLog("Cannot use one screen login with AuthEngineStateless == No.", LogLevel.Error);
						throw new ValidateUserException("Authentication mode not supported.", state);
					}
					var pclen = providerLogic.GetPasscodeLen();

					var missingInfoRet = CheckMissingInfo(domainUser, state, providerLogic, false);
					if (missingInfoRet.AI)
						return missingInfoRet;

					if (UsersStates.Instance.hasUsernameAndState(domainUsername, state))
					{
						if (Logger.I.ShouldLog(LogLevel.Debug))
							Logger.Instance.WriteToLog(string.Format("Removing state: '{0}'.", state), LogLevel.Debug);
						UsersStates.Instance.removeUsernameAndState(domainUsername, state);
					}

					var pincodeAndToken = validateUser.PinCode;
					if (pincodeAndToken.Length < pclen)
						throw new ValidateUserException("Invalid PinCode and Token sent.", state);

					var token = pincodeAndToken.Substring((pincodeAndToken.Length - pclen), pclen);
					var pincode = pincodeAndToken.Substring(0, (pincodeAndToken.Length - pclen));

					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format("Validating pincode '{0}' token '{1}'", pincode, token), LogLevel.Debug);

					var userStateDetail = UsersStates.Instance.addUsernameAndState(domainUsername, state);
					validatePinCode(
						domainUsername,
						providerLogicAndPinCode.PinCode,
						providerLogicAndPinCode.PanicPinCode,
						pincode,
						userStateDetail
					);
					if (sc.NotifyPinCodeIncorrectOnAccess
					    && !userStateDetail.PinCodeValidated)
					{
						throw new ValidateUserException("Pincode validation failed", state);
					}

					missingInfoRet = CheckMissingInfo(domainUser, state, providerLogic, true);
					if (missingInfoRet.AI)
						return missingInfoRet;
					
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("ServerLogic.ValidateUser providerLogic.ValidateUser", LogLevel.Debug);
					ValidateUserRet vur = providerLogic.ValidateUser(state, domainUser.Username, domainUser.Domain);

					try
					{
						if (Logger.I.ShouldLog(LogLevel.Debug))
							Logger.Instance.WriteToLog("ServerLogic.ValidateUser providerLogic.ValidatePin", LogLevel.Debug);
						
						var vpr = new ValidatePinRet();
						providerLogic.ValidatePin(vpr, state, domainUser.Username, domainUser.Domain, token);						
						if (!vpr.Validated)
						{
							var emergencyProviderLogic = new EmergencyProviderLogic();
							emergencyProviderLogic.Using(sc).Using(this);
							emergencyProviderLogic.ValidatePin(vpr, state, domainUser.Username, domainUser.Domain, token);
							if (vpr.Validated)
								vpr.Error = string.Empty;
						}
						
						if (!string.IsNullOrEmpty(vpr.Error) || !vpr.Validated) {
							if (Logger.I.ShouldLog(LogLevel.Debug) && !string.IsNullOrEmpty(vpr.Error))
								Logger.Instance.WriteToLog("ValidateUser ValidatePin error: " + vpr.Error, LogLevel.Debug);
							if (Logger.I.ShouldLog(LogLevel.Debug) && !vpr.Validated)
								Logger.Instance.WriteToLog("ValidateUser ValidatePin !vpr.Validated: ", LogLevel.Debug);
							throw new ValidateUserException("ValidateUser.Pin failed", state);
						}
						
						if (Logger.I.ShouldLog(LogLevel.Debug))
							Logger.Instance.WriteToLog("ServerLogic.ValidateUser providerLogic.ValidatePin SUCCESS", LogLevel.Debug);
					}
					catch (BaseAuthException ex)
					{
						throw new ValidateUserException(ex.Message, state);
					}
					vur.PName = AuthDisabledProviderLogic.LogicName;
					vur.PinCodeEnabled = isPinCodeEnabled();
					vur.State = state;
					
					vur.Validated = userStateDetail.PinCodeValidated;
					vur.Panic = userStateDetail.Panic;

                    if (sc.AuthEngineMutualAuth) {                        
                        vur.MutualAuthChallengeMessage = GetMutualAuthChallengeMessage(domainUser.Username, domainUser.Domain);                        
                    }

                    if (vur.Validated) {
                        Tracker.Instance.TrackEvent("User Validation Successful", Tracker.Instance.DefaultEventCategory);
                    }                    

					return vur;
				}
			}
			catch (ValidateUserException vue) {
				Logger.Instance.WriteToLog("ServerLogic.ValidateUser ValidateUserException: " + vue.Message, LogLevel.Debug);                
				return new ValidateUserRet
				{
					//PName = "Error", WI expects it empty to be an error.. otherwise it redirects to check pin page
					Error = vue.Message,
					State = vue.State,
					Validated = false,
				};
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(string.Format("ServerLogic.ValidateUser ERROR: {0} - {1}", validateUser.User, ex.Message), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("ServerLogic.ValidateUser STACK: {0} - {1}", validateUser.User, ex.StackTrace), LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				
				return new ValidateUserRet
				{
					//PName = "Error", WI expects it empty to be an error.. otherwise it redirects to check pin page
					Error = "ValidateUser unexpected error",
					State = state,
					Validated = false,
				};
			}
		}

		public RetBase ResyncUser(CommandBase cmd)
		{
			Logger.Instance.WriteToLog("ServerLogic.ResyncUser", LogLevel.Debug);
			ResyncHotp resyncHotp = (ResyncHotp)cmd;
			ReplaceDomain(cmd);
			ProviderLogicAndPinCode providerLogicAndPinCode = GetUserProviderLogicAndPinCode(resyncHotp.User, resyncHotp.Org);
			IProviderLogic providerLogic = providerLogicAndPinCode.ProviderLogic;
			return providerLogic.Resync(resyncHotp.User, resyncHotp.Org, resyncHotp.Action, resyncHotp.Parameters, resyncHotp.Token1, resyncHotp.Token2);
		}

		private class ProviderLogicAndPinCode
		{
			public IProviderLogic ProviderLogic { get; set; }
			public string PinCode { get; set; }
			public string PanicPinCode { get; set; }
		}

		private ProviderLogicAndPinCode GetUserProviderLogicAndPinCode(string user, string orgname)
		{
			try
			{
				var ret = new ProviderLogicAndPinCode();
				ret.PinCode = string.Empty;
				using (var queries = DBQueriesProvider.Get())
				{
					const string query = @"
SELECT u.ID, u.AuthEnabled, u.PinCode, u.PANIC_PINCODE, up.authProviderId, up.config
FROM SMS_CONTACT u
LEFT JOIN UserProviders up
	ON u.ID = up.userId AND up.active=1 AND up.selected=1
WHERE AD_USERNAME = @USER AND ORG_NAME = @ORG
								";

					using (var dtGet = queries.Query(query,
					                                 new DBQueryParm("USER", user),
					                                 new DBQueryParm("ORG", orgname)
					                                ))
					{
						if (dtGet.Rows.Count == 0)
						{
							if (sc.AuthEngineDefaultEnabled)
								throw new ServerLogicException(string.Format("ServerLogic.Found no user '{0}' - '{1}'.", user, orgname));
							ret.ProviderLogic = ProviderLogicFactory.GetByName(ProviderLogicFactory.AuthDisabledLogic);
							return ret;
						}
						var row = dtGet.Rows[0];

						var authEnabled = false;
						object activeVal = row["AuthEnabled"];
						if (activeVal != null && activeVal != DBNull.Value)
							authEnabled = row.Field<bool>("AuthEnabled");
						if (!authEnabled)
						{
							ret.ProviderLogic = ProviderLogicFactory.GetByName(ProviderLogicFactory.AuthDisabledLogic);
							return ret;
						}

						var authProviderId = 0;
						object authProviderIdVal = row["authProviderId"];
						if (authProviderIdVal != null && authProviderIdVal != DBNull.Value)
							authProviderId = Convert.ToInt32(row["authProviderId"]);
						if (authProviderId == 0)
							throw new ServerLogicException(string.Format("ServerLogic.Found invalid authProviderId '{0}' - '{1}'.", user, orgname));

						var config = string.Empty;
						object configVal = row["config"];
						if (configVal != null && configVal != DBNull.Value)
							config = Convert.ToString(row["config"]);

						object val = row["PinCode"];
						if (val != null && val != DBNull.Value)
							ret.PinCode = Convert.ToString(row["PinCode"]);
						
						val = row["PANIC_PINCODE"];
						if (val != null && val != DBNull.Value)
							ret.PanicPinCode = Convert.ToString(row["PANIC_PINCODE"]);

						var p = sc.Providers.Find(o => o.Id == authProviderId);
						if (p != null && !string.IsNullOrEmpty(p.Name))
						{
							ret.ProviderLogic = ProviderLogicFactory.GetByName(p.Name)
								.Using(sc)
								.Using(this)
								.Using(p)
								.UsingConfig(config)
								;
							return ret;
						}
					}
				}
				throw new ServerLogicException(string.Format("GetUserProviderLogic found no logic for '{0}' - '{1}'.", user, orgname));
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.GetUserProviderLogicAndPinCode ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.GetUserProviderLogicAndPinCode STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public RetBase UpdatePassCodeLen(CommandBase cmd)
		{
			var len = (UpdateTokenLen)cmd;
			if (len.External)
				checkAdmin(len);
			var ret = new UpdateTokenLenRet();

			try
			{
				using (var queries = DBQueriesProvider.Get())
				{
					queries.NonQuery(
						@"UPDATE SETTINGS SET VALUE = @VAL WHERE OBJECT = 'SMS_SERVICE' AND SETTING = 'PASSCODE_LEN'",
						new DBQueryParm("VAL", len.Length)
					);
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.UpdatePassCodeLen ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.UpdatePassCodeLen STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error updating length";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}

		public void checkAuth(CommandBase cmd)
		{
			if (!cmd.Authenticated && !sc.AuthEngineDisableUserVerification)
				throw new PermissionException("Only authenticated users can access.");
		}
		private bool isAdmin(CommandBase cmd)
		{
			checkAuth(cmd);
			DomainUsername domainUser = GetDomainUsername(cmd.Identity.Name);
			PermissionsRet pret = (PermissionsRet)getPermissionsAndCheckSameDomain(new Permissions()
			                                                                       {
			                                                                       	Identity = cmd.Identity,
			                                                                       	User = domainUser.Username,
			                                                                       	Org = domainUser.Domain,
			                                                                       	Authenticated = cmd.Authenticated,
			                                                                       });
			if (pret.UserType == UserType.Administrator)
				return true;
			return false;
		}
		public void checkAdmin(CommandBase cmd)
		{
			if (!isAdmin(cmd))
				throw new PermissionException("User does not have permission.");
		}
		public void checkSameDomainAndAdmin(UserOrgCommand cmd)
		{
			checkAdmin(cmd);
			DomainUsername domainUser = GetDomainUsername(cmd.Identity.Name);

			cmd.User = domainUser.Username;
			PermissionsRet pret = (PermissionsRet)getPermissionsAndCheckSameDomain(cmd);
			cmd.Org = domainUser.Domain;
			if (pret.UserType != UserType.Administrator)
			{
				throw new PermissionException("User cannot access information other than own.");
			}
		}
		public void checkSameDomainSameUserOrAdmin(UserOrgCommand cmd)
		{
			checkAuth(cmd);
			DomainUsername domainUser = GetDomainUsername(cmd.Identity.Name);

			string oldUser = cmd.User;
			if (!sc.AuthEngineDisableUserVerification)
				cmd.User = domainUser.Username;
			PermissionsRet pret = (PermissionsRet)getPermissionsAndCheckSameDomain(cmd);
			if (!sc.AuthEngineDisableUserVerification)
			{
				cmd.User = oldUser;
				cmd.Org = domainUser.Domain;
			}
			if (pret.UserType != UserType.Administrator)
			{
				if (cmd.User.ToLowerInvariant() != domainUser.Username.ToLowerInvariant() && !sc.AuthEngineDisableUserVerification)
					throw new PermissionException("User cannot access information other than own.");
			}
		}

		private bool isPinCodeEnabled()
		{
			return (sc.AuthEnginePinCode == PinCodeOption.Enforced
			        || sc.AuthEnginePinCode == PinCodeOption.True);
		}

		public RetBase UserDetails(CommandBase cmd)
		{
			Logger.Instance.WriteToLog("ServerLogic.UserDetails Start", LogLevel.Debug);
			var ret = new DetailsRet();

			try
			{
				var det = (Details)cmd;
				ReplaceDomain(det);
				checkSameDomainSameUserOrAdmin(det);

				using (var queries = DBQueriesProvider.Get())
				{
					foreach (DataRow row in queries.Query(
						@"SELECT * FROM SMS_CONTACT WHERE AD_USERNAME = @USER AND ORG_NAME = @ORG ",
						new DBQueryParm(@"USER", det.User),
						new DBQueryParm(@"ORG", det.Org)
					).Rows)
					{
						ret.Fullname = row["DISPLAY_NAME"].ToString();
						ret.LastName = row.Field<string>("LastName");
						ret.FirstName = row.Field<string>("FirstName");
						ret.Phone = row["INPUT_PHONE_NUMBER"].ToString();
						ret.Mobile = row["MOBILE_NUMBER"].ToString();
						ret.Email = row["Email"].ToString();
						ret.UserType = row["USER_TYPE"].ToString();
						bool? authEnabledSql = row.Field<bool?>("AUTHENABLED");
						ret.AuthEnabled = (authEnabledSql == null || !authEnabledSql.HasValue) ? false : authEnabledSql.Value;
						ret.PinCodeEnabled = isPinCodeEnabled();
						ret.HasPinCodeEntered = ((row["PINCODE"] != DBNull.Value) && !string.IsNullOrEmpty(row["PINCODE"].ToString().Trim()));
						ret.PinCodeLength = sc.AuthEnginePinCodeLength;
						ret.Locked = row.Field<bool>("locked");
						ret.LockdownMode = this.sc.AuthEngineLockDownMode;
						ret.MobileOverrided = row.Field<bool>("mobileOverrided");
						ret.EmailOverrided = row.Field<bool>("emailOverrided");
						ret.PhoneOverrided = row.Field<bool>("phoneOverrided");
                        ret.UPN = row["UPN"].ToString();
					}
				}

				Logger.Instance.WriteToLog("ServerLogic.UserDetails Out: " + ret.Fullname + " | " + ret.Phone + " | " + ret.Mobile + " | " + ret.UserType, LogLevel.Debug);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.UserDetails ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.UserDetails STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error getting details";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}

		public RetBase ValidatePin(CommandBase cmd)
		{
            Tracker.Instance.TrackEvent("PIN Validation Attempt", Tracker.Instance.DefaultEventCategory);

            var vpin = (ValidatePin)cmd;
			var ret = new ValidatePinRet();
			string logicName = string.Empty;
			if (string.IsNullOrEmpty(vpin.State))
				vpin.State = string.Empty;
			Logger.Instance.WriteToLog(string.Format("Validating user '{0}' - '{1}' State: '{2}'", vpin.User, vpin.Pin, vpin.State), LogLevel.Debug);
			if (vpin.PinCode == null)
				vpin.PinCode = string.Empty;

			try
			{
				logicName = ValidatePin(vpin, ret);

                var domainUser = GetDomainUser(vpin.User);
                
				CheckUserVault(domainUser.Username, domainUser.Domain, ret);
			}
			catch (BaseAuthException ex)
			{
				Logger.Instance.WriteToLog(string.Format("ServerLogic.ValidatePin Auth ERROR: {0} - {1}", vpin.User, ex.Message), LogLevel.Info);
				Logger.Instance.WriteToLog(string.Format("ServerLogic.ValidatePin Auth STACK: {0} - {1}", vpin.User, ex.StackTrace), LogLevel.Debug);
				ret.Error = ex.Message;
				ret.Validated = false;
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(string.Format("ServerLogic.ValidatePin ERROR: {0} - {1}", vpin.User, ex.Message), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("ServerLogic.ValidatePin STACK: {0} - {1}", vpin.User, ex.StackTrace), LogLevel.Debug);
				ret.Error = "Error - Probably state did not match.";
				ret.Validated = false;
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			if (!string.IsNullOrEmpty(ret.Error))
				Logger.Instance.WriteToLog(string.Format("Validate error ({0}): {1}", logicName, ret.Error), LogLevel.Debug);
			Logger.Instance.WriteToLog(string.Format("Validate result ({0}): {1}", logicName, ret.Validated.ToString()), LogLevel.Debug);

			return ret;
		}

		private string ValidatePin(ValidatePin vpin, ValidatePinRet vpr)
		{
            var domainUser = GetDomainUser(vpin.User);

			UserStateDetail usd = UsersStates.Instance.getByUsernameAndState(domainUser.GetDomainUsername(), vpin.State);

			ProviderLogicAndPinCode providerLogicAndPinCode = GetUserProviderLogicAndPinCode(domainUser.Username, domainUser.Domain);

			DBQueries.UserData userData;
			using(var queries = DBQueriesProvider.Get()) {
				userData = queries.GetUserData(domainUser.Username, domainUser.Domain);
				if (userData.UserStatus != 1)
					throw new ValidationException("Invalid user.");
			}
			
			validatePinCode(domainUser.GetDomainUsername(), providerLogicAndPinCode.PinCode, providerLogicAndPinCode.PanicPinCode, vpin.PinCode, usd);

			IProviderLogic providerLogic = providerLogicAndPinCode.ProviderLogic;
			if (
				//providerLogic.UsesPincode() &&
				!usd.PinCodeValidated
			)
				throw new ValidationException("Pincode not validated.");

			providerLogic.ValidatePin(vpr, vpin.State, domainUser.Username, domainUser.Domain, vpin.Pin);
			if (!vpr.Validated)
			{
				var emergencyProviderLogic = new EmergencyProviderLogic();
				emergencyProviderLogic.Using(sc).Using(this);
				emergencyProviderLogic.ValidatePin(vpr, vpin.State, domainUser.Username, domainUser.Domain, vpin.Pin);
				if (vpr.Validated)
					vpr.Error = string.Empty;
			}
			if (vpr.Validated)
			{
				using (var queries = DBQueriesProvider.Get())
				{
					var queryResult = queries.NonQuery(string.Format(@"UPDATE [SMS_CONTACT] SET [{0}]=0, [{1}]=0 WHERE ID = @USERID", ServerLogic.TempPinCode, ServerLogic.TempPInfo), new DBQueryParm("USERID", userData.Id));
				}
			} else {
				var checkRet = providerLogic.CheckMissingUserInfo(domainUser.Username, domainUser.Domain);
				if (checkRet.AI)
					providerLogic.ClearUserInfo(domainUser.Username, domainUser.Domain);
			}
			vpr.Panic = usd.Panic;
			return providerLogic.Name;
		}

		public RetBase UserList(CommandBase cmd)
		{
			var users = (Users)cmd;

			var ret = new UsersRet();
			string filter = string.Empty;
			if (Logger.I.ShouldLog(LogLevel.Debug))
				Logger.Instance.WriteToLog("ServerLogic.UserList Start - OrgName: " + users.Org, LogLevel.Debug);
			#if DEBUG
			Logger.Instance.WriteToLog(string.Format("ServerLogic.UserList At: {0} - Total {1} ", users.At, users.Total), LogLevel.Debug);
			#endif
			try
			{
				string username = string.Empty;
				if (users.External)
				{
					checkAuth(cmd);
					DomainUsername domainUser = GetDomainUsername(users.Identity.Name);
					users.Org = domainUser.Domain;
					if (!sc.AuthEngineDisableUserVerification)
					{
						if (!isAdmin(users))
						{
							username = users.Identity.GetLogin();
							filter = " AD_USERNAME = @UN AND ";
						}
					}
				}

				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog(string.Format("ServerLogic.UserList Querying: {0} {1} users starting {2}", users.Org, users.Total, users.At), LogLevel.Debug);
				
				var hideAdminsFilter = string.Empty;
				if ( users.Admins != null ) {
					if ( users.Admins == true )
						hideAdminsFilter = string.Format(@" AND (USER_TYPE = '{0}')", UserType.Administrator);
					else
						hideAdminsFilter = string.Format(@" AND (USER_TYPE = '{0}')", UserType.User);
				}
				
				var overrideFilter = string.Empty;
				if (users.Overrided) {
					overrideFilter = @" AND (phoneOverrided=1 OR emailOverrided=1 OR mobileOverrided=1) ";
				}
				var providerFilter = string.Empty;
				if (!string.IsNullOrEmpty(users.FName)) {
					var provider = sc.Providers.SingleOrDefault(p => 
					                                            !string.IsNullOrWhiteSpace(p.FriendlyName) 
					                                            && p.FriendlyName == users.FName
					                                            );
					if ( provider == null )
						provider = sc.Providers.SingleOrDefault(p => p.Name == users.FName);
					if ( provider != null ) {
						providerFilter = string.Format(@" AND id IN (
SELECT userId FROM UserProviders WHERE authProviderId={0} AND selected=1 AND active=1)", provider.Id);
					}
				}
				var textFilter = string.Empty;
				if (!string.IsNullOrEmpty(users.Text)) {
					textFilter = @" AND (AD_USERNAME LIKE '%' + @TEXTFILTER + '%' OR LASTNAME LIKE '%' + @TEXTFILTER + '%' OR FIRSTNAME LIKE '%' + @TEXTFILTER + '%') ";
					//textFilter = @" AND (AD_USERNAME LIKE '%' + @TEXTFILTER + '%' OR DISPLAY_NAME LIKE '%' + @TEXTFILTER + '%') ";
				}
				using (var queries = DBQueriesProvider.Get())
				{
					string query = string.Format(@"

WITH ORDERED_USERS AS
(
    SELECT AD_USERNAME,DISPLAY_NAME,LASTNAME,FIRSTNAME,phoneOverrided,emailOverrided,mobileOverrided,
    ROW_NUMBER() OVER (ORDER BY AD_USERNAME) AS RowNumber
    FROM SMS_CONTACT
		WHERE {0} ORG_NAME = @ORGNAME AND userStatus <> 0 {1} {2} {3} {4}
)
SELECT AD_USERNAME,DISPLAY_NAME,LASTNAME,FIRSTNAME,phoneOverrided,emailOverrided,mobileOverrided
FROM ORDERED_USERS
WHERE RowNumber BETWEEN @STARTAT AND @STARTTO;
", filter, overrideFilter, providerFilter, hideAdminsFilter, textFilter);
					var parms = new List<DBQueryParm>();

					parms.Add(new DBQueryParm("STARTAT", users.At));
					parms.Add(new DBQueryParm("STARTTO", users.Total));
					parms.Add(new DBQueryParm("ORGNAME", users.Org));
					if (!string.IsNullOrEmpty(filter))
						parms.Add(new DBQueryParm("UN", username));
					if (!string.IsNullOrEmpty(textFilter))
						parms.Add(new DBQueryParm("TEXTFILTER", users.Text));

					var table = queries.Query(query, parms);
					var sb = new StringBuilder();
					foreach (DataRow row in table.Rows) {
						ret.Users.Add(new UserRet { 
							u = row.Field<string>("AD_USERNAME"), d = row.Field<string>("DISPLAY_NAME"),
							l = row.Field<string>("LASTNAME"), f = row.Field<string>("FIRSTNAME")
						});
					}
				}
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog("ServerLogic.UserList Count: " + ret.Users.Count, LogLevel.Debug);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.UserList ERROR Message: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.UserList ERROR Stack: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = string.Format("Error getting user list ({0})", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}

		public RetBase getPermissionsAndCheckSameDomain(UserOrgCommand cmd)
		{            
			PermissionsRet ret = new PermissionsRet();
			ret.Error = string.Format("Incorrect user Domain: '{0}' User: '{1}'", cmd.Org, cmd.User);

			if (!cmd.Authenticated)
				throw new PermissionException("Only authenticated users can validate.");            
			ReplaceDomain(cmd);

			//if (cmd.Identity.GetLogin().ToLowerInvariant() != cmd.User.ToLowerInvariant())
			//    throw new PermissionException("User request does not match.");

			DataTable table;
			using (var queries = DBQueriesProvider.Get())
			{
				var query = @"
SELECT ORG_NAME,userStatus,USER_TYPE FROM SMS_CONTACT WHERE AD_USERNAME = @USER AND ORG_NAME = @ORG
";
				table = queries.Query(query,
				                      new DBQueryParm("USER", cmd.User),
				                      new DBQueryParm("ORG", cmd.Org)
				                     );

				if (table.Rows.Count == 0)
					throw new PermissionException(string.Format("Incorrect user Domain: '{0}' User: '{1}'", cmd.Org, cmd.User));
			}
			ret.Status = false;
			ret.UserType = "";
			foreach (DataRow row in table.Rows)
			{
				string org = row["ORG_NAME"].ToString().ToLowerInvariant();
				DomainUsername domainUser = GetDomainUsername(cmd.Identity.Name);

				if (!sc.AuthEngineDisableUserVerification)
				{
					var domain = domainUser.Domain.ToLowerInvariant();
					if (domain != org)
						throw new PermissionException("User request domain does not match.");
				}
				ret.Status = (Convert.ToInt16(row["userStatus"]) == 0 ? false : true);
				ret.UserType = row["USER_TYPE"].ToString();
				ret.Error = string.Empty;
			}
			return ret;
		}

		public RetBase ValidatePermissions(CommandBase cmd)
		{
			Permissions perm = (Permissions)cmd;

			PermissionsRet ret = new PermissionsRet();
			ret.Error = string.Format("Incorrect user Domain: '{0}' User: '{1}'", perm.Org, perm.User);
			try
			{
				if (cmd.External && cmd.Identity.GetLogin().ToLowerInvariant() != perm.User.ToLowerInvariant())
					throw new PermissionException("User request does not match.");
				ret = (PermissionsRet)getPermissionsAndCheckSameDomain(perm);
				ret.AllowEmergency = sc.AuthEngineAllowEmergencyPasscode;
				ret.PinCode = isPinCodeEnabled();
			}
			catch (PermissionException ex)
			{
				ret.Error = ex.Message;
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.ValidatePermissions ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.ValidatePermissions STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error validating permissions";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			if (!string.IsNullOrEmpty(ret.Error))
			{
				Logger.Instance.WriteToLog(string.Format("ServerLogic.ValidatePermissions returned for user {0} {1} an error: {2} "
				                                         , perm.Org, perm.User
				                                         , ret.Error), LogLevel.Info);
			}
			return ret;
		}

		public RetBase UpdateFull(CommandBase cmd)
		{
			var pop = (UpdateFullDetails)cmd;

			var ret = new UpdateFullDetailsRet();
			try
			{
				ReplaceDomain(pop);
				if (pop.External)
					checkSameDomainSameUserOrAdmin(pop);
				
				long userId;
				using (var queries = DBQueriesProvider.Get())
					userId = queries.GetUserId(pop.User, pop.Org);

				if (pop.PinCode == null)
					pop.PinCode = string.Empty;

				var changeUserTypeSQL = string.Empty;
				var newPinCodeSQL = string.Empty;
				var changeLockedDownSQL = string.Empty;

				if (isAdmin(pop))
				{
					changeUserTypeSQL = @"
						, [USER_TYPE] = @UT
						, [utOverridden]=
							CASE WHEN [USER_TYPE]<>@UT THEN 1 ELSE 0 END
";
					if (sc.AuthEngineLockDownMode)
						changeLockedDownSQL = ", locked = @locked ";
				}

				var panicPincode = string.Empty;
				if (pop.PCChange)
				{
					if (sc.AuthEnginePinCode != PinCodeOption.AD && sc.AuthEnginePinCode != PinCodeOption.False)
					{
						if (sc.AuthEnginePinCode == PinCodeOption.Enforced && pop.PinCode == string.Empty)
							throw new ValidationException("Pincode field cannot be empty.");
						if (pop.PinCode != string.Empty)
						{
							if (pop.PinCode.Length != sc.AuthEnginePinCodeLength)
								throw new ValidationException(string.Format("Pincode must be {0} characters long.", sc.AuthEnginePinCodeLength));
							
							if (sc.AuthEnginePinCodePanic && pop.PinCode == TextHelper.Reverse(pop.PinCode))
								throw new ValidationException("Pincode cannot be equal when read backwards to allow panic mode.");
							panicPincode = encryption.Encrypt(CryptoHelper.HashPincode(TextHelper.Reverse(pop.PinCode)));
                            pop.PinCode = encryption.Encrypt(CryptoHelper.HashPincode(pop.PinCode));
						}

					}
					newPinCodeSQL = @", PinCode=@PINCODE, PANIC_PINCODE=@PANIC_PINCODE ";
				}
				var query = string.Format(@"
UPDATE SMS_CONTACT SET
[INPUT_PHONE_NUMBER]=@TN,
[Email]=@EMAILVAL,
[MOBILE_NUMBER]=@MOBILE,
[phoneOverrided]=
	CASE WHEN [INPUT_PHONE_NUMBER]<>@TN THEN 1 ELSE [phoneOverrided] END,
[emailOverrided]=
	CASE WHEN [Email]<>@EMAILVAL THEN 1 ELSE [emailOverrided] END,
[mobileOverrided]=
	CASE WHEN [MOBILE_NUMBER]<>@MOBILE THEN 1 ELSE [mobileOverrided] END,
[UPN]=@UPN,
[LASTNAME]=@LASTNAME,
[FIRSTNAME]=@FIRSTNAME
--[DISPLAY_NAME]=@DN
{0} {1} {2}
WHERE ID = @userId", changeUserTypeSQL, newPinCodeSQL, changeLockedDownSQL);

				if (pop.Email == null)
					pop.Email = string.Empty;
				using (var queries = DBQueriesProvider.Get())
				{
					var parms = new List<DBQueryParm> {
						new DBQueryParm(@"userID", userId),
						new DBQueryParm(@"TN", pop.Phone),
						new DBQueryParm(@"MOBILE", pop.Mobile),
						//new DBQueryParm(@"DN", pop.Fullname),
						new DBQueryParm(@"UT", pop.UserType),
						new DBQueryParm(@"PINCODE", pop.PinCode),
						new DBQueryParm(@"PANIC_PINCODE", panicPincode),
						new DBQueryParm(@"EMAILVAL", pop.Email),
						new DBQueryParm(@"locked", pop.Locked),
                        new DBQueryParm(@"UPN", pop.UPN)
					};
					if ( pop.FirstName == null )
						parms.Add(new DBQueryParm(@"FIRSTNAME", DBNull.Value));
					else
						parms.Add(new DBQueryParm(@"FIRSTNAME", pop.FirstName));
					if ( pop.LastName == null )
						parms.Add(new DBQueryParm(@"LASTNAME", DBNull.Value));
					else
						parms.Add(new DBQueryParm(@"LASTNAME", pop.LastName));
					var nonQueryResult = queries.NonQuery(query,parms);
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("ServerLogic.UpdateFull nonQueryResult: " + nonQueryResult, LogLevel.Debug);
				}
			}
			catch (ValidationException ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.UpdateFull Validation ERROR: " + ex.Message, LogLevel.Debug);
				ret.Error = ex.Message;
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.UpdateFull ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.UpdateFull STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error updating contact.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}

		private void fillIdAndEnableDisableProvider(Provider p)
		{
			try
			{
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog(string.Format("ServerLogic.fillIdAndEnableDisableProvider: Provider {0}", p.Name), LogLevel.Debug);
				fillProviderId(p);
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog(string.Format("ServerLogic.fillIdAndEnableDisableProvider: Provider {0}={1}", p.Name, p.Id), LogLevel.Debug);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(string.Format("ServerLogic.fillIdAndEnableDisableProvider ERROR: {0}", ex.Message), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("ServerLogic.fillIdAndEnableDisableProvider STACK: {0}", ex.StackTrace), LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw new Exception(string.Format("Could not find provider '{0}' in database.", p.Name));
			}
		}

		private void fillProviderId(Provider p)
		{
			using (var queries = DBQueriesProvider.Get())
			{
				p.Id = queries.QueryScalar<int>(
					@"SELECT id FROM AuthProviders WHERE name = @pName",
					new DBQueryParm(@"pName", p.Name)
				);
			}
		}
		
		public RetBase ProvidersList(CommandBase cmd)
		{
			var ret = new UserProvidersRet();
			try {
				checkAdmin(cmd);
				foreach(var provider in sc.Providers.Where(p => p.Enabled)) {
					ret.Providers.Add(new UserProvider {
					                  	Name = provider.Name
					                  	, FName = provider.FriendlyName
					                  	, Enabled = true
					                  });
				}
			} catch(Exception ex) {
				ret.Error = ex.Message;
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}

		public RetBase GetUserProviders(CommandBase cmd)
		{
			Logger.Instance.WriteToLog(string.Format("ServerLogic.GetUserProviders"), LogLevel.Debug);
			UserProviders up = (UserProviders)cmd;

			UserProvidersRet ret = new UserProvidersRet();
			try
			{
				if (cmd.External)
					checkSameDomainSameUserOrAdmin(up);
				ReplaceDomain(up);
				using (var queries = DBQueriesProvider.Get())
				{
					var query = @"
SELECT authProviderId,selected,config FROM UserProviders
WHERE active = 1 AND userId IN (SELECT ID FROM SMS_CONTACT WHERE AD_USERNAME = @USER AND ORG_NAME = @ORG )
";
					var table = queries.Query(query,
					                          new DBQueryParm(@"USER", up.User),
					                          new DBQueryParm(@"ORG", up.Org)
					                         );

					foreach (DataRow row in table.Rows)
					{
						var authProviderId = Convert.ToInt32(row[0]);
						var p = sc.Providers.Find(o => o.Id == authProviderId);
						if (p != null && !string.IsNullOrEmpty(p.Name)) {
							ret.Providers.Add(new UserProvider()
							                  {
							                  	Name = p.Name,
							                  	FName = p.FriendlyName,
							                  	Selected = Convert.ToBoolean(row[1]),
							                  	Config = ProviderLogicFactory.GetByName(p.Name)
							                  		.RemoveSecretsFromConfig(row[2].ToString())
							                  });
						} else {
							if (Logger.I.ShouldLog(LogLevel.Info))
								Logger.Instance.WriteToLog(string.Format("{0} {1} has unregistered provider with id {2} .", up.User, up.Org, authProviderId), LogLevel.Info);
						}
					}
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format("{0} {1} got {2} providers.", up.User, up.Org, ret.Providers.Count), LogLevel.Debug);
					ret.TotpWindow = sc.OATHCalcTotpWindow;
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.GetUserProviders ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.GetUserProviders STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error getting user providers.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}

		public RetBase SetUserProvider(CommandBase cmd)
		{
			Logger.Instance.WriteToLog(string.Format("ServerLogic.SetUserProvider"), LogLevel.Debug);
			var sup = (SetUserProvider)cmd;
			var ret = new SetUserProviderRet() { Locked = false };
			var currentConfig = string.Empty;
			
			var errorDetails = string.Empty;
			
			try
			{
				var shouldLockdownUser = false;
				if (cmd.External)
					checkSameDomainSameUserOrAdmin(sup);
				ReplaceDomain(sup);

				if (sup.Provider == null)
					throw new Exception("Provider is null");

				Provider p = sc.Providers.Find(o => o.Name == sup.Provider.Name);
				if (p == null || string.IsNullOrEmpty(p.Name))
					throw new Exception(string.Format("Provider does not exists '{0}'", sup.Provider.Name));

				long userId;
				using (var queries = DBQueriesProvider.Get())
					userId = queries.GetUserId(sup.User, sup.Org);

				if (sc.AuthEngineLockDownMode)
				{
					if (!isAdmin(sup))
					{
						using (var queries = DBQueriesProvider.Get())
						{
							var query = @"SELECT locked FROM SMS_CONTACT WHERE ID = @userId";
							var lockedInDatabase = queries.QueryScalar<bool>(query, new DBQueryParm(@"userId", userId));
							if (lockedInDatabase)
								throw new Exception("User is locked down, no further change can be made to provider settings. Please contact your administrator.");
							shouldLockdownUser = true;
						}
					}
				}
								
				using (var queries = DBQueriesProvider.Get())
				{
					const string query = @"UPDATE UserProviders SET selected=@PSEL WHERE userId = @userID";
					queries.NonQuery(query,
					                 new DBQueryParm(@"userID", userId),
					                 new DBQueryParm(@"PSEL", "0")
					                );
				}
				if (string.IsNullOrEmpty(sup.Provider.Config))
				{
					sup.Provider.Config = p.Config;
					if (sup.Provider.Config == null)
						sup.Provider.Config = string.Empty;
				}
				
				IProviderLogic pLogic;
				try {
					pLogic = ProviderLogicFactory.GetByName(p.Name)
						.Using(sc)
						.Using(this)
						.Using(p)
						.UsingConfig(sup.Provider.Config)
						;
				} catch( Exception ex ) {
					Logger.I.WriteToLog("SetUserProvider.GetByName ERROR: " + ex.Message, LogLevel.Error);
					Logger.I.WriteToLog("SetUserProvider.GetByName STACK: " + ex.InnerException, LogLevel.Debug);
					throw;
				}
				if (p.Name == ProviderLogicFactory.OATHCalcLogic) {
					try
					{                        
						var oathLogic = (OATHCalcProviderLogic)pLogic;
						oathLogic.ReplaceHardTokens();
						sup.Provider.Config = oathLogic.GetJoinedConfigs();                        
					} catch (InvalidSerialException) {
						throw;
					}
				}
				
				using (var queries = DBQueriesProvider.Get())
				{
					var query = @"
						UPDATE UserProviders SET selected=@PSEL,config=@PCONF
						WHERE userId = @userId
							AND authProviderId=@PID";

					ret.Out = queries.NonQuery(query,
					                           new DBQueryParm(@"userID", userId),
					                           new DBQueryParm(@"PID", p.Id),
					                           new DBQueryParm(@"PSEL", "1"),
					                           new DBQueryParm(@"PCONF", sup.Provider.Config)
					                          );
				}
				
				try {
					pLogic.PostSelect(sup.User, sup.Org, userId, p.Id, true);
				} catch( Exception ex ) {
					Logger.I.WriteToLog("SetUserProvider.PostSelect ERROR: " + ex.Message, LogLevel.Error);
					Logger.I.WriteToLog("SetUserProvider.PostSelect STACK: " + ex.InnerException, LogLevel.Debug);
					throw;
				}
				if (shouldLockdownUser)
				{
					using (var queries = DBQueriesProvider.Get())
					{
						var query = @"UPDATE SMS_CONTACT SET locked=@locked WHERE ID = @userId";
						queries.NonQuery(query,
						                 new DBQueryParm(@"userId", userId),
						                 new DBQueryParm(@"locked", true)
						                );
						ret.Locked = true;
					}
				}
				if ( ! string.IsNullOrWhiteSpace(errorDetails) ) {
					ret.Error = "There were some errors when setting provider: " + errorDetails;
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.I.WriteToLog(ret.Error, LogLevel.Debug);
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("SetUserProvider ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("SetUserProvider ERROR Trace: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error setting provider.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}

		internal void getUsersVoid()
		{
			this.getUsers();
		}

		public bool GettingUsers
		{
			get
			{
				if (!Monitor.TryEnter(lockUpdateUsersObj))
				{
					return true;
				}
				try
				{
				}
				finally
				{
					Monitor.Exit(lockUpdateUsersObj);
				}
				return false;
			}
		}

        private void AddDefaultDomain(string domain, AdReplacements replacements)
        {
            try
            {
                using (var queries = DBQueriesProvider.Get())
                {
                    Logger.Instance.WriteToLog(string.Format("ServerLogic.AddDefaultDomain: Adding domain: {0}", domain), LogLevel.Debug);
                    int added = queries.AddDomain(domain);
                    if (added > 0)
                    {
                        foreach (var replacement in replacements)
                        {
                            Logger.Instance.WriteToLog(string.Format("ServerLogic.AddDefaultDomain: Adding replacement: {0} => {1}", replacement.Key, replacement.Value), LogLevel.Debug);
                            queries.AddAlias(replacement.Value, replacement.Key);
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("AddDefaultDomain ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("AddDefaultDomain ERROR Trace: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        private void GetSpecifiedReplacements(AdReplacements replacements)
        {
            using (var queries = DBQueriesProvider.Get())
            {
                string domain = getDefaultDomain();
                var aliases = queries.GetAliases(domain);
                foreach (var alias in aliases)
                {
                    replacements.Add(alias, domain);
                    Logger.Instance.WriteToLog(string.Format("ServerLogic.GetSpecifiedReplacements: Adding domain replacement: {0} = {1}", alias, domain), LogLevel.Debug);
                }
            }
        }

        private void CheckEncryption()
        {
            try {
                using (var queries = DBQueriesProvider.Get()) {
                    queries.CheckEncryption(encryption);
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("CheckEncryption ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("CheckEncryption ERROR Trace: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

		public bool getUsers()
		{            
			Logger.Instance.WriteToLog("getUsers Begin", LogLevel.Debug);

            CheckEncryption();

			foreach (var p in sc.Providers)
			{
				if (p.Id == 0)
				{
					fillProviderId(p);
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format("ServerLogic.getUsers Provider filled id {0} ({1})", p.Name, p.Id), LogLevel.Debug);
				}
			}

			if (!Monitor.TryEnter(lockUpdateUsersObj, 200))
			{
				Logger.Instance.WriteToLog("getUsers was locked, return", LogLevel.Debug);
				return false;
			}
			try
			{
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog("getUsers Getting users", LogLevel.Debug);
				
				string lastChecksum;
				using(var queries = DBQueriesProvider.Get()) {
					lastChecksum = queries.GetSetting("CONFIG_CHK", "AUTHGATEWAY");
				}
				
				var currentChecksum = sc.GetChecksum();
				if (lastChecksum == null || lastChecksum != currentChecksum) {
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.Instance.WriteToLog("getUsers Cleaning uSNChange", LogLevel.DebugVerbose);
					ClearUSNChanged();
					using(var queries = DBQueriesProvider.Get()) {
						queries.SetSetting("CONFIG_CHK", currentChecksum, "AUTHGATEWAY");
					}
				}                
                				
				IUsersGetter usersGetter = registry.Get<IUsersGetter>();

                string defaultDomain = "";
                AdReplacements defaultReplacements = new AdReplacements();
                UserGettersConfig userGettersConfig = new UserGettersConfig(this.sc, this.onlineControllers, this.offlineControllers);
                usersGetter.GetDomainAndReplacements(userGettersConfig, out defaultDomain, defaultReplacements);
                
                if (!String.IsNullOrEmpty(defaultDomain))
                {
                    Logger.Instance.WriteToLog("Add replacements to DB start", LogLevel.Debug);
                    AddDefaultDomain(defaultDomain, defaultReplacements);

                    replacements.Add(defaultDomain, defaultDomain);
                    GetSpecifiedReplacements(replacements);
                    Logger.Instance.WriteToLog("Add replacements to DB end", LogLevel.Debug);
                }

                if (!isUsersPollingMaster) {
                    Logger.Instance.WriteToLog("getUsers: not a polling master, return", LogLevel.Debug);
                    return false;
                }

                try {
                    usersGetter.GetUsers(userGettersConfig, AddFullUser);
                }
                catch (ADPollException) {
                    StoreADPollFailureTime();
                }

                Logger.Instance.WriteToLog("getUsers Getting users End", LogLevel.Debug);
                                

				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog("getUsers Deactivating/Deleting old users", LogLevel.Debug);
				DeactivateAndDeleteOldUsers();
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog("getUsers Deactivating/Deleting old users end", LogLevel.Debug);

				return true;
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("getUsers ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("getUsers ERROR Trace: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				return false;
			}
			finally
			{
				Monitor.Exit(lockUpdateUsersObj);
			}
		}

        private void StoreADPollFailureTime()
        {
            try {
                using (var queries = DBQueriesProvider.Get()) {
                    queries.StorePollFailureTime();
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("StoreADPollFailureTime ERROR: " + ex.Message, LogLevel.Debug);
                Logger.Instance.WriteToLog("StoreADPollFailureTime ERROR Trace: " + ex.StackTrace, LogLevel.Debug);
            }
        }

        private DateTime GetLastADPollFailureTime()
        {
            DateTime dt = DateTime.MinValue;

            try {
                using (var queries = DBQueriesProvider.Get()) {
                    dt = queries.GetLastPollFailureTime();
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("GetLastADPollFailureTime ERROR: " + ex.Message, LogLevel.Debug);
                Logger.Instance.WriteToLog("GetLastADPollFailureTime ERROR Trace: " + ex.StackTrace, LogLevel.Debug);
            }

            return dt;
        }

		public void ClearUSNChanged()
		{
			try {
				using (var queries = DBQueriesProvider.Get())
				{
					queries.NonQuery(string.Format(@"UPDATE [{0}] SET [uSNChanged] = ''", DBQueries.SMS_CONTACT));
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ClearUSNChanged ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ClearUSNChanged STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}
		
		public int DeactivateAndDeleteOldUsers()
		{
			const int BATCH_SIZE = 1000;
			var hours = sc.AuthEngineRemoveUsersAfterXHours;
			if (hours == 0) // If set to 0 hours, it's disabled
				return 0;
            DateTime lastPollFailureTime = GetLastADPollFailureTime();
            if (DateTime.Now.AddHours(-hours) < lastPollFailureTime)
                return 0;

			var minutes = hours * 60;
			var deactivatedAndDeletedUsers = 0;
			try
			{
				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.I.WriteToLog(string.Format("ServerLogic.DeactivateAndDeleteOldUsers start."), LogLevel.DebugVerbose);
				var halfMinutes = Math.Truncate((decimal)(minutes / 2)) + 1;
				var idsToDeactivate = new List<string>();
				using (var queries = DBQueriesProvider.Get())
				{
					var usersToDeactivate = queries.Query(string.Format(@"
SELECT ID FROM SMS_CONTACT WHERE ( 
(UPDATE_DATE IS NULL AND CREATION_DATE < convert(datetime,DATEADD(minute, -{0}, GETDATE())))
OR UPDATE_DATE < convert(datetime,DATEADD(minute, -{0}, GETDATE()))
) AND userStatus<>0", halfMinutes));
					foreach (DataRow row in usersToDeactivate.Rows)
						idsToDeactivate.Add(row["ID"].ToString());
				}
				
				var totalUsers = 0;
				var amountUsersToConsiderDisaster = 0;
				using (var queries = DBQueriesProvider.Get())
				{
					totalUsers = queries.QueryScalar<int>("SELECT COUNT(*) FROM SMS_CONTACT");
					if ( sc.ADDisasterPercentage == 0 ) // Disable, always delete
						amountUsersToConsiderDisaster = totalUsers + 1;
					else
						amountUsersToConsiderDisaster = (int)Math.Round(totalUsers * (sc.ADDisasterPercentage / 100m));
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose)) {
						if ( amountUsersToConsiderDisaster > totalUsers )
							Logger.I.WriteToLog(string.Format("DeactivateAndDeleteOldUsers checking for disaster disabled"), LogLevel.DebugVerbose);
						else
							Logger.I.WriteToLog(string.Format("DeactivateAndDeleteOldUsers checking for disaster (affect more than {0} of {1})", amountUsersToConsiderDisaster, totalUsers), LogLevel.DebugVerbose);
					}
				}
				
				if (idsToDeactivate.Count == 0)
				{
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog(string.Format("ServerLogic.DeactivateAndDeleteOldUsers found no users to deactivate."), LogLevel.DebugVerbose);
				}
				else
				{
					if ( idsToDeactivate.Count >= amountUsersToConsiderDisaster ) {
						if (Logger.I.ShouldLog(LogLevel.Info))
							Logger.I.WriteToLog(string.Format("DeactivateAndDeleteOldUsers disaster prevention triggered, no users will be deactivated. Tried to deactivate {0} users of {1} (limit is {2}).", idsToDeactivate.Count, totalUsers, amountUsersToConsiderDisaster), LogLevel.Info);
						return 0;
					}
					using (var queries = DBQueriesProvider.Get())
					{
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.I.WriteToLog(string.Format("ServerLogic.DeactivateAndDeleteOldUsers deactivating {0} users", idsToDeactivate.Count), LogLevel.DebugVerbose);
						for (var currIdx = 0; currIdx < idsToDeactivate.Count; currIdx += BATCH_SIZE)
						{
							var deactivatedUsers = queries.NonQuery(
								@"UPDATE SMS_CONTACT SET userStatus=0 WHERE ID IN ("
								+ string.Join(",", idsToDeactivate.Skip(currIdx).Take(BATCH_SIZE))
								+ ")"
							);
							if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
								Logger.I.WriteToLog(string.Format("ServerLogic.DeactivateAndDeleteOldUsers deactivated {0} users.", deactivatedUsers), LogLevel.DebugVerbose);
							deactivatedAndDeletedUsers += deactivatedUsers;
						}
					}
				}

				var idsToDelete = new List<string>();
				using (var queries = DBQueriesProvider.Get())
				{
					var usersToDelete = queries.Query(string.Format(@"
SELECT ID FROM SMS_CONTACT WHERE ( 
(UPDATE_DATE IS NULL AND CREATION_DATE < convert(datetime,DATEADD(minute, -{0}, GETDATE())))
OR UPDATE_DATE < convert(datetime,DATEADD(minute, -{0}, GETDATE())) 
) AND userStatus=0", minutes));
					foreach (DataRow row in usersToDelete.Rows)
						idsToDelete.Add(row["ID"].ToString());
				}
				if (idsToDelete.Count == 0)
				{
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog(string.Format("ServerLogic.DeactivateAndDeleteOldUsers found no users to delete."), LogLevel.DebugVerbose);
				}
				else
				{
					if ( idsToDelete.Count >= amountUsersToConsiderDisaster ) {
						if (Logger.I.ShouldLog(LogLevel.Info))
							Logger.I.WriteToLog(string.Format("DeactivateAndDeleteOldUsers disaster prevention triggered, no users will be deleted. Tried to delete {0} users of {1} (limit is {2}).", idsToDelete.Count, totalUsers, amountUsersToConsiderDisaster), LogLevel.Info);
						return 0;
					}
					using (var queries = DBQueriesProvider.Get())
					{
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.I.WriteToLog(string.Format("ServerLogic.DeactivateAndDeleteOldUsers deleting {0} users", idsToDelete.Count), LogLevel.DebugVerbose);
						for (var currIdx = 0; currIdx < idsToDelete.Count; currIdx += BATCH_SIZE)
						{
							var deletedUsers = queries.NonQuery(
								@"DELETE FROM SMS_CONTACT WHERE ID IN ("
								+ string.Join(",", idsToDelete.Skip(currIdx).Take(BATCH_SIZE))
								+ ")"
							);
							if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
								Logger.I.WriteToLog(string.Format("ServerLogic.DeactivateAndDeleteOldUsers removed {0} users.", deletedUsers), LogLevel.DebugVerbose);
							deactivatedAndDeletedUsers += deletedUsers;
						}
					}
				}

				return deactivatedAndDeletedUsers;
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("DeleteOldUsers ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("DeleteOldUsers STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return 0;
		}

        private void PollUsersInBackground()
        {
            var thread = new System.Threading.Thread(this.getUsersVoid);
            thread.IsBackground = true;
            thread.Start();
        }

		private RetBase PollUsers(CommandBase cmd)
		{            
			var ret = new PollUsersRet();
			try
			{
				if (cmd.External)
					checkAdmin(cmd);
                if (isUsersPollingMaster)
                    PollUsersInBackground();
                else
                    using (var queries = DBQueriesProvider.Get()) {
                        queries.CommandPoll();
                    }
			}
			catch (Exception ex)
			{
				ret.Error = "An error occurred, details where logged in the server.";
				Logger.Instance.WriteToLog("PollUsers ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("PollUsers STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}

		RetBase SendToken(CommandBase cmd)
		{
			var ret = new SendTokenRet();
			try
			{
				Logger.Instance.WriteToLog("ServerLogic.SendToken", LogLevel.Debug);
				var sendTokenCmd = (SendToken)cmd;

				if (cmd.External)
					checkSameDomainSameUserOrAdmin(sendTokenCmd);
				if (sendTokenCmd.Emergency)
					checkAdmin(sendTokenCmd);

				ReplaceDomain(sendTokenCmd);
				DomainUsername domainUser = new DomainUsername(sendTokenCmd.Org, sendTokenCmd.User);
				try
				{
					if (sendTokenCmd.Emergency)
					{
						var providerLogic = new EmergencyProviderLogic();
						providerLogic.Using(sc).Using(this);
						providerLogic.SendToken(domainUser.Username, domainUser.Domain);
						ret.Pin = providerLogic.GetPin();
					}
					else
					{
						ProviderLogicAndPinCode providerLogicAndPinCode = GetUserProviderLogicAndPinCode(domainUser.Username, domainUser.Domain);
						providerLogicAndPinCode.ProviderLogic.SendToken(domainUser.Username, domainUser.Domain);
					}
				}
				catch (Exception ex)
				{
					ret.Error = ex.Message;
				}
			}
			catch (Exception ex)
			{
				ret.Error = "An error occurred, details where logged in the server.";
				Logger.Instance.WriteToLog("ServerLogic.SendToken ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.SendToken STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}
		
		RetBase ClearPinAction(CommandBase cmd)
		{
			var ret = new ClearPinRet();
			try
			{
				Logger.Instance.WriteToLog("ServerLogic.SendToken", LogLevel.Debug);
				var sendTokenCmd = (ClearPin)cmd;

				if (cmd.External)
					checkAdmin(sendTokenCmd);

				ReplaceDomain(sendTokenCmd);
				var domainUser = new DomainUsername(sendTokenCmd.Org, sendTokenCmd.User);
				using (var queries = DBQueriesProvider.Get()) {
					var userId = queries.GetUserId(domainUser.Username, domainUser.Domain);
					var parms = new List<DBQueryParm>();
					parms.Add(new DBQueryParm(@"USERID", userId));
					parms.Add(new DBQueryParm(@"PINCODE", string.Empty));

					queries.NonQuery(
						string.Format(@"UPDATE [SMS_CONTACT] SET [PINCODE] = @PINCODE,[{0}]=0 WHERE ID = @USERID", TempPinCode),
						parms);
				}
			}
			catch (Exception ex)
			{
				ret.Error = "An error occurred, details where logged in the server.";
				Logger.Instance.WriteToLog("ServerLogic.ClearPin ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.ClearPin STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}

		public string getDefaultDomain()
		{
			if (!string.IsNullOrEmpty(sc.AuthEngineDefaultDomain))
				return sc.AuthEngineDefaultDomain;
			if (NetworkInformation.LocalComputer.Status == NetworkInformation.JoinStatus.Domain)
				return NetworkInformation.LocalComputer.DomainName;
			return Environment.MachineName;
		}

		public DomainUsername GetDomainUsername(string user)
		{
            var du = identityHelper.GetElseDefaultDomainUsername(getDefaultDomain(), user);
			//Logger.Instance.WriteToLog(string.Format("GetDomainUsername: Domain '{0}' in {1} replacements", du.Domain, replacements.Count), LogLevel.Debug);

			var replacement = replacements.ReplacementFor(du.Domain);
			if (replacement == du.Domain)
			{
				return du;
			}
			if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
				Logger.Instance.WriteToLog(string.Format("GetDomainUsername: Replaced '{0}' with '{1}'", du.Domain, replacement), LogLevel.DebugVerbose);
			return new DomainUsername(replacement, du.Username);
		}

		public void ReplaceDomain(CommandBase cmd)
		{
			var cmdUserOrg = cmd as UserOrgCommand;
			if (cmdUserOrg == null)
				return;            
			cmdUserOrg.Org = replacements.ReplacementFor(cmdUserOrg.Org);            
		}

		public DomainUsername ReplaceDomainUsername(DomainUsername domainUsername)
		{
			return new DomainUsername(replacements.ReplacementFor(domainUsername.Domain), domainUsername.Username);
		}

		public void AddReplacement(string from, string to)
		{
			replacements.Add(from, to);
		}

		public void RemoveReplacement(string from)
		{
			replacements.Remove(from);
		}

		private ConcurrentQueue<string> onlineControllers { get; set; }

		private ConcurrentQueue<string> offlineControllers { get; set; }

		public void AutoDetectServersIfNecessary(string baseDn)
		{
			var autodetect = false;
			if (string.Compare(sc.ADServerAddress, "autodetect", true) == 0)
			{
				autodetect = true;
			}
			else
			{
				onlineControllers.Enqueue(sc.ADServerAddress);
				foreach (var server in sc.ExtraDCs)
				{
					if (string.Compare(server.Name, "autodetect", true) == 0)
					{
						autodetect = true;
						break;
					}
				}
			}
			if (autodetect)
			{
				Logger.Instance.WriteToLog("ServerLogic.AutoDetectServersIfNecessary Auto detecting servers", LogLevel.Info);
				try
				{
					var domainName = AdHelper.GetDNfromBaseDN(baseDn);
					foreach (var server in NsLookup.GetSRVRecords(string.Format(@"_ldap._tcp.dc._msdcs.{0}.", domainName)))
					{
						Logger.Instance.WriteToLog("ServerLogic.AutoDetectServersIfNecessary DC detected: " + server, LogLevel.Info);
						onlineControllers.Enqueue(server);
					}
				}
				catch (Exception ex)
				{
					Logger.Instance.WriteToLog("ServerLogic.AutoDetectServersIfNecessary ERROR: " + ex.Message, LogLevel.Error);
					Logger.Instance.WriteToLog("ServerLogic.AutoDetectServersIfNecessary STACK: " + ex.StackTrace, LogLevel.Debug);
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				}
			}
			else
			{
				foreach (var server in sc.ExtraDCs)
				{
					if (string.IsNullOrWhiteSpace(server.Name))
						continue;
					onlineControllers.Enqueue(server.Name);
				}
			}
		}

		public void CheckServers()
		{
			if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
				Logger.I.WriteToLog("AdOrLocalUsersGetter.CheckServers", LogLevel.DebugVerbose);

			string firstServer = string.Empty;
			string currentServer;

			try
			{
				while (offlineControllers.TryDequeue(out currentServer))
				{
					if (string.IsNullOrEmpty(firstServer))
					{
						firstServer = currentServer;
					}
					else if (firstServer == currentServer)
					{
						offlineControllers.Enqueue(currentServer);
						break;
					}
					if (AdHelper.IsServerOnline(currentServer, sc.ADUsername, sc.ADPassword))
					{
						onlineControllers.Enqueue(currentServer);
					}
					else
					{
						offlineControllers.Enqueue(currentServer);
					}
				}
				if (Logger.I.ShouldLog(LogLevel.Info))
					Logger.I.WriteToLog(string.Format(
						"Servers online {0} offline {1}"
						, onlineControllers.Count, offlineControllers.Count), LogLevel.Info);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.CheckServers ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.CheckServers STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}
		
		public void CheckUserVault(string user, string org, VaultRetBase vpr)
		{
			var vaultingEnabled = sc.PasswordVaulting;
			if ( ! vaultingEnabled )
				return;
			var password = getUserVaultPassword(user, org);
			var ldapRootNoContainer = string.Format("LDAP://{0}/{1}", sc.ADServerAddress, sc.ADBaseDN);
			vpr.ADP = true;
			try {
				vpr.ADG = AdHelper.ValidateUserAdWithGroups(ldapRootNoContainer, user, password);
				vpr.ADP = false;
				vpr.ADPASS = password;
			} catch (BadLogonException ex) {
				if (Logger.I.ShouldLog(LogLevel.Debug)) {
					Logger.I.WriteToLog(string.Format("ERROR: {0}", ex.Message), LogLevel.Debug);
					Logger.I.WriteToLog(string.Format("STACK: {0}", ex.StackTrace), LogLevel.DebugVerbose);
				}
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			} catch (ServerException ex) {
				if (Logger.I.ShouldLog(LogLevel.Debug)) {
					Logger.I.WriteToLog(string.Format("ERROR: {0}", ex.Message), LogLevel.Debug);
					Logger.I.WriteToLog(string.Format("STACK: {0}", ex.StackTrace), LogLevel.DebugVerbose);
				}
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			} catch (Exception ex) {
				Logger.I.WriteToLog(string.Format("ERROR: {0}", ex.Message), LogLevel.Error);
				Logger.I.WriteToLog(string.Format("STACK: {0}", ex.StackTrace), LogLevel.DebugVerbose);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}
		
		RetBase SetUserVault(CommandBase cmd)
		{
			var si = (SetUVault)cmd;
			var ret = new SetUVaultRet() { State = si.State };

			var vaultingEnabled = sc.PasswordVaulting;
			if ( ! vaultingEnabled ) {
				ret.Error = "Set info not allowed by configuration.";
				return ret;
			}
			
			try {
				var domainUser = GetDomainUser(si.User);				
				
				setUserVaultPassword(domainUser.Username, domainUser.Domain, si.Value);
				CheckUserVault(domainUser.Username, domainUser.Domain, ret);
				
			} catch (Exception ex) {
				ret.Error = ex.Message;
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}
		
		void setUserVaultPassword(string user, string org, string password) {
			using (var queries = DBQueriesProvider.Get())
			{
				var userId = queries.GetUserId(user, org);
				
				const string queryId = @"SELECT [id] FROM [Vault] WHERE [userId]=@USERID";
				var dt = queries.Query(queryId, new List<DBQueryParm> {
				                       	new DBQueryParm(@"USERID", userId)
				                       });
				string query;
				if (dt.Rows.Count == 0) {
					query = @"INSERT INTO [Vault]([content],[userId]) VALUES(@CONTENT,@USERID)";
				} else {
					query = @"UPDATE [Vault] SET [content]=@CONTENT WHERE [userId]=@USERID";
				}
								
				string encryptedPassword;
				if (string.IsNullOrEmpty(password)) {
					encryptedPassword = string.Empty;
				} else {
					var iters = (new Random()).Next(10, 2000);
					var salt = RandomKeyGenerator.Generate(16, RKGBase.Base32);
					var rijndaelKey = new RijndaelEnhanced(sc.VaultPassword + "CCSSMS2SWGBccssms2swgb", "2SWGBccssms2swgb", 8, 64, 256, "SHA1", salt, iters);
					encryptedPassword = salt + "$" + iters + "$" + rijndaelKey.Encrypt(password);
				 	//encryptedPassword = AESThenHMAC.SimpleEncryptWithPassword(password, sc.VaultPassword + "CCSSMS2SWGBccssms2swgb", AESThenHMAC.NewKey());
				}
				
				var parms = new List<DBQueryParm>();
				parms.Add(new DBQueryParm(@"USERID", userId));
				parms.Add(new DBQueryParm(@"CONTENT", encryptedPassword));
				queries.NonQuery(query, parms);
			}
		}
		
		string getUserVaultPassword(string user, string org) {
			DataTable dt;
			using (var queries = DBQueriesProvider.Get())
			{
				var userId = queries.GetUserId(user, org);
				var parms = new List<DBQueryParm>();
				parms.Add(new DBQueryParm(@"USERID", userId));
				const string query = @"SELECT [content] FROM [Vault] WHERE [userId]=@USERID";
				dt = queries.Query(query, parms);
			}
			if (dt.Rows.Count == 0)
				return string.Empty;
			if (dt.Rows[0].IsNull("content"))
				return string.Empty;
			var password = dt.Rows[0].Field<string>("content");
			if (string.IsNullOrEmpty(password))
				return string.Empty;
			try {
				var passwordComponents = password.Split(new [] { '$' }, 3);
				var salt = passwordComponents[0];
				var iters = Convert.ToInt32(passwordComponents[1]);
				var rijndaelKey = new RijndaelEnhanced(sc.VaultPassword + "CCSSMS2SWGBccssms2swgb", "2SWGBccssms2swgb", 8, 64, 256, "SHA1", salt, iters);
				return rijndaelKey.Decrypt(passwordComponents[2]);
			} catch {
				// If cannot be decoded, a new one is needed ;)
				return string.Empty;
			}
			//return AESThenHMAC.SimpleDecryptWithPassword(password, sc.VaultPassword + "CCSSMS2SWGBccssms2swgb", 32);
		}

        public RetBase GetDomains(CommandBase cmd)
        {
            var ret = new DomainsRet();
            try
            {
                using (var queries = DBQueriesProvider.Get())
                {
                    var domains = queries.GetDomains();
                    ret.Domains = domains; 
                }                 
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("ServerLogic.GetDomains ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.GetDomains STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error getting domains.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }

            return ret;
        }

        public RetBase GetAliases(CommandBase cmd)
        {
            var ret = new AliasesRet();

            try
            {
                Aliases aliasesCmd = (Aliases)cmd;
                using (var queries = DBQueriesProvider.Get())
                {
                    var aliases = queries.GetAliases(aliasesCmd.Domain);
                    ret.Aliases = aliases;
                }  
            }
            catch(Exception ex)
            {
                Logger.Instance.WriteToLog("ServerLogic.GetAliases ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.GetAliases STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error getting aliases.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
            return ret;
        }

        public RetBase UpdateAliases(CommandBase cmd)
        {
            Logger.Instance.WriteToLog("ServerLogic.UpdateAliases", LogLevel.Debug);
            var ret = new UpdateAliasesRet();

            try
            {
                UpdateDomainAliases updateAliasesCmd = (UpdateDomainAliases)cmd;
                using (var queries = DBQueriesProvider.Get())
                {
                    var aliases = queries.GetAliases(updateAliasesCmd.Domain);

                    var addedAliases = updateAliasesCmd.Aliases.Except(aliases).ToList();
                    var removedAliases = aliases.Except(updateAliasesCmd.Aliases).ToList();                    

                    foreach (var alias in addedAliases)
                    {
                        queries.AddAlias(updateAliasesCmd.Domain, alias);
                        Tracker.Instance.TrackEvent("Add Domain Alias", Tracker.Instance.DefaultEventCategory);
                    }

                    foreach (var alias in removedAliases)
                    {
                        queries.RemoveAlias(updateAliasesCmd.Domain, alias);
                        Tracker.Instance.TrackEvent("Remove Domain Alias", Tracker.Instance.DefaultEventCategory);
                    }
                    
                }  
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("ServerLogic.UpdateAliases ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.UpdateAliases STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error updating aliases.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }

            return ret;
        }

        public RetBase GetPanic(CommandBase cmd)
        {
            Logger.Instance.WriteToLog("ServerLogic.GetPanic", LogLevel.Debug);
            var ret = new GetPanicRet();
            try
            {
                GetPanicState getPanicCmd = (GetPanicState)cmd;
                using (var queries = DBQueriesProvider.Get())
                {
                    ret.Panic = queries.GetPanicState(getPanicCmd.User, replacements.ReplacementFor(getPanicCmd.Org));
                }

                return ret;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("ServerLogic.GetPanic ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.GetPanic STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error getting panic state.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }

            return ret;
        }

        public RetBase ResetPanic(CommandBase cmd)
        {
            Logger.Instance.WriteToLog("ServerLogic.ResetPanic", LogLevel.Debug);
            var ret = new ResetPanicRet();

            try
            {
                ResetPanicState resetPanicCmd = (ResetPanicState)cmd;
                using (var queries = DBQueriesProvider.Get())
                {
                    queries.SetPanicState(resetPanicCmd.User, replacements.ReplacementFor(resetPanicCmd.Org), false);
                }

                return ret;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("ServerLogic.ResetPanic ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.ResetPanic STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error resetting panic state.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }

            return ret;
        }

        public RetBase SetAuthImages(CommandBase cmd)
        {
            Logger.Instance.WriteToLog("ServerLogic.SetAuthImages", LogLevel.Debug);
            var ret = new SetUserAuthImagesRet();

            try {
                SetUserAuthImages setImagesCmd = (SetUserAuthImages)cmd;
                using (var queries = DBQueriesProvider.Get()) {                    
                    if (!string.IsNullOrEmpty(setImagesCmd.LeftImage))
                        queries.SetLeftImage(setImagesCmd.User, replacements.ReplacementFor(setImagesCmd.Org), setImagesCmd.LeftImage);
                    if (!string.IsNullOrEmpty(setImagesCmd.RightImage))
                        queries.SetRightImage(setImagesCmd.User, replacements.ReplacementFor(setImagesCmd.Org), setImagesCmd.RightImage);                    
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.SetAuthImages ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.SetAuthImages STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error setting mutual authorization images.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }

            return ret;
        }

        public RetBase GetAuthImages(CommandBase cmd)
        {
            Logger.Instance.WriteToLog("ServerLogic.GetAuthImages", LogLevel.Debug);
            var ret = new GetUserAuthImagesRet();

            try {
                GetUserAuthImages getImagesCmd = (GetUserAuthImages)cmd;                
                using (var queries = DBQueriesProvider.Get()) {
                    byte[] leftImageBytes, rightImageBytes;
                    long leftImageId, rightImageId;
                    queries.GetImages(getImagesCmd.User, replacements.ReplacementFor(getImagesCmd.Org), out leftImageBytes, out rightImageBytes, out leftImageId, out rightImageId);
                    ret.LeftImageBytes = leftImageBytes;
                    ret.RightImageBytes = rightImageBytes;
                    ret.LeftImageId = leftImageId;
                    ret.RightImageId = rightImageId;                    
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.GetAuthImages ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.GetAuthImages STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error getting mutual authorization images.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }

            return ret;
        }

        public RetBase StoreAuthImage(CommandBase cmd)
        {            
            var ret = new StoreImageRet();

            try {
                StoreImage storeImageCmd = (StoreImage)cmd;
                using (var queries = DBQueriesProvider.Get()) {
                    queries.StoreImage(storeImageCmd.Url, storeImageCmd.Category, storeImageCmd.ImageBytes);
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.StoreAuthImage ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.StoreAuthImage STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error storing image.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }

            return ret;
        }

        public RetBase GetImage(CommandBase cmd)
        {
            var ret = new GetImageRet();

            try {
                GetImage getImageCmd = (GetImage)cmd;
                using (var queries = DBQueriesProvider.Get()) {
                    ret.ImageBytes = queries.GetImage(getImageCmd.Url);
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.GetImage ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.GetImage STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error getting image.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }

            return ret;
        }

        public RetBase GetImageCategories(CommandBase cmd)
        {
            var ret = new GetImageCategoriesRet();

            try {
                GetImageCategories getImageCategoriesCmd = (GetImageCategories)cmd;
                using (var queries = DBQueriesProvider.Get()) {
                    ret.Categories = queries.GetImageCategories();
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.GetImageCategories ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.GetImageCategories STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error getting image categories.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }

            return ret;
        }

        public RetBase GetAuthImagesByCategory(CommandBase cmd)
        {
            var ret = new GetImagesByCategoryRet();

            try {
                GetImagesByCategory getImagesByCategoryCmd = (GetImagesByCategory)cmd;
                using (var queries = DBQueriesProvider.Get()) {
                    ret.Urls = queries.GetImageUrlsByCategory(getImagesByCategoryCmd.Category);
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.GetImagesByCategory ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.GetImagesByCategory STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error getting images.";                
            }

            return ret;
        }

        private string GetAeInstanceName()
        {            
            return string.Format("{0} {1}", System.Net.Dns.GetHostName(), MACAddress.Get());
        }        

        private void ExecuteCommand(string command)
        {
            string[] commands = command.Split(',');

            foreach (string cmd in commands){
                string[] words = cmd.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                switch (words[0]) {
                    case "PREFERENCE":
                        try {
                            PollingPreference preference = (PollingPreference)int.Parse(words[1]); 
                            SetUsersPollingPreference(preference);
                        }
                        catch {
                            throw new Exception("Cannot set users polling preference: wrong command format.");
                        }
                        break;
                    case "IMAGES_PREFERENCE":
                        try {
                            PollingPreference preference = (PollingPreference)int.Parse(words[1]);
                            SetImagesPollingPreference(preference);
                        }
                        catch {
                            throw new Exception("Cannot set images polling preference: wrong command format.");
                        }
                        break;                    
                    case "MASTER":
                        try {
                            SetUsersPollingMasterStatus(bool.Parse(words[1]));                            
                        }
                        catch {
                            throw new Exception("Cannot set users polling master status: wrong command format.");
                        }
                        break;
                    case "IMAGES_MASTER":
                        try {
                            SetImagesPollingMasterStatus(bool.Parse(words[1]));
                        }
                        catch {
                            throw new Exception("Cannot set images polling master status: wrong command format.");
                        }
                        break;
                    case "POLL":
                        PollUsersInBackground();
                        break;
                    default:
                        throw new Exception("Unknown command: " + words[0]);                        
                }
            }
        }

        private void UpdateUsersPollingMasterStatus(List<PollingServer> pollingServers)
        {
            string instanceName = GetAeInstanceName();

            pollingServers = pollingServers.OrderBy(x => x.UsersPollingPreference).ThenBy(x => x.InstanceName).ToList();

            if (isUsersPollingMaster) {
                // check for another masters
                List<PollingServer> masters = pollingServers.Where(x => x.IsUsersPollingMaster).ToList();
                if (masters.Count > 1) {
                    List<PollingServer> otherMasters = masters;
                    otherMasters.RemoveAll(x => x.InstanceName == instanceName);

                    foreach (PollingServer otherMaster in otherMasters) {
                        if (sc.AuthEnginePollingPreference < otherMaster.UsersPollingPreference) { // lower priority: takeover
                            Logger.Instance.WriteToLog("ServerLogic.Heartbeat detected a lower preference users polling master: " + otherMaster.InstanceName, LogLevel.Debug);
                            using (var queries = DBQueriesProvider.Get()) {
                                queries.CommandMasterStatus(otherMaster.InstanceName, false);
                            }
                        }
                        else if (sc.AuthEnginePollingPreference == otherMaster.UsersPollingPreference) { // same priority: back off, log a warning
                            Logger.Instance.WriteToLog("ServerLogic.Heartbeat detected an equal preference users polling master: " + otherMaster.InstanceName, LogLevel.Debug);
                            SetUsersPollingMasterStatus(false);
                        }
                        else { // higher priority: back off
                            Logger.Instance.WriteToLog("ServerLogic.Heartbeat detected a higher preference users polling master: " + otherMaster.InstanceName, LogLevel.Debug);
                            SetUsersPollingMasterStatus(false);
                        }
                    }
                }
            }
            else if (pollingServers[0].InstanceName == instanceName) {// not a master but found itself on top:
                List<PollingServer> masters = pollingServers.Where(x => x.IsUsersPollingMaster).ToList();
                if (masters.Count == 0) {
                    // no masters in the table:  become a master
                    SetUsersPollingMasterStatus(true);
                }
                else {
                    foreach (PollingServer otherMaster in masters) {
                        if (sc.AuthEnginePollingPreference < otherMaster.UsersPollingPreference) { // lower priority: takeover
                            Logger.Instance.WriteToLog("ServerLogic.Heartbeat detected a lower preference users polling master: " + otherMaster.InstanceName, LogLevel.Debug);
                            SetUsersPollingMasterStatus(true);
                            using (var queries = DBQueriesProvider.Get()) {
                                queries.CommandMasterStatus(otherMaster.InstanceName, false);
                            }
                        }
                        else if (sc.AuthEnginePollingPreference == otherMaster.UsersPollingPreference) { // same priority: do nothing, log a warning if both are most preferred
                            if (sc.AuthEnginePollingPreference == PollingPreference.MostPreferred)
                                Logger.Instance.WriteToLog("ServerLogic.Heartbeat WARNING: detected another MostPreferred users polling server: " + otherMaster.InstanceName, LogLevel.Info);
                        }
                        else { // higher priority: shouldnt happen, or sorting error
                            throw new Exception("Heartbeats aren't sorted by users polling preference.");
                        }
                    }
                }
            }
        }

        private void UpdateImagesPollingMasterStatus(List<PollingServer> pollingServers)
        {
            string instanceName = GetAeInstanceName();

            pollingServers = pollingServers.OrderBy(x => x.ImagesPollingPreference).ThenBy(x => x.InstanceName).ToList();

            if (isImagesPollingMaster) {
                // check for another masters
                List<PollingServer> masters = pollingServers.Where(x => x.IsImagesPollingMaster).ToList();
                if (masters.Count > 1) {
                    List<PollingServer> otherMasters = masters;
                    otherMasters.RemoveAll(x => x.InstanceName == instanceName);

                    foreach (PollingServer otherMaster in otherMasters) {
                        if (sc.MutualAuthImagesPollingPreference < otherMaster.ImagesPollingPreference) { // lower priority: takeover                            
                            using (var queries = DBQueriesProvider.Get()) {
                                queries.CommandImagesMasterStatus(otherMaster.InstanceName, false);
                            }
                        }
                        else if (sc.MutualAuthImagesPollingPreference == otherMaster.ImagesPollingPreference) { // same priority: back off, log a warning                            
                            SetImagesPollingMasterStatus(false);
                        }
                        else { // higher priority: back off                            
                            SetImagesPollingMasterStatus(false);
                        }
                    }
                }
            }
            else if (pollingServers[0].InstanceName == instanceName) {// not a master but found itself on top:
                List<PollingServer> masters = pollingServers.Where(x => x.IsImagesPollingMaster).ToList();
                if (masters.Count == 0) {
                    // no masters in the table:  become a master
                    SetImagesPollingMasterStatus(true);
                }
                else {
                    foreach (PollingServer otherMaster in masters) {
                        if (sc.MutualAuthImagesPollingPreference < otherMaster.ImagesPollingPreference) { // lower priority: takeover                            
                            SetImagesPollingMasterStatus(true);
                            using (var queries = DBQueriesProvider.Get()) {
                                queries.CommandImagesMasterStatus(otherMaster.InstanceName, false);
                            }
                        }
                        else if (sc.MutualAuthImagesPollingPreference == otherMaster.ImagesPollingPreference) { // same priority: do nothing, log a warning if both are most preferred
                            if (sc.MutualAuthImagesPollingPreference == PollingPreference.MostPreferred)
                                Logger.Instance.WriteToLog("ServerLogic.Heartbeat WARNING: detected another MostPreferred images polling server: " + otherMaster.InstanceName, LogLevel.Debug);
                        }
                        else { // higher priority: shouldnt happen, or sorting error
                            throw new Exception("Heartbeats aren't sorted by images polling preference.");
                        }
                    }
                }
            }
        }

        private void UpdatePollingMasterStatuses()
        {            
            using (var queries = DBQueriesProvider.Get()) {                
                DataTable aliveServers = queries.GetAliveServers();
                List<PollingServer> pollingServers = GetPollingServersFromDataTable(aliveServers);

                UpdateUsersPollingMasterStatus(pollingServers);
                UpdateImagesPollingMasterStatus(pollingServers);
                
            }
        }                

        internal void Heartbeat()
        {
            try {
                Logger.Instance.WriteToLog("Heartbeat", LogLevel.Debug);
                using (var queries = DBQueriesProvider.Get()) {                    
                    string instanceName = GetAeInstanceName();
                    queries.WriteHeartbeat(instanceName, (int)sc.AuthEnginePollingPreference, isUsersPollingMaster, (int)sc.MutualAuthImagesPollingPreference, isImagesPollingMaster);
                    queries.DeleteOldHeartbeats(sc.AuthEngineWaitingInterval);

                    List<string> commands = queries.FetchCommands(instanceName);                    
                    foreach (string command in commands) {
                        Logger.Instance.WriteToLog("ServerLogic.Heartbeat received command: "+ command, LogLevel.Debug);
                        ExecuteCommand(command);
                    }   
                                                                         
                }

                UpdatePollingMasterStatuses();
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.Heartbeat ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.Heartbeat STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        private class PollingServer
        {
            public string InstanceName;            
            public PollingPreference UsersPollingPreference;
            public PollingPreference ImagesPollingPreference;
            public bool IsUsersPollingMaster;
            public bool IsImagesPollingMaster;
        }

        private List<PollingServer> GetPollingServersFromDataTable(DataTable dt)
        {
            List<PollingServer> servers = new List<PollingServer>();
            
            foreach (DataRow row in dt.Rows) {
                PollingServer server = new PollingServer();

                server.InstanceName = row.Field<string>(0);                
                server.UsersPollingPreference = (PollingPreference)row.Field<int>(1);
                server.IsUsersPollingMaster = row.Field<bool>(2);
                server.ImagesPollingPreference = (PollingPreference)row.Field<int>(3);
                server.IsImagesPollingMaster = row.Field<bool>(4);

                servers.Add(server);
            }

            return servers;
        }

        public RetBase GetAliveServers(CommandBase cmd)
        {
            var ret = new GetAliveServersRet();

            try {
                Logger.Instance.WriteToLog("GetAliveServers", LogLevel.Debug);
                GetAliveServers getServersCmd = (GetAliveServers)cmd;
                using (var queries = DBQueriesProvider.Get()) {

                    DataTable aliveServers = queries.GetAliveServers();
                    List<PollingServer> pollingServers = GetPollingServersFromDataTable(aliveServers);
                    
                    List<GetAliveServersRet.PollingServer> servers = new List<GetAliveServersRet.PollingServer>();                    

                    foreach (PollingServer pollingServer in pollingServers) {
                        GetAliveServersRet.PollingServer server = new GetAliveServersRet.PollingServer();
                        
                        server.Hostname = pollingServer.InstanceName.Split(' ')[0];
                        server.MACAddress = pollingServer.InstanceName.Split(' ')[1];

                        server.UsersPollingPreference = pollingServer.UsersPollingPreference;
                        server.ImagesPollingPreference = pollingServer.ImagesPollingPreference;

                        servers.Add(server);
                    }

                    ret.Servers = servers;
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.GetAliveServers ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.GetAliveServers STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error getting alive servers.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }

            return ret;
        }

        private void SetUsersPollingPreference(PollingPreference preference)
        {
            sc.AuthEnginePollingPreference = preference;
            Tracker.Instance.TrackEvent("Change Polling Preference", Tracker.Instance.DefaultEventCategory);
        }

        private void SetImagesPollingPreference(PollingPreference preference)
        {
            sc.MutualAuthImagesPollingPreference = preference;
        }

        private void SetUsersPollingMasterStatus(bool masterStatus)
        {
            isUsersPollingMaster = masterStatus;
            Logger.Instance.WriteToLog("ServerLogic.SetUsersPollingMasterStatus: " + isUsersPollingMaster, LogLevel.Info);
        }

        private void SetImagesPollingMasterStatus(bool masterStatus)
        {
            isImagesPollingMaster = masterStatus;
            Logger.Instance.WriteToLog("ServerLogic.SetImagesPollingMasterStatus: " + isImagesPollingMaster, LogLevel.Debug);
        }

        public RetBase SetServerPreferences(CommandBase cmd)
        {
            Logger.Instance.WriteToLog("ServerLogic.SetServerPreferences ", LogLevel.Debug);
            var ret = new SetServerPreferencesRet();

            try {
                SetServerPreferences setServerPreferencesCmd = (SetServerPreferences)cmd;
                string instanceName = string.Format("{0} {1}", setServerPreferencesCmd.Hostname, setServerPreferencesCmd.MACAddress);
                if (instanceName == GetAeInstanceName()) {
                    SetUsersPollingPreference((PollingPreference)setServerPreferencesCmd.UsersPollingPreference);
                    //SetImagesPollingPreference((PollingPreference)setServerPreferencesCmd.ImagesPollingPreference);
                }
                else {                    
                    using (var queries = DBQueriesProvider.Get()) {
                        queries.CommandUsersPollingPreference(instanceName, (int)setServerPreferencesCmd.UsersPollingPreference);
                        //queries.CommandImagesPollingPreference(instanceName, (int)setServerPreferencesCmd.ImagesPollingPreference);
                    }
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.SetServerPreferences ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.SetServerPreferences STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error setting server preferences.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }

            return ret;
        }

        public RetBase GetImagesPollingMasterStatus(CommandBase cmd)
        {            
            var ret = new GetImagesPollingMasterStatusRet();
            ret.IsImagesPollingMaster = isImagesPollingMaster;         
            return ret;
        }

        public RetBase ApplyOATHCalcDefaults(CommandBase cmd)
        {
            var ret = new ApplyOATHCalcDefaultsRet();

            try {
                ApplyOATHCalcDefaults applyOATHCalcDefaultsCmd = (ApplyOATHCalcDefaults)cmd;
                using (var queries = DBQueriesProvider.Get()) {
                    queries.ApplyOATHCalcDefaults(applyOATHCalcDefaultsCmd.DefaultConfig);
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.ApplyOATHCalcDefaults ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.ApplyOATHCalcDefaults STACK: " + ex.StackTrace, LogLevel.Debug);
                ret.Error = "Error applying OATHCalc defaults.";                
            }

            return ret;
        }

        private void StoreReportDate(string aeInstance)
        {
            try {
                using (var queries = DBQueriesProvider.Get()) {
                    queries.StoreReportTime(aeInstance);
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.StoreReportDate ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.StoreReportDate STACK: " + ex.StackTrace, LogLevel.Debug);                
            }

        }

        private DateTime RetrieveLastReportDate(string instanceName)
        {
            DateTime datetime = DateTime.MaxValue;
            try {                
                using (var queries = DBQueriesProvider.Get()) {
                    datetime = queries.GetLastReportTime(instanceName);
                }                
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.RetrieveLastReportDate ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.RetrieveLastReportDate STACK: " + ex.StackTrace, LogLevel.Debug);
            }
            return datetime;
        }

        internal void Report(bool checkLastReportDate = false)
        {
            try {

                DateTime lastReportDate = RetrieveLastReportDate(GetAeInstanceName());

                if (DateTime.Now.AddDays(-1) < lastReportDate)
                    return;

                Logger.Instance.WriteToLog("ServerLogic.Report", LogLevel.Debug);

                string eventCategory = "AuthEngineMonitorEvent";

                Tracker.Instance.TrackEvent("AuthEngine Running", eventCategory);                

                foreach (var provider in sc.Providers.Where(p => p.Enabled)) {
                    Tracker.Instance.TrackEvent(provider.Name + " Provider On", eventCategory);
                }

                //Tracker.Instance.TrackCollectedEvents();
                Tracker.Instance.ForceSync();

                StoreReportDate(GetAeInstanceName());
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ServerLogic.Report ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ServerLogic.Report STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
        }
        
	}

}
