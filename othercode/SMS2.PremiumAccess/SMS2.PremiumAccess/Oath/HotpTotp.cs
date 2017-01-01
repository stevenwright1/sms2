using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SMS2.Shared.Oath
{
	public class HotpTotp
	{
		public static string HOTP(string K, long C, int digits = 6)
		{
			return HotpTotp.HOTP(Encoding.UTF8.GetBytes(K), C, digits);
		}
		public static string HOTP(byte[] K, long C, int digits = 6)
		{
			HMACSHA1 hMACSHA = new HMACSHA1(K);
			byte[] hash = hMACSHA.ComputeHash(HotpTotp.LongToByteArray(C));
			string text = HotpTotp.Truncate(hash).ToString();
			if (text.Length < digits)
			{
				text = new string('0', digits) + text;
			}
			return text.Substring(checked(text.Length - digits));
		}
		public static string TOTP(string K, int digits = 6, int window = 30)
		{
			return HotpTotp.TOTP(Encoding.UTF8.GetBytes(K), DateTime.UtcNow, digits, window);
		}
		public static string TOTP(string K, DateTime dt, int digits = 6, int window = 30)
		{
			return HotpTotp.TOTP(Encoding.UTF8.GetBytes(K), dt, digits, window);
		}
		public static string TOTP(byte[] K, int digits = 6, int window = 30)
		{
			return HotpTotp.TOTP(K, DateTime.UtcNow, digits, window);
		}
		public static string TOTP(byte[] K, DateTime dt, int digits = 6, int window = 30)
		{
			DateTime d = new DateTime(1970, 1, 1);
			long c = Convert.ToInt64((dt - d).TotalSeconds / (double)window);
			return HotpTotp.HOTP(K, c, digits);
		}
		public static byte[] KeyFromHexString(string Key)
		{
			checked
			{
				byte[] array = new byte[Convert.ToInt32((double)Key.Length / 2.0) - 1 + 1];
				int arg_28_0 = 0;
				int num = array.Length - 1;
				for (int i = arg_28_0; i <= num; i++)
				{
					try
					{
						array[i] = Convert.ToByte(Key.Substring(i * 2, 2), 16);
					}
					catch
					{
						throw new Exception("Could not parse key as hexadecimal");
					}
				}
				return array;
			}
		}
		private static byte[] LongToByteArray(long longnum)
		{
			List<byte> list = new List<byte>();
			int num = 0;
			checked
			{
				do
				{
					list.Insert(0, (byte)(longnum & 255L));
					longnum >>= 8;
					num++;
				}
				while (num <= 7);
				return list.ToArray();
			}
		}
		private static long Truncate(byte[] hash)
		{
			int num = (int)(hash[checked(hash.Length - 1)] & 15);
			return (long)checked((int)(hash[num] & 127) << 24 | (int)(hash[num + 1] & 255) << 16 | (int)(hash[num + 2] & 255) << 8 | (int)(hash[num + 3] & 255));
		}
	}
}
