using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Security.Cryptography;
using AuthGateway.Shared;

namespace AuthGateway.OATH.Cmd
{

	static class OATHCmd
	{
		static Dictionary<string, Arg> knownArgs = new Dictionary<string, Arg>();
		public static void Main()
		{
			string[] args = System.Environment.GetCommandLineArgs();

			knownArgs.Add("hash", new Arg("hotp", "Can be hotp or totp"));
			knownArgs.Add("keytype", new Arg("plain", "plain,hex or base32 the key type, it will be converted before use"));
			knownArgs.Add("counter", new Arg("0", "0..n"));
			knownArgs.Add("digits", new Arg("6", "Usually 6 or 8 (amount of digits returned)"));
			knownArgs.Add("window", new Arg("30", "window value used in TOTP"));
			if ((args.Length <= 1)) {
				PrintUsage("No arguments supplied");
				return;
			}

			string key = string.Empty;

			for (int I = 1; I <= args.Length - 1; I++) {
				string arg = args[I];
				if (arg.StartsWith("-")) {
					arg = arg.Substring(1);
					string[] argParse = arg.Split('=');
					if (argParse.Length != 2 || !knownArgs.ContainsKey(argParse[0])) {
						PrintUsage("Invalid command line parameter: " + arg);
						return;
					} else {
						knownArgs[argParse[0]].Value = argParse[1];
					}
				} else {
					if (key.Equals(string.Empty)) {
						// We don't have a key yet, so assign it
						key = arg;
					} else {
						// More than one key found in command line?
						PrintUsage("More than one key found in command line" + arg);
						return;
					}
				}
			}

			byte[] realKey = Encoding.UTF8.GetBytes(key);
			if (knownArgs["keytype"].Value.Equals("hex")) {
				realKey = HotpTotp.KeyFromHexString(key);
			} else if (knownArgs["keytype"].Value.Equals("base32")) {
				realKey = Base32Encoding.ToBytes(key);
			}
			if (knownArgs["hash"].Value.Equals("totp")) {
				Console.WriteLine(HotpTotp.TOTP(realKey, int.Parse(knownArgs["digits"].Value), int.Parse(knownArgs["window"].Value)));
			} else if (knownArgs["hash"].Value.Equals("hotp")) {
				Console.WriteLine(HotpTotp.HOTP(realKey, long.Parse(knownArgs["counter"].Value), int.Parse(knownArgs["digits"].Value)));
			} else {
				Console.WriteLine("Invalid hash type");
			}
		}

		private static void PrintUsage(string errorMessage = "")
		{
			if ((errorMessage.Length > 0)) {
				Console.WriteLine("Error: " + errorMessage);
			}
			Console.WriteLine("Usage: once.upon.a.vb.time.cmd <key> ");
			Console.WriteLine("Optional parameters: ");
			foreach (KeyValuePair<string, Arg> a in knownArgs) {
				Console.WriteLine("\t -" + a.Key + "=" + a.Value.Value + " " + a.Value.Explain);
			}
			Console.ReadKey();
		}

		private class Arg
		{
			private string _value;

			private string _explain;
			public Arg(string value, string explain)
			{
				_value = value;
				_explain = explain;
			}
			public string Value {
				get { return _value; }
				set { _value = value; }
			}
			public string Explain {
				get { return _explain; }
			}
		}
	}
}
