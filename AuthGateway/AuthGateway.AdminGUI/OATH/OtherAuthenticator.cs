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
using System.Text;
using System.Text.RegularExpressions;

namespace AuthGateway.AdminGUI.OATH
{
	public class OtherAuthenticator : IAuthenticator
	{
		public bool IsSerial()
		{
			return false;
		}

		public bool ShowQR()
		{
			return true;
		}

		public bool HexEncodedChecked()
		{
			return false;
		}

		public bool HexEncodedEnabled()
		{
			return true;
		}

		public bool ShowGenerate()
		{
			return true;
		}

		public string GetGeneratedToken()
		{
			byte[] rand = new byte[10];
			CryptoRandom.Instance().NextBytes(rand);
			return Base32Encoding.ToString(rand);
		}

		public string GetSecretFormattedForQR(string secret, string user, bool isTotp, string counterOrWindow)
		{
			return secret;
		}

		public string GetEncodedSharedSecret(string secret, bool hexEncodedChecked)
		{
			string sharedSecret = null;
			if (hexEncodedChecked) {
				sharedSecret = secret;
			} else {
				byte[] secretBytes = null;
				secretBytes = new UTF8Encoding(false).GetBytes(secret);
				sharedSecret = HexConversion.ToString(secretBytes);
			}
			return sharedSecret;
		}

        public string DecodeSharedSecret(string secret, bool hexEncodedChecked)
        {
            string sharedSecret = null;
            if (hexEncodedChecked) {
                sharedSecret = secret;
            }
            else {
                byte[] secretBytes = HexConversion.ToBytes(secret);
                sharedSecret = new UTF8Encoding(false).GetString(secretBytes);                 
            }
            return sharedSecret;
        }

		public void Validate(string secret, bool hexEncodedChecked)
		{
			if (string.IsNullOrEmpty(secret)) {
				throw new ArgumentException("Shared secret cannot be empty.");
			}
			if (hexEncodedChecked && Regex.IsMatch(secret, "[^0-9A-Fa-f]")) {
				throw new ArgumentException("Hex shared secret only allows the following set of characters: 0-9 A-F");
			}
		}
		
		public string SharedSecretLabel() {
			return "Shared Secret:";
		}
	}
}
