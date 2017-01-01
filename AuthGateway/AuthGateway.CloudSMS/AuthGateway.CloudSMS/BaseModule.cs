using System;
using AuthGateway.Shared;
using AuthGateway.Shared.XmlMessages.Request.Command.CloudSms;
using AuthGateway.Shared.XmlMessages.Response.Ret.CloudSms;

namespace SMSService
{
	public abstract class BaseModule : IModule
	{
		protected const string PASSCODE_LABEL = "{passcode}";
		protected SystemConfiguration sc;
		protected CloudSMSModuleConfig mc;

		public BaseModule()
		{
			
		}
		public void SetConfiguration(SystemConfiguration sc)
		{
			this.sc = sc;
		}
		public void SetModuleConfig(CloudSMSModuleConfig mc)
		{
			this.mc = mc;
		}
		

		public abstract SendSmsRet SendSMSMessage(SendSms cmd);

		public abstract string TypeName
		{
			get;
		}

		protected virtual string replaceCodeAndFullname(string messageTemplate, string code)
		{
			if (messageTemplate == null)
				messageTemplate = string.Empty;
			if (!messageTemplate.Contains(PASSCODE_LABEL))
			{
				if (!string.IsNullOrEmpty(messageTemplate))
					messageTemplate += " ";
				messageTemplate += code;
			}
			else
				messageTemplate = messageTemplate.Replace(PASSCODE_LABEL, code);
			return messageTemplate;
		}

		protected string getModuleParameterValueOrFail(string parameterName)
		{
			var parameter = this.mc.ModuleParameters.GetByName(parameterName);
			if (parameter != null)
				return parameter.Value;
			else
				throw new ArgumentException(string.Format("{0} module expects '{1}' input parameter.", this.TypeName, parameterName));
		}

		protected string getModuleParameterValueOrDefault(string parameterName, string defValue)
		{
			var parameter = this.mc.ModuleParameters.GetByName(parameterName);
			if (parameter != null)
				return parameter.Value;
			else
				return defValue;
		}
	}
}
