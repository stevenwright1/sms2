using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine
{
	public class TokensRet : RetBase
	{
		public Int64 Available { get; set; }
	}

	public class PollUsersRet : RetBase
	{

	}

	public class AllMsgsRet : RetBase
	{
		public AllMsgsRet() {
			Messages = new List<TemplateMessage>();
		}
		[XmlArrayItem(Type = typeof(TemplateMessage))]
		public List<TemplateMessage> Messages { get; set; }
	}

	public class TokenLenRet : RetBase
	{
		public int Length { get; set; }
	}

	public class ValidateUserRet : RetBase
	{
		public string CreditsRemaining { get; set; }
		public string PName { get; set; }
		public string State { get; set; }
		public string Extra { get; set; }
		public bool Validated { get; set; }
		public bool PinCodeEnabled { get; set; }
		public bool AI { get; set; } // Ask Information, message in Extra
		public string AIF { get; set; } // Ask Information Field
		public bool Panic { get; set; }
        public string MutualAuthChallengeMessage { get; set; }
	}

	public class UpdateMessageRet : RetBase
	{
	}

	public class SetSettingRet : RetBase
	{
	}
	
	public class GetSettingsRet : RetBase
	{
		public GetSettingsRet() {
			Settings = new List<Setting>();
		}
		[XmlArrayItem(Type = typeof(Setting))]
		public List<Setting> Settings { get; set; }
	}
	
	public class SetSettingsRet : RetBase
	{
	}

	public class UpdateTokenLenRet : RetBase
	{
	}

	public class DetailsRet : RetBase
	{
		public string Phone { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public string Fullname { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string UserType { get; set; }
		public bool AuthEnabled { get; set; }
		public bool HasPinCodeEntered { get; set; }
		public bool PinCodeEnabled { get; set; }
		public int PinCodeLength { get; set; }
		public bool Locked { get; set; }
		public bool LockdownMode { get; set; }
		public bool MobileOverrided { get; set; }
		public bool PhoneOverrided { get; set; }
		public bool EmailOverrided { get; set; }
        public string UPN { get; set; }
	}

	public class SetInfoRet : RetBase
	{
		public string State { get; set; }
		public string Extra { get; set; }
		public bool AI { get; set; } // Ask Information, message in Extra
		public string AIF { get; set; } // Ask Information Field
	}
	
	public abstract class VaultRetBase : RetBase {
		protected VaultRetBase() {
			ADG = new List<string>();
		}
		public bool ADP { get; set; } // Ask Password
		[XmlArrayItem(Type = typeof(string))]
		public List<string> ADG { get; set; }
		public string ADPASS { get; set; }
	}
	
	public class SetUVaultRet : VaultRetBase
	{
		public string State { get; set; }
	}

	public class ValidatePinRet : VaultRetBase
	{
		public bool Validated { get; set; }
		public bool Panic { get; set; }
	}

	public class UsersRet : RetBase
	{
		public UsersRet()
		{
			this.Users = new List<UserRet>();
		}

		public List<UserRet> Users { get; set; }
	}

	public class UserRet
	{
		[XmlAttribute("u")]
		public string u { get; set; }
		[XmlAttribute("d")]
		public string d { get; set; }
		[XmlAttribute("l")]
		public string l { get; set; }
		[XmlAttribute("f")]
		public string f { get; set; }
	}

	public class PermissionsRet : RetBase
	{
		public bool Status { get; set; }
		public string UserType { get; set; }
		public bool AllowEmergency { get; set; }
		public bool PinCode { get; set; }
	}

	public class AddFullDetailsRet : RetBase
	{
		public int Out { get; set; }
	}

	public class UpdateFullDetailsRet : RetBase
	{
	}

	public class UserProvidersRet : RetBase
	{
		public UserProvidersRet()
			: base()
		{
			Providers = new List<UserProvider>();
		}
		[XmlArrayItem(Type = typeof(UserProvider))]
		public List<UserProvider> Providers { get; set; }
		public int TotpWindow { get; set; }
	}

	public class SetUserProviderRet : RetBase
	{
		public int Out { get; set; }
		public bool Locked { get; set; }
	}

	public class ResyncHotpRet : RetBase
	{
		public int Out { get; set; }
		public string Extra { get; set; }
	}

	public class SendTokenRet : RetBase
	{
		public string Pin { get; set; }
	}
	
	public class ClearPinRet : RetBase
	{
	}

	public class GetAvailableModulesRet : RetBase
	{
		public GetAvailableModulesRet()
		{
			this.Modules = new List<string>();
		}

		public List<string> Modules { get; set; }
	}

    public class DomainsRet : RetBase
    {
        public DomainsRet()
        {
            this.Domains = new List<string>();
        }

        public List<string> Domains { get; set; }
    }

    public class AliasesRet : RetBase
    {
        public AliasesRet()
        {
            this.Aliases = new List<string>();
        }

        public List<string> Aliases { get; set; }
    }

    public class UpdateAliasesRet : RetBase
    {
    }

    public class GetPanicRet : RetBase
    {
        public bool Panic;
    }

    public class ResetPanicRet : RetBase
    {        
    }

    public class SetUserAuthImagesRet : RetBase
    {
    }
    public class GetUserAuthImagesRet : RetBase
    {
        public byte[] LeftImageBytes;
        public byte[] RightImageBytes;
        public long LeftImageId;
        public long RightImageId;
    }

    public class StoreImageRet : RetBase
    {
    }

    public class GetImageRet : LogOptionsRetBase
    {
        public byte[] ImageBytes;
    }

    public class GetImageCategoriesRet : RetBase
    {
        public string[] Categories;
    }

    public class GetImagesByCategoryRet : RetBase
    {
        public string[] Urls;
    }
    
    public class GetAliveServersRet : RetBase
    {
        public class PollingServer
        {
            public string Hostname;
            public string MACAddress;
            public PollingPreference UsersPollingPreference;
            public PollingPreference ImagesPollingPreference;            
        }

        public List<PollingServer> Servers;
    }

    public class SetServerPreferencesRet : RetBase
    {

    }

    public class GetImagesPollingMasterStatusRet : RetBase
    {
        public bool IsImagesPollingMaster;
    }

    public class ApplyOATHCalcDefaultsRet : RetBase
    {

    }
}
