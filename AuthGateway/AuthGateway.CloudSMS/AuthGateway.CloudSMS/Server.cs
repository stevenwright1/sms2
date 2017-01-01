using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Timers;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Serializer;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request;
using AuthGateway.Shared.XmlMessages.Response;

namespace SMSService
{
	public class Server
	{

		private Double _stampInterval = 60 * 1000 * 5; // 5 Minutes
		private System.Timers.Timer stampTimer;

		private ServerLogic serverManager;

		private TcpListener listener;
		private bool listening;
		private object lockobj = new object();

		private SystemConfiguration sc;

		private string CloudSMSPrivateKey = @"<RSAKeyValue><Modulus>6aAfvgvCoJHA/nopdwSoenUg8Bsi15ZLny2iLaTGlhvlZp/O9GDJYEWKsxSM8EjMScncjWQ/sJKBXr0MJdWJHAGydpxI90hj2wSu2lG1/vvxBlXj5l7nrPC0Wl3KHU0379wzZ3rznYZU9MuYNIwhkwjZtJ5DerEjmhSQGngQpFJggFW+hLI2yZDHs40T/fHpWxchgjCPMbx+ZeHu+XGGeqgzrFU61qXkKRdAzRVbwGaC/padyHUw15R+qgIpWASGzYAEzzfCQiSlwk30exDp5r+HTAlNSj8truCy+HsLnXx/iBFVUpY4ukA2mS34Ccjn3e6bVYLtjm4+nUyZHpzffw==</Modulus><Exponent>AQAB</Exponent><P>9lwyXPeDIFURORpKSIHTYN1vvOK1aoSjRcomI7UwT4045kfxme9kfbDDtXElA7TE3mZpUVsYGbyuNGNwg8hgL+Yb1hT0o2W1FOhih+Cmr/kDgEHnDH0urmjd4Qnw3mN9LGixQdXPZP4ggtzQAt6Jt4X3f7PaS7bivo6wizCkRCc=</P><Q>8sRdDJWfYGQnednowptPAsu+dHsiCWHC+B6D6Oh3f8LBgtNfKpVa/1k/F970tIkcltZaiZ8217PqdBPoh7XavBsUreY6ChC4UGU9j7ULwkMRnrSW8ETJKk3HAYN4rtskdopgFE7W/i/7Q8EvIuagVTjK7uSI0+bljkEE75MdaOk=</Q><DP>l0nrC7hb6CEVYJHKiFhhrJyPn9lBO1aUxajsXwVH07KP/Kq1raibd6xzoxsGdg4uz7zodDOSy0tZV4axc9w7ZW7ULVXVHfq5h0tmJrdI5cvv4HWYI81EO4repvHp30gNeJYaNKnOoOCGqpZbj0eGHxO/98ZFCjYXbJpHxFJsrgU=</DP><DQ>QHgdLcZeOL7gLN/NjECqTtfEkDJS66Lmn/WamjOB6I/Ty+ZOE4TuUXll4/T1jywKR5RNHtcFPWsuC/1tdvy4RdP7PeMx7pJaIB+CpbMymDgvabITk2Lw+ScGfkRnvCe+GyzMLxhwx7f+RhP7bI1KbtdSLPbLz1o2A/0ITocG/GE=</DQ><InverseQ>KO5EyGbrr/V7XCEjIVuIOK9r7jw+uIQoi3kuIBtnSpa985LnmucE7zX/hfbKJYbfFPXr+V2AKEOnwZatEypJ1t0e8OkvQ/Vkhx91NtOG21VZOiPM/rQ+CVIgsU4/RuUZRHAHHN/a6BIAz34I3I5X70qH5hbN+Wh5ghTQQcQUaOU=</InverseQ><D>QGw/VJC5or2Okp3tQTyvmNUjuBJDkV+IiuRyjFObz1jx8VKwJwNphHqovKk2FCx+6PoZL4Qta+t6cT2AVA6GOIrfQ4XlOQtlPcgUz28o2J9w32Zf6e7RxAa8NQ8X5OyeOC5onmhxNUcNtlNOLW9W8szF++CIixS91IdHFYV2UVk/EdIHLB0c/3TKg9m4CQYwv9aUIfHQbP3j2qPGb7k4ITS7XzHzzrHSTee90QEzHZLDMGeGAILbiPJ5Jo8sBBpr7VsfE1lNur9Vrtciyr06En3bN3urQWkqLSmRV3UDe/SuTwgoveoc4QBLQOCwauvDUN1FmQFkMLAR9bduT2+SgQ==</D></RSAKeyValue>";

