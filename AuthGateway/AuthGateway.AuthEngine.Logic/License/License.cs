using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AuthGateway.Shared.Serializer;

public class License
{
	public DateTime AuthEngineEndDate { get; set; }
}

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
public class LicenseXmlLoader
{
	private RijndaelManaged aes;
	public LicenseXmlLoader()
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
	public License LoadFrom(string file)
	{
		var license = new License();
		var licenseXml = Generic.Open<LicenseXml>(file);
		license.AuthEngineEndDate =
				DateTime.FromBinary(Convert.ToInt64(Decrypt(Convert.FromBase64String(licenseXml.AuthEngine)))).ToUniversalTime();
		return license;
	}
	private string Decrypt(byte[] keyb)
	{
		using (var dec = aes.CreateDecryptor(aes.Key, aes.IV))
		{
			using (var ms = new MemoryStream())
			{
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
}
public class LicenseDocLoader
{
	public License LoadFrom(string file)
	{
		XmlDocument doc = new XmlDocument();
		var txt = File.ReadAllText(file, Encoding.UTF8);
		txt = txt.Trim();
		doc.LoadXml(txt);
		var license = new License()
		{
			AuthEngineEndDate = DateTime.FromBinary(Convert.ToInt64(readFrom(doc, "AuthEngine"))).ToUniversalTime()
		};
		return license;
	}
	private string readFrom(XmlDocument doc, string itemName)
	{
		XmlNodeList list = doc.GetElementsByTagName(itemName);
		foreach (XmlElement item in list)
		{
			var aes = new RijndaelManaged();
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
			using (var dec = aes.CreateDecryptor(aes.Key, aes.IV))
			{
				using (var ms = new MemoryStream())
				{
					byte[] keyb = Convert.FromBase64String(item.InnerText);
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
		return string.Empty;
	}
}