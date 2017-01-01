using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AuthGateway.OATH.XmlProcessor;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;

namespace AuthGateway.OATH
{
	public class ProcessorException : Exception
	{
		public ProcessorException(string message) : base(message)
		{
		}
	}

	public class Processor
	{

		public static string Process(tokenquery tq)
		{
			if (tq.key == null) {
				throw new ProcessorException("Key not specified.");
			}
			if (tq.hash == null) {
				throw new ProcessorException("Hash not specified.");
			}
			ITokenProcessor tokenProcessor = null;
			tq.hash = tq.hash.ToLower();
			switch (tq.hash)
			{
				case "totp":
					tokenProcessor = new TotpProcessor();
					break;
				case "hotp":
					tokenProcessor = new HotpProcessor();
					break;
				default:
					throw new Exception(string.Format("Unknown hash type '{0}'", tq.hash));
			}
			if (Logger.I.ShouldLog(LogLevel.Debug))
				Logger.I.WriteToLog(string.Format("Processor.Process ID '{0}' HASH '{1}' ", tq.id, tq.hash ), LogLevel.Debug);
			return OutputXML(tokenProcessor.Process(tq));
		}

		public static string Process(System.IO.Stream str)
		{
			str.Position = 0;
			return Process(ParseXML(str));
		}

		public static tokenquery ParseXML(System.IO.Stream str)
		{
			XmlSerializer ser = new XmlSerializer(typeof(tokenquery));
			XmlReader reader = XmlReader.Create(str);
			return ((tokenquery)ser.Deserialize(reader));
		}

		public static string OutputXML(tokenresponse tr)
		{
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			Encoding Utf8WithoutBOM = new UTF8Encoding(false);
			XmlTextWriter xtWriter = new System.Xml.XmlTextWriter(ms, Utf8WithoutBOM);

			XmlSerializer ser = new XmlSerializer(typeof(tokenresponse));
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("", "");
			ser.Serialize(xtWriter, tr, ns);
			xtWriter.Flush();
			ms.Seek(0, System.IO.SeekOrigin.Begin);
			string retStr = Encoding.UTF8.GetString(ms.ToArray());
#if DEBUG
			if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
				Logger.Instance.WriteToLog("OATHCalc OutputXML to client: " + retStr, LogLevel.DebugVerbose);
#endif
			if (Logger.I.ShouldLog(LogLevel.Debug))
				Logger.Instance.WriteToLog(string.Format("Processor.OutputXML finished, sending {0} length XML", retStr.Length), LogLevel.Debug);
			return retStr;
		}
	}

	public interface ITokenProcessor
	{
		tokenresponse Process(tokenquery tq);
	}

	public class HotpProcessor : ITokenProcessor
	{
		public tokenresponse Process(tokenquery tq)
		{
			tokenresponse response = new tokenresponse();
			byte[] key = null;
			if (tq.hexhash == "1") {
				key = HotpTotp.KeyFromHexString(tq.key);
			} else {
				key = Encoding.UTF8.GetBytes(tq.key);
			}
			long counter = 0;
			if (!long.TryParse(tq.counter, out counter)) {
				throw new ProcessorException("Could not parse counter");
			}

			int digits = 0;
			if (!int.TryParse(tq.digits, out digits)) {
				throw new ProcessorException("Could not parse digits");
			}

			long aftervalues = 0;
			if (!Int64.TryParse(tq.aftervalues, out aftervalues)) {
				throw new ProcessorException("Could not parse aftervalues");
			}
			List<value> aftervaluesList = new List<value>();

			response.token = HotpTotp.HOTP(key, counter, digits);

			if ((tq.resynctokens != null)) {
				if (tq.resynctokens.Length > 0) {
					if (tq.resynctokens.Length < 2) {
						throw new ProcessorException("We use at least 2 tokens to resync");
					}
					string token1 = tq.resynctokens[0].Value;
					string token2 = tq.resynctokens[1].Value;
					int tokensMatched = 0;

					if (aftervalues == 0) {
						aftervalues = 10000;
					}
					aftervalues = counter + aftervalues;
					for (long i = counter; i <= aftervalues; i++) {
						string currentToken = HotpTotp.HOTP(key, i, digits);
						if (currentToken == token1) {
							tokensMatched = tokensMatched + 1;
						} else {
							if (tokensMatched > 0 && currentToken == token2) {
								response.token = token1;
								aftervaluesList.Add(new value {
									id = Convert.ToString(i - 1),
									Value = token1
								});
								aftervaluesList.Add(new value {
									id = Convert.ToString(i),
									Value = token2
								});
								response.aftervalues = aftervaluesList.ToArray();
								break; // TODO: might not be correct. Was : Exit For
							} else {
								tokensMatched = 0;
							}
						}
					}
					aftervalues = 0;
				}
			}

			if (aftervalues > 0) {
				for (long i = 1; i <= aftervalues; i++) {
					value val = new value();
					val.id = Convert.ToString(i);
					val.Value = HotpTotp.HOTP(key, counter + i, digits);
					aftervaluesList.Add(val);
				}
				response.aftervalues = aftervaluesList.ToArray();
			}

			return response;
		}
	}

