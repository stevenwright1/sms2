using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.AuthEngine.Logic.ProviderLogic;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Serializer;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request;
using AuthGateway.Shared.XmlMessages.Request.Command.CloudSms;
using AuthGateway.Shared.XmlMessages.Response;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.CloudSms;

namespace AuthGateway.AuthEngine.ProviderLogic
{
	public class CloudSMSProviderLogic : BaseSendTokenProviderLogic, IProviderLogic, IDisposable
	{
		private RSACryptoServiceProvider cloudSmsRsaProvider = new RSACryptoServiceProvider();
		public override string Name { get { return "CloudSMSProviderLogic"; } }
		public override string DestinyField { get { return "MOBILE_NUMBER"; } }
		public override void DestinyFieldValidate(string value) {
			if (string.IsNullOrEmpty(value))
				throw new Exception("Invalid number");
			bool isEmail = Regex.IsMatch(value, @"^\+?[0-9]+$", RegexOptions.IgnoreCase);
			if (!isEmail) {
				throw new Exception("Invalid phone number");
			}
		}
		protected override string DestinyFieldMissingError { 
			get { 
				if (sc == null || string.IsNullOrEmpty(sc.FieldMissingErrorMobilePhone))
					return "Enter your mobile phone number:";
				return sc.FieldMissingErrorMobilePhone;
			} 
		}

		public override IProviderLogic Using(SystemConfiguration sc)
		{
			this.sc = sc;
			if (sc.CloudSMSUseEncryption)
				cloudSmsRsaProvider.FromXmlString(sc.getCloudSMSPublicKey());
			return this;
		}

		public override int InsertTokensAva(Int64 Value)
		{
			Logger.Instance.WriteToLog("InsertTokensAva(" + Value + ")", LogLevel.Debug);
			Int32 Out = 0;
			try
			{
				using (var queries = DBQueriesProvider.Get())
					Out = queries.NonQuery("INSERT INTO TOKEN_LOG(QUANTITY) VALUES(@QUANTITY)", new DBQueryParm(@"QUANTITY", Value));

				return Out;
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + ".InsertTokensAva ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + ".InsertTokensAva STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				return Out;
			}
		}

