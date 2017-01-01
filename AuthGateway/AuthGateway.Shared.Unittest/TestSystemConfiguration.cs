using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AuthGateway.Shared.Serializer;

using NUnit.Framework;

namespace AuthGateway.Shared.Unittest
{
	/// <summary>
	/// Summary description for TestSystemConfiguration
	/// </summary>
	[TestFixture]
	public class TestSystemConfiguration
	{
		[Test]
		public void TestPProvider()
		{
			Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "TestSettings"));
			Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Settings"));
			SystemConfiguration sc = new SystemConfiguration(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Void"));
			Provider p = new Provider();
			p.Name = "Test";
			p.Enabled = true;
			p.AdGroup = "TestGroup";
			p.Default = true;
			p.Config = "";
			sc.Providers.Clear();
			sc.Providers.Add(p);

			sc.WriteXMLCredentials();

			sc = new SystemConfiguration(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Void"));
			sc.LoadSettings();
			Assert.AreEqual(p.Name, sc.Providers[0].Name);
		}
		[Test]
		public void TestTwoProviders()
		{
			Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "TestSettings"));
			Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Settings"));
			SystemConfiguration sc = new SystemConfiguration(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Void"));
			Provider p = new Provider()
			{
				Name = "Test",
				Enabled = true,
				AdGroup = "TestGroup",
				Default = true,
				Config = "TestConfig"
			};
			Provider p2 = new Provider()
			{
				Name = "Test2",
				Enabled = false,
				AdGroup = "TestGroup2",
				Default = false,
				Config = "TestConfig2"
			};
			sc.Providers.Clear();
			sc.Providers.Add(p);
			sc.Providers.Add(p2);

			sc.WriteXMLCredentials();

			sc = new SystemConfiguration(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Void"));
			sc.LoadSettings();
			Assert.AreEqual(p.Name, sc.Providers[0].Name);
		}

		[Test]
		public void TestPasswordsEncryptedIfNot()
		{
			Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "TestSettings"));
			Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Settings"));
			SystemConfiguration sc = new SystemConfiguration(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Void"));
			sc.ADPassword = "test_ADPassword";
			sc.DbPassword = "test_DbPassword";
			sc.EmailConfig.Password = "test_EmailPassword";
			sc.WriteXMLCredentials();

			var sccontent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Settings", "Configuration.xml"));
			Assert.IsFalse(sccontent.Contains("test_ADPassword"));
			Assert.IsFalse(sccontent.Contains("test_DbPassword"));
			Assert.IsFalse(sccontent.Contains("test_EmailPassword"));

			var sc2 = new SystemConfiguration(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Void"));
			sc2.LoadSettings();
			Assert.AreEqual(sc.ADPassword, sc2.ADPassword);
			Assert.AreEqual(sc.DbPassword, sc2.DbPassword);
			Assert.AreEqual(sc.EmailConfig.Password, sc2.EmailConfig.Password);
		}

		[Test]
		public void TestPasswordsEncryptedOnLoad()
		{
			Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "TestSettings"));
			Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Settings"));
			SystemConfiguration sc = new SystemConfiguration(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Void"));
			sc.ADPassword = "test_ADPassword";
			sc.DbPassword = "test_DbPassword";
			sc.EmailConfig.Password = "test_EmailPassword";
			sc.WriteXMLCredentials();

			var sccontent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Settings", "Configuration.xml"));
			Assert.IsFalse(sccontent.Contains("test_ADPassword"));
			Assert.IsFalse(sccontent.Contains("test_DbPassword"));
			Assert.IsFalse(sccontent.Contains("test_EmailPassword"));

			sccontent = Regex.Replace(sccontent, @"<ADPassword>.*</ADPassword>", "<ADPassword>test_ADPassword</ADPassword>");
			sccontent = Regex.Replace(sccontent, @"<DBPassword>.*</DBPassword>", "<DBPassword>test_DbPassword</DBPassword>");
			sccontent = Regex.Replace(sccontent, @"<Password>.*</Password>", "<Password>test_EmailPassword</Password>");

			File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Settings", "Configuration.xml"), sccontent);
			
			var sc2 = new SystemConfiguration(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Void"));
			sc2.LoadSettings(true);
			Assert.AreEqual(sc.ADPassword, sc2.ADPassword);
			Assert.AreEqual(sc.DbPassword, sc2.DbPassword);
			Assert.AreEqual(sc.EmailConfig.Password, sc2.EmailConfig.Password);

			sccontent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Settings", "Configuration.xml"));
			Assert.IsFalse(sccontent.Contains("test_ADPassword"), "ad");
			Assert.IsFalse(sccontent.Contains("test_DbPassword"), "db");
			Assert.IsFalse(sccontent.Contains("test_EmailPassword"), "email");

		}

		[Test]
		public void TestPartialEmailConfigDeserialization()
		{
			var xml = @"
<EmailConfig>
	<Username>wrightcss@stub.com</Username>
	<Password>encrypted:AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAk9PA3hAcbk+K6+lp3d62fgQAAAACAAAAAAAQZgAAAAEAACAAAADPn7SrLSUh3DOAjXbw+Xj0Oc5gRXqlFJcvWcsg4hKPEAAAAAAOgAAAAAIAACAAAAD8y94OJi9nxQJiUY1j6Wlw7p+EV9G+2Dm0NFc+y6rMwBAAAACNRTp2dbdWJmpNSXCeEBLXQAAAAG/KsWPH7J2RoHqIb6QKR3/dUFG0ceHtN/2Lh19liDVs6B/lNEhKMgb3UchPt38wWhHnhELdvbsJKb2CvZPmaug=</Password>
</EmailConfig>
";
			var emailConfig = Generic.Deserialize<EmailConfig>(xml);

		}

		[Test]
		public void TestTextLocalCloudSMSModuleConfigEncryptIssue73()
		{
			var sc = new SystemConfiguration();
			CloudSMSModuleConfig testModuleConfig;
			testModuleConfig = new CloudSMSModuleConfig()
			{
				TypeName = "Textlocal",
				ModuleParameters = new ModuleParameters() {
					new ModuleParameter() { Name = "Username", Value = "steven@stevenwright.co.uk" },
					new ModuleParameter() { Name = "Password", Value = "Password", Encrypt = true },
					new ModuleParameter() { Name = "Proxy", Value = "http://proxy:port", Encrypt = false },
					new ModuleParameter() { Name = "ProxyUsername", Value = "test", Encrypt = false },
					new ModuleParameter() { Name = "ProxyPassword", Value = "test", Encrypt = true },
				}
			};
			sc.CloudSMSConfiguration.CloudSMSModules[0] = testModuleConfig;
			sc.WriteXMLCredentials("textlocal.xml");

			using (var fileStream = File.OpenText("textlocal.xml"))
			{
				var texto = fileStream.ReadToEnd();
				Assert.IsFalse(texto.Contains("Password"), "The password was not encrypted");
			}
			Assert.IsTrue(sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("Password").Value == "Password");
		}

		[Test]
		public void TestCloudSMSModuleConfig()
		{
			var sc = new SystemConfiguration();
			sc.ManualDomainReplacements.Add(new DomainReplacement("AnotherDomainName"));

			CloudSMSModuleConfig testModuleConfig;

			testModuleConfig = sc.CloudSMSConfiguration.CloudSMSModules[0];
			var cloudSMSAsDefaultModuleConfig = Generic.Serialize<CloudSMSModuleConfig>(testModuleConfig);
			Console.WriteLine(cloudSMSAsDefaultModuleConfig);
			sc.WriteXMLCredentials("regexp.xml");

			testModuleConfig = new CloudSMSModuleConfig()
			{
				TypeName = "Twilio",
				ModuleParameters = new ModuleParameters() {
					new ModuleParameter() { Name = "AccountSid", Value = "ACd02ef72d128099e4989011e707dac1b9" },
					new ModuleParameter() { Name = "AuthToken", Value = "615ae4875a2aee8caa0a7b2995701c5b" },
					new ModuleParameter() { Name = "TwilioService", Value = "SMS" },
					new ModuleParameter() { Name = "From", Value = "+441158241527" },
				}
			};
			sc.CloudSMSConfiguration.CloudSMSModules[0] = testModuleConfig;
			sc.WriteXMLCredentials("twilio.sms.xml");

			testModuleConfig = new CloudSMSModuleConfig()
			{
				TypeName = "Twilio",
				ModuleParameters = new ModuleParameters() 
					{ 
						new ModuleParameter() { Name = "AccountSid", Value = "TWILIO ACCOUNT SID" },
						new ModuleParameter() { Name = "AuthToken", Value = "TWILIO AUTH TOKEN" },
						new ModuleParameter() { Name = "From", Value = "TWILIO PHONE NUMBER" },
						new ModuleParameter() { Name = "voice", Value = "alice" },
						new ModuleParameter() { Name = "language", Value = "en-GB" },
					}
			};
			sc.CloudSMSConfiguration.CloudSMSModules[0] = testModuleConfig;
			sc.WriteXMLCredentials("twilio.voice.xml");


			testModuleConfig = new CloudSMSModuleConfig()
			{
				TypeName = "Textlocal",
				ModuleParameters = new ModuleParameters() {
					new ModuleParameter() { Name = "Username", Value = "steven@stevenwright.co.uk" },
					new ModuleParameter() { Name = "Password", Value = "Password", Encrypt = true },
					new ModuleParameter() { Name = "Proxy", Value = "http://proxy:port", Encrypt = false },
					new ModuleParameter() { Name = "ProxyUsername", Value = "test", Encrypt = false },
					new ModuleParameter() { Name = "ProxyPassword", Value = "test", Encrypt = true },
				}
			};
			sc.CloudSMSConfiguration.CloudSMSModules[0] = testModuleConfig;
			sc.WriteXMLCredentials("textlocal.xml");


			testModuleConfig = new CloudSMSModuleConfig()
			{
				TypeName = "CMDirect",
				ModuleParameters = new ModuleParameters() {
					new ModuleParameter() { Name = "ProductToken", Value = "PRODUCT TOKEN" },
					new ModuleParameter() { Name = "Sender", Value = "SENDER MOBILE PHONE" },
					new ModuleParameter() { Name = "Tariff", Value = "OPTIONAL TARIFF PARAMETER" },
					new ModuleParameter() { Name = "Proxy", Value = "http://proxy:port", Encrypt = false },
					new ModuleParameter() { Name = "ProxyUsername", Value = "test", Encrypt = false },
					new ModuleParameter() { Name = "ProxyPassword", Value = "test", Encrypt = true },
				}
			};
			sc.CloudSMSConfiguration.CloudSMSModules[0] = testModuleConfig;
			sc.WriteXMLCredentials("cmdirect.xml");

			testModuleConfig = new CloudSMSModuleConfig()
			{
				TypeName = "GSM",
				ModuleParameters = new ModuleParameters()
				{
					new ModuleParameter() {
						Name = "input1", Value = "atz"
					},
					new ModuleParameter() {
						Name = "expected1", Value = "OK"
					},
					new ModuleParameter() {
						Name = "input2", Value = "at+csq"
					},
					new ModuleParameter() {
						Name = "expected2", Value = "([0-9]+)", Output=true
					},
					new ModuleParameter() {
						Name = "input3", Value = "at+cfun=1"
					},
					new ModuleParameter() {
						Name = "input4", Value = "at+cops?"
					},
					new ModuleParameter() {
						Name = "input5", Value = "at+cmgf=1"
					},
					new ModuleParameter() {
						Name = "input6", Value = "at+cmgs=\"{destiny}\"" // 07525218010
					},
					new ModuleParameter() {
						Name = "input7", Value = "{message}" // + (char)26
					},
					new ModuleParameter() {
						Name = "comport", Value = "COM13"
					},
					new ModuleParameter() {
						Name = "baudrate", Value = "9600"
					},
				}
			};
			sc.CloudSMSConfiguration.CloudSMSModules[0] = testModuleConfig;
			sc.WriteXMLCredentials("gsm.xml");




			testModuleConfig = new CloudSMSModuleConfig()
			{
				TypeName = CloudSMSModuleConfig.REGEXP,
				ModuleParameters = new ModuleParameters() 
				{ 
					//new ModuleParameter() { Name = "", Value = "" },
					new ModuleParameter() { Name = "Url", Value = @"http://www.textapp.net/webservice/httpservice.aspx" },
					new ModuleParameter() { Name = "Regex", Value = @"<Code>1</Code>" },
					new ModuleParameter() { Name = "CreditsRemaining", Value = "1", Output = true},
					new ModuleParameter() { Name = "method", Value = @"sendsms" },
						
					new ModuleParameter() { Name = "externalLogin", Value = @"CHANGE THIS CLIENT_ID" },
					new ModuleParameter() { Name = "password", Value = @"CHANGE THIS CLIENT_PASS", Encrypt = true },

					new ModuleParameter() { Name = "originator", Value = @"SMS2" },

					new ModuleParameter() { Name = "clientBillingReference", Value = @"1" },
					new ModuleParameter() { Name = "validity", Value = @"72" },
					new ModuleParameter() { Name = "characterSetID", Value = @"2" },
					new ModuleParameter() { Name = "replyMethodID", Value = @"1" },
					new ModuleParameter() { Name = "replyData", Value = @"" },
					new ModuleParameter() { Name = "statusNotificationUrl", Value = @"" },

					new ModuleParameter() { Name = "clientMessageReference", Value = @"{guid}" },
					new ModuleParameter() { Name = "destinations", Value = @"{destination}" },
					new ModuleParameter() { Name = "body", Value = @"{message}" },
				}
			};
			sc.CloudSMSConfiguration.CloudSMSModules[0] = testModuleConfig;
			sc.WriteXMLCredentials("textanywhere.xml");
		}

		[Test]
		public void TestExtraDCs()
		{
			Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "TestSettings"));
			Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Settings"));
			SystemConfiguration sc = new SystemConfiguration(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Void"));
			sc.ExtraDCs.Add(new DC("test1"));
			sc.ExtraDCs.Add(new DC("test2"));
			sc.WriteXMLCredentials();

			var sc2 = new SystemConfiguration(Path.Combine(Environment.CurrentDirectory, "TestSettings", "Void"));
			sc2.LoadSettings();
			Assert.AreEqual(3, sc.ExtraDCs.Count);
		}