	public class TotpProcessor : ITokenProcessor
	{
		public tokenresponse Process(tokenquery tq)
		{
			tokenresponse response = new tokenresponse();

			try {
				byte[] key = null;
				if (tq.hexhash == "1") {
					key = HotpTotp.KeyFromHexString(tq.key);
				} else {
					key = Encoding.UTF8.GetBytes(tq.key);
				}

				int counter = 0;
				if (string.IsNullOrWhiteSpace(tq.counter)) {
					counter = 0;
				} else {
					if (!int.TryParse(tq.counter, out counter)) {
						throw new ProcessorException("Could not parse counter");
					}
				}
				
				int window = 30;
				if (string.IsNullOrWhiteSpace(tq.window)) {
					window = 30;
				} else {
					if (!int.TryParse(tq.window, out window)) {
						throw new ProcessorException("Could not parse window");
					}
				}

				int digits = 0;
				if (!int.TryParse(tq.digits, out digits)) {
					throw new ProcessorException("Could not parse digits");
				}

				DateTime dt = DateTime.UtcNow;
				int secondsToSkew = window * counter;
				dt = dt.AddSeconds(secondsToSkew);

				response.token = HotpTotp.TOTP(key, dt, digits, window);

				int aftervaluesToCalculate = 0;
				if (!int.TryParse(tq.aftervalues, out aftervaluesToCalculate)) {
					throw new ProcessorException("Could not parse digits");
				}
				int beforevaluesToCalculate = 0;
				if (!int.TryParse(tq.beforevalues, out beforevaluesToCalculate)) {
					throw new ProcessorException("Could not parse digits");
				}

				if (aftervaluesToCalculate > 0) {
					List<value> aftervaluesList = new List<value>();
					DateTime afterDt = default(DateTime);
					for (int i = 1; i <= aftervaluesToCalculate; i++) {
						afterDt = dt.AddSeconds((window * i));
						value val = new value();
						val.id = Convert.ToString(i);
						val.Value = HotpTotp.TOTP(key, afterDt, digits, window);
						aftervaluesList.Add(val);
					}
					response.aftervalues = aftervaluesList.ToArray();
				}

				if (beforevaluesToCalculate > 0) {
					List<value> beforevaluesList = new List<value>();
					DateTime beforeDt = default(DateTime);
					for (int i = 1; i <= beforevaluesToCalculate; i++) {
						beforeDt = dt.AddSeconds((-window * i));
						value val = new value();
						val.id = Convert.ToString(i);
						val.Value = HotpTotp.TOTP(key, beforeDt, digits, window);
						beforevaluesList.Add(val);
					}
					response.beforevalues = beforevaluesList.ToArray();
				}
				if (tq.resynctokens != null && tq.resynctokens.Length == 1) {
					var token = tq.resynctokens[0].Value;
					response.aftervalues = response.aftervalues.Where(x => x.Value == token).ToArray();
					response.beforevalues = response.beforevalues.Where(x => x.Value == token).ToArray();
				}

				return response;
			} catch (Exception ex) {
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw ex;
			}
		}
	}
}
