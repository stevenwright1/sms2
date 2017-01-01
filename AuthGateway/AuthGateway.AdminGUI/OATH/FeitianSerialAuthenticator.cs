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
	public class FeitianSerialAuthenticator : IAuthenticator
	{
		public bool IsSerial()
		{
			return true;
		}

		public bool ShowQR()
		{
			return false;
		}

		public bool HexEncodedChecked()
		{
			return false;
		}

		public bool HexEncodedEnabled()
		{
			return false;
		}

		public bool ShowGenerate()
		{
			return false;
		}

		public string GetGeneratedToken()
		{
			return string.Empty;
		}

		public string GetSecretFormattedForQR(string secret, string user, bool isTotp, string counterOrWindow)
		{
			return secret;
		}

		public string GetEncodedSharedSecret(string secret, bool hexEncodedChecked)
		{
			return "feitian:" + secret;
		}

        public string DecodeSharedSecret(string secret, bool hexEncodedChecked)
        {
            string sharedSecret = string.Empty;

            if (sharedSecret.StartsWith("feitian:")) {
                sharedSecret = sharedSecret.Substring("feitian:".Length);
            }

            return sharedSecret;
        }

		public void Validate(string secret, bool hexEncodedChecked)
		{
			if (string.IsNullOrEmpty(secret)) {
				throw new ArgumentException("Serial number cannot be empty.");
			}

			if (Regex.IsMatch(secret, "[^0-9]")) {
				throw new ArgumentException("Feitian serial only allows numbers.");
			}
		}
		
		public string SharedSecretLabel() {
			return "Serial Number:";
		}
	}
}
