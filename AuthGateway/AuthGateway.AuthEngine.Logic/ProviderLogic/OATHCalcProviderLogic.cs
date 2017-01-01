using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AuthGateway.OATH.XmlProcessor;
using AuthGateway.Shared;
using AuthGateway.Shared.Helper;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Serializer;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

using AuthGateway.AdminGUI.OATH;
using AuthGateway.AuthEngine.Logic;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.AuthEngine.Logic.Helpers;

namespace AuthGateway.AuthEngine.ProviderLogic
{
	public class OATHCalcProviderLogic : IProviderLogic
	{
		private SystemConfiguration sc;
		private ServerLogic serverLogic;
		private Dictionary<string, string> configs;
		private Provider provider;

		public SetInfoRet CheckMissingUserInfo(string user, string org) { 
			var ret = new SetInfoRet();
			// Skip if we have 0 or more than 1 device configs
			if (configs.Count != 1)
				return ret;
			var firstConfig = configs.First();
			var firstModel = HotpTotpModel.Unserialize(firstConfig.Value);
			
			ret.AI = ( string.IsNullOrEmpty(firstModel.Secret) || firstModel.Secret == "__serial:");
			if (ret.AI)
			{
				ret.AIF = "sharedsecret";
				ret.Extra = "Please enter your token serial number or leavy empty.";
			}
			return ret;	
		}
		public bool SetMissingUserInfo(string user, string org, string field, string fieldValue) {
			if ( field != "sharedsecret" ) {
				return false;
			}
			var checkRet = CheckMissingUserInfo(user, org);
			if (!checkRet.AI)
				throw new ValidationException("User does not have missing info.");
			
			var firstConfig = configs.First();
			var firstModel = HotpTotpModel.Unserialize(firstConfig.Value);
			if ( string.IsNullOrEmpty(fieldValue) ) {
				var rand = new byte[10];
				CryptoRandom.Instance().NextBytes(rand);
				firstModel.Secret = HexConversion.ToString(rand);
			} else {
				try {
					ReplaceHardToken(firstModel, fieldValue);
				} catch( InvalidSerialException ex) {
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
					return false;
				}
			}
			configs[firstConfig.Key] = HotpTotpModel.Serialize(firstModel);
			
			SaveConfigs(user, org);
			
			return true; 
		}
		public void ClearUserInfo(string user, string org) { }

		public string Name { get { return "OATHCalcProviderLogic"; } }
		public IProviderLogic Using(SystemConfiguration sc)
		{
			this.sc = sc;
			if (sc.OATHCalcUseEncryption)
				oathRsaProvider.FromXmlString(sc.OATHCalcPublicKey);

			return this;
		}
		public IProviderLogic Using(ServerLogic serverLogic)
		{
			this.serverLogic = serverLogic;
			return this;
		}
		public IProviderLogic Using(Provider provider)
		{
			this.provider = provider;
			return this;
		}
		public IProviderLogic UsingConfig(string config)
		{
			configs = new Dictionary<string, string>();

			var configsSplit = config.Split(new string[] { "|%|" }, StringSplitOptions.None);
			for (var i = 0; i < configsSplit.Length; i++)
			{
				if (string.IsNullOrWhiteSpace(configsSplit[i]))
					continue;
				
				var model = HotpTotpModel.Unserialize(configsSplit[i]);
				configs.Add(model.DeviceName, HotpTotpModel.Serialize(model));
			}

			return this;
		}

		private RSACryptoServiceProvider oathRsaProvider = new RSACryptoServiceProvider();

		public string RemoveSecretsFromConfig(string configs)
		{
			if (string.IsNullOrEmpty(configs))
				return configs;

			var configsSplit = configs.Split(new string[] { "|%|" }, StringSplitOptions.None);
			for(var i = 0; i < configsSplit.Length; i++)
			{
				var model = HotpTotpModel.Unserialize(configsSplit[i]);
				configsSplit[i] = HotpTotpModel.Serialize(model);
			}

			return string.Join("|%|", configsSplit);
		}

