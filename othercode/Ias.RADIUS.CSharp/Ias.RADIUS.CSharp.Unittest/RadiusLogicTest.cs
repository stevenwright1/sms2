using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using AuthGateway.AuthEngine;
using AuthGateway.AuthEngine.Logic;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.AuthEngine.Logic.Helpers;
using AuthGateway.AuthEngine.Unittest;
using AuthGateway.Shared;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using NUnit.Framework;

namespace Ias.RADIUS.CSharp.Unittest
{
	
	
	public static class RadiusLogicTest
	{
		[TestFixture]
		public class TestAuth : RadiusBaseTest
		{
			[Test]
			public void SimpleOath5Times()
			{
				SetupRadiusLogicTest();
				for (var i = 0; i < 5; i++)
				{
					var rchal = backend.ActOn(OrgUsername, "", string.Empty);
					Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rchal.status, rchal.message);
					var rauth = backend.ActOn(OrgUsername, rchal.message, rchal.state);
					Assert.AreEqual(RadiusLogicBackend.ACCEPT, rauth.status, string.Format("Iteration {0} : {1}", i, rchal.message));
					Thread.Sleep(sc.MinTimeBetweenRadiusRequestsPerUser + 100);
				}
			}

			[Test]
			public void AuthOneScreen()
			{
				SetupRadiusLogicTest(false);
				sc.AuthEngineChallengeResponse = false;
				startAuthEngineServer();
				startCloudSMSServer();
				backend = new RadiusLogicBackend(sc);

				var afdUser = insertUser("qweasd");
				string token;
				ActOnRet rauth;
				for (var i = 0; i < 4; i++)
				{
					token = sendToken(afdUser);
					rauth = backend.ActOn(afdUser.Org + @"\" + afdUser.User, "qweasd" + token, string.Empty);
					Assert.AreEqual(RadiusLogicBackend.ACCEPTNOSTATE, rauth.status, rauth.message);
				}
			}

			[Test]
			public void ShouldFail()
			{
				SetupRadiusLogicTest(false);
				
				backend = new RadiusLogicBackend(sc);

				var afdUser = insertUser("qweasd");
				ActOnRet rauth;
				rauth = backend.ActOn(afdUser.Org + @"\" + afdUser.User, "qweasd", string.Empty);
				Assert.AreEqual(0, rauth.status);
				Assert.IsFalse(string.IsNullOrEmpty(rauth.message), rauth.message);
				Assert.IsNotNull(rauth.state);

				Thread.Sleep(sc.MinTimeBetweenRadiusRequestsPerUser + 50);
				rauth = backend.ActOn(afdUser.Org + @"\" + afdUser.User, "qweasd", "test");
				Assert.AreEqual(0, rauth.status);
				Assert.IsFalse(string.IsNullOrEmpty(rauth.message), rauth.message);
				Assert.IsNotNull(rauth.state);

				startAuthEngineServer();
				startCloudSMSServer();

				var rchal = backend.ActOn(OrgUsername, "test", string.Empty);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rchal.status, rchal.message);

				StopAuthEngineAndCloudSMS();

				rauth = backend.ActOn(OrgUsername, rchal.message, rchal.state);
				Assert.AreEqual(0, rauth.status);
				Assert.IsFalse(string.IsNullOrEmpty(rauth.message));
				Assert.IsNotNull(rauth.state);
			}

			[Test]
			public void AuthPinTan()
			{
				SetupRadiusLogicTest(false);

				startAuthEngineServer();
				startCloudSMSServer();
				
				backend = new RadiusLogicBackend(sc);

				var usersheets = new TestUserSheets(sc, this);
				usersheets.generateSheets();
				
				var orgUsername = usersheets.User.Org + @"\" + usersheets.User.User;
				for (var i = 0; i < 5; i++)
				{

					var rchal = backend.ActOn(orgUsername, "", string.Empty);
					Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rchal.status, rchal.message);

					var coord = rchal.message.Remove(0, rchal.message.IndexOf("Provide value for '") + "Provide value for '".Length);
					coord = coord.Remove(coord.LastIndexOf('\''));

					var col = TextHelper.String2Number(coord.Substring(0, 1));
					var row = Convert.ToInt32(coord.Substring(1));

					var pin = usersheets.getCode(col, row);

					var rauth = backend.ActOn(orgUsername, pin, rchal.state);
					Assert.AreEqual(RadiusLogicBackend.ACCEPT, rauth.status, string.Format("Iteration {0} : {1}", i, rauth.message));

					Thread.Sleep(sc.MinTimeBetweenRadiusRequestsPerUser + 2);
				}
			}
		}
		
