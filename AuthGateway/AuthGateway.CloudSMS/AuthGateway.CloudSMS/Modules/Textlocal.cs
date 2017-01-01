using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.CloudSms;
using AuthGateway.Shared.XmlMessages.Response.Ret.CloudSms;

namespace SMSService.Modules
{
	public class Textlocal : BaseModule
	{
		private TextlocalConfig config = null;

		public override string TypeName { get { return "Textlocal"; } }

		public override SendSmsRet SendSMSMessage(SendSms cmd)
		{
			Logger.Instance.WriteToLog("ServerLogic.SendSMSMessage start", LogLevel.Debug);
			if (config == null)
			{
				this.config = new TextlocalConfig();
				this.config.Username = this.getModuleParameterValueOrFail("Username");
				this.config.Password = this.getModuleParameterValueOrFail("Password");
				this.config.Proxy = this.getModuleParameterValueOrDefault("Proxy", string.Empty);
				this.config.ProxyUsername = this.getModuleParameterValueOrDefault("ProxyUsername", string.Empty);
				this.config.ProxyPassword = this.getModuleParameterValueOrDefault("ProxyPassword", string.Empty);
			}

            string replacementString = replaceCodeAndFullname(cmd.Message, cmd.Code);
            byte[] ba = Encoding.BigEndianUnicode.GetBytes(replacementString);
            var hexString = BitConverter.ToString(ba);
            cmd.Message = "@U" + hexString.Replace("-", "");            

			SendSms sms = (SendSms)cmd;
			string txtlocalresponse = ReadHtmlPage(sms.Destination, sms.Message);
			Dictionary<string, string> parsed = ParseHtmlResponse(txtlocalresponse);
			SendSmsRet ret = new SendSmsRet();
			ret.CreditsRemaining = (parsed.ContainsKey("CreditsRemaining")) ? parsed["CreditsRemaining"] : string.Empty;
			if (parsed.ContainsKey("Error"))
				ret.Error = parsed["Error"];
			if (parsed.ContainsKey("ERROR"))
				ret.Error = parsed["ERROR"];
			if (sc.BaseSendTokenTestMode)
			{
				if (!string.IsNullOrEmpty(ret.Error))
				{
					Logger.Instance.WriteToLog("ServerLogic.SendSMSMessage Error cleared in test mode: " + ret.Error, LogLevel.Debug);
					ret.Error = string.Empty;
				}
				ret.CreditsRemaining = "7357";
			}

			return ret;
		}

		public Dictionary<string, string> ParseHtmlResponse(string response)
		{
			Logger.Instance.WriteToLog("ServerLogic.ParseHtmlResponse start", LogLevel.Debug);
			Dictionary<string, string> ret = new Dictionary<string, string>();
			string[] splitBr = response.Split(new string[] { "<br>" }, StringSplitOptions.None);
			foreach (string splitted in splitBr)
			{
				string[] splitKeyVal = splitted.Split(new char[] { '=' }, 2);
				if (splitKeyVal.Length == 2)
				{
					ret.Add(splitKeyVal[0], splitKeyVal[1]);
				}
				else
				{
					if (splitted.StartsWith("ERROR"))
					{
						if (!ret.ContainsKey("ERROR"))
						{
							ret.Add("ERROR", string.Empty);
						}
						ret["ERROR"] = splitted;
					}
					else
					{
						if (!ret.ContainsKey("OTHERS"))
						{
							ret.Add("OTHERS", string.Empty);
						}
						ret["OTHERS"] += splitKeyVal[0];
					}
				}
			}
			return ret;
		}

		public String ReadHtmlPage(String mobilePhone, String Msgs)
		{
			Logger.Instance.WriteToLog("ServerLogic.ReadHtmlPage", LogLevel.Debug);
			String result = String.Empty;

			try
			{
				String strPost =
						"info=1&from=SMS_Validation"
						+ "&uname=" + this.config.Username
						+ "&pword=" + this.config.Password
						+ "&selectednums=" + mobilePhone
						+ "&message=" + Uri.EscapeDataString(Msgs)
						;
				if (sc.BaseSendTokenTestMode)
					strPost += "&test=1";
				StreamWriter myWriter = null;
				HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create("http://www.txtlocal.com/sendsmspost.php");

				if (!string.IsNullOrEmpty(this.config.Proxy))
				{
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format(this.TypeName + ".ReadHtmlPage Using proxy '{0}'", this.config.Proxy), LogLevel.Debug);
					var proxy = new WebProxy(this.config.Proxy);
					if (!string.IsNullOrEmpty(this.config.ProxyUsername))
						proxy.Credentials = new NetworkCredential(this.config.ProxyUsername, this.config.ProxyPassword);
					objRequest.Proxy = proxy;
				}

				objRequest.Method = "POST";
				objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
				objRequest.ContentType = "application/x-www-form-urlencoded";

				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.Instance.WriteToLog("Request String: " + strPost, LogLevel.DebugVerbose);
				try
				{
					myWriter = new StreamWriter(objRequest.GetRequestStream());
					myWriter.Write(strPost);
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
						Logger.Instance.WriteToLog("TextLocal Response: " + result, LogLevel.DebugVerbose);
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

	class TextlocalConfig
	{
		public string Username { get; set; }
		public string Password { get; set; }

		public string Proxy { get; set; }
		public string ProxyUsername { get; set; }
		public string ProxyPassword { get; set; }

	}
}
