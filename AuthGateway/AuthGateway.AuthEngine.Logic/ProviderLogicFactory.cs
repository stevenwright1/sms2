using System;
using System.Collections.Generic;
using AuthGateway.AuthEngine.ProviderLogic;
using AuthGateway.AuthEngine.Logic.ProviderLogic;

namespace AuthGateway.AuthEngine
{
	internal static class ProviderLogicFactory
	{
		public const string CloudSMSLogic = "CloudSMS";
		public const string OATHCalcLogic = "OATHCalc";
		public const string AuthDisabledLogic = "AuthDisabled";
		public const string PINTANLogic = "PINTAN";
		public const string EmailLogic = "Email";
		public const string NHSLogic = "NHS";
		public const string StaticLogic = "Static";
		public const string OneTimeLogic = "OneTime";
		public const string Passthrough = "Passthrough";
		
		static readonly Dictionary<string, IProviderLogic> providers = new Dictionary<string, IProviderLogic>() {
						{ CloudSMSLogic, new CloudSMSProviderLogic() }
						,{ OATHCalcLogic, new OATHCalcProviderLogic() }
						,{ AuthDisabledLogic, new AuthDisabledProviderLogic() }
						,{ PINTANLogic, new PINTANProviderLogic() }
						,{ EmailLogic, new EmailProviderLogic() }
						,{ NHSLogic, new NHSProviderLogic() }
						,{ StaticLogic, new StaticProviderLogic() }
						,{ OneTimeLogic, new OneTimeProviderLogic() }
						,{ Passthrough, new PassthroughProviderLogic() }
				};
		public static IProviderLogic GetByName(string name)
		{
			switch (name)
			{
				case CloudSMSLogic:
					return new CloudSMSProviderLogic();
				case OATHCalcLogic:
					return new OATHCalcProviderLogic();
				case AuthDisabledLogic:
					return new AuthDisabledProviderLogic();
				case PINTANLogic:
					return new PINTANProviderLogic();
				case EmailLogic:
					return new EmailProviderLogic();
				case StaticLogic:
					return new StaticProviderLogic();
				case OneTimeLogic:
					return new OneTimeProviderLogic();
				case Passthrough:
					return new PassthroughProviderLogic();
			}
			if (!providers.ContainsKey(name))
				throw new Exception(string.Format("ProviderFactory - Provider '{0}' not found", name));
			return providers[name];
		}
	}
}