		[TestFixture]
		public class Flood : RadiusBaseTest
		{
			[Test]
			public void GivesSameState()
			{
				SetupRadiusLogicTest(false);
				
				sc.MinTimeBetweenRadiusRequestsPerUser = 2000000;
				startAuthEngineServer();
				startCloudSMSServer();
				
				backend = new RadiusLogicBackend(sc);
				
				ActOnRet r;
				r = backend.ActOn(OrgUsername, "test", string.Empty);
				Assert.IsFalse(string.IsNullOrEmpty(r.state));
				var state1 = r.state;
				r = backend.ActOn(OrgUsername, "test", string.Empty);
				Assert.AreEqual(state1, r.state);
			}
			
			[Test]
			public void GivesSameStateThreaded()
			{
				sc.MinTimeBetweenRadiusRequestsPerUser = 2000000;
				startAuthEngineServer();
				startCloudSMSServer();
				backend = new RadiusLogicBackend(sc);

				const int numThreads = 100;
				ManualResetEvent resetEvent = new ManualResetEvent(false);
				int toProcess = numThreads;
				List<TestDataHolder> states = new List<TestDataHolder>();
				for (int i = 0; i < numThreads; i++)
				{
					var dataHolder = new TestDataHolder();
					states.Add(dataHolder);
					new Thread(delegate(object holder)
					           {
					           	var data = (TestDataHolder)holder;
					           	var rchal = backend.ActOn(OrgUsername, "test", string.Empty);
					           	if (rchal.status != RadiusLogicBackend.CHALLENGE)
					           		Console.WriteLine("NOT CHALLENGE: " + rchal.message);

					           	data.Data = rchal.state;
					           	var rauth = backend.ActOn(OrgUsername, rchal.message, rchal.state);
					           	data.Validated = (rauth.status == RadiusLogicBackend.ACCEPT);
					           	if (rauth.status != RadiusLogicBackend.ACCEPT && rchal.status == RadiusLogicBackend.CHALLENGE)
					           		Console.WriteLine("NOT ACCEPT: " + rauth.message);

					           	if (Interlocked.Decrement(ref toProcess) == 0)
					           		resetEvent.Set();
					           }).Start(dataHolder);
				}
				resetEvent.WaitOne();
				var firstState = states[0].Data;
				var threadNro = 0;
				var atLeastOneAccepted = false;
				foreach (var d in states)
				{
					Assert.AreEqual(firstState, d.Data, string.Format("Thread {0} has different state.", threadNro));
					if (d.Validated == true)
						atLeastOneAccepted = true;
					threadNro++;
				}
				Assert.IsTrue(atLeastOneAccepted);
			}

			[Test]
			public void AfterGivesDifferentState()
			{
				SetupRadiusLogicTest();
				ActOnRet r;
				r = backend.ActOn(OrgUsername, "test", string.Empty);
				Assert.IsFalse(string.IsNullOrEmpty(r.state));
				var state1 = r.state;

				Thread.Sleep(sc.MinTimeBetweenRadiusRequestsPerUser + 500);

				r = backend.ActOn(OrgUsername, "test", string.Empty);
				Assert.IsFalse(string.IsNullOrEmpty(r.state));
				Assert.AreNotEqual(state1, r.state);
			}
			
			private class TestDataHolder
			{
				public string Data { get; set; }
				public bool Validated { get; set; }
			}
		}
		
		[TestFixture]
		public class PasswordVault : RadiusBaseTest
		{
			const string vaultTestPassword = "Password0$";
			
