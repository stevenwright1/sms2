
namespace AuthGateway.Wix.ServiceConfig.Dialog.OathCalc
{
	public class ConfigServiceUser : BaseConfigServiceUser
	{
		public ConfigServiceUser(Wizard wizard, Shared.SystemConfiguration systemConfiguration)
			: base(wizard, systemConfiguration)
		{

		}
		protected override string ServiceFriendlyName
		{
			get { return "OathCalc"; }
		}
		protected override string UsernameConfigName
		{
			get { return "OATHUSERNAME"; }
		}
		protected override string PasswordeConfigName
		{
			get { return "OATHPASSWORD"; }
		}
	}
}
