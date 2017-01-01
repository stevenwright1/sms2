using System;
using System.Security.Cryptography;
using System.Text;
using SMS2.PremiumAccess.Helper;

namespace SMS2.PremiumAccess.Encryption
{
	public class CryptoHelper
	{
		public static byte[] GetSHA1(byte[] data) {
			SHA1 sha = new SHA1CryptoServiceProvider();
			return sha.ComputeHash(data);
		}

		public static string GetSHA1String(string dataString) {
			return HexConversion.ToString(
				GetSHA1(Encoding.ASCII.GetBytes(dataString))
			);
		}

		public static string DecryptSettingIfNecessary(string p, string moresalt)
		{
			if (string.IsNullOrEmpty(p))
				return p;
			if (!p.StartsWith("encrypted:"))
				return p;
			p = p.Replace("encrypted:", "");
			var bytesToDecrypt = Convert.FromBase64String(p);
			var salt = Encoding.UTF8.GetBytes("rndslt" + moresalt);
			return Encoding.UTF8.GetString(ProtectedData.Unprotect(bytesToDecrypt, salt, DataProtectionScope.LocalMachine));
		}

		public static string EncryptSettingIfNecessary(string p, string moresalt)
		{
			if (string.IsNullOrEmpty(p))
				return p;
			if (p.StartsWith("encrypted:"))
				return p;
			var salt = Encoding.UTF8.GetBytes("rndslt" + moresalt);
			var bytesToEncrypt = Encoding.UTF8.GetBytes(p);
			return "encrypted:" + Convert.ToBase64String(ProtectedData.Protect(bytesToEncrypt, salt, DataProtectionScope.LocalMachine));
		}
	}
}
