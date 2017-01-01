using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AuthGateway.Shared.XmlMessages.Response.Ret.CloudSms
{
	public class SendSmsRet : RetBase
	{
		public string CreditsRemaining { get; set; }
	}

	public class GetModulesRet : RetBase
	{
		public GetModulesRet()
		{
			this.Modules = new List<string>();
		}

		public List<string> Modules { get; set; }
	}
}
