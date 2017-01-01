using System;
using AuthGateway.AuthEngine.ProviderLogic;
using AuthGateway.Shared;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using AuthGateway.AuthEngine.Logic.ProviderLogic;

using NUnit.Framework;

namespace AuthGateway.AuthEngine.Unittest
{
	[TestFixture]
	public class TestAuthEngineCloudSMS : TestWithServers
	{
		[Test]
		public void TestSendSMS()
		{
			var passwordParameter = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("Password");
			var password = string.Empty;
			if (passwordParameter != null)
				password = passwordParameter.Value;

			if (password == "123456")
				Assert.Inconclusive("CloudSMS test config not configured");
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			startAuthEngineServer();
			startCloudSMSServer();
			var pl = (CloudSMSProviderLogic)new CloudSMSProviderLogic().Using(sc);
			pl.SendSMSRequest("00541169531549", "Test Message", "code", "");
			Assert.IsFalse(counterLogger.HasError(), "No errors expected");
		}

		[Test]
		public void TestSendSMSUnencrypted()
		{
			var passwordParameter = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleParameters.GetByName("Password");
			var password = string.Empty;
			if (passwordParameter != null)
				password = passwordParameter.Value;
			if (password == "123456")
				Assert.Inconclusive("CloudSMS test config not configured");
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			startAuthEngineServer();
			startCloudSMSServer();
			var pl = (CloudSMSProviderLogic)new CloudSMSProviderLogic().Using(sc);
			pl.SendSMSRequest("NUMBER", "Test Message", "code", "");
			Assert.IsFalse(counterLogger.HasError(), "No errors expected");
		}

		[Test]
		public void TestCloudSMSValidateUser()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			startAuthEngineServer();
			startCloudSMSServer();
			UserProvider up = new UserProvider() { Name = "CloudSMS", Config = "" };
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
			Assert.AreEqual(1, ret.Out);

			ValidateUser vu = new ValidateUser();
			vu.User = this.OrgUsername;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("7357", vuret.CreditsRemaining);
			ValidatePin vp = new ValidatePin();
			vp.User = this.OrgUsername;
			vp.Pin = vuret.Error;
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(vpret.Validated);
		}

		[Test]
		public void TestCloudSMSSendToken()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineChallengeResponse = false;
			sc.BaseSendTokenTestMode = true;
			startAuthEngineServer();
			startCloudSMSServer();

			var user = insertUser();

			UserProvider up = new UserProvider() { Name = "CloudSMS", Config = "" };
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = user.User, Org = user.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
			Assert.AreEqual(1, ret.Out);

