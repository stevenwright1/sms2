using System;
using System.Net;
using System.Reflection;
using System.Text;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages.Request.Command.CloudSms;
using AuthGateway.Shared.XmlMessages.Response.Ret.CloudSms;
using RestSharp;
using Twilio;

namespace SMSService.Modules
{
	public class Twilio : BaseModule
	{
		private TwilioRestClient twilio;
		private TwilioConfig config = null;

		public override string TypeName { get { return "Twilio"; } }


		public override SendSmsRet SendSMSMessage(SendSms cmd)
		{
			if (this.config == null)
			{
				this.config = new TwilioConfig();
				this.config.Via = this.getModuleParameterValueOrDefault("TwilioService", "Voice");
				this.config.AccountSid = this.getModuleParameterValueOrFail("AccountSid");
				this.config.AuthToken = this.getModuleParameterValueOrFail("AuthToken");
				this.config.From = this.getModuleParameterValueOrFail("From");
				this.config.Voice = this.getModuleParameterValueOrDefault("voice", "alice");
				this.config.Language = this.getModuleParameterValueOrDefault("language", "en-GB");
			}

			this.twilio = new TwilioRestClient(this.config.AccountSid, this.config.AuthToken);

			if (this.mc.ModuleParameters.GetByName("Proxy") != null)
			{
				var proxyValue = this.mc.ModuleParameters.GetByName("Proxy").Value;
				Logger.Instance.WriteToLog(string.Format(this.TypeName + ".SendSMSMessage Using proxy '{0}'", proxyValue), LogLevel.Debug);
				var restClientField = typeof(TwilioRestClient).GetField("_client", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
				var restClient = (RestClient)restClientField.GetValue(this.twilio);

				var proxy = new WebProxy(proxyValue);
				if (this.mc.ModuleParameters.GetByName("ProxyUsername") != null)
				{
					var proxyPassword = string.Empty;
					if (this.mc.ModuleParameters.GetByName("ProxyPassword") != null)
						proxyPassword = this.mc.ModuleParameters.GetByName("ProxyPassword").Value;
					proxy.Credentials = new NetworkCredential(
						this.mc.ModuleParameters.GetByName("ProxyUsername").Value
						, proxyPassword);
				}
				restClient.Proxy = proxy;
			}

			switch (this.config.Via)
			{
				case "SMS":
					return sendTwilioSMS(cmd);
				case "Voice":
					return sendTwilioVoice(cmd);
				default:
					throw new Exception("Parameter not expected");
			}
		}

		private SendSmsRet sendTwilioVoice(SendSms cmd)
		{
			var ret = new SendSmsRet();
			ret.Error = "An unexpected error occurred.";
			try
			{
				var code = addSpacesAndPeriodsToCode(cmd.Code);

				cmd.Message = replaceCodeAndFullname(cmd.Message, code);

				var twimlUrl = "http://twimlets.com/echo?Twiml=";
				var twiml = string.Format(@"<Response>
<Pause length=""3""/>
<Say voice=""{1}"" language=""{2}"">{0}</Say>
<Hangup/>
</Response>", cmd.Message, config.Voice, config.Language);

				var echoUrl = string.Format("{0}{1}", twimlUrl, Uri.EscapeUriString(twiml));

				var call = this.twilio.InitiateOutboundCall(this.config.From, cmd.Destination, echoUrl);
				if (Logger.Instance.ShouldLog(LogLevel.DebugVerbose))
					Logger.Instance.WriteToLog(string.Format(this.TypeName + ".sendTwilioVoice calling voice with data: From '{0}' To '{1}' Message '{2}'",
						this.config.From, cmd.Destination, twiml), LogLevel.DebugVerbose);

				if (!string.IsNullOrEmpty(call.Status) && call.Status != "Failed")
					ret.Error = string.Empty;
				else
				{
					if (call.RestException != null)
					{
						Logger.Instance.WriteToLog(string.Format(
							this.TypeName + ".sendTwilioVoice ERROR from Twilio: {0} Code: {1} Status: {2}  More Information: {3}",
							call.RestException.Message,
							call.RestException.Code,
							call.RestException.Status,
							call.RestException.MoreInfo
							), LogLevel.Error);
					}
					else
					{
						Logger.Instance.WriteToLog(string.Format(
							this.TypeName + ".sendTwilioVoice ERROR: {0}",
							"Unexpected error when calling Twilio InitiateOutboundCall"
							), LogLevel.Error);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(string.Format(this.TypeName + ".sendTwilioVoice ERROR: {0}", ex.Message), LogLevel.Debug);
				Logger.Instance.WriteToLog(string.Format(this.TypeName + ".sendTwilioVoice ERROR: {0}", ex.StackTrace), LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}

		private string addSpacesAndPeriodsToCode(string p)
		{
			if (string.IsNullOrEmpty(p))
				return p;
			var sb = new StringBuilder();
			var charIndex = 1;
			for (var i = 0; i < p.Length; i++)
			{
				sb.Append(p[i]);
				if (charIndex >= 2)
				{
					sb.Append(". ");
					charIndex = 0;
				}
				charIndex++;
			}
			return sb.ToString();
		}

		private SendSmsRet sendTwilioSMS(SendSms cmd)
		{
			var ret = new SendSmsRet();
			ret.Error = "An unexpected error occurred.";
			try
			{
				cmd.Message = replaceCodeAndFullname(cmd.Message, cmd.Code);
				var sms = this.twilio.SendSmsMessage(this.config.From, cmd.Destination, cmd.Message);
				if (Logger.Instance.ShouldLog(LogLevel.DebugVerbose))
					Logger.Instance.WriteToLog(string.Format(this.TypeName + ".sendTwilioSMS sending SMS with data: From '{0}' To '{1}' Message '{2}'",
						this.config.From, cmd.Destination, cmd.Message), LogLevel.DebugVerbose);
				if (!string.IsNullOrEmpty(sms.Status) && sms.Status != "Failed")
					ret.Error = string.Empty;
				else
				{
					Logger.Instance.WriteToLog(string.Format(this.TypeName + ".sendTwilioSMS ERROR from Twilio: {0}", sms.RestException.Message), LogLevel.Error);
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(string.Format(this.TypeName + ".sendTwilioSMS ERROR: {0}", ex.Message), LogLevel.Debug);
				Logger.Instance.WriteToLog(string.Format(this.TypeName + ".sendTwilioSMS ERROR: {0}", ex.StackTrace), LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}
	}

	class TwilioConfig
	{
		public string AccountSid { get; set; }
		public string AuthToken { get; set; }
		public string Via { get; set; }
		public string From { get; set; }
		public string Voice { get; set; }
		public string Language { get; set; }
	}
}
