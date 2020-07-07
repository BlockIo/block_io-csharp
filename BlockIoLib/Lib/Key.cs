using System;
using Base58Check;
using System.Linq;

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

        public Key ExtractKeyFromEncryptedPassphrase(string EncryptedData, string B64Key)
        {
            string Decrypted = Helper.Decrypt(EncryptedData, B64Key); // this returns a hex string
            byte[] Unhexlified = Helper.HexStringToByteArray(Decrypted);
            byte[] Hashed = Helper.SHA256_hash(Unhexlified);

            return new Key(Hashed);
        }
        public Key ExtractKeyFromPassphrase(string HexPass)
        {
            byte[] Unhexlified = Helper.HexStringToByteArray(HexPass);
            byte[] Hashed = Helper.SHA256_hash(Unhexlified);

            return new Key(Hashed);
        }
    }
}
