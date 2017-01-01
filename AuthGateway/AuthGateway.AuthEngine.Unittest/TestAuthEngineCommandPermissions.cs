using System;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using AuthGateway.AuthEngine.Logic;

using NUnit.Framework;

namespace AuthGateway.AuthEngine.Unittest
{
	[TestFixture]
	public class TestAuthEngineCommandPermissions : TestWithServers
	{
		protected AddFullDetails TestAdmin { get; set; }
		protected AddFullDetails TestUser { get; set; }
		protected AddFullDetails TestUser2 { get; set; }
		protected AddFullDetails TestCacho { get; set; }

		[SetUp]
		public void Setup()
		{
			Console.WriteLine("Setup");
			startAuthEngineServer();
			TestAdmin = insertAdminUser();
			TestCacho = insertUserCachoDomain();
			TestUser = insertUser();
			TestUser2 = insertUser2();
		}
		[Test]
		public void TestUserListAdminShouldGetAll()
		{
			Users users = new Users();
			users.Identity = new FakeIdentity(TestAdmin);
			users.Authenticated = true;
			users.External = true;
			users.Org = TestAdmin.Org;
			users.Total = 500;
			UsersRet ret = (UsersRet)aeServerLogic.Actioner.Do(users);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error expected: " + ret.Error);
			Assert.AreEqual(3, ret.Users.Count, "All users from the domain expected.");
		}

		[Test]
		public void TestUserListUserShouldGetOne()
		{
			Users users = new Users();
			users.Identity = new FakeIdentity(TestUser);
			users.Authenticated = true;
			users.External = true;
			users.Org = TestUser.Org;
			users.Total = 500;
			UsersRet ret = (UsersRet)aeServerLogic.Actioner.Do(users);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error expected: " + ret.Error);
			Assert.AreEqual(1, ret.Users.Count, "Only himself was expected.");
		}

		[Test]
		public void TestUserListNoAuthShouldGetNone()
		{
			Users users = new Users();
			users.External = true;
			users.Org = TestAdmin.Org;
			users.Total = 500;
			UsersRet ret = (UsersRet)aeServerLogic.Actioner.Do(users);
			Assert.IsFalse(string.IsNullOrEmpty(ret.Error), "And error was expected");
			Assert.AreEqual(0, ret.Users.Count, "No user should be listed.");
		}

