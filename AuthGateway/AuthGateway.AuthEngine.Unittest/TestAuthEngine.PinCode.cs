using System.Text;
using System.Linq;
using AuthGateway.Shared;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;

using NUnit.Framework;

namespace AuthGateway.AuthEngine.Unittest
{
	[TestFixture]
	public class TestAuthEnginePinCode : TestWithServers
	{
		[Test]
		public void ValidateUserPinCodeOffIsValidated()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEnginePinCode = PinCodeOption.False;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			startAuthEngineServer();
			startOATHServer();

			var user = insertUser(string.Empty);

			var ufdret = setupTestUserWithoutPincode(user);
			Assert.IsTrue(string.IsNullOrEmpty(ufdret.Error), "Error response is not empty: " + ufdret.Error);

			ValidateUser vu = new ValidateUser();
			vu.User = user.Org + "\\" + user.User;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.IsTrue(string.IsNullOrEmpty(vuret.Error), "Error response is not empty: " + vuret.Error);
			Assert.IsTrue(vuret.Validated, "Validated is not true");
		}

		[Test]
		public void ValidatePinCodeOff()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEnginePinCode = PinCodeOption.False;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			startAuthEngineServer();
			startOATHServer();

			var user = insertUser(string.Empty);

			var ufdret = setupTestUserWithoutPincode(user);
			Assert.IsTrue(string.IsNullOrEmpty(ufdret.Error), "Error response is not empty: " + ufdret.Error);

