using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using AuthGateway.AuthEngine.Logic;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.AuthEngine.Logic.Helpers;
using AuthGateway.Shared;
using AuthGateway.Shared.Identity;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;
using AuthGateway.Shared.Serializer;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;

using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using NUnit.Framework;

namespace AuthGateway.AuthEngine.Unittest
{
	

	[TestFixture]
	public class AuthEngineTests : TestWithServers
	{

		private void checkFlags(int flags)
		{
			Console.WriteLine("===============");
			Console.WriteLine(flags.ToString());
			var flagBits = (UserAccountControl)flags;
			Console.WriteLine(flagBits.ToString());

			if ((flags & 2) == 0)
				Console.WriteLine("ENABLED");
			else
				Console.WriteLine("DISBLED");

			if ((flagBits & UserAccountControl.ACCOUNTDISABLE) == UserAccountControl.ACCOUNTDISABLE)
				Console.WriteLine("UserAccountControl.ACCOUNTDISABLE");

			if ((flagBits & UserAccountControl.NORMAL_ACCOUNT) == UserAccountControl.NORMAL_ACCOUNT)
				Console.WriteLine("UserAccountControl.NORMAL_ACCOUNT");
		}
		[Test]
		public void testBits()
		{
			checkFlags(512);
			checkFlags(66082);
			checkFlags(532480);
			checkFlags(514);
			checkFlags(512);
			checkFlags(544);
			checkFlags(546);

		}
		[Test]
		[Ignore]
		public void TestVacio()
		{
			//Logger.Instance.EmptyLoggers();
			sc.ADServerAddress = "192.168.1.147";
			sc.ADUsername = "Administrator";
			sc.ADPassword = "Password0$";
			sc.ADContainer = "OU=newOU";
			sc.ADBaseDN = "DC=pasargentina,DC=corp";
			//sc.ADFilter = "(&(objectClass=user))";

			sc.ManualDomainReplacements.Add(new DomainReplacement("Sorete"));
			registry.AddOrSet<IUsersGetter>(new AdOrLocalUsersGetter());
			startAuthEngineServer(registry);
			Assert.IsTrue(true);
			var permret = (PermissionsRet)aeServerLogic.ValidatePermissions(new Permissions()
			                                                                {
			                                                                	User = "UserNo1",
			                                                                	Org = "Sorete",
			                                                                	External = false,
			                                                                	Authenticated = true
			                                                                });
			Console.WriteLine(permret.UserType);
			Console.WriteLine(permret.Error);
			Assert.IsTrue(permret.UserType == UserType.User, "User is: ", permret.UserType);
			Assert.IsTrue(string.IsNullOrEmpty(permret.Error), "Error is not empty: " + permret.Error);
		}


		[Test]
		public void TestValidateShouldNotFailWithoutOrg()
		{
			sc.AuthEngineDefaultDomain = null;
			startAuthEngineServer();
			startCloudSMSServer();

			UserProvider up = new UserProvider() { Name = "CloudSMS", Config = "" };
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
				new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
			Assert.AreEqual(1, ret.Out);

			ValidateUser vu = new ValidateUser();
			vu.User = this.Username;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);

		}

		[Test]
		public void TestGetUserProviders()
		{
			startAuthEngineServer();
			UserProviders up = new UserProviders();
			up.User = CurrentIdentity.GetLogin();
			up.Org = CurrentIdentity.GetDomain();            
			UserProvidersRet upr = (UserProvidersRet)aeServerLogic.Actioner.Do(up);
			Assert.IsTrue(string.IsNullOrEmpty(upr.Error), string.Format("No errors expected but returned: {0}", upr.Error));
			Assert.AreEqual(7, upr.Providers.Count, string.Format("Expected {0} providers", 7));
		}
		
		[Test]
		public void TestGetUserProvidersNoDefault()
		{
			sc.Providers.ForEach(x => x.Default = false);
			RunSQL(
				@"UPDATE UserProviders
SET selected = 0
");
			startAuthEngineServer();
			Assert.IsFalse(counterLogger.HasError(), "No errors should have been reported.");
		}

		private List<RetBase> actionsRan;
		[Test]
		public void TestSerializeActioner()
		{
			AuthEngineRequest req = new AuthEngineRequest();
			req.Commands.Add(new Tokens());
			//req.Commands.Add(new ValidateUser() { User = "test" });
			req.Commands.Add(new Details() { User = "test", Org = "testorgname" });
			String ser = Generic.Serialize<AuthEngineRequest>(req);

			Console.Write(ser);
			req = Generic.Deserialize<AuthEngineRequest>(ser);
			Assert.IsTrue(req.Commands.Count == 2);
			Assert.IsInstanceOf<Details>(req.Commands[1]);

			actionsRan = new List<RetBase>();
			var actioner = new ActionerInstance();
			actioner.Add<Details>(new ActionerInstance.CommandAction(ActionRan));
			actioner.Add<Tokens>(new ActionerInstance.CommandAction(ActionRan));
			foreach (CommandBase cmd in req.Commands)
				actionsRan.Add(actioner.Do(cmd));
			Assert.AreEqual(2, actionsRan.Count, "Should've ran all 2 actions");
		}

		private RetBase ActionRan(CommandBase cmd)
		{
			return new UpdateFullDetailsRet();
		}

		[Test]
		public void genkey()
		{
			RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048);
			Console.WriteLine(RSA.ToXmlString(true));
			Console.WriteLine(RSA.ToXmlString(false));
		}

		[Test]
		public void genaeskey()
		{
			int keySize = 32;
			int ivSize = 16;
			byte[] key = new Byte[keySize];
			byte[] iv = new Byte[ivSize];
			RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
			random.GetBytes(key);
			random.GetBytes(iv);
			Console.WriteLine("KEY: " + Convert.ToBase64String(key));
			Console.WriteLine("IV: " + Convert.ToBase64String(iv));
		}

		[Test]
		public void TestServiceLogicShouldFillProviderIds()
		{
			startAuthEngineServer();
			Assert.AreEqual(1, sc.Providers[0].Id);
			Assert.AreEqual(2, sc.Providers[1].Id);
		}

		[Test]
		public void TestUserListShouldBeOrdered()
		{
			sc.AuthEngineUseEncryption = false;
			startAuthEngineServer();
			var afdlast = new AddFullDetails
			{
				Fullname = "zzzzzzz",
				User = "zzzzzzz",
				Org = "testdomain",
				UserType = UserType.User,
				Enabled = true,
				Mobile = "",
				OrgType = "DOMAIN",
				Phone = "",
				Sid = "123",
				Providers = new List<UserProvider> { new UserProvider { Name = "CloudSMS", Enabled = true } }
			};
			var afdfirst = new AddFullDetails
			{
				Fullname = "aaaaaa",
				User = "aaaaaa",
				Org = "testdomain",
				UserType = UserType.User,
				Enabled = true,
				Mobile = "",
				OrgType = "DOMAIN",
				Phone = "",
				Sid = "123",
				Providers = new List<UserProvider> { new UserProvider { Name = "CloudSMS", Enabled = true } }
			};
			var afdsecond = new AddFullDetails
			{
				Fullname = "bbbbbb",
				User = "bbbbbb",
				Org = "testdomain",
				UserType = UserType.User,
				Enabled = true,
				Mobile = "",
				OrgType = "DOMAIN",
				Phone = "",
				Sid = "123",
				Providers = new List<UserProvider> { new UserProvider { Name = "CloudSMS", Enabled = true } }
			};
			var afdthird_admin = insertAdminUser();
			if (!string.IsNullOrEmpty(aeServerLogic.AddFullUser(afdlast).Error))
				throw new Exception("Error inserting user.");
			if (!string.IsNullOrEmpty(aeServerLogic.AddFullUser(afdfirst).Error))
				throw new Exception("Error inserting user.");
			if (!string.IsNullOrEmpty(aeServerLogic.AddFullUser(afdsecond).Error))
				throw new Exception("Error inserting user.");


			Users users = new Users();
			users.Identity = new FakeIdentity(afdthird_admin);
			users.Authenticated = true;
			users.External = true;
			users.Org = afdthird_admin.Org;
			users.Total = 500;
			UsersRet ret = (UsersRet)aeServerLogic.Actioner.Do(users);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error expected: " + ret.Error);
			Assert.AreEqual(ret.Users[0].u, afdfirst.User);
			Assert.AreEqual(ret.Users[1].u, afdsecond.User);
			Assert.AreEqual(ret.Users[ret.Users.Count - 1].u, afdlast.User);
		}

