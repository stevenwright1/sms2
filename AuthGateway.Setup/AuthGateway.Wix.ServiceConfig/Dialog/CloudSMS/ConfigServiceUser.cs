
namespace AuthGateway.Wix.ServiceConfig.Dialog.CloudSMS
{

	public partial class ConfigServiceUser : BaseConfigServiceUser
	{
		public ConfigServiceUser(Wizard wizard, Shared.SystemConfiguration systemConfiguration) : base(wizard, systemConfiguration)
		{
			
		}
		protected override string ServiceFriendlyName
		{
			get { return "CloudSMS"; }
		}
		protected override string UsernameConfigName
		{
			get { return "CSMSUSERNAME"; }
		}
		protected override string PasswordeConfigName
		{
			get { return "CSMSPASSWORD"; }
		}
	}
}
