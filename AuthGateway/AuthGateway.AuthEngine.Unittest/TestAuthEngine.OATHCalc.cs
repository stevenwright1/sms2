using System;
using System.Text;
using AuthGateway.AuthEngine.Logic;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.AuthEngine.Logic.Helpers;
using AuthGateway.AuthEngine.Logic.ProviderLogic;
using AuthGateway.OATH;
using AuthGateway.Shared;
using AuthGateway.Shared.Helper;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using NUnit.Framework;

namespace AuthGateway.AuthEngine.Unittest
{
	[TestFixture]
	public class TestAuthEngineOATHCalc : TestWithServers
	{
		[Test]
		public void TestOATHCalcValidateUserHotpEmergencyToken()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			startAuthEngineServer();
			startOATHServer();

			UserProvider up = new UserProvider()
			{
				Name = "OATHCalc",
				Config = "HOTP,"
					+ HexConversion.ToString(Encoding.UTF8.GetBytes("12345678901234567890"))
					+ ",0"
			};
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
				new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
			Assert.AreEqual(1, ret.Out);

			var emergency = new EmergencyProviderLogic().Using(aeServerLogic).Using(sc);
			emergency.SendToken(this.Username, getOrgOrBaseDN());
			
			ValidateUser vu = new ValidateUser();
			vu.User = this.OrgUsername;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("1", vuret.CreditsRemaining);
			Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
			var vp = new ValidatePin() {
				User = this.OrgUsername,
				//Pin = "755224",
				Pin = ((EmergencyProviderLogic)emergency).GetPin(),
				State = vuret.State
			};

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
			Assert.IsTrue(vpret.Validated, "Validated is not true");
		}

		
		[Test]
		public void TestOATHCalcValidateUserHotp()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			startAuthEngineServer();
			startOATHServer();

			UserProvider up = new UserProvider()
			{
				Name = "OATHCalc",
				Config = "HOTP,"
					+ HexConversion.ToString(Encoding.UTF8.GetBytes("12345678901234567890"))
					+ ",0"
			};
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
				new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
			Assert.AreEqual(1, ret.Out);

