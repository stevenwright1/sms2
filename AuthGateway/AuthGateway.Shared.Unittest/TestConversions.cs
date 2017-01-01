using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace AuthGateway.Shared.Unittest
{
    [TestFixture]
    public class TestConversions
    {
        [Test]
        public void TestBase32EncodingClassDecode()
        {
            string encoded = "JBSWY3DPEHPK3PXP";
            byte[] key = { (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o', (byte)'!',
                             (byte)0xDE, (byte)0xAD, (byte)0xBE, (byte)0xEF };
            byte[] decoded = Base32Encoding.ToBytes(encoded);
            Assert.AreEqual(key.Length,decoded.Length,"Length are different");
            for(int i=0;i<key.Length;i++)
            {
                Assert.AreEqual(key[i],decoded[i],"Byte does not match");
            }
        }

        [Test]
        public void TestBase32EncodingClassEncode()
        {
            byte[] key = { (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o', (byte)'!',
                             (byte)0xDE, (byte)0xAD, (byte)0xBE, (byte)0xEF };
            string encoded = Base32Encoding.ToString(key);
            Assert.AreEqual("JBSWY3DPEHPK3PXP", encoded);
        }

        [Test]
        public void TestByteArrayToHexString()
        {
            string encoded = "JBSWY3DPEHPK3PXP";
            Assert.AreEqual("48656C6C6F21DEADBEEF", HexConversion.ToString(Base32Encoding.ToBytes(encoded)));
        }
    }
}
