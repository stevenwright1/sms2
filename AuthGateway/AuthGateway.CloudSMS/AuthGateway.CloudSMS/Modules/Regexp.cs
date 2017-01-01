using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages.Request.Command.CloudSms;
using AuthGateway.Shared.XmlMessages.Response.Ret.CloudSms;
using System.Text.RegularExpressions;

namespace SMSService.Modules
{
	public class Regexp : BaseModule
	{
		public override string TypeName { get { return "Regexp"; } }

		public override SendSmsRet SendSMSMessage(SendSms cmd)
		{
			Logger.Instance.WriteToLog(this.TypeName + ".SendSMSMessage start", LogLevel.Debug);

			cmd.Message = replaceCodeAndFullname(cmd.Message, cmd.Code);

			string txtlocalresponse = ReadHtmlPage(cmd.Destination, cmd.Message);
			var parsed = new Dictionary<string, string>();
			var success = ParseHtmlResponse(txtlocalresponse, parsed);
			SendSmsRet ret = new SendSmsRet();
			ret.CreditsRemaining = (parsed.ContainsKey("CreditsRemaining")) ? parsed["CreditsRemaining"] : string.Empty;
			if (parsed.ContainsKey("Error"))
				ret.Error = parsed["Error"];
			if (parsed.ContainsKey("ERROR"))
				ret.Error = parsed["ERROR"];
			if (!success && string.IsNullOrEmpty(ret.Error))
				ret.Error = "Regex did not match in response.";
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

		public bool ParseHtmlResponse(string response, Dictionary<string, string> parsed)
		{
			Logger.Instance.WriteToLog(this.TypeName + ".ParseHtmlResponse start", LogLevel.Debug);

			var regexParameter = this.mc.ModuleParameters.GetByName("Regex");
			if (regexParameter == null)
				throw new Exception(this.TypeName + " expected input 'Regex' parameter");

			var re = new Regex(regexParameter.Value);
			var match = re.Match(response);
			if (!match.Success)
				return false;

			var outparms = mc.ModuleParameters.GetOutputParameters();
			var groupNames = re.GetGroupNames();

			foreach (var o in outparms)
			{
				if (groupNames.Contains(o.Name))
					parsed.Add(o.Name, match.Groups[o.Name].Value);
				else
					parsed.Add(o.Name, o.Value);
			}

			return true;
		}

		public String ReadHtmlPage(String mobilePhone, String Msgs)
		{
			Logger.Instance.WriteToLog(this.TypeName + ".ReadHtmlPage", LogLevel.Debug);
			String result = String.Empty;

			var urlParameter = this.mc.ModuleParameters.GetByName("Url");
			if (urlParameter == null)
				throw new Exception(this.TypeName + " expected input 'Url' parameter");

			try
			{
				var inputs = this.mc.ModuleParameters.GetInputParameters(new List<string>() { "Url", "Regex" });
				var sb = new StringBuilder();
				foreach (var input in inputs)
				{
					var inputValue = input.Value;
					inputValue = inputValue
						.Replace("{destination}", mobilePhone)
						.Replace("{message}", Msgs)
						.Replace("{guid}", Guid.NewGuid().ToString("N"))
						;
					sb.AppendFormat("{0}={1}&", input.Name, Uri.EscapeUriString(inputValue));
				}
				if (sc.BaseSendTokenTestMode)
					sb.AppendFormat("{0}={1}&", "test", "1");
				sb.Length = sb.Length - 1;
				StreamWriter myWriter = null;
				HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(urlParameter.Value);

				if (this.mc.ModuleParameters.GetByName("Proxy") != null)
				{
					var proxyValue = this.mc.ModuleParameters.GetByName("Proxy").Value;
					Logger.Instance.WriteToLog(string.Format(this.TypeName + ".ReadHtmlPage Using proxy '{0}'", proxyValue), LogLevel.Debug);
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
					objRequest.Proxy = proxy;
				}

				objRequest.Method = "POST";
				var postParameters = sb.ToString();
				objRequest.ContentLength = Encoding.UTF8.GetByteCount(postParameters);
				objRequest.ContentType = "application/x-www-form-urlencoded";

				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.Instance.WriteToLog("Request String: " + postParameters, LogLevel.DebugVerbose);
				try
				{
					myWriter = new StreamWriter(objRequest.GetRequestStream());
					myWriter.Write(postParameters);
				}
				catch (Exception e)
				{
					Logger.Instance.WriteToLog("ERROR.ReadHtmlPage.Error sending message: " + e.Message, LogLevel.Error);
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, e);
					return "ERROR=Error sending message.";
				}
				finally
				{
					myWriter.Close();
				}

				HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

				using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
				{
					result = sr.ReadToEnd();
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.Instance.WriteToLog(this.TypeName + " Response: " + result, LogLevel.DebugVerbose);
					// Close and clean up the StreamReader
					sr.Close();
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ERROR.ReadHtmlPage.An error occurred contacting SMS service: " + ex.Message, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				return "ERROR=An error occurred contacting SMS service.";
			}

			return result;
		}
	}
}
