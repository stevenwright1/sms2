using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using AuthGateway.Shared.Serializer;

namespace AuthGateway.LicenseCreator
{
	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = true)]
	public class LicenseXml
	{
		public string Name { get; set; }
		public string PhoneNumber { get; set; }
		public string CompanyName { get; set; }
		public string AuthEngine { get; set; }
		public string CloudSMS { get; set; }
		public string OathCalc { get; set; }
		public string Check { get; set; }
	}

	public class LicenseHandler
	{
		private RijndaelManaged aes;

		public LicenseHandler()
		{
			aes = new RijndaelManaged();
			byte[] key = Convert.FromBase64String("vCdqx6HGDAWeW1xqcacZqcjGmf30FeRizTG8oIzBJ4s=");
			byte[] iv = Convert.FromBase64String("9F72tkLwkigOWjbWhkf2yg==");
			aes = new RijndaelManaged()
			{
				Padding = PaddingMode.PKCS7,
				Mode = CipherMode.CBC,
				KeySize = 256,
				Key = key,
				IV = iv,
			};
		}
		public void Save(string file, DateTime licenseExpires)
		{
			var path = Path.GetDirectoryName(file);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			File.WriteAllText(file, SaveToString(licenseExpires), Encoding.UTF8);
		}
		public string SaveToString(DateTime licenseExpires)
		{
			var xml = new LicenseXml();
			xml.AuthEngine = Convert.ToBase64String(Encrypt(licenseExpires.ToUniversalTime().ToBinary().ToString()));
			return Generic.Serialize<LicenseXml>(xml);
		}

		private byte[] Encrypt(string text)
		{
			using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
			{
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
							swEncrypt.Write(text);
					}
					return msEncrypt.ToArray();
				}
			}
		}

		private string Decrypt(string text)
		{
			using (var dec = aes.CreateDecryptor(aes.Key, aes.IV))
			{
				using (var ms = new MemoryStream())
				{
					byte[] keyb = Convert.FromBase64String(text);
					ms.Write(keyb, 0, keyb.Length);
					ms.Position = 0;
					using (CryptoStream cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
					{
						using (StreamReader sr = new StreamReader(cs))
							return sr.ReadToEnd();
					}
				}
			}
		}

		public LicenseXml LoadFromFile(string file)
		{
			var txt = File.ReadAllText(file, Encoding.UTF8);
			return Load(txt);
		}

		public LicenseXml Load(string text)
		{
			text = text.Trim();
			var xml = Generic.Deserialize<LicenseXml>(text);
			return xml;
		}

		public DateTime getDecryptedTime(string timestr)
		{
			return DateTime.FromBinary(Convert.ToInt64(Decrypt(timestr))).ToUniversalTime();
		}
	}
}
