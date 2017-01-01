
namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class AuthEngineConfigServiceUser : BaseConfigServiceUser
	{
		public AuthEngineConfigServiceUser(Wizard wizard, Shared.SystemConfiguration systemConfiguration)
			: base(wizard, systemConfiguration)
		{

		}
		protected override string ServiceFriendlyName
		{
			get { return "AuthEngine"; }
		}
		protected override string UsernameConfigName
		{
			get { return "AEUSERNAME"; }
		}
		protected override string PasswordeConfigName
		{
			get { return "AEPASSWORD"; }
		}
	}
}
