using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;

using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Serializer;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages.Request;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;

namespace AuthGateway.MutualAuthImages
{
    class AuthEngineProxy
    {
        private SystemConfiguration SC = null;

        private RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();

        public AuthEngineProxy(SystemConfiguration sysConf)
		{
			Load(sysConf);
		}

        private void Load(SystemConfiguration sysConf)
        {
            SC = sysConf;
            LoadRsaProvider(SC);
        }
        private void LoadRsaProvider(SystemConfiguration sysConf)
        {
            if ((sysConf.AuthEngineUseEncryption)) {
                rsaProvider.FromXmlString(sysConf.getAuthEnginePublicKey());
            }
        }
        private string SendData(AuthEngineRequest req)
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
                    }
                    else {
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
                    }
                    else {
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
                        }
                        catch (Exception ex) {
#if DEBUG
							returndata.Position = 0;
							StreamReader sr = new StreamReader(returndata);
							Logger.Instance.WriteToLog("SendData.FAILED TO DECRYPT ERROR: " + ex.Message, LogLevel.Error);
							Logger.Instance.WriteToLog("SendData.FAILED TO DECRYPT STACK: " + ex.Message, LogLevel.Error);
							Logger.Instance.WriteToLog("SendData.FAILED TO DECRYPT: " + sr.ReadToEnd(), LogLevel.Debug);
#endif
                            throw;
                        }
                    }
                    else {
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
            }
            finally {
                tcpClient.Close();
            }
        }
        public GetImageCategoriesRet GetImageCategories()
        {
            try {
                AuthEngineRequest req = new AuthEngineRequest();
                GetImageCategories cmd = new GetImageCategories();                
                req.Commands.Add(cmd);
                string returndata = SendData(req);

                GetImageCategoriesRet ret = (GetImageCategoriesRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                return ret;
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("GetImageCategories ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("GetImageCategories TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public StoreImageRet StoreImage(string url, string category, byte[] imageBytes)
        {
            try {
                AuthEngineRequest req = new AuthEngineRequest();
                StoreImage cmd = new StoreImage();
                cmd.Url = url;
                cmd.Category = category;
                cmd.ImageBytes = imageBytes;
                req.Commands.Add(cmd);
                string returndata = SendData(req);
                Logger.Instance.WriteToLog("StoreImage Returned: " + returndata, LogLevel.Debug);

                StoreImageRet ret = (StoreImageRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                return ret;
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("StoreImage ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("StoreImage TRACE: " + ex.StackTrace, LogLevel.Error);                
                throw ex;
            }
        }

        public GetImagesPollingMasterStatusRet GetImagesPollingMasterStatus()
        {
            try {
                AuthEngineRequest req = new AuthEngineRequest();
                GetImagesPollingMasterStatus cmd = new GetImagesPollingMasterStatus();                
                req.Commands.Add(cmd);
                string returndata = SendData(req);
                Logger.Instance.WriteToLog("GetImagesPollingMasterStatus Returned: " + returndata, LogLevel.Debug);

                GetImagesPollingMasterStatusRet ret = (GetImagesPollingMasterStatusRet)Generic.Deserialize<AuthEngineResponse>(returndata).Responses[0];
                return ret;
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("GetImagesPollingMasterStatus ERROR: " + ex.Message, LogLevel.Error);
                Logger.Instance.WriteToLog("GetImagesPollingMasterStatus TRACE: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

    }
}
