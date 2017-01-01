using System;

namespace AuthGateway.MassUserSetup
{
	[Serializable()]
	public class ToUser
	{
		public string Username { get; set; }
		public string Fullname { get; set; }
		public string Email { get; set; }
		public SendStatus Status { get; set; }
	}

	public enum SendStatus
	{
		NotSent,
		Sent,
		Error
	}

	public interface IConfigurator
	{

		void Configurate(ToUser toUser);
	}
	public class ConfigCloudSMS : IConfigurator
	{
		public void Configurate(ToUser toUser)
		{
			throw new NotImplementedException();
		}
	}
	public class ConfigOATHCalc : IConfigurator
	{
		public void Configurate(ToUser toUser)
		{
			throw new NotImplementedException();
		}
	}
	public class ConfigEmail : IConfigurator
	{
		public void Configurate(ToUser toUser)
		{
			throw new NotImplementedException();
		}
	}
}
