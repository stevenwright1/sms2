using System;
using AuthGateway.Shared;

using NUnit.Framework;

namespace AuthGateway.AuthEngine.Unittest
{
	[TestFixture]
	public class TestLicense
	{
		[Test]
		//[DeploymentItem(@"Resources\test20120630.lic")]
		public void TestLicenseXmlLoader()
		{
			var asd = new LicenseXmlLoader();
			var lic = asd.LoadFrom(@"Resources\test20120630.lic");
			Assert.AreEqual(2012, lic.AuthEngineEndDate.Year);
			Assert.AreEqual(06, lic.AuthEngineEndDate.Month);
			Assert.AreEqual(30, lic.AuthEngineEndDate.Day);
		}

		[Test]
		//[DeploymentItem(@"Resources\test20120630.lic")]
		public void TestLicenseDocLoader()
		{
			var asd = new LicenseDocLoader();
			var lic = asd.LoadFrom(@"Resources\test20120630.lic");
			Assert.AreEqual(2012, lic.AuthEngineEndDate.Year);
			Assert.AreEqual(06, lic.AuthEngineEndDate.Month);
			Assert.AreEqual(30, lic.AuthEngineEndDate.Day);
		}

		[Test]
		public void TestLicenseXmlDisplayExpireDate()
		{
			var lic = new License();
			Console.WriteLine(lic.AuthEngineEndDate.Year);
			Console.WriteLine(lic.AuthEngineEndDate.Month);
			Console.WriteLine(lic.AuthEngineEndDate.Day);
			Assert.Inconclusive("This test is only to check some license file for expire date.");
		}
	}
}