		private RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();

		public bool Listening { get { return listening; } }

		public ServerLogic getServerLogic()
		{
			return serverManager;
		}

		public void StartService(object data)
		{                        
			Logger.Instance.WriteToLog("Server.StartService", LogLevel.Info);
			var parms = (object[])data;
			sc = (SystemConfiguration)parms[0];
			Logger.Instance.SetFlushOnWrite((sc.CloudSMSFlushOnWrite));
			if (stampTimer == null)
			{
				stampTimer = new System.Timers.Timer();
				stampTimer.Elapsed += new ElapsedEventHandler(stampTimerElapsed);
				stampTimer.Interval = _stampInterval;
				stampTimer.Enabled = true;
			}
			else
				stampTimer.Start();

			try
			{
				Logger.Instance.WriteToLog("Service queue start", LogLevel.Info);
				serverManager = new ServerLogic(sc);
				//serverManager.InitializeClientXMLCredentials();

				NetworkInformation info = NetworkInformation.LocalComputer;

				Logger.Instance.WriteToLog("SMS IP Address: " + sc.getSMSServerAddress(), LogLevel.Info);
				Logger.Instance.WriteToLog("SMS Port: " + sc.CloudSMSServerPort, LogLevel.Info);
				serverManager.Init();

				lock (lockobj)
				{
					if (string.IsNullOrEmpty(sc.getCloudSMSPrivateKey()))
						sc.setCloudSMSPrivateKey(CloudSMSPrivateKey);

					if (sc.CloudSMSUseEncryption)
						rsaProvider.FromXmlString(sc.getCloudSMSPrivateKey());
					listener = new TcpListener(sc.getSMSServerAddress(), sc.CloudSMSServerPort);
#if DEBUG
					Logger.Instance.WriteToLog("CloudSMS.Server.Listening to clients", LogLevel.Debug);
#endif
					listener.Start();
					listening = true;
				}

				do
				{
					TcpClient client = listener.AcceptTcpClient();
					Logger.Instance.WriteToLog("CloudSMS.Server.Client accepted", LogLevel.Debug);
					ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessClient), client);
				}
				while (listening);
			}
			catch (Exception ex)
			{
				if (ex.Message != null && ex.Message.Contains("WSACancelBlockingCall"))
					return;
				Logger.Instance.WriteToLog(ex, new List<string>() { "CloudSMS", "Server", "StartService" });
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			finally
			{
				if (serverManager != null)
				{
					serverManager.Stop();
					serverManager = null;
				}
				if (stampTimer != null)
					stampTimer.Stop();
			}
		}

		public void StopService()
		{
			if (stampTimer != null)
				stampTimer.Stop();

			if (listening)
			{
				lock (lockobj)
				{
					listener.Stop();
					listening = false;
					Logger.Instance.WriteToLog("Service queue stopped", LogLevel.Info);
					Logger.Instance.Flush();
				}
			}            
		}

