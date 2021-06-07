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
        public void Encrypt()
        {
            var encryptedData = Helper.Encrypt(controlClearText, aesKey);
            Assert.AreEqual(encryptedData, controlCipherText);
        }

        [Test]
        public void Decrypt()
        {
            var decryptedData = Helper.Decrypt(controlCipherText, aesKey);
            Assert.AreEqual(decryptedData, controlClearText);
        }
    }
}
