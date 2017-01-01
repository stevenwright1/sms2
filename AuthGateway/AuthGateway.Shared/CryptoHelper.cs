using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AuthGateway.Shared
{
	public static class CryptoHelper
	{
		public static string HashPincode(string pincode) {
			return GetSHA1String(pincode);
			//TODO: Implement something salty!
			var salt = new byte[32];
			CryptoRandom.Instance().NextBytes(salt); 
			return "sha256" + "|$|" + Convert.ToBase64String(salt) + "|$|" + GetSHA256String(salt + pincode);
		}

		public static byte[] GetSHA1(byte[] data) {
			SHA1 sha = new SHA1CryptoServiceProvider();
			return sha.ComputeHash(data);
		}

		public static string GetSHA1String(string dataString) {
			return HexConversion.ToString(
				GetSHA1(Encoding.ASCII.GetBytes(dataString))
			);
		}
		
		public static byte[] GetSHA256(byte[] data) {
			var sha = new SHA256CryptoServiceProvider();
			return sha.ComputeHash(data);
		}
		
		public static string GetSHA256String(string dataString) {
			return Convert.ToBase64String(
				GetSHA256(Encoding.UTF8.GetBytes(dataString))
			);
		}

		public static string DecryptSettingIfNecessary(string p, string moresalt)
		{
			if (string.IsNullOrEmpty(p))
				return p;
			if (!p.StartsWith("encrypted:"))
				return p;
			p = p.Replace("encrypted:", "");
			var bytesToDecrypt = Convert.FromBase64String(p);
			var salt = Encoding.UTF8.GetBytes("rndslt" + moresalt);
			return Encoding.UTF8.GetString(ProtectedData.Unprotect(bytesToDecrypt, salt, DataProtectionScope.LocalMachine));
		}

		public static string EncryptSettingIfNecessary(string p, string moresalt)
		{
			if (string.IsNullOrEmpty(p))
				return p;
			if (p.StartsWith("encrypted:"))
				return p;
			var salt = Encoding.UTF8.GetBytes("rndslt" + moresalt);
			var bytesToEncrypt = Encoding.UTF8.GetBytes(p);
			return "encrypted:" + Convert.ToBase64String(ProtectedData.Protect(bytesToEncrypt, salt, DataProtectionScope.LocalMachine));
		}
	}

	/*
	 * This work (Modern Encryption of a String C#, by James Tuley), 
	 * identified by James Tuley, is free of known copyright restrictions.
	 * https://gist.github.com/4336842
	 * http://creativecommons.org/publicdomain/mark/1.0/ 
	 */
	public static class AESThenHMAC
	{
		private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

		//Preconfigured Encryption Parameters
		public static readonly int BlockBitSize = 128;
		public static readonly int KeyBitSize = 256;

		//Preconfigured Password Key Derivation Parameters
		public static readonly int SaltBitSize = 64;
		public static readonly int Iterations = 10000;
		public static readonly int MinPasswordLength = 12;

		/// <summary>
		/// Helper that generates a random key on each call.
		/// </summary>
		/// <returns></returns>
		public static byte[] NewKey()
		{
			var key = new byte[KeyBitSize / 8];
			Random.GetBytes(key);
			return key;
		}

		/// <summary>
		/// Simple Encryption (AES) then Authentication (HMAC) for a UTF8 Message.
		/// </summary>
		/// <param name="secretMessage">The secret message.</param>
		/// <param name="cryptKey">The crypt key.</param>
		/// <param name="authKey">The auth key.</param>
		/// <param name="nonSecretPayload">(Optional) Non-Secret Payload.</param>
		/// <returns>
		/// Encrypted Message
		/// </returns>
		/// <exception cref="System.ArgumentException">Secret Message Required!;secretMessage</exception>
		/// <remarks>
		/// Adds overhead of (Optional-Payload + BlockSize(16) + Message-Padded-To-Blocksize +  HMac-Tag(32)) * 1.33 Base64
		/// </remarks>
		public static string SimpleEncrypt(string secretMessage, byte[] cryptKey, byte[] authKey,
										   byte[] nonSecretPayload = null)
		{
			if (string.IsNullOrEmpty(secretMessage))
				throw new ArgumentException("Secret Message Required!", "secretMessage");

			var plainText = Encoding.UTF8.GetBytes(secretMessage);
			var cipherText = SimpleEncrypt(plainText, cryptKey, authKey, nonSecretPayload);
			return Convert.ToBase64String(cipherText);
		}

		/// <summary>
		/// Simple Authentication (HMAC) then Decryption (AES) for a secrets UTF8 Message.
		/// </summary>
		/// <param name="encryptedMessage">The encrypted message.</param>
		/// <param name="cryptKey">The crypt key.</param>
		/// <param name="authKey">The auth key.</param>
		/// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
		/// <returns>
		/// Decrypted Message
		/// </returns>
		/// <exception cref="System.ArgumentException">Encrypted Message Required!;encryptedMessage</exception>
		public static string SimpleDecrypt(string encryptedMessage, byte[] cryptKey, byte[] authKey,
										   int nonSecretPayloadLength = 0)
		{
			if (string.IsNullOrWhiteSpace(encryptedMessage))
				throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

			var cipherText = Convert.FromBase64String(encryptedMessage);
			var plainText = SimpleDecrypt(cipherText, cryptKey, authKey, nonSecretPayloadLength);
			return plainText == null ? null : Encoding.UTF8.GetString(plainText);
		}

		/// <summary>
		/// Simple Encryption (AES) then Authentication (HMAC) of a UTF8 message
		/// using Keys derived from a Password (PBKDF2).
		/// </summary>
		/// <param name="secretMessage">The secret message.</param>
		/// <param name="password">The password.</param>
		/// <param name="nonSecretPayload">The non secret payload.</param>
		/// <returns>
		/// Encrypted Message
		/// </returns>
		/// <exception cref="System.ArgumentException">password</exception>
		/// <remarks>
		/// Significantly less secure than using random binary keys.
		/// Adds additional non secret payload for key generation parameters.
		/// </remarks>
		public static string SimpleEncryptWithPassword(string secretMessage, string password,
													   byte[] nonSecretPayload = null)
		{
			if (string.IsNullOrEmpty(secretMessage))
				throw new ArgumentException("Secret Message Required!", "secretMessage");

			var plainText = Encoding.UTF8.GetBytes(secretMessage);
			var cipherText = SimpleEncryptWithPassword(plainText, password, nonSecretPayload);
			return Convert.ToBase64String(cipherText);
		}

		/// <summary>
		/// Simple Authentication (HMAC) and then Descryption (AES) of a UTF8 Message
		/// using keys derived from a password (PBKDF2). 
		/// </summary>
		/// <param name="encryptedMessage">The encrypted message.</param>
		/// <param name="password">The password.</param>
		/// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
		/// <returns>
		/// Decrypted Message
		/// </returns>
		/// <exception cref="System.ArgumentException">Encrypted Message Required!;encryptedMessage</exception>
		/// <remarks>
		/// Significantly less secure than using random binary keys.
		/// </remarks>
		public static string SimpleDecryptWithPassword(string encryptedMessage, string password,
													   int nonSecretPayloadLength = 0)
		{
			if (string.IsNullOrWhiteSpace(encryptedMessage))
				throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

			var cipherText = Convert.FromBase64String(encryptedMessage);
			var plainText = SimpleDecryptWithPassword(cipherText, password, nonSecretPayloadLength);
			return plainText == null ? null : Encoding.UTF8.GetString(plainText);
		}

		/// <summary>
		/// Simple Encryption(AES) then Authentication (HMAC) for a UTF8 Message.
		/// </summary>
		/// <param name="secretMessage">The secret message.</param>
		/// <param name="cryptKey">The crypt key.</param>
		/// <param name="authKey">The auth key.</param>
		/// <param name="nonSecretPayload">(Optional) Non-Secret Payload.</param>
		/// <returns>
		/// Encrypted Message
		/// </returns>
		/// <remarks>
		/// Adds overhead of (Optional-Payload + BlockSize(16) + Message-Padded-To-Blocksize +  HMac-Tag(32)) * 1.33 Base64
		/// </remarks>
		public static byte[] SimpleEncrypt(byte[] secretMessage, byte[] cryptKey, byte[] authKey, byte[] nonSecretPayload = null)
		{
			//User Error Checks
			if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
				throw new ArgumentException(String.Format("Key needs to be {0} bit!", KeyBitSize), "cryptKey");

			if (authKey == null || authKey.Length != KeyBitSize / 8)
				throw new ArgumentException(String.Format("Key needs to be {0} bit!", KeyBitSize), "authKey");

			if (secretMessage == null || secretMessage.Length < 1)
				throw new ArgumentException("Secret Message Required!", "secretMessage");

			//non-secret payload optional
			nonSecretPayload = nonSecretPayload ?? new byte[] { };

			byte[] cipherText;
			byte[] iv;

			using (var aes = new AesManaged
			{
				KeySize = KeyBitSize,
				BlockSize = BlockBitSize,
				Mode = CipherMode.CBC,
				Padding = PaddingMode.PKCS7
			})
			{

				//Use random IV
				aes.GenerateIV();
				iv = aes.IV;

				using (var encrypter = aes.CreateEncryptor(cryptKey, iv))
				using (var cipherStream = new MemoryStream())
				{
					using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
					using (var binaryWriter = new BinaryWriter(cryptoStream))
					{
						//Encrypt Data
						binaryWriter.Write(secretMessage);
					}

					cipherText = cipherStream.ToArray();
				}

			}

			//Assemble encrypted message and add authentication
			using (var hmac = new HMACSHA256(authKey))
			using (var encryptedStream = new MemoryStream())
			{
				using (var binaryWriter = new BinaryWriter(encryptedStream))
				{
					//Prepend non-secret payload if any
					binaryWriter.Write(nonSecretPayload);
					//Prepend IV
					binaryWriter.Write(iv);
					//Write Ciphertext
					binaryWriter.Write(cipherText);
					binaryWriter.Flush();

					//Authenticate all data
					var tag = hmac.ComputeHash(encryptedStream.ToArray());
					//Postpend tag
					binaryWriter.Write(tag);
				}
				return encryptedStream.ToArray();
			}

		}

		/// <summary>
		/// Simple Authentication (HMAC) then Decryption (AES) for a secrets UTF8 Message.
		/// </summary>
		/// <param name="encryptedMessage">The encrypted message.</param>
		/// <param name="cryptKey">The crypt key.</param>
		/// <param name="authKey">The auth key.</param>
		/// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
		/// <returns>Decrypted Message</returns>
		public static byte[] SimpleDecrypt(byte[] encryptedMessage, byte[] cryptKey, byte[] authKey, int nonSecretPayloadLength = 0)
		{

			//Basic Usage Error Checks
			if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
				throw new ArgumentException(String.Format("CryptKey needs to be {0} bit!", KeyBitSize), "cryptKey");

			if (authKey == null || authKey.Length != KeyBitSize / 8)
				throw new ArgumentException(String.Format("AuthKey needs to be {0} bit!", KeyBitSize), "authKey");

			if (encryptedMessage == null || encryptedMessage.Length == 0)
				throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

			using (var hmac = new HMACSHA256(authKey))
			{
				var sentTag = new byte[hmac.HashSize / 8];
				//Calculate Tag
				var calcTag = hmac.ComputeHash(encryptedMessage, 0, encryptedMessage.Length - sentTag.Length);
				var ivLength = (BlockBitSize / 8);

				//if message length is to small just return null
				if (encryptedMessage.Length < sentTag.Length + nonSecretPayloadLength + ivLength)
					return null;

				//Grab Sent Tag
				Array.Copy(encryptedMessage, encryptedMessage.Length - sentTag.Length, sentTag, 0, sentTag.Length);

				//Compare Tag with constant time comparison
				var compare = 0;
				for (var i = 0; i < sentTag.Length; i++)
					compare |= sentTag[i] ^ calcTag[i];

				//if message doesn't authenticate return null
				if (compare != 0)
					return null;

				using (var aes = new AesManaged
				{
					KeySize = KeyBitSize,
					BlockSize = BlockBitSize,
					Mode = CipherMode.CBC,
					Padding = PaddingMode.PKCS7
				})
				{

					//Grab IV from message
					var iv = new byte[ivLength];
					Array.Copy(encryptedMessage, nonSecretPayloadLength, iv, 0, iv.Length);

					using (var decrypter = aes.CreateDecryptor(cryptKey, iv))
					using (var plainTextStream = new MemoryStream())
					{
						using (var decrypterStream = new CryptoStream(plainTextStream, decrypter, CryptoStreamMode.Write))
						using (var binaryWriter = new BinaryWriter(decrypterStream))
						{
							//Decrypt Cipher Text from Message
							binaryWriter.Write(
								encryptedMessage,
								nonSecretPayloadLength + iv.Length,
								encryptedMessage.Length - nonSecretPayloadLength - iv.Length - sentTag.Length
							);
						}
						//Return Plain Text
						return plainTextStream.ToArray();
					}
				}
			}
		}

		/// <summary>
		/// Simple Encryption (AES) then Authentication (HMAC) of a UTF8 message
		/// using Keys derived from a Password (PBKDF2)
		/// </summary>
		/// <param name="secretMessage">The secret message.</param>
		/// <param name="password">The password.</param>
		/// <param name="nonSecretPayload">The non secret payload.</param>
		/// <returns>
		/// Encrypted Message
		/// </returns>
		/// <exception cref="System.ArgumentException">Must have a password of minimum length;password</exception>
		/// <remarks>
		/// Significantly less secure than using random binary keys.
		/// Adds additional non secret payload for key generation parameters.
		/// </remarks>
		public static byte[] SimpleEncryptWithPassword(byte[] secretMessage, string password, byte[] nonSecretPayload = null)
		{
			nonSecretPayload = nonSecretPayload ?? new byte[] { };

			//User Error Checks
			if (string.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
				throw new ArgumentException(String.Format("Must have a password of at least {0} characters!", MinPasswordLength), "password");

			if (secretMessage == null || secretMessage.Length == 0)
				throw new ArgumentException("Secret Message Required!", "secretMessage");

			var payload = new byte[((SaltBitSize / 8) * 2) + nonSecretPayload.Length];

			Array.Copy(nonSecretPayload, payload, nonSecretPayload.Length);
			int payloadIndex = nonSecretPayload.Length;

			byte[] cryptKey;
			byte[] authKey;
			//Use Random Salt to prevent pre-generated weak password attacks.
			using (var generator = new Rfc2898DeriveBytes(password, SaltBitSize / 8, Iterations))
			{
				var salt = generator.Salt;

				//Generate Keys
				cryptKey = generator.GetBytes(KeyBitSize / 8);

				//Create Non Secret Payload
				Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
				payloadIndex += salt.Length;
			}

			//Deriving separate key, might be less efficient than using HKDF, 
			//but now compatible with RNEncryptor which had a very similar wireformat and requires less code than HKDF.
			using (var generator = new Rfc2898DeriveBytes(password, SaltBitSize / 8, Iterations))
			{
				var salt = generator.Salt;

				//Generate Keys
				authKey = generator.GetBytes(KeyBitSize / 8);

				//Create Rest of Non Secret Payload
				Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
			}

			return SimpleEncrypt(secretMessage, cryptKey, authKey, payload);
		}

		/// <summary>
		/// Simple Authentication (HMAC) and then Descryption (AES) of a UTF8 Message
		/// using keys derived from a password (PBKDF2). 
		/// </summary>
		/// <param name="encryptedMessage">The encrypted message.</param>
		/// <param name="password">The password.</param>
		/// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
		/// <returns>
		/// Decrypted Message
		/// </returns>
		/// <exception cref="System.ArgumentException">Must have a password of minimum length;password</exception>
		/// <remarks>
		/// Significantly less secure than using random binary keys.
		/// </remarks>
		public static byte[] SimpleDecryptWithPassword(byte[] encryptedMessage, string password, int nonSecretPayloadLength = 0)
		{
			//User Error Checks
			if (string.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
				throw new ArgumentException(String.Format("Must have a password of at least {0} characters!", MinPasswordLength), "password");

			if (encryptedMessage == null || encryptedMessage.Length == 0)
				throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

			var cryptSalt = new byte[SaltBitSize / 8];
			var authSalt = new byte[SaltBitSize / 8];

			//Grab Salt from Non-Secret Payload
			Array.Copy(encryptedMessage, nonSecretPayloadLength, cryptSalt, 0, cryptSalt.Length);
			Array.Copy(encryptedMessage, nonSecretPayloadLength + cryptSalt.Length, authSalt, 0, authSalt.Length);

			byte[] cryptKey;
			byte[] authKey;

			//Generate crypt key
			using (var generator = new Rfc2898DeriveBytes(password, cryptSalt, Iterations))
			{
				cryptKey = generator.GetBytes(KeyBitSize / 8);
			}
			//Generate auth key
			using (var generator = new Rfc2898DeriveBytes(password, authSalt, Iterations))
			{
				authKey = generator.GetBytes(KeyBitSize / 8);
			}

			return SimpleDecrypt(encryptedMessage, cryptKey, authKey, cryptSalt.Length + authSalt.Length + nonSecretPayloadLength);
		}

	}
	
	// http://www.obviex.com/samples/EncryptionWithSalt.aspx
	// You may use any original code samples available on this Site under the 
	// terms of either the MIT or the GPL v.3 license. Obviex is not responsible, 
	// accountable, or liable for any damages, problems, or difficulties resulting 
	// from use of the code samples available or referenced on this Site.
	
	///////////////////////////////////////////////////////////////////////////////
	// SAMPLE: Illustrates symmetric key encryption and decryption using Rijndael
	//         algorithm. In addition to performing encryption, this sample 
	//         explains how to add a salt value (randomly generated bytes) to 
	//         plain text before the value is encrypted. This can help reduce the
	//         risk of dictionary attacks.
	//
	// To run this sample, create a new Visual C# project using the Console 
	// Application template and replace the contents of the Module1.vb file with
	// the code below.
	//
	// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
	// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
	// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
	//
	// Copyright (C) 2003 Obviex(TM). All rights reserved.
	//
	 
	/// <summary>
	/// This class uses a symmetric key algorithm (Rijndael/AES) to encrypt and
	/// decrypt data. As long as it is initialized with the same constructor
	/// parameters, the class will use the same key. Before performing encryption,
	/// the class can prepend random bytes to plain text and generate different
	/// encrypted values from the same plain text, encryption key, initialization
	/// vector, and other parameters. This class is thread-safe.
	/// </summary>
	/// <remarks>
	/// Be careful when performing encryption and decryption. There is a bug
	/// ("feature"?) in .NET Framework, which causes corruption of encryptor/
	/// decryptor if a cryptographic exception occurs during encryption/
	/// decryption operation. To correct the problem, re-initialize the class
	/// instance when a cryptographic exception occurs.
	/// </remarks>
	public class RijndaelEnhanced
	{
	    #region Private members
	    // If hashing algorithm is not specified, use SHA-1.
	    private static string  DEFAULT_HASH_ALGORITHM  = "SHA1";
	 
	    // If key size is not specified, use the longest 256-bit key.
	    private static int     DEFAULT_KEY_SIZE        = 256;
	 
	    // Do not allow salt to be longer than 255 bytes, because we have only
	    // 1 byte to store its length. 
	    private static int     MAX_ALLOWED_SALT_LEN    = 255;
	 
	    // Do not allow salt to be smaller than 4 bytes, because we use the first
	    // 4 bytes of salt to store its length. 
	    private static int     MIN_ALLOWED_SALT_LEN    = 4;
	 
	    // Random salt value will be between 4 and 8 bytes long.
	    private static int     DEFAULT_MIN_SALT_LEN    = MIN_ALLOWED_SALT_LEN;
	    private static int     DEFAULT_MAX_SALT_LEN    = 8;
	 
	    // Use these members to save min and max salt lengths.
	    private int     minSaltLen              = -1;
	    private int     maxSaltLen              = -1;
	 
	    // These members will be used to perform encryption and decryption.
	    private ICryptoTransform encryptor      = null;
	    private ICryptoTransform decryptor      = null;
	    #endregion
	 
	    #region Constructors
	    /// <summary>
	    /// Use this constructor if you are planning to perform encryption/
	    /// decryption with 256-bit key, derived using 1 password iteration,
	    /// hashing without salt, no initialization vector, electronic codebook
	    /// (ECB) mode, SHA-1 hashing algorithm, and 4-to-8 byte long salt.
	    /// </summary>
	    /// <param name="passPhrase">
	    /// Passphrase from which a pseudo-random password will be derived.
	    /// The derived password will be used to generate the encryption key.
	    /// Passphrase can be any string. In this example we assume that the
	    /// passphrase is an ASCII string. Passphrase value must be kept in
	    /// secret.
	    /// </param>
	    /// <remarks>
	    /// This constructor is not recommended because it does not use
	    /// initialization vector and uses the ECB cipher mode, which is less
	    /// secure than the CBC mode.
	    /// </remarks>
	    public RijndaelEnhanced(string  passPhrase) : 
	        this(passPhrase, null)
	    {
	    }
	 
	    /// <summary>
	    /// Use this constructor if you are planning to perform encryption/
	    /// decryption with 256-bit key, derived using 1 password iteration,
	    /// hashing without salt, cipher block chaining (CBC) mode, SHA-1
	    /// hashing algorithm, and 4-to-8 byte long salt.
	    /// </summary>
	    /// <param name="passPhrase">
	    /// Passphrase from which a pseudo-random password will be derived.
	    /// The derived password will be used to generate the encryption key.
	    /// Passphrase can be any string. In this example we assume that the
	    /// passphrase is an ASCII string. Passphrase value must be kept in
	    /// secret.
	    /// </param>
	    /// <param name="initVector">
	    /// Initialization vector (IV). This value is required to encrypt the
	    /// first block of plaintext data. For RijndaelManaged class IV must be
	    /// exactly 16 ASCII characters long. IV value does not have to be kept
	    /// in secret.
	    /// </param>
	    public RijndaelEnhanced(string  passPhrase,
	                            string  initVector) :
	        this(passPhrase, initVector, -1)
	    {
	    }
	 
	    /// <summary>
	    /// Use this constructor if you are planning to perform encryption/
	    /// decryption with 256-bit key, derived using 1 password iteration,
	    /// hashing without salt, cipher block chaining (CBC) mode, SHA-1 
	    /// hashing algorithm, and 0-to-8 byte long salt.
	    /// </summary>
	    /// <param name="passPhrase">
	    /// Passphrase from which a pseudo-random password will be derived.
	    /// The derived password will be used to generate the encryption key
	    /// Passphrase can be any string. In this example we assume that the
	    /// passphrase is an ASCII string. Passphrase value must be kept in
	    /// secret.
	    /// </param>
	    /// <param name="initVector">
	    /// Initialization vector (IV). This value is required to encrypt the
	    /// first block of plaintext data. For RijndaelManaged class IV must be
	    /// exactly 16 ASCII characters long. IV value does not have to be kept
	    /// in secret.
	    /// </param>
	    /// <param name="minSaltLen">
	    /// Min size (in bytes) of randomly generated salt which will be added at
	    /// the beginning of plain text before encryption is performed. When this
	    /// value is less than 4, the default min value will be used (currently 4
	    /// bytes).
	    /// </param>
	    public RijndaelEnhanced(string  passPhrase,
	                            string  initVector,
	                            int     minSaltLen) :
	        this(passPhrase, initVector, minSaltLen, -1)
	    {
	    }
	 
	    /// <summary>
	    /// Use this constructor if you are planning to perform encryption/
	    /// decryption with 256-bit key, derived using 1 password iteration,
	    /// hashing without salt, cipher block chaining (CBC) mode, SHA-1
	    /// hashing algorithm. Use the minSaltLen and maxSaltLen parameters to
	    /// specify the size of randomly generated salt.
	    /// </summary>
	    /// <param name="passPhrase">
	    /// Passphrase from which a pseudo-random password will be derived.
	    /// The derived password will be used to generate the encryption key.
	    /// Passphrase can be any string. In this example we assume that the
	    /// passphrase is an ASCII string. Passphrase value must be kept in
	    /// secret.
	    /// </param>
	    /// <param name="initVector">
	    /// Initialization vector (IV). This value is required to encrypt the
	    /// first block of plaintext data. For RijndaelManaged class IV must be
	    /// exactly 16 ASCII characters long. IV value does not have to be kept
	    /// in secret.
	    /// </param>
	    /// <param name="minSaltLen">
	    /// Min size (in bytes) of randomly generated salt which will be added at
	    /// the beginning of plain text before encryption is performed. When this
	    /// value is less than 4, the default min value will be used (currently 4
	    /// bytes).
	    /// </param>
	    /// <param name="maxSaltLen">
	    /// Max size (in bytes) of randomly generated salt which will be added at
	    /// the beginning of plain text before encryption is performed. When this
	    /// value is negative or greater than 255, the default max value will be
	    /// used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
	    /// than the specified min value (which can be adjusted to default value),
	    /// salt will not be used and plain text value will be encrypted as is.
	    /// In this case, salt will not be processed during decryption either.
	    /// </param>
	    public RijndaelEnhanced(string  passPhrase,
	                            string  initVector,
	                            int     minSaltLen,
	                            int     maxSaltLen) :
	        this(passPhrase, initVector, minSaltLen, maxSaltLen, -1)
	    {
	    }
	 
	    /// <summary>
	    /// Use this constructor if you are planning to perform encryption/
	    /// decryption using the key derived from 1 password iteration,
	    /// hashing without salt, cipher block chaining (CBC) mode, and
	    /// SHA-1 hashing algorithm.
	    /// </summary>
	    /// <param name="passPhrase">
	    /// Passphrase from which a pseudo-random password will be derived.
	    /// The derived password will be used to generate the encryption key.
	    /// Passphrase can be any string. In this example we assume that the
	    /// passphrase is an ASCII string. Passphrase value must be kept in
	    /// secret.
	    /// </param>
	    /// <param name="initVector">
	    /// Initialization vector (IV). This value is required to encrypt the
	    /// first block of plaintext data. For RijndaelManaged class IV must be
	    /// exactly 16 ASCII characters long. IV value does not have to be kept
	    /// in secret.
	    /// </param>
	    /// <param name="minSaltLen">
	    /// Min size (in bytes) of randomly generated salt which will be added at
	    /// the beginning of plain text before encryption is performed. When this
	    /// value is less than 4, the default min value will be used (currently 4
	    /// bytes).
	    /// </param>
	    /// <param name="maxSaltLen">
	    /// Max size (in bytes) of randomly generated salt which will be added at
	    /// the beginning of plain text before encryption is performed. When this
	    /// value is negative or greater than 255, the default max value will be 
	    /// used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
	    /// than the specified min value (which can be adjusted to default value),
	    /// salt will not be used and plain text value will be encrypted as is.
	    /// In this case, salt will not be processed during decryption either.
	    /// </param>
	    /// <param name="keySize">
	    /// Size of symmetric key (in bits): 128, 192, or 256.
	    /// </param>
	    public RijndaelEnhanced(string  passPhrase,
	                            string  initVector,
	                            int     minSaltLen,
	                            int     maxSaltLen,
	                            int     keySize) :
	        this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize, null)
	    {
	    }
	 
	    /// <summary>
	    /// Use this constructor if you are planning to perform encryption/
	    /// decryption using the key derived from 1 password iteration, hashing 
	    /// without salt, and cipher block chaining (CBC) mode.
	    /// </summary>
	    /// <param name="passPhrase">
	    /// Passphrase from which a pseudo-random password will be derived.
	    /// The derived password will be used to generate the encryption key.
	    /// Passphrase can be any string. In this example we assume that the
	    /// passphrase is an ASCII string. Passphrase value must be kept in
	    /// secret.
	    /// </param>
	    /// <param name="initVector">
	    /// Initialization vector (IV). This value is required to encrypt the
	    /// first block of plaintext data. For RijndaelManaged class IV must be
	    /// exactly 16 ASCII characters long. IV value does not have to be kept
	    /// in secret.
	    /// </param>
	    /// <param name="minSaltLen">
	    /// Min size (in bytes) of randomly generated salt which will be added at
	    /// the beginning of plain text before encryption is performed. When this
	    /// value is less than 4, the default min value will be used (currently 4
	    /// bytes).
	    /// </param>
	    /// <param name="maxSaltLen">
	    /// Max size (in bytes) of randomly generated salt which will be added at
	    /// the beginning of plain text before encryption is performed. When this
	    /// value is negative or greater than 255, the default max value will be
	    /// used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
	    /// than the specified min value (which can be adjusted to default value),
	    /// salt will not be used and plain text value will be encrypted as is.
	    /// In this case, salt will not be processed during decryption either.
	    /// </param>
	    /// <param name="keySize">
	    /// Size of symmetric key (in bits): 128, 192, or 256.
	    /// </param>
	    /// <param name="hashAlgorithm">
	    /// Hashing algorithm: "MD5" or "SHA1". SHA1 is recommended.
	    /// </param>
	    public RijndaelEnhanced(string  passPhrase,
	                            string  initVector,
	                            int     minSaltLen,
	                            int     maxSaltLen, 
	                            int     keySize,
	                            string  hashAlgorithm) : 
	        this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize, 
	             hashAlgorithm, null)
	    {
	    }
	 
	    /// <summary>
	    /// Use this constructor if you are planning to perform encryption/
	    /// decryption using the key derived from 1 password iteration, and
	    /// cipher block chaining (CBC) mode.
	    /// </summary>
	    /// <param name="passPhrase">
	    /// Passphrase from which a pseudo-random password will be derived.
	    /// The derived password will be used to generate the encryption key.
	    /// Passphrase can be any string. In this example we assume that the
	    /// passphrase is an ASCII string. Passphrase value must be kept in
	    /// secret.
	    /// </param>
	    /// <param name="initVector">
	    /// Initialization vector (IV). This value is required to encrypt the
	    /// first block of plaintext data. For RijndaelManaged class IV must be
	    /// exactly 16 ASCII characters long. IV value does not have to be kept
	    /// in secret.
	    /// </param>
	    /// <param name="minSaltLen">
	    /// Min size (in bytes) of randomly generated salt which will be added at
	    /// the beginning of plain text before encryption is performed. When this
	    /// value is less than 4, the default min value will be used (currently 4
	    /// bytes).
	    /// </param>
	    /// <param name="maxSaltLen">
	    /// Max size (in bytes) of randomly generated salt which will be added at
	    /// the beginning of plain text before encryption is performed. When this
	    /// value is negative or greater than 255, the default max value will be
	    /// used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
	    /// than the specified min value (which can be adjusted to default value),
	    /// salt will not be used and plain text value will be encrypted as is.
	    /// In this case, salt will not be processed during decryption either.
	    /// </param>
	    /// <param name="keySize">
	    /// Size of symmetric key (in bits): 128, 192, or 256.
	    /// </param>
	    /// <param name="hashAlgorithm">
	    /// Hashing algorithm: "MD5" or "SHA1". SHA1 is recommended.
	    /// </param>
	    /// <param name="saltValue">
	    /// Salt value used for password hashing during key generation. This is
	    /// not the same as the salt we will use during encryption. This parameter
	    /// can be any string.
	    /// </param>
	    public RijndaelEnhanced(string  passPhrase,
	                            string  initVector,
	                            int     minSaltLen,
	                            int     maxSaltLen, 
	                            int     keySize,
	                            string  hashAlgorithm,
	                            string  saltValue) : 
	        this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize, 
	             hashAlgorithm, saltValue, 1)
	    {
	    }
	 
	    /// <summary>
	    /// Use this constructor if you are planning to perform encryption/
	    /// decryption with the key derived from the explicitly specified
	    /// parameters.
	    /// </summary>
	    /// <param name="passPhrase">
	    /// Passphrase from which a pseudo-random password will be derived.
	    /// The derived password will be used to generate the encryption key
	    /// Passphrase can be any string. In this example we assume that the
	    /// passphrase is an ASCII string. Passphrase value must be kept in
	    /// secret.
	    /// </param>
	    /// <param name="initVector">
	    /// Initialization vector (IV). This value is required to encrypt the
	    /// first block of plaintext data. For RijndaelManaged class IV must be
	    /// exactly 16 ASCII characters long. IV value does not have to be kept
	    /// in secret.
	    /// </param>
	    /// <param name="minSaltLen">
	    /// Min size (in bytes) of randomly generated salt which will be added at
	    /// the beginning of plain text before encryption is performed. When this
	    /// value is less than 4, the default min value will be used (currently 4
	    /// bytes).
	    /// </param>
	    /// <param name="maxSaltLen">
	    /// Max size (in bytes) of randomly generated salt which will be added at
	    /// the beginning of plain text before encryption is performed. When this
	    /// value is negative or greater than 255, the default max value will be
	    /// used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
	    /// than the specified min value (which can be adjusted to default value),
	    /// salt will not be used and plain text value will be encrypted as is.
	    /// In this case, salt will not be processed during decryption either.
	    /// </param>
	    /// <param name="keySize">
	    /// Size of symmetric key (in bits): 128, 192, or 256.
	    /// </param>
	    /// <param name="hashAlgorithm">
	    /// Hashing algorithm: "MD5" or "SHA1". SHA1 is recommended.
	    /// </param>
	    /// <param name="saltValue">
	    /// Salt value used for password hashing during key generation. This is
	    /// not the same as the salt we will use during encryption. This parameter
	    /// can be any string.
	    /// </param>
	    /// <param name="passwordIterations">
	    /// Number of iterations used to hash password. More iterations are
	    /// considered more secure but may take longer.
	    /// </param>
	    public RijndaelEnhanced(string  passPhrase,
	                            string  initVector,
	                            int     minSaltLen,
	                            int     maxSaltLen, 
	                            int     keySize,
	                            string  hashAlgorithm,
	                            string  saltValue,
	                            int     passwordIterations)
	    {
	        // Save min salt length; set it to default if invalid value is passed.
	        if (minSaltLen < MIN_ALLOWED_SALT_LEN)
	            this.minSaltLen = DEFAULT_MIN_SALT_LEN;
	        else
	            this.minSaltLen = minSaltLen;
	 
	        // Save max salt length; set it to default if invalid value is passed.
	        if (maxSaltLen < 0 || maxSaltLen > MAX_ALLOWED_SALT_LEN)
	            this.maxSaltLen = DEFAULT_MAX_SALT_LEN;
	        else
	            this.maxSaltLen = maxSaltLen;
	 
	        // Set the size of cryptographic key.
	        if (keySize <= 0)
	            keySize = DEFAULT_KEY_SIZE;
	 
	        // Set the name of algorithm. Make sure it is in UPPER CASE and does
	        // not use dashes, e.g. change "sha-1" to "SHA1".
	        if (hashAlgorithm == null)
	            hashAlgorithm = DEFAULT_HASH_ALGORITHM;
	        else
	            hashAlgorithm = hashAlgorithm.ToUpper().Replace("-", "");
	 
	        // Initialization vector converted to a byte array.
	        byte[] initVectorBytes = null;
	 
	        // Salt used for password hashing (to generate the key, not during
	        // encryption) converted to a byte array.
	        byte[] saltValueBytes  = null;
	 
	        // Get bytes of initialization vector.
	        if (initVector == null)
	            initVectorBytes = new byte[0];
	        else
	            initVectorBytes = Encoding.ASCII.GetBytes(initVector);
	 
	        // Get bytes of salt (used in hashing).
	        if (saltValue == null)
	            saltValueBytes = new byte[0];
	        else
	            saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
	 
	        // Generate password, which will be used to derive the key.
			byte[] keyBytes;
	        using (var password = new Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations)) {
				keyBytes = password.GetBytes(keySize / 8);
			}
	 
	        // Convert key to a byte array adjusting the size from bits to bytes.
	 
	        // Initialize Rijndael key object.
	        var symmetricKey = new RijndaelManaged();
	 
	        // If we do not have initialization vector, we cannot use the CBC mode.
	        // The only alternative is the ECB mode (which is not as good).
	        if (initVectorBytes.Length == 0)
	            symmetricKey.Mode = CipherMode.ECB;
	        else
	            symmetricKey.Mode = CipherMode.CBC;
	 
	        // Create encryptor and decryptor, which we will use for cryptographic
	        // operations.
	        encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
	        decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
	    }
	    #endregion
	 
	    #region Encryption routines
	    /// <summary>
	    /// Encrypts a string value generating a base64-encoded string.
	    /// </summary>
	    /// <param name="plainText">
	    /// Plain text string to be encrypted.
	    /// </param>
	    /// <returns>
	    /// Cipher text formatted as a base64-encoded string.
	    /// </returns>
	    public string Encrypt(string plainText)
	    {
	        return Encrypt(Encoding.UTF8.GetBytes(plainText));
	    }
	 
	    /// <summary>
	    /// Encrypts a byte array generating a base64-encoded string.
	    /// </summary>
	    /// <param name="plainTextBytes">
	    /// Plain text bytes to be encrypted.
	    /// </param>
	    /// <returns>
	    /// Cipher text formatted as a base64-encoded string.
	    /// </returns>
	    public string Encrypt(byte[] plainTextBytes)
	    {
	        return Convert.ToBase64String(EncryptToBytes(plainTextBytes));
	    }
	 
	    /// <summary>
	    /// Encrypts a string value generating a byte array of cipher text.
	    /// </summary>
	    /// <param name="plainText">
	    /// Plain text string to be encrypted.
	    /// </param>
	    /// <returns>
	    /// Cipher text formatted as a byte array.
	    /// </returns>
	    public byte[] EncryptToBytes(string plainText)
	    {
	        return EncryptToBytes(Encoding.UTF8.GetBytes(plainText));
	    }
	 
	    /// <summary>
	    /// Encrypts a byte array generating a byte array of cipher text.
	    /// </summary>
	    /// <param name="plainTextBytes">
	    /// Plain text bytes to be encrypted.
	    /// </param>
	    /// <returns>
	    /// Cipher text formatted as a byte array.
	    /// </returns>
	    public byte[] EncryptToBytes(byte[] plainTextBytes)
	    {
	        // Add salt at the beginning of the plain text bytes (if needed).
	        byte[] plainTextBytesWithSalt = AddSalt(plainTextBytes);
	 
	        // Encryption will be performed using memory stream.
	        MemoryStream memoryStream = new MemoryStream();
	        
	        // Let's make cryptographic operations thread-safe.
	        lock (this)
	        {
	            // To perform encryption, we must use the Write mode.
	            CryptoStream cryptoStream = new CryptoStream(
	                                               memoryStream, 
	                                               encryptor,
	                                                CryptoStreamMode.Write);
	 
	            // Start encrypting data.
	            cryptoStream.Write( plainTextBytesWithSalt, 
	                                0, 
	                               plainTextBytesWithSalt.Length);
	             
	            // Finish the encryption operation.
	            cryptoStream.FlushFinalBlock();
	 
	            // Move encrypted data from memory into a byte array.
	            byte[] cipherTextBytes = memoryStream.ToArray();
	               
	            // Close memory streams.
	            memoryStream.Close();
	            cryptoStream.Close();
	 
	            // Return encrypted data.
	            return cipherTextBytes;
	        }
	    }
	    #endregion
	 
	    #region Decryption routines
	    /// <summary>
	    /// Decrypts a base64-encoded cipher text value generating a string result.
	    /// </summary>
	    /// <param name="cipherText">
	    /// Base64-encoded cipher text string to be decrypted.
	    /// </param>
	    /// <returns>
	    /// Decrypted string value.
	    /// </returns>
	    public string Decrypt(string cipherText)
	    {
	        return Decrypt(Convert.FromBase64String(cipherText));
	    }
	 
	    /// <summary>
	    /// Decrypts a byte array containing cipher text value and generates a
	    /// string result.
	    /// </summary>
	    /// <param name="cipherTextBytes">
	    /// Byte array containing encrypted data.
	    /// </param>
	    /// <returns>
	    /// Decrypted string value.
	    /// </returns>
	    public string Decrypt(byte[] cipherTextBytes)
	    {
	        return Encoding.UTF8.GetString(DecryptToBytes(cipherTextBytes));
	    }
	 
	    /// <summary>
	    /// Decrypts a base64-encoded cipher text value and generates a byte array
	    /// of plain text data.
	    /// </summary>
	    /// <param name="cipherText">
	    /// Base64-encoded cipher text string to be decrypted.
	    /// </param>
	    /// <returns>
	    /// Byte array containing decrypted value.
	    /// </returns>
	    public byte[] DecryptToBytes(string cipherText)
	    {
	        return DecryptToBytes(Convert.FromBase64String(cipherText));
	    }
	 
	    /// <summary>
	    /// Decrypts a base64-encoded cipher text value and generates a byte array
	    /// of plain text data.
	    /// </summary>
	    /// <param name="cipherTextBytes">
	    /// Byte array containing encrypted data.
	    /// </param>
	    /// <returns>
	    /// Byte array containing decrypted value.
	    /// </returns>
	    public byte[] DecryptToBytes(byte[] cipherTextBytes)
	    {
	        byte[] decryptedBytes      = null;
	        byte[] plainTextBytes      = null;
	        int    decryptedByteCount  = 0;
	        int    saltLen             = 0;
	 
	        MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
	 
	        // Since we do not know how big decrypted value will be, use the same
	        // size as cipher text. Cipher text is always longer than plain text
	        // (in block cipher encryption), so we will just use the number of
	        // decrypted data byte after we know how big it is.
	        decryptedBytes = new byte[cipherTextBytes.Length];
	 
	        // Let's make cryptographic operations thread-safe.
	        lock (this)
	        {
	            // To perform decryption, we must use the Read mode.
	            CryptoStream  cryptoStream = new CryptoStream(
	                                               memoryStream,
	                                               decryptor,
	                                               CryptoStreamMode.Read);
	 
	            // Decrypting data and get the count of plain text bytes.
	            decryptedByteCount  = cryptoStream.Read(decryptedBytes,
	                                                    0, 
	                                                    decryptedBytes.Length);
	            // Release memory.
	            memoryStream.Close();
	            cryptoStream.Close();
	        }
	 
	        // If we are using salt, get its length from the first 4 bytes of plain
	        // text data.
	        if (maxSaltLen > 0 && maxSaltLen >= minSaltLen)
	        {
	            saltLen =   (decryptedBytes[0] & 0x03) |
	                        (decryptedBytes[1] & 0x0c) |
	                        (decryptedBytes[2] & 0x30) |
	                        (decryptedBytes[3] & 0xc0);
	        }
	 
	        // Allocate the byte array to hold the original plain text (without salt).
	        plainTextBytes = new byte[decryptedByteCount - saltLen];
	 
	        // Copy original plain text discarding the salt value if needed.
	        Array.Copy(decryptedBytes, saltLen, plainTextBytes, 
	                    0, decryptedByteCount - saltLen);
	 
	        // Return original plain text value.
	        return plainTextBytes;
	    }
	    #endregion
	 
	    #region Helper functions
	    /// <summary>
	    /// Adds an array of randomly generated bytes at the beginning of the
	    /// array holding original plain text value.
	    /// </summary>
	    /// <param name="plainTextBytes">
	    /// Byte array containing original plain text value.
	    /// </param>
	    /// <returns>
	    /// Either original array of plain text bytes (if salt is not used) or a
	    /// modified array containing a randomly generated salt added at the 
	    /// beginning of the plain text bytes. 
	    /// </returns>
	    private byte[] AddSalt(byte[] plainTextBytes)
	    {
	        // The max salt value of 0 (zero) indicates that we should not use 
	        // salt. Also do not use salt if the max salt value is smaller than
	        // the min value.
	        if (maxSaltLen == 0 || maxSaltLen < minSaltLen)
	            return plainTextBytes;
	 
	        // Generate the salt.
	        byte[] saltBytes = GenerateSalt();
	 
	        // Allocate array which will hold salt and plain text bytes.
	        byte[] plainTextBytesWithSalt = new byte[plainTextBytes.Length +
	                                                 saltBytes.Length];
	        // First, copy salt bytes.
	        Array.Copy(saltBytes, plainTextBytesWithSalt, saltBytes.Length);
	 
	        // Append plain text bytes to the salt value.
	        Array.Copy( plainTextBytes, 0, 
	                    plainTextBytesWithSalt, saltBytes.Length,
	                    plainTextBytes.Length);
	 
	        return plainTextBytesWithSalt;
	    }
	 
	    /// <summary>
	    /// Generates an array holding cryptographically strong bytes.
	    /// </summary>
	    /// <returns>
	    /// Array of randomly generated bytes.
	    /// </returns>
	    /// <remarks>
	    /// Salt size will be defined at random or exactly as specified by the
	    /// minSlatLen and maxSaltLen parameters passed to the object constructor.
	    /// The first four bytes of the salt array will contain the salt length
	    /// split into four two-bit pieces.
	    /// </remarks>
	    private byte[] GenerateSalt()
	    {
	        // We don't have the length, yet.
	        int saltLen = 0;
	 
	        // If min and max salt values are the same, it should not be random.
	        if (minSaltLen == maxSaltLen)
	            saltLen = minSaltLen;
	        // Use random number generator to calculate salt length.
	        else
	            saltLen = GenerateRandomNumber(minSaltLen, maxSaltLen);
	 
	        // Allocate byte array to hold our salt.
	        byte[] salt = new byte[saltLen];
	 
	        // Populate salt with cryptographically strong bytes.
	        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
	 
	        rng.GetNonZeroBytes(salt);
	 
	        // Split salt length (always one byte) into four two-bit pieces and
	        // store these pieces in the first four bytes of the salt array.
	        salt[0] = (byte)((salt[0] & 0xfc) | (saltLen & 0x03));
	        salt[1] = (byte)((salt[1] & 0xf3) | (saltLen & 0x0c));
	        salt[2] = (byte)((salt[2] & 0xcf) | (saltLen & 0x30));
	        salt[3] = (byte)((salt[3] & 0x3f) | (saltLen & 0xc0));
	 
	        return salt;
	    }
	 
	    /// <summary>
	    /// Generates random integer.
	    /// </summary>
	    /// <param name="minValue">
	    /// Min value (inclusive).
	    /// </param>
	    /// <param name="maxValue">
	    /// Max value (inclusive).
	    /// </param>
	    /// <returns>
	    /// Random integer value between the min and max values (inclusive).
	    /// </returns>
	    /// <remarks>
	    /// This methods overcomes the limitations of .NET Framework's Random
	    /// class, which - when initialized multiple times within a very short
	    /// period of time - can generate the same "random" number.
	    /// </remarks>
	    private int GenerateRandomNumber(int minValue, int maxValue)
	    {
	        // We will make up an integer seed from 4 bytes of this array.
	        byte[] randomBytes = new byte[4];
	 
	        // Generate 4 random bytes.
	        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
	        rng.GetBytes(randomBytes);
	 
	        // Convert four random bytes into a positive integer value.
	        int seed = ((randomBytes[0] & 0x7f) << 24) |
	                    (randomBytes[1]         << 16) |
	                    (randomBytes[2]         << 8 ) |
	                    (randomBytes[3]);
	 
	        // Now, this looks more like real randomization.
	        Random  random  = new Random(seed);
	 
	        // Calculate a random number.
	        return random.Next(minValue, maxValue+1);
	    }
	    #endregion
	}
}
