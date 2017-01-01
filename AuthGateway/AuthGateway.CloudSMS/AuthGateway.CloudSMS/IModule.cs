using System;
using AuthGateway.Shared;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.CloudSms;
using AuthGateway.Shared.XmlMessages.Response.Ret.CloudSms;
namespace SMSService
{
	public interface IModule
	{
		string TypeName { get; }
		void SetConfiguration(SystemConfiguration sc);
		void SetModuleConfig(CloudSMSModuleConfig mc);
		SendSmsRet SendSMSMessage(SendSms cmd);
	}
}
