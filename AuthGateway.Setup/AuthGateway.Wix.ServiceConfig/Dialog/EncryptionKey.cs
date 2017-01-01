using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using AuthGateway.Shared;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class EncryptionKey : UserControl, IWizardScreen
	{
		protected SystemConfiguration sc;
		protected Wizard wizard;

		private string AES256Key;

		public EncryptionKey()
		{
			InitializeComponent();
		}
        public EncryptionKey(Wizard wizard, SystemConfiguration sc)
			: this()
		{
			this.wizard = wizard;
			this.sc = sc;

            tbKey.Text = sc.AuthEngineEncryptionKey;
		}

		public bool SkipNext()
		{
			return false;
		}

		public bool SkipPrevious()
		{
			return false;
		}

		public UserControl GetControl()
		{
			return this;
		}

		public bool Store()
		{
            // Input Validation BEGIN: 2016-10-16 - An AES 256-bit key can be expressed as a hexadecimal string with 64 characters. It will require 44 characters in base64.
            if (!tbKey.Text.Trim().Length.Equals(44)) {
                wizard.ShowError("Input was not a BASE64 encoded AES256 key.");
                return false;
            }
            // Input Validation END
			sc.AuthEngineEncryptionKey = tbKey.Text.Trim();
			return true;
		}

		public IWizardScreen GetWizardScreen()
		{
			return this;
		}		

        private void btnGenKey_Click(object sender, EventArgs e)
        {
            if (tbKey.Text.Trim().Length.Equals(44)) {
                wizard.ShowError("Refusing to create a new key as a key is already displayed.\r\n\r\nOn an existing installation, changing the AES key shown here will make all sensitive data within the SMS2 database inaccessible. ");
            }
            if (!tbKey.Text.Trim().Length.Equals(44)) {
                try {
                    AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
                    aesProvider.KeySize = 256;
                    aesProvider.GenerateIV();
                    aesProvider.GenerateKey();
                    tbKey.Text = Convert.ToBase64String(aesProvider.Key);
                }
                catch {
                    wizard.ShowError("Error generating encryption key.");
                }
            }
        }

	}
}
