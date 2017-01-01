using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Principal;
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



namespace AuthGateway.AuthEngine
{
	public class Server
	{
		private TcpListener listener;
		private bool listening;
		private object lockobj = new object();

		//ad 2: set interval to 1 minute (= 60,000 milliseconds)
		private Double _getNewUsersTimerInterval = 60 * 1000 * 20;
		private System.Timers.Timer getNewUsersTimer;
		private string licp = string.Empty;
		private Double _stampInterval = 60 * 1000 * 5; // 5 Minutes
        private Double _reportingInterval = 60 * 1000 * 60 * 24; // 1 day
		private System.Timers.Timer stampTimer;
		private System.Timers.Timer checkServersTimer;        
        private System.Timers.Timer heartbeatTimer;
        private System.Timers.Timer reportingTimer;

		private ServerLogic serverLogic;

		private SystemConfiguration sc;

		private string AuthEnginePrivKey = @"<RSAKeyValue><Modulus>0704aTJ5fG5jYRPui9ml7OIx6s2cE6QkQTaXZsDd9/BplBwVMxEUjw2HIl1D7rYdbXpSlWcSKsSWcTOsOtD3QdmD3cHAFW36pKU7q7HV8QDf2y7Sys4ATp9O4v/mTyaJ1O36xdwW/+VHAal82QDBXdDdRMKDqgKfTwcciQCZpU4mnjX4Ejme8tXys5jcWuyl4eDjebSlCWtIZktJKLwfmEv0nsntJggXObthmHnr/rW8Se2p7D7qEUyAnL+eM6DlNtx0gMeczILRx3qb4HrVzGgjYcj3RKhisG9aSIijCN8UoATsxsKhXIz0nUPjYV6nI4jUVzmctgun1RqsBnpoOw==</Modulus><Exponent>AQAB</Exponent><P>8WqmvmsrDu1cRQfaTjKmTnH7p6uu10S7kg1GDnqP/eMr7Ga/0NcZWSWFWDUsabub5a5HITEK31X5yKWTA4JQigTxOjtiCTVPxqG2k/HIq6zx/ZehQOrh3miOiaC1uWxRH2nvEbrNOVLLmeARQwA0oO0mIwTZB9hn6eicvs0nbEs=</P><Q>4IegZ+l11szKaDlC8ZeCVW6amqrnTLI9ItgXuJSEVFLeUjQ4uukd0WjQGA8BtA66O6SnYIwkUw8gqynIMSGSiowHi/1j7anDiiuI3+n97nbiMEuP7345Zo/Ru6fviZZFbG5BDon2UuHcLK1vQYso4jTUqsma6P3OZJKDQOUDndE=</Q><DP>gCiVAktUHWWGcSL9EjwzKzu5U8aBV8gmJx+izDbmT+qUO7hEJfK6gye3BR+dRzgQR7rgCc/GLM+wfYLga6F3bf23rakunyLNCe55RUq6s+Boyq3/Lb5DT9WDra5CKoBFBH8xKeFX5xF8AmD/6OioB7I9Z6PALzkD2RnidCvmvMc=</DP><DQ>lGryrF2VaHScw0I+ryYgoEppZh3coUUcxoCjRX7e7kKM9TfR3DOmYztuesjIrnYhwrU3csmQZVsVlC8dRuTTUkP35SCNQpe6SwY2BxVMpqntIFGChqQgW1xsHiiHXaJ4p5FF7c9ihS0Jdnr6lQ/g6Y5UmcSEYskK+k4i9u2rc1E=</DQ><InverseQ>z4S4ki9WkDCgSBXLEErI9w620tqv0pJ4V0eruV8mOq1MYSk8Bv0nDjrKUUQNF4uiFRG1dOlFvUlhuL/ZWLRZ+GXvM3nj3joWzgHTNq/Pi25TDB/yvOUaBrblq5gokej3axw1SjxA3UF30gzlWCiJJzli/pBkx3IPSXS9pYeNeXc=</InverseQ><D>fg0Mkxu4VQpGYVmDToAwljgGbXkf7FVwO95q/YHd4qedws2BFViau5rbEusg8PA7zpvepBCrMQi9YwDXDGCwgeQi65ZXaqqBZxjy0ADbk1Do80wJszA14JhYVyzuh7oyna9a9gVTL93niqbCq1EWzGn1/+Qoi8Jp4psiMrFHxq8NTRmshUlxIry9BC1whIIbi3dJPGYBguDwzPUX2Xwa+d6So+03JWzeKB9VLtC+HlG4KEi4PGQH/RwCoReg/DCNEOTdMQLCK/qQdudWqI4/YdlLcaSbfsKgqZ1Y7AkJdlQiEYyufslDZ++4nyu4voAJVl+dpji/XE5flgB0LPDe4Q==</D></RSAKeyValue>";
		private string CloudSMSPublicKey = @"<RSAKeyValue><Modulus>6aAfvgvCoJHA/nopdwSoenUg8Bsi15ZLny2iLaTGlhvlZp/O9GDJYEWKsxSM8EjMScncjWQ/sJKBXr0MJdWJHAGydpxI90hj2wSu2lG1/vvxBlXj5l7nrPC0Wl3KHU0379wzZ3rznYZU9MuYNIwhkwjZtJ5DerEjmhSQGngQpFJggFW+hLI2yZDHs40T/fHpWxchgjCPMbx+ZeHu+XGGeqgzrFU61qXkKRdAzRVbwGaC/padyHUw15R+qgIpWASGzYAEzzfCQiSlwk30exDp5r+HTAlNSj8truCy+HsLnXx/iBFVUpY4ukA2mS34Ccjn3e6bVYLtjm4+nUyZHpzffw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

