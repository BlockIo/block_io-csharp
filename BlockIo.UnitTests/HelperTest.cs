using NUnit.Framework;
using System;

namespace BlockIoLib.UnitTests
{
    public class HelperTest
    {
        string pin;
        string aesKey;
        string controlClearText; 
        string controlCipherText;

        [SetUp]
        public void Setup()
        {
            pin = "123456";
            aesKey = Helper.PinToAesKey(pin);
            controlClearText = "I'm a little tea pot short and stout";
            controlCipherText = "7HTfNBYJjq09+vi8hTQhy6lCp3IHv5rztNnKCJ5RB7cSL+NjHrFVv1jl7qkxJsOg";
        }
        [Test]
        public void Sha256()
        {
            var controlData = "5f78c33274e43fa9de5659265c1d917e25c03722dcb0b8d27db8d5feaa813953";
            var testData = "deadbeef";
            var shaData = Helper.ByteArrayToHexString(Helper.SHA256_hash(Helper.HexStringToByteArray(testData)));
            Assert.AreEqual(shaData, controlData);
        }

        [Test]
        public void PinToAes()
        {
            var controlData = "0EeMOVtm5YihUYzdCNgleqIUWkwgvNBcRmr7M0t9GOc=";
            Assert.AreEqual(aesKey, controlData);
        }

		[Test]
		public void PinToAesWithSalt()
		{
			var salt = "922445847c173e90667a19d90729e1fb";
			var s_pin = "deadbeef";
			var encryptionKey = Helper.PinToAesKey(s_pin, salt, 500000);
			Console.WriteLine("encryptionKey={0}", encryptionKey);
			Assert.AreEqual(Helper.ByteArrayToHexString(Convert.FromBase64String(encryptionKey)), "f206403c6bad20e1c8cb1f3318e17cec5b2da0560ed6c7b26826867452534172");
		}
		
        [Test]
        public void EncryptWithAes256Ecb()
        {
            var encryptedData = Helper.Encrypt(controlClearText, aesKey);
            Assert.AreEqual(encryptedData["aes_cipher_text"], controlCipherText);
        }
		
        [Test]
        public void DecryptWithAes256Ecb()
        {
            var decryptedData = Helper.Decrypt(controlCipherText, aesKey);
            Assert.AreEqual(decryptedData, controlClearText);
        }
		
		[Test]
		public void EncryptWithAes256Cbc()
		{
			var encryptionKey = Helper.PinToAesKey("deadbeef", "922445847c173e90667a19d90729e1fb", 500000);
			var encryptedData = Helper.Encrypt("beadbeef", encryptionKey, "11bc22166c8cf8560e5fa7e5c622bb0f", "AES-256-CBC");
			Assert.AreEqual(encryptedData["aes_cipher_text"], "LExu1rUAtIBOekslc328Lw==");
		}
		
		[Test]
		public void DecryptWithAes256Cbc()
		{
			var encryptionKey = Helper.PinToAesKey("deadbeef", "922445847c173e90667a19d90729e1fb", 500000);
			var encryptedData = "LExu1rUAtIBOekslc328Lw==";
			Assert.AreEqual(Helper.Decrypt(encryptedData, encryptionKey, "11bc22166c8cf8560e5fa7e5c622bb0f", "AES-256-CBC"), "beadbeef");
		}
		
    	[Test]
		public void EncryptWithAes256Gcm()
		{
			var encryptionKey = Helper.PinToAesKey("deadbeef", "922445847c173e90667a19d90729e1fb", 500000);
			var encryptedData = Helper.Encrypt("beadbeef", encryptionKey, "a57414b88b67f977829cbdca", "AES-256-GCM", "");
			Assert.AreEqual(encryptedData["aes_cipher_text"], "ELV56Z57KoA=");
			Assert.AreEqual(encryptedData["aes_auth_tag"], "adeb7dfe53027bdda5824dc524d5e55a");
		}
		
		[Test]
		public void DecryptWithAes256Gcm()
		{
			var encryptionKey = Helper.PinToAesKey("deadbeef", "922445847c173e90667a19d90729e1fb", 500000);
			var encryptedData = "ELV56Z57KoA=";
			var authTag = "adeb7dfe53027bdda5824dc524d5e55a";
			Assert.AreEqual(Helper.Decrypt(encryptedData, encryptionKey, "a57414b88b67f977829cbdca", "AES-256-GCM", authTag, ""), "beadbeef");
		}
		
		[Test]
		public void DecryptWithAes256GcmSmallAuthTag()
		{
			var encryptionKey = Helper.PinToAesKey("deadbeef", "922445847c173e90667a19d90729e1fb", 500000);
			var encryptedData = "ELV56Z57KoA=";
			var authTag = "adeb7dfe53027bdda5824dc524d5e5";

			try {
				Helper.Decrypt(encryptedData, encryptionKey, "a57414b88b67f977829cbdca", "AES-256-GCM", authTag, "");
				Assert.Fail("Test failed.");
			} catch (Exception ex) {
				Assert.AreEqual(ex.Message, "Auth tag must be 16 bytes exactly.");
			}
		}
		
    }

}
