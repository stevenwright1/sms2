using NUnit.Framework;

namespace AuthGateway.Shared.Unittest
{
    [TestFixture]
    public class TestRandomKeyGenerator
    {
        [Test]
        public void KeyGenTestB10()
        {
            string key = RandomKeyGenerator.Generate(6, RKGBase.Base10);
            foreach (char k in key.ToCharArray())
                Assert.IsTrue(k >= '0' && k <= '9');
        }

        [Test]
        public void KeyGenTestB32()
        {
            string key = RandomKeyGenerator.Generate(6, RKGBase.Base32);
            foreach (char k in key.ToCharArray())
                Assert.IsTrue((k >= 'A' && k <= 'Z') || (k >= '2' && k <= '7'));
        }
    }
}
