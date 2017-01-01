using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuthGateway.Shared
{
	public static class HexConversion
	{
		public static string ToString(byte[] bytes)
		{
			char[] c = new char[bytes.Length * 2];
			byte b;
			for (int i = 0; i < bytes.Length; i++)
			{
				b = ((byte)(bytes[i] >> 4));
				c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
				b = ((byte)(bytes[i] & 0xF));
				c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
			}
			return new string(c);
		}

		public static byte[] ToBytes(string hexString)
		{
			byte[] hexBytes = new byte[(hexString.Length / 2)];
			for (int z = 0; z < hexBytes.Length; z++)
			{
				try
				{
					hexBytes[z] = Convert.ToByte(hexString.Substring((z * 2), 2), 0x10);
				}
				catch
				{
					throw new Exception("Could not parse key as hexadecimal");
				}
			}
			return hexBytes;
		}
	}
}
