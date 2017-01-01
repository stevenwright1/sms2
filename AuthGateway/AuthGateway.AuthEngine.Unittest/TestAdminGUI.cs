using System;
using System.Collections.Generic;
using AuthGateway.AdminGUI;
using AuthGateway.AuthEngine.Logic.ProviderLogic;
using AuthGateway.Shared;
using AuthGateway.Shared.Identity;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using MigraDoc.Rendering;
using NUnit.Framework;

namespace AuthGateway.AuthEngine.Unittest
{
	[TestFixture]
	public class TestAdminGUI : TestWithServers
	{
		[Test]
		public void TestAdminGUITokens()
		{
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			startAuthEngineServer();
			startCloudSMSServer();
			Variables clientLogic = new Variables(sc);
			TokensRet tr = clientLogic.GetTokens();
			Assert.IsTrue(string.IsNullOrEmpty(tr.Error));
		}

		[Test]
		public void TestAdminGUITokensEncrypted()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			startAuthEngineServer();
			startCloudSMSServer();
			Variables clientLogic = new Variables(sc);
			clientLogic.LoadRsaProvider(sc);
			TokensRet tr = clientLogic.GetTokens();
			Assert.IsTrue(string.IsNullOrEmpty(tr.Error));
		}

		[Test]
		public void TestAdminGUIUserProviders()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			startAuthEngineServer();
			startCloudSMSServer();
			Variables clientLogic = new Variables(sc);
			clientLogic.LoadRsaProvider(sc);
			UserProvidersRet tr = clientLogic.GetProviders(CurrentIdentity.GetLogin(), CurrentIdentity.GetDomain());
			Assert.IsTrue(string.IsNullOrEmpty(tr.Error));
			Assert.AreEqual(7, tr.Providers.Count, string.Format("Expected {0} providers", 7));
		}

		[Test]
		[Ignore]
		public void TestAdminGUIUsers()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			//DBQueriesProvider.SetQueriesProvider(new DBQueriesProviderNew(sc.GetSQLConnectionString(true)));
			startAuthEngineServer();

			var testUsers = 10000;
			Logger.Instance.AddLogger(new FileLogger(".", "testAdminGUIUsersLog"), LogLevel.All);

			Variables clientLogic = new Variables(sc);
			var retCurrentUsers = clientLogic.GetUsers(0, testUsers * 2, false, string.Empty, true, string.Empty);
			Assert.IsTrue(string.IsNullOrEmpty(retCurrentUsers.Error), "ret.Error: " + retCurrentUsers.Error);

			var currentUsers = retCurrentUsers.Users.Count;

			var users = new List<AddFullDetails>();
			insertXUsers(users, testUsers, this.Org);
			Assert.AreEqual(testUsers, users.Count);

			aeServerLogic.InsertOrUpdateUsers(users);

			var ret = clientLogic.GetUsers(0, testUsers * 2, false, string.Empty, true, string.Empty);
			Assert.IsFalse(string.IsNullOrEmpty(ret.Error), "ret.Error should have been populated");

			var untilClear = clientLogic.loadUsersUntilClear(false, string.Empty, true, string.Empty);
			Assert.AreEqual(testUsers + currentUsers, untilClear.Count);
		}

		[Test]
		public void TestAdminGUISendToken()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineChallengeResponse = false;
			startAuthEngineServer();
			startCloudSMSServer();
			Variables clientLogic = new Variables(sc);
			clientLogic.LoadRsaProvider(sc);
			UserProvider up = new UserProvider();
			up.Name = "CloudSMS";
			up.Config = "";
			SetUserProviderRet ret = clientLogic.SetProvider(CurrentIdentity.GetLogin(), CurrentIdentity.GetDomain(), up);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
			Assert.AreEqual(1, ret.Out);
			
			var stret = clientLogic.SendToken(CurrentIdentity.GetLogin(), CurrentIdentity.GetDomain());
			var tokenInt = 0;
			if (Int32.TryParse(stret.Error, out tokenInt))
			{
				stret.Error = string.Empty;
			}
			else
			{
				Assert.Fail("Error should have been the token.");
			}
			Assert.IsTrue(string.IsNullOrEmpty(stret.Error), "Error is: " + stret.Error);
		}
		[Test]
		public void TestAdminGUISendEmergencyToken()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineChallengeResponse = false;
			startAuthEngineServer();
			startCloudSMSServer();

			Variables clientLogic = new Variables(sc);
			clientLogic.LoadRsaProvider(sc);
			UserProvider up = new UserProvider();
			up.Name = "CloudSMS";
			up.Config = "";
			SetUserProviderRet ret = clientLogic.SetProvider(CurrentIdentity.GetLogin(), CurrentIdentity.GetDomain(), up);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
			Assert.AreEqual(1, ret.Out);

			RunSQL(string.Format("UPDATE SMS_CONTACT SET User_Type = '{1}' WHERE AD_USERNAME='{0}'", CurrentIdentity.GetLogin(), UserType.Administrator));
			
			var stret = clientLogic.SendToken(CurrentIdentity.GetLogin(), CurrentIdentity.GetDomain(), true);
			Assert.IsTrue(Int32.Parse(stret.Pin) > 0);
			Assert.IsTrue(string.IsNullOrEmpty(stret.Error));
		}

		[Test]
		public void TestAdminGUISendEmergencyTokenFailsNotAdmin()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineChallengeResponse = false;
			startAuthEngineServer();
			startCloudSMSServer();

			Variables clientLogic = new Variables(sc);
			clientLogic.LoadRsaProvider(sc);
			UserProvider up = new UserProvider();
			up.Name = "CloudSMS";
			up.Config = "";
			SetUserProviderRet ret = clientLogic.SetProvider(CurrentIdentity.GetLogin(), CurrentIdentity.GetDomain(), up);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
			Assert.AreEqual(1, ret.Out);

			RunSQL(string.Format("UPDATE SMS_CONTACT SET User_Type = '{1}' WHERE AD_USERNAME='{0}'", CurrentIdentity.GetLogin(), UserType.User));
			
			var stret = clientLogic.SendToken(CurrentIdentity.GetLogin(), CurrentIdentity.GetDomain(), true);
			Assert.That(stret.Error, Is.Not.Null.Or.Empty);
		}

		[Test]
		public void TestAdminGUIPrintSheet()
		{
			sc.AuthEngineDefaultEnabled = true;
			sc.AuthEngineUseEncryption = false;
			startAuthEngineServer();
			var user = insertUser(string.Empty);
			setupTestUserWithoutPincode(user);

			var up = new UserProvider()
			{
				Name = "PINTAN",
				Config = ","
					+ ""
					+ ","
			};
			aeServerLogic.SetUserProvider(new SetUserProvider() { User = user.User, Org = user.Org, Provider = up });
			
			Variables clientLogic = new Variables(sc);
			var ret = clientLogic.ResyncProvider(user.User, user.Org, PINTANProviderLogic.ACTION_GENERATESHEETS, string.Empty, string.Empty, string.Empty);
			var ucpintan = new ucPINTAN();
			var doc = ucpintan.ProcessPINTANData(TextHelper.ParsePipeML(ret.Extra));
			var renderer = ucpintan.GetDocumentPdfRenderer(doc);
			string filename = "test.pdf";
			renderer.Save(filename);
		}

		[Test]
		public void TestAdminGUIGetModules()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDisableUserVerification = true;

			startAuthEngineServer();
			startCloudSMSServer();

			var user = insertUser();
			setupTestUserWithoutPincode(user);

			var clientLogic = new Variables(sc);
			clientLogic.LoadRsaProvider(sc);

			var gamr = clientLogic.GetModules(user.User, user.Org);
			Assert.AreEqual(1, gamr.Count);
		}
		
		[Test]
		public void TestAdminGUISetPincodeTrueNotEmptyModifies() {
			sc.AuthEngineDisableUserVerification = true;
			sc.AuthEnginePinCode = PinCodeOption.True;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			
			startAuthEngineServer();
			
			var user = insertUser("testPinCodeModifiedNOTMATCHING");
			
			var clientLogic = new Variables(sc);
			clientLogic.LoadRsaProvider(sc);
			var ret = clientLogic.UpdateDetails("test.user", "2234","2344","test user lastname", "firstname","User", 
			                          "testdomain", true, "testPi", true, "email", false, "test.user@testdomain");
			Assert.That(ret.Error, Is.Null);
			var pincode = QueryValSQL("SELECT pincode FROM SMS_CONTACT WHERE AD_USERNAME='test.user'");
			Assert.That((string)pincode[0][0], Is.EqualTo(CryptoHelper.HashPincode("testPi")));
		}

		[Test]
		public void TestAdminGUIPincodeChangedFalseDoesNotModify()
		{
			sc.AuthEngineDisableUserVerification = true;
			sc.AuthEnginePinCode = PinCodeOption.True;
			startAuthEngineServer();

			var user = insertUser("testPi");

			var clientLogic = new Variables(sc);
			clientLogic.LoadRsaProvider(sc);
			var ret = clientLogic.UpdateDetails("test.user", "2234", "2344", "test user lastname", "firstname", "User",
                                      "testdomain", true, "testPiSARLANGA", false, "email", false, "test.user@testdomain");
			Assert.That(ret.Error, Is.Null);
			var pincode = QueryValSQL("SELECT pincode FROM SMS_CONTACT WHERE AD_USERNAME='test.user'");
			Assert.That((string)pincode[0][0], Is.EqualTo(CryptoHelper.HashPincode("testPi")));
		}
	}
	
	public class ExtendedOath : ucOATH
	{
		public ExtendedOath(ProviderConfigContainer pcCont)
			: base(pcCont)
		{
			
		}
		public void addConfig(string name, string config) {
			this.addConfigDevice(name, config);
		}
		public void Del() 
		{
			this.btnDel_Click(null, null);
		}
		public void setSecret(string text)
		{
			this.setSharedSecret(text);
		}
		public string getSecret()
		{
			return this.getSharedSecret();
		}
	}
	
	public class DummyProviderConfigContainer : ProviderConfigContainer
	{
		private string Org;
		private string User;
		private Variables ClientLogic;
		public ProviderConfig pc { get; set; }
		public DummyProviderConfigContainer(string org, string user, Variables clientLogic ) {
			this.Org = org;
			this.User = user;
			this.ClientLogic = clientLogic;
		}
		
		public string getDomain()
		{
			return Org;
		}
		
		public string getUser()
		{
			return User;
		}
		
		public System.Windows.Forms.Control getControl()
		{
			throw new NotImplementedException();
		}
		
		public Variables getClientLogic()
		{
			return ClientLogic;
		}
		
		public List<UserProvider> getProviders()
		{
			throw new NotImplementedException();
		}
		
		public bool isSelectedProvider(string name)
		{
			throw new NotImplementedException();
		}
		
		public void ShowError(string message)
		{
			throw new NotImplementedException();
		}
		
		public void SaveSelectedProvider(bool validate)
		{
			pc.BeforeSave();
			var uProvider = new UserProvider() 
			{
				Name = pc.getName(), 
				Enabled = true, 
				Selected = true,
				Config = pc.getConfig()
			};
			var ret = ClientLogic.SetProvider(getUser(), getDomain(), uProvider);
			Assert.That(ret.Error, Is.Null.Or.Empty);
		}
		
		public void ShowWarning(string message)
		{
			throw new NotImplementedException();
		}
		
		public bool ShowConfirm(string message)
		{
			return true;
		}
	}
}
