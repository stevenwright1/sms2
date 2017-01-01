using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;

namespace AuthGateway.Shared.XmlMessages.Response
{
	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class AuthEngineResponse : EncryptCheckResponse
	{
		[XmlArrayItem(Type = typeof(TokensRet))]
		[XmlArrayItem(Type = typeof(TokenLenRet))]
		[XmlArrayItem(Type = typeof(ValidateUserRet))]
		[XmlArrayItem(Type = typeof(UpdateTokenLenRet))]
		[XmlArrayItem(Type = typeof(DetailsRet))]
		[XmlArrayItem(Type = typeof(ValidatePinRet))]
		[XmlArrayItem(Type = typeof(UsersRet))]
		[XmlArrayItem(Type = typeof(PermissionsRet))]
		[XmlArrayItem(Type = typeof(AddFullDetailsRet))]
		[XmlArrayItem(Type = typeof(UpdateFullDetailsRet))]
		[XmlArrayItem(Type = typeof(UserProvidersRet))]
		[XmlArrayItem(Type = typeof(SetUserProviderRet))]
		[XmlArrayItem(Type = typeof(ResyncHotpRet))]
		[XmlArrayItem(Type = typeof(PollUsersRet))]
		[XmlArrayItem(Type = typeof(SendTokenRet))]
		[XmlArrayItem(Type = typeof(ClearPinRet))]
		[XmlArrayItem(Type = typeof(GetAvailableModulesRet))]
		[XmlArrayItem(Type = typeof(SetInfoRet))]
		[XmlArrayItem(Type = typeof(AllMsgsRet))]
		[XmlArrayItem(Type = typeof(UpdateMessageRet))]
		[XmlArrayItem(Type = typeof(SetSettingRet))]
		[XmlArrayItem(Type = typeof(SetSettingsRet))]
		[XmlArrayItem(Type = typeof(GetSettingsRet))]
		[XmlArrayItem(Type = typeof(SetUVaultRet))]
        [XmlArrayItem(Type = typeof(DomainsRet))]
        [XmlArrayItem(Type = typeof(AliasesRet))]
        [XmlArrayItem(Type = typeof(UpdateAliasesRet))]
        [XmlArrayItem(Type = typeof(GetPanicRet))]
        [XmlArrayItem(Type = typeof(ResetPanicRet))]
        [XmlArrayItem(Type = typeof(SetUserAuthImagesRet))]
        [XmlArrayItem(Type = typeof(GetUserAuthImagesRet))]
        [XmlArrayItem(Type = typeof(StoreImageRet))]
        [XmlArrayItem(Type = typeof(GetImageRet))]
        [XmlArrayItem(Type = typeof(GetImageCategoriesRet))]
        [XmlArrayItem(Type = typeof(GetImagesByCategoryRet))]
        [XmlArrayItem(Type = typeof(GetAliveServersRet))]
        [XmlArrayItem(Type = typeof(SetServerPreferencesRet))]
        [XmlArrayItem(Type = typeof(GetImagesPollingMasterStatusRet))]
        [XmlArrayItem(Type = typeof(ApplyOATHCalcDefaultsRet))]

		public List<RetBase> Responses { get; set; }

		public AuthEngineResponse()
		{
			Responses = new List<RetBase>();
		}
	}
}
