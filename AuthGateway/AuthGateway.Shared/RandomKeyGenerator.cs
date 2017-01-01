using System;
using System.Text;

namespace AuthGateway.Shared
{
	public static class RandomKeyGenerator
	{
		static char[] zeroToNine = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
		static char[] base32 = { 'A','B','C','D','E','F','G','H','I','J','K','L',
																					 'M','N','O','P','Q','R','S','T','U','V','W',
																					 'X','Y','Z', '2', '3', '4', '5', '6', '7' };

		public static String Generate(int len, RKGBase rkgbase)
		{
			if (rkgbase == RKGBase.Base10)
				return Generate(len, zeroToNine);
			if (rkgbase == RKGBase.Base32)
				return Generate(len, base32);
			throw new ArgumentException("Invalid RKBBase");
		}

		public static String Generate(int len, char[] chars)
		{
			StringBuilder sb = new StringBuilder();
			int charslen = chars.Length;
			for (int i_key = 0; i_key < len; i_key++)
				sb.Append(chars[CryptoRandom.Instance().Next(0, charslen)]);

			return sb.ToString();
		}
	}
}
