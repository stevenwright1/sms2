using AuthGateway.AuthEngine.Logic.Helpers;
using System.Collections.Generic;

using NUnit.Framework;

namespace AuthGateway.AuthEngine.Unittest
{
	[TestFixture]
	public class TestIdentityHelper
	{
		[Test]
		[ExpectedException(typeof(DomainUsernameNotFoundException))]
		public void TestWithoutPatterns()
		{
			var ih = new IdentityHelper(new List<string>());
			ih.GetDomainUsername(@"domain\user");
		}

		[Test]
		[ExpectedException(typeof(DefaultDomainNotSetException))]
		public void TestElseWithoutPatterns()
		{
			var ih = new IdentityHelper(new List<string>());
			ih.GetElseDefaultDomainUsername(string.Empty, @"domain\user");
		}

		[Test]
		public void TestElse()
		{
			var ih = new IdentityHelper(new List<string>());
			var du = ih.GetElseDefaultDomainUsername("Domain", @"User");
			Assert.AreEqual("User", du.Username);
			Assert.AreEqual("Domain", du.Domain);
		}

		[Test]
		public void TestDefaultPatternSlash()
		{
			var ih = new IdentityHelper();
			var du = ih.GetDomainUsername(@"Domain\User");
			Assert.AreEqual("User", du.Username);
			Assert.AreEqual("Domain", du.Domain);
		}

		[Test]
		public void TestDefaultPatternAt()
		{
			var ih = new IdentityHelper();
			var du = ih.GetDomainUsername(@"User@Domain.com");
			Assert.AreEqual("User", du.Username);
			Assert.AreEqual("Domain", du.Domain);
		}

		[Test]
		[ExpectedException(typeof(DomainUsernameNotFoundException))]
		public void TestDefaultPatternNotMatch()
		{
			var ih = new IdentityHelper();
			var du = ih.GetDomainUsername(@"UserDomain.com");
			Assert.AreEqual("User", du.Username);
			Assert.AreEqual("Domain", du.Domain);
		}
	}
}