		private RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();

		public ServerLogic getServerLogic()
		{
			return serverLogic;
		}

		public bool Listening
		{
			get
			{
				return listening;
			}
		}

		public bool GettingUsers
		{
			get
			{
				return serverLogic.GettingUsers;
			}
		}        

		public void StartService(object data)
		{
			try {                
                Tracker.Instance.TrackEvent("Start AuthEngine", Tracker.Instance.DefaultEventCategory);                
                
				object[] d = (object[])data;
				Logger.Instance.WriteToLog("Server.StartService", LogLevel.Info);
				sc = (SystemConfiguration)d[0];
				string path = d[1].ToString();

				var registry = (Registry)d[2];

				var loader = new LicenseXmlLoader();
				licp = path + @"\Settings\lice";
				Logger.Instance.WriteToLog("Loading license from: " + path + @"\Settings\license.xml", LogLevel.Debug);
				var lic = loader.LoadFrom(path + @"\Settings\license.xml");
				if (DateTime.UtcNow >= lic.AuthEngineEndDate)
				{
					Logger.Instance.WriteToLog("License invalid/expired.", LogLevel.Error);
					Logger.Instance.Flush();
					throw new Exception("License Expired");
					//return;
				}                

				Logger.Instance.WriteToLog("Service queue start", LogLevel.Debug);
				lock (lockobj)
				{
					if (!listening)
					{
						if (string.IsNullOrEmpty(sc.getAuthEnginePrivateKey()))
							sc.setAuthEnginePrivateKey(AuthEnginePrivKey);
						if (string.IsNullOrEmpty(sc.getCloudSMSPublicKey()))
							sc.setCloudSMSPublicKey(CloudSMSPublicKey);

						if (sc.AuthEngineUseEncryption)
							rsaProvider.FromXmlString(sc.getAuthEnginePrivateKey());

						Logger.Instance.WriteToLog("Initialize ServerLogic", LogLevel.Debug);
						serverLogic = new ServerLogic(sc, registry);
						serverLogic.Init();

                        if (!serverLogic.EncryptionExampleValid()) {
                            Logger.Instance.WriteToLog("Failed to check the encryption key.", LogLevel.Error);
                            throw new Exception("Failed to check the encryption key.");
                            
                        }

						Logger.Instance.WriteToLog(string.Format("Listening on Address/Port: {0}:{1}", sc.getAuthEngineServerAddress(), sc.AuthEngineServerPort), LogLevel.Info);

                        reportingTimerElapsed(null, null);
                        heartbeatTimerElapsed(null, null);

                        if (heartbeatTimer == null) {
                            heartbeatTimer = new System.Timers.Timer();
                            heartbeatTimer.Elapsed += new ElapsedEventHandler(heartbeatTimerElapsed);
                            heartbeatTimer.Interval = sc.AuthEngineHeartbeatInterval * 1000;
                            heartbeatTimer.Enabled = true;
                        }
                        else
                            heartbeatTimer.Start();                        

                        if (reportingTimer == null) {
                            reportingTimer = new System.Timers.Timer();
                            reportingTimer.Elapsed += new ElapsedEventHandler(reportingTimerElapsed);
                            reportingTimer.Interval = _reportingInterval;
                            reportingTimer.Enabled = true;
                        }
                        else
                            reportingTimer.Start();

						//tmrElapsed(null, null); // GET USERS

						if (sc.AuthEngineUsersPollInterval > 0)
						{
							new Thread(() =>
							{
								tmrElapsed(null, null);
							}).Start();

							if (getNewUsersTimer == null)
							{
								_getNewUsersTimerInterval = 60 * 1000 * sc.AuthEngineUsersPollInterval;
								getNewUsersTimer = new System.Timers.Timer();
								getNewUsersTimer.Elapsed += new ElapsedEventHandler(tmrElapsed);
								getNewUsersTimer.Enabled = true;
								getNewUsersTimer.Interval = _getNewUsersTimerInterval;
							}
							else
								getNewUsersTimer.Start();
						}

						if (stampTimer == null)
						{
							stampTimer = new System.Timers.Timer();
							stampTimer.Elapsed += new ElapsedEventHandler(stampTimerElapsed);
							stampTimer.Interval = _stampInterval;
							stampTimer.Enabled = true;
						}
						else
							stampTimer.Start();

						if (checkServersTimer == null)
						{
							checkServersTimer = new System.Timers.Timer();
							checkServersTimer.Elapsed += new ElapsedEventHandler(checkServersTimerElapsed);
							checkServersTimer.Interval = _stampInterval;
							checkServersTimer.Enabled = true;
						}
						else
							checkServersTimer.Start();                        

						listener = new TcpListener(sc.getAuthEngineServerAddress(), sc.AuthEngineServerPort);
						listener.Start();
						listening = true;
					}
				}

				do
				{
					TcpClient client = listener.AcceptTcpClient();
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("Client accepted", LogLevel.Debug);
					ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessClient), client);
				}
				while (listening);
			}
			catch (Exception ex)
			{                
				if (ex.Message != null && ex.Message.Contains("WSACancelBlockingCall"))
					return;
				Logger.Instance.WriteToLog(ex, new List<string>() { "AuthEngine", "Server", "StartService" });
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				if (ex.Message == "License Expired")
					throw ex;
				if (sc.StopServiceOnException)
					throw;
			}
			finally
			{
				this.StopService();
			}
		}

		public void StopService()
		{            

			lock (lockobj)
			{
				if (listening)
				{
					Logger.Instance.WriteToLog("Server.StopService", LogLevel.Info);
					if (getNewUsersTimer != null) getNewUsersTimer.Stop();
					if (stampTimer != null) stampTimer.Stop();
					if (checkServersTimer != null) checkServersTimer.Stop();
                    if (heartbeatTimer != null) heartbeatTimer.Stop();
					listener.Stop();
					listening = false;
					if (serverLogic != null)
					{
						serverLogic.Stop();
						serverLogic = null;
					}
                    
					Logger.Instance.WriteToLog("Service queue stopped", LogLevel.Debug);
					Logger.Instance.Flush();
				}                
			}
            
		}

		private WindowsIdentity AuthenticateUser(NetworkStream ns)
		{
			WindowsIdentity identity;
			using (NegotiateStream negstream = new NegotiateStream(ns, true))
			{
				negstream.AuthenticateAsServer();
				if (!negstream.IsAuthenticated || !negstream.RemoteIdentity.IsAuthenticated)
					throw new Exception("IsAuthenticated is not true.");
				if (!(negstream.RemoteIdentity is WindowsIdentity))
					throw new Exception("Cannot get user WindowsIdentity: " + negstream.RemoteIdentity.Name);
				identity = negstream.RemoteIdentity as WindowsIdentity;
				Logger.Instance.WriteToLog("User authenticated: " + identity.Name, LogLevel.Debug);
			}
			return identity;
		}

		public void ProcessClient(object c)
		{
			Logger.Instance.WriteToLog("Server.ProcessClient", LogLevel.Debug);
			TcpClient client = (TcpClient)c;

			int recv = 0;
			byte[] data = new byte[1024];
			byte[] final = new byte[1024];
			NetworkStream ns = client.GetStream();

			MemoryStream all = new MemoryStream();

			WindowsIdentity identity = null;
			try
			{
				Logger.Instance.WriteToLog("Read command", LogLevel.Debug);
				do
				{
					data = new byte[1024];
					recv = ns.Read(data, 0, data.Length);
					all.Write(data, 0, recv);
				} while (ns.DataAvailable);

				bool encrypted = false;

				RijndaelManaged aes = null;
				ICryptoTransform encryptor = null;
				AuthEngineRequest exchangeKeyReq;
				byte[] key;
				byte[] iv;

				all.Position = 0;

				string rawreq = string.Empty;
				var firstByte = all.ReadByte();
				var secondByte = all.ReadByte();
				if (firstByte != '<' || (secondByte != 'A' && secondByte != '?')) // ENCRYPTED
				{
					all.Position = 0;
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("Encrypted connection", LogLevel.Debug);
					if (!sc.AuthEngineUseEncryption)
						throw new ArgumentException("Unexpected input.");
					if (rsaProvider == null)
						throw new Exception("Crypto not initalized.");

					// Get AuthEngineRequest exchange key
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("Decrypt handshake", LogLevel.Debug);
					byte[] decrypted = rsaProvider.Decrypt(all.ToArray(), false);
					string handshake = Encoding.UTF8.GetString(decrypted);
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("Handshake: " + handshake, LogLevel.Debug);
					exchangeKeyReq = Generic.Deserialize<AuthEngineRequest>(handshake);
					key = Convert.FromBase64String(exchangeKeyReq.CKey);
					iv = Convert.FromBase64String(exchangeKeyReq.CIV);
					aes = new RijndaelManaged();
					aes.Padding = PaddingMode.PKCS7;
					aes.Mode = CipherMode.CBC;
					aes.KeySize = 256;
					aes.Key = key;
					aes.IV = iv;
					encrypted = true;

					// Send AutheEngineReponse exchange key OK
					AuthEngineResponse exchangeKeyRet = new AuthEngineResponse();
					exchangeKeyRet.CHK = "OK";
					string respond = Generic.Serialize<AuthEngineResponse>(exchangeKeyRet);

					if (Logger.I.ShouldLog(LogLevel.Debug))
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

					if (!string.IsNullOrEmpty(exchangeKeyReq.auth))
					{
						identity = AuthenticateUser(ns);
					}

					all = new MemoryStream();

					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog("Read command and decrypt", LogLevel.Debug);
					// Get the new encrypted request
					do
					{
						data = new byte[1024];
						recv = ns.Read(data, 0, data.Length);
						all.Write(data, 0, recv);
					} while (ns.DataAvailable);
					all.Position = 0;
					try
					{
						encryptor = aes.CreateDecryptor(aes.Key, aes.IV);
						using (MemoryStream ds = new MemoryStream())
						{
							using (CryptoStream cscrypt = new CryptoStream(all, encryptor, CryptoStreamMode.Read))
							{
								using (StreamReader srDecrypt = new StreamReader(cscrypt))
								{
									rawreq = srDecrypt.ReadToEnd();
								}
							}
						}
					}
							catch
#if DEBUG
 (Exception ex)
#endif
							{
#if DEBUG
								Logger.Instance.WriteToLog("AuthEngine.Server.ProcessClient FAILED TO DECRYPT ERROR: " + ex.Message, LogLevel.Error);
								Logger.Instance.WriteToLog("AuthEngine.Server.ProcessClient FAILED TO DECRYPT STACK: " + ex.StackTrace, LogLevel.Error);
								all.Position = 0;
								var encryptedData = new StreamReader(all).ReadToEnd();
								Logger.Instance.WriteToLog("AuthEngine.Server.ProcessClient FAILED TO DECRYPT: " + encryptedData, LogLevel.Debug);
#endif
								throw;
							}
				}
				else
				{
					rawreq = Encoding.UTF8.GetString(all.ToArray());
#if DEBUG
					Logger.Instance.WriteToLog("Raw request: " + rawreq, LogLevel.Debug);
#endif
					AuthEngineRequest checkReq = Generic.Deserialize<AuthEngineRequest>(rawreq);
					if (!string.IsNullOrEmpty(checkReq.auth))
					{
						AuthEngineResponse checkRet = new AuthEngineResponse();
						checkRet.CHK = "OK";
						final = Encoding.UTF8.GetBytes(Generic.Serialize<AuthEngineResponse>(checkRet));
						ns.Write(final, 0, final.Length);

						identity = AuthenticateUser(ns);

						all = new MemoryStream();
						if (Logger.I.ShouldLog(LogLevel.Debug))
							Logger.Instance.WriteToLog("Read command after Auth.OK", LogLevel.Debug);
						do
						{
							data = new byte[1024];
							recv = ns.Read(data, 0, data.Length);
							all.Write(data, 0, recv);
						} while (ns.DataAvailable);
						all.Position = 0;
						rawreq = Encoding.UTF8.GetString(all.ToArray());
					}
				}

                if (rawreq.Contains(string.Format("<Level>{0}</Level>", LogLevel.DebugVerbose))) {                    
                    Logger.Instance.WriteToLog("Client Request: " + rawreq, LogLevel.DebugVerbose);
                } else
                    if (Logger.I.ShouldLog(LogLevel.Debug)) {
                        Logger.Instance.WriteToLog("Client Request: " + rawreq, LogLevel.Debug);                        
                    }

				AuthEngineRequest req = Generic.Deserialize<AuthEngineRequest>(rawreq);

				AuthEngineResponse ret = new AuthEngineResponse();
				// We will handle one command per XML for now
				bool authenticated = (identity != null);
				foreach (CommandBase cmd in req.Commands)
				{
					cmd.Authenticated = authenticated;
					if (authenticated) cmd.Identity = identity;
					cmd.External = true;
					ret.Responses.Add(serverLogic.Actioner.Do(cmd));
				}

				if (recv != 0)
				{
					string respond = Generic.Serialize<AuthEngineResponse>(ret);
                    if (respond.Contains(string.Format("<Level>{0}</Level>", LogLevel.DebugVerbose))) {
                        Logger.Instance.WriteToLog("Server Response: " + respond, LogLevel.DebugVerbose);
                    }
                    else                
					if (Logger.I.ShouldLog(LogLevel.Debug))
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
				Logger.Instance.WriteToLog("Process Client Stack: " + ex.StackTrace, LogLevel.Error);
				Logger.Instance.Flush();
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			finally
			{
				try
				{
					ns.Close();
					client.Close();
				}
				catch
				{
					// Ignore close warnings
				}
			}
		}

		private void tmrElapsed(object sender, ElapsedEventArgs e)
		{
			var loader = new LicenseDocLoader();
			var lic = loader.LoadFrom(licp + "nse.xml");
			if (DateTime.UtcNow >= lic.AuthEngineEndDate)
			{
				Logger.Instance.WriteToLog("License invalid/expired.", LogLevel.Error);
				StopService();
				throw new Exception("License Expired");
				//return;
			}
			Logger.Instance.WriteToLog("Timer: Get New Users", LogLevel.Info);
			this.getUsers();
		}

		public bool getUsers()
		{
			if (serverLogic == null)
			{
				Logger.Instance.WriteToLog("Server.getUsers serverLogic null", LogLevel.Debug);
				return false;
			}
			return serverLogic.getUsers();
		}

		private void stampTimerElapsed(object sender, ElapsedEventArgs e)
		{
			Logger.Instance.WriteToLog("-- STAMP --", LogLevel.Info);
			Logger.Instance.Flush();
		}

		private void checkServersTimerElapsed(object sender, ElapsedEventArgs e)
		{
			serverLogic.CheckServers();
		}

        private void heartbeatTimerElapsed(object sender, ElapsedEventArgs e)
        {
            serverLogic.Heartbeat();
        }

        private void reportingTimerElapsed(object sender, ElapsedEventArgs e)
        {                        
            serverLogic.Report(e == null);
        }
	}
}
