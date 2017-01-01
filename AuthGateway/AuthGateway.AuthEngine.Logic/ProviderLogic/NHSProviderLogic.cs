using System;
using System.Net;
using System.Net.Mail;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.AuthEngine.Logic.ProviderLogic;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using System.Security;
using AuthGateway.AuthEngine.Logic.Helpers;

namespace AuthGateway.AuthEngine.ProviderLogic
{
	public class NHSProviderLogic : BaseSendTokenProviderLogic, IProviderLogic
	{
		const string PASSCODE_LABEL = "{passcode}";

		public override string Name { get { return "NHSProviderLogic"; } }
		public override string DestinyField { get { return "Email"; } }
		protected override string DestinyFieldMissingError { 
			get {
				if (sc == null || string.IsNullOrEmpty(sc.FieldMissingErrorEmail))
					return "Enter your e-mail address:";
				return sc.FieldMissingErrorEmail;
			}
		}

		public override IProviderLogic Using(SystemConfiguration sc)
		{
			this.sc = sc;
			return this;
		}

		public override int InsertTokensAva(Int64 Value)
		{
			return 0;
		}

		public override void SendToken(string user, string org)
		{
			if (!sc.AuthEngineStateless)
				throw new Exception(ERROR_SEND_AHEAD);
			var vur = this.ValidateUser(string.Empty, user, org);
			if (!string.IsNullOrEmpty(vur.Error))
			{
				if (sc.BaseSendTokenTestMode)
					throw new Exception(vur.Error); // Token
				else
				{
					Logger.Instance.WriteToLog(this.Name + ".SendToken ERROR: " + vur.Error, LogLevel.Debug);
					throw new Exception("An error occurred when sending token.");
				}
			}
		}

		protected override void sendTokenToUser(ValidateUserRet ret, string email, string Passcode, string messageTemplate, TemplateMessage tm)
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
					messageTemplate += Passcode;
				}
				else
					messageTemplate = messageTemplate.Replace(PASSCODE_LABEL, Passcode);

				Logger.Instance.WriteToLog(
					string.Format(this.Name + ".sendTokenToUser Email settings: server '{0}' port '{1}' user '{2}'(stripped) password '{3}' ssl '{4}'",
					              sc.EmailConfig.Server,
					              sc.EmailConfig.Port,
					              sc.EmailConfig.Username,
					              password.Substring(0,2),
					              sc.EmailConfig.EnableSSL.ToString()
					             ),
					LogLevel.Debug);
				serverLogic.Registry.Get<IMailSender>().Send(sc.EmailConfig, email, tm.Title, messageTemplate, password);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + ".sendTokenToUser ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + ".sendTokenToUser STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw;
			}
		}
	}
}