		public ValidateUserRet ValidateUser(string state, string user, string org)
		{
            Tracker.Instance.TrackEvent("User Validation Attempt with " + Name, Tracker.Instance.DefaultEventCategory);
			ValidateUserRet ret = new ValidateUserRet();
			ret.CreditsRemaining = "1";
			ret.PName = this.Name;
            Tracker.Instance.TrackCustomEvent("User Validation Success with " + Name, Tracker.Instance.DefaultEventCategory, MACAddress.Get());
			return ret;
		}

		private tokenresponse Send(tokenquery tq)
		{
			String returndata = string.Empty;
			using (TcpClient tcpClient = new TcpClient())
			{
				try
				{

					tcpClient.Connect(sc.OATHCalcServerIp, sc.OATHCalcServerPort);

					NetworkStream ns = tcpClient.GetStream();

					if (ns.CanWrite && ns.CanRead)
					{
						Byte[] key;
						Byte[] iv;
						bool encrypted = false;
						tokenquery exchangeKeyRequest = new tokenquery();

						RijndaelManaged aes = new RijndaelManaged();

						byte[] sendBytes;
						MemoryStream retData = new MemoryStream();

						if (sc.OATHCalcUseEncryption)
						{
							Logger.Instance.WriteToLog("OATHCalc using encryption.", LogLevel.Debug);
							int keySize = 32;
							int ivSize = 16;
							key = new Byte[keySize];
							iv = new Byte[ivSize];
							RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
							random.GetBytes(key);
							random.GetBytes(iv);
							exchangeKeyRequest.Nullify();
							exchangeKeyRequest.CKey = Convert.ToBase64String(key);
							exchangeKeyRequest.CIV = Convert.ToBase64String(iv);

							string handShake = Generic.Serialize<tokenquery>(exchangeKeyRequest);
							Logger.Instance.WriteToLog("OATHCalc.ClientHandshake: " + handShake, LogLevel.Debug);
							sendBytes = oathRsaProvider.Encrypt(Encoding.UTF8.GetBytes(handShake), false);
							ns.Write(sendBytes, 0, sendBytes.Length);

							sendBytes = new byte[tcpClient.ReceiveBufferSize];
							do
							{
								int bread = ns.Read(sendBytes, 0, tcpClient.ReceiveBufferSize);
								retData.Write(sendBytes, 0, bread);
							} while (ns.DataAvailable);

							aes = new RijndaelManaged();
							aes.Padding = PaddingMode.PKCS7;
							aes.Mode = CipherMode.CBC;
							aes.KeySize = 256;
							aes.Key = key;
							aes.IV = iv;
							ICryptoTransform cryptor = aes.CreateDecryptor(aes.Key, aes.IV);
							retData.Position = 0;

							using (CryptoStream cs = new CryptoStream(retData, cryptor, CryptoStreamMode.Read))
							{
								using (StreamReader sr = new StreamReader(cs))
									returndata = sr.ReadToEnd();
							}
							tokenresponse exchangeKeyResponse = Generic.Deserialize<tokenresponse>(returndata);
							if (String.IsNullOrEmpty(exchangeKeyResponse.CHK))
								throw new Exception("Crypt check failed");
							retData = new MemoryStream();

							encrypted = true;
						}

						returndata = Generic.Serialize<tokenquery>(tq);

						Logger.Instance.WriteToLog("OATHCalc Query: " + returndata, LogLevel.Debug);
						if (encrypted)
						{
							ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
							using (MemoryStream msEncrypt = new MemoryStream())
							{
								using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
								using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
									swEncrypt.Write(returndata);
								sendBytes = msEncrypt.ToArray();
							}
						}
						else
							sendBytes = Encoding.UTF8.GetBytes(returndata);
						ns.Write(sendBytes, 0, sendBytes.Length);

						Byte[] output = new Byte[tcpClient.ReceiveBufferSize];
						do
						{
							int bread = ns.Read(output, 0, output.Length);
							retData.Write(output, 0, bread);
						} while (ns.DataAvailable);

						retData.Position = 0;
						if (encrypted)
						{
							try {
								ICryptoTransform cryptor = aes.CreateDecryptor(aes.Key, aes.IV);
								using (CryptoStream cs = new CryptoStream(retData, cryptor, CryptoStreamMode.Read))
								{
									using (StreamReader sr = new StreamReader(cs))
									{
										returndata = sr.ReadToEnd();
									}
								}
							}
							catch
#if DEBUG
 (Exception ex)
#endif
							{
#if DEBUG
								retData.Position = 0;
								var encryptedData = new StreamReader(retData).ReadToEnd();
								Logger.Instance.WriteToLog(this.Name + ".Send FAILED TO DECRYPT ERROR: " + ex.Message, LogLevel.Error);
								Logger.Instance.WriteToLog(this.Name + ".Send FAILED TO DECRYPT STACK: " + ex.StackTrace, LogLevel.Error);
								Logger.Instance.WriteToLog(this.Name + ".Send FAILED TO DECRYPT: " + encryptedData, LogLevel.Debug);
#endif
								throw;
							}
						}
						else
						{
							using (StreamReader sr = new StreamReader(retData))
								returndata = sr.ReadToEnd();
						}
						returndata = returndata.TrimEnd(new char[] { (char)0 }).Trim();
						if (Logger.I.ShouldLog(LogLevel.Debug))
							Logger.Instance.WriteToLog(string.Format("OATHCalc Responsed with a string of {0} chars.", returndata.Length), LogLevel.Debug);
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.Instance.WriteToLog(string.Format("OATHCalc Response: {0}", returndata), LogLevel.DebugVerbose);
						return Generic.Deserialize<tokenresponse>(returndata);
					}

					return new tokenresponse() { };
				}
				catch (Exception ex)
				{
					Logger.Instance.WriteToLog("OATHCalcProviderLogic.Send ERROR Message: " + ex.Message, LogLevel.Error);
					Logger.Instance.WriteToLog("OATHCalcProviderLogic.Send ERROR Stack: " + ex.StackTrace, LogLevel.Debug);
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
					throw;
				}
			}
		}

