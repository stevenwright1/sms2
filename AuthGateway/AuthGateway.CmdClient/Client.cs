using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using AuthGateway.Shared;
using AuthGateway.Shared.Serializer;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using System.Security.Cryptography;
using AuthGateway.Shared.XmlMessages.Response;
namespace Client
{

	public class CmdClient
	{

		public SystemConfiguration SC = null;

		private RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
		public CmdClient()
		{
			LoadSettings();
		}

		public static void Main()
		{
			SystemConfiguration remoteServer = new SystemConfiguration(Application.StartupPath, "SettingsPublic");
			remoteServer.LoadSettings();
			remoteServer.WriteXMLCredentials();

			//Deleted Code..old code from another programer
			//WriteXML()

			string[] args = Environment.GetCommandLineArgs();

			for (int i = 0; i <= args.Length - 1; i++) {
				Console.WriteLine("Arg: " + i + " is " + args[i]);
			}

			CmdClient cClient = new CmdClient();

			if (args.Length == 2) {
				//args(1) = Environment.UserName
				cClient.SendLoginDetails(args[1]);
			} else if (args.Length == 3) {
				//   args(0) = Environment.UserName
				//  args(1) = "123456"
				cClient.ValidatePin(args[1], args[2]);
			} else {
				Console.WriteLine("Parameters not Found");
				Console.ReadLine();
				return;
			}

			//ValidatePin(Environment.UserName & "|" & 386187)
			Console.Title = "CmdClient";
			Console.BackgroundColor = ConsoleColor.White;
			Console.ForegroundColor = ConsoleColor.Blue;
		}

		private string SendData(AuthEngineRequest req)
		{
			TcpClient tcpClient = new TcpClient();
			MemoryStream returndata = new MemoryStream();

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
				Console.WriteLine(string.Format("Connecting to {0}:{1}", SC.getAuthEngineServerAddress(), SC.AuthEngineServerPort));
				tcpClient.Connect(SC.getAuthEngineServerAddress(), SC.AuthEngineServerPort);
				NetworkStream networkStream = tcpClient.GetStream();

				//Debugger.Break()
				string cmdStr = null;
				byte[] sendBytes = null;

				RijndaelManaged aes = null;

				string retStr = string.Empty;

				if (networkStream.CanWrite & networkStream.CanRead) {
					byte[] bytes = new byte[tcpClient.ReceiveBufferSize + 1];
					if (encrypted) {
						exchangeKeyRequest.Commands = null;
						cmdStr = Generic.Serialize<AuthEngineRequest>(exchangeKeyRequest);
						Console.WriteLine("Client Handshake: " + cmdStr);
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
						returndata = new MemoryStream();
					}

					cmdStr = Generic.Serialize<AuthEngineRequest>(req);
					Console.WriteLine("Client Request: " + cmdStr);
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

					do {
						int bread = networkStream.Read(bytes, 0, Convert.ToInt32(tcpClient.ReceiveBufferSize));
						returndata.Write(bytes, 0, bread);
					} while (networkStream.DataAvailable);

					returndata.Position = 0;
					if (encrypted) {
						ICryptoTransform cryptor = aes.CreateDecryptor(aes.Key, aes.IV);
						CryptoStream cs = new CryptoStream(returndata, cryptor, CryptoStreamMode.Read);
						StreamReader sr = new StreamReader(cs);
						retStr = sr.ReadToEnd();
						sr.Close();
						cs.Close();
					} else {
						StreamReader sr = new StreamReader(returndata);
						retStr = sr.ReadToEnd();
					}

					Console.WriteLine("Server Returned: " + retStr);
				}

				tcpClient.Close();

				return retStr;
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
				return ex.Message;
			}
		}

		public string ValidatePin(string User, string Pin)
		{
			AuthEngineRequest req = new AuthEngineRequest();
			ValidatePin cmd = new ValidatePin();
			cmd.User = User;
			cmd.Pin = Pin;
			req.Commands.Add(cmd);
			return SendData(req);
		}


		private void SendLoginDetails(string Username)
		{
			AuthEngineRequest req = new AuthEngineRequest();
			ValidateUser cmd = new ValidateUser();
			cmd.User = Username;
			req.Commands.Add(cmd);
			SendData(req);
		}

		private IPAddress IPAddress()
		{
			//To get local address
			string LocalHostName = null;

			LocalHostName = Dns.GetHostName();

			IPHostEntry ipEnter = Dns.GetHostEntry(LocalHostName);
			IPAddress[] IpAdd = ipEnter.AddressList;

			return IpAdd[0];
		}

		public void LoadSettings()
		{
			SC = new AuthGateway.Shared.SystemConfiguration(Application.StartupPath);
			if ((SC.AuthEngineUseEncryption)) {
				rsaProvider.FromXmlString(SC.getAuthEnginePublicKey());
			}
		}

		public string TrimAll(string TextIn, string TrimChar = " ", string CtrlChar = "\0")
		{
			return TextIn.TrimEnd(new char[] { '\0' }).Trim();
		}

	}
}
