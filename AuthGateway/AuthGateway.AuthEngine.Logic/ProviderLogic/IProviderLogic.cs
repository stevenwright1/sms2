using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using AuthGateway.Shared;
using AuthGateway.Shared.XmlMessages;

namespace AuthGateway.AuthEngine.ProviderLogic
{
	public interface IProviderLogic
	{
		string Name { get; }
		string RemoveSecretsFromConfig(string config);
		ValidateUserRet ValidateUser(string state, string user, string org);
		void ValidatePin(ValidatePinRet ret, string state, string user, string org, string pin);
		RetBase Resync(string user, string org, string action, string parameters, string token1, string token2);
		IProviderLogic Using(SystemConfiguration sc);
		IProviderLogic Using(ServerLogic serverLogic);
		IProviderLogic Using(Provider provider);
		IProviderLogic UsingConfig(string config);
		bool UsesPincode();
		int GetPasscodeLen();
		void SendToken(string user, string org);

		void PostSelect(string user, string org, long userId, int authProviderId, bool manuallySet);

		SetInfoRet CheckMissingUserInfo(string user, string org);
		bool SetMissingUserInfo(string user, string org, string field, string fieldValue);
		void ClearUserInfo(string user, string org);
	}
}
