using System.Collections.Generic;
using System.Xml.Serialization;

namespace AuthGateway.Shared.XmlMessages.Request.Command.CloudSms
{
	public class SendSms : CommandBase
	{
		public string ModuleName { get; set; }
		public string Destination { get; set; }
		public string Message { get; set; }
		public string Code { get; set; }
	}

	public class GetModules : CommandBase
	{

	}
}
