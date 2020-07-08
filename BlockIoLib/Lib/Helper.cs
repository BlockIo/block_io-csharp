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
        public static string Encrypt(string data, string key)
        {
            using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
            {
                byte[] keyArr = Convert.FromBase64String(key);
                byte[] KeyArrBytes32Value = new byte[32];
                Array.Copy(keyArr, KeyArrBytes32Value, 32);
                csp.Key = keyArr;
                csp.Padding = PaddingMode.PKCS7;
                csp.Mode = CipherMode.ECB;
                ICryptoTransform encrypter = csp.CreateEncryptor();
                return Convert.ToBase64String(encrypter.TransformFinalBlock(ASCIIEncoding.UTF8.GetBytes(data), 0, ASCIIEncoding.UTF8.GetBytes(data).Length));
            }
        }

        public static string Decrypt(string data, string key)
        {
            string plaintext = null;
            using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
            {
                byte[] keyArr = Convert.FromBase64String(key);
                byte[] KeyArrBytes32Value = new byte[32];
                Array.Copy(keyArr, KeyArrBytes32Value, 32);
                csp.Key = Convert.FromBase64String(key);
                csp.Padding = PaddingMode.PKCS7;
                csp.Mode = CipherMode.ECB;
                ICryptoTransform decrypter = csp.CreateDecryptor();

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(data)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decrypter, CryptoStreamMode.Read))
                    {

                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            try
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                            
                            catch (Exception ex)
                            {
                                if(ex.Message.Contains("Padding is invalid and cannot be removed."))
                                {
                                    throw new Exception("Pin supplied is incorrect.");
                                }
                                
                            }
                        }
                    }
                }

                return plaintext;
            }
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "").ToLower();
        }

        public static string PinToAesKey(string pin)
        {
            byte[] salt = new byte[0]; //empty salt

            string firstHash = ByteArrayToHexString(KeyDerivation.Pbkdf2(
            password: pin,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1024,
            numBytesRequested: 16));


            byte[] key = KeyDerivation.Pbkdf2(
            password: firstHash.ToLower(),
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1024,
            numBytesRequested: 32);

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

            return "";

        }
    }
}