		public void ProcessClient(object c)
		{
			TcpClient client = (TcpClient)c;

			int recv = 0;
			byte[] data = new byte[1024];
			byte[] final = new byte[1024];
			byte[] Response = new byte[1024];
			NetworkStream ns = client.GetStream();

			MemoryStream all = new MemoryStream();

			NetworkInformation info = NetworkInformation.LocalComputer;
#if DEBUG
			Logger.Instance.WriteToLog("CloudSMS.Server.ProcessClient Client Accepted", LogLevel.Debug);
#endif
			try
			{
				do
				{
					data = new byte[1023];
					recv = ns.Read(data, 0, data.Length);
					all.Write(data, 0, recv);
				} while (ns.DataAvailable);

				bool encrypted = false;

				RijndaelManaged aes = null;
				ICryptoTransform encryptor = null;
				CloudSmsRequest exchangeKeyReq;
				byte[] key;
				byte[] iv;

				all.Position = 0;

				if (all.ReadByte() != '<' || all.ReadByte() != 'C') // ENCRYPTED
				{
					all.Position = 0;
					Logger.Instance.WriteToLog("Encrypted connection", LogLevel.Debug);
					if (!sc.CloudSMSUseEncryption)
						throw new ArgumentException("Unexpected input.");
					if (rsaProvider == null)
						throw new Exception("Crypto not initalized.");

					// Get CloudSMSRequest exchange key
					Logger.Instance.WriteToLog("Decrypt handshake", LogLevel.Debug);
					byte[] decrypted = rsaProvider.Decrypt(all.ToArray(), false);
					string handshake = Encoding.UTF8.GetString(decrypted);
					Logger.Instance.WriteToLog("Handshake: " + handshake, LogLevel.Debug);
					exchangeKeyReq = Generic.Deserialize<CloudSmsRequest>(handshake);
					key = Convert.FromBase64String(exchangeKeyReq.CKey);
					iv = Convert.FromBase64String(exchangeKeyReq.CIV);
					aes = new RijndaelManaged();
					aes.Padding = PaddingMode.PKCS7;
					aes.Mode = CipherMode.CBC;
					aes.KeySize = 256;
					aes.Key = key;
					aes.IV = iv;
					encrypted = true;

					// Send CloudSmsReponse exchange key OK
					CloudSmsResponse exchangeKeyRet = new CloudSmsResponse();
					exchangeKeyRet.CHK = "OK";
					string respond = Generic.Serialize<CloudSmsResponse>(exchangeKeyRet);

					Logger.Instance.WriteToLog("Handshake OK", LogLevel.Debug);

					encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
					using (MemoryStream msEncrypt = new MemoryStream())
					{
						using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
						{
							using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
								swEncrypt.Write(respond);
						}
						final = msEncrypt.ToArray();
					}
					ns.Write(final, 0, final.Length);

					all = new MemoryStream();

					Logger.Instance.WriteToLog("Read command and decrypt", LogLevel.Debug);
					// Get the new encrypted request
					do
					{
						data = new byte[1024];
						recv = ns.Read(data, 0, data.Length);
						all.Write(data, 0, recv);
					} while (ns.DataAvailable);
				}
				all.Position = 0;

				string rawreq = string.Empty;

				if (encrypted)
				{
					encryptor = aes.CreateDecryptor(aes.Key, aes.IV);
					using (CryptoStream cscrypt = new CryptoStream(all, encryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(cscrypt))
						{
							rawreq = srDecrypt.ReadToEnd();
						}
					}
#if DEBUG
					Logger.Instance.WriteToLog("Client Request IS encryted: " + rawreq, LogLevel.Debug);
#endif
				}
				else
				{
					rawreq = Encoding.UTF8.GetString(all.ToArray());
#if DEBUG
					Logger.Instance.WriteToLog("Client Request is NOT encryted: " + rawreq, LogLevel.Debug);
#endif
				}

				Logger.Instance.WriteToLog("Client Request: " + rawreq, LogLevel.Debug);

				CloudSmsRequest req = Generic.Deserialize<CloudSmsRequest>(rawreq);

				string strResponse = string.Empty;

				CloudSmsResponse response = new CloudSmsResponse();

				// We will handle one command per XML for now
				foreach (CommandBase cmd in req.Commands)
				{
					cmd.External = true;
					response.Responses.Add(serverManager.Actioner.Do(cmd));
				}

				if (recv != 0)
				{
					string respond = Generic.Serialize<CloudSmsResponse>(response);
					Logger.Instance.WriteToLog("Server Response: " + respond, LogLevel.Debug);
					if (encrypted)
					{
						encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
						using (MemoryStream msEncrypt = new MemoryStream())
						{
							using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
							{
								using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
									swEncrypt.Write(respond);
							}
							final = msEncrypt.ToArray();
						}
					}
					else
						final = Encoding.UTF8.GetBytes(respond);
					ns.Write(final, 0, final.Length);
				}
			}
			catch (Exception ex)
			{
				if (ex.Message != null && ex.Message.Contains("WSACancelBlockingCall"))
					return;
				Logger.Instance.WriteToLog("Process Client ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("Process Client Trace: " + ex.StackTrace, LogLevel.Error);
				Logger.Instance.Flush();
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			finally
			{
				ns.Close();
				client.Close();
			}
		}

		private void stampTimerElapsed(object sender, ElapsedEventArgs e)
		{
			Logger.Instance.WriteToLog("-- STAMP --", LogLevel.Info);
			Logger.Instance.Flush();
		}
	}
}
