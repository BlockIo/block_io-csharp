using System;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Linq;
using System.IO;
using NBitcoin;

namespace BlockIoLib
{
    public class Helper
    {
        public static string Encrypt(string data, string key, string iv = null, string cipher_type = "AES-256-ECB", string auth_data = null)
        {
            using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
            {
                byte[] keyArr = Convert.FromBase64String(key);
                byte[] KeyArrBytes32Value = new byte[32];
                Array.Copy(keyArr, KeyArrBytes32Value, 32);
                csp.Key = keyArr;

		if (cipher_type == "AES-256-ECB") {
		    csp.Padding = PaddingMode.PKCS7;
		    csp.Mode = CipherMode.ECB;
		} else if (cipher_type == "AES-256-CBC") {
		    csp.Padding = PaddingMode.PKCS7;
		    csp.Mode = CipherMode.CBC;
		} else if (cipher_type == "AES-256-GCM") {
		    // TODO using AesGcm class: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aesgcm.encrypt?view=netcore-3.1#System_Security_Cryptography_AesGcm_Encrypt_System_Byte___System_Byte___System_Byte___System_Byte___System_Byte___
		} else {
		    throw new Exception("Unsupported cipher " + cipher_type);
		}
		
		if (iv != null)
		    csp.IV = HexStringToByteArray(iv);

                ICryptoTransform encrypter = csp.CreateEncryptor();
                return Convert.ToBase64String(encrypter.TransformFinalBlock(ASCIIEncoding.UTF8.GetBytes(data), 0, ASCIIEncoding.UTF8.GetBytes(data).Length));
            }
        }

        public static string Decrypt(string data, string key, string iv = null, string cipher_type = "AES-256-ECB", string auth_tag = null, string auth_data = null)
        { // encrypted_data, b64_enc_key, iv = nil, cipher_type = "AES-256-ECB", auth_tag = nil, auth_data = nil
            using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
            {
                byte[] keyArr = Convert.FromBase64String(key);
                byte[] KeyArrBytes32Value = new byte[32];
                Array.Copy(keyArr, KeyArrBytes32Value, 32);
                csp.Key = keyArr;

		if (cipher_type == "AES-256-ECB") {
		    csp.Padding = PaddingMode.PKCS7;
		    csp.Mode = CipherMode.ECB;
		} else if (cipher_type == "AES-256-CBC") {
		    csp.Padding = PaddingMode.PKCS7;
		    csp.Mode = CipherMode.CBC;
		} else if (cipher_type == "AES-256-GCM") {
		    // TODO implement using AesGcm class
		} else {
		    throw new Exception("Unsupported cipher " + cipher_type);
		}
		
		if (iv != null)
		    csp.IV = HexStringToByteArray(iv);

                ICryptoTransform decrypter = csp.CreateDecryptor();
                return ASCIIEncoding.UTF8.GetString(decrypter.TransformFinalBlock(Convert.FromBase64String(data), 0, Convert.FromBase64String(data).Length));
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
