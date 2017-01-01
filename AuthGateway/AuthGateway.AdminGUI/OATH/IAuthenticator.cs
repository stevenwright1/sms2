using System;
using System.Linq;
namespace AuthGateway.AdminGUI.OATH
{
	public interface IAuthenticator
	{
		bool ShowQR();
		bool HexEncodedChecked();
		bool HexEncodedEnabled();
		bool ShowGenerate();
		string GetGeneratedToken();
		string GetSecretFormattedForQR(string secret, string user, bool isTotp, string counterOrWindow);

		string GetEncodedSharedSecret(string secret, bool hexEncodedChecked);

        string DecodeSharedSecret(string secret, bool hexEncodedChecked);

		void Validate(string secret, bool hexEncodedChecked);
		
		string SharedSecretLabel();

		bool IsSerial();
	}
}
