using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using AuthGateway.Shared;
using System.Text.RegularExpressions;

namespace AuthGateway.AdminGUI.OATH
{
	public class FeitianTokenAuthenticator : IAuthenticator
	{
		public bool IsSerial()
		{
			return false;
		}

		public bool ShowQR()
		{
			return false;
		}

		public bool HexEncodedChecked()
		{
			return true;
		}

		public bool HexEncodedEnabled()
		{
			return false;
		}

		public bool ShowGenerate()
		{
			return true;
		}

		public string GetGeneratedToken()
		{
			byte[] rand = new byte[10];
			CryptoRandom.Instance().NextBytes(rand);
			return HexConversion.ToString(rand);
		}

		public string GetSecretFormattedForQR(string secret, string user, bool isTotp, string counterOrWindow)
		{
			return secret;
		}

		public string GetEncodedSharedSecret(string secret, bool hexEncodedChecked)
		{
			return secret;
		}

        public string DecodeSharedSecret(string secret, bool hexEncodedChecked)
        {
            return secret;
        }

		public void Validate(string secret, bool hexEncodedChecked)
		{
			if (string.IsNullOrEmpty(secret)) {
				throw new ArgumentException("Shared secret cannot be empty.");
			}

			if (Regex.IsMatch(secret, "[^0-9A-Fa-f]")) {
				throw new ArgumentException("Hex shared secret only allows the following set of characters: 0-9 A-F");
			}
		}
		
		public string SharedSecretLabel() {
			return "Shared Secret:";
		}
	}
}
