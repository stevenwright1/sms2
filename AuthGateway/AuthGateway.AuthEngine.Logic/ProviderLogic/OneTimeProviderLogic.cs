using System;
using System.Data;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.AuthEngine.Logic.Helpers;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using AuthGateway.AuthEngine.ProviderLogic;

namespace AuthGateway.AuthEngine.Logic.ProviderLogic
{
	public class OneTimeProviderLogic : BaseSendTokenProviderLogic
	{
		const string PASSCODE_LABEL = "{passcode}";
		
		public override SetInfoRet CheckMissingUserInfo(string user, string org) { return new SetInfoRet(); }
		public override bool SetMissingUserInfo(string user, string org, string field, string fieldValue) { return false; }
		

		public override string Name { get { return "OneTimeProviderLogic"; } }
		public override string DestinyField { get { return "Email"; } }
		protected override bool SkipsState { get { return true; } }
		public override int TokenExpireTime() { return sc.OneTimeTokenExpireTimeMinutes; }
		public override int InsertTokensAva(Int64 Value) { return 0; }
		
		public override IProviderLogic Using(SystemConfiguration sc)
		{
			this.sc = sc;
			return this;
		}
		
		public override ValidateUserRet ValidateUser(string state, string user, string org)
		{
			var ret = new ValidateUserRet {
				CreditsRemaining = "1",
				PName = Name
			};
			return ret;
		}

		public override void ValidatePin(ValidatePinRet ret, string state, string user, string org, string pin)
		{
			var hash = CryptoHelper.GetSHA1String(pin);
			base.ValidatePin(ret, state, user, org, hash);
		}
		
		public override int GetPasscodeLen()
		{
			return Convert.ToInt32(6);
		}

		public override void SendToken(string user, string org)
		{
			
		}
		
		public override void PostSelect(string user, string org, long userId, int authProviderId, bool manuallySet)
		{
			if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
				Logger.I.WriteToLog(string.Format("{0}.PostSelect start.", this.Name), LogLevel.DebugVerbose);
			const string USERNAME_LABEL = "{username}";
			const string FULLNAME_LABEL = "{fullname}";
			const string FIRSTNAME_LABEL = "{firstname}";
			const string LASTNAME_LABEL = "{lastname}";

			var pin = RandomKeyGenerator.Generate(6, sc.AuthEngineKeyBase);
			var pinHash = CryptoHelper.GetSHA1String(pin);

			var email = string.Empty;
			var displayName = string.Empty;
			var firstName = string.Empty;
			var lastName = string.Empty;

			var tm = new TemplateMessage {
				Message = "Your secure passcode is: {passcode}"
			};
			using (var queries = DBQueriesProvider.Get())
			{
				var dt = queries.Query(
					@"SELECT Email,DISPLAY_NAME,[FIRSTNAME],[LASTNAME] from SMS_CONTACT WHERE ID=@userId"
					, new DBQueryParm("userId", userId));
				
				DataRow row = dt.Rows[0];
				email = ( row["Email"] != null && row["Email"] != DBNull.Value )
					? row["Email"].ToString()
					: "";
				displayName = ( row["DISPLAY_NAME"] != null && row["DISPLAY_NAME"] != DBNull.Value )
					? row["DISPLAY_NAME"].ToString()
					: "";
				firstName = row.Field<string>("FIRSTNAME");
				lastName = row.Field<string>("LASTNAME");
				tm = queries.GetTemplateMessage("OneTime Setup E-mail");
			}
			
			var messageTemplate = tm.Message;
			messageTemplate = messageTemplate.Replace(USERNAME_LABEL, user);
			messageTemplate = messageTemplate.Replace(FULLNAME_LABEL, displayName);
			messageTemplate = messageTemplate.Replace(FIRSTNAME_LABEL, firstName);
			messageTemplate = messageTemplate.Replace(LASTNAME_LABEL, lastName);
						
			InsertTokenInDB(string.Empty, user, org, pinHash, TokenExpireTime());
			sendTokenToUser(null, email, pin, messageTemplate, tm );
			if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
				Logger.I.WriteToLog(string.Format("{0}.PostSelect end.", Name), LogLevel.DebugVerbose);
		}
		
		protected override void sendTokenToUser(ValidateUserRet ret, string destiny, string pin, string messageTemplate, TemplateMessage tm)
		{
			try
			{
				var password = CryptoHelper.DecryptSettingIfNecessary(sc.EmailConfig.Password, "EmailPassword");
				if (string.IsNullOrEmpty(sc.EmailConfig.Username))
					sc.EmailConfig.Username = string.Empty;
				if (string.IsNullOrEmpty(password))
					password = "   ";

				if (!messageTemplate.Contains(PASSCODE_LABEL))
				{
					if (!string.IsNullOrEmpty(messageTemplate))
						messageTemplate += " ";
					messageTemplate += pin;
				}
				else
					messageTemplate = messageTemplate.Replace(PASSCODE_LABEL, pin);
				
				if (sc.BaseSendTokenTestMode)
					serverLogic.Registry.AddOrSet<string>(pin);

				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog(
						string.Format("{0}.sendTokenToUser Email settings: server '{1}' port '{2}' user '{3}'(stripped) password '{4}' ssl '{5}'",
							Name,						              
							sc.EmailConfig.Server, 
							sc.EmailConfig.Port,
							sc.EmailConfig.Username,
							password.Substring(0,2),
							sc.EmailConfig.EnableSSL
							),
						LogLevel.Debug);
				serverLogic.Registry.Get<IMailSender>().Send(sc.EmailConfig, destiny, tm.Title, messageTemplate, password);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(Name + ".sendTokenToUser ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(Name + ".sendTokenToUser STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw;
			}
		}
	}
}
