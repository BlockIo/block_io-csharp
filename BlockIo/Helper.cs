using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Linq;
using System.IO;
using NBitcoin;

namespace BlockIoLib
{
    public class Helper
    {
        public static Dictionary<string,string> Encrypt(string data, string key, string iv = null, string cipher_type = "AES-256-ECB", string auth_data = null)
        {

			Dictionary<string,string> response = new Dictionary<string,string>();
			response.Add("aes_iv", iv);
			response.Add("aes_cipher", cipher_type);
			response.Add("aes_auth_data", auth_data);
			
			byte[] keyArr = Convert.FromBase64String(key);
			byte[] KeyArrBytes32Value = new byte[32];
			Array.Copy(keyArr, KeyArrBytes32Value, 32);
			
			if (cipher_type != "AES-256-GCM") {
				using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
				{
					csp.Key = keyArr;
					
					if (cipher_type == "AES-256-ECB") {
						csp.Padding = PaddingMode.PKCS7;
						csp.Mode = CipherMode.ECB;
					} else if (cipher_type == "AES-256-CBC") {
						csp.Padding = PaddingMode.PKCS7;
						csp.Mode = CipherMode.CBC;
					} else {
						throw new Exception("Unsupported cipher " + cipher_type);
					}
					
					if (iv != null)
						csp.IV = HexStringToByteArray(iv);
					
					ICryptoTransform encrypter = csp.CreateEncryptor();
					response.Add("aes_cipher_text", Convert.ToBase64String(encrypter.TransformFinalBlock(ASCIIEncoding.UTF8.GetBytes(data), 0, ASCIIEncoding.UTF8.GetBytes(data).Length)));
					response.Add("aes_auth_tag", null);
				}
			} else {
				// AES-256-GCM
				
				using (var cipher = new AesGcm(keyArr))
				{
					byte[] authTag = new byte[16];
					byte[] cipherText = new byte[ASCIIEncoding.UTF8.GetBytes(data).Length];
					byte[] nonce = HexStringToByteArray(iv);
					byte[] associatedData = HexStringToByteArray(auth_data);
					
					cipher.Encrypt(nonce, ASCIIEncoding.UTF8.GetBytes(data), cipherText, authTag, associatedData);

					response.Add("aes_cipher_text", Convert.ToBase64String(cipherText));
					response.Add("aes_auth_tag", ByteArrayToHexString(authTag));
				}
			}

			return response;
        }

        public static string Decrypt(string data, string key, string iv = null, string cipher_type = "AES-256-ECB", string auth_tag = null, string auth_data = null)
        {
			
			byte[] keyArr = Convert.FromBase64String(key);
			byte[] KeyArrBytes32Value = new byte[32];
			Array.Copy(keyArr, KeyArrBytes32Value, 32);
			
			if (cipher_type != "AES-256-GCM") {
				using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
				{
					csp.Key = keyArr;
					
					if (cipher_type == "AES-256-ECB") {
						csp.Padding = PaddingMode.PKCS7;
						csp.Mode = CipherMode.ECB;
					} else if (cipher_type == "AES-256-CBC") {
						csp.Padding = PaddingMode.PKCS7;
						csp.Mode = CipherMode.CBC;
					} else {
						throw new Exception("Unsupported cipher " + cipher_type);
					}
					
					if (iv != null)
						csp.IV = HexStringToByteArray(iv);
					
					ICryptoTransform decrypter = csp.CreateDecryptor();
					return ASCIIEncoding.UTF8.GetString(decrypter.TransformFinalBlock(Convert.FromBase64String(data), 0, Convert.FromBase64String(data).Length));
				}
			} else {
				// AES-256-GCM

				if (auth_tag.Length != 32)
					throw new Exception("Auth tag must be 16 bytes exactly.");
				
				using (var cipher = new AesGcm(keyArr))
				{
					byte[] authTag = HexStringToByteArray(auth_tag);
					byte[] cipherText = Convert.FromBase64String(data);
					byte[] nonce = HexStringToByteArray(iv);
					byte[] associatedData = HexStringToByteArray(auth_data);
					byte[] decryptedData = new byte[Convert.FromBase64String(data).Length];
					
					cipher.Decrypt(nonce, cipherText, authTag, decryptedData, associatedData);

					return ASCIIEncoding.UTF8.GetString(decryptedData);
				}

			}
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "").ToLower();
        }

        public static string PinToAesKey(string pin, string salt = "", int iterations = 2048, int phase1_key_length = 16, int phase2_key_length = 32, string hash_function = "SHA256")
        {

	    if (hash_function != "SHA256")
		throw new Exception("Unknown hash function specified. Are you using current version of this library?");

            string firstHash = ByteArrayToHexString(KeyDerivation.Pbkdf2(
            password: pin,
            salt: ASCIIEncoding.UTF8.GetBytes(salt), // TODO: why is this different from HexStringToByteArray?
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: iterations/2,
            numBytesRequested: phase1_key_length));

            byte[] key = KeyDerivation.Pbkdf2(
            password: firstHash.ToLower(),
            salt: ASCIIEncoding.UTF8.GetBytes(salt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: iterations/2,
            numBytesRequested: phase2_key_length);

            return Convert.ToBase64String(key);
        }

        public static byte[] SHA256_hash(byte[] value)
        {
            return new SHA256CryptoServiceProvider().ComputeHash(value);
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        public static string SignInputs(Key PrivKey, string DataToSign, string PubKeyToVerify)
        {
            var PubKey = PrivKey.PubKey.ToHex();
            if(PubKey == PubKeyToVerify)
                return ByteArrayToHexString(PrivKey.Sign(new uint256 (HexStringToByteArray(DataToSign))).ToDER());

            return null;

        }
    }
}