		public void ValidatePin(ValidatePinRet ret, string state, string user, string org, string pin)
		{
            Tracker.Instance.TrackEvent("PIN Validation Attempt with" + Name, Tracker.Instance.DefaultEventCategory);

			ret.Validated = false;
			Logger.Instance.WriteToLog("OATHCalcProviderLogic.ValidatePin Start", LogLevel.Debug);

			foreach(var config in configs.Values)
			{
				try
				{
					String returndata = string.Empty;

					tokenquery req = new tokenquery();
					var model = HotpTotpModel.Unserialize(config);
					
					var deviceName = model.DeviceName;
					req.hash = model.Type.ToLower();
					if (req.hash == "totp")
					{
						req.window = model.Window;
						if (sc.OATHCalcTotpWindow > 0)
						{
							req.aftervalues = sc.OATHCalcTotpWindow.ToString();
							req.beforevalues = sc.OATHCalcTotpWindow.ToString();
						}
					}
					else
					{
						if (sc.OATHCalcHotpAfterWindow > 0)
						{
							req.aftervalues = sc.OATHCalcHotpAfterWindow.ToString();
						}
					}
					req.key = model.Secret;
					req.counter = model.CounterSkew;
					req.hexhash = "1";

					tokenresponse tokenResponse = Send(req);

					if (req.hash == "totp")
					{
						ValidateTotpPin(ret, user, org, pin, req, tokenResponse, deviceName);
					}
					else
					{
						ValidateHotpPin(ret, user, org, pin, tokenResponse, deviceName);
					}
				}
				catch (Exception ex)
				{
					Logger.Instance.WriteToLog("OATHCalcProviderLogic.ValidatePin ERROR Message: " + ex.Message, LogLevel.Error);
					Logger.Instance.WriteToLog("OATHCalcProviderLogic.ValidatePin ERROR Stack: " + ex.StackTrace, LogLevel.Debug);
					ret.Error = "Error OATHCalc.ValidatePin";
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				}
				if (ret.Validated)
					return;
			}

            if (ret.Validated) {
                Tracker.Instance.TrackCustomEvent("PIN Validation Success with" + Name, Tracker.Instance.DefaultEventCategory, MACAddress.Get());
            }
		}

