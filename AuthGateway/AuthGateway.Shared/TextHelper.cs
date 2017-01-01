using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

namespace AuthGateway.Shared
{
	public static class TextHelper
	{
		public static string Reverse( string s )
		{
		    char[] charArray = s.ToCharArray();
		    Array.Reverse( charArray );
		    return new string( charArray );
		}
		
		public static String Number2String(int number)
		{
			int startLetter = (int)'A';
			Char c = (Char)(startLetter + (number - 1));
			return c.ToString();
		}

		public static int String2Number(string number)
		{
			number = number.ToUpper();
			Char A = 'A';
			Char c = Convert.ToChar(number);
			return Convert.ToInt32(c - A + 1);
		}

		public static Dictionary<string, string> ParsePipeML(string ml)
		{
			var ret = new Dictionary<string, string>();

			var reg = new Regex(@"\|([a-zA-Z0-9]+)\|([^|]+)\|\/[a-zA-Z0-9]+\|");
			foreach (Match match in reg.Matches(ml))
			{
				ret.Add(match.Groups[1].Value, match.Groups[2].Value);
			}
			return ret;
		}

		public static string GeneratePipeML(Dictionary<string, string> dict) {
			var sb = new StringBuilder();
			foreach (var kp in dict)
			{
				sb.AppendFormat("|{0}|{1}|/{0}|", kp.Key, kp.Value);
			}
			return sb.ToString();
		}
	}
}
