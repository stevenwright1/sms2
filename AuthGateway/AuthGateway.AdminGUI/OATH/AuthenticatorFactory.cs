using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
namespace AuthGateway.AdminGUI.OATH
{
	public class AuthenticatorFactory
	{
		public const string GoogleAuthenticator = "Google Authenticator";
		public const string FeitianToken = "Feitian Token";
		public const string FeitianSerial = "Feitian Serial";
		public const string Other = "Other";
		public static IAuthenticator Build(string auth)
		{
			switch (auth) {
				case GoogleAuthenticator:
					return new GoogleAuthenticator();
				case FeitianToken:
					return new FeitianTokenAuthenticator();
				case FeitianSerial:
					return new FeitianSerialAuthenticator();
				case Other:
					return new OtherAuthenticator();
				default:
					throw new NotSupportedException();
			}
		}
	}
}
