using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AuthGateway.Shared
{
    public class AESEncryption
    {
        private AesCryptoServiceProvider aesProvider;

        public static string EncryptionMark = "AESencrypted:";

        public AESEncryption(string key)
        {
            if (!string.IsNullOrEmpty(key)) {
                aesProvider = new AesCryptoServiceProvider();
                aesProvider.Key = Convert.FromBase64String(key);
                aesProvider.IV = Encoding.ASCII.GetBytes("2SWGBccssms2swgb");
            }
        }

        public static bool IsEncrypted(string input)
        {
            return input.StartsWith(EncryptionMark);
        }

        public string Encrypt(string input)
        {
            if (aesProvider == null)
                return input;

            string result = string.Empty;
            
            ICryptoTransform encryptor = aesProvider.CreateEncryptor();
            using (MemoryStream msEncrypt = new MemoryStream()) {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                        swEncrypt.Write(input/*"AuthEngine"*/);
                    }
                    byte[] encrypted = msEncrypt.ToArray();
                    result = Convert.ToBase64String(encrypted);
                    AuthGateway.Shared.Log.Logger.Instance.WriteToLog(result, AuthGateway.Shared.Log.LogLevel.Debug);
                }
            }            

            return EncryptionMark + result;
        }

        public string Decrypt(string input)
        {
            if (aesProvider == null)
                return input;

            if (!IsEncrypted(input)) {
                throw new Exception("Trying to decrypt a string that wasn't encrypted with this encryptor.");
            }

            input = input.Substring(EncryptionMark.Length);

            string result = string.Empty;

            ICryptoTransform decryptor = aesProvider.CreateDecryptor();
            
            byte[] cipherText = Convert.FromBase64String(input);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText)) {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                        result = srDecrypt.ReadToEnd();                        
                    }
                }
            }

            return result;
        }
    }
}