			ValidateUser vu = new ValidateUser();
			vu.User = user.Org + "\\" + user.User;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.IsTrue(string.IsNullOrEmpty(vuret.Error), "An unexpected error in validateUser: " + vuret.Error);
			Assert.AreEqual("1", vuret.CreditsRemaining);
			Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
			ValidatePin vp = new ValidatePin();
			vp.User = user.Org + "\\" + user.User;
			vp.Pin = "755224";
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
			Assert.IsTrue(vpret.Validated, "Validated is not true");
		}
		
		[Test]
		public void ValidatePinCodeEmptyAfterSettingToTrue()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEnginePinCode = PinCodeOption.False;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			startAuthEngineServer();
			startOATHServer();

			var user = insertUser(string.Empty);

			var ufdret = setupTestUserWithoutPincode(user);
			Assert.IsTrue(string.IsNullOrEmpty(ufdret.Error), "Error response is not empty: " + ufdret.Error);

			for(var i = 1; i<3; i++) {
				ValidateUser vu = new ValidateUser();
				vu.User = user.Org + "\\" + user.User;
				ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
				Assert.IsTrue(string.IsNullOrEmpty(vuret.Error), "An unexpected error in validateUser: " + i + vuret.Error);
				Assert.AreEqual("1", vuret.CreditsRemaining);
				Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
				ValidatePin vp = new ValidatePin();
				vp.User = user.Org + "\\" + user.User;
				vp.Pin = "755224";
				if (i==2)
					vp.Pin = "287082";
				vp.State = vuret.State;
				if (i==2)
					System.Console.Write("break");
				ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
				Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + i + vpret.Error);
				Assert.IsTrue(vpret.Validated, "Validated is not true " + i);
				
				if (i==1) {
					StopAuthEngineAndCloudSMS();
					sc.AuthEnginePinCode = PinCodeOption.True;
					startAuthEngineServer();
					startOATHServer();
					System.Threading.Thread.Sleep(31*1000);
				}
			}
		}

		[Test]
		public void ValidatePinCodeOffWithPincodeSet()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEnginePinCode = PinCodeOption.False;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			startAuthEngineServer();
			startOATHServer();

			var user = insertUser(string.Empty);

			var ufdret = setupTestUserWithPincode(user, "test");
			Assert.IsTrue(string.IsNullOrEmpty(ufdret.Error), "Error response is not empty: " + ufdret.Error);

			ValidateUser vu = new ValidateUser();
			vu.User = user.Org + "\\" + user.User;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.IsTrue(string.IsNullOrEmpty(vuret.Error), "An unexpected error in validateUser: " + vuret.Error);
			Assert.AreEqual("1", vuret.CreditsRemaining);
			Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
			ValidatePin vp = new ValidatePin();
			vp.User = user.Org + "\\" + user.User;
			vp.Pin = "755224";
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
			Assert.IsTrue(vpret.Validated, "Validated is not true");
		}

		[Test]
		public void ValidatePinCodeOnAndSetNotMatchedShouldFail()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEnginePinCode = PinCodeOption.True;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			startAuthEngineServer();
			startOATHServer();
			
			var user = insertUser(string.Empty);

			var ufdret = setupTestUserWithPincode(user, "pc1234");
			Assert.IsTrue(string.IsNullOrEmpty(ufdret.Error), "Error response is not empty: " + ufdret.Error);

			ValidateUser vu = new ValidateUser();
			vu.User = user.Org + "\\" + user.User;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
			ValidatePin vp = new ValidatePin();
			vp.User = user.Org + "\\" + user.User;
			vp.Pin = "755224";
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsFalse(string.IsNullOrEmpty(vpret.Error), "Error response should be empty: " + vpret.Error);
			Assert.IsFalse(vpret.Validated, "Should not validate");
		}

        [Test]
        public void ValidatePinCodePanicMode()
        {
            sc.AuthEngineUseEncryption = false;
            sc.CloudSMSUseEncryption = false;
            sc.OATHCalcUseEncryption = false;
            sc.AuthEnginePinCode = PinCodeOption.True;
            ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
            sc.AuthEnginePinCodePanic = true;
            ServerLogic.setSetting("AESETTING", "AuthEnginePinCodePanic", sc.AuthEnginePinCodePanic.ToString());
            startAuthEngineServer();
            startOATHServer();

            // create user
            var user = insertUser(string.Empty);

            var ufdret = setupTestUserWithPincode(user, "pc1234");
            Assert.IsTrue(string.IsNullOrEmpty(ufdret.Error), "Error response is not empty: " + ufdret.Error);

            string pincode = "pc1234";
            string reversedPincode = new string(pincode.Reverse().ToArray());

            // try to log in with a normal pin
            
            ValidateUser vu = new ValidateUser();
            vu.User = user.Org + "\\" + user.User;
            vu.PinCode = pincode;
            ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
            Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
            ValidatePin vp = new ValidatePin();
            vp.User = user.Org + "\\" + user.User;
            vp.Pin = "755224";
            vp.State = vuret.State;

            ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
            Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response should be empty: " + vpret.Error);
            Assert.IsTrue(vpret.Validated);
            Assert.IsFalse(vpret.Panic);            

            // try to login with pin reversed            
            ValidateUser vUserReversedPin = new ValidateUser();
            vUserReversedPin.User = user.Org + "\\" + user.User;
            vUserReversedPin.PinCode = reversedPincode;
            ValidateUserRet vUserReversedPinRet = (ValidateUserRet)aeServerLogic.Actioner.Do(vUserReversedPin);
            Assert.IsFalse(string.IsNullOrEmpty(vUserReversedPinRet.State));
            ValidatePin vReversedPin = new ValidatePin();
            vReversedPin.User = user.Org + "\\" + user.User;
            vReversedPin.Pin = "755224";
            vReversedPin.State = vUserReversedPinRet.State;

            ValidatePinRet vReversedPinRet = (ValidatePinRet)aeServerLogic.Actioner.Do(vReversedPin);
            Assert.IsTrue(string.IsNullOrEmpty(vReversedPinRet.Error), "Error response should be empty: " + vReversedPinRet.Error);
            Assert.IsTrue(vReversedPinRet.Panic);
            
            // try to login with a normal pin

            ValidateUser vu1 = new ValidateUser();
            vu1.User = user.Org + "\\" + user.User;
            vu1.PinCode = pincode;
            ValidateUserRet vuret1 = (ValidateUserRet)aeServerLogic.Actioner.Do(vu1);            
            Assert.IsFalse(vuret1.Validated);
        }

		[Test]
		public void ValidatePinCodeOnAndEmpty()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEnginePinCode = PinCodeOption.True;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			startAuthEngineServer();
			startOATHServer();

			var user = insertUser(string.Empty);

			var ufdret = setupTestUserWithPincode(user, "");
			Assert.IsTrue(string.IsNullOrEmpty(ufdret.Error), "Error response is not empty: " + ufdret.Error);

			ValidateUser vu = new ValidateUser();
			vu.User = user.Org + "\\" + user.User;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("1", vuret.CreditsRemaining);
			Assert.IsFalse(string.IsNullOrEmpty(vuret.State));

			ValidatePin vp = new ValidatePin();
			vp.User = user.Org + "\\" + user.User;
			vp.Pin = "755224";
			vp.State = vuret.State;
			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is not empty: " + vpret.Error);
			Assert.IsTrue(vpret.Validated, "Validated is not true");
		}
		
		[Test]
		public void ValidatePinCodeOnAndEmptyNoChallenge()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEngineChallengeResponse = false; 
			sc.AuthEnginePinCode = PinCodeOption.True;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			startAuthEngineServer();
			startOATHServer();

			var user = insertUser(null);

			var ufdret = setupTestUserWithPincode(user, "");
			Assert.IsTrue(string.IsNullOrEmpty(ufdret.Error), "Error response is not empty: " + ufdret.Error);

			ValidateUser vu = new ValidateUser();
			vu.User = user.Org + "\\" + user.User;
			vu.PinCode = "755224";
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.IsTrue(string.IsNullOrEmpty(vuret.Error), "Error response is not empty: " + vuret.Error);
			Assert.IsTrue(vuret.Validated, "Validated is not true");
			Assert.AreEqual("1", vuret.CreditsRemaining);
		}

		[Test]
		public void TestUpdateUserWithPincodeEnforcedAndSetToEmptyShouldFail()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEnginePinCode = PinCodeOption.Enforced;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			startAuthEngineServer();
			startOATHServer();

			var user = insertUser(string.Empty);

			var ufdret = setupTestUserWithPincode(user, "");
			Assert.AreEqual("Pincode field cannot be empty.", ufdret.Error);
		}

		[Test]
		public void ValidatePinCodeOnAndSetShouldValidateInValidateUser()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEnginePinCode = PinCodeOption.True;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			startAuthEngineServer();
			startOATHServer();

			var user = insertUser(string.Empty);

			var ufdret = setupTestUserWithPincode(user, "pc1234");
			Assert.IsTrue(string.IsNullOrEmpty(ufdret.Error), "Error response is not empty: " + ufdret.Error);

			ValidateUser vu = new ValidateUser();
			vu.PinCode = "pc1234";
			vu.User = user.Org + "\\" + user.User;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("1", vuret.CreditsRemaining);
			Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
			ValidatePin vp = new ValidatePin();
			vp.User = user.Org + "\\" + user.User;
			vp.Pin = "755224";
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response was not excepted: " + vpret.Error);
			Assert.IsTrue(vpret.Validated, "Should validate");
		}

		[Test]
		public void ValidatePinCodeOnAndSetShouldValidateInValidatePin()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEnginePinCode = PinCodeOption.True;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			startAuthEngineServer();
			startOATHServer();

			var user = insertUser(string.Empty);

			var ufdret = setupTestUserWithPincode(user, "pc1234");
			Assert.IsTrue(string.IsNullOrEmpty(ufdret.Error), "Error response is not empty: " + ufdret.Error);

			ValidateUser vu = new ValidateUser();
			vu.User = user.Org + "\\" + user.User;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
			ValidatePin vp = new ValidatePin();
			vp.User = user.Org + "\\" + user.User;
			vp.Pin = "755224";
			vp.PinCode = "pc1234";
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response was not excepted: " + vpret.Error);
			Assert.IsTrue(vpret.Validated, "Should validate");
		}
	}
}