		private void ValidateHotpPin(ValidatePinRet ret, string user, string org, string pin, tokenresponse tokenResponse, string deviceName)
		{
			if (tokenResponse.token == pin)
			{
				if (IncrementCounter(user, org, deviceName) > 0)
					ret.Validated = true;
				else
					ret.Error = "Failed to update counter.";
			}
			else
			{
				if (sc.OATHCalcHotpAfterWindow > 0)
				{
					if (tokenResponse.aftervalues != null)
					{
						foreach (value val in tokenResponse.aftervalues)
						{
							if (val.Value == pin)
							{
								if (IncrementCounterPlus(user, org, val.id, deviceName) > 0)
									ret.Validated = true;
								else
									ret.Error = "Failed to update counter.";

								break;
							}
						}
					}
				}
			}
		}

		private void ValidateTotpPin(ValidatePinRet ret, string user, string org, string pin, tokenquery req, tokenresponse tokenResponse, string deviceName)
		{
			long skew;
			if (!long.TryParse(req.counter, out skew))
				skew = 0;
			ret.Validated = false;

			if (tokenResponse.token == pin)
			{
				ret.Validated = true;
			}
			else
			{
				if (sc.OATHCalcTotpWindow > 0)
				{
					if (tokenResponse.aftervalues != null && !ret.Validated)
					{
						var tempSkew = skew;
						foreach (value val in tokenResponse.aftervalues)
						{
							tempSkew++;
							if (val.Value == pin)
							{
								ret.Validated = true;
								skew = tempSkew;
								break;
							}
						}
					}
					if (tokenResponse.beforevalues != null && !ret.Validated)
					{
						var tempSkew = skew;
						foreach (value val in tokenResponse.beforevalues)
						{
							tempSkew--;
							if (val.Value == pin)
							{
								ret.Validated = true;
								skew = tempSkew;
								break;
							}
						}
					}
				}
			}
			if (ret.Validated && IncrementCounter(user, org, (skew - 1), deviceName) <= 0)
				ret.Error = "Failed to update counter.";
		}

		private int IncrementCounter(string user, string org, string deviceName)
		{
			var config = configs[deviceName];
			var model = HotpTotpModel.Unserialize(config);
			return IncrementCounter(user, org, model.CounterSkew, deviceName);
		}

		private int IncrementCounterPlus(string user, string org, string plusCounter, string deviceName)
		{
			var config = configs[deviceName];
			var model = HotpTotpModel.Unserialize(config);
			Int64 c = Convert.ToInt64(model.CounterSkew);
			Int64 cplus = Convert.ToInt64(plusCounter);
			
			return IncrementCounter(user, org, c + cplus, deviceName);
		}

		private int IncrementCounter(string user, string org, string counter, string deviceName)
		{
			if (!string.IsNullOrEmpty(counter.Trim()))
			{
				Int64 c = Convert.ToInt64(counter);
				return IncrementCounter(user, org, c, deviceName);
			}
			return 0;
		}

		private int IncrementCounter(string user, string org, Int64 counter, string deviceName)
		{
				counter++;
				return SaveCounter(user, org, counter, deviceName);
		}

		private int SaveCounter(string user, string org, Int64 counter, string deviceName)
		{
			if (provider == null)
				throw new Exception("Provider is null");

			var config = configs[deviceName];

			var model = HotpTotpModel.Unserialize(config);
			
			model.CounterSkew = Convert.ToString(counter);
			
			configs[deviceName] = HotpTotpModel.Serialize(model);


			if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
				Logger.I.WriteToLog(string.Format(@"Updating incremented counter config for '{0}\{1}'", org, user), LogLevel.DebugVerbose);

			return SaveConfigs(user, org);
		}
		
		public string GetJoinedConfigs() {
			var configsToJoin = new List<string>();
			configsToJoin.AddRange(configs.Values);
			return string.Join("|%|", configsToJoin);
		}
		