			void setUserVaultPassword(string username, string password) {
				using (var queries = DBQueriesProvider.Get())
				{
					var userIdDt = queries.Query(@"
		SELECT TOP 1 ID FROM [SMS_CONTACT] WHERE [SMS_CONTACT].AD_USERNAME = @aduser
	", new DBQueryParm(@"aduser", username));
					var userId = Convert.ToInt64(userIdDt.Rows[0][0]);
					
					const string queryId = @"SELECT [id] FROM [Vault] WHERE [userId]=@USERID";
					var dt = queries.Query(queryId, new List<DBQueryParm> {
					                       	new DBQueryParm(@"USERID", userId)
					                       });
					string query;
					if (dt.Rows.Count == 0) {
						query = @"INSERT INTO [Vault]([content],[userId]) VALUES(@CONTENT,@USERID)";
					} else {
						query = @"UPDATE [Vault] SET [content]=@CONTENT WHERE [userId]=@USERID";
					}
					
					string encryptedPassword;
					if (string.IsNullOrEmpty(password)) {
						encryptedPassword = string.Empty;
					} else {
						var iters = (new Random()).Next(10, 2000);
						var salt = RandomKeyGenerator.Generate(16, RKGBase.Base32);
						var rijndaelKey = new RijndaelEnhanced(sc.VaultPassword + "CCSSMS2SWGBccssms2swgb", "2SWGBccssms2swgb", 8, 64, 256, "SHA1", salt, iters);
						encryptedPassword = salt + "$" + iters + "$" + rijndaelKey.Encrypt(password);
					 	//encryptedPassword = AESThenHMAC.SimpleEncryptWithPassword(password, sc.VaultPassword + "CCSSMS2SWGBccssms2swgb", AESThenHMAC.NewKey());
					}
					
					var parms = new List<DBQueryParm>();
					parms.Add(new DBQueryParm(@"USERID", userId));
					parms.Add(new DBQueryParm(@"CONTENT", encryptedPassword));
					queries.NonQuery(query, parms);
				}
			}
			
			[Test]
			public void EmptyVaultShouldAskPassword()
			{
				ServerLogic.setSetting("RADIUS", "PasswordVaulting", true.ToString());
				
				sc.AuthEngineChallengeResponse = true;
				var fakeUserGetter = new FakeUserGetterWithList();
				registry.AddOrSet<IUsersGetter>(fakeUserGetter);

				SetupRadiusLogicTest();
				
				//var afdUser = insertUser(string.Empty);
				//var orgusername = afdUser.Org + @"\" + afdUser.User;
				var orgusername = OrgUsername;
				
				ActOnRet rauth;
				rauth = backend.ActOn(orgusername, string.Empty, string.Empty);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rauth.status, rauth.message);
				
				rauth = backend.ActOn(orgusername, rauth.message, rauth.state);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rauth.status, rauth.message);
				
				rauth = backend.ActOn(orgusername, vaultTestPassword, rauth.state);
				Assert.AreEqual(RadiusLogicBackend.ACCEPT, rauth.status, rauth.message);
			}
			
			[Test]
			public void InvalidVaultShouldAskPassword()
			{
				SetupRadiusLogicTest(false);
				sc.AuthEngineChallengeResponse = true;
				ServerLogic.setSetting("RADIUS", "PasswordVaulting", true.ToString());
				
				startAuthEngineServer();
				startCloudSMSServer();
				backend = new RadiusLogicBackend(sc);

				//var afdUser = insertUser();
				//var orgusername = afdUser.Org + @"\" + afdUser.User;
				var orgusername = OrgUsername;
				setUserVaultPassword( Username, "not");
				ActOnRet rauth;
				rauth = backend.ActOn(orgusername, string.Empty, string.Empty);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rauth.status, rauth.message);
				
				rauth = backend.ActOn(orgusername, rauth.message, rauth.state);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rauth.status, rauth.message);
				
