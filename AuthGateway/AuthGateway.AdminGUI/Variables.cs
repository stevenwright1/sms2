using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Serializer;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using Microsoft.VisualBasic;

namespace AuthGateway.AdminGUI
{

	public class Variables
	{

		public string OrgNameP = "";
		public bool IsAdmin;
		public SystemConfiguration SC = null;

		private RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
		public Variables(SystemConfiguration sysConf)
		{
			Load(sysConf);
		}
		public Variables(string scPath)
		{
			string userConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Wright.AdminGUI");
			SystemConfiguration config = new SystemConfiguration();
			try {
				Logger.Instance.WriteToLog(string.Format("Loading config from: {0} looking at {1} ", Application.StartupPath, "SettingsPublic"), LogLevel.Error);
				config = new SystemConfiguration(Application.StartupPath, "SettingsPublic");
			} catch {
				Logger.Instance.WriteToLog(string.Format("Trying config from: {0} looking at {1} ", userConfig + Path.DirectorySeparatorChar + "Dummy", "Settings"), LogLevel.Error);
				if (File.Exists(userConfig + Path.DirectorySeparatorChar + "Configuration.xml")) {
					Logger.Instance.WriteToLog(string.Format("Loading config from: {0} looking at {1} ", userConfig + Path.DirectorySeparatorChar + "Dummy", "Settings"), LogLevel.Error);
					config = new SystemConfiguration(userConfig + Path.DirectorySeparatorChar + "Dummy");
				}
			}
			config.LoadSettings();
			Load(config);
		}

		private void Load(SystemConfiguration sysConf)
		{
			SC = sysConf;
			LoadRsaProvider(SC);
		}