		int SaveConfigs(string user, string org) {
			var joinedConfigs = GetJoinedConfigs();
			using (var queries = DBQueriesProvider.Get())
			{
				var userId = queries.GetUserId(user, org);
				const string query = @"UPDATE UserProviders SET config=@PCONF
					WHERE userId = @UserId AND authProviderId=@PID ";

				var parms = new List<DBQueryParm>();
				parms.Add(new DBQueryParm(@"UserId", userId));
				parms.Add(new DBQueryParm(@"PID", provider.Id));
				parms.Add(new DBQueryParm(@"PCONF", joinedConfigs));
				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.I.WriteToLog(string.Format(@"Saved new config for '{0}\{1}': {2}", org, user, joinedConfigs), LogLevel.DebugVerbose);
				return queries.NonQuery(query,parms);
			}
		}

		public RetBase Resync(string user, string org, string action, string parameters, string token1, string token2)
		{
			ResyncHotpRet ret = new ResyncHotpRet();
			ret.Out = 0;

			var deviceName = parameters;
			try
			{
				if (provider == null)
					throw new Exception("Provider is null");

				if (string.IsNullOrWhiteSpace(deviceName))
					deviceName = "Default";

				if (!configs.ContainsKey(deviceName))
					throw new OATHException("Invalid device name or not found.");

				tokenquery req = new tokenquery();

				var model = HotpTotpModel.Unserialize(configs[deviceName]);
				
				req.hash = model.Type.ToLower();
				if (req.hash != "hotp" && req.hash != "totp")
					throw new Exception("Resync only supported for HOTP/TOTP");
				
				req.key = model.Secret;
				req.counter = model.CounterSkew;
				req.window = model.Window;
				req.hexhash = "1";
				req.aftervalues = "10000";
				if (req.hash == "totp") {
					req.beforevalues = "500";
					req.resynctokens = new [] { new value { Value = token1 } };
				} else
					req.resynctokens = new value[] { new value() { Value = token1 }, new value() { Value = token2 } };
				tokenresponse tr = Send(req);

				switch (req.hash) {
					case "hotp":
						if (tr.aftervalues == null || tr.aftervalues.Length != 2)
							ret.Error = "Could not find matching tokens";
						else
							ret.Out = IncrementCounter(user, org, tr.aftervalues[1].id, deviceName);
						break;
					case "totp":
						if (tr.token == token1)
							ret.Out = IncrementCounter(user, org, (0 - 1), deviceName);
						else
						{
							var match = false;
							if (!match)
							{
								foreach (var afterValue in tr.aftervalues)
								{
									if (afterValue.Value == token1)
									{
										ret.Out = IncrementCounter(user, org, (Convert.ToInt64(afterValue.id) - 1), deviceName);
										match = true;
										break;
									}
								}
							}
							if (!match)
							{
								foreach (var beforeValue in tr.beforevalues)
								{
									if (beforeValue.Value == token1)
									{
										ret.Out = IncrementCounter(user, org, ((Convert.ToInt64(beforeValue.id) * -1) - 1), deviceName);
										match = true;
										break;
									}
								}
							}
							if (!match) {
								ret.Error = "Could not find matching tokens";
							}
						}
						break;
					default:
						ret.Error = string.Format("Incorrect hash type'{0}'", req.hash);
						break;
				}
			}
			catch (OATHException ex)
			{
				ret.Error = ex.Message;
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("OATHCalcProviderLogic.Resync ERROR Message: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("OATHCalcProviderLogic.Resync ERROR Stack: " + ex.StackTrace, LogLevel.Debug);
				ret.Error = "Error OATHCalc.Resync";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}

		public bool UsesPincode()
		{
			return true;
		}
		public int GetPasscodeLen()
		{
			return Convert.ToInt32(new tokenquery().digits);
		}

		public void SendToken(string user, string org)
		{

		}

		public void ReplaceHardTokens()
		{			
			var serials = new List<string>();
			const string feitian_pattern = @",feitian:([0-9]+),";
			const string serial_pattern = @",__serial:([0-9]+),";
			
			var newConfigs = new Dictionary<string, string>();
			foreach(var config in configs) {
				foreach(Match m in Regex.Matches(config.Value, feitian_pattern)) {
					serials.Add(m.Groups[1].Value);
				}
				foreach(Match m in Regex.Matches(config.Value, serial_pattern)) {
					serials.Add(m.Groups[1].Value);
				}
				if (serials.Count == 0)
					return;
				
				var configModel = HotpTotpModel.Unserialize(config.Value);
				
				foreach(var serial in serials) {
					OATHCalcProviderLogic.ReplaceHardToken(configModel, serial);
				}
				
				newConfigs[config.Key] = HotpTotpModel.Serialize(configModel);
			}
			foreach(var newConfig in newConfigs) {
				configs[newConfig.Key] = newConfig.Value;
			}
		}
		
		public static void ReplaceHardToken(HotpTotpModel model, string serial) {
			using (var queries = DBQueriesProvider.Get())
			{
				var parms = new List<DBQueryParm>();
				parms.Add(new DBQueryParm(@"serial", serial));
				var tokenDt = queries.Query(@"SELECT * FROM [HardToken] WHERE [serial] = @serial", parms);
				
				if (tokenDt.Rows.Count == 0)
					throw new InvalidSerialException();
				var tokenRow = tokenDt.Rows[0];
				
				model.Secret = tokenRow.Field<string>("key");
				if (tokenDt.Columns.Contains("tokentype")) {
					model.Type = ( tokenRow.Field<string>("tokentype").ToUpperInvariant() == HotpTotpModel.HOTP )
						? HotpTotpModel.HOTP 
						: HotpTotpModel.TOTP;
			    } else
					model.Type = HotpTotpModel.TOTP;
				
				if (model.Type == HotpTotpModel.TOTP) {
					if (tokenDt.Columns.Contains("window")) {
						var window = tokenRow.Field<string>("window");
						if (!string.IsNullOrWhiteSpace(window) && Regex.IsMatch(window, @"^[0-9]+$"))
							model.Window = window;
						else
							model.Window = "60";
					}
				}
			}
		}
		
		public void PostSelect(string user, string org, long userId, int authProviderId, bool manuallySet)
		{
			const string USERNAME_LABEL = "{username}";
			const string FULLNAME_LABEL = "{fullname}";
			const string FIRSTNAME_LABEL = "{firstname}";
			const string LASTNAME_LABEL = "{lastname}";
			const string SHAREDSECRET_LABEL = "{sharedsecret}";
			const string URL_LABEL = "{url}";
			
			if (manuallySet) {
				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog(Name + ".PostSelect skipped: manually set", LogLevel.DebugVerbose);
				return;
			}

			if (!provider.AutoSetup) {
				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.I.WriteToLog(Name + ".PostSelect skipped: !autoSetup", LogLevel.DebugVerbose);
				return;
			}
			
			if (Logger.I.ShouldLog(LogLevel.Debug))
				Logger.Instance.WriteToLog(Name + ".PostSelect", LogLevel.Debug);

			var tm = new TemplateMessage {
				Title = "WrightCCS - SMS2 - QR Configuration image",
				Message = @"
This is your SMS2 configuration code, scan it using Google Authenticator:<br/> <img src=""cid:{attachment}"" />
"
			};
			
			var config = string.Empty;
			var email = string.Empty;
			var displayName = string.Empty;
			var firstName = string.Empty;
			var lastName = string.Empty;
			using (var queries = DBQueriesProvider.Get())
			{
				const string emailQuery = @"SELECT [Email], [DISPLAY_NAME],[FIRSTNAME],[LASTNAME] FROM [SMS_CONTACT] WHERE ID=@UserId";
				using (var dtGet = queries.Query(emailQuery,
	                     new DBQueryParm(@"UserId", userId)
	                    ))
				{
					if (dtGet.Rows.Count == 0)
					{
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.I.WriteToLog("PostSelect skipped: No e-mail found", LogLevel.DebugVerbose);
						return; // No email found
					}
					DataRow row = dtGet.Rows[0];
					email = Convert.ToString(row[0]);
					displayName = ( row["DISPLAY_NAME"] != null && row["DISPLAY_NAME"] != DBNull.Value )
					? row["DISPLAY_NAME"].ToString()
					: "";
					firstName = row.Field<string>("FIRSTNAME");
					lastName = row.Field<string>("LASTNAME");
				}
				if (string.IsNullOrEmpty(email)) {
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog("PostSelect skipped: E-mail empty or not found", LogLevel.DebugVerbose);
					return;
				}
				
				const string query = @"SELECT [config] FROM [UserProviders]
					WHERE userId = @UserId AND authProviderId=@PID ";				
				using (var dtGet = queries.Query(query,
	                     new DBQueryParm(@"UserId", userId),
	                     new DBQueryParm(@"PID", provider.Id)
	                    ))
				{
					if (dtGet.Rows.Count == 0)
					{
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.I.WriteToLog("PostSelect skipped: No config found", LogLevel.DebugVerbose);
						return; // No config found
					}
						config = Convert.ToString(dtGet.Rows[0][0]);
				}
				if (string.IsNullOrEmpty(config)) {
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog("PostSelect skipped: No config found or empty config", LogLevel.DebugVerbose);
					return;
				}
				
				tm = queries.GetTemplateMessage("OATH Setup E-mail");
			}
			
			UsingConfig(config);
			if (configs.Count == 0) {
				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.I.WriteToLog("PostSelect skipped: No devices configured", LogLevel.DebugVerbose);
				return;
			}
			
			try {
				var encoder = new QrEncoder(ErrorCorrectionLevel.M);
							
				var authenticator = new GoogleAuthenticator();
				var model = HotpTotpModel.Unserialize(configs.First().Value);
				QrCode qrCode;
			
				if (!string.IsNullOrWhiteSpace(model.Secret)) {
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog("PostSelect skipped: secret already set", LogLevel.DebugVerbose);
					return; // Some secret was already set, do not reset
				}
				
				var secret = authenticator.GetGeneratedToken();
				var isTotp = (model.Type == HotpTotpModel.TOTP);
				model.Secret = authenticator.GetEncodedSharedSecret(secret, authenticator.HexEncodedChecked());
				var counterOrWindow = (isTotp) ? model.Window : model.CounterSkew;
				var qrSecret = authenticator.GetSecretFormattedForQR(secret, user, isTotp, counterOrWindow);
				encoder.TryEncode(qrSecret, out qrCode);
				
				configs[model.DeviceName] = HotpTotpModel.Serialize(model);
				SaveConfigs(user, org);
				
				var messageTemplate = tm.Message;
				
				messageTemplate = messageTemplate.Replace(USERNAME_LABEL, user);
				messageTemplate = messageTemplate.Replace(FULLNAME_LABEL, displayName);
				messageTemplate = messageTemplate.Replace(FIRSTNAME_LABEL, firstName);
				messageTemplate = messageTemplate.Replace(LASTNAME_LABEL, lastName);
				messageTemplate = messageTemplate.Replace(URL_LABEL, qrSecret);
				messageTemplate = messageTemplate.Replace(SHAREDSECRET_LABEL, secret);
				
				var password = sc.EmailConfig.Password;
				using(var attachment = new MemoryStream()) {
					var renderer = new GraphicsRenderer(
				    	new FixedModuleSize(8, QuietZoneModules.Two));
					renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, attachment);
					
					attachment.Flush();
					attachment.Position = 0;
					serverLogic.Registry.Get<IMailSender>().Send(sc.EmailConfig, password, email, tm.Title, messageTemplate, "qr.png", attachment);
				}
				
			} catch(Exception ex) {
				Logger.Instance.WriteToLog(Name + ".PostSelect ERROR Message: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(Name + ".PostSelect ERROR Stack: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			//model.Secret 
			//serverLogic.Registry.Get<IMailSender>().Send(sc.EmailConfig, destiny, messageTemplate, password);
		}
	}

	public interface IOATHCalcValidatePin
	{
		bool validateToken(tokenresponse tokenResponse, string pin);
	}

	public class OATHException : Exception
	{
		public OATHException(string msg) : base(msg)
		{
		}
	}
}
