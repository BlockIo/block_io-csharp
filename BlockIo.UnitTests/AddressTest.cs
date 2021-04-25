using NBitcoin;
using NUnit.Framework;

namespace BlockIoLib.UnitTests
{
    public class AddressTest
    {
        Network network;
        Key privkey1;
        Key privkey2;

        [SetUp]
        public void Setup()
        {
            network = Litecoin.Instance.Testnet;
            privkey1 = new Key(Helper.HexStringToByteArray("ef4fc6cfd682494093bbadf041ba4341afbe22b224432e21a4bc4470c5b939d4"));
            privkey2 = new Key(Helper.HexStringToByteArray("123f37eb9a7f24a120969a1b2d6ac4859fb8080cfc2e8d703abae0f44305fc12"));
        }

        [Test]
        public void TestPubKeys()
        {
            Assert.AreEqual(privkey1.PubKey.ToHex(), "03820317ad251bca573c8fda2b8f26ffc9aae9d5ecb15b50ee08d8f9e009def38e");
            Assert.AreEqual(privkey2.PubKey.ToHex(), "0238de8c9eb2842ecaf0cc61ee6ba23fe4e46f1cfd82eac0910e1d8e865bd76df9");
        }

        [Test]
        public void TestP2WPKHOverP2SHAddress()
        {
            var scriptAddress = privkey1.PubKey.WitHash.GetAddress(network).GetScriptAddress().ToString();
            
            Assert.AreEqual(scriptAddress, "Qgn9vENxxnNCPun8CN6KR1PPB7WCo9oxqc");
        }

        [Test]
        public void TestP2WPKHAddress()
        {
            var P2wpkhAddr = privkey1.PubKey.WitHash.GetAddress(network).ToString();

            Assert.AreEqual(P2wpkhAddr, "tltc1qk2erszs7fp407kh94e6v3yhfq2njczjvg4hnz6");
        }

        [Test]
        public void TestP2SHAddress()
        {
            var P2shMultiSigAddr = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new[] { privkey1.PubKey, privkey2.PubKey }).GetScriptAddress(network).ToString();

            Assert.AreEqual(P2shMultiSigAddr, "QPZMy7ivpYdkJRLhtTx7tj5Fa4doQ2auWk");
        }

        [Test]
        public void TestP2WSHOverP2SHAddress()
        {
            var WitScriptId = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new[] { privkey1.PubKey, privkey2.PubKey }).WitHash;
            var P2shWrappedMultiSigAddr = PayToWitScriptHashTemplate.Instance.GenerateScriptPubKey(WitScriptId).GetScriptAddress(network).ToString();

            Assert.AreEqual(P2shWrappedMultiSigAddr, "QeyxkrKbgKvxbBY1HLiBYjMnZx1HDRMYmd");
        }

        [Test]
        public void TestWitnessV0Address()
        {
            var WitnessV0Address = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new[] { privkey1.PubKey, privkey2.PubKey }).GetWitScriptAddress(network).ToString();

            Assert.AreEqual(WitnessV0Address, "tltc1q6s4cxsg5q4vm0ksst6rxn68h6ksrwumy9tvzgqa6jxuqllxyzh0qxt7q8g");
        }

        [Test]
        public void TestP2PKHAddress()
        {
            var P2pkhAddr = privkey1.PubKey.GetAddress(ScriptPubKeyType.Legacy, network).ToString();

            Assert.AreEqual(P2pkhAddr, "mwop54ocwGjeErSTLCKgKxrdYp1k9o6Cgk");
        }
    }
}
