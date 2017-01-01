using AuthGateway.AuthEngine.Logic.Helpers;
using AuthGateway.AuthEngine.ProviderLogic;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;

namespace AuthGateway.AuthEngine.Logic.ProviderLogic
{
	public class PassthroughProviderLogic : IProviderLogic
	{
		public const string LogicName = "PassthroughProviderLogic";

		public SetInfoRet CheckMissingUserInfo(string user, string org) { return new SetInfoRet(); }
		public bool SetMissingUserInfo(string user, string org, string field, string fieldValue) { return false; }
		public void ClearUserInfo(string user, string org) { }

		public string Name
		{
			get { return LogicName; }
		}

		public string RemoveSecretsFromConfig(string config)
		{
			return config;
		}

		public ValidateUserRet ValidateUser(string state, string user, string org)
		{
            Tracker.Instance.TrackEvent("User Validation Attempt with " + LogicName, Tracker.Instance.DefaultEventCategory);
            Tracker.Instance.TrackCustomEvent("User Validation Success with " + LogicName, Tracker.Instance.DefaultEventCategory, MACAddress.Get());

			return new ValidateUserRet() { 
				PName = this.Name,
				CreditsRemaining = "1",
				Validated = true
			};
		}

		public void ValidatePin(ValidatePinRet ret, string state, string user, string org, string pin)
		{
            Tracker.Instance.TrackEvent("PIN Validation Attempt with" + Name, Tracker.Instance.DefaultEventCategory);
            Tracker.Instance.TrackCustomEvent("PIN Validation Success with" + Name, Tracker.Instance.DefaultEventCategory, MACAddress.Get());
			ret.Validated = true;
		}

		public RetBase Resync(string user, string org, string action, string parameters, string token1, string token2)
		{
			return new ResyncHotpRet() { Out = 1 };
		}

		public IProviderLogic Using(Shared.SystemConfiguration sc)
		{
			return this;
		}

		public IProviderLogic Using(ServerLogic serverLogic)
		{
			return this;
		}

		public IProviderLogic Using(Shared.Provider provider)
		{
			return this;
		}

		public IProviderLogic UsingConfig(string config)
		{
			return this;
		}
		public bool UsesPincode()
		{
			return false;
		}
		public int GetPasscodeLen()
		{
			return 0;
		}

		public void SendToken(string user, string org)
		{
			
		}

		public void PostSelect(string user, string org, long userId, int authProviderId, bool manuallySet)
		{
			
		}
	}
}
