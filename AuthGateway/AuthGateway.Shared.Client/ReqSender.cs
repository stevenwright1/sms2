using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Serializer;
using AuthGateway.Shared.XmlMessages.Request;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;

namespace AuthGateway.Shared.Client
{
	public abstract class ReqSender<T, RESP> where T: EncryptableRequest, new() where RESP: EncryptCheckResponse, new()
	{
		protected RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
		private IPAddress address;
		private int port;
		private bool encrypt;

		protected abstract string Name { get; }

		public ReqSender(IPAddress address, int port, bool encrypt, string encryptPubKey)
		{
			this.address = address;
			this.port = port;
			this.encrypt = encrypt;
			if (!string.IsNullOrEmpty(encryptPubKey))
				rsaProvider.FromXmlString(encryptPubKey);
		}

		public virtual string SendData(T req)
		{
			Logger.Instance.WriteToLog(this.Name + ".SendData Start", LogLevel.Debug);
			try
			{
				String returndata = string.Empty;
				using (TcpClient tcpClient = new TcpClient())
				{
					tcpClient.Connect(this.address, this.port);

					NetworkStream ns = tcpClient.GetStream();

					if (ns.CanWrite && ns.CanRead)
					{
						Byte[] key;
						Byte[] iv;
						bool encrypted = false;
						var exchangeKeyRequest = new T();

						RijndaelManaged aes = new RijndaelManaged();

						byte[] sendBytes;
						MemoryStream retData = new MemoryStream();

						if (this.encrypt)
						{
							Logger.Instance.WriteToLog(this.Name + " using encryption.", LogLevel.Debug);
							int keySize = 32;
							int ivSize = 16;
							key = new Byte[keySize];
							iv = new Byte[ivSize];
							var random = new RNGCryptoServiceProvider();
							random.GetBytes(key);
							random.GetBytes(iv);
							exchangeKeyRequest.Nullify();
							exchangeKeyRequest.CKey = Convert.ToBase64String(key);
							exchangeKeyRequest.CIV = Convert.ToBase64String(iv);

							string handShake = Generic.Serialize<T>(exchangeKeyRequest);
							Logger.Instance.WriteToLog(this.Name + ".ClientHandshake: " + handShake, LogLevel.Debug);
							sendBytes = rsaProvider.Encrypt(Encoding.UTF8.GetBytes(handShake), false);
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
							var exchangeKeyResponse = Generic.Deserialize<RESP>(returndata);
							if (String.IsNullOrEmpty(exchangeKeyResponse.CHK))
								throw new Exception("Crypt check failed");
							retData = new MemoryStream();

							encrypted = true;
						}

						returndata = Generic.Serialize<T>(req);

						Logger.Instance.WriteToLog(this.Name + ".Request: " + returndata, LogLevel.Debug);
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
							ICryptoTransform cryptor = aes.CreateDecryptor(aes.Key, aes.IV);
							using (CryptoStream cs = new CryptoStream(retData, cryptor, CryptoStreamMode.Read))
							{
								using (StreamReader sr = new StreamReader(cs))
								{
									returndata = sr.ReadToEnd();
								}
							}
						}
						else
						{
							using (StreamReader sr = new StreamReader(retData))
								returndata = sr.ReadToEnd();
						}
						returndata = returndata.TrimEnd(new char[] { (char)0 }).Trim();

						Logger.Instance.WriteToLog(this.Name + ".SendData Return Data: " + returndata, LogLevel.Debug);
					}

					tcpClient.Close();
					return returndata.ToString();
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + ".SendData ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + ".SendData STACK: " + ex.StackTrace, LogLevel.Debug);
				throw;
			}
		}
	}

	public class AuthEngineSender : ReqSender<AuthEngineRequest, AuthEngineResponse>
	{
		public AuthEngineSender(SystemConfiguration sc) 
			: base(sc.AuthEngineServerAddress, 
			sc.AuthEngineServerPort, 
			sc.AuthEngineUseEncryption, 
			sc.getAuthEnginePublicKey())
		{
		
		}
		public UserProvidersRet GetProviders(UserProviders cmd)
		{
			string returndata = "";
			UserProvidersRet defRet = new UserProvidersRet { Error = "Error getting user providers" };
			try
			{
				AuthEngineRequest req = new AuthEngineRequest();
				req.Commands.Add(cmd);
				returndata = SendData(req);
				Logger.Instance.WriteToLog(this.Name + "GetProviders Server Returned: " + returndata, LogLevel.Debug);

				return (UserProvidersRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + "GetProviders Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + "GetProviders Trace: " + ex.StackTrace, LogLevel.Error);
			}
			return defRet;
		}

		public UserProvidersRet GetProviders(string username, string org)
		{
			var up = new UserProviders();
			up.User = username;
			up.Org = org;
			return GetProviders(up);
		}

		public PermissionsRet validatePermissions(string username, string org)
		{
			PermissionsRet defRet = new PermissionsRet { Error = "Error validating user permissions" };
			try
			{
				AuthEngineRequest req = new AuthEngineRequest();
				Permissions perms = new Permissions();
				perms.User = username;
				perms.Org = org;
				req.Commands.Add(perms);
				string returndata = SendData(req);

				Logger.Instance.WriteToLog(this.Name + "validatePermissions.Server Returned: " + returndata, LogLevel.Debug);

				return (PermissionsRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
			}
			catch (SocketException ex)
			{
				Logger.Instance.WriteToLog(this.Name + "validatePermissions Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + "validatePermissions StackTrace: " + ex.StackTrace, LogLevel.Error);
				defRet.Error = "Error contacting AuthEngine.";
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + "validatePermissions Error: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + "validatePermissions StackTrace: " + ex.StackTrace, LogLevel.Error);
			}
			return defRet;
		}


		protected override string Name
		{
			get { return "AuthEngineSender"; }
		}
	}

	public class CloudSMSSender : ReqSender<CloudSmsRequest, CloudSmsResponse>
	{
		public CloudSMSSender(SystemConfiguration sc)
			: base(sc.getSMSServerAddress(), 
			sc.CloudSMSServerPort, 
			sc.CloudSMSUseEncryption, 
			sc.getCloudSMSPublicKey())
		{

		}

		protected override string Name
		{
			get { return "CloudSMSSender"; }
		}
	}
}
