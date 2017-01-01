using System;
using System.Text.RegularExpressions;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.AuthEngine.Logic.ProviderLogic;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using AuthGateway.AuthEngine.Logic.Helpers;

namespace AuthGateway.AuthEngine.ProviderLogic
{
	public class EmailProviderLogic : BaseSendTokenProviderLogic, IProviderLogic
	{
		const string PASSCODE_LABEL = "{passcode}";

		public override string Name { get { return "EmailProviderLogic"; } }
		public override string DestinyField { get { return "Email"; } }
		public override void DestinyFieldValidate(string value) {
			if (string.IsNullOrEmpty(value))
				throw new Exception("Invalid Email Destiny");
			bool isEmail = Regex.IsMatch(value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
			if (!isEmail) {
				throw new Exception("Invalid Email Destiny address");
			}
		}
		protected override string DestinyFieldMissingError { 
			get {
				if (sc == null || string.IsNullOrEmpty(sc.FieldMissingErrorEmail))
					return "Enter your e-mail address:";
				return sc.FieldMissingErrorEmail;
			}
		}

		protected override string TemplateLabel { 
			get { return "Passcode E-mail Message"; }
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

		protected override void sendTokenToUser(ValidateUserRet ret, string destiny, string pin, string messageTemplate, TemplateMessage tm)
		{
			try
			{
				var password = CryptoHelper.DecryptSettingIfNecessary(sc.EmailConfig.Password, "EmailPassword");
				if (sc.EmailConfig.Username == null)
					sc.EmailConfig.Username = string.Empty;
				if (password == null)
					password = string.Empty;

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
							(string.IsNullOrEmpty(password)) ? "" : password.Substring(0,2),
							sc.EmailConfig.EnableSSL
							),
						LogLevel.Debug);
				
				serverLogic.Registry.Get<IMailSender>().Send(sc.EmailConfig, destiny, tm.Title, messageTemplate, password);
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
