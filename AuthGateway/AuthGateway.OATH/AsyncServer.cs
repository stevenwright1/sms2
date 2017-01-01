using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using AuthGateway.OATH.XmlProcessor;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Serializer;
using AuthGateway.Shared.Tracking;

namespace AuthGateway.OATH
{

	public class AsyncServer
	{
// Thread signal.

		public ManualResetEvent allDone = new ManualResetEvent(false);
		private Socket listener = null;

		private bool keepRunning = false;

		private SystemConfiguration sc;

		private string rsaPrivateKey = "<RSAKeyValue><Modulus>2Z7mqSaJQCzdQwIyEDvYetGaZjC9+mRVyL/cSy2iD9RyL0QHVjuilY8goDNc9Vbqxg/w0dvkq2B3XpsXsd47ma+REN/nMNI+owmPs+RgP6bBy8aCOdViEHyUs3G2Jowz5zOmeBavnE6Hel9ANUyjdVlRlaE9B9s27xXXR+/YPOuAccD3/J0Vte9tjfQ6aOvw1VLs3REMeLI/eOk2XZswynszNq65UlzTQ7WGoOuhskuj4QT+B1pjO5qvf3bjGo6FbsaUenmRMqVBtqwv6Sy7moX1XipNveO9n9SpZy5F5B1jP0PAnxA4x/igZDB1zTdbPygwXR/3M76kc+xLGaa1xQ==</Modulus><Exponent>AQAB</Exponent><P>/LoXVWTzoAZsJWnzVk78zUOmorby3l3BG7w3Av5IW6fqglohca/frOfodRgfjmrHx5DUXtHskyrdWcDKVmEGuBXtsHDWikVpfo49lk3D8JgeKZLNj5J7dK0lwLlFdQpmX3VTbxdN+6zUMW8VynqurcIfnZQp5feDsoe33+36f3s=</P><Q>3HBqjlKXwB4powmnLc/DAR1D+4E4KKyHSV/b9Q+7H5UyrVegwTgeqwQoNn0Rp1K2KkaTOf8L2DXhYEtjU8XPiMxVJWieqL6KJCW3nCXiLUnF/279YqQMHKxG0iQhH+RsKV0Bw1qp1rif12cP4VtvH3Cuns8usETgVQS8zLUM+78=</Q><DP>a8svwqORajLzE64yNSDxoNd3DrX7ty7D/AF2cVdmI3nmg3zQAP6j58rukmscopEW7x7uBheB0W+aA/tAEkHGLORlgjlOuKFMksc2q5I1vbwUOWU9OjyfXa+wh5g+cOJjsdmIIb0N7QfVZdRctgVH1iMTexHEIStGR/KtUdzeWFs=</DP><DQ>rqTZptxEoc7DygmRy7e4lR9shsu/hGn73OP3TdYiuEjqF28/SxV9Jpxqh5Da9aeP7zpu1hn8dlVps3LGxM4JOCY6pyKV4LbklvLS3wEciijSlyaF1SqG9gh/K2m4XJ4KG1M2XGFuAVHQQUXDzRU1msEZd3RJVxmaaYERW1Vtbd0=</DQ><InverseQ>g/EBvNBBOJqD9KPnmjHhmAvQ1MI3XUksPrhJhTEen3icWv5moQWUBXjTphHLtmEiZr6cy7Dnn+2bHKmdEpyiXHIjcX0RZfgC/MweZXA3WzgGICP0CcrrpxQR9jQI0EG6PpZXGxaRkUtyLbu9Tsy+ziY3D4f3qTjQ8cmHpAEq0gY=</InverseQ><D>Fl4OtzQOx1nVJB4Fp0V/aqBAjmBUJnhJmOifa9q03YuocNM9Lc/TVivv3odo9o6IcvMQfYFsb3Dq0/584PtGhPVWo7VfvdTeO0OwUX0Xp3MsWBV/LRKRkcA8SHVXk361lv3oDk/GnPo1Uo6XuuK1qkoT6J05+KS/cLN9/RbZ0ujxGJGTM8YuM1JTdfHimp0OrYhLDQfXZxHaL9CVdvjQ4EbZdcAZ0LUFWGzJVttCXVcHBZX34S8Qmp4hDEhqYiJ+aTLhVQtt4r6+6TiFtJ6xd1ip+JSUg8Ulh9cK+f5cYdnfuXKLincHjvix2xegypUF1J8qRHIJtWLK3b+dufODwQ==</D></RSAKeyValue>";

