using System;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.AuthEngine.ProviderLogic;
using AuthGateway.Shared.Log;

namespace AuthGateway.AuthEngine.Logic.ProviderLogic
{
	public class EmergencyProviderLogic : BaseSendTokenProviderLogic
	{
		private string pin = string.Empty;
		
		protected override bool SkipsState { get { return true; } }
		public override int TokenExpireTime() { return sc.EmergencyTokenExpireTimeMinutes; }
		
		public override IProviderLogic Using(Shared.SystemConfiguration sc)
		{
			this.sc = sc;
			return this;
		}

		public override int InsertTokensAva(long Value)
		{
			return 0;
		}

		public override string DestinyField
		{
			get { return "ID"; }
		}

		protected override void sendTokenToUser(Shared.XmlMessages.Response.Ret.AuthEngine.ValidateUserRet ret, string destiny, string Passcode, string messageTemplate, TemplateMessage tm)
		{
			pin = Passcode;
		}

		public override void SendToken(string user, string org)
		{
			var vur = ValidateUser(string.Empty, user, org);
			if (!string.IsNullOrEmpty(vur.Error))
			{
				if (sc.BaseSendTokenTestMode)
					throw new Exception(vur.Error); // Token
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.I.WriteToLog(Name + ".SendToken ERROR: " + vur.Error, LogLevel.Debug);
				throw new Exception("An error occurred when sending token.");
			}
		}

		public string GetPin()
		{
			return pin;
		}

		public override string Name
		{
			get { return "EmergencyProviderLogic"; }
		}
	}
}