		private string sendToCloudSMS(List<CommandBase> commands)
		{
			Logger.Instance.WriteToLog(this.Name + ".sendToCloudSMS Start", LogLevel.Debug);
			try
			{
				String returndata = string.Empty;
				using (TcpClient tcpClient = new TcpClient())
				{
					tcpClient.Connect(sc.getSMSServerAddress(), sc.CloudSMSServerPort);

					NetworkStream ns = tcpClient.GetStream();

					if (ns.CanWrite && ns.CanRead)
					{
						Byte[] key;
						Byte[] iv;
						bool encrypted = false;
						CloudSmsRequest exchangeKeyRequest = new CloudSmsRequest();

						RijndaelManaged aes = new RijndaelManaged();

						byte[] sendBytes;
						MemoryStream retData = new MemoryStream();

						if (sc.CloudSMSUseEncryption)
						{
							Logger.Instance.WriteToLog("Using encryption.", LogLevel.Debug);
							exchangeKeyRequest.Commands = null;
							int keySize = 32;
							int ivSize = 16;
							key = new Byte[keySize];
							iv = new Byte[ivSize];
							RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
							random.GetBytes(key);
							random.GetBytes(iv);
							exchangeKeyRequest.CKey = Convert.ToBase64String(key);
							exchangeKeyRequest.CIV = Convert.ToBase64String(iv);

							string handShake = Generic.Serialize<CloudSmsRequest>(exchangeKeyRequest);
							Logger.Instance.WriteToLog("ClientHandshake: " + handShake, LogLevel.Debug);
							sendBytes = cloudSmsRsaProvider.Encrypt(Encoding.UTF8.GetBytes(handShake), false);
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
							CloudSmsResponse exchangeKeyResponse = Generic.Deserialize<CloudSmsResponse>(returndata);
							if (String.IsNullOrEmpty(exchangeKeyResponse.CHK))
								throw new Exception("Crypt check failed");
							retData = new MemoryStream();

							encrypted = true;
						}

						CloudSmsRequest req = new CloudSmsRequest();
						req.Commands = commands;

						returndata = Generic.Serialize<CloudSmsRequest>(req);

						Logger.Instance.WriteToLog(this.Name + ".sendToCloudSMS: " + returndata, LogLevel.Debug);
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
							try
							{
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
								Logger.Instance.WriteToLog(this.Name + ".sendToCloudSMS FAILED TO DECRYPT ERROR: " + ex.Message, LogLevel.Error);
								Logger.Instance.WriteToLog(this.Name + ".sendToCloudSMS FAILED TO DECRYPT STACK: " + ex.StackTrace, LogLevel.Error);
								Logger.Instance.WriteToLog(this.Name + ".sendToCloudSMS FAILED TO DECRYPT: " + encryptedData, LogLevel.Debug);
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

						Logger.Instance.WriteToLog(this.Name + ".sendToCloudSMS Return Data: " + returndata, LogLevel.Debug);
					}

					tcpClient.Close();
					return returndata.ToString();
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + ".sendToCloudSMS ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + ".sendToCloudSMS STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw ex;
			}
		}

		private string sendToCloudSMS(CommandBase command)
		{
			return sendToCloudSMS(new List<CommandBase>() { command });
		}


		public List<string> GetModules()
		{
			GetModules msg = new GetModules();
			var message = sendToCloudSMS(msg);

#if DEBUG
			Logger.Instance.WriteToLog("GetModules<-response: " + message, LogLevel.Debug);
#endif
			CloudSmsResponse response;
			if (!string.IsNullOrEmpty(message))
				response = Generic.Deserialize<CloudSmsResponse>(message);
			else
				response = new CloudSmsResponse();
			GetModulesRet getmodulesresponse = new GetModulesRet();
			if (response.Responses.Count == 0)
				getmodulesresponse.Error = "No response from CloudSMS";
			else
				getmodulesresponse = (GetModulesRet)response.Responses[0];

			if (!string.IsNullOrEmpty(getmodulesresponse.Error))
				throw new Exception(getmodulesresponse.Error);
			return getmodulesresponse.Modules;
		}

		public string SendSMSRequest(string Mobile, string Msg, string code, string config)
		{
			Logger.Instance.WriteToLog(this.Name + ".SendSMSRequest Start", LogLevel.Debug);
			try
			{
				SendSms sendSms = new SendSms();
				sendSms.Destination = Mobile;
				sendSms.Message = Msg;
				sendSms.Code = code;
				sendSms.ModuleName = config;
				return sendToCloudSMS(sendSms);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + ".SendSMSRequest ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + ".SendSMSRequest STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				return string.Empty;
			}
		}

		public override void SendToken(string user, string org)
		{
			if (!sc.AuthEngineStateless)
				throw new Exception(ERROR_SEND_AHEAD);
			var vur = this.ValidateUser(string.Empty, user, org);
			if (!string.IsNullOrEmpty(vur.Error))
			{
				if (sc.BaseSendTokenTestMode)
					throw new Exception(vur.Error); // Token
				else
				{
					Logger.Instance.WriteToLog(this.Name + ".SendToken ERROR: " + vur.Error, LogLevel.Debug);
					throw new Exception("An error occurred when sending token.");
				}
			}
		}

		protected override void sendTokenToUser(ValidateUserRet ret, string mobilePhone, String Passcode, string messageTemplate, TemplateMessage tm)
		{
			string csmsreponsestr = SendSMSRequest(mobilePhone, messageTemplate, Passcode, config);
			CloudSmsResponse csmsresponse;
			if (!string.IsNullOrEmpty(csmsreponsestr))
				csmsresponse = Generic.Deserialize<CloudSmsResponse>(csmsreponsestr);
			else
				csmsresponse = new CloudSmsResponse();
			SendSmsRet sendsmsret = new SendSmsRet();
			if (csmsresponse.Responses.Count == 0)
				sendsmsret.Error = "No response from CloudSMS";
			else
				sendsmsret = (SendSmsRet)csmsresponse.Responses[0];
			if (string.IsNullOrEmpty(sendsmsret.Error))
			{
				ret.CreditsRemaining = sendsmsret.CreditsRemaining;
				Double tokens = 0;
				if (Double.TryParse(sendsmsret.CreditsRemaining, out tokens))
					InsertTokensAva(Convert.ToInt64(tokens));
				if (sc.BaseSendTokenTestMode) // Return Passcode in test mode
					ret.Error = Passcode;
			}
			else
				ret.Error = sendsmsret.Error;
		}

		public void Dispose()
		{
			if (cloudSmsRsaProvider != null)
				cloudSmsRsaProvider.Dispose();
		}
	}
}