				rauth = backend.ActOn(orgusername, vaultTestPassword, rauth.state);
				Assert.AreEqual(RadiusLogicBackend.ACCEPT, rauth.status, rauth.message);				
			}
			
			[Test]
			public void ValidVaultShouldNotAskPassword()
			{
				SetupRadiusLogicTest(false);
				sc.AuthEngineChallengeResponse = true;
				ServerLogic.setSetting("RADIUS", "PasswordVaulting", true.ToString());
				
				startAuthEngineServer();
				startCloudSMSServer();
				backend = new RadiusLogicBackend(sc);

				//var afdUser = insertUser();
				//var orgusername = afdUser.Org + @"\" + afdUser.User;
				var orgusername = OrgUsername;
				setUserVaultPassword( Username, vaultTestPassword);
				ActOnRet rauth;
				rauth = backend.ActOn(orgusername, string.Empty, string.Empty);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rauth.status, rauth.message);
				
				rauth = backend.ActOn(orgusername, rauth.message, rauth.state);
				Assert.AreEqual(RadiusLogicBackend.ACCEPT, rauth.status, rauth.message);
			}
		}
		
		[TestFixture]
		public class AskMissingInfo : RadiusBaseTest
		{
			
			[Test]
			public void EmptyMobileShouldAlwaysAskForIt()
			{
				SetupRadiusLogicTest(false);
				this.sc.BaseSendTokenTestMode = false;

				sc.AuthEnginePinCode = PinCodeOption.True;
				ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
				startAuthEngineServer();
				startCloudSMSServer();
				this.backend = new RadiusLogicBackend(this.sc);

				var afdUser = insertUser("");
				
				ActOnRet rauth;
				using (var queries = DBQueriesProvider.Get())
					queries.Query(string.Format(@"UPDATE [SMS_CONTACT] SET [MOBILE_NUMBER] = '' WHERE [AD_USERNAME] = '{0}'", afdUser.User));
				for (var i = 0; i < 4; i++)
				{
					rauth = backend.ActOn(afdUser.Org + @"\" + afdUser.User, "", string.Empty);
					Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rauth.status, rauth.message);
				}
			}
			
			[Test]
			public void MobileNumber() {
				var fakeUserGetter = new FakeUserGetterWithList();
				registry.AddOrSet<IUsersGetter>(fakeUserGetter);

				SetupRadiusLogicTest();
				
				var afdUser = insertUser(string.Empty);
				var userorg = string.Format("{0}\\{1}", afdUser.Org, afdUser.User);
				using (var queries = DBQueriesProvider.Get())
					queries.Query(string.Format(@"UPDATE [SMS_CONTACT] SET [MOBILE_NUMBER] = '' WHERE [AD_USERNAME] = '{0}'", afdUser.User));

				var rchalmobile = backend.ActOn(userorg, "", string.Empty);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rchalmobile.status, rchalmobile.message);
				Assert.That(rchalmobile.message, Is.StringContaining("phone"));

				var rchal = backend.ActOn(userorg, "1234", rchalmobile.state);
				Assert.That(rchal.state, Is.EqualTo(rchalmobile.state));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rchal.status, string.Format("{0}", rchalmobile.message));
				using (var queries = DBQueriesProvider.Get())
					Assert.That(queries.QueryScalar<string>(@"SELECT [MOBILE_NUMBER] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = @user", new DBQueryParm("user", afdUser.User)), Is.EqualTo("1234"));

				var rauth = backend.ActOn(userorg, rchal.message, rchal.state);
				Assert.That(rauth.state, Is.EqualTo(rchal.state));
				Assert.AreEqual(RadiusLogicBackend.ACCEPT, rauth.status, string.Format("{0}", rchal.message));
				
				using (var queries = DBQueriesProvider.Get())
					Assert.That(queries.QueryScalar<bool>(string.Format(@"SELECT [{0}] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = @user", ServerLogic.TempPInfo), new DBQueryParm("user", afdUser.User)), Is.False);
			}
			
			[Test]
			public void MobileTwiceAndAuth() {
				var afd = setupFakeUserPoll();
				
				SetupRadiusLogicTest();
				
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				var rchalmobile = backend.ActOn(userorg, "", string.Empty);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rchalmobile.status, rchalmobile.message);

				var rchal = backend.ActOn(userorg, "", rchalmobile.state);
				Assert.That(rchal.state, Is.EqualTo(rchalmobile.state));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rchal.status, string.Format("{0}", rchalmobile.message));
				using (var queries = DBQueriesProvider.Get())
					Assert.That(queries.QueryScalar<string>(@"SELECT [MOBILE_NUMBER] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = @user", new DBQueryParm("user", afd.User)), Is.EqualTo(""));

				var rchal2 = backend.ActOn(userorg, "1234", rchalmobile.state);
				Assert.That(rchal2.state, Is.EqualTo(rchal.state));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, rchal2.status, string.Format("{0}", rchal.message));
				using (var queries = DBQueriesProvider.Get())
					Assert.That(queries.QueryScalar<string>(@"SELECT [MOBILE_NUMBER] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = @user", new DBQueryParm("user", afd.User)), Is.EqualTo("1234"));
				
				var rauth = backend.ActOn(userorg, rchal2.message, rchal2.state);
				Assert.That(rauth.state, Is.EqualTo(rchal2.state));
				Assert.AreEqual(RadiusLogicBackend.ACCEPT, rauth.status, string.Format("{0}", rchal2.message));
			}

			[Test]
			public void AskPinMobileAndAuth()
			{
				var afd = setupFakeUserPoll();
				
				SetupRadiusLogicTest(false);
				sc.AuthEnginePinCode = PinCodeOption.Enforced;
				ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
				RadiusStartAuthEngine();
				
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				var r1 = backend.ActOn(userorg, "", string.Empty);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
				
				var r2 = backend.ActOn(userorg, "abcdef", r1.state);
				Assert.That(r2.state, Is.EqualTo(r1.state));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r2.status, r2.message);

				var r4 = backend.ActOn(userorg, "1234", r2.state);
				Assert.That(r4.state, Is.EqualTo(r1.state));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r4.status, r4.message);
				
				var r5 = backend.ActOn(userorg, r4.message, r4.state);
				Assert.That(r5.state, Is.EqualTo(r1.state));
				Assert.AreEqual(RadiusLogicBackend.ACCEPT, r5.status, string.Format("{0}", r5.message));
				
				using (var queries = DBQueriesProvider.Get()) {
					Assert.That(queries.QueryScalar<bool>(string.Format(@"SELECT [{0}] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = @user", ServerLogic.TempPinCode), new DBQueryParm("user", afd.User)), Is.False);
					Assert.That(queries.QueryScalar<bool>(string.Format(@"SELECT [{0}] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = @user", ServerLogic.TempPInfo), new DBQueryParm("user", afd.User)), Is.False);
				}
			}
			
			[Test]
			public void AskPinAlreadySetThenMobileAndAuth()
			{
				var afd = getInsertUserData();
				afd.Phone = string.Empty;
				afd.Mobile = string.Empty;
				afd.PinCode = "test";
				var fakeUserGetter = new FakeUserGetterWithList();
				fakeUserGetter.UserList.Add(afd);
				registry.AddOrSet<IUsersGetter>(fakeUserGetter);
				
				SetupRadiusLogicTest(false);
				sc.AuthEnginePinCode = PinCodeOption.Enforced;
				ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
				RadiusStartAuthEngine();
				
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				var r1 = backend.ActOn(userorg, "test", string.Empty);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
				
				var r2 = backend.ActOn(userorg, "abcdef", r1.state);
				Assert.That(r2.state, Is.EqualTo(r1.state));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r2.status, r2.message);

				var r4 = backend.ActOn(userorg, "1234", r2.state);
				Assert.That(r4.state, Is.EqualTo(r1.state));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r4.status, r4.message);
				
				var r5 = backend.ActOn(userorg, r4.message, r4.state);
				Assert.That(r5.state, Is.EqualTo(r1.state));
				Assert.AreEqual(RadiusLogicBackend.ACCEPT, r5.status, string.Format("{0}", r5.message));
				
				using (var queries = DBQueriesProvider.Get()) {
					Assert.That(queries.QueryScalar<bool>(string.Format(@"SELECT [{0}] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = @user", ServerLogic.TempPinCode), new DBQueryParm("user", afd.User)), Is.False);
					Assert.That(queries.QueryScalar<bool>(string.Format(@"SELECT [{0}] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = @user", ServerLogic.TempPInfo), new DBQueryParm("user", afd.User)), Is.False);
				}
			}
			
			
			[Test]
			public void AskPinOnFirstTimeMobileAndAuth()
			{
				var afd = setupFakeUserPoll();
				
				SetupRadiusLogicTest(false);
				sc.AuthEnginePinCode = PinCodeOption.True;
				ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
				RadiusStartAuthEngine();
				
				using (var queries = DBQueriesProvider.Get()) {
					Assert.That(queries.QueryScalar<string>(string.Format(@"SELECT [{0}] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = @user", "PINCODE"), new DBQueryParm("user", afd.User)), Is.Empty);
				}
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				var r1 = backend.ActOn(userorg, "", string.Empty);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
				Assert.That(r1.message, Is.StringContaining("Pincode"));
				
				var r2 = backend.ActOn(userorg, "abcdef", r1.state);
				Assert.That(r2.state, Is.EqualTo(r1.state));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r2.status, r2.message);

				var r4 = backend.ActOn(userorg, "1234", r2.state);
				Assert.That(r4.state, Is.EqualTo(r1.state));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r4.status, r4.message);
				
				var r5 = backend.ActOn(userorg, r4.message, r4.state);
				Assert.That(r5.state, Is.EqualTo(r1.state));
				Assert.AreEqual(RadiusLogicBackend.ACCEPT, r5.status, string.Format("{0}", r5.message));
				
				using (var queries = DBQueriesProvider.Get()) {
					Assert.That(queries.QueryScalar<string>(string.Format(@"SELECT [{0}] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = @user", "PINCODE"), new DBQueryParm("user", afd.User)), Is.Not.Empty);
					Assert.That(queries.QueryScalar<bool>(string.Format(@"SELECT [{0}] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = @user", ServerLogic.TempPinCode), new DBQueryParm("user", afd.User)), Is.False);
					Assert.That(queries.QueryScalar<bool>(string.Format(@"SELECT [{0}] FROM [SMS_CONTACT] WHERE [AD_USERNAME] = @user", ServerLogic.TempPInfo), new DBQueryParm("user", afd.User)), Is.False);
				}
			}
			
			[Test]
			public void AgedUserDoesGetAsked()
			{
				var afd = setupFakeUserPoll();
				SetupRadiusLogicTest();
				
				using (var queries = DBQueriesProvider.Get())
					queries.NonQuery(string.Format(@"UPDATE [SMS_CONTACT] SET [CREATION_DATE] = '2000-01-01 01:10' WHERE [AD_USERNAME] = '{0}'", afd.User));
				
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				var r1 = backend.ActOn(userorg, "", string.Empty);
				Assert.That(r1.message, Is.StringContaining("phone"));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
			}

			[Test]
			public void AgedALittleGetsAsked()
			{
				var afd = setupFakeUserPoll();
				SetupRadiusLogicTest();
				
				using (var queries = DBQueriesProvider.Get())
					queries.NonQuery(string.Format(@"UPDATE [SMS_CONTACT] SET [CREATION_DATE]=DATEADD(hh,-8,GETDATE()) WHERE [AD_USERNAME]='{0}'", afd.User));
				
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				var r1 = backend.ActOn(userorg, "", string.Empty);
				Assert.That(r1.message, Is.StringContaining("phone"));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
			}

			
			[Test]
			public void DoNotAsk()
			{
				var afd = setupFakeUserPoll();
				SetupRadiusLogicTest(false);
				sc.AuthEngineAskMissingInfo = false;
				ServerLogic.setSetting("AESETTING", "AuthEngineAskMissingInfo", sc.AuthEngineAskMissingInfo.ToString());
				RadiusStartAuthEngine();
				
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				var r1 = backend.ActOn(userorg, "", string.Empty);
				Assert.That(r1.message, Is.Not.StringContaining("phone"));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
			}
			
			[Test]
			public void AskEmailValidates()
			{
				var afd = new AddFullDetails()
				{
					Fullname = "Test User",
					User = "test.user",
					Org = "testdomain",
					UserType = UserType.User,
					Enabled = true,
					Mobile = "00541169531549",
					OrgType = "DOMAIN",
					Phone = "",
					Sid = "123",
					Email = "",
					PinCode = "",
					Providers = new List<UserProvider> { 
						new UserProvider { Name = "Email", Enabled = true },
					},
					AuthEnabled = sc.AuthEngineDefaultEnabled,
				};
				var fakeMailSender = new FakeMailSender();
				registry.AddOrSet<IMailSender>(fakeMailSender);
				setupFakeUserPoll(afd);
				SetupRadiusLogicTest();
				
				setSelectedActiveProvider(afd, "Email");
				
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				var r1 = backend.ActOn(userorg, "", string.Empty);
				Assert.That(r1.message, Is.StringContaining("e-mail"));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
				
				var r2 = backend.ActOn(userorg, "test@mail.com", r1.state);
				Assert.That(r2.message, Is.Not.StringContaining("e-mail"));
			}
			
			[Test]
			public void AskEmailValidateFail()
			{
				var afd = new AddFullDetails()
				{
					Fullname = "Test User",
					User = "test.user",
					Org = "testdomain",
					UserType = UserType.User,
					Enabled = true,
					Mobile = "00541169531549",
					OrgType = "DOMAIN",
					Phone = "",
					Sid = "123",
					Email = "",
					PinCode = "",
					Providers = new List<UserProvider> { 
						new UserProvider { Name = "Email", Enabled = true },
					},
					AuthEnabled = sc.AuthEngineDefaultEnabled,
				};
				var fakeMailSender = new FakeMailSender();
				registry.AddOrSet<IMailSender>(fakeMailSender);
				setupFakeUserPoll(afd);
				SetupRadiusLogicTest();

				setSelectedActiveProvider(afd, "Email");
								
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				var r1 = backend.ActOn(userorg, "", string.Empty);
				Assert.That(r1.message, Is.StringContaining("e-mail"));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
				
				var r2 = backend.ActOn(userorg, "abcdef", r1.state);
				Assert.That(r2.message, Is.StringContaining("e-mail"));
			}
			
			[TestCase("+123")]
			[TestCase("123")]
			public void AskPhoneValidates(string phone)
			{
				var afd = new AddFullDetails
				{
					Fullname = "Test User",
					User = "test.user",
					Org = "testdomain",
					UserType = UserType.User,
					Enabled = true,
					Mobile = "",
					OrgType = "DOMAIN",
					Phone = "",
					Sid = "123",
					Email = "",
					PinCode = "",
					Providers = new List<UserProvider> { 
						new UserProvider { Name = "CloudSMS", Enabled = true },
					},
					AuthEnabled = sc.AuthEngineDefaultEnabled,
				};
				var fakeMailSender = new FakeMailSender();
				registry.AddOrSet<IMailSender>(fakeMailSender);
				setupFakeUserPoll(afd);
				SetupRadiusLogicTest();
				
				setSelectedActiveProvider(afd, "CloudSMS");
				
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				var r1 = backend.ActOn(userorg, "", string.Empty);
				Assert.That(r1.message, Is.StringContaining("mobile phone"));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
				
				var r2 = backend.ActOn(userorg, phone, r1.state);
				Assert.That(r2.message, Is.Not.StringContaining("mobile phone"));
			}
			
			[TestCase("")]
			[TestCase("abc")]
			[TestCase("+a1234")]
			[TestCase("123e")]
			public void AskPhoneValidateFail(string phone)
			{
				var afd = new AddFullDetails
				{
					Fullname = "Test User",
					User = "test.user",
					Org = "testdomain",
					UserType = UserType.User,
					Enabled = true,
					Mobile = "",
					OrgType = "DOMAIN",
					Phone = "",
					Sid = "123",
					Email = "",
					PinCode = "",
					Providers = new List<UserProvider> { 
						new UserProvider { Name = "CloudSMS", Enabled = true },
					},
					AuthEnabled = sc.AuthEngineDefaultEnabled,
				};
				var fakeMailSender = new FakeMailSender();
				registry.AddOrSet<IMailSender>(fakeMailSender);
				setupFakeUserPoll(afd);
				SetupRadiusLogicTest();

				setSelectedActiveProvider(afd, "CloudSMS");
								
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				var r1 = backend.ActOn(userorg, "", string.Empty);
				Assert.That(r1.message, Is.StringContaining("mobile phone"));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
				
				var r2 = backend.ActOn(userorg, phone, r1.state);
				Assert.That(r2.message, Is.StringContaining("mobile phone"));
			}
			
			[Test]
			public void AskPhoneValidateFailKeepsPhoneEmpty()
			{
				var afd = setupFakeUserPoll();
				SetupRadiusLogicTest();
				
				setSelectedActiveProvider(afd, "CloudSMS");
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);
				
				var r1 = backend.ActOn(userorg, "", string.Empty);
				Assert.That(r1.message, Is.StringContaining("mobile phone"));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
				
				var r2 = backend.ActOn(userorg, "12345", r1.state);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
				Assert.That(r2.message, Is.Not.StringContaining("mobile phone"));
				
				var r3 = backend.ActOn(userorg, "Not the passcode!", r1.state);
				Assert.AreEqual(RadiusLogicBackend.REJECT, r3.status, r3.message);
				
				var r4 = backend.ActOn(userorg, "", string.Empty);
				Assert.That(r1.message, Is.StringContaining("mobile phone"));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r4.status, r4.message);
				
				var r5 = backend.ActOn(userorg, "54321", r4.state);
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r5.status, r5.message);
				Assert.That(r5.message, Is.Not.StringContaining("mobile phone"));
				
				var r6 = backend.ActOn(userorg, r5.message, r4.state);
				Assert.AreEqual(RadiusLogicBackend.ACCEPT, r6.status, r6.message);
			}
			
			AddFullDetails setupSerialTest() {
				var afd = getInsertUserData();
				afd.Providers = new List<UserProvider> { 
						new UserProvider { Name = "OATHCalc", Enabled = true },
				};
				setupFakeUserPoll(afd);
				SetupRadiusLogicTest();

				setSelectedActiveProvider(afd, "OATHCalc");
				return afd;
			}
			
			[Test]
			public void AskFeitianAgainWhenInvalidSerial() {
				var afd = setupSerialTest();
								
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				var r1 = backend.ActOn(userorg, "", string.Empty);
				Assert.That(r1.message, Is.StringContaining("token serial"));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, r1.status, r1.message);
				
				var r2 = backend.ActOn(userorg, "my serial", r1.state);
				Assert.That(r2.message, Is.StringContaining("enter your token serial"));
			}
			
			[Test]
			public void AskFeitianSerialAndPass() {
				var afd = setupSerialTest();
								
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				using (var queries = DBQueriesProvider.Get()) {
					var key = HexConversion.ToString(Encoding.UTF8.GetBytes("test"));
					queries.NonQuery("INSERT INTO [HardToken]([serial],[key],[tokentype]) VALUES('testserial','" + key + "', 'hotp');");
				}
				
				ActOnRet ret;
				ret = backend.ActOn(userorg, "", string.Empty);
				Assert.That(ret.message, Is.StringContaining("token serial"));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, ret.status, ret.message);
				
				ret = backend.ActOn(userorg, "testserial", ret.state);
				Assert.That(ret.message, Is.StringContaining("Message challenge"));
				
				ret = backend.ActOn(userorg, "431881", ret.state);
				Assert.AreEqual(RadiusLogicBackend.ACCEPT, ret.status, ret.message);
			}
			
			[Test]
			public void AskFeitianSerialTokenEmptySetsRandomSecret() {
				var afd = setupSerialTest();
								
				var userorg = string.Format("{0}\\{1}", afd.Org, afd.User);

				ActOnRet ret;
				ret = backend.ActOn(userorg, "", string.Empty);
				Assert.That(ret.message, Is.StringContaining("token serial"));
				Assert.AreEqual(RadiusLogicBackend.CHALLENGE, ret.status, ret.message);
				
				ret = backend.ActOn(userorg, "", ret.state);
				Assert.That(ret.message, Is.StringContaining("Message challenge"));
				
				ret = backend.ActOn(userorg, "random string should fail", ret.state);
				Assert.That(ret.message, Is.StringContaining("Pin validation failed"));
			}
			
			AddFullDetails setupFakeUserPoll(AddFullDetails afd = null)
			{
				if (afd == null)
					afd = getInsertUserData();
				afd.Phone = string.Empty;
				afd.Mobile = string.Empty;
				afd.Email = string.Empty;
				var fakeUserGetter = new FakeUserGetterWithList();
				fakeUserGetter.UserList.Add(afd);
				registry.AddOrSet<IUsersGetter>(fakeUserGetter);
				return afd;
			}
			
			void setSelectedActiveProvider(AddFullDetails afd, string providerName) {
				using (var queries = DBQueriesProvider.Get()) {
					Assert.That(queries.NonQuery(string.Format(@"
UPDATE [UserProviders] 
	SET [UserProviders].Selected = 0
WHERE
	[UserProviders].userId IN (
		SELECT ID FROM [SMS_CONTACT] WHERE [SMS_CONTACT].AD_USERNAME = '{0}'
	)
", afd.User)), Is.EqualTo(afd.Providers.Count));
					
					Assert.That(queries.NonQuery(string.Format(@"
UPDATE [UserProviders] 
	SET [UserProviders].Selected = 1, [UserProviders].Active = 1 
WHERE
	[UserProviders].userId IN (
		SELECT ID FROM [SMS_CONTACT] WHERE [SMS_CONTACT].AD_USERNAME = '{0}'
	)
	AND
	[UserProviders].authProviderId IN (
		SELECT ID FROM [AuthProviders] WHERE [AuthProviders].name = '{1}'
	)
", afd.User, providerName)), Is.EqualTo(1));
				}
			}
		}
	}
}
