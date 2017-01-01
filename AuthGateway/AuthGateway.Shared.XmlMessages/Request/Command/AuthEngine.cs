using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine
{
	public class UserOrgCommand : CommandBase
	{
		public string User { get; set; }
		public string Org { get; set; }
	}
	public class PollUsers : CommandBase
	{

	}
	public class Tokens : CommandBase
	{

	}

	public class AllMsgs : CommandBase
	{
		
	}

	public class TokenLen : CommandBase
	{

	}

	public class ValidateUser : CommandBase
	{
		public string User { get; set; }
		public string PinCode { get; set; }
		public string State { get; set; }
	}

	public class ResyncHotp : UserOrgCommand
	{
		public string Action { get; set; }
		public string Parameters { get; set; }
		public string Token1 { get; set; }
		public string Token2 { get; set; }
	}

	public class UpdateMessage : CommandBase
	{
		public string Label { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public string Legend { get; set; }
	}

	public class GetSettings : CommandBase
	{
		public GetSettings() {
			Settings = new List<string>();
		}
		
		public string Object { get; set; }
		
		[XmlArrayItem(Type = typeof(string))]
		public List<string> Settings { get; set; }
	}

    public class GetUserSettings : CommandBase
    {
        public GetUserSettings() {
			Settings = new List<string>();
		}
		
		public string Object { get; set; }
		
		[XmlArrayItem(Type = typeof(string))]
		public List<string> Settings { get; set; }
    }
	
	public class SetSetting : CommandBase
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}
	
	public class SetSettings : CommandBase
	{
		public SetSettings() {
			Settings = new List<Setting>();
		}
		[XmlArrayItem(Type = typeof(Setting))]
		public List<Setting> Settings { get; set; }
	}
	
	public class UpdateTokenLen : CommandBase
	{
		public int Length { get; set; }
	}

	public class Details : UserOrgCommand
	{
	}

	public class SetInfo : CommandBase
	{
		public string User { get; set; }
		public string Field { get; set; }
		public string Value { get; set; }
		public string State { get; set; }
	}
	
	public class SetUVault : CommandBase
	{
		public string User { get; set; }
		public string Value { get; set; }
		public string State { get; set; }
	}

	public class ValidatePin : CommandBase
	{
		public string User { get; set; }
		public string Pin { get; set; }
		public string State { get; set; }
		public string PinCode { get; set; }
	}

	public class Users : CommandBase
	{
		public Users() {
			Admins = null;
		}
		
		public int Total { get; set; }
		public string Org { get; set; }

		public int At { get; set; }
		public bool Overrided { get; set; }
		public bool? Admins { get; set; }
		
		public string FName { get; set; }
		public string Text { get; set; }
	}

	public class Permissions : UserOrgCommand
	{
	}

	public class AddFullDetails : UserOrgCommand
	{
		public AddFullDetails()
			: base()
		{
			Providers = new List<UserProvider>();
			PinCode = string.Empty;
		}
		[XmlArrayItem(Type = typeof(UserProvider))]
		public List<UserProvider> Providers { get; set; }

		public string Sid { get; set; }
		public string Phone { get; set; }
		public string Mobile { get; set; }
		public string Fullname { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string UserType { get; set; }
		public string OrgType { get; set; }
		public bool? Enabled { get; set; }
		public bool? AuthEnabled { get; set; }
		public string PinCode { get; set; }

		public string Email { get; set; }

		string _uSNChanged;
		public string uSNChanged { 
			get {
				// In case we do not fill it, make it unique 
				if (_uSNChanged == null)
					_uSNChanged = GetHashCode().ToString();
				return _uSNChanged;
			}
			set {
				_uSNChanged = value;
			}
		}

        public string UPN { get; set; }
	}

	public class UpdateFullDetails : UserOrgCommand
	{
		public string Phone { get; set; }
		public string Mobile { get; set; }
		public string Fullname { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string UserType { get; set; }
		public bool AuthEnabled { get; set; }
		public string PinCode { get; set; }
		public bool PCChange { get; set; }

		public object Email { get; set; }

		public bool Locked { get; set; }
        public string UPN { get; set; }
	}

	public class UserProviders : UserOrgCommand
	{
	}
	
	public class ProvidersList : CommandBase
	{
	}

	public class SetUserProvider : UserOrgCommand
	{
		public UserProvider Provider { get; set; }
	}

	public class SendToken : UserOrgCommand
	{
		public bool Emergency { get; set; }
	}

	public class ClearPin : UserOrgCommand
	{
	}
	
	public class GetAvailableModules : UserOrgCommand
	{

	}

    public class Domains : CommandBase
    {
    }

    public class Aliases : CommandBase
    {
        public string Domain { get; set; }
    }

    public class UpdateDomainAliases : CommandBase
    {
        public string Domain { get; set; }
        public List<string> Aliases { get; set; }        
    }

    public class GetPanicState : UserOrgCommand
    {       
    }

    public class ResetPanicState : UserOrgCommand
    {       
    }

    public class SetUserAuthImages : UserOrgCommand
    {
        public string LeftImage { get; set; }
        public string RightImage { get; set; }
    }

    public class GetUserAuthImages : UserOrgCommand
    {
    }

    public class StoreImage : LogOptionsCommandBase
    {        
        public string Url;
        public string Category;
        public byte[] ImageBytes;        
    }

    public class GetImage : CommandBase
    {
        public string Url;     
    }

    public class GetImageCategories : CommandBase
    {

    }

    public class GetImagesByCategory : CommandBase
    {
        public string Category;
    }

    public class GetAliveServers : CommandBase
    {

    }

    public class SetServerPreferences : CommandBase
    {
        public string Hostname;
        public string MACAddress;
        public PollingPreference UsersPollingPreference;
        public PollingPreference ImagesPollingPreference;
    }

    public class GetImagesPollingMasterStatus : CommandBase
    {

    }

    public class ApplyOATHCalcDefaults : CommandBase
    {
        public string DefaultConfig;
    }
}