		[Test]
		public void UpdateFullDetailsSameUserShouldPass()
		{
			UpdateFullDetails ufd = new UpdateFullDetails();
			ufd.Identity = new FakeIdentity(TestUser);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;
			ufd.Phone = "123";
			ufd.Mobile = "123";
			ufd.Fullname = "fullname";
            ufd.UPN = TestUser.User + "@" + TestUser.Org;
			ufd.UserType = UserType.User;
			UpdateFullDetailsRet ret = (UpdateFullDetailsRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected but got:" + ret.Error);
		}

		[Test]
		[ExpectedException(typeof(PermissionException))]
		public void UpdateFullDetailsSameUserNotAdminPromoteToAdminShouldFail()
		{
			UpdateFullDetails ufd = new UpdateFullDetails();
			ufd.Identity = new FakeIdentity(TestUser);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;
			ufd.Phone = "123";
			ufd.Mobile = "123";
			ufd.Fullname = "fullname";
			ufd.UserType = UserType.Administrator;
            ufd.UPN = string.Format("{0}@{1}", TestUser.User, TestUser.Org);
			UpdateFullDetailsRet ret = (UpdateFullDetailsRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected but got:" + ret.Error);
			aeServerLogic.checkSameDomainAndAdmin(ufd);
		}

		[Test]
		public void UpdateFullDetailsOtherUserNotAdminShouldFail()
		{
			UpdateFullDetails ufd = new UpdateFullDetails();
			ufd.Identity = new FakeIdentity(TestUser);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser2.User;
			ufd.Org = TestUser2.Org;
			ufd.Phone = "123";
			ufd.Mobile = "123";
			ufd.Fullname = "fullname";
			ufd.UserType = UserType.Administrator;
			UpdateFullDetailsRet ret = (UpdateFullDetailsRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsFalse(string.IsNullOrEmpty(ret.Error), "An error was expected");
		}

		[Test]
		public void UpdateFullDetailsAdminShouldPromoteToAdmin()
		{
			UpdateFullDetails ufd = new UpdateFullDetails();
			ufd.Identity = new FakeIdentity(TestAdmin);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;
			ufd.Phone = "123";
			ufd.Mobile = "123";
			ufd.Fullname = "fullname";
            ufd.UPN = string.Format("{0}@{1}", TestUser.User, TestUser.Org);
			ufd.UserType = UserType.Administrator;
			UpdateFullDetailsRet ret = (UpdateFullDetailsRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected but got:" + ret.Error);
			ufd.Identity = new FakeIdentity(TestUser);
			aeServerLogic.checkSameDomainAndAdmin(ufd);
		}

		[Test]
		public void DetailsToSameUserShouldPass()
		{
			Details ufd = new Details();
			ufd.Identity = new FakeIdentity(TestUser);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;            
			DetailsRet ret = (DetailsRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected");
			Assert.AreEqual(TestUser.Fullname, ret.Fullname);
		}

		[Test]
		public void DetailsToOtherUserShouldFail()
		{
			Details ufd = new Details();
			ufd.Identity = new FakeIdentity(TestUser);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser2.User;
			ufd.Org = TestUser2.Org;
			DetailsRet ret = (DetailsRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsFalse(string.IsNullOrEmpty(ret.Error), "An error was expected");
			Assert.IsTrue(string.IsNullOrEmpty(ret.Mobile));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Phone));
		}

		[Test]
		public void DetailsToAdminShouldPass()
		{
			Details ufd = new Details();
			ufd.Identity = new FakeIdentity(TestAdmin);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;
			DetailsRet ret = (DetailsRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected");
			Assert.AreEqual(TestUser.Fullname, ret.Fullname);
		}

		[Test]
		public void GetUserProvidersAdminShouldPass()
		{
			UserProviders ufd = new UserProviders();
			ufd.Identity = new FakeIdentity(TestAdmin);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;
			UserProvidersRet ret = (UserProvidersRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected");
			aeServerLogic.checkSameDomainSameUserOrAdmin(ufd);
		}

		[Test]
		[ExpectedException(typeof(PermissionException))]
		public void GetUserProvidersOtherUserShouldFail()
		{
			UserProviders ufd = new UserProviders();
			ufd.Identity = new FakeIdentity(TestUser2);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;
			UserProvidersRet ret = (UserProvidersRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsFalse(string.IsNullOrEmpty(ret.Error), "An error was expected");
			aeServerLogic.checkSameDomainSameUserOrAdmin(ufd);
		}

		[Test]
		public void GetUserProvidersSameUserShouldPass()
		{
			UserProviders ufd = new UserProviders();
			ufd.Identity = new FakeIdentity(TestUser);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;
			UserProvidersRet ret = (UserProvidersRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected");
			aeServerLogic.checkSameDomainSameUserOrAdmin(ufd);
		}

		[Test]
		public void SetUserProvidersAdminShouldPass()
		{
			SetUserProvider ufd = new SetUserProvider();
			ufd.Identity = new FakeIdentity(TestAdmin);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;
			ufd.Provider = new UserProvider() { Name = "CloudSMS", Selected = true, Enabled = true };
			SetUserProviderRet ret = (SetUserProviderRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected");
			aeServerLogic.checkSameDomainSameUserOrAdmin(ufd);
		}

		[Test]
		[ExpectedException(typeof(PermissionException))]
		public void SetUserProvidersOtherUserShouldFail()
		{
			SetUserProvider ufd = new SetUserProvider();
			ufd.Identity = new FakeIdentity(TestUser2);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;
			ufd.Provider = new UserProvider() { Name = "CloudSMS", Selected = true, Enabled = true };
			SetUserProviderRet ret = (SetUserProviderRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsFalse(string.IsNullOrEmpty(ret.Error), "An error was expected");
			aeServerLogic.checkSameDomainSameUserOrAdmin(ufd);
		}

		[Test]
		public void SetUserProvidersSameUserShouldPass()
		{
			SetUserProvider ufd = new SetUserProvider();
			ufd.Identity = new FakeIdentity(TestUser);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;
			ufd.Provider = new UserProvider() { Name = "CloudSMS", Selected = true, Enabled = true };
			SetUserProviderRet ret = (SetUserProviderRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected");
			aeServerLogic.checkSameDomainSameUserOrAdmin(ufd);
		}

		[Test]
		public void UpdateFullDetailsSameUserShouldPassOrgToUpper()
		{
			Details ufd = new Details();
			ufd.Identity = new FakeIdentity(TestUser.Org.ToUpperInvariant() + "SARASA\\" + TestUser.User);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;
			aeServerLogic.AddReplacement(TestUser.Org + "SARASA", TestUser.Org);
			DetailsRet ret = (DetailsRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected");
			Assert.AreEqual(TestUser.Fullname, ret.Fullname);
			aeServerLogic.RemoveReplacement(TestUser.Org + "sarasa");
		}

		[Test]
		public void UpdateFullDetailsSameUserShouldPassOrgToLower()
		{
			Details ufd = new Details();
			ufd.Identity = new FakeIdentity(TestUser.Org.ToUpperInvariant() + "SARASA\\" + TestUser.User);
			ufd.Authenticated = true;
			ufd.External = true;
			ufd.User = TestUser.User;
			ufd.Org = TestUser.Org;
			aeServerLogic.AddReplacement(TestUser.Org + "sarasa", TestUser.Org);
			DetailsRet ret = (DetailsRet)aeServerLogic.Actioner.Do(ufd);
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "No error was expected");
			Assert.AreEqual(TestUser.Fullname, ret.Fullname);
			aeServerLogic.RemoveReplacement(TestUser.Org + "sarasa");
		}
	}
}