		[Test]
		public void TestDeleteOldUsersDoNotDeleteNew()
		{
			sc.AuthEngineRemoveUsersAfterXHours = 2;
			var user = insertUser();
			var user2 = insertUser2();
			var countBefore = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT")[0][0];
			aeServerLogic.DeactivateAndDeleteOldUsers();
			var countAfter = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT")[0][0];
			Assert.That(countBefore, Is.EqualTo(countAfter));
		}
		
		[Test]
		public void TestDeleteOldUsersNoUpdateButOldCreationDate()
		{
			sc.AuthEngineRemoveUsersAfterXHours = 2;
			var user = insertUser();
			var user2 = insertUser2();
			var updateDateMinus3 = DateTime.Now.AddHours(-3).ToString("yyyy-MM-dd HH:mm:ss");
			var setUserToDelete = RunSQL("UPDATE SMS_CONTACT SET CREATION_DATE='" + updateDateMinus3 + "' WHERE AD_USERNAME='" + user.User + "'");
			var countBefore = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT")[0][0];
			Assert.That(setUserToDelete, Is.EqualTo(1));
			aeServerLogic.DeactivateAndDeleteOldUsers();
			var ret = aeServerLogic.UserList(new Users() { Org = "testdomain", Total = 500 });
			Assert.AreEqual(1, ((UsersRet)ret).Users.Count);
			var countAfter = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT")[0][0];
			Assert.That((countBefore - 1), Is.EqualTo(countAfter));
		}

		
		[Test]
		[TestCase(3, 4)]
		[TestCase(2.1, 3)]
		public void TestDeleteOldUsersDeActivate(double substractHours, int removeAfterHours)
		{
			sc.AuthEngineRemoveUsersAfterXHours = removeAfterHours;
			var user = insertUser();
			var user2 = insertUser2();
			var updateDateMinus3 = DateTime.Now.AddHours(-substractHours).ToString("yyyy-MM-dd HH:mm:ss");
			var setUserToDelete = RunSQL("UPDATE SMS_CONTACT SET UPDATE_DATE='" + updateDateMinus3 + "' WHERE AD_USERNAME='" + user.User + "'");
			var countBefore = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT WHERE userStatus=0")[0][0];
			Assert.That(setUserToDelete, Is.EqualTo(1));
			aeServerLogic.DeactivateAndDeleteOldUsers();
			var countAfter = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT WHERE userStatus=0")[0][0];
			Assert.That((countBefore + 1), Is.EqualTo(countAfter));
		}

		[Test]
		public void TestDeleteOldUsersAfter2hours()
		{
			sc.AuthEngineRemoveUsersAfterXHours = 2;
			var user = insertUser();
			var user2 = insertUser2();
			var updateDateMinus3 = DateTime.Now.AddHours(-3).ToString("yyyy-MM-dd HH:mm:ss");
			var setUserToDelete = RunSQL("UPDATE SMS_CONTACT SET UPDATE_DATE='" + updateDateMinus3 + "' WHERE AD_USERNAME='" + user.User + "'");
			var countBefore = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT")[0][0];
			Assert.That(setUserToDelete, Is.EqualTo(1));
			aeServerLogic.DeactivateAndDeleteOldUsers();
			var ret = aeServerLogic.UserList(new Users() { Org = "testdomain", Total = 500 });
			Assert.AreEqual(1, ((UsersRet)ret).Users.Count);
			var countAfter = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT")[0][0];
			Assert.That((countBefore - 1), Is.EqualTo(countAfter));
		}

		[Test]
		public void TestDeleteOldUsersShouldNotDeleteWith0Hours()
		{
			sc.AuthEngineRemoveUsersAfterXHours = 0;
			var user = insertUser();
			var user2 = insertUser2();
			var updateDateMinus3 = DateTime.Now.AddHours(-90).ToString("yyyy-MM-dd HH:mm:ss");
			RunSQL("UPDATE SMS_CONTACT SET UPDATE_DATE='" + updateDateMinus3 + "' WHERE AD_USERNAME='test.user'");
			aeServerLogic.DeactivateAndDeleteOldUsers();
			var ret = aeServerLogic.UserList(new Users() { Org = "testdomain", Total = 500 });
			Assert.AreEqual(2, ((UsersRet)ret).Users.Count);
		}

		[Test]
		public void TestDeleteOldUsersShouldNotDeleteRecentCreated()
		{
			sc.AuthEngineRemoveUsersAfterXHours = 2;
			var user = insertUser();
			var user2 = insertUser2();
			aeServerLogic.DeactivateAndDeleteOldUsers();
			var ret = aeServerLogic.UserList(new Users() { Org = "testdomain", Total = 500 });
			Assert.AreEqual(2, ((UsersRet)ret).Users.Count);
		}

		[Test]
		[TestCase(0.75, false)]
		[TestCase(0.25, true)]
		public void TestDisasterPreventDeactivate(decimal percentegeToDeactivate, bool expectDeactivate)
		{
			sc.ADDisasterPercentage = 50;
			sc.AuthEngineRemoveUsersAfterXHours = 4;
			var user = insertUser();
			var user2 = insertUser2();
			var updateDateMinus3 = DateTime.Now.AddHours(-3).ToString("yyyy-MM-dd HH:mm:ss");
			
			var totalCount = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT")[0][0];
			var testUsersCount = (int)Math.Ceiling(totalCount * percentegeToDeactivate);
			var idsRows = QueryValSQL("SELECT TOP " + testUsersCount + " ID FROM SMS_CONTACT");
			var ids = new List<long>();
			foreach(var row in idsRows)
				ids.Add(Convert.ToInt64(row[0]));
			
			var setUserToDelete = RunSQL("UPDATE SMS_CONTACT SET userStatus=1, UPDATE_DATE='" + updateDateMinus3 + "' WHERE ID IN (" + string.Join(",", ids.ToArray()) + ")");
			var countBefore = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT WHERE ID IN ("
			                                   + string.Join(",", ids.ToArray())
			                                   + ") AND userStatus=1")[0][0];
			
			Assert.That(setUserToDelete, Is.EqualTo(testUsersCount));
			aeServerLogic.DeactivateAndDeleteOldUsers();
			
			var countAfter = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT WHERE ID IN ("
			                                  + string.Join(",", ids.ToArray())
			                                  + ") AND userStatus=1")[0][0];
			
			if ( expectDeactivate )
				Assert.That(countAfter, Is.LessThan(countBefore));
			else
				Assert.That(countAfter, Is.EqualTo(countBefore));
		}
		
		[Test]
		[TestCase(0.75, false)]
		[TestCase(0.25, true)]
		public void TestDisasterPreventDelete(decimal percentegeToDelete, bool expectDelete)
		{
			sc.ADDisasterPercentage = 50;
			sc.AuthEngineRemoveUsersAfterXHours = 4;
			var user = insertUser();
			var user2 = insertUser2();
			var updateDateMinus3 = DateTime.Now.AddHours(-5).ToString("yyyy-MM-dd HH:mm:ss");
			
			var totalCount = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT")[0][0];
			var testUsersCount = (int)Math.Ceiling(totalCount * percentegeToDelete);
			var idsRows = QueryValSQL("SELECT TOP " + testUsersCount + " ID FROM SMS_CONTACT");
			var ids = new List<long>();
			foreach(var row in idsRows)
				ids.Add(Convert.ToInt64(row[0]));
			
			var setUserToDelete = RunSQL("UPDATE SMS_CONTACT SET USERSTATUS = 0, UPDATE_DATE='" + updateDateMinus3 + "' WHERE ID IN (" + string.Join(",", ids.ToArray()) + ")");
			var countBefore = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT WHERE ID IN ("
			                                   + string.Join(",", ids.ToArray())
			                                   + ") AND userStatus=0")[0][0];
			
			Assert.That(setUserToDelete, Is.EqualTo(testUsersCount));
			aeServerLogic.DeactivateAndDeleteOldUsers();
			
			var countAfter = (int)QueryValSQL("SELECT COUNT(*) FROM SMS_CONTACT WHERE ID IN ("
			                                  + string.Join(",", ids.ToArray())
			                                  + ") AND userStatus=0")[0][0];
			
			if ( expectDelete )
				Assert.That(countAfter, Is.LessThan(countBefore));
			else
				Assert.That(countAfter, Is.EqualTo(countBefore));
		}
		
