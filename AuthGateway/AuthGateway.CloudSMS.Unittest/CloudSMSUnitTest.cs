using System.Collections.Generic;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;
using AuthGateway.Shared.XmlMessages.Request.Command.CloudSms;
using AuthGateway.Shared.XmlMessages.Response.Ret.CloudSms;
using SMSService;
using SMSService.Modules;

using NUnit.Framework;

namespace AuthGateway.CloudSMS.Unittest
{
	[TestFixture]
	public class CloudSMSUnitTest
	{
		[Test]
		public void TestTextLocalParseHtmlResponse()
		{
			string test = @"TestMode=1<br>MessageReceived=Your SMS PassCODE is:750138518<br>MessageCount=1<br>From=SecureAuth - using default (Submitted > 11 chars)<br>CreditsAvailable=47.4<br>MessageLength=30<br>NumberContacts=1<br>CreditsRequired=1.2<br>CreditsRemaining=46.2<br>Testmode Active - Nothing Sent";
			SystemConfiguration sc = new SystemConfiguration();
			var tl = new Textlocal();
			Dictionary<string, string> response = tl.ParseHtmlResponse(test);
			Assert.IsTrue(response.ContainsKey("MessageReceived"));
			Assert.IsTrue(response.ContainsKey("CreditsRemaining"));
		}

		[Test]
		public void TestRegexpParseHtmlResponse()
		{
			string test = @"TestMode=1<br>MessageReceived=Your SMS PassCODE is:750138518<br>MessageCount=1<br>From=SecureAuth - using default (Submitted > 11 chars)<br>CreditsAvailable=47.4<br>MessageLength=30<br>NumberContacts=1<br>CreditsRequired=1.2<br>CreditsRemaining=46.2<br>Testmode Active - Nothing Sent";
			SystemConfiguration sc = new SystemConfiguration();
			var t = new Regexp();
			t.SetConfiguration(sc);
			t.SetModuleConfig(sc.CloudSMSConfiguration.CloudSMSModules[0]);
			var parsed = new Dictionary<string, string>();
			Assert.IsTrue(t.ParseHtmlResponse(test, parsed));
			Assert.IsTrue(parsed.ContainsKey("CreditsRemaining"));
			Assert.That(parsed["CreditsRemaining"], Is.EqualTo("46.2"));
		}
		
		[Test]
		public void TestRegexpParseCreditsRemaining()
		{
			string test = @"SUCCESS 93";
			SystemConfiguration sc = new SystemConfiguration();
			var t = new Regexp();
			
			var testModuleConfig = new CloudSMSModuleConfig()
			{
				TypeName = CloudSMSModuleConfig.REGEXP,
				ModuleParameters = new ModuleParameters() 
				{ 
					new ModuleParameter() { Name = "Regex", Value = @"SUCCESS (?'CreditsRemaining'\d+)" },
					new ModuleParameter() { Name = "CreditsRemaining", Value = "1", Output = true},
				}
			};
			sc.CloudSMSConfiguration.CloudSMSModules[0] = testModuleConfig;
			
			t.SetConfiguration(sc);
			t.SetModuleConfig(sc.CloudSMSConfiguration.CloudSMSModules[0]);
			var parsed = new Dictionary<string, string>();
			Assert.IsTrue(t.ParseHtmlResponse(test, parsed));
			Assert.That(parsed["CreditsRemaining"], Is.EqualTo("93"));
		}

		[Test]
		public void TestGetAvailableModulesWithEmptyModuleNameReturnTypeName()
		{
			SystemConfiguration sc = new SystemConfiguration();
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDisableUserVerification = true;

			var sl = new ServerLogic(sc);
			var getModulesRet = (GetModulesRet)sl.GetModules(new GetModules() { });
			Assert.AreEqual(1, getModulesRet.Modules.Count);
			Assert.AreEqual("Regexp", getModulesRet.Modules[0]);
		}

		[Test]
		public void TestGetAvailableModulesReturnModuleName()
		{
			SystemConfiguration sc = new SystemConfiguration();
			sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleName = "Testing123";
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDisableUserVerification = true;

			var sl = new ServerLogic(sc);
			var getModulesRet = (GetModulesRet)sl.GetModules(new GetModules() { });
			Assert.AreEqual(1, getModulesRet.Modules.Count);
			Assert.AreEqual("Testing123", getModulesRet.Modules[0]);
		}

