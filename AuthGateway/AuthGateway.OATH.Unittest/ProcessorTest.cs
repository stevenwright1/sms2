using System;
using System.Linq;
using System.IO;

using AuthGateway.OATH;
using AuthGateway.OATH.XmlProcessor;

using NUnit.Framework;

namespace AuthGateway.OATH.Unittest
{



	///<summary>
	///This is a test class for ProcessorTest and is intended
	///to contain all ProcessorTest Unit Tests
	///</summary>
	[TestFixture]
	public class ProcessorTest
	{
		///<summary>
		///A test for OutputXML
		///</summary>
		[Test]
		public void OutputXMLTest()
		{
			tokenresponse tr = new tokenresponse();
			tr.token = "1234";
			string actual = null;
			actual = Processor.OutputXML(tr);
			string expectedXml = new System.IO.StreamReader("Resources\\TokenResponse1.xml").ReadToEnd();
			Assert.AreEqual(expectedXml, actual);
		}

		///<summary>
		///A test for ParseXML
		///</summary>
		[Test]
		public void ParseXMLTest()
		{
			StreamReader str = new StreamReader("Resources\\TokenQuery1.xml");
			tokenquery actual = null;
			actual = Processor.ParseXML(str.BaseStream);
			Assert.AreEqual("12345678901234567890", actual.key);
		}

		[Test]
		public void Test_HotpProcessor()
		{
			ITokenProcessor tokenProcessor = new HotpProcessor();
			tokenquery tq = new tokenquery();
			tq.key = "12345678901234567890";
			tokenresponse tr = tokenProcessor.Process(tq);
			Assert.AreEqual(tr.token, "755224");
		}

		[Test]
		public void Test_HotpProcessorAfterValues()
		{
			ITokenProcessor tokenProcessor = new HotpProcessor();
			tokenquery tq = new tokenquery();
			tq.key = "12345678901234567890";
			tq.aftervalues = "5";
			tokenresponse tr = tokenProcessor.Process(tq);
			Assert.AreEqual("755224", tr.token);
			Assert.AreEqual("287082", tr.aftervalues[0].Value);
			Assert.AreEqual("359152", tr.aftervalues[1].Value);
		}

		[Test]
		public void Test_HotpProcessorResync()
		{
			ITokenProcessor tokenProcessor = new HotpProcessor();
			tokenquery tq = new tokenquery();
			tq.key = "12345678901234567890";
			tq.resynctokens = new value[] {
				new value { Value = "359152" },
				new value { Value = "969429" }
			};
			tq.aftervalues = "10000";

			tokenresponse tr = tokenProcessor.Process(tq);
			Assert.AreEqual("359152", tr.token);
			Assert.AreEqual("359152", tr.aftervalues[0].Value);
			Assert.AreEqual("969429", tr.aftervalues[1].Value);
			Assert.AreEqual("3", tr.aftervalues[1].id);
		}

		[Test]
		public void Test_HotpProcessorResyncFail()
		{
			ITokenProcessor tokenProcessor = new HotpProcessor();
			tokenquery tq = new tokenquery();
			tq.key = "12345678901234567890";
			tq.resynctokens = new value[] {
				new value { Value = "xxxxxx" },
				new value { Value = "xxxxxx" }
			};
			tq.counter = "10";
			tq.aftervalues = "10000";

			tokenresponse tr = tokenProcessor.Process(tq);

			Assert.IsNull(tr.aftervalues);
			Assert.IsNull(tr.beforevalues);
		}

		[Test]
		public void Test_TotpProcessor()
		{
			ITokenProcessor tokenProcessor = new TotpProcessor();
			tokenquery tq = new tokenquery();
			tq.key = "12345678901234567890";
			tokenresponse tr = tokenProcessor.Process(tq);
			Assert.AreEqual(HotpTotp.TOTP(tq.key, DateTime.UtcNow), tr.token);
		}

		[Test]
		public void Test_TotpProcessorAfterBeforeValues()
		{
			ITokenProcessor tokenProcessor = new TotpProcessor();
			tokenquery tq = new tokenquery();
			tq.hash = "totp";
			tq.key = "12345678901234567890";
			tq.aftervalues = "2";
			tq.beforevalues = "1";
			tokenresponse tr = tokenProcessor.Process(tq);
			DateTime dt = DateTime.UtcNow;
			Assert.AreEqual(HotpTotp.TOTP(tq.key, dt), tr.token);
			Assert.AreEqual(HotpTotp.TOTP(tq.key, dt.AddSeconds(30)), tr.aftervalues[0].Value);
			Assert.AreEqual(HotpTotp.TOTP(tq.key, dt.AddSeconds(60)), tr.aftervalues[1].Value);
			Assert.AreEqual(HotpTotp.TOTP(tq.key, dt.AddSeconds(-30)), tr.beforevalues[0].Value);
		}
		
		[Test]
		public void Test_TotpWindow()
		{
			var tokenProcessor = new TotpProcessor();
			var tq = new tokenquery {
				hash = "totp",
				key = "12345678901234567890",
				window = "5",
			};
			DateTime dt = DateTime.UtcNow;
			var tr = tokenProcessor.Process(tq);
			Assert.AreEqual(HotpTotp.TOTP(tq.key, dt, 6, 5), tr.token);
			Assert.AreNotEqual(HotpTotp.TOTP(tq.key, dt.AddSeconds(6), 6, 5), tr.token);
		}
	}
}
