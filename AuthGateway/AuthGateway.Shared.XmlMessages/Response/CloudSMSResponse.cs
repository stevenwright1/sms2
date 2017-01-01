using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;
using AuthGateway.Shared.Serializer;
using AuthGateway.Shared.XmlMessages.Response.Ret.CloudSms;

namespace AuthGateway.Shared.XmlMessages.Response
{
	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class CloudSmsResponse : EncryptCheckResponse
	{
		[XmlArrayItem(Type = typeof(SendSmsRet))]
		[XmlArrayItem(Type = typeof(GetModulesRet))]
		public List<RetBase> Responses { get; set; }

		public CloudSmsResponse()
		{
			Responses = new List<RetBase>();
		}
	}
}