			sendToken(user);
		}

		[Test]
		public void TestCloudSMSSendEmergencyToken()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineChallengeResponse = false;
			sc.BaseSendTokenTestMode = true;
			startAuthEngineServer();
			startCloudSMSServer();

			var user = insertAdminUser();

			UserProvider up = new UserProvider() { Name = "CloudSMS", Config = "" };
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = user.User, Org = user.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
			Assert.AreEqual(1, ret.Out);

			var cmd = new SendToken();
			cmd.Identity = new FakeIdentity(user);
			cmd.Authenticated = true;
			cmd.User = user.User;
			cmd.Org = user.Org;
			cmd.Emergency = true;

			var cmdret = (SendTokenRet)aeServerLogic.Actioner.Do(cmd);
			Assert.IsTrue(Int32.Parse(cmdret.Pin) > 0);
			Assert.IsTrue(string.IsNullOrEmpty(cmdret.Error));
		}

		private string sendToken(AddFullDetails user)
		{
			var cmd = new SendToken();
			cmd.Identity = new FakeIdentity(user);
			cmd.User = user.User;
			cmd.Org = user.Org;

			var cmdret = (SendTokenRet)aeServerLogic.Actioner.Do(cmd);

			var token = 0;
			try
			{
				token = Convert.ToInt32(cmdret.Error);
			}
			catch
			{

			}
			Assert.AreNotEqual(0, token, "We should have received new token in .Error property");
			return cmdret.Error;
		}

		[Test]
		public void TestCloudSMSSendTokenFailNotStateless()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineChallengeResponse = true;
			sc.BaseSendTokenTestMode = true;
			startAuthEngineServer();
			startCloudSMSServer();

			var user = insertUser();

			UserProvider up = new UserProvider() { Name = "CloudSMS", Config = "" };
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = user.User, Org = user.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
			Assert.AreEqual(1, ret.Out);

			var cmd = new SendToken();
			cmd.Identity = new FakeIdentity(user);
			cmd.User = user.User;
			cmd.Org = user.Org;

			var cmdret = (SendTokenRet)aeServerLogic.Actioner.Do(cmd);
			Assert.IsFalse(string.IsNullOrEmpty(cmdret.Error));
			Assert.IsTrue(cmdret.Error.Contains("ahead"));
		}

		[Test]
		public void TestCloudSMSValidateUserWithPinAndNotChallenge()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;

			sc.AuthEnginePinCode = PinCodeOption.True;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			
			sc.AuthEngineChallengeResponse = false;

			sc.CloudSMSConfiguration.CloudSMSModules[0] = new CloudSMSModuleConfig()
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

			sc.BaseSendTokenTestMode = true;
			startAuthEngineServer();
			startCloudSMSServer();
			var pincode = "123456";
			var user = insertUser(pincode);

			UserProvider up = new UserProvider() { Name = "CloudSMS", Config = "" };
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = user.User, Org = user.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
			Assert.AreEqual(1, ret.Out);

			var token = sendToken(user);

			ValidateUser vu = new ValidateUser();
			vu.User = user.Org + @"\" + user.User;
			vu.PinCode = pincode + token.ToString();

			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual(AuthDisabledProviderLogic.LogicName, vuret.PName);

			var newtoken = 0;
			try
			{
				newtoken = Convert.ToInt32(vuret.Error);
			}
			catch
			{

			}
			Assert.AreNotEqual(0, newtoken, "We should have received new token in .Error property");
			Assert.AreNotEqual(token, vuret.Error, "We should have received a new token instead of old one.");
		}

		[Test]
		public void TestCloudSMSValidateUserWithPinAndNotChallengeFailWrongToken()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDefaultEnabled = true;

			sc.AuthEngineChallengeResponse = false;

			sc.BaseSendTokenTestMode = true;

			startAuthEngineServer();
			startCloudSMSServer();
			var pincode = "123456";
			var user = insertUser(CryptoHelper.HashPincode(pincode));

			UserProvider up = new UserProvider() { Name = "CloudSMS", Config = "" };
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = user.User, Org = user.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
			Assert.AreEqual(1, ret.Out);

			var token = sendToken(user);

			ValidateUser vu = new ValidateUser();
			vu.User = user.Org + @"\" + user.User;
			var wrongToken = new String('0', token.ToString().Length);
			vu.PinCode = pincode + wrongToken;

			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.IsFalse(string.IsNullOrEmpty(vuret.Error));
			Assert.IsFalse(vuret.Validated);
		}
		
		[Test]
		public void TestCloudSMSValidateUserWithPinAndNotChallengeWorks()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDefaultEnabled = true;

			sc.AuthEngineChallengeResponse = false;
			sc.AuthEnginePinCode = PinCodeOption.True;

			sc.BaseSendTokenTestMode = true;

			startAuthEngineServer();
			startCloudSMSServer();
			var pincode = "123456";
			var user = insertUser((pincode));

			UserProvider up = new UserProvider() { Name = "CloudSMS", Config = "" };
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = user.User, Org = user.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
			Assert.AreEqual(1, ret.Out);

			var token = sendToken(user);

			ValidateUser vu = new ValidateUser();
			vu.User = user.Org + @"\" + user.User;
			vu.PinCode = pincode + token;

			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.IsFalse(string.IsNullOrEmpty(vuret.Error));
			Assert.IsTrue(vuret.Validated);
		}

		[Test]
		public void TestCloudSMSValidateUserPreviousTokenShouldBeInvalidAndNextShouldBeValid()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			startAuthEngineServer();
			startCloudSMSServer();

			UserProvider up = new UserProvider() { Name = "CloudSMS", Config = "" };
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
			Assert.AreEqual(1, ret.Out);

			ValidateUser vu = new ValidateUser();
			vu.User = this.OrgUsername;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			var pin1 = vuret.Error;
			var state1 = vuret.State;

			vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			var pin2 = vuret.Error;
			var state2 = vuret.State;

			vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			var pin3 = vuret.Error;
			var state3 = vuret.State;


			ValidatePin vp = new ValidatePin();
			vp.User = this.OrgUsername;

			vp.State = state2;
			vp.Pin = pin2;
			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(vpret.Validated);

			vp.State = state3;
			vp.Pin = pin3;
			vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(vpret.Validated);

			vp.State = state1;
			vp.Pin = pin1;
			vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsFalse(vpret.Validated);
		}
	}
}
