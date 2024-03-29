﻿using Microsoft.VisualBasic;
using NUnit.Framework;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlockIoLib.UnitTests
{
    public class KeyTest
    {
        string wif;
        string passphrase;

        string pubKeyFromWif;
        string controlPubKeyFromWif;
        Key privKeyFromWif;
        string controlPrivKeyFromWif;

        string dataToSign;
        string controlSignedDataWifKey;
        string controlSignedDataPassphraseKey;

        string pubKeyFromPassphrase;
        string controlPubKeyFromPassphrase;
        Key privKeyFromPassphrase;
        string controlPrivKeyFromPassphrase;

        [SetUp]
        public void Setup()
        {
            wif = "L1cq4uDmSKMiViT4DuR8jqJv8AiiSZ9VeJr82yau5nfVQYaAgDdr";
            passphrase = "deadbeef";
            dataToSign = "e76f0f78b7e7474f04cc14ad1343e4cc28f450399a79457d1240511a054afd63";
            controlPrivKeyFromPassphrase = "5f78c33274e43fa9de5659265c1d917e25c03722dcb0b8d27db8d5feaa813953";
            controlPubKeyFromPassphrase = "02953b9dfcec241eec348c12b1db813d3cd5ec9d93923c04d2fa3832208b8c0f84";
            controlPrivKeyFromWif = "833e2256c42b4a41ee0a6ee284c39cf8e1978bc8e878eb7ae87803e22d48caa9";
            controlPubKeyFromWif = "024988bae7e0ade83cb1b6eb0fd81e6161f6657ad5dd91d216fbeab22aea3b61a0";
            controlSignedDataWifKey = "3044022061753424b6936ca4cfcc81b883dab55f16d84d3eaf9d5da77c1e25f54fda963802200d3db78e8f5aac62909c2a89ab1b2b413c00c0860926e824f37a19fa140c79f4";
            controlSignedDataPassphraseKey = "304402204ac97a4cdad5f842e745e27c3ffbe08b3704900baafab602277a5a196c3a4a3202202bacdf06afaf58032383447a9f3e9a42bfaeabf6dbcf9ab275d8f24171d272cf";

            privKeyFromWif = new Key().FromWif(wif);
            pubKeyFromWif = privKeyFromWif.PubKey.ToHex();

            privKeyFromPassphrase = new Key().ExtractKeyFromPassphrase(passphrase);
            pubKeyFromPassphrase = privKeyFromPassphrase.PubKey.ToHex();
        }
        [Test]
        public void PrivKeyFromWif()
        {
            Assert.AreEqual(privKeyFromWif.ToHex(), controlPrivKeyFromWif);
        }
        [Test]
        public void PubKeyFromWif()
        {
            Assert.AreEqual(pubKeyFromWif, controlPubKeyFromWif);
        }
        [Test]
        public void PubKeyFromPassphrase()
        {
            Assert.AreEqual(pubKeyFromPassphrase, controlPubKeyFromPassphrase);
        }
        [Test]
        public void PrivKeyFromPassphrase()
        {
            Assert.AreEqual(privKeyFromPassphrase.ToHex(), controlPrivKeyFromPassphrase);
        }
        [Test]
        public void SignDataWifKey()
        {
            Assert.AreEqual(Helper.SignInputs(privKeyFromWif, dataToSign, pubKeyFromWif), controlSignedDataWifKey);
        }
        [Test]
        public void SignDataPassphraseKey()
        {
            Assert.AreEqual(Helper.SignInputs(privKeyFromPassphrase, dataToSign, pubKeyFromPassphrase), controlSignedDataPassphraseKey);
        }

		[Test]
		public void DynamicExtractKeyWithAes256Ecb()
		{
			var userKey = JsonConvert.DeserializeObject<Dictionary<string,dynamic>>("{\"encrypted_passphrase\":\"3wIJtPoC8KO6S7x6LtrN0g==\",\"public_key\":\"02f87f787bffb30396984cb6b3a9d6830f32d5b656b3e39b0abe4f3b3c35d99323\",\"algorithm\":{\"pbkdf2_salt\":\"\",\"pbkdf2_iterations\":2048,\"pbkdf2_hash_function\":\"SHA256\",\"pbkdf2_phase1_key_length\":16,\"pbkdf2_phase2_key_length\":32,\"aes_iv\":null,\"aes_cipher\":\"AES-256-ECB\",\"aes_auth_tag\":null,\"aes_auth_data\":null}}");
			var key = new Key().DynamicExtractKey(userKey, "deadbeef");
			Assert.AreEqual(key.PubKey.ToHex(), userKey["public_key"]);
		}
		
		[Test]
		public void DynamicExtractKeyWithAes256Cbc()
		{
			var userKey = JsonConvert.DeserializeObject<Dictionary<string,dynamic>>("{\"encrypted_passphrase\":\"LExu1rUAtIBOekslc328Lw==\",\"public_key\":\"02f87f787bffb30396984cb6b3a9d6830f32d5b656b3e39b0abe4f3b3c35d99323\",\"algorithm\":{\"pbkdf2_salt\":\"922445847c173e90667a19d90729e1fb\",\"pbkdf2_iterations\":500000,\"pbkdf2_hash_function\":\"SHA256\",\"pbkdf2_phase1_key_length\":16,\"pbkdf2_phase2_key_length\":32,\"aes_iv\":\"11bc22166c8cf8560e5fa7e5c622bb0f\",\"aes_cipher\":\"AES-256-CBC\",\"aes_auth_tag\":null,\"aes_auth_data\":null}}");
			var key = new Key().DynamicExtractKey(userKey, "deadbeef");
			Assert.AreEqual(key.PubKey.ToHex(), userKey["public_key"]);
		}
		
		[Test]
		public void DynamicExtractKeyWithAes256Gcm()
		{
			var userKey = JsonConvert.DeserializeObject<Dictionary<string,dynamic>>("{\"encrypted_passphrase\":\"ELV56Z57KoA=\",\"public_key\":\"02f87f787bffb30396984cb6b3a9d6830f32d5b656b3e39b0abe4f3b3c35d99323\",\"algorithm\":{\"pbkdf2_salt\":\"922445847c173e90667a19d90729e1fb\",\"pbkdf2_iterations\":500000,\"pbkdf2_hash_function\":\"SHA256\",\"pbkdf2_phase1_key_length\":16,\"pbkdf2_phase2_key_length\":32,\"aes_iv\":\"a57414b88b67f977829cbdca\",\"aes_cipher\":\"AES-256-GCM\",\"aes_auth_tag\":\"adeb7dfe53027bdda5824dc524d5e55a\",\"aes_auth_data\":\"\"}}");
			var key = new Key().DynamicExtractKey(userKey, "deadbeef");
			Assert.AreEqual(key.PubKey.ToHex(), userKey["public_key"]);
		}
		
    }
}