		[Test]
		public void TestTwilioSendSMS()
		{
			SystemConfiguration sc = new SystemConfiguration();
			var mc = new CloudSMSModuleConfig()
			{
				TypeName = "Twilio",
				ModuleParameters = new ModuleParameters() {
					new ModuleParameter() { Name = "AccountSid", Value = "ACd02ef72d128099e4989011e707dac1b9" },
					new ModuleParameter() { Name = "AuthToken", Value = "615ae4875a2aee8caa0a7b2995701c5b" },
					new ModuleParameter() { Name = "TwilioService", Value = "SMS" },
					new ModuleParameter() { Name = "From", Value = "+441158241527" },
				}
			};
			sc.CloudSMSConfiguration.CloudSMSModules[0] = mc;
			var t = new Twilio();
			t.SetConfiguration(sc);
			t.SetModuleConfig(mc);
			var smscmd = new SendSms();
			//smscmd.Destination = "+5491169531549";
			smscmd.Destination = "+441158241527";
			smscmd.Message = "Test message";
			var ret = t.SendSMSMessage(smscmd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
		}

		[Test]
		public void TestTwilioSendVoice()
		{
			SystemConfiguration sc = new SystemConfiguration();
			var mc = new CloudSMSModuleConfig()
			{
				TypeName = "Twilio",
				ModuleParameters = new ModuleParameters() {
					new ModuleParameter() { Name = "AccountSid", Value = "ACd02ef72d128099e4989011e707dac1b9" },
					new ModuleParameter() { Name = "AuthToken", Value = "615ae4875a2aee8caa0a7b2995701c5b" },
					new ModuleParameter() { Name = "From", Value = "+441158241527" },
					new ModuleParameter() { Name = "voice", Value = "alice" },
					new ModuleParameter() { Name = "language", Value = "en-GB" },
				}
			};
			var t = new Twilio();
			t.SetConfiguration(sc);
			t.SetModuleConfig(mc);
			var smscmd = new SendSms();
			//smscmd.Destination = "+5491169531549";
			smscmd.Destination = "+447525218010";
			smscmd.Message = "Test message";
			var ret = t.SendSMSMessage(smscmd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
		}

		[Test]
		public void TestTwilioSendVoiceFailAndLogError()
		{
			SystemConfiguration sc = new SystemConfiguration();
			var mc = new CloudSMSModuleConfig()
			{
				TypeName = "Twilio",
				ModuleParameters = new ModuleParameters() {
					new ModuleParameter() { Name = "AccountSid", Value = "thisisatest" },
					new ModuleParameter() { Name = "AuthToken", Value = "thisisatest" },
					new ModuleParameter() { Name = "From", Value = "+441158241527" },
					new ModuleParameter() { Name = "voice", Value = "alice" },
					new ModuleParameter() { Name = "language", Value = "en-GB" },
				}
			};
			sc.CloudSMSConfiguration.CloudSMSModules[0] = mc;
			var t = new Twilio();
			t.SetConfiguration(sc);
			t.SetModuleConfig(mc);
			var smscmd = new SendSms();
			//smscmd.Destination = "+5491169531549";
			smscmd.Destination = "+447525218010";
			smscmd.Message = "Test message";
			using (var memLogger = new MemoryLogger()) {
				Logger.Instance.AddLogger(memLogger);

				var ret = t.SendSMSMessage(smscmd);
				Assert.IsFalse(string.IsNullOrEmpty(ret.Error));

				var memLoggerText = memLogger.GetText();
				Logger.Instance.RemoveLogger(memLogger);

				Assert.IsTrue(memLoggerText.Contains("Authenticate"));
			}
		}

		[Test]
		public void TestGsmSender()
		{
			SenderFactory.Testing = true;

			SystemConfiguration sc = new SystemConfiguration();
			var mc = new CloudSMSModuleConfig()
			{
				TypeName = "Gsm",
				ModuleParameters = new ModuleParameters() {
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
			sc.CloudSMSConfiguration.CloudSMSModules[0] = mc;
			var gsm = new Gsm();
			gsm.SetConfiguration(sc);
			gsm.SetModuleConfig(mc);
			var smscmd = new SendSms();
			smscmd.Destination = "+447525218010";
			smscmd.Message = "Test message";
			gsm.SendSMSMessage(smscmd);
			var testsender = (GsmTestSender)GsmProcessor.Instance.GetSender();
			Assert.AreEqual(1, testsender.Messages);
		}

		[Test]
		public void TestTextAnywhere()
		{
			Assert.Inconclusive("THIS WOULD SEND A REAL TEST MESSAGE");
			SystemConfiguration sc = new SystemConfiguration();

			var mc = new CloudSMSModuleConfig()
			{
				TypeName = CloudSMSModuleConfig.REGEXP,
				ModuleParameters = new ModuleParameters() 
					{ 
						//new ModuleParameter() { Name = "", Value = "" },
						new ModuleParameter() { Name = "Url", Value = @"http://www.textapp.net/webservice/httpservice.aspx" },
						new ModuleParameter() { Name = "Regex", Value = @"<Code>1</Code>" },
						new ModuleParameter() { Name = "CreditsRemaining", Value = "1", Output = true},
						new ModuleParameter() { Name = "method", Value = @"sendsms" },
						
						new ModuleParameter() { Name = "externalLogin", Value = @"U13207688" },
						new ModuleParameter() { Name = "password", Value = @"1kapgsgi", Encrypt = true },

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
			sc.CloudSMSConfiguration.CloudSMSModules[0] = mc;
			var tl = new Regexp();
			tl.SetConfiguration(sc);
			tl.SetModuleConfig(mc);

			var smscmd = new SendSms();
			smscmd.Destination = "+447525218010";
			smscmd.Message = "Test message from TextAnywhere";
			var ret = tl.SendSMSMessage(smscmd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
		}
	}
}
