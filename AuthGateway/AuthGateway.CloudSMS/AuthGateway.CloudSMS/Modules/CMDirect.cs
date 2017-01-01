using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.CloudSms;
using AuthGateway.Shared.XmlMessages.Response.Ret.CloudSms;

namespace SMSService.Modules
{
	public class CMDirect : BaseModule
	{
		private CMDirectConfig config = null;

		public override string TypeName { get { return "CMDirect"; } }

		public override SendSmsRet SendSMSMessage(SendSms cmd)
		{
			Logger.Instance.WriteToLog(this.TypeName + ".SendSMSMessage start", LogLevel.Debug);
			if (config == null)
			{
				this.config = new CMDirectConfig();
				this.config.Url = this.getModuleParameterValueOrDefault("Url", "http://gateway.cmdirect.nl/cmdirect/gateway.ashx");
				this.config.ProductToken = this.getModuleParameterValueOrFail("ProductToken");
				this.config.Sender = this.getModuleParameterValueOrFail("Sender");
				this.config.Tariff = this.getModuleParameterValueOrDefault("Tariff", "0");

				this.config.Proxy = this.getModuleParameterValueOrDefault("Proxy", string.Empty);
				this.config.ProxyUsername = this.getModuleParameterValueOrDefault("ProxyUsername", string.Empty);
				this.config.ProxyPassword = this.getModuleParameterValueOrDefault("ProxyPassword", string.Empty);
			}
			
			cmd.Message = replaceCodeAndFullname(cmd.Message, cmd.Code);

			SendSms sms = (SendSms)cmd;
			SendSmsRet ret = new SendSmsRet();
			ret.CreditsRemaining = "1";
			try
			{
				sendMessage(sms.Destination, sms.Message);
				ret.Error = string.Empty;
			}
			catch (Exception ex)
			{
				if (Logger.I.ShouldLog(LogLevel.Debug))
				{
					Logger.Instance.WriteToLog(this.TypeName + ".SendSMSMessage ERROR: " + ex.Message, LogLevel.Debug);
					Logger.Instance.WriteToLog(this.TypeName + ".SendSMSMessage STACK: " + ex.StackTrace, LogLevel.Debug);
				}
				ret.Error = "An error occurred while sending the SMS.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			if (sc.BaseSendTokenTestMode)
			{
				if (!string.IsNullOrEmpty(ret.Error))
				{
					Logger.Instance.WriteToLog(this.TypeName + ".SendSMSMessage Error cleared in test mode: " + ret.Error, LogLevel.Debug);
					ret.Error = string.Empty;
				}
				ret.CreditsRemaining = "7357";
			}

			return ret;
		}

		public void sendMessage(String mobilePhone, String Msgs)
		{
			if (Logger.I.ShouldLog(LogLevel.Debug))
				Logger.Instance.WriteToLog(this.TypeName + ".sendMessage", LogLevel.Debug);

			try
			{
				var webClient = new WebClient();
				var xml = createXml(this.config.ProductToken, this.config.Sender, mobilePhone, Convert.ToInt32(this.config.Tariff), Msgs);
				if (!string.IsNullOrWhiteSpace(this.config.Proxy)) 
				{
					webClient.Proxy = new WebProxy(this.config.Proxy);
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog(string.Format("{0} - Uusing proxy '{1}'", this.TypeName, this.config.Proxy), LogLevel.DebugVerbose);

					if (!string.IsNullOrWhiteSpace(this.config.ProxyUsername) && !string.IsNullOrWhiteSpace(this.config.ProxyPassword))
					{
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.I.WriteToLog(string.Format("{0} - Uusing proxy username '{1}'", this.TypeName, this.config.ProxyUsername), LogLevel.DebugVerbose);
						webClient.Proxy.Credentials = new NetworkCredential(this.config.ProxyUsername, this.config.ProxyPassword);
					}
				}
				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.I.WriteToLog(string.Format("{0} - Sending '{1}'", this.TypeName, xml), LogLevel.DebugVerbose);
				var result = webClient.UploadString(this.config.Url, xml);
				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.I.WriteToLog(string.Format("{0} - Result '{1}'", this.TypeName, result), LogLevel.DebugVerbose);

				if (!string.IsNullOrWhiteSpace(result))
					throw new Exception("Incorrect response: " + result);
			}
			catch (WebException ex)
			{
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		private static string createXml(string productToken, string sender, string recipient, int tariff, string message)
		{
			return
			new XElement("MESSAGES",
				new XElement("AUTHENTICATION",
					new XElement("PRODUCTTOKEN", productToken)
				),
				new XElement("TARIFF", tariff),
				new XElement("MSG",
					new XElement("FROM", sender),
					new XElement("TO", recipient),
					new XElement("BODY", message)
				)
			).ToString()
			;
		}
	}

	class CMDirectConfig
	{
		public string Url { get; set; }
		public string ProductToken { get; set; }
		public string Sender { get; set; }
		public string Tariff { get; set; }

		public string Proxy { get; set; }
		public string ProxyUsername { get; set; }
		public string ProxyPassword { get; set; }

	}
}
