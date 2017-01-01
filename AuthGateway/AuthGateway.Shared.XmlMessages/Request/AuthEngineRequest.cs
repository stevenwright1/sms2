using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;

namespace AuthGateway.Shared.XmlMessages.Request
{
	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class AuthEngineRequest : EncryptableRequest
	{
		[XmlArrayItem(Type = typeof(Tokens))]
		[XmlArrayItem(Type = typeof(TokenLen))]
		[XmlArrayItem(Type = typeof(ValidateUser))]
		[XmlArrayItem(Type = typeof(UpdateTokenLen))]
		[XmlArrayItem(Type = typeof(Details))]
		[XmlArrayItem(Type = typeof(ValidatePin))]
		[XmlArrayItem(Type = typeof(Users))]
		[XmlArrayItem(Type = typeof(Permissions))]
		[XmlArrayItem(Type = typeof(AddFullDetails))]
		[XmlArrayItem(Type = typeof(UpdateFullDetails))]
		[XmlArrayItem(Type = typeof(UserProviders))]
		[XmlArrayItem(Type = typeof(SetUserProvider))]
		[XmlArrayItem(Type = typeof(ResyncHotp))]
		[XmlArrayItem(Type = typeof(PollUsers))]
		[XmlArrayItem(Type = typeof(SendToken))]
		[XmlArrayItem(Type = typeof(ClearPin))]
		[XmlArrayItem(Type = typeof(GetAvailableModules))]
		[XmlArrayItem(Type = typeof(ProvidersList))]
		[XmlArrayItem(Type = typeof(SetInfo))]
		[XmlArrayItem(Type = typeof(AllMsgs))]
		[XmlArrayItem(Type = typeof(UpdateMessage))]
		[XmlArrayItem(Type = typeof(GetSettings))]
        [XmlArrayItem(Type = typeof(GetUserSettings))]
		[XmlArrayItem(Type = typeof(SetSettings))]
		[XmlArrayItem(Type = typeof(SetSetting))]
		[XmlArrayItem(Type = typeof(SetUVault))]
        [XmlArrayItem(Type = typeof(Domains))]
        [XmlArrayItem(Type = typeof(Aliases))]
        [XmlArrayItem(Type = typeof(UpdateDomainAliases))]
        [XmlArrayItem(Type = typeof(GetPanicState))]
        [XmlArrayItem(Type = typeof(ResetPanicState))]
        [XmlArrayItem(Type = typeof(SetUserAuthImages))]
        [XmlArrayItem(Type = typeof(GetUserAuthImages))]
        [XmlArrayItem(Type = typeof(StoreImage))]
        [XmlArrayItem(Type = typeof(GetImage))]
        [XmlArrayItem(Type = typeof(GetImageCategories))]
        [XmlArrayItem(Type = typeof(GetImagesByCategory))]
        [XmlArrayItem(Type = typeof(GetAliveServers))]
        [XmlArrayItem(Type = typeof(SetServerPreferences))]
        [XmlArrayItem(Type = typeof(GetImagesPollingMasterStatus))]
        [XmlArrayItem(Type = typeof(ApplyOATHCalcDefaults))]

		public List<CommandBase> Commands { get; set; }

		[XmlAttribute()]
		public string auth { get; set; }

		public AuthEngineRequest()
		{
			Commands = new List<CommandBase>();
		}
	}
}
