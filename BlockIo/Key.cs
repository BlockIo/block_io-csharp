using System;
using Base58Check;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace BlockIoLib
{
    public class Key: NBitcoin.Key
    {
        public Key(byte[] data, int count = -1, bool fCompressedIn = true) : base(data, count = -1, fCompressedIn = true)
        {
            
        }

        //Random key
        public Key(bool fCompressedIn = true) : base(fCompressedIn = true)
        {

        }
        public Key FromHex(string PrivKeyHex)
        {
            return new Key(Helper.HexStringToByteArray(PrivKeyHex.PadLeft(64, '0')));
        }
        public Key FromWif(string PrivKey)
        {
            byte[] ExtendedKeyBytes = Base58CheckEncoding.Decode(PrivKey);
            bool Compressed = false;

            //skip the version byte
            ExtendedKeyBytes = ExtendedKeyBytes.Skip(1).ToArray();
            if (ExtendedKeyBytes.Length == 33)
            {
                if (ExtendedKeyBytes[32] != 0x01)
                {
                    throw new ArgumentException("Invalid compression flag", "PrivKey");
                }
                ExtendedKeyBytes = ExtendedKeyBytes.Take(ExtendedKeyBytes.Count() - 1).ToArray();
                Compressed = true;
            }

            if (ExtendedKeyBytes.Length != 32)
            {
                throw new ArgumentException("Invalid WIF payload length", "PrivKey");
            }

            return new Key(ExtendedKeyBytes, -1, Compressed);
        }

		public Key DynamicExtractKey(dynamic userKey, string secretPin)
		{

			var algorithm = userKey["algorithm"];

			if (object.ReferenceEquals(algorithm, null))
			{ // use the legacy algorithm

				algorithm = new Dictionary<string,dynamic>(){};
				
				algorithm.Add("pbkdf2_salt", "");
				algorithm.Add("pbkdf2_iterations", 2048);
				algorithm.Add("pbkdf2_hash_function", "SHA256");
				algorithm.Add("pbkdf2_phase1_key_length", 16);
				algorithm.Add("pbkdf2_phase2_key_length", 32);
				algorithm.Add("aes_iv", null);
				algorithm.Add("aes_cipher", "AES-256-ECB");
				algorithm.Add("aes_auth_tag", null);
				algorithm.Add("aes_auth_data", null);
			}

			// string pin, string salt = "", int iterations = 2048, int phase1_key_length = 16, int phase2_key_length = 32, string hash_function = "SHA256"
			string B64Key = Helper.PinToAesKey(secretPin, (string)algorithm["pbkdf2_salt"],
											   (int)algorithm["pbkdf2_iterations"],
											   (int)algorithm["pbkdf2_phase1_key_length"],
											   (int)algorithm["pbkdf2_phase2_key_length"],
											   (string)algorithm["pbkdf2_hash_function"]);
			
			// string data, string key, string iv = null, string cipher_type = "AES-256-ECB", string auth_tag = null, string auth_data = null
			string Decrypted = Helper.Decrypt((string)userKey["encrypted_passphrase"],
											  B64Key,
											  (string)algorithm["aes_iv"],
											  (string)algorithm["aes_cipher"],
											  (string)algorithm["aes_auth_tag"],
											  (string)algorithm["aes_auth_data"]);
			
			return this.ExtractKeyFromPassphrase(Decrypted);

		}
		
        public Key ExtractKeyFromEncryptedPassphrase(string EncryptedData, string B64Key)
        {
            string Decrypted = Helper.Decrypt(EncryptedData, B64Key); // this returns a hex string
			return this.ExtractKeyFromPassphrase(Decrypted);
        }

        public Key ExtractKeyFromPassphrase(string HexPass)
        {
            byte[] Unhexlified = Helper.HexStringToByteArray(HexPass);
            byte[] Hashed = Helper.SHA256_hash(Unhexlified);

            return new Key(Hashed);
        }

        public Key ExtractKeyFromPassphraseString(string pass)
        {
            byte[] password = Encoding.ASCII.GetBytes(pass);
            byte[] Hashed = Helper.SHA256_hash(password);

            return new Key(Hashed);
        }
    }
}