		[Test]
		public void TestSameUserWithSameSIDShouldUpdate()
		{
			var user = insertUser();
			var userId = (Int64)QueryValSQL("SELECT ID FROM SMS_CONTACT WHERE AD_USERNAME = 'test.user'")[0][0];
			var userOldSid = (string)QueryValSQL("SELECT SID FROM SMS_CONTACT WHERE AD_USERNAME = 'test.user'")[0][0];
			var userUpdateDateTmp = QueryValSQL("SELECT UPDATE_DATE FROM SMS_CONTACT WHERE AD_USERNAME = 'test.user'")[0][0];
			string userUpdateDate = null;
			if (!(userUpdateDateTmp is DBNull))
				userUpdateDate = userUpdateDateTmp.ToString();
			
			var ret = aeServerLogic.AddFullUser(getInsertUserData());
			Thread.Sleep(1500);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected");
			var userSameId = (Int64)QueryValSQL("SELECT ID FROM SMS_CONTACT WHERE AD_USERNAME = 'test.user'")[0][0];
			var userSameSid = (string)QueryValSQL("SELECT SID FROM SMS_CONTACT WHERE AD_USERNAME = 'test.user'")[0][0];
			var userNewerUpdateDate = (DateTime)QueryValSQL("SELECT UPDATE_DATE FROM SMS_CONTACT WHERE AD_USERNAME = 'test.user'")[0][0];
			Assert.AreEqual(userId, userSameId, "Row ID should be the same as user should've been updated");
			Assert.AreEqual(userOldSid, userSameSid, "It should get the same SID");
			Assert.AreEqual(user.Sid, userSameSid, "It should get the same SID");
			Assert.IsNull(userUpdateDate);
			Assert.IsTrue(DateTime.Now > userNewerUpdateDate, "UPDATE_DATE should've been updated.");
			Assert.IsTrue(DateTime.Now.AddMinutes(-1) < userNewerUpdateDate, "UPDATE_DATE should've been updated.");
		}

		[Test]
		public void TestSameUserWithDifferentSIDShouldDeleteOldOne()
		{
			var user = insertUser();
			user.Phone = "1234";
			user.Sid = "CHANGED";
			var userId = (Int64)QueryValSQL("SELECT ID FROM SMS_CONTACT WHERE AD_USERNAME = 'test.user'")[0][0];
			var userOldSid = (string)QueryValSQL("SELECT SID FROM SMS_CONTACT WHERE AD_USERNAME = 'test.user'")[0][0];
			var ret = aeServerLogic.AddFullUser(user);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected");
			var userChangedId = (Int64)QueryValSQL("SELECT ID FROM SMS_CONTACT WHERE AD_USERNAME = 'test.user'")[0][0];
			var userChangedSid = (string)QueryValSQL("SELECT SID FROM SMS_CONTACT WHERE AD_USERNAME = 'test.user'")[0][0];
			Assert.AreNotEqual(userId, userChangedId, "Row ID should be different as user should've been deleted/reinserted");
			Assert.AreNotEqual(userOldSid, userChangedSid, "It should get the new SID");
			Assert.AreEqual(user.Sid, userChangedSid, "It should get the new SID");
		}

		[Test]
		public void TestValidateUserWithState()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDefaultEnabled = false;
			startAuthEngineServer();

			var user = insertUser();
			user.Phone = "1234";
			var username = user.Org + "\\" + user.User;
			