		[Test]
		public void TestBadConfigLog()
		{
			var testsettings = Path.Combine(Environment.CurrentDirectory, "TestSettings");
			Directory.CreateDirectory(testsettings);
			var testsettingssettings = Path.Combine(Environment.CurrentDirectory, "TestSettings", "Settings");
			Directory.CreateDirectory(testsettingssettings);

			using (FileStream fs = File.Open(Path.Combine(testsettingssettings, "Configuration.xml"), FileMode.Create, FileAccess.Write))
			{
				using (StreamWriter log = new StreamWriter(fs, Encoding.UTF8))
				{
					log.WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?>
<User>
  <AuthEngineServerIP>127.0.0.100</AuthEngineServerIP>
</User>
");
				}
			}

			var sc = new SystemConfiguration(Path.Combine(testsettings, "Void"));
			sc.LoadSettings();
			Assert.AreEqual("127.0.0.100", sc.AuthEngineServerAddress.ToString());

			using (FileStream fs = File.Open(Path.Combine(testsettingssettings, "Configuration.xml"), FileMode.Create, FileAccess.Write))
			{
				using (StreamWriter log = new StreamWriter(fs, Encoding.UTF8))
				{
					log.WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?>
<User>
  <AuthEngineServerIP>127.0.0.100</CACA>
</User>
");
				}
			}
			try
			{
				sc = new SystemConfiguration(Path.Combine(testsettings, "Void"));
				sc.LoadSettings();
			}
			catch (Exception ex)
			{
				Assert.IsTrue(ex.Message.Contains("line"));
			}
		}
	}
}
