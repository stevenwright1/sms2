using System;
using System.Collections.Generic;
using System.Text;

using AuthGateway.Shared.Tracking;

namespace AuthGateway.OATH
{

	public class HotpTotp
	{
		public static string HOTP(string K, long C, int digits = 6)
		{
			return HOTP(Encoding.UTF8.GetBytes(K), C, digits);
		}

		public static string HOTP(byte[] K, long C, int digits = 6)
		{
			System.Security.Cryptography.HMACSHA1 hmac_sha1 = new System.Security.Cryptography.HMACSHA1(K);
			byte[] hash = hmac_sha1.ComputeHash(LongToByteArray(C));
			long hashTruncated = Truncate(hash);
			string result = hashTruncated.ToString();
			if (result.Length < digits) {
				result = new string('0', digits) + result;
			}
			return result.Substring(result.Length - digits);
		}

		public static string TOTP(string K, int digits = 6, int window = 30)
		{
			return TOTP(Encoding.UTF8.GetBytes(K), DateTime.UtcNow, digits, window);
		}

		public static string TOTP(string K, System.DateTime dt, int digits = 6, int window = 30)
		{
			return TOTP(Encoding.UTF8.GetBytes(K), dt, digits, window);
		}

		public static string TOTP(byte[] K, int digits = 6, int window = 30)
		{
			return TOTP(K, DateTime.UtcNow, digits, window);
		}

		public static string TOTP(byte[] K, System.DateTime dt, int digits = 6, int window = 30)
		{
			long C = Convert.ToInt64((((dt - new DateTime(1970, 1, 1)).TotalSeconds) / window));
			return HOTP(K, C, digits);
		}

		public static byte[] KeyFromHexString(string Key)
		{
			byte[] realKey = new byte[Convert.ToInt32((Key.Length / 2))];
			int z = 0;
			for (z = 0; z <= realKey.Length - 1; z++) {
				try {
					realKey[z] = Convert.ToByte(Key.Substring((z * 2), 2), 0x10);
				} catch (Exception e) {                    
					throw new Exception("Could not parse key as hexadecimal", e);
				}
			}
			return realKey;
		}

		private static byte[] LongToByteArray(long longnum)
		{
			List<byte> ret = new List<byte>();
			for (int I = 0; I <= 7; I++) {
				ret.Insert(0, (byte)(longnum & 0xff));
				longnum >>= 8;
			}
			return ret.ToArray();
		}

		private static long Truncate(byte[] hash)
		{
			int offset = hash[hash.Length - 1] & 0xf;
			return ((hash[offset] & 0x7f) << 24) | ((hash[offset + 1] & 0xff) << 16) | ((hash[offset + 2] & 0xff) << 8) | ((hash[offset + 3] & 0xff));
		}
	}
}