			ValidateUser vu = new ValidateUser();
			vu.User = username;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("1", vuret.CreditsRemaining);
			Assert.IsTrue(string.IsNullOrEmpty(vuret.Error));
			Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
			ValidatePin vp = new ValidatePin();
			vp.User = username;
			vp.Pin = vuret.Error;
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsTrue(vpret.Validated);
		}


        [Test]
        public void TestValidateUserByUsername()
        {
            sc.AuthEngineUseEncryption = true;
            sc.CloudSMSUseEncryption = true;
            sc.AuthEngineDefaultEnabled = false;
            sc.ManualDomainReplacements.Clear();            
            startAuthEngineServer();

            AddFullDetails user = insertUser("", "", "MYCOOLCOMPANY");

            var username = user.User;

            ValidateUser vu = new ValidateUser();
            vu.User = username;            
            ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
            Assert.AreEqual("1", vuret.CreditsRemaining);            
            Assert.IsTrue(string.IsNullOrEmpty(vuret.Error), vuret.Error);
            Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
            ValidatePin vp = new ValidatePin();
            vp.User = username;
            vp.Pin = vuret.Error;
            vp.State = vuret.State;

            ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
            Assert.IsTrue(vpret.Validated);
        }        

        private void SetSingleLoginOption(string optionSettingName)
        {
            ServerLogic.setSetting("AESETTING", "AuthEngineAllowUPNLogin", "False");
            ServerLogic.setSetting("AESETTING", "AuthEngineAllowPre2000Login", "False");
            ServerLogic.setSetting("AESETTING", "AuthEngineAllowMobileNumberLogin", "False");
            ServerLogic.setSetting("AESETTING", "AuthEngineAllowEmailLogin", "False");
            ServerLogic.setSetting("AESETTING", "AuthEngineAllowAliasesInLogin", "False");
            ServerLogic.setSetting("AESETTING", "AuthEngineSAMLoginPreferred", "True");

            ServerLogic.setSetting("AESETTING", optionSettingName, "True");
        }

        private void InitLoginOptionTest(string optionSettingName)
        {
            sc.AuthEngineUseEncryption = true;
            sc.CloudSMSUseEncryption = true;
            sc.AuthEngineDefaultEnabled = false;
            SetSingleLoginOption(optionSettingName);

            startAuthEngineServer();
        }

        private void TestValidateUserOptions(AddFullDetails user, string username)
        {           
            ValidateUser vu = new ValidateUser();
            vu.User = username;
            ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);            
            Assert.AreEqual("1", vuret.CreditsRemaining);            
            Assert.IsTrue(string.IsNullOrEmpty(vuret.Error));
            Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
            ValidatePin vp = new ValidatePin();
            vp.User = username;
            vp.Pin = vuret.Error;
            vp.State = vuret.State;

            ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
            Assert.IsTrue(vpret.Validated);
        }        

        [Test]
        public void TestValidateUserByMobileNumber()
        {
            InitLoginOptionTest("AuthEngineAllowMobileNumberLogin");
            AddFullDetails user = getInsertUserData();
            user.Org = "MYCOOLCOMPANY";  
            user.Mobile = "79508597929";
            insertUser3(user);

            var username = user.Mobile; 

            TestValidateUserOptions(user, username);   
        }

        [Test]
        public void TestValidateUserByEmail()
        {
            InitLoginOptionTest("AuthEngineAllowEmailLogin");
            AddFullDetails user = getInsertUserData();
            user.Org = "MYCOOLCOMPANY";  
            user.Email = "test@test.test";
            insertUser3(user);                      
            
            var username = user.Email; 

            TestValidateUserOptions(user, username);
        }

        [Test]
        public void TestValidateUserByUPN()
        {
            InitLoginOptionTest("AuthEngineAllowUPNLogin");
            AddFullDetails user = getInsertUserData();
            user.Org = "MYCOOLCOMPANY";  
            user.UPN = "test@test.test";
            insertUser3(user);
            var username = user.UPN;

            TestValidateUserOptions(user, username);
        }

        [Test]
        public void TestValidateUserByDefaultUPN()
        {
            InitLoginOptionTest("AuthEngineAllowUPNLogin");
            var user = insertUser();            
            var username = string.Format("{0}@{1}", user.User, user.Org);

            TestValidateUserOptions(user, username);
        }

        protected UpdateFullDetailsRet updateUser(AddFullDetails user)
        {           
            UpdateFullDetails ufd = new UpdateFullDetails();
            ufd.Identity = new FakeIdentity(user);
            ufd.Authenticated = true;
            ufd.AuthEnabled = true;
            ufd.Fullname = user.Fullname;
            ufd.Mobile = user.Mobile;
            ufd.Phone = user.Phone;
            ufd.User = user.User;
            ufd.Org = user.Org;
            ufd.UserType = user.UserType;            
            ufd.PCChange = true;
            ufd.UPN = user.UPN;
            return (UpdateFullDetailsRet)aeServerLogic.Actioner.Do(ufd);
        }

        [Test]
        public void TestValidateUserByUpdatedUPN()
        {
            InitLoginOptionTest("AuthEngineAllowUPNLogin");
            AddFullDetails user = getInsertUserData();
            user.Org = "MYCOOLCOMPANY";
            user.UPN = "test@test.test";
            insertUser3(user);
            var username = user.UPN;

            TestValidateUserOptions(user, username);

            user.UPN = "new@test.test";
            var updatedUser = updateUser(user);            

            username = user.UPN;

            TestValidateUserOptions(user, username);
        }

        [Test]
        public void TestValidateUserByPre2000Login()
        {
            InitLoginOptionTest("AuthEngineAllowPre2000Login");
            var user = insertUser();            
            var username = user.Org + "\\" + user.User;

            TestValidateUserOptions(user, username);
        }

		[Test]
		public void TestValidatePinWithUnexistantStateShouldNotValidate()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDefaultEnabled = false;
			startAuthEngineServer();

			ValidatePin vp = new ValidatePin();
			vp.User = this.OrgUsername;
			vp.Pin = "SomePin";
			vp.State = "NOTEXISTS";

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsFalse(string.IsNullOrEmpty(vpret.Error));
			Assert.IsFalse(vpret.Validated);
		}

		[Test]
		public void TestValidateUserWithWrongStateShouldNotValidate()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDefaultEnabled = false;
			startAuthEngineServer();
			startCloudSMSServer();
			
			var user = insertUser();
			user.Phone = "1234";
			var username = user.Org + "\\" + user.User;

			ValidateUser vu = new ValidateUser();
			vu.User = username;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.AreEqual("1", vuret.CreditsRemaining);
			Assert.IsTrue(string.IsNullOrEmpty(vuret.Error));
			Assert.IsFalse(string.IsNullOrEmpty(vuret.State));
			ValidatePin vp = new ValidatePin();
			vp.User = username;
			vp.Pin = vuret.Error;
			vp.State = "NOTEXISTS";

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsFalse(string.IsNullOrEmpty(vpret.Error));
			Assert.IsFalse(vpret.Validated);
		}

		[Test]
		[Timeout(60000)]
		[Ignore]
		public void TestPollUsersLock()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDefaultEnabled = false;
			startAuthEngineServer();

			PollUsers vu = new PollUsers();
			PollUsersRet vuret = (PollUsersRet)aeServerLogic.Actioner.Do(vu);
			Assert.IsFalse(Monitor.TryEnter(aeServerLogic.LockUpdateUsersObj));
			Assert.IsTrue(string.IsNullOrEmpty(vuret.Error));
			try
			{
				while (!Monitor.TryEnter(aeServerLogic.LockUpdateUsersObj))
				{
					Thread.Sleep(100);
				}
			}
			finally
			{
				Monitor.Exit(aeServerLogic.LockUpdateUsersObj);
			}
			try
			{
				Assert.IsTrue(Monitor.TryEnter(aeServerLogic.LockUpdateUsersObj));
			}
			finally
			{
				Monitor.Exit(aeServerLogic.LockUpdateUsersObj);
			}
		}

		[Test]
		public void TestGetDefaultDomainOrComputerName()
		{
			sc.AuthEngineDefaultDomain = null;
			var sl = new ServerLogic(sc);

			Assert.IsFalse(string.IsNullOrEmpty(sl.getDefaultDomain()));

			sc.AuthEngineDefaultDomain = string.Empty;
			Assert.IsFalse(string.IsNullOrEmpty(sl.getDefaultDomain()));
		}

		[Test]
		public void TestAddFullUser1()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDefaultEnabled = true;
			startAuthEngineServer();

			var users = new List<AddFullDetails>();
			for (var i = 0; i < 1; i++)
			{
				AddFullDetails add = new AddFullDetails();
				add.Sid = "up.Sid.Value" + i;
				add.User = "up.SamAccountName" + i;
				add.Phone = string.Empty;
				add.Mobile = string.Empty;
				add.Fullname = "up.Name" + i;
				add.Email = string.Empty;
                add.UPN = "up@upn" + i;

				add.AuthEnabled = (sc.AuthEngineDefaultEnabled);
				add.UserType = UserType.User;
				foreach (var provider in sc.Providers)
				{
					add.Providers.Add(new UserProvider()
					                  {
					                  	Name = provider.Name
					                  		,
					                  	Enabled = provider.Enabled
					                  		,
					                  	Provider = provider
					                  });
				}
				add.Org = "domainName1";
				add.OrgType = "DOMAIN";
				add.Enabled = true;
				users.Add(add);
			}

			aeServerLogic.InsertOrUpdateUsers(users);
			var cmd = new Users();
			cmd.External = false;
			cmd.Org = "domainName1";
			cmd.Total = 1;
			var ul = (UsersRet)aeServerLogic.UserList(cmd);
			Assert.AreEqual(1, ul.Users.Count);
		}        

		[Test]
		[Ignore] // It takes too long
		public void TestAddFullUsers20k()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDefaultEnabled = true;
			startAuthEngineServer();

			var cmd = new Users() { External = false, Org = "domainName20K", Total = 30000 };

			var users = new List<AddFullDetails>();
			insertXUsers(users, 20000);

			Thread t;
			// Insert users
			t = new Thread(new ParameterizedThreadStart(callInsertOrUpdateUsers));
			t.Start(users);
			//for (var i = 0; i < 5; i++)
			//{
			//	var ult = (UsersRet)aeServerLogic.UserList(cmd);
			//	Assert.IsTrue(string.IsNullOrWhiteSpace(ult.Error), "GOT ERROR:" + ult.Error);
			//	Thread.Sleep(100);
			//}
			t.Join();

			// Update users
			t = new Thread(new ParameterizedThreadStart(callInsertOrUpdateUsers));
			t.Start(users);
			//for (var i = 0; i < 5; i++)
			//{
			//	var ult = (UsersRet)aeServerLogic.UserList(cmd);
			//	Assert.IsTrue(string.IsNullOrWhiteSpace(ult.Error), "GOT ERROR:" + ult.Error);
			//	Thread.Sleep(100);
			//}
			t.Join();

			var ul = (UsersRet)aeServerLogic.UserList(cmd);
			Assert.IsTrue(string.IsNullOrWhiteSpace(ul.Error), "GOT ERROR:" + ul.Error);
			Assert.AreEqual(20000, ul.Users.Count);
		}
		
		[Test]
		public void TestGetUsersTextFilter()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDefaultEnabled = true;
			startAuthEngineServer();

			var users = new List<AddFullDetails>();
			insertXUsers(users, 200);

			Thread t;
			// Insert users
			t = new Thread(new ParameterizedThreadStart(callInsertOrUpdateUsers));
			t.Start(users);
			t.Join();

			// Update users
			t = new Thread(new ParameterizedThreadStart(callInsertOrUpdateUsers));
			t.Start(users);
			t.Join();

			var cmd = new Users { External = false, Org = "domainName20K", Total = 30000 };
			var ul = (UsersRet)aeServerLogic.UserList(cmd);
			Assert.IsTrue(string.IsNullOrWhiteSpace(ul.Error), "GOT ERROR:" + ul.Error);
			Assert.AreEqual(200, ul.Users.Count);
			
			cmd = new Users { External = false, Org = "domainName20K", Total = 30000, Text = "up.SamAccountName" };
			ul = (UsersRet)aeServerLogic.UserList(cmd);
			Assert.IsTrue(string.IsNullOrWhiteSpace(ul.Error), "GOT ERROR:" + ul.Error);
			Assert.AreEqual(200, ul.Users.Count);
			
			cmd = new Users { External = false, Org = "domainName20K", Total = 30000, Text = "up.SamAccountName199" };
			ul = (UsersRet)aeServerLogic.UserList(cmd);
			Assert.IsTrue(string.IsNullOrWhiteSpace(ul.Error), "GOT ERROR:" + ul.Error);
			Assert.AreEqual(1, ul.Users.Count);
		}

		[Test]
		[Ignore] // It takes too long
		public void TestAddFullUsers20kFaked()
		{
			Logger.Instance.EmptyLoggers();
			//Logger.Instance.AddLogger(new ConsoleLogger(), LogLevel.All);
			Logger.Instance.AddLogger(new FileLogger(Environment.CurrentDirectory, "TestAuthEngine.TestAddFullUsers20k.log", 20, 10), LogLevel.All);
			Logger.Instance.SetFlushOnWrite(true);
			Logger.Instance.SetLogLevel(LogLevel.All);

			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDefaultEnabled = true;
			var registry = new Registry();
			registry.AddOrSet<IUsersGetter>(new FakeUsersGets());
			startAuthEngineServer(registry);

			var cmd = new Users() { External = false, Org = "Org", Total = 30000 };

			for (var i = 0; i < 2; i++)
			{
				aeServer.getUsers();
			}

			var ul = (UsersRet)aeServerLogic.UserList(cmd);
			Assert.IsTrue(string.IsNullOrWhiteSpace(ul.Error), "GOT ERROR:" + ul.Error);
			Assert.AreEqual(20000, ul.Users.Count);
		}

		private void callInsertOrUpdateUsers(object usersObj)
		{
			var users = (List<AddFullDetails>)usersObj;
			aeServerLogic.InsertOrUpdateUsers(users);
		}


		[Test]
		public void TestGetAvailableModules()
		{
			sc.AuthEngineUseEncryption = true;
			sc.CloudSMSUseEncryption = true;
			sc.AuthEngineDisableUserVerification = true;

			startAuthEngineServer();
			startCloudSMSServer();

			var user = insertUser();
			setupTestUserWithoutPincode(user);

			var gamr = (GetAvailableModulesRet)aeServerLogic.GetAvailableModules(new GetAvailableModules() { User = user.User, Org = user.Org });
			Assert.AreEqual(1, gamr.Modules.Count);
		}

		public class TestAdOrLocalUsersGetts : LocalUsersGetter
		{
			private TestWithServers servers;
			public TestAdOrLocalUsersGetts(TestWithServers servers)
			{
				this.servers = servers;
			}
			protected override List<AddFullDetails> GetLocalUsers()
			{
				var baseList = base.GetLocalUsers();

				var afd = servers.getInsertUserData();

				checkUserProviders(afd);

				baseList.Add(afd);
				return baseList;
			}
		}

		/* 
		 * https://bitbucket.org/stevenwright/sms2/issue/55/deactivating-providers-as-admin-crashes
		 */
		[Test]
		public void TestDeactivatingProvidersCrashesPlatform()
		{
			sc.AuthEngineUseEncryption = true;

			Assert.AreEqual(sc.Providers[0].Name, "CloudSMS");

			startAuthEngineServer();

			var user = insertUser();
			//setupTestUserWithoutPincode(user);

			var userId = (Int64)QueryValSQL("SELECT ID FROM SMS_CONTACT WHERE AD_USERNAME = 'test.user'")[0][0];
			var cloudSMSId = (int)QueryValSQL("SELECT id FROM AuthProviders WHERE name = 'CloudSMS'")[0][0];
			var oathCalcId = (int)QueryValSQL("SELECT id FROM AuthProviders WHERE name = 'OATHCalc'")[0][0];

			var initialCloudSMSLine = QueryValSQL(string.Format(
				"SELECT active, selected FROM UserProviders WHERE userId = '{0}' AND authProviderId = '{1}' ",
				userId, cloudSMSId
			))[0];
			var initialOathCalcLine = QueryValSQL(string.Format(
				"SELECT active, selected FROM UserProviders WHERE userId = '{0}' AND authProviderId = '{1}' ",
				userId, oathCalcId
			))[0];

			Assert.AreEqual(1, (byte)initialCloudSMSLine[0]);
			Assert.AreEqual(1, (byte)initialCloudSMSLine[1]);

			Assert.AreEqual(1, (byte)initialOathCalcLine[0]);
			Assert.AreEqual(0, (byte)initialOathCalcLine[1]);

			StopAuthEngineAndCloudSMS();



			Assert.AreEqual(sc.Providers[0].Name, "CloudSMS");
			sc.Providers[0].Enabled = false;
			sc.Providers[0].Default = false;
			sc.Providers[1].Default = true;

			var fake_registry = new Registry();
			fake_registry.AddOrSet<IUsersGetter>(new TestAdOrLocalUsersGetts(this));

			startAuthEngineServer(fake_registry);

			var cloudSMSLine = QueryValSQL(string.Format(
				"SELECT active, selected FROM UserProviders WHERE userId = '{0}' AND authProviderId = '{1}' ",
				userId, cloudSMSId
			))[0];
			var oathCalcLine = QueryValSQL(string.Format(
				"SELECT active, selected FROM UserProviders WHERE userId = '{0}' AND authProviderId = '{1}' ",
				userId, oathCalcId
			))[0];

			Assert.AreEqual(0, (byte)cloudSMSLine[0]);
			Assert.AreEqual(0, (byte)cloudSMSLine[1]);

			Assert.AreEqual(1, (byte)oathCalcLine[0]);
			Assert.AreEqual(1, (byte)oathCalcLine[1]);
		}
	}

	public partial class TestAuthEngine
	{
		[TestFixture]
		public class OverrideWithADInfo : TestWithServers
		{
			[Test]
			public void OverridesWhenTrue()
			{
				sc.AuthEngineOverrideWithADInfo = true;
				
				AddFullDetails afd = overrideTestLogic("1234", "4321", @"test@mail.com");
				
				var changedPhoneAndMobile = getPhoneAndMobileAndEmail(afd.User);
				Assert.That((string)changedPhoneAndMobile[0][0], Is.EqualTo(afd.Phone));
				Assert.That((string)changedPhoneAndMobile[0][1], Is.EqualTo(afd.Mobile));
				Assert.That((string)changedPhoneAndMobile[0][2], Is.EqualTo(afd.Email));
			}

			[Test]
			public void OverridesWhenFalseBecausePhoneOverridedEquals0()
			{
				AddFullDetails afd = overrideTestLogic("1234", "4321", @"test@mail.com");

				var changedPhoneAndMobile = getPhoneAndMobileAndEmail(afd.User);
				Assert.That((string)changedPhoneAndMobile[0][0], Is.EqualTo(afd.Phone));
				Assert.That((string)changedPhoneAndMobile[0][1], Is.EqualTo(afd.Mobile));
				Assert.That((string)changedPhoneAndMobile[0][2], Is.EqualTo(afd.Email));
			}

			[Test]
			public void OverridesWhenEmptyEvenIfFalse()
			{
				AddFullDetails afd = overrideTestLogic("", "", "");

				var changedPhoneAndMobile = getPhoneAndMobileAndEmail(afd.User);
				Assert.That((string)changedPhoneAndMobile[0][0], Is.EqualTo(afd.Phone));
				Assert.That((string)changedPhoneAndMobile[0][1], Is.EqualTo(afd.Mobile));
				Assert.That((string)changedPhoneAndMobile[0][2], Is.EqualTo(afd.Email));
			}
			
			AddFullDetails overrideTestLogic(string phone, string mobile, string email)
			{
				sc.AuthEngineUseEncryption = true;
				sc.CloudSMSUseEncryption = true;
				sc.AuthEngineDefaultEnabled = true;

				var afd = getInsertUserData("");
				afd.Phone = phone;
				afd.Mobile = mobile;
				afd.Email = email;

				var fakeUserGetter = new FakeUserGetterWithList();
				fakeUserGetter.UserList.Clear();
				fakeUserGetter.UserList.Add(afd);
				
				var fakeregistry = new Registry();
				fakeregistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				startAuthEngineServer(fakeregistry);

				var phoneAndMobile = getPhoneAndMobileAndEmail(afd.User);
				Assert.That((string)phoneAndMobile[0][0], Is.EqualTo(afd.Phone));
				Assert.That((string)phoneAndMobile[0][1], Is.EqualTo(afd.Mobile));
				Assert.That((string)phoneAndMobile[0][2], Is.EqualTo(afd.Email));

				afd = getInsertUserData("");
				afd.Phone = "1111";
				afd.Mobile = "2222";
				afd.Email = "test@mailmodified.com";
				fakeUserGetter.UserList.Clear();
				fakeUserGetter.UserList.Add(afd);
				
				aeServerLogic.getUsers();
				return afd;
			}

			private IList<object[]> getPhoneAndMobileAndEmail(string user)
			{
				return QueryValSQL(
					string.Format(@"SELECT [INPUT_PHONE_NUMBER], [MOBILE_NUMBER], [EMAIL]
	FROM SMS_CONTACT WHERE AD_USERNAME = '{0}'", user));
			}
		}
		
		[TestFixture]
		public class NHS : TestWithServers
		{
			[Test]
			public void ShouldSetProvider()
			{
				sc.AuthEngineDefaultDomain = null;
				startAuthEngineServer();
				startCloudSMSServer();

				UserProvider up = new UserProvider() { Name = "NHS", Config = "" };
				SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
				Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
				Assert.AreEqual(1, ret.Out);

				ValidateUser vu = new ValidateUser();
				vu.User = this.Username;
				ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			}

			[Test]
			public void ShouldAddProvider()
			{
				var nhsProvider = sc.Providers.Single(p => p.Name == "NHS");
				sc.Providers.Remove(nhsProvider);

				Assert.AreEqual(6, sc.Providers.Count);

				using (var queries = DBQueriesProvider.Get())
				{
					queries.NonQuery(string.Format(@"DELETE FROM SMS_CONTACT
						WHERE AD_USERNAME = '{0}' AND ORG_NAME = '{1}'", this.Username, getOrgOrBaseDN()));
				}

				startAuthEngineServer();

				Assert.AreEqual(6, getUserProvidersCount());

				var up = new UserProvider() { Name = "NHS", Config = "" };
				var setUserProviderCmd = new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up };
				SetUserProviderRet ret;

				Assert.AreEqual(6, getUserProvidersCount());
				ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(setUserProviderCmd));
				Assert.IsFalse(string.IsNullOrEmpty(ret.Error));
				Assert.AreEqual(0, ret.Out);

				StopAuthEngineAndCloudSMS();
				sc.Providers.Add(nhsProvider);
				startAuthEngineServer();

				Assert.AreEqual(7, getUserProvidersCount());
				ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(setUserProviderCmd));
				Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
				Assert.AreEqual(1, ret.Out);
			}

			private int getUserProvidersCount()
			{
				var org = getOrgOrBaseDN();
				using (var queries = DBQueriesProvider.Get())
				{
					var userProvidersCount = queries.QueryScalar<int>(
						string.Format(@"SELECT COUNT(*) FROM UserProviders
							WHERE userId = {0}", queries.GetUserId(this.Username, org)));
					return userProvidersCount;
				}
			}
		}

		[TestFixture]
		public class Poll : TestWithServers
		{
			protected FakeUserGetterWithList fakeUserGetter;
			protected AddFullDetails admin;
			protected AddFullDetails user1;
			protected AddFullDetails user2;
			
			void setupAuthEngine() {
				var testRegistry = new Registry();
				fakeUserGetter = new FakeUserGetterWithList();
				
				user1 = getInsertUserData("", "user1");
				admin = getInsertAdminData();
				user2 = getInsertUserData("", "user2");
				
				fakeUserGetter.UserList.Add(user1);
				fakeUserGetter.UserList.Add(admin);
				fakeUserGetter.UserList.Add(user2);
				
				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				RunSQL(@"DELETE FROM SMS_CONTACT");
				startAuthEngineServer(testRegistry);
			}
			
			[Test]
			public void ShouldUpdateUserTypes() {
				setupAuthEngine();
				var users = QueryValSQL(string.Format(@"
SELECT ID FROM SMS_CONTACT
WHERE [USER_TYPE]='{0}'", UserType.User));
				Assert.That(users.Count, Is.EqualTo(2));
				
				var admins = QueryValSQL(string.Format(@"
SELECT ID FROM SMS_CONTACT
WHERE [USER_TYPE]='{0}'", UserType.Administrator));
				Assert.That(admins.Count, Is.EqualTo(1));
				
				user2.UserType = UserType.Administrator;
				aeServerLogic.getUsers();
				
				admins = QueryValSQL(string.Format(@"
SELECT ID, utOverridden FROM SMS_CONTACT
WHERE [USER_TYPE]='{0}' ORDER BY ID", UserType.Administrator));
				Assert.That(admins.Count, Is.EqualTo(2));
				Assert.That(admins[1][1], Is.EqualTo(false));
			}
			
			[Test]
			public void ShouldNotOverrideOverriddenByUpdate() {
				setupAuthEngine();
				var users = QueryValSQL(string.Format(@"
SELECT ID FROM SMS_CONTACT
WHERE [USER_TYPE]='{0}'", UserType.User));
				Assert.That(users.Count, Is.EqualTo(2));
				
				var admins = QueryValSQL(string.Format(@"
SELECT ID FROM SMS_CONTACT
WHERE [USER_TYPE]='{0}'", UserType.Administrator));
				Assert.That(admins.Count, Is.EqualTo(1));
				
				var ufd = new UpdateFullDetails {
					Identity = new FakeIdentity(admin),
					Authenticated = true,
					External = false,
					User = user2.User,
					Org = user2.Org,
					UserType = UserType.Administrator,
					Phone = user2.Phone,
					Mobile = user2.Mobile,
					Fullname = user2.Fullname,
					AuthEnabled = user2.AuthEnabled.Value,
					PinCode = user2.PinCode,
					Email = user2.Email,
                    UPN = user2.User + "@" + user2.Org
				};
				var ret = (UpdateFullDetailsRet)aeServerLogic.Actioner.Do(ufd);
				Assert.That(ret.Error, Is.Null.Or.Empty, "Error returned: " + ret.Error);
				admins = QueryValSQL(string.Format(@"
SELECT ID, utOverridden FROM SMS_CONTACT
WHERE [USER_TYPE]='{0}' ORDER BY ID", UserType.Administrator));
				Assert.That(admins.Count, Is.EqualTo(2));
				Assert.That(admins[1][1], Is.EqualTo(true));
				
				admin.UserType = UserType.User;
				aeServerLogic.getUsers();
				admins = QueryValSQL(string.Format(@"
SELECT ID, utOverridden FROM SMS_CONTACT
WHERE [USER_TYPE]='{0}' ORDER BY ID", UserType.Administrator));
				Assert.That(admins.Count, Is.EqualTo(1));
				Assert.That(admins[0][1], Is.EqualTo(true));
			}
		}
		
		[TestFixture]
		public class UserList : TestWithServers
		{
			[Test]
			public void FilterByProvider() {
				
				var testRegistry = new Registry();
				var fakeUserGetter = new FakeUserGetterWithList();
				var fakeMailSender = new FakeMailSender();
				
				var user1 = getInsertUserData("", "user1");
				var user2 = getInsertUserData("", "user2");
				var user3 = getInsertUserData("", "user3");
				
				fakeUserGetter.UserList.Add(user1);
				fakeUserGetter.UserList.Add(user2);
				fakeUserGetter.UserList.Add(user3);
				
				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				
				RunSQL(@"DELETE FROM SMS_CONTACT");
				
				startAuthEngineServer(testRegistry);
				
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider { User = user2.User, Org = user1.Org, Provider = new UserProvider { Name = "OneTime", Config = "" } }));

				var users = QueryValSQL(@"SELECT ID FROM SMS_CONTACT");
				Assert.That(users.Count, Is.EqualTo(3));
				
				var usersInProvider = (UsersRet)aeServerLogic.UserList(new Users { Total = fakeUserGetter.UserList.Count * 3, Org = user1.Org, FName = "" });
				Assert.That(usersInProvider.Users.Count, Is.EqualTo(3));
				
				usersInProvider = (UsersRet)aeServerLogic.UserList(new Users { Total = fakeUserGetter.UserList.Count * 3, Org = user1.Org, FName = "XenMobile-Enrolment" });
				Assert.That(usersInProvider.Users.Count, Is.EqualTo(1));
				
				usersInProvider = (UsersRet)aeServerLogic.UserList(new Users { Total = fakeUserGetter.UserList.Count * 3, Org = user1.Org, FName = "CloudSMS" });
				Assert.That(usersInProvider.Users.Count, Is.EqualTo(2));
			}
		}
		
		[TestFixture]
		public class Email : TestWithServers
		{
			AddFullDetails getUser() {
				var afd = new AddFullDetails
				{
					Fullname = "Test User",
					User = "test.user",
					Org = "testdomain",
					UserType = UserType.User,
					Enabled = true,
					//Mobile = "+447525218010",
					Mobile = "00541169531549",
					OrgType = "DOMAIN",
					Phone = "",
					Sid = "123",
					Email = "steven@wrightccs.com",
					PinCode = string.Empty,
					Providers = new List<UserProvider> { 
						new UserProvider { Name = "CloudSMS", Enabled = true },
						new UserProvider
						{
							Name = "OATHCalc",
							Config = "HOTP,"
									+ HexConversion.ToString(Encoding.UTF8.GetBytes("12345678901234567890"))
									+ ",0",
									Enabled = true
						}, 
						new UserProvider { Name = "PINTAN", Enabled = true } ,
						new UserProvider { Name = "OneTime", Enabled = true } ,
						new UserProvider { Name = "Email", Enabled = true } ,
					},
					AuthEnabled = sc.AuthEngineDefaultEnabled,
				};
				return afd;
			}
			
			class DummyLogger : ILogger
			{
				readonly IList<LogEntry> entries = new List<LogEntry>();
				public void Write(LogEntry message)
				{
					entries.Add(message);
				}
				public IList<LogEntry> getEntries() { return entries; }
			}
					
			[Test]
			public void TestFailException()
			{
				
				var errorLogger = new DummyLogger();
				Logger.Instance.AddLogger(errorLogger, LogLevel.Error);
			
				sc.AuthEngineDefaultDomain = null;
				
				var testRegistry = new Registry();
				var fakeMailSender = new FakeMailSender();
				
				var fakeUserGetter = new FakeUserGetterWithList();
				
				var afd = getUser();
				fakeUserGetter.UserList.Add(afd);
				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				
				startAuthEngineServer(testRegistry);
				startCloudSMSServer();

				var up = new UserProvider { Name = "Email", Config = "" };
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider { User = afd.User, Org = afd.Org, Provider = up }));
				Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
				Assert.AreEqual(1, ret.Out);

				var vu = new ValidateUser { User = afd.Org + "\\" + afd.User };
				var vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
				
				var errors = errorLogger.getEntries();
				Assert.That(errors.Count, Is.EqualTo(2));
				Assert.That(errors[1].Message, Is.StringContaining("resolve"));
			}
			
			[Test]
			public void ShouldSetProvider()
			{
				sc.AuthEngineDefaultDomain = null;
				
				var testRegistry = new Registry();
				var fakeMailSender = new FakeMailSender();
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				
				startAuthEngineServer(testRegistry);
				startCloudSMSServer();

				UserProvider up = new UserProvider() { Name = "Email", Config = "" };
				SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = this.Username, Org = this.Org, Provider = up }));
				Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
				Assert.AreEqual(1, ret.Out);

				ValidateUser vu = new ValidateUser();
				vu.User = this.Username;
				ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
				Assert.That(fakeMailSender.MailsSent.Count, Is.EqualTo(1));
			}
		}
		
		public static class ServerLogicTest 
		{
			[TestFixture]
			public class messages : TestWithServers
			{
				[Test]
				public void shouldNotFailGetMessages()
				{
					var testRegistry = new Registry();
					var fakeUserGetter = new FakeUserGetterWithList();
					testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
					startAuthEngineServer(testRegistry);
					
					var ret = (AllMsgsRet)aeServerLogic.getMessages(new AllMsgs());
					Assert.That(ret.Messages.Count, Is.EqualTo(5));
				}
				
				[Test]
				public void shouldUpdateSecondMessage()
				{
					var testRegistry = new Registry();
					var fakeUserGetter = new FakeUserGetterWithList();
					testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
					startAuthEngineServer(testRegistry);
					
					var cmd = new UpdateMessage {
						Label = @"OATH Setup E-mail",
						Title = "test title",
						Message = "test message"
					};
					var updateRet = aeServerLogic.updateMessage(cmd);
					Assert.That(updateRet.Error, Is.Null.Or.Empty);
					
					var ret = (AllMsgsRet)aeServerLogic.getMessages(new AllMsgs());
					Assert.That(ret.Messages.Count, Is.EqualTo(5));
					
					Assert.That(ret.Messages[2].Message, Is.EqualTo(cmd.Message));
				}
			}
			
			[TestFixture]
			public class getDefaultProvider : TestWithServers
			{
				[Test]
				public void shouldFirstSelectDefaultWithADGroup()
				{
					var testRegistry = new Registry();
					var fakeUserGetter = new FakeUserGetterWithList();
					var afd = getInsertUserData();
					fakeUserGetter.UserList.Add(afd);
					testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
					startAuthEngineServer(testRegistry);
					
					afd.Providers.ForEach(x => { x.Enabled = true; x.Provider.Default = false; } );
					
					var oneTime = afd.Providers.Single(x => x.Name == "OneTime");
					oneTime.Provider.AdGroup = "Test";
					oneTime.Provider.Default = true;
					
					var otherWithADGroup = afd.Providers.Single(x => x.Name == "PINTAN");
					otherWithADGroup.Provider.AdGroup = "Test";
					
					var defaultProvider = aeServerLogic.getDefaultProvider(afd);
					Assert.That(defaultProvider.Name == "OneTime");
				}
				
				[Test]
				public void shouldSelectDefaultNoGroupIfAdGroupNotEnabled()
				{
					var testRegistry = new Registry();
					var fakeUserGetter = new FakeUserGetterWithList();
					var afd = getInsertUserData();
					fakeUserGetter.UserList.Add(afd);
					testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
					startAuthEngineServer(testRegistry);
					
					afd.Providers.ForEach(x => { x.Enabled = true; x.Provider.Default = false; } );
					var oneTime = afd.Providers.Single(x => x.Name == "OneTime");
					oneTime.Provider.AdGroup = "Test";
					oneTime.Provider.Default = true;
					oneTime.Enabled = false;
					
					var otherWithADGroup = afd.Providers.Single(x => x.Name == "PINTAN");
					otherWithADGroup.Provider.Default = true;
					
					var defaultProvider = aeServerLogic.getDefaultProvider(afd);
					Assert.That(defaultProvider.Name == "PINTAN");
				}
				
				[Test]
				public void willThrowExceptionWhenNoneFound()
				{
					var testRegistry = new Registry();
					var fakeUserGetter = new FakeUserGetterWithList();
					var afd = getInsertUserData("");
					fakeUserGetter.UserList.Add(afd);
					testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
					startAuthEngineServer(testRegistry);
					afd.Providers.Clear();
					Assert.Throws<ServerLogicException>(() => aeServerLogic.getDefaultProvider(afd));
				}
				
				[Test]
				public void willSelectFirstWhenNoneEnabled()
				{
					var testRegistry = new Registry();
					var fakeUserGetter = new FakeUserGetterWithList();
					var afd = getInsertUserData();
					fakeUserGetter.UserList.Add(afd);
					testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
					startAuthEngineServer(testRegistry);
					afd.Providers.ForEach(x => x.Enabled = false);
					Assert.That(aeServerLogic.getDefaultProvider(afd).Name == afd.Providers[0].Name);
				}
			}
		
			[TestFixture]
			public class vault : TestWithServers {
				[Test]
				public void checkPassword() {
					ServerLogic.setSetting("RADIUS", "PasswordVaulting", "True");
					startAuthEngineServer();
					
					var user = "KDev";
					var org = Org;
					
					Assert.That(SystemConfiguration.getBoolOrDef(
						ServerLogic.getSetting("PasswordVaulting", "RADIUS")
						, false), Is.True);
					
					var ret = new SetUVaultRet();
					aeServerLogic.CheckUserVault(user, org, ret);
				}
			}
		}
		
		[TestFixture]
		public class UserProvidersTest : TestWithServers
		{
			[Test]
			public void EnabledProviderInGroupOverridesNoneGroup()
			{
				var testRegistry = new Registry();
				var fakeMailSender = new FakeMailSender();
				var fakeUserGetter = new FakeUserGetterWithList();
				
				var afd = getInsertUserData("");
				
				fakeUserGetter.UserList.Add(afd);
				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				startAuthEngineServer(testRegistry);
				
				var ret = (UserProvidersRet)(aeServerLogic.GetUserProviders(
					new UserProviders { User = afd.User, Org = afd.Org }
				));
				Assert.That(ret.Providers.Single(x => x.Name == "CloudSMS").Selected, Is.True);
				
				sc.Providers.Single(x => x.Name == "OneTime").AdGroup = "Override";
				try {
					aeServerLogic.getUsers();
					
					try
					{
						while (!Monitor.TryEnter(aeServerLogic.LockUpdateUsersObj))
						{
							Thread.Sleep(100);
						}
					}
					finally
					{
						Monitor.Exit(aeServerLogic.LockUpdateUsersObj);
					}
					
					ret = (UserProvidersRet)(aeServerLogic.GetUserProviders(
						new UserProviders { User = afd.User, Org = afd.Org }
					));
					Assert.That(ret.Providers.Single(x => x.Name == "CloudSMS").Selected, Is.False);
					Assert.That(ret.Providers.Single(x => x.Name == "OneTime").Selected, Is.True);
				} finally {
					sc.Providers.Single(x => x.Name == "OneTime").AdGroup = "Override";
				}
			}
		}
		
		[TestFixture]
		public class OneTime : TestWithServers
		{
			[Test]
			public void ShouldSetProvider()
			{
				var testRegistry = new Registry();
				var fakeMailSender = new FakeMailSender();

				sc.AuthEngineDefaultDomain = null;
				
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				startAuthEngineServer(testRegistry);

				startCloudSMSServer();

				var up = new UserProvider { Name = "OneTime", Config = "" };
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider { User = Username, Org = Org, Provider = up }));
				Assert.IsTrue(string.IsNullOrEmpty(ret.Error));
				Assert.AreEqual(1, ret.Out);

				var vu = new ValidateUser();
				vu.User = Username;
				vu.PinCode = "";
				var vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			}
			
			[Test]
			public void ShouldSendMailOnSelect() {
				var testRegistry = new Registry();
				var fakeMailSender = new FakeMailSender();
				var fakeUserGetter = new FakeUserGetterWithList();
				
				var afd = getInsertUserData("");
				
				fakeUserGetter.UserList.Add(afd);
				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				startAuthEngineServer(testRegistry);
				
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider { User = afd.User, Org = afd.Org, Provider = new UserProvider { Name = "OneTime", Config = "" } }));
				Assert.That(fakeMailSender.MailsSent.Count == 1 );
			}
			
			[Test]
			public void ShouldValidate() {
				var testRegistry = new Registry();
				var fakeUserGetter = new FakeUserGetterWithList();
				var fakeMailSender = new FakeMailSender();
				
				var afd = getInsertUserData("");

				fakeUserGetter.UserList.Add(afd);
				
				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				
				sc.AuthEngineChallengeResponse = false;
				startAuthEngineServer(testRegistry);
				
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider { User = afd.User, Org = afd.Org, Provider = new UserProvider { Name = "OneTime", Config = "" } }));

				var pin = testRegistry.Get<string>();
				
				var logs = QueryValSQL(
					string.Format(
						@"SELECT * FROM SMS_LOG WHERE AD_USERNAME='{0}' and ORG_NAME='{1}'"
	              		, afd.User, afd.Org
	              	));
				Assert.AreEqual(1, logs.Count, "There should be 1 token only generated for this user");
				
				var vu = new ValidateUser();
				vu.User = afd.Org + "\\" + afd.User;
				vu.PinCode = pin;
				var vuret = (ValidateUserRet)aeServerLogic.ValidateUser(vu);
				Assert.That(vuret.Validated, Is.True);
				Assert.That(vuret.Error, Is.Null.Or.Empty);
				logs = QueryValSQL(
					string.Format(
						@"SELECT * FROM SMS_LOG WHERE AD_USERNAME='{0}' and ORG_NAME='{1}'"
	              		, afd.User, afd.Org
	              	));
				Assert.AreEqual(1, logs.Count, "There should be 1 token only generated for this user");
			}
			
			[Test]
			public void ShouldValidateChallenge() {
				var testRegistry = new Registry();
				var fakeUserGetter = new FakeUserGetterWithList();
				var fakeMailSender = new FakeMailSender();
				
				var afd = getInsertUserData("");

				fakeUserGetter.UserList.Add(afd);
				
				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				
				sc.AuthEngineChallengeResponse = true;
				startAuthEngineServer(testRegistry);
				
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider { User = afd.User, Org = afd.Org, Provider = new UserProvider { Name = "OneTime", Config = "" } }));

				var pin = testRegistry.Get<string>();
				
				var vu = new ValidateUser {
					User = afd.Org + "\\" + afd.User
				};
				var vuret = (ValidateUserRet)aeServerLogic.ValidateUser(vu);
				Assert.That(vuret.Validated, Is.True);
				Assert.That(vuret.Error, Is.Null.Or.Empty);
				
				var vp = new ValidatePin {
					Pin = pin,
					User = afd.Org + "\\" + afd.User,
					State = vuret.State
				};
				var vpret = (ValidatePinRet)aeServerLogic.ValidatePin(vp);
				Assert.That(vpret.Validated, Is.True);
				Assert.That(vpret.Error, Is.Null.Or.Empty);
			}
			
			[Test]
			public void LoadsTemplateFromPasscodeTemplate() {
				var testRegistry = new Registry();
				var fakeUserGetter = new FakeUserGetterWithList();
				var fakeMailSender = new FakeMailSender();
				
				var afd = getInsertUserData("");

				fakeUserGetter.UserList.Add(afd);
				
				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				
				sc.AuthEngineChallengeResponse = false;
				startAuthEngineServer(testRegistry);
				
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider { User = afd.User, Org = afd.Org, Provider = new UserProvider { Name = "OneTime", Config = "" } }));

				var mail = fakeMailSender.MailsSent[0];
				Assert.That(mail.Template, Is.StringContaining("OneTime secure passcode"));
			}
			
			[Test]
			public void ShouldNotValidateTwice() {
				var testRegistry = new Registry();
				var fakeUserGetter = new FakeUserGetterWithList();
				var fakeMailSender = new FakeMailSender();
				
				var afd = getInsertUserData("");

				fakeUserGetter.UserList.Add(afd);
				
				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				
				sc.AuthEngineChallengeResponse = false;
				startAuthEngineServer(testRegistry);
				
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider { User = afd.User, Org = afd.Org, Provider = new UserProvider { Name = "OneTime", Config = "" } }));

				var pin = testRegistry.Get<string>();
				
				var vu = new ValidateUser();
				vu.User = afd.Org + "\\" + afd.User;
				vu.PinCode = pin;
				var vuret = (ValidateUserRet)aeServerLogic.ValidateUser(vu);
				Assert.That(vuret.Validated, Is.True);
				vuret = (ValidateUserRet)aeServerLogic.ValidateUser(vu);
				Assert.That(vuret.Validated, Is.False);
			}
			
			[Test]
			public void ShouldReValidate() {
				var testRegistry = new Registry();
				var fakeUserGetter = new FakeUserGetterWithList();
				var fakeMailSender = new FakeMailSender();
				
				var afd = getInsertUserData("");

				fakeUserGetter.UserList.Add(afd);
				
				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				
				sc.AuthEngineChallengeResponse = false;
				startAuthEngineServer(testRegistry);
				
				var setUP = new SetUserProvider { User = afd.User, Org = afd.Org, Provider = new UserProvider { Name = "OneTime", Config = "" } };
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(setUP));

				var pin = testRegistry.Get<string>();
				
				var vu = new ValidateUser();
				vu.User = afd.Org + "\\" + afd.User;
				vu.PinCode = pin;
				var vuret = (ValidateUserRet)aeServerLogic.ValidateUser(vu);
				Assert.That(vuret.Validated, Is.True);
				vuret = (ValidateUserRet)aeServerLogic.ValidateUser(vu);
				Assert.That(vuret.Validated, Is.False);
				
				ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(setUP));
				pin = testRegistry.Get<string>();
				vu.PinCode = pin;
				vuret = (ValidateUserRet)aeServerLogic.ValidateUser(vu);
				Assert.That(vuret.Validated, Is.True);
			}
			
			[Test]
			public void ShouldFailExpired() {
				var testRegistry = new Registry();
				var fakeUserGetter = new FakeUserGetterWithList();
				var fakeMailSender = new FakeMailSender();
				
				var afd = getInsertUserData("");

				fakeUserGetter.UserList.Add(afd);
				
				testRegistry.AddOrSet<IUsersGetter>(fakeUserGetter);
				testRegistry.AddOrSet<IMailSender>(fakeMailSender);
				
				sc.AuthEngineChallengeResponse = false;
				sc.OneTimeTokenExpireTimeMinutes = -2;
				startAuthEngineServer(testRegistry);
				
				var ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider { User = afd.User, Org = afd.Org, Provider = new UserProvider { Name = "OneTime", Config = "" } }));

				var pin = testRegistry.Get<string>();
				
				var vu = new ValidateUser();
				vu.User = afd.Org + "\\" + afd.User;
				vu.PinCode = pin;
				var vuret = (ValidateUserRet)aeServerLogic.ValidateUser(vu);
				Assert.That(vuret.Validated, Is.False);
				Assert.That(vuret.Error, Is.Not.Null.And.Not.Empty);
			}
		}
	}
}
