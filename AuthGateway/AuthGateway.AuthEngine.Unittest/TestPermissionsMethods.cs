using System;
using System.Collections.Generic;
using System.Security.Principal;

using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.AuthEngine.Logic;

using NUnit.Framework;

namespace AuthGateway.AuthEngine.Unittest
{
    [TestFixture]
    public class TestPermissionsMethods : TestWithServers
    {
        [SetUp]
        public void Setup()
        {
					if (aeServerLogic == null)
					{
						aeServerLogic = new ServerLogic(sc);
						aeServerLogic.Init();
					}
        }
        [Test]
        public void TestCheckAdmin()
        {
            var afd = insertAdminUser();
            afd.Identity = new FakeIdentity() { Auth = true, Username = afd.Org + "\\" + afd.User };
            afd.Authenticated = true;
            aeServerLogic.checkAdmin(afd);
        }

        [Test]
        [ExpectedException(typeof(PermissionException))]
        public void TestCheckAdminFailNoAdmin()
        {
            var afd = insertUser();
            afd.Identity = new FakeIdentity() { Auth = true, Username = afd.Org + "\\" + afd.User };
            afd.Authenticated = true;
            aeServerLogic.checkAdmin(afd);
        }

        [Test]
        public void AdminRunningCommandToOtherUserShouldPass()
        {
            // A user in the same domain when you're admin
            var afd = insertAdminUser();
            afd.Identity = new FakeIdentity() { Auth = true, Username = afd.Org + "\\" + afd.User };
            afd.Authenticated = true;
            afd.User = "test.cacho";
            aeServerLogic.checkSameDomainAndAdmin(afd);
        }

        [Test]
        [ExpectedException(typeof(PermissionException))]
        public void UserRunningCommandToOtherUserShouldFail()
        {
            // A user in the same domain when you're NOT admin
            var afd = insertUser();
            afd.Identity = new FakeIdentity() { Auth = true, Username = afd.Org + "\\" + afd.User };
            afd.Authenticated = true;
            afd.User = "test.cacho";
            aeServerLogic.checkSameDomainAndAdmin(afd);
        }

        [Test]
        [ExpectedException(typeof(PermissionException))]
        public void AdminCheckEvenIfCommandIsToYouShouldFail()
        {
            // Yourself when you're NOT admin
            var afd = insertUser();
            afd.Identity = new FakeIdentity() { Auth = true, Username = afd.Org + "\\" + afd.User };
            afd.Authenticated = true;
            aeServerLogic.checkSameDomainAndAdmin(afd);
        }

        [Test]
        [ExpectedException(typeof(PermissionException))]
        public void AdminButCommandToOtherDomainShouldFail()
        {
            // Admin other domain NOT
            var afd = insertAdminUser();
            afd.Identity = new FakeIdentity() { Auth = true, Username = afd.Org + "\\" + afd.User };
            afd.Authenticated = true;
            afd.Org = "test.other";
            aeServerLogic.checkSameDomainAndAdmin(afd);
        }

        [Test]
        public void UserCommandToSelfShouldPass()
        {
            // Yourself when you're NOT admin you should!
            var afd = insertUser();
            afd.Identity = new FakeIdentity() { Auth = true, Username = afd.Org + "\\" + afd.User };
            afd.Authenticated = true;
            aeServerLogic.checkSameDomainSameUserOrAdmin(afd);
        }

        [Test]
        public void AdminCommandToOtherShouldPass()
        {
            // Anyone when you're admin you should!
            var afd = insertAdminUser();
            afd.Identity = new FakeIdentity() { Auth = true, Username = afd.Org + "\\" + afd.User };
            afd.Authenticated = true;
            afd.User = "test.cacho";
            aeServerLogic.checkSameDomainSameUserOrAdmin(afd);
        }

        [Test]
        [ExpectedException(typeof(PermissionException))]
        public void UserCommandToOtherUserShouldFail()
        {
            // Other user when you're NOT admin you should NOT!
            var afd = insertUser();
            afd.Identity = new FakeIdentity(afd);
            afd.Authenticated = true;
            afd.User = "test.cacho";
            aeServerLogic.checkSameDomainSameUserOrAdmin(afd);
        }

        [Test]
        [ExpectedException(typeof(PermissionException))]
        public void UserCommandToUserOnOtherDomainShouldFail()
        {
            // Same user on different domain (it does not exist) NOT 
            var afd = insertUser();
            afd.Identity = new FakeIdentity(afd);
            afd.Authenticated = true;
            afd.Org = "test.cacho";
            aeServerLogic.checkSameDomainSameUserOrAdmin(afd);
        }

        [Test]
        [ExpectedException(typeof(PermissionException))]
        public void UserCommandToUserOnOtherDomainAndExistsShouldFail()
        {
            // Same user on different domain (it does exist) NOT 
            var afdcacho = insertUserCachoDomain();
            var afd = insertUser();
            afd.Identity = new FakeIdentity(afd);
            afd.Authenticated = true;
            afd.Org = "testcacho";
            aeServerLogic.checkSameDomainSameUserOrAdmin(afd);
        }
    }
}
