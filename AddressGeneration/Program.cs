using System;
using NBitcoin;

namespace AddressGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            ///////BTC////////////
            var BtcPubkey = ExtPubKey.Parse("BIP32 PUBKEY", Network.Main);
            var BtcAddr = BtcPubkey.Derive(0).Derive(0).PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
            Console.WriteLine("Btc P2PKH: " + BtcAddr);
            //////////////////

            ////////BTCTEST/////////////
            var BtcTestPubKey = ExtPubKey.Parse("BIP32 PUBKEY", Network.TestNet);
            var BtcTestAddr = BtcTestPubKey.Derive(0).Derive(0).PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
            Console.WriteLine("BtcTest P2PKH: " + BtcTestAddr);
            /////////////////////////

            ///////LTC////////////
            var LtcPubkey = ExtPubKey.Parse("BIP32 PUBKEY", Litecoin.Instance.Mainnet);
            var LtcAddr = LtcPubkey.Derive(0).Derive(0).PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
            Console.WriteLine("ltc P2PKH: " + LtcAddr);
            //////////////////

            ///////LTCTEST////////////

            var LtcTestPubkey = ExtPubKey.Parse("BIP32 PUBKEY", Litecoin.Instance.Testnet);
            var LtcTestAddr = LtcTestPubkey.Derive(0).Derive(0).PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
            Console.WriteLine("ltcTest P2PKH: " + LtcTestAddr);
            //////////////////

            ///////DOGE////////////

            var DogePubkey = ExtPubKey.Parse("BIP32 PUBKEY", Dogecoin.Instance.Mainnet);
            var DogeAddr = DogePubkey.Derive(0).Derive(0).PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
            Console.WriteLine("Doge P2PKH: " + DogeAddr);
            //////////////////

            ///////DOGETEST////////////

            var DogeTestPubkey = ExtPubKey.Parse("BIP32 PUBKEY", Dogecoin.Instance.Testnet);
            var DogeTestAddr = DogeTestPubkey.Derive(0).Derive(0).PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
            Console.WriteLine("dogetest P2PKH: " + DogeTestAddr);
            //////////////////

        }
    }
}