		public string SendData(AuthEngineRequest req)
		{
			TcpClient tcpClient = new TcpClient();
			MemoryStream returndata = new MemoryStream();
			tcpClient.ReceiveTimeout = 5000;

			bool encrypted = false;

			byte[] Key = new byte[1];
			byte[] IV = new byte[1];
			AuthEngineRequest exchangeKeyRequest = new AuthEngineRequest();
			if (SC.AuthEngineUseEncryption) {
				int keySize = 31;
				//Convert.ToInt32(256 / 8) - 1
				int ivSize = 15;
				// Convert.ToInt32(128 / 8) - 1
				Key = new byte[keySize + 1];
				IV = new byte[ivSize + 1];

				RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
				random.GetBytes(Key);
				random.GetBytes(IV);
				exchangeKeyRequest.CKey = Convert.ToBase64String(Key);
				exchangeKeyRequest.CIV = Convert.ToBase64String(IV);
				encrypted = true;
			}

			try {
				tcpClient.Connect(SC.getAuthEngineServerAddress(), SC.AuthEngineServerPort);
				NetworkStream networkStream = tcpClient.GetStream();

				string cmdStr = null;
				byte[] sendBytes = null;

				RijndaelManaged aes = null;

				string retStr = string.Empty;

				if (networkStream.CanWrite & networkStream.CanRead) {
					byte[] bytes = new byte[tcpClient.ReceiveBufferSize + 1];
					if (encrypted) {
						exchangeKeyRequest.auth = "T";
						exchangeKeyRequest.Commands = null;
						cmdStr = Generic.Serialize<AuthEngineRequest>(exchangeKeyRequest);
						Logger.Instance.WriteToLog("Client Handshake: " + cmdStr, LogLevel.Debug);
						sendBytes = rsaProvider.Encrypt(Encoding.UTF8.GetBytes(cmdStr), false);
						networkStream.Write(sendBytes, 0, sendBytes.Length);
						do {
							int bread = networkStream.Read(bytes, 0, Convert.ToInt32(tcpClient.ReceiveBufferSize));
							returndata.Write(bytes, 0, bread);
						} while (networkStream.DataAvailable);
						aes = new RijndaelManaged();
						aes.Padding = PaddingMode.PKCS7;
						aes.Mode = CipherMode.CBC;
						aes.KeySize = 256;
						aes.Key = Key;
						aes.IV = IV;
						ICryptoTransform cryptor = aes.CreateDecryptor(aes.Key, aes.IV);
						returndata.Position = 0;
						CryptoStream cs = new CryptoStream(returndata, cryptor, CryptoStreamMode.Read);
						StreamReader sr = new StreamReader(cs);
						retStr = sr.ReadToEnd();
						sr.Close();
						cs.Close();
						AuthEngineResponse exchangeKeyResponse = Generic.Deserialize<AuthEngineResponse>(retStr);
						if ((string.IsNullOrEmpty(exchangeKeyResponse.CHK))) {
							throw new Exception("Crypt check failed");
						}
					// Send AuthEngineRequest.Auth
					} else {
						AuthEngineRequest exchangeAuthReq = new AuthEngineRequest();
						exchangeAuthReq.auth = "True";
						cmdStr = Generic.Serialize<AuthEngineRequest>(exchangeAuthReq);
						sendBytes = Encoding.UTF8.GetBytes(cmdStr);
						networkStream.Write(sendBytes, 0, sendBytes.Length);
						do {
							int bread = networkStream.Read(bytes, 0, Convert.ToInt32(tcpClient.ReceiveBufferSize));
							returndata.Write(bytes, 0, bread);
						} while (networkStream.DataAvailable);
						returndata.Position = 0;

						StreamReader sr = new StreamReader(returndata);
						retStr = sr.ReadToEnd();
						AuthEngineResponse exchangeAuthResp = Generic.Deserialize<AuthEngineResponse>(retStr);
						if (string.IsNullOrEmpty(exchangeAuthResp.CHK)) {
							throw new Exception("Error sending AuthEngineRequest.Auth");
						}
					}

					System.Net.Security.NegotiateStream negotiateStream = new System.Net.Security.NegotiateStream(networkStream, true);
					negotiateStream.AuthenticateAsClient();

					cmdStr = Generic.Serialize<AuthEngineRequest>(req);
                    if (cmdStr.Contains(string.Format("<Level>{0}</Level>", LogLevel.DebugVerbose))) {
                        Logger.Instance.WriteToLog("Client Request: " + cmdStr, LogLevel.DebugVerbose);
                    }
                    else                
					    Logger.Instance.WriteToLog("SendData.Client Request: " + cmdStr, LogLevel.Debug);
					if (encrypted) {
						ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
						MemoryStream msEncrypt = new MemoryStream();
						CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
						StreamWriter swEncrypt = new StreamWriter(csEncrypt);
						swEncrypt.Write(cmdStr);
						swEncrypt.Close();
						csEncrypt.Close();
						sendBytes = msEncrypt.ToArray();
					} else {
						sendBytes = Encoding.UTF8.GetBytes(cmdStr);
					}
					networkStream.Write(sendBytes, 0, sendBytes.Length);

					returndata = new MemoryStream();
					do {
						int bread = networkStream.Read(bytes, 0, Convert.ToInt32(tcpClient.ReceiveBufferSize));
						returndata.Write(bytes, 0, bread);
					} while (networkStream.DataAvailable);
					returndata.Position = 0;


					if (encrypted) {
						try {
							ICryptoTransform cryptor = aes.CreateDecryptor(aes.Key, aes.IV);
							CryptoStream cs = new CryptoStream(returndata, cryptor, CryptoStreamMode.Read);
							StreamReader sr = new StreamReader(cs);
							retStr = sr.ReadToEnd();
							sr.Close();
							cs.Close();
						} catch (Exception ex) {
							#if DEBUG
							returndata.Position = 0;
							StreamReader sr = new StreamReader(returndata);
							Logger.Instance.WriteToLog("SendData.FAILED TO DECRYPT ERROR: " + ex.Message, LogLevel.Error);
							Logger.Instance.WriteToLog("SendData.FAILED TO DECRYPT STACK: " + ex.Message, LogLevel.Error);
							Logger.Instance.WriteToLog("SendData.FAILED TO DECRYPT: " + sr.ReadToEnd(), LogLevel.Debug);
							#endif
							throw;
						}
					} else {
						StreamReader sr = new StreamReader(returndata);
						retStr = sr.ReadToEnd();
					}

                    if (retStr.Contains(string.Format("<Level>{0}</Level>", LogLevel.DebugVerbose))) {
                        Logger.Instance.WriteToLog("Server Response: " + retStr, LogLevel.DebugVerbose);
                    }
                    else                
					    Logger.Instance.WriteToLog("SendData.Server Response: " + retStr, LogLevel.Debug);
				}
				return retStr;
			} finally {
				tcpClient.Close();
			}
		}

