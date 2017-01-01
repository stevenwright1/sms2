
namespace AuthGateway.Wix.ServiceConfig.Dialog.MutualAuthImages
{

	public partial class ConfigServiceUser : BaseConfigServiceUser
	{
		public ConfigServiceUser(Wizard wizard, Shared.SystemConfiguration systemConfiguration) : base(wizard, systemConfiguration)
		{
			
		}
		protected override string ServiceFriendlyName
		{
			get { return "MutualAuthImages"; }
		}
		protected override string UsernameConfigName
		{
			get { return "MAIUSERNAME"; }
		}
		protected override string PasswordeConfigName
		{
			get { return "MAIPASSWORD"; }
		}
	}
}
