using System;
using System.Collections.Generic;
using System.Data;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.AuthEngine.Logic.Helpers;
using AuthGateway.AuthEngine.ProviderLogic;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;

namespace AuthGateway.AuthEngine.Logic.ProviderLogic
{
	public abstract class BaseSendTokenProviderLogic : IProviderLogic
	{
		protected SystemConfiguration sc;
		protected ServerLogic serverLogic;
		protected string config;
		protected Provider provider;

		public virtual SetInfoRet CheckMissingUserInfo(string user, string org)
		{
			var ret = new SetInfoRet();
			var foundUser = false;
			var foundData = false;
			using (var queries = DBQueriesProvider.Get())
			{
				string query = @"SELECT ID," + DestinyField + @",ORG_NAME," + ServerLogic.TempPInfo
					+ @" FROM SMS_CONTACT WHERE AD_USERNAME = @USER AND ORG_NAME = @ON ";

				var table = queries.Query(
					query,
					new DBQueryParm(@"USER", user),
					new DBQueryParm(@"ON", org)
					);
				foundUser = table.Rows.Count > 0;
				if (foundUser)
				{
					DataRow row = table.Rows[0];
					var isTemporaryInfo = row.Field<bool>(ServerLogic.TempPInfo);
					if (isTemporaryInfo)
						foundData = false;
					else if (!String.IsNullOrEmpty(row[DestinyField].ToString()))
						foundData = true;
				}
			}
			ret.AI = foundUser && !foundData;
			if (ret.AI)
			{
				ret.AIF = DestinyField;
				ret.Extra = DestinyFieldMissingError;
			}
			return ret;
		}

		public virtual bool SetMissingUserInfo(string user, string org, string field, string userVal) 
		{
			var pinCode = string.Empty;
			
			var checkRet = CheckMissingUserInfo(user, org);
			if (!checkRet.AI)
				throw new ValidationException("User does not have missing info.");
			try {
				DestinyFieldValidate(userVal);
			} catch (Exception ex) {                
				return false;
			}
			if (field == DestinyField && !string.IsNullOrEmpty(userVal))
			{
				using (var queries = DBQueriesProvider.Get())
				{
					var userId = queries.GetUserId(user, org);

					var parms = new List<DBQueryParm>();
					parms.Add(new DBQueryParm(@"USERID", userId));
					parms.Add(new DBQueryParm(@"userVal", userVal));
					var queryResult = queries.NonQuery(string.Format(@"UPDATE [SMS_CONTACT] SET [{0}] = @userVal, [{1}] = 1 WHERE ID = @USERID", field, ServerLogic.TempPInfo), parms);
					
					if ( DestinyField == "Email" ) {
						queries.NonQuery(@"UPDATE [SMS_CONTACT] SET [emailOverrided]=1 WHERE ID = @USERID", new DBQueryParm(@"USERID", userId));
					}
				}
				return true;
			}
			return false;
		}
		
		public virtual void ClearUserInfo(string user, string org) {
			using (var queries = DBQueriesProvider.Get())
			{
				var userId = queries.GetUserId(user, org);

				var parms = new List<DBQueryParm>();
				parms.Add(new DBQueryParm(@"USERID", userId));
				parms.Add(new DBQueryParm(@"userVal", string.Empty));
				var queryResult = queries.NonQuery(string.Format(@"UPDATE [SMS_CONTACT] SET [{0}] = @userVal, [{1}] = 1 WHERE ID = @USERID", DestinyField, ServerLogic.TempPInfo), parms);
				
				if ( DestinyField == "Email" ) {
					queries.NonQuery(@"UPDATE [SMS_CONTACT] SET [emailOverrided]=0 WHERE ID = @USERID", new DBQueryParm(@"USERID", userId));
				}
			}
		}

		public abstract string DestinyField { get; }
		public virtual void DestinyFieldValidate(string value) { 
		}
		protected virtual string DestinyFieldMissingError { get { return string.Format("Required field {0} missing", DestinyField); } }

		protected virtual string TemplateLabel { 
			get { return "Passcode SMS Message"; }
		}
		
		protected const string ERROR_SEND_AHEAD = @"Error Sending Passcode: You have attempted to send ahead a one time passcode but you also have Challenge Response mode activated, sending ahead passcodes is not required in Challenge Response mode.";

		public virtual int TokenExpireTime() {
			return sc.CloudSMSTokenExpireTime;
		}
		
		public IProviderLogic Using(ServerLogic serverLogic)
		{
			this.serverLogic = serverLogic;
			return this;
		}
		public IProviderLogic Using(Provider provider)
		{
			this.provider = provider;
			return this;
		}
		public IProviderLogic UsingConfig(string config)
		{
			this.config = config;
			return this;
		}
		public abstract IProviderLogic Using(SystemConfiguration sc);

		public abstract int InsertTokensAva(Int64 Value);

