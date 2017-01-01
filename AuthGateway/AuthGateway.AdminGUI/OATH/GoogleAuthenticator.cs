using System;
using System.Linq;

using AuthGateway.Shared;
using AuthGateway.Shared.Tracking;

namespace AuthGateway.AdminGUI.OATH
{
	public class GoogleAuthenticator : IAuthenticator
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
			return Base32Encoding.ToString(rand);
		}

		public string GetSecretFormattedForQR(string secret, string user, bool isTotp, string counterOrWindow)
		{
			//otpauth://totp/alice@google.com?secret=JBSWY3DPEHPK3PXP
			//otpauth://hotp/alice@google.com?secret=JBSWY3DPEHPK3PXP&counter=1
			if (!isTotp) {
				if (string.IsNullOrWhiteSpace(counterOrWindow)) {
					counterOrWindow = "1";
				}
				return string.Format("otpauth://hotp/{0}?secret={1}&counter={2}", user, secret, counterOrWindow);
			} else {
				var period = (!string.IsNullOrWhiteSpace(counterOrWindow) && counterOrWindow != "30" ) 
					? string.Format("&period={0}", counterOrWindow)
					: string.Empty;
				return string.Format("otpauth://totp/{0}?secret={1}{2}", user, secret, period);
			}
		}

		public string GetEncodedSharedSecret(string secret, bool hexEncodedChecked)
		{
			string sharedSecret = null;
			byte[] secretBytes = null;
			secretBytes = Base32Encoding.ToBytes(secret);
			sharedSecret = HexConversion.ToString(secretBytes);
			return sharedSecret;
		}

        public string DecodeSharedSecret(string secret, bool hexEncodedChecked)
        {             
            byte[] secretBytes = HexConversion.ToBytes(secret);
            string sharedSecret = Base32Encoding.ToString(secretBytes);            
            return sharedSecret;
        }

		public void Validate(string secret, bool hexEncodedChecked)
		{
			if (string.IsNullOrEmpty(secret)) {
				throw new ArgumentException("Shared secret cannot be empty.");
			}
			try {
				Base32Encoding.ToBytes(secret);
			} catch (ArgumentException ex) {
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw new ArgumentException("Google Authenticator only allows Base32 shared secrets.");
			}
		}
		
		public string SharedSecretLabel() {
			return "Shared Secret:";
		}
	}
}
