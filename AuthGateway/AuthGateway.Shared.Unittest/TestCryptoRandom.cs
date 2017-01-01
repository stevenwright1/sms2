using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace AuthGateway.Shared.Unittest
{
	[TestFixture]
	public class TestCryptoRandom
	{
		[Test]
		public void TestCrypto()
		{
			var counts = new Dictionary<int, int>();
			for (var i = 0; i < 10000; i++)
			{
				var rand = CryptoRandom.Instance().Next(2);
				if (!counts.ContainsKey(rand))
					counts.Add(rand, 0);
				counts[rand]++;
			}
			foreach (var kp in counts)
				Console.WriteLine(kp.Key.ToString() + ": " + kp.Value.ToString());
			Assert.AreEqual(2, counts.Keys.Count);
			Assert.IsTrue(counts.ContainsKey(0));
			Assert.IsTrue(counts.ContainsKey(1));
		}
		
		[Test]
		[TestCase("test")]
		[TestCase("fgkdfg jkfg ")]
		[TestCase(" ")]
		public void AesThenHmac(string input) {
			var encrypted = AESThenHMAC.SimpleEncryptWithPassword(input, "CCSSMS2SWGBccssms2swgb", AESThenHMAC.NewKey());
			var encryptedAgain = AESThenHMAC.SimpleEncryptWithPassword(input, "CCSSMS2SWGBccssms2swgb", AESThenHMAC.NewKey());
			/*
			Console.WriteLine(encrypted);
			Console.WriteLine(encryptedAgain);
			*/
			Assert.AreNotEqual(encrypted, encryptedAgain);
			
			var decrypted = AESThenHMAC.SimpleDecryptWithPassword(encrypted, "CCSSMS2SWGBccssms2swgb", 32);
			var decryptedAgain = AESThenHMAC.SimpleDecryptWithPassword(encryptedAgain, "CCSSMS2SWGBccssms2swgb", 32);
			Assert.AreEqual(decrypted, decryptedAgain);
		}
		
		[Test]
		[TestCase("test")]
		[TestCase("fgkdfg jkfg ")]
		[TestCase(" ")]
		public void RijndaelEnchanced(string input) {
			var iters = (new Random()).Next(10, 2000);
			var salt = RandomKeyGenerator.Generate(16, RKGBase.Base32);
			var rijndaelKey = new RijndaelEnhanced("CCSSMS2SWGBccssms2swgb", "2SWGBccssms2swgb", 8, 64, 256, "SHA1", salt, iters);
			
			var encrypted = rijndaelKey.Encrypt(input);
			var encryptedAgain = rijndaelKey.Encrypt(input);
			var encryptedFull = salt + "$" + iters + "$" + encrypted;
			var encryptedFullAgain = salt + "$" + iters + "$" + encryptedAgain;
			Console.WriteLine(encryptedFull);
			Console.WriteLine(encryptedFullAgain);
			
			Assert.AreNotEqual(encrypted, encryptedAgain);

			var passwordComponents = encryptedFull.Split(new [] { '$' }, 3);
			salt = passwordComponents[0];
			iters = Convert.ToInt32(passwordComponents[1]);			
			rijndaelKey = new RijndaelEnhanced("CCSSMS2SWGBccssms2swgb", "2SWGBccssms2swgb", 8, 16, 256, "SHA1", salt, iters);
			var decrypted = rijndaelKey.Decrypt(passwordComponents[2]);
			
			passwordComponents = encryptedFullAgain.Split(new [] { '$' }, 3);
			salt = passwordComponents[0];
			iters = Convert.ToInt32(passwordComponents[1]);			
			rijndaelKey = new RijndaelEnhanced("CCSSMS2SWGBccssms2swgb", "2SWGBccssms2swgb", 8, 16, 256, "SHA1", salt, iters);
			var decryptedAgain = rijndaelKey.Decrypt(passwordComponents[2]);
			
			Assert.AreEqual(decrypted, decryptedAgain);			
		}
	}
}
