using System;
using AuthGateway.AuthEngine.Logic.Helpers;
using AuthGateway.AuthEngine.ProviderLogic;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;

namespace AuthGateway.AuthEngine.Logic.ProviderLogic
{
	public class StaticProviderLogic : IProviderLogic
	{
		private SystemConfiguration sc;
		private ServerLogic serverLogic;
		private string staticOtp;
		private Provider provider;
		
		public SetInfoRet CheckMissingUserInfo(string user, string org) { return new SetInfoRet(); }
		public bool SetMissingUserInfo(string user, string org, string field, string fieldValue) { return false; }
		public void ClearUserInfo(string user, string org) { }

		public string Name { get { return "StaticProviderLogic"; } }
		public IProviderLogic Using(SystemConfiguration sc)
		{
			this.sc = sc;
			return this;
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
			staticOtp = config;

			return this;
		}

		public string RemoveSecretsFromConfig(string configs)
		{
			return configs;
		}

		public ValidateUserRet ValidateUser(string state, string user, string org)
		{
            Tracker.Instance.TrackEvent("User Validation Attempt with " + Name, Tracker.Instance.DefaultEventCategory);
            Tracker.Instance.TrackCustomEvent("User Validation Success with " + Name, Tracker.Instance.DefaultEventCategory, MACAddress.Get());

			ValidateUserRet ret = new ValidateUserRet();
			ret.CreditsRemaining = "1";
			ret.PName = this.Name;
			return ret;
		}

		public void ValidatePin(ValidatePinRet ret, string state, string user, string org, string pin)
		{
            Tracker.Instance.TrackEvent("PIN Validation Attempt with" + Name, Tracker.Instance.DefaultEventCategory);

			try
			{
				ret.Validated = false;
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog(this.Name + ".ValidatePin Start", LogLevel.Debug);
	
				if (CryptoHelper.HashPincode(pin) == staticOtp)
					ret.Validated = true;
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + ".ValidatePin ERROR Message: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + ".ValidatePin ERROR Stack: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error OATHCalc.ValidatePin";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

            if (ret.Validated) {
                Tracker.Instance.TrackCustomEvent("PIN Validation Success with" + Name, Tracker.Instance.DefaultEventCategory, MACAddress.Get());
            }
		}

		public RetBase Resync(string user, string org, string action, string parameters, string token1, string token2)
		{
			var ret = new ResyncHotpRet();
			ret.Out = 0;

			try
			{

			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + ".Resync ERROR Message: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + ".Resync ERROR Stack: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error OATHCalc.Resync";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}

		public bool UsesPincode()
		{
			return true;
		}
		
		public int GetPasscodeLen()
		{
			return Convert.ToInt32(6);
		}

		public void SendToken(string user, string org)
		{

		}
		
		public void PostSelect(string user, string org, long userId, int authProviderId, bool manuallySet)
		{
			
		}
	}
}