		public string RemoveSecretsFromConfig(string config)
		{
			return config;
		}

		public bool InsertTokenInDB(string state, String user, String org, String Pin, int expireTimeMinutes)
		{
			try
			{
				using (var queries = DBQueriesProvider.Get())
				{
					var parms = new List<DBQueryParm>();
					parms.Add(new DBQueryParm(@"USER", user.TrimEnd()));
					parms.Add(new DBQueryParm(@"PIN", Pin.TrimEnd()));
					parms.Add(new DBQueryParm(@"ON", org));
					parms.Add(new DBQueryParm(@"STATE", state));
					parms.Add(new DBQueryParm(@"DUEDATE", expireTimeMinutes));
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("User: " + user + " - Pin: " + Pin, LogLevel.Debug);
					queries.NonQuery(
						@"INSERT INTO SMS_LOG (AD_USERNAME,PASSCODE,ORG_NAME,STATE,DUE_DATE)
						VALUES(@USER,@PIN,@ON,@STATE,(dateadd(minute,(@DUEDATE),getdate())))", parms);
					Logger.Instance.WriteToLog("Successfully inserted SMS code.", LogLevel.Debug);
				}
				return true;
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + ".InsertTokenInDB ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + ".InsertTokenInDB STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				return false;
			}
		}

		public virtual ValidateUserRet ValidateUser(string state, string user, string org)
		{
            Tracker.Instance.TrackEvent("User Validation Attempt with " + Name, Tracker.Instance.DefaultEventCategory);

			var ret = new ValidateUserRet {
				PName = Name
			};

			const string USERNAME_LABEL = "{username}";
			const string FULLNAME_LABEL = "{fullname}";
			const string FIRSTNAME_LABEL = "{firstname}";
			const string LASTNAME_LABEL = "{lastname}";

			string destiny = string.Empty;
			string fullname = string.Empty;
			var firstName = string.Empty;
			var lastName = string.Empty;
			var foundDestinyData = false;
			var foundUser = false;

			try
			{
				var tm = new TemplateMessage {
					Message = "Your secure passcode is: {passcode}"
				};
				using (var queries = DBQueriesProvider.Get())
				{
					string query = @"SELECT ID," + DestinyField + ",DISPLAY_NAME,[FIRSTNAME],[LASTNAME],ORG_NAME FROM SMS_CONTACT WHERE AD_USERNAME = @USER";
					query += " AND ORG_NAME = @ON ";

					var table = queries.Query(
						query,
						new DBQueryParm(@"USER", user),
						new DBQueryParm(@"ON", org)
						);
					foundUser = table.Rows.Count > 0;
					foreach (DataRow row in table.Rows)
					{
						if (!String.IsNullOrEmpty(row[DestinyField].ToString()) || sc.BaseSendTokenTestMode)
						{
							org = row["ORG_NAME"].ToString();
							fullname = row["DISPLAY_NAME"].ToString();
							firstName = row.Field<string>("FIRSTNAME");
							lastName = row.Field<string>("LASTNAME");
							destiny = row[DestinyField].ToString();
							foundDestinyData = true;
						}
					}
					tm = queries.GetTemplateMessage(TemplateLabel);
				}
				if (foundDestinyData)
				{
					String Passcode = RandomKeyGenerator.Generate(GetPasscodeLen()
							, sc.AuthEngineKeyBase);
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("ValidateUser User: " + user + " | Passcode: " + Passcode, LogLevel.Debug);

					InsertTokenInDB(state, user, org, Passcode, TokenExpireTime());

					string messageTemplate = tm.Message;
					messageTemplate = messageTemplate.Replace(USERNAME_LABEL, user);
					messageTemplate = messageTemplate.Replace(FULLNAME_LABEL, fullname);
					messageTemplate = messageTemplate.Replace(FIRSTNAME_LABEL, firstName);
					messageTemplate = messageTemplate.Replace(LASTNAME_LABEL, lastName);

					sendTokenToUser(ret, destiny, Passcode, messageTemplate, tm);
				}
				else
				{
					if (foundUser && sc.AuthEngineAskMissingInfo && sc.AuthEngineAskProviderInfo)
					{
						ret.AI = true;
						ret.AIF = DestinyField;
						ret.Extra = DestinyFieldMissingError;
					} else 
						ret.Error = "Domain\\Username or required field to send token not found";
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + ".ValidateUser ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + ".ValidateUser STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error validating user and/or sending message";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

            if (string.IsNullOrEmpty(ret.Error)) {
                Tracker.Instance.TrackCustomEvent("User Validation Success with " + Name, Tracker.Instance.DefaultEventCategory, MACAddress.Get());
            }

			return ret;
		}

		protected abstract void sendTokenToUser(ValidateUserRet ret, string destiny, string pin, string messageTemplate, TemplateMessage tm);

		public RetBase Resync(string user, string org, string action, string parameters, string token1, string token2)
		{
			throw new Exception("Resync not supported by " + this.Name);
		}

		public bool UsesPincode()
		{
			return true;
		}

		public virtual int GetPasscodeLen()
		{
			try
			{
				using (var queries = DBQueriesProvider.Get())
				{
					var len = queries.QueryScalar<string>("SELECT VALUE FROM SETTINGS WHERE OBJECT = 'SMS_SERVICE' AND SETTING = 'PASSCODE_LEN'");
					int length;
					if (!string.IsNullOrEmpty(len) && Int32.TryParse(len, out length))
						return length;
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + ".GetPasscodeLen ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + ".GetPasscodeLen STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return 6;
		}

		public abstract void SendToken(string user, string org);

		public abstract string Name { get; }
		
		protected virtual bool SkipsState { get { return false; } }

		public virtual void ValidatePin(ValidatePinRet ret, string state, string user, string org, string pin)
		{
            Tracker.Instance.TrackEvent("PIN Validation Attempt with" + Name, Tracker.Instance.DefaultEventCategory);
            
			ret.Validated = false;

			try
			{
				using (var queries = DBQueriesProvider.Get())
				{
					string query = @"SELECT ID,PASSCODE,DUE_DATE FROM SMS_LOG WHERE invalid=0 AND AD_USERNAME=@USER AND PASSCODE=@PIN";
					if (!SkipsState)
						query += @" AND STATE=@STATE";
					if (!string.IsNullOrEmpty(org))
						query += @" AND ORG_NAME=@ON ";

					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format("ValidatePin query: {0}", query), LogLevel.Debug);

					var parms = new List<DBQueryParm>();
					parms.Add(new DBQueryParm(@"USER", user));
					parms.Add(new DBQueryParm(@"PIN", pin));
					parms.Add(new DBQueryParm(@"ON", org));
					parms.Add(new DBQueryParm(@"STATE", state));

					var table = queries.Query(query, parms);
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format(@"ValidatePin got '{0}' valid tokens for '{2}\{1}' state '{3}'.",
							table.Rows.Count, user, org, state)
							, LogLevel.Debug);
					if (table.Rows.Count == 0)
						return;

					int rowId = 0;
					String PinOut = string.Empty;
					DateTime Due_Date = DateTime.MinValue;

					foreach (DataRow row in table.Rows)
					{
						rowId = Convert.ToInt32(row["ID"]);
						PinOut = row["PASSCODE"].ToString();
						Due_Date = DateTime.Parse(row["DUE_DATE"].ToString());
					}

					var nowDateTime = DateTime.Now;
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format("{0}.ValidatePin now is '{1}' and should be before '{2}'", Name, nowDateTime.ToString(), Due_Date.ToString()), LogLevel.Debug);

					if (nowDateTime < Due_Date)
					{
						if (Logger.I.ShouldLog(LogLevel.Debug))
							Logger.Instance.WriteToLog(string.Format("{0}.ValidatePin date correct, now compare supplied '{1}' with existing '{2}'", Name, pin, PinOut), LogLevel.Debug);
						ret.Validated = String.Equals(pin, PinOut, StringComparison.InvariantCultureIgnoreCase);
						InvalidatePriorAndCurrentTokens(org, user, rowId);
					} else {
						if (Logger.I.ShouldLog(LogLevel.Debug))
							Logger.Instance.WriteToLog(string.Format("{0}.ValidatePin expired token", Name), LogLevel.Debug);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + ".ValidatePin ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + ".ValidatePin STACK: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error validating pin";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

            if (string.IsNullOrEmpty(ret.Error)) {
                Tracker.Instance.TrackCustomEvent("PIN Validation Success with" + Name, Tracker.Instance.DefaultEventCategory, MACAddress.Get());
            }
		}

		private void InvalidatePriorAndCurrentTokens(string org, string user, int rowId)
		{
#if DEBUG
			Logger.Instance.WriteToLog(this.Name + string.Format(".InvalidatePriorAndCurrentTokens {0}\\{1} rowId: {2} ", org, user, rowId), LogLevel.Debug);
#endif
			using (var queries = DBQueriesProvider.Get())
			{
				var query = "UPDATE SMS_LOG SET invalid=1 WHERE invalid=0 AND ID <= @ROWID AND AD_USERNAME = @USER AND ORG_NAME=@ON ";
				var parms = new List<DBQueryParm>();
				parms.Add(new DBQueryParm(@"USER", user));
				parms.Add(new DBQueryParm(@"ON", org));
				parms.Add(new DBQueryParm(@"ROWID", rowId));
				queries.NonQuery(query, parms);
			}
		}
		
		
		public virtual void PostSelect(string user, string org, long userId, int authProviderId, bool manuallySet)
		{
			
		}
	}
}
