using System.Collections.Generic;
using System.Xml.Serialization;
using AuthGateway.Shared.XmlMessages.Request.Command.CloudSms;

namespace AuthGateway.Shared.XmlMessages.Request
{
	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class CloudSmsRequest : EncryptableRequest
	{
		[XmlArrayItem(Type = typeof(SendSms))]
		[XmlArrayItem(Type = typeof(GetModules))]
		public List<CommandBase> Commands { get; set; }

		public CloudSmsRequest()
		{
			Commands = new List<CommandBase>();
		}
	}
}