		public UsersRet GetUsers(int startAt, int Total, bool onlyOverriden, string filterByFriendlyProviderName, bool? showAdmins, string text)
		{
			string returndata = string.Empty;
			var defRet = new UsersRet { Error = "Error getting users" };
			try {
				var req = new AuthEngineRequest();
				var cmd = new Users {
					Admins = showAdmins
				};
				cmd.At = startAt;
				cmd.Total = Total;
				cmd.Org = OrgNameP;
				cmd.Overrided = onlyOverriden;
				cmd.FName = filterByFriendlyProviderName;
				cmd.Text = text;
				req.Commands.Add(cmd);
				returndata = SendData(req);

				Logger.Instance.WriteToLog("GetUsers Server Returned: " + returndata, LogLevel.Debug);

				AuthEngineResponse response = Generic.Deserialize<AuthEngineResponse>(returndata);
				return (UsersRet)response.Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("GetUsers Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("GetUsers Trace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public TokenLenRet LoadPassCodeLen()
		{
			string returndata = "";
			TokenLenRet defRet = new TokenLenRet { Error = "Error loading Passcode length" };
			try {
				AuthEngineRequest req = new AuthEngineRequest();
				TokenLen cmd = new TokenLen();
				req.Commands.Add(cmd);
				returndata = SendData(req);
				Logger.Instance.WriteToLog("LoadPassCodeLen Server Returned: " + returndata, LogLevel.Debug);
				return (TokenLenRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("LoadPassCodeLen Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("LoadPassCodeLen Trace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}
		
		public SetSettingRet SaveSetting(string name, string value) {
			var defRet = new SetSettingRet { Error = "Error updating setting." };
			try {
				var req = new AuthEngineRequest();
				var cmd = new SetSetting();
				cmd.Name = name;
				cmd.Value = value;
				req.Commands.Add(cmd);
				var returndata = SendData(req);

				Logger.Instance.WriteToLog("SaveSetting Server Returned: " + returndata, LogLevel.Debug);
				return (SetSettingRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("SaveSetting Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("SaveSetting StackTrace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}
		
		public SetSettingsRet SaveSettings(List<Setting> settings) {
			var defRet = new SetSettingsRet { Error = "Error updating settings." };
			try {
				var req = new AuthEngineRequest();
				var cmd = new SetSettings();
				cmd.Settings = settings;
				req.Commands.Add(cmd);
				var returndata = SendData(req);

				Logger.Instance.WriteToLog("SaveSettings Server Returned: " + returndata, LogLevel.Debug);
				return (SetSettingsRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("SaveSettings Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("SaveSettings StackTrace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public List<Setting> AeGetSettings() {
			try {
				var req = new AuthEngineRequest();
				var cmd = new GetSettings();
				req.Commands.Add(cmd);
				var retData = SendData(req);
				Logger.Instance.WriteToLog("getSettings Server Returned: " + retData, LogLevel.Debug);
				var ret = (GetSettingsRet)Generic.Deserialize<AuthEngineResponse>(retData).Responses[0];
				return ret.Settings;
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("getSettings Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("getSettings Trace: " + ex.StackTrace, LogLevel.Error);
				throw new RemoteException("Error getting settings.");
			}
		}

        public List<Setting> AeGetUserSettings()
        {
            try {
                var req = new AuthEngineRequest();
                var cmd = new GetUserSettings();
                req.Commands.Add(cmd);
                var retData = SendData(req);
                Logger.Instance.WriteToLog("getUserSettings Server Returned: " + retData, LogLevel.Debug);
                var ret = (GetSettingsRet)Generic.Deserialize<AuthEngineResponse>(retData).Responses[0];
                return ret.Settings;
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("getUserSettings Error: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("getUserSettings Trace: " + ex.StackTrace, LogLevel.Error);
                throw new RemoteException("Error getting user settings.");
            }
        }
		
		public List<TemplateMessage> getTemplateMessages() {
			try {
				var req = new AuthEngineRequest();
				var cmd = new AllMsgs();
				req.Commands.Add(cmd);
				var retData = SendData(req);
				Logger.Instance.WriteToLog("getTemplateMessages Server Returned: " + retData, LogLevel.Debug);
				var allMsgsRet = (AllMsgsRet)Generic.Deserialize<AuthEngineResponse>(retData).Responses[0];
				var messages = allMsgsRet.Messages
					.Select(x => new TemplateMessage {
			        	Label = x.Label,
			        	Title = x.Title,
                    	Message = x.Message
                    		.Replace("\r\n", "\n")
                    		.Replace("\n","\r\n"),
                    	Legend = x.Legend
                    		.Replace("\r\n", "\n")
                    		.Replace("\n","\r\n"),
                    	Order = x.Order
                    })
					.OrderBy(x => x.Order)
					.ToList()
					;
				if (Logger.I.ShouldLog(LogLevel.Debug))
					messages.ForEach(x => 
				                            Logger.I.WriteToLog(string.Format("getTemplateMessages template: {0} {1} {2} {3}", x.Label, x.Legend, x.Message, x.Order)
				                                                , LogLevel.Debug)
				                           );
				return messages;
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("getTemplateMessages Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("getTemplateMessages Trace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw new RemoteException("Error loading messages.");
			}
		}

		public TokensRet GetTokens()
		{
			string returndata = "";
			TokensRet defRet = new TokensRet { Error = "Error getting tokens" };
			try {
				AuthEngineRequest req = new AuthEngineRequest();
				Tokens cmd = new Tokens();
				req.Commands.Add(cmd);
				returndata = SendData(req);
				Logger.Instance.WriteToLog("GetTokens Server Returned: " + returndata, LogLevel.Debug);

				return (TokensRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("GetTokens Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("GetTokens Trace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public DetailsRet GetDetails(string Username, string OrgName)
		{
			string returndata = "";
			DetailsRet defRet = new DetailsRet { Error = "Error getting details" };
			try {
				Logger.Instance.WriteToLog("GetDetails.Sending", LogLevel.Debug);
				AuthEngineRequest req = new AuthEngineRequest();
				Details cmd = new Details();
				cmd.User = Username;
				cmd.Org = OrgName;
				req.Commands.Add(cmd);
				returndata = SendData(req);

				Logger.Instance.WriteToLog("GetDetails.Server Returned: " + returndata, LogLevel.Debug);
				return (DetailsRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("GetDetails Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("GetDetails Trace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public UpdateMessageRet updateMessage(string label, string title, string message)
		{
			var defRet = new UpdateMessageRet { Error = "Error updating message " };
			try {
				var req = new AuthEngineRequest();
				var cmd = new UpdateMessage();
				cmd.Label = label;
				cmd.Title = title;
				cmd.Message = message;
				req.Commands.Add(cmd);
				var returndata = SendData(req);

				Logger.Instance.WriteToLog("Server Returned: " + returndata, LogLevel.Debug);
				return (UpdateMessageRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("updateMessage Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("updateMessage StackTrace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public UpdateTokenLenRet UpdatePassCodeLen(string Value)
		{
			string returndata = "";
			UpdateTokenLenRet defRet = new UpdateTokenLenRet { Error = "Error updating Passcode length" };
			try {
				AuthEngineRequest req = new AuthEngineRequest();
				UpdateTokenLen cmd = new UpdateTokenLen();
				cmd.Length = Convert.ToInt32(Value);
				req.Commands.Add(cmd);
				returndata = SendData(req);
				Logger.Instance.WriteToLog("UpdatePassCodeLen.Server Returned: " + returndata, LogLevel.Debug);
				return (UpdateTokenLenRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("UpdatePassCodeLen Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("UpdatePassCodeLen StackTrace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public void LoadRsaProvider(SystemConfiguration sysConf)
		{
			if ((sysConf.AuthEngineUseEncryption)) {
				rsaProvider.FromXmlString(sysConf.getAuthEnginePublicKey());
			}
		}

		public int DomainOrWG()
		{
			NetworkInformation info = NetworkInformation.LocalComputer;

			return NetworkInformation.DomainOrWG();
		}

		public PermissionsRet validatePermissions(string Username, string OrgName)
		{
			PermissionsRet defRet = new PermissionsRet { Error = "Error validating user permissions" };
			try {
				AuthEngineRequest req = new AuthEngineRequest();
				Permissions perms = new Permissions();
				perms.User = Username;
				perms.Org = OrgName;
				req.Commands.Add(perms);
				string returndata = SendData(req);

				Logger.Instance.WriteToLog("validatePermissions.Server Returned: " + returndata, LogLevel.Debug);

				return (PermissionsRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (SocketException ex) {
				Logger.Instance.WriteToLog("validatePermissions Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("validatePermissions StackTrace: " + ex.StackTrace, LogLevel.Error);
				defRet.Error = "Error contacting AuthEngine.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("validatePermissions Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("validatePermissions StackTrace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public UpdateFullDetailsRet UpdateDetails(string Username, string Phone, string Mobile
			, string lastName, string firstName, string UserType, string Orgname, bool authEnabled
			, string pinCode, bool pinCodeChanged, string email, bool lockedDown, string UPN)
		{
			string returndata = string.Empty;
			UpdateFullDetailsRet defRet = new UpdateFullDetailsRet { Error = "Error in update details" };

			try {
				var req = new AuthEngineRequest();
				var cmd = new UpdateFullDetails();
				cmd.User = Username;
				cmd.Phone = Phone;
				cmd.Mobile = Mobile;
				//cmd.Fullname = Fullname;
				cmd.LastName = lastName;
				cmd.FirstName = firstName;
				cmd.UserType = UserType;
				cmd.Org = Orgname;
				cmd.AuthEnabled = authEnabled;
				cmd.PinCode = pinCode;
				cmd.PCChange = pinCodeChanged;
				cmd.Email = email;
				cmd.Locked = lockedDown;
                cmd.UPN = UPN;
				req.Commands.Add(cmd);
				returndata = SendData(req);

				Logger.Instance.WriteToLog("UpdateDetails.Server Returned: " + returndata, LogLevel.Debug);
				return (UpdateFullDetailsRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("UpdateDetails Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("UpdateDetails StackTrace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public UserProvidersRet GetProviders(UserProviders cmd)
		{
			var returndata = "";
			var defRet = new UserProvidersRet { Error = "Error getting user providers" };
			try {
				var req = new AuthEngineRequest();
				req.Commands.Add(cmd);
				returndata = SendData(req);
				Logger.Instance.WriteToLog("GetProviders Server Returned: " + returndata, LogLevel.Debug);

				return (UserProvidersRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("GetProviders Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("GetProviders Trace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public UserProvidersRet GetProviders(string user, string org)
		{
			var cmd = new UserProviders {
				User = user,
				Org = org
			};
			return GetProviders(cmd);
		}
		
		public List<string> GetProvidersList() {
			var cmd = new ProvidersList();
			var ret = new List<string>();
			try {
				var req = new AuthEngineRequest();
				req.Commands.Add(cmd);
				var returndata = SendData(req);
				Logger.Instance.WriteToLog("GetProvidersList Server Returned: " + returndata, LogLevel.Debug);

				var providersList = (UserProvidersRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
				foreach(var provider in providersList.Providers) {
					ret.Add(provider.FName);
				}
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("GetProvidersList Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("GetProvidersList Trace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw;
			}
			
			return ret;
		}

		public SetUserProviderRet SetProvider(string user, string org, UserProvider provider)
		{
			string returndata = "";
			SetUserProviderRet defRet = new SetUserProviderRet { Error = "Error setting user provider" };
			try {
				AuthEngineRequest req = new AuthEngineRequest();
				SetUserProvider cmd = new SetUserProvider();
				cmd.User = user;
				cmd.Org = org;
				cmd.Provider = provider;
				req.Commands.Add(cmd);
				returndata = SendData(req);
				Logger.Instance.WriteToLog("SetProvider Server Returned: " + returndata, LogLevel.Debug);

				return (SetUserProviderRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("SetProvider Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("SetProvider Trace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public ResyncHotpRet ResyncProvider(string user, string org, string action, string parameters, string token1, string token2)
		{
			string returndata = "";
			ResyncHotpRet defRet = new ResyncHotpRet { Error = "Error in resync provider" };
			try {
				AuthEngineRequest req = new AuthEngineRequest();
				ResyncHotp cmd = new ResyncHotp();
				cmd.User = user;
				cmd.Org = org;
				cmd.Token1 = token1;
				cmd.Token2 = token2;
				cmd.Action = action;
				cmd.Parameters = parameters;
				req.Commands.Add(cmd);
				returndata = SendData(req);
				Logger.Instance.WriteToLog("ResyncProvider Server Returned: " + returndata, LogLevel.Debug);

				return (ResyncHotpRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("ResyncProvider Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ResyncProvider Trace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public string TrimAll(string TextIn, string TrimChar = " ", string CtrlChar = "\0")
		{
			return TextIn.TrimEnd(new char[] { Convert.ToChar(Constants.vbNullChar) }).Trim();
		}

		public PollUsersRet PollUsers()
		{
			var cmd = new PollUsers();
			var req = new AuthEngineRequest();
			var defRet = new PollUsersRet { Error = "Error calling to poll users." };
			req.Commands.Add(cmd);
			try {
				string returndata = SendData(req);
				return (PollUsersRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("PollUsersRet Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("PollUsersRet Trace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public SendTokenRet SendToken(string user, string domain, bool emergency = false)
		{
			SendTokenRet defRet = new SendTokenRet { Error = "Error sending Passcode" };
			try {
				AuthEngineRequest req = new AuthEngineRequest();
				SendToken cmd = new SendToken();
				cmd.User = user;
				cmd.Org = domain;
				cmd.Emergency = emergency;
				req.Commands.Add(cmd);
				string returndata = SendData(req);
				Logger.Instance.WriteToLog("SendToken Server Returned: " + returndata, LogLevel.Debug);

				SendTokenRet ret = (SendTokenRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
				if (!string.IsNullOrEmpty(ret.Error)) {
					if (SC.BaseSendTokenTestMode) {
						return ret;
					}
					defRet.Error = defRet.Error + ": " + ret.Error;
				} else {
					return ret;
				}
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("SendToken Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("SendToken Trace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}
		
		public ClearPinRet ClearPin(string user, string domain)
		{
			var cmd = new ClearPin {
				User = user,
				Org = domain
			};
			var req = new AuthEngineRequest();
			var defRet = new ClearPinRet { Error = "Error calling remote clear pin procedure." };
			req.Commands.Add(cmd);
			try {
				var returndata = SendData(req);
				return (ClearPinRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("ClearPin Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ClearPin Trace: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return defRet;
		}

		public List<string> GetModules(string user, string domain)
		{
			try {
				AuthEngineRequest req = new AuthEngineRequest();
				GetAvailableModules cmd = new GetAvailableModules();
				cmd.User = user;
				cmd.Org = domain;
				req.Commands.Add(cmd);
				string returndata = SendData(req);
				Logger.Instance.WriteToLog("GetModules Returned: " + returndata, LogLevel.Debug);

				GetAvailableModulesRet ret = (GetAvailableModulesRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
				if (!string.IsNullOrEmpty(ret.Error)) {
					throw new Exception(ret.Error);
				}
				return ret.Modules;
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("GetModules ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("GetModules TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw ex;
			}
		}

		public List<UserRet> loadUsersUntilClear(bool onlyOverriden, string filterByProviderFriendlyName, bool? showAdmins, string text)
		{
			return loadUsersUntilClear(null, null, onlyOverriden, filterByProviderFriendlyName, showAdmins, text);
		}


		public List<UserRet> loadUsersUntilClear(Action<List<UserRet>> cb, Action checkCancel, bool onlyOverriden, string filterByProviderFriendlyName, bool? showAdmins, string text)
		{
			const int usersPerRequest = 130;
			UsersRet userList = null;
			var loadedUsers = new List<UserRet>();
			var loadedUsersCount = 0;

			userList = GetUsers(1, usersPerRequest, onlyOverriden, filterByProviderFriendlyName, showAdmins, text);

			while (userList.Users.Count > 0) {
				if (checkCancel != null)
					checkCancel();
				if ((!string.IsNullOrEmpty(userList.Error))) {
					throw new Exception(userList.Error);
				}
				loadedUsersCount += userList.Users.Count;
				// Only fill loadedUsers when no callback is set
				if (cb != null)
					cb(userList.Users);
				else
					loadedUsers.AddRange(userList.Users);
				userList = GetUsers(loadedUsersCount + 1, (loadedUsersCount + usersPerRequest), onlyOverriden, filterByProviderFriendlyName, showAdmins, text);
			}
			if ((!string.IsNullOrEmpty(userList.Error))) {
				throw new Exception(userList.Error);
			}

			return loadedUsers;
		}

        public List<string> GetDomains()
        {
            try
            {
                AuthEngineRequest req = new AuthEngineRequest();
                Domains cmd = new Domains();                
                req.Commands.Add(cmd);
                string returndata = SendData(req);
                Logger.Instance.WriteToLog("GetDomains Returned: " + returndata, LogLevel.Debug);

                DomainsRet ret = (DomainsRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                if (!string.IsNullOrEmpty(ret.Error))
                {
                    throw new Exception(ret.Error);
                }
                return ret.Domains;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("GetDomains ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("GetDomains TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public List<string> GetAliases(string domain)
        {
            Logger.Instance.WriteToLog("GetAliases", LogLevel.Debug);
            try
            {
                AuthEngineRequest req = new AuthEngineRequest();
                Aliases cmd = new Aliases();
                cmd.Domain = domain;
                req.Commands.Add(cmd);
                string returndata = SendData(req);
                Logger.Instance.WriteToLog("GetAliases Returned: " + returndata, LogLevel.Debug);

                AliasesRet ret = (AliasesRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                if (!string.IsNullOrEmpty(ret.Error))
                {
                    throw new Exception(ret.Error);
                }
                return ret.Aliases;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("GetAliases ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("GetAliases TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public void UpdateAliases(string domain, List<string> aliases)
        {
            Logger.Instance.WriteToLog("UpdateAliases", LogLevel.Debug);
            try
            {
                AuthEngineRequest req = new AuthEngineRequest();
                UpdateDomainAliases cmd = new UpdateDomainAliases();
                cmd.Domain = domain;
                cmd.Aliases = aliases;
                req.Commands.Add(cmd);
                string returndata = SendData(req);
                Logger.Instance.WriteToLog("UpdateAliases Returned: " + returndata, LogLevel.Debug);

                UpdateAliasesRet ret = (UpdateAliasesRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                if (!string.IsNullOrEmpty(ret.Error))
                {
                    throw new Exception(ret.Error);
                }
                
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("UpdateAliases ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("UpdateAliases TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public bool GetPanicState(string user, string domain)
        {
            try
            {
                AuthEngineRequest req = new AuthEngineRequest();
                GetPanicState cmd = new GetPanicState();
                cmd.User = user;
                cmd.Org = domain;                
                req.Commands.Add(cmd);                
                string returndata = SendData(req);
                Logger.Instance.WriteToLog("GetPanicState Returned: " + returndata, LogLevel.Debug);

                GetPanicRet ret = (GetPanicRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                if (!string.IsNullOrEmpty(ret.Error))
                {
                    throw new Exception(ret.Error);
                }
                return ret.Panic;                
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("GetPanicState ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("GetPanicState TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public void ResetPanicState(string user, string domain)
        {
            try
            {
                AuthEngineRequest req = new AuthEngineRequest();
                ResetPanicState cmd = new ResetPanicState();
                cmd.User = user;
                cmd.Org = domain;
                req.Commands.Add(cmd);
                string returndata = SendData(req);
                Logger.Instance.WriteToLog("ResetPanicState Returned: " + returndata, LogLevel.Debug);

                ResetPanicRet ret = (ResetPanicRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                if (!string.IsNullOrEmpty(ret.Error))
                {
                    throw new Exception(ret.Error);
                }
                
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("ResetPanicState ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ResetPanicState TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public SetUserAuthImagesRet SetUserAuthImages(string user, string domain, string leftImage, string rightImage)
        {
            try {
                AuthEngineRequest req = new AuthEngineRequest();
                SetUserAuthImages cmd = new SetUserAuthImages();
                cmd.User = user;
                cmd.Org = domain;
                cmd.LeftImage = leftImage;
                cmd.RightImage = rightImage;
                req.Commands.Add(cmd);
                string returndata = SendData(req);
                Logger.Instance.WriteToLog("SetUserAuthImages Returned: " + returndata, LogLevel.Debug);

                SetUserAuthImagesRet ret = (SetUserAuthImagesRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                return ret;

            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("SetUserAuthImages ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("SetUserAuthImages TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public GetUserAuthImagesRet GetUserAuthImages(string user, string domain)
        {
            Logger.Instance.WriteToLog("GetUserAuthImages", LogLevel.Debug);
            try {
                AuthEngineRequest req = new AuthEngineRequest();
                GetUserAuthImages cmd = new GetUserAuthImages();
                cmd.User = user;
                cmd.Org = domain;                
                req.Commands.Add(cmd);
                string returndata = SendData(req);
                //Logger.Instance.WriteToLog("GetUserAuthImages Returned: " + returndata, LogLevel.Debug);

                GetUserAuthImagesRet ret = (GetUserAuthImagesRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                return ret;

            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("GetUserAuthImages ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("GetUserAuthImages TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }        

        public GetImageRet GetImage(string url)
        {
            try {
                AuthEngineRequest req = new AuthEngineRequest();
                GetImage cmd = new GetImage();
                cmd.Url = url;                
                req.Commands.Add(cmd);
                string returndata = SendData(req);
                //Logger.Instance.WriteToLog("RetrieveImage Returned: " + returndata, LogLevel.Debug);

                GetImageRet ret = (GetImageRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                return ret;
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("GetImage ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("GetImage TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public GetImagesByCategoryRet GetImagesByCategory(string category)
        {
            try {
                AuthEngineRequest req = new AuthEngineRequest();
                GetImagesByCategory cmd = new GetImagesByCategory();
                cmd.Category = category;
                req.Commands.Add(cmd);
                string returndata = SendData(req);
                //Logger.Instance.WriteToLog("RetrieveImage Returned: " + returndata, LogLevel.Debug);

                GetImagesByCategoryRet ret = (GetImagesByCategoryRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                return ret;
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("GetImagesByCategory ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("GetImagesByCategory TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public GetAliveServersRet GetAliveServers()
        {
            try {
                AuthEngineRequest req = new AuthEngineRequest();
                GetAliveServers cmd = new GetAliveServers();                
                req.Commands.Add(cmd);
                string returndata = SendData(req);
                Logger.Instance.WriteToLog("GetAliveServers Returned: " + returndata, LogLevel.Debug);

                GetAliveServersRet ret = (GetAliveServersRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                return ret;
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("GetAliveServers ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("GetAliveServers TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public SetServerPreferencesRet SetServerPreferences(string hostname, string macAdrress, PollingPreference usersPollingPreference/*, PollingPreference imagesPollingPreference*/)
        {
            try {
                AuthEngineRequest req = new AuthEngineRequest();
                SetServerPreferences cmd = new SetServerPreferences();
                cmd.Hostname = hostname;
                cmd.MACAddress = macAdrress;
                cmd.UsersPollingPreference = usersPollingPreference;
                //cmd.ImagesPollingPreference = imagesPollingPreference;
                req.Commands.Add(cmd);
                string returndata = SendData(req);
                Logger.Instance.WriteToLog("SetServerPreferences Returned: " + returndata, LogLevel.Debug);

                SetServerPreferencesRet ret = (SetServerPreferencesRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                return ret;
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("SetServerPreferences ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("SetServerPreferences TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public ApplyOATHCalcDefaultsRet ApplyOATHCalcDefaults(string defaultConfig)
        {
            try {
                AuthEngineRequest req = new AuthEngineRequest();
                ApplyOATHCalcDefaults cmd = new ApplyOATHCalcDefaults();
                cmd.DefaultConfig = defaultConfig;
                req.Commands.Add(cmd);

                string returndata = SendData(req);
                Logger.Instance.WriteToLog("ApplyOATHCalcDefaults Returned: " + returndata, LogLevel.Debug);

                ApplyOATHCalcDefaultsRet ret = (ApplyOATHCalcDefaultsRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                return ret;
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("ApplyOATHCalcDefaults ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("ApplyOATHCalcDefaults TRACE: " + ex.StackTrace, LogLevel.Error);                
                throw ex;
            }

        }

	}

	public class RemoteException : Exception
	{
		public RemoteException(string message) : base(message) {
			
		}
	}
}