			ValidateUser vu = new ValidateUser();
			vu.User = this.OrgUsername;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("1", vuret.CreditsRemaining);
			Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
			ValidatePin vp = new ValidatePin();
			vp.User = this.OrgUsername;
			vp.Pin = "755224";
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
			Assert.IsTrue(vpret.Validated, "Validated is not true");
		}

		[Test]
		public void TestOATHCalcValidateUserDelayedTotp()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			startAuthEngineServer();
			startOATHServer();

			UserProvider up = new UserProvider()
			{
				Name = "OATHCalc",
				Config = "TOTP,"
					+ HexConversion.ToString(Encoding.UTF8.GetBytes("12345678901234567890"))
					+ ","
			};
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
				new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
			Assert.AreEqual(1, ret.Out);

			sc.OATHCalcTotpWindow = 10;

			String oldtoken = HotpTotp.TOTP(Encoding.UTF8.GetBytes("12345678901234567890"), (DateTime.UtcNow.AddSeconds(-90)), 6, 30);
			Assert.AreNotEqual(oldtoken, HotpTotp.TOTP(Encoding.UTF8.GetBytes("12345678901234567890"), 6, 30));

			ValidateUser vu = new ValidateUser();
			vu.User = this.OrgUsername;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("1", vuret.CreditsRemaining);

			ValidatePin vp = new ValidatePin();
			vp.User = this.OrgUsername;
			vp.Pin = oldtoken;
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
			Assert.IsTrue(vpret.Validated, "Validated is not true");
		}

		[Test]
		public void TestOATHCalcValidateUserTotpWithDifferentWindow()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			startAuthEngineServer();
			startOATHServer();

			var sharedSecret = Encoding.UTF8.GetBytes("12345678901234567890");

			var up = new UserProvider()
			{
				Name = "OATHCalc",
				Config = "TOTP,"
					+ HexConversion.ToString(sharedSecret)
					+ ",0,5,Device Name"
			};
			var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
				new SetUserProvider { User = Username, Org = Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
			
			var vu = new ValidateUser { User = OrgUsername };
			var vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("1", vuret.CreditsRemaining);

			var token = HotpTotp.TOTP(sharedSecret, 6, 5);
			Assert.AreNotEqual(HotpTotp.TOTP(sharedSecret, 6, 30), token);
			
			var vp = new ValidatePin {
				User = OrgUsername,
				Pin = token,
				State = vuret.State 
			};

			var vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
		}
		
		[Test]
		public void TestOATHCalcValidateUserTotpWith2Skew()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			startAuthEngineServer();
			startOATHServer();

			var sharedSecret = Encoding.UTF8.GetBytes("12345678901234567890");

			var up = new UserProvider()
			{
				Name = "OATHCalc",
				Config = "TOTP,"
					+ HexConversion.ToString(sharedSecret)
					+ ","
			};
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
				new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
			Assert.AreEqual(1, ret.Out);

			sc.OATHCalcTotpWindow = 10;

			var oldtoken = HotpTotp.TOTP(sharedSecret, (DateTime.UtcNow.AddSeconds(-30)), 6, 30);
			var currentToken = HotpTotp.TOTP(sharedSecret, 6, 30);
			var skew1token = HotpTotp.TOTP(sharedSecret, (DateTime.UtcNow.AddSeconds(+30)), 6, 30);

			var vu = new ValidateUser();
			vu.User = this.OrgUsername;
			var vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("1", vuret.CreditsRemaining);

			var vp = new ValidatePin();
			vp.User = this.OrgUsername;
			vp.Pin = skew1token;
			vp.State = vuret.State;

			var vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
			Assert.IsTrue(vpret.Validated, "Skew1 should be valid");

			var userId = (Int64)QueryValSQL("SELECT ID FROM SMS_CONTACT WHERE AD_USERNAME = '" + this.Username + "'")[0][0];
			var oathCalcId = (int)QueryValSQL("SELECT id FROM AuthProviders WHERE name = 'OATHCalc'")[0][0];
			var oathCalcLine = (string)QueryValSQL(string.Format(
				"SELECT config FROM UserProviders WHERE userId = '{0}' AND authProviderId = '{1}' ",
				userId, oathCalcId))[0][0];
			Assert.IsTrue(oathCalcLine.Split(new char[] { ',' })[2] == "1");

			var skew2token = HotpTotp.TOTP(sharedSecret, (DateTime.UtcNow.AddSeconds(+60)), 6, 30);
			for (var i = 0; i < 2; i++)
			{
				vu = new ValidateUser();
				vu.User = this.OrgUsername;
				vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
				Assert.AreEqual("1", vuret.CreditsRemaining);

				vp = new ValidatePin();
				vp.User = this.OrgUsername;
				vp.Pin = skew2token;
				vp.State = vuret.State;

				vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
				Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
				Assert.IsTrue(vpret.Validated, string.Format("Skew2 in iteration '{0}' should be valid", i));

				oathCalcLine = (string)QueryValSQL(string.Format(
					"SELECT config FROM UserProviders WHERE userId = '{0}' AND authProviderId = '{1}' ",
					userId, oathCalcId))[0][0];
				var skewInConfig = oathCalcLine.Split(new char[] { ',' })[2];
				Assert.IsTrue(skewInConfig == 2.ToString(), string.Format("Skew in iteration '{0}' is '{1}' and should be 2, config line: {2}", i, skewInConfig, oathCalcLine));
				System.Threading.Thread.Sleep(31 * 1000);
				skew2token = HotpTotp.TOTP(sharedSecret, (DateTime.UtcNow.AddSeconds(+60)), 6, 30);
			}
		}

		[Test]
		public void TestOATHCalcValidateUserDelayedTotpShouldFailWindow0()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			startAuthEngineServer();
			startOATHServer();

			UserProvider up = new UserProvider()
			{
				Name = "OATHCalc",
				Config = "TOTP,"
					+ HexConversion.ToString(Encoding.UTF8.GetBytes("12345678901234567890"))
					+ ","
			};
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
				new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
			Assert.AreEqual(1, ret.Out);

			sc.OATHCalcTotpWindow = 0;

			var oldtoken = HotpTotp.TOTP(Encoding.UTF8.GetBytes("12345678901234567890"), (DateTime.UtcNow.AddSeconds(-90)), 6, 30);
			var currentToken = HotpTotp.TOTP(Encoding.UTF8.GetBytes("12345678901234567890"), 6, 30);
			Assert.AreNotEqual(oldtoken, currentToken);

			ValidateUser vu = new ValidateUser();
			vu.User = this.OrgUsername;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("1", vuret.CreditsRemaining);

			ValidatePin vp = new ValidatePin();
			vp.User = this.OrgUsername;
			vp.Pin = oldtoken;
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
			Assert.IsFalse(vpret.Validated, "Validated is true");
		}

		[Test]
		public void TestOATHCalcValidateUserHotpEncrypted()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.OATHCalcUseEncryption = true;
			startAuthEngineServer();
			startOATHServer();

			UserProvider up = new UserProvider()
			{
				Name = "OATHCalc",
				Config = "HOTP,"
					+ HexConversion.ToString(Encoding.UTF8.GetBytes("12345678901234567890"))
					+ ",0"
			};
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
				new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
			Assert.AreEqual(1, ret.Out);

			ValidateUser vu = new ValidateUser();
			vu.User = this.OrgUsername;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("1", vuret.CreditsRemaining);
			ValidatePin vp = new ValidatePin();
			vp.User = this.OrgUsername;
			vp.Pin = "755224";
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
			Assert.IsTrue(vpret.Validated, "Validated is not true");
		}

		[Test]
		public void TestOATHCalcValidateUserHotpResync()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			startAuthEngineServer();
			startOATHServer();

			UserProvider up = new UserProvider()
			{
				Name = "OATHCalc",
				Config = "HOTP,"
					+ HexConversion.ToString(Encoding.UTF8.GetBytes("12345678901234567890"))
					+ ",0"
			};
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
				new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
			Assert.AreEqual(1, ret.Out);

			ResyncHotp rh = new ResyncHotp();
			rh.User = this.Username;
			rh.Org = this.Org;
			rh.Token1 = "359152";
			rh.Token2 = "969429";

			ResyncHotpRet rhret = (ResyncHotpRet)aeServerLogic.Actioner.Do(rh);
			Assert.IsTrue(string.IsNullOrEmpty(rhret.Error), rhret.Error);
			Assert.AreEqual(rhret.Out, 1);

			UserProviders uproviders = new UserProviders();
			uproviders.User = this.Username;
			uproviders.Org = this.Org;
			UserProvidersRet uprovidersret = (UserProvidersRet)aeServerLogic.Actioner.Do(uproviders);
			uprovidersret.Providers.Find(o => o.Name == "OATHCalc").Config.EndsWith(",4");
		}

		[Test]
		public void TestOATHCalcValidateUserHotpAfterValues()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			startAuthEngineServer();
			startOATHServer();

			UserProvider up = new UserProvider()
			{
				Name = "OATHCalc",
				Config = "HOTP,"
					+ HexConversion.ToString(Encoding.UTF8.GetBytes("12345678901234567890"))
					+ ",0"
			};
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
				new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
			Assert.AreEqual(1, ret.Out);

			ValidateUser vu = new ValidateUser();
			vu.User = this.OrgUsername;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("1", vuret.CreditsRemaining);
			ValidatePin vp = new ValidatePin();
			vp.User = this.OrgUsername;
			vp.Pin = "969429";
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
			Assert.IsTrue(vpret.Validated, "Validated is not true");

			UserProviders uproviders = new UserProviders();
			uproviders.User = this.Username;
			uproviders.Org = this.Org;
			UserProvidersRet uprovidersret = (UserProvidersRet)aeServerLogic.Actioner.Do(uproviders);
			uprovidersret.Providers.Find(o => o.Name == "OATHCalc").Config.EndsWith(",4");

			vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
			Assert.IsFalse(vpret.Validated, "Validated is true, we should be at n+1");
		}

		private string getPincode()
		{
			return new String('_', sc.AuthEnginePinCodeLength);
		}
		private string getToken()
		{
			return "755224";
		}


		private ValidateUserRet ValidateUserHotpWithToken(string pincode, string token)
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEngineDefaultEnabled = true;
			sc.AuthEnginePinCode = PinCodeOption.True;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			sc.AuthEngineChallengeResponse = false;
			startAuthEngineServer();
			startOATHServer();
			

			var user = insertUser(getPincode());

			UserProvider up = new UserProvider()
			{
				Name = "OATHCalc",
				Config = "HOTP,"
					+ HexConversion.ToString(Encoding.UTF8.GetBytes("12345678901234567890"))
					+ ",0"
			};
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
				new SetUserProvider() { User = user.User, Org = user.Org, Provider = up }));

			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
			Assert.AreEqual(1, ret.Out);

			var vu = new ValidateUser();

			vu.User = user.Org + @"\" + user.User;
			vu.PinCode = pincode + token;

			var vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.IsTrue(string.IsNullOrEmpty(vuret.State));
			return vuret;
		}

		[Test]
		public void TestOATHCalcValidateUserHotpWithToken()
		{
			var vuret = ValidateUserHotpWithToken(getPincode(), getToken());
			Assert.AreEqual(new AuthDisabledProviderLogic().Name, vuret.PName);
			Assert.AreEqual("1", vuret.CreditsRemaining);
			Assert.IsTrue(vuret.Validated);
		}

		[Test]
		public void TestOATHCalcValidateUserHotpFailWithTokenInvalidPincode()
		{
			var vuret = ValidateUserHotpWithToken(new String('I', sc.AuthEnginePinCodeLength), getToken());
			Assert.IsFalse(vuret.Validated);
		}

		[Test]
		public void TestOATHCalcValidateUserHotpFailWithTokenInvalidToken()
		{
			var vuret = ValidateUserHotpWithToken(getPincode(), "123456");
			Assert.IsFalse(vuret.Validated);
		}

		[Test]
		public void TestOATHCalcValidateUserHotpFailWithTokenInvalidPincodeAndToken()
		{
			var vuret = ValidateUserHotpWithToken(new String('I', sc.AuthEnginePinCodeLength), "123456");
			Assert.IsFalse(vuret.Validated);
		}
	}
	
	public partial class TestAuthEngine {
		[TestFixture]
		public class OathCalc : TestWithServers
		{
			private string getPincode()
			{
				return new String('_', sc.AuthEnginePinCodeLength);
			}
			private string getToken()
			{
				return "755224";
			}

			[Test]
			public void ShouldGetReplacementsInPostSelect() {
				var testRegistry = new Registry();
				var fakeUserGetter = new FakeUserGetterWithList();
				var fakeMailSender = new FakeMailSender();
				
				var afd = getInsertUserData("");

				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				sc.Providers.ForEach(x => x.Default = false);
				sc.Providers.ForEach(x => x.Enabled = false);
				var oath = sc.Providers.Find(x => x.Name == "OATHCalc");
				oath.AutoSetup = true;
				oath.Enabled = true;
				oath.Default = true;
				
				sc.AuthEngineChallengeResponse = false;
				
				startAuthEngineServer(testRegistry);
				
				aeServerLogic.updateMessage(new UpdateMessage {
				                            	Label = "OATH Setup E-mail",
				                            	Title = "yeap, the title",
												Message = @"
we have a {url} a {sharedsecret} a {passcode} a {username}
a {fullname} a {firstname} a {lastname} and... nothing else I guess... oh yes, the <img src=""cid:{attachment}""/>!
",
												External = false
				                            });
				aeServerLogic.AddFullUser(afd);
				
				var mail = fakeMailSender.MailsSent[0];
				Assert.That(mail.Template, Is.Not.StringContaining("{url}"));
				Assert.That(mail.Template, Is.Not.StringContaining("{sharedsecret}"));
				//Assert.That(mail.Template, Is.Not.StringContaining("{passcode}")); // OATH has no passcode..
				Assert.That(mail.Template, Is.Not.StringContaining("{username}"));
				Assert.That(mail.Template, Is.Not.StringContaining("{fullname}"));
				Assert.That(mail.Template, Is.Not.StringContaining("{firstname}"));
				Assert.That(mail.Template, Is.Not.StringContaining("{lastname}"));
				Assert.That(mail.Template, Is.StringContaining("otpauth://"));
				Assert.That(mail.Template, Is.StringContaining("here_should_go_the_attachment_cid"));
			}
			
			[Test]
			public void PeriodShouldntBeSetOnPostSelect() {				
				var testRegistry = new Registry();
				var fakeUserGetter = new FakeUserGetterWithList();
				var fakeMailSender = new FakeMailSender();
				
				var afd = getInsertUserData("");

				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				sc.Providers.ForEach(x => x.Default = false);
				sc.Providers.ForEach(x => x.Enabled = false);
				var oath = sc.Providers.Find(x => x.Name == "OATHCalc");
				oath.AutoSetup = true;
				oath.Enabled = true;
				oath.Default = true;
				var model = new HotpTotpModel {
					Type = HotpTotpModel.TOTP,
					Secret = "",
					CounterSkew = "0",
					DeviceName = "Default",
					Window = "30"
				};
				oath.Config = HotpTotpModel.Serialize(model);
								
				sc.AuthEngineChallengeResponse = false;
				
				startAuthEngineServer(testRegistry);
				
				aeServerLogic.updateMessage(new UpdateMessage {
				                            	Label = "OATH Setup E-mail",
				                            	Title = "yeap, the title",
												Message = @"
we have a {url} a {sharedsecret} a {passcode} a {username}
a {fullname} a {firstname} a {lastname} and... nothing else I guess... oh yes, the <img src=""cid:{attachment}""/>!
",
												External = false
				                            });
				aeServerLogic.AddFullUser(afd);
				
				var mail = fakeMailSender.MailsSent[0];
				Assert.That(mail.Template, Is.Not.StringContaining("period="));
				Assert.That(mail.Template, Is.StringContaining("otpauth://"));
			}
			
			[Test]
			public void ShouldFailNotFoundFeitian() {
				sc.AuthEngineUseEncryption = false;
				sc.CloudSMSUseEncryption = false;
				sc.OATHCalcUseEncryption = false;
				sc.AuthEngineDefaultEnabled = true;
				sc.AuthEnginePinCode = PinCodeOption.True;
				sc.AuthEngineChallengeResponse = false;
				startAuthEngineServer();
				startOATHServer();
				
				var user = insertUser(getPincode());
				
				var up = new UserProvider
				{
					Name = "OATHCalc",
					Config = "HOTP,"
						+ ",feitian:12345678901234567890"
						+ ",0"
				};
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = user.User, Org = user.Org, Provider = up }));
				
				Assert.IsFalse(string.IsNullOrEmpty(ret.Error), "Error response shoud not be empty.");
				Assert.AreEqual(0, ret.Out);
			}
			
			[Test]
			public void ShouldReplaceFeitian() {
				sc.AuthEngineUseEncryption = false;
				sc.CloudSMSUseEncryption = false;
				sc.OATHCalcUseEncryption = false;
				sc.AuthEngineDefaultEnabled = true;
				sc.AuthEnginePinCode = PinCodeOption.True;
				sc.AuthEngineChallengeResponse = false;
				startAuthEngineServer();
				startOATHServer();
				
				var user = insertUser(getPincode());
				
				using (var queries = DBQueriesProvider.Get())
				{
					queries.NonQuery("INSERT INTO [HardTokeN]([serial], [key]) VALUES('12345678901234567890', 'DEADBEEFDEADBEEFDEADBEEF')");
				}
				
				var up = new UserProvider()
				{
					Name = "OATHCalc",
					Config = "HOTP,"
						+ ",feitian:12345678901234567890"
						+ ",0"
				};
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = user.User, Org = user.Org, Provider = up }));
				
				Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
				Assert.AreEqual(1, ret.Out);
				
				var userId = QueryValSQL(string.Format("SELECT [ID] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = '{0}'", user.User))[0][0].ToString();
				var cfg = QueryValSQL(string.Format("SELECT [config] FROM [UserProviders] WHERE [authProviderId] = 2 AND [userId] = {0}", userId))[0][0].ToString();
				
				Assert.That(cfg, Is.StringContaining("DEADBEEFDEADBEEFDEADBEEF"));
			}
			
			[Test]
			public void ShouldReplaceSerial() {
				sc.AuthEngineUseEncryption = false;
				sc.CloudSMSUseEncryption = false;
				sc.OATHCalcUseEncryption = false;
				sc.AuthEngineDefaultEnabled = true;
				sc.AuthEnginePinCode = PinCodeOption.True;
				sc.AuthEngineChallengeResponse = false;
				startAuthEngineServer();
				startOATHServer();
				
				var user = insertUser(getPincode());
				
				using (var queries = DBQueriesProvider.Get())
				{
					queries.NonQuery("INSERT INTO [HardTokeN]([serial], [key]) VALUES('12345678901234567890', 'DEADBEEFDEADBEEFDEADBEEF')");
				}
				
				var up = new UserProvider()
				{
					Name = "OATHCalc",
					Config = "HOTP,"
						+ ",__serial:12345678901234567890"
						+ ",0"
				};
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = user.User, Org = user.Org, Provider = up }));
				
				Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
				Assert.AreEqual(1, ret.Out);
				
				var userId = QueryValSQL(string.Format("SELECT [ID] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = '{0}'", user.User))[0][0].ToString();
				var cfg = QueryValSQL(string.Format("SELECT [config] FROM [UserProviders] WHERE [authProviderId] = 2 AND [userId] = {0}", userId))[0][0].ToString();
				
				Assert.That(cfg, Is.StringContaining("DEADBEEFDEADBEEFDEADBEEF"));
			}
		}
	}
	
	public static class OATHCalcTests {
		[TestFixture]
		public class HotpTests : TestWithServers {
			[Test]
			public void ShouldResync() {
				startAuthEngineServer();
				startOATHServer();
				
				var user = insertUser();
				
				const string secret = "12345678901234567890";
				var encodedSecret = HexConversion.ToString(Encoding.UTF8.GetBytes(secret));
				
				var model = new HotpTotpModel {
					Type = HotpTotpModel.HOTP,
					Secret = encodedSecret,
					CounterSkew = "1",
					DeviceName = "Default"
				};
				var up = new UserProvider
				{
					Name = "OATHCalc",
					Config = HotpTotpModel.Serialize(model)
				};
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider { User = user.User, Org = user.Org, Provider = up }));
				
				Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response shoud be empty: " + ret.Error);				
				
				var resyncRet = (ResyncHotpRet)(aeServerLogic.ResyncUser(
					new ResyncHotp { 
						User = user.User
						, Org = user.Org				
						, Token1 = HotpTotp.HOTP(secret, 4)
						, Token2 = HotpTotp.HOTP(secret, 5)
					}));
				Assert.IsTrue(string.IsNullOrEmpty(resyncRet.Error), "Error response shoud be empty: " + resyncRet.Error);
				
				var userId = QueryValSQL(string.Format("SELECT [ID] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = '{0}'", user.User))[0][0].ToString();
				var cfg = QueryValSQL(string.Format("SELECT [config] FROM [UserProviders] WHERE [authProviderId] = 2 AND [userId] = {0}", userId))[0][0].ToString();
				
				Assert.That(cfg, Is.StringContaining(",6,"));
			}
		}
		
		[TestFixture]
		public class TotpTests : TestWithServers {
			[Test]
			public void ShouldResyncTwoWindows() {
				startAuthEngineServer();
				startOATHServer();
				
				var user = insertUser();
				
				const string secret = "12345678901234567890";
				var encodedSecret = HexConversion.ToString(Encoding.UTF8.GetBytes(secret));
				
				var model = new HotpTotpModel {
					Type = HotpTotpModel.TOTP,
					Secret = encodedSecret,
					CounterSkew = "0",
					DeviceName = "Default",
					Window = "60"
				};
				var up = new UserProvider
				{
					Name = "OATHCalc",
					Config = HotpTotpModel.Serialize(model)
				};
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider { User = user.User, Org = user.Org, Provider = up }));
				
				Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response shoud be empty: " + ret.Error);
				
				var dt = DateTime.UtcNow;
				var token1 = HotpTotp.TOTP(secret, dt.AddSeconds(70), 6, 60);
				var resyncRet = (ResyncHotpRet)(aeServerLogic.ResyncUser(
					new ResyncHotp { 
						User = user.User
						, Org = user.Org				
						, Token1 = token1
					}));
				Assert.IsTrue(string.IsNullOrEmpty(resyncRet.Error), "Error response shoud be empty: " + resyncRet.Error);
				
				var userId = QueryValSQL(string.Format("SELECT [ID] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = '{0}'", user.User))[0][0].ToString();
				var cfg = QueryValSQL(string.Format("SELECT [config] FROM [UserProviders] WHERE [authProviderId] = 2 AND [userId] = {0}", userId))[0][0].ToString();
				
				Assert.That(cfg, Is.StringContaining(",1,"));
			}
		}
	}
}