		private RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
		public void StartServer(SystemConfiguration sc, int maxconnections)
		{                        
			this.sc = sc;

			if ((string.IsNullOrEmpty(sc.OATHCalcPrivateKey))) {
				sc.OATHCalcPrivateKey = rsaPrivateKey;
			}

			if ((sc.OATHCalcUseEncryption)) {
				rsaProvider.FromXmlString(sc.OATHCalcPrivateKey);
			}

			listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listener.Bind(new IPEndPoint(sc.OATHCalcServerIp, sc.OATHCalcServerPort));
			listener.Listen(maxconnections);
			keepRunning = true;
			Logger.Instance.WriteToLog("AsyncServer - Listen start", LogLevel.Info);
			while (keepRunning)
			{
				allDone.Reset();
				try
				{
					listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
				}
				catch (Exception ex)
				{
					if ((ex.Message != null && ex.Message.Contains("WSACancelBlockingCall")))
					{
						return;
					}
					Logger.Instance.WriteToLog("OATHCalc loop ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
					Logger.Instance.WriteToLog("OATHCalc loop Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Error);
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				}
				allDone.WaitOne();
			}
			Logger.Instance.WriteToLog("AsyncServer - Listen Stop", LogLevel.Info);
		}
		//Main

		public void StopServer()
		{
			keepRunning = false;
			allDone.Set();
			try {
				if (listener != null)
					listener.Dispose();
			} catch {
			}            
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			allDone.Set();
			try {
				Socket listener = (Socket)ar.AsyncState;
				Socket handler = listener.EndAccept(ar);

				StateObject state = new StateObject();
				state.workSocket = handler;
				state.ms = new MemoryStream();
				state.checkTimer = new Timer(CheckTimeout, state, 5000, System.Threading.Timeout.Infinite);
				handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
			} catch (Exception ex) {
				if ((ex.Message != null && (ex.Message.Contains("WSACancelBlockingCall") || ex.Message.Contains("Object name: 'System.Net.Sockets.Socket'")))) {
					return;
				}
				Logger.Instance.WriteToLog("OATHCalc AcceptCallback ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
				Logger.Instance.WriteToLog("OATHCalc AcceptCallback Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}
		//AcceptCallback

		private void CheckTimeout(object stateInfo)
		{
			StateObject si = (StateObject)stateInfo;
			//If Not si.dataReceived Then
			try {
				if ((si.workSocket != null) && si.workSocket.Connected) {
					FinalizeSocket(si.workSocket);
				}
			} catch (Exception ex) {
				if ((ex.Message != null && ex.Message.Contains("WSACancelBlockingCall"))) {
					return;
				}
				Logger.Instance.WriteToLog("OATHCalc CheckTimeout ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
				Logger.Instance.WriteToLog("OATHCalc CheckTimeout Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			//End If
		}

		private void ReadCallback(IAsyncResult ar)
		{
			string content = string.Empty;

			// Retrieve the state object and the handler socket
			// from the asynchronous state object.
			StateObject state = (StateObject)ar.AsyncState;
			Socket handler = state.workSocket;

			try {
				int bytesRead = handler.EndReceive(ar);
				if (bytesRead > 0) {
					// There  might be more data, so store the data received so far.
					state.ms.Write(state.buffer, 0, bytesRead);
					if (bytesRead < StateObject.BufferSize) {
						state.dataReceived = true;
					} else {
						handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
					}
				}
				try {
					if (state.dataReceived) {
						if ((handler != null)) {
							state.ms.Position = 0;

							tokenquery exchangeKeyReq = null;
							byte[] key = null;
							byte[] iv = null;

							// <
							if (state.ms.ReadByte() != 60 || state.ms.ReadByte() != Convert.ToInt32('t')) {
								state.ms.Position = 0;
								if (Logger.I.ShouldLog(LogLevel.Debug))
									Logger.Instance.WriteToLog("Encrypted connection", LogLevel.Debug);
								if ((!sc.CloudSMSUseEncryption)) {
									throw new ArgumentException("Unexpected input.");
								}
								if ((rsaProvider == null)) {
									throw new Exception("Crypto not initalized.");
								}

								Logger.Instance.WriteToLog("Decrypt handshake", LogLevel.Debug);
								byte[] decrypted = rsaProvider.Decrypt(state.ms.ToArray(), false);
								string handshake = Encoding.UTF8.GetString(decrypted);
								if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
									Logger.Instance.WriteToLog("Handshake: " + handshake, LogLevel.DebugVerbose);
								exchangeKeyReq = Generic.Deserialize<tokenquery>(handshake);
								key = Convert.FromBase64String(exchangeKeyReq.CKey);
								iv = Convert.FromBase64String(exchangeKeyReq.CIV);
								state.aes = new RijndaelManaged();
								state.aes.Padding = PaddingMode.PKCS7;
								state.aes.Mode = CipherMode.CBC;
								state.aes.KeySize = 256;
								state.aes.Key = key;
								state.aes.IV = iv;
								state.encrypted = true;

								tokenresponse exchangeKeyRet = new tokenresponse();
								exchangeKeyRet.CHK = "OK";
								string respond = Generic.Serialize<tokenresponse>(exchangeKeyRet);

								Logger.Instance.WriteToLog("Handshake OK", LogLevel.Debug);

								ICryptoTransform encryptor = state.aes.CreateEncryptor(state.aes.Key, state.aes.IV);
								MemoryStream msEncrypt = new MemoryStream();
								CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
								StreamWriter swEncrypt = new StreamWriter(csEncrypt);
								byte[] handshakeOkEncrypted = null;
								using (msEncrypt) {
									using (csEncrypt) {
										using (swEncrypt) {
											swEncrypt.Write(respond);
										}
									}
									handshakeOkEncrypted = msEncrypt.ToArray();
								}
								SendAndReceiveEncrypted(state, handshakeOkEncrypted);
							} else {
								Send(handler, Processor.Process(state.ms));
							}
						}
						state.dataReceived = false;
					}
				} catch (Exception ex) {
					if ((ex.Message != null && ex.Message.Contains("WSACancelBlockingCall"))) {
						return;
					}
					Logger.Instance.WriteToLog("OATHCalc ReadCallback.state.dataReceived ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
					Logger.Instance.WriteToLog("OATHCalc ReadCallback.state.dataReceived Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Debug);
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
					if (sc.StopServiceOnException)
					throw;
				}
			} catch (SocketException ex) {
				Logger.Instance.WriteToLog("OATHCalc ReadCallback.SocketException ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
				Logger.Instance.WriteToLog("OATHCalc ReadCallback.SocketException Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Debug);
				FinalizeSocket(handler);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				if (sc.StopServiceOnException)
					throw;
			} catch (ObjectDisposedException ex) {
				Logger.Instance.WriteToLog("OATHCalc ReadCallback.ObjectDisposedException ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
				Logger.Instance.WriteToLog("OATHCalc ReadCallback.ObjectDisposedException Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				if (sc.StopServiceOnException)
					throw;
			}
		}
		//ReadCallback

		private void FinalizeSocket(Socket handler)
		{
			try {
				if ((handler != null)) {
					handler.Shutdown(SocketShutdown.Both);
					handler.Close();
				}
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("FinalizeSocket ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("FinalizeSocket Stack: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}

		private void Send(Socket handler, byte[] byteData)
		{
			try {
				handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
			} catch (SocketException ex) {
				Logger.Instance.WriteToLog("OATHCalc Send ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
				Logger.Instance.WriteToLog("OATHCalc Send Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Debug);
				//FinalizeSocket(handler)
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}
		//Send

		private void Send(Socket handler, string data)
		{
			Send(handler, Encoding.UTF8.GetBytes(data));
		}
		//Send

		private void SendCallback(IAsyncResult ar)
		{
			// Retrieve the socket from the state object.
			Socket handler = (Socket)ar.AsyncState;

			try {
				int bytesSent = handler.EndSend(ar);
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("OATHCalc SendCallback ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
				Logger.Instance.WriteToLog("OATHCalc SendCallback Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				if (sc.StopServiceOnException)
					throw;
			} finally {
				//FinalizeSocket(handler)
			}
		}
		//SendCallback

		private void SendAndReceiveEncrypted(StateObject state, byte[] byteData)
		{
			try {
				Socket handler = state.workSocket;
				handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendAndReceiveEncryptedCallback), state);
			} catch (SocketException ex) {
				Logger.Instance.WriteToLog("OATHCalc SendAndReceiveEncrypted ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
				Logger.Instance.WriteToLog("OATHCalc SendAndReceiveEncrypted Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Debug);
				//FinalizeSocket(state.workSocket)
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}
		//Send

		private void SendAndReceiveEncryptedCallback(IAsyncResult ar)
		{
			// Retrieve the socket from the state object.
			StateObject state = (StateObject)ar.AsyncState;
			Socket handler = state.workSocket;
			try {
				int bytesSent = handler.EndSend(ar);
				if (state.encryptedReceive) {
					state.ms = new MemoryStream();
					state.checkTimer = new Timer(CheckTimeout, state, 5000, System.Threading.Timeout.Infinite);
					handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadEncryptedCallback), state);
				}
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("OATHCalc SendAndReceiveEncrypted ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
				Logger.Instance.WriteToLog("OATHCalc SendAndReceiveEncrypted Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				if (sc.StopServiceOnException)
					throw;
			} finally {
				if (!state.encryptedReceive) {
					//FinalizeSocket(handler)
				}
			}
		}
		//SendCallback

		private void ReadEncryptedCallback(IAsyncResult ar)
		{
			string content = string.Empty;

			// Retrieve the state object and the handler socket
			// from the asynchronous state object.
			StateObject state = (StateObject)ar.AsyncState;
			Socket handler = state.workSocket;

			try {
				int bytesRead = handler.EndReceive(ar);
				if (bytesRead > 0) {
					// There  might be more data, so store the data received so far.
					state.ms.Write(state.buffer, 0, bytesRead);
					if (bytesRead < StateObject.BufferSize) {
						state.dataReceived = true;
					} else {
						handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadEncryptedCallback), state);
					}
				}

				if (state.dataReceived) {
					if ((handler != null)) {
						state.ms.Position = 0;

						string response = null;
						using (ICryptoTransform encryptor = state.aes.CreateDecryptor(state.aes.Key, state.aes.IV)) {
							using (CryptoStream cscrypt = new CryptoStream(state.ms, encryptor, CryptoStreamMode.Read)) {
								using (StreamReader srDecrypt = new StreamReader(cscrypt)) {
									response = Processor.Process(Generic.Deserialize<tokenquery>(srDecrypt.ReadToEnd()));
								}
							}
						}
						byte[] byteReponse = null;
						using (ICryptoTransform encryptor = state.aes.CreateEncryptor(state.aes.Key, state.aes.IV)) {
							using (MemoryStream ms = new MemoryStream()) {
								using (CryptoStream cscrypt = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
									using (StreamWriter swEncrypt = new StreamWriter(cscrypt)) {
										swEncrypt.Write(response);
									}
								}
								byteReponse = ms.ToArray();
							}
						}
						state.ms = new MemoryStream();
						state.encryptedReceive = false;
						state.dataReceived = false;
						SendAndReceiveEncrypted(state, byteReponse);
					}
				}
			} catch (SocketException ex) {
				Logger.Instance.WriteToLog("OATHCalc ReadCallback.SocketException ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
				Logger.Instance.WriteToLog("OATHCalc ReadCallback.SocketException Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				if (sc.StopServiceOnException)
					throw;
				//FinalizeSocket(handler)
			} catch (ObjectDisposedException ex) {
				Logger.Instance.WriteToLog("OATHCalc ReadCallback.ObjectDisposedException ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
				Logger.Instance.WriteToLog("OATHCalc ReadCallback.ObjectDisposedException Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				if (sc.StopServiceOnException)
					throw;
			} catch (Exception ex) {
				Logger.Instance.WriteToLog("OATHCalc ReadCallback.Exception ERROR: " + ex.Message, AuthGateway.Shared.Log.LogLevel.Error);
				Logger.Instance.WriteToLog("OATHCalc ReadCallback.Exception Trace: " + ex.StackTrace, AuthGateway.Shared.Log.LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				if (sc.StopServiceOnException)
					throw;
			}
		}
		//ReadCallback

	}

	// State object for reading client data asynchronously

	public class StateObject
	{
// Client  socket.
		public Socket workSocket = null;
// Size of receive buffer.
		public const int BufferSize = 1024;
// Receive buffer.
		public byte[] buffer = new byte[BufferSize + 1];
// Received data string.
		public MemoryStream ms;
		public Timer checkTimer;
		public bool dataReceived = false;
		public bool encrypted = false;
		public bool encryptedReceive = true;
		public RijndaelManaged aes = null;
	}
}
//StateObject
