using System;
using NBitcoin;
using NBitcoin.DataEncoders;
using NUnit.Framework;

namespace BlockIoLib.UnitTests
{
    public class NBitcoinTransactionTest
    {
        long Fee;
        long PreOutputValue;
        Network network;
        Key privkey1;
        Key privkey2;

        [SetUp]
        public void Setup()
        {
            Fee = 10000;
            PreOutputValue = 1000000000;
            network = Litecoin.Instance.Testnet;
            privkey1 = new Key(Helper.HexStringToByteArray("ef4fc6cfd682494093bbadf041ba4341afbe22b224432e21a4bc4470c5b939d4"));
            privkey2 = new Key(Helper.HexStringToByteArray("123f37eb9a7f24a120969a1b2d6ac4859fb8080cfc2e8d703abae0f44305fc12"));
        }

        [Test]
        public void TestTransactionP2SHToP2WSHOverP2SH()
        {
            var P2shMultiSig = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new[] { privkey1.PubKey, privkey2.PubKey });
            var WitScriptId = P2shMultiSig.WitHash;

            var from_addr = P2shMultiSig.GetScriptAddress(network);
            var to_addr = PayToWitScriptHashTemplate.Instance.GenerateScriptPubKey(WitScriptId).GetScriptAddress(network);

            Assert.AreEqual(from_addr.ToString(), "QPZMy7ivpYdkJRLhtTx7tj5Fa4doQ2auWk");
            Assert.AreEqual(to_addr.ToString(), "QeyxkrKbgKvxbBY1HLiBYjMnZx1HDRMYmd");
            var txOut = new TxOut(PreOutputValue, from_addr);
            var InputCoin = new ScriptCoin(new OutPoint(new uint256("4ad80b9776f574a125f89e96bda75bb6fe046f7560847d16446bbdcdc160be62"), 1), txOut, P2shMultiSig);

            var txBuilder = network.CreateTransactionBuilder();


            var unsignedTx = txBuilder
                .AddCoins(InputCoin)
                .AddKeys(privkey1, privkey2)
                .Send(to_addr, PreOutputValue - Fee)
                .SendFees(Fee)
                .BuildTransaction(false);

            Assert.AreEqual(unsignedTx.ToHex(), "010000000162be60c1cdbd6b44167d8460756f04feb65ba7bd969ef825a174f576970bd84a0100000000ffffffff01f0a29a3b0000000017a914c99a494597ade09b5194f9ec8e02d96607ae64798700000000");

            var sighash0 = unsignedTx.GetSignatureHash(InputCoin.GetScriptCode(), 0, SigHash.All ).ToString();

            //Assert.AreEqual(sighash0, "93a075651d1b6b79cd9bf128bf5e15001fe65865defea6cedab0a1da438f565e");

            var signedTx = txBuilder.SignTransaction(unsignedTx);

            Assert.AreEqual(signedTx.ToHex(), "010000000162be60c1cdbd6b44167d8460756f04feb65ba7bd969ef825a174f576970bd84a01000000d900473044022009143b07279ef6d5317865672e9fc28ada31314abf242ae786917b92cf027ac002207544d055f2b8bb249dc0294d565c6d538f4e04f9b142331fa103d82e0498a181014730440220561f9c23560c6d994c666b9b327f3ef1d9c0b29d0404396d1d6c7a86fc45fc7d02201909041cbe02fc9367f8ce019278629e3f8eae9b7a33fc8223e6fa89e368bd810147522103820317ad251bca573c8fda2b8f26ffc9aae9d5ecb15b50ee08d8f9e009def38e210238de8c9eb2842ecaf0cc61ee6ba23fe4e46f1cfd82eac0910e1d8e865bd76df952aeffffffff01f0a29a3b0000000017a914c99a494597ade09b5194f9ec8e02d96607ae64798700000000");
            Assert.AreEqual(signedTx.GetHash().ToString(), "2464c6122378ee5ed9a42d5192e15713b107924d05d15b58254eb7b2030118c7");
        }

        [Test]
        public void TestTransactionP2WSHOverP2SHToP2WPKH()
        {
            var P2shMultiSig = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new[] { privkey1.PubKey, privkey2.PubKey });
            var WitScriptId = P2shMultiSig.WitHash;

            var P2shWrappedMultiSig = PayToWitScriptHashTemplate.Instance.GenerateScriptPubKey(WitScriptId);

            var from_addr = P2shWrappedMultiSig.GetScriptAddress(network); // P2wsh-over-P2sh
            var to_addr = BitcoinAddress.Create("tltc1qk2erszs7fp407kh94e6v3yhfq2njczjvg4hnz6", network); //P2WPKH

            var prevTxId = "2464c6122378ee5ed9a42d5192e15713b107924d05d15b58254eb7b2030118c7";
            var prevOutputValue = 1000000000 - Fee;
            var outputValue = prevOutputValue - Fee;

            var txOut = new TxOut(prevOutputValue, from_addr);
            var InputCoin = new ScriptCoin(new OutPoint(new uint256(prevTxId), 0), txOut, P2shMultiSig);

            var txBuilder = network.CreateTransactionBuilder();


            var unsignedTx = txBuilder
                .AddCoins(InputCoin)
                .AddKeys(privkey1, privkey2)
                .Send(to_addr, outputValue)
                .SendFees(Fee)
                .BuildTransaction(false);

            Assert.AreEqual(unsignedTx.ToHex(), "0100000001c7180103b2b74e25585bd1054d9207b11357e192512da4d95eee782312c664240000000000ffffffff01e07b9a3b00000000160014b2b2380a1e486aff5ae5ae74c892e902a72c0a4c00000000");

            var sighash0 = unsignedTx.GetSignatureHash(InputCoin.GetScriptCode(), 0, SigHash.All).ToString();

            //Assert.AreEqual(sighash0, "e1c684f769c0e186be215ece3b7c1f3f23985ecbafafe0c8d43936fcd79eafdc");

            var signedTx = txBuilder.SignTransaction(unsignedTx);

            Assert.AreEqual(signedTx.ToHex(), "01000000000101c7180103b2b74e25585bd1054d9207b11357e192512da4d95eee782312c664240000000023220020d42b8341140559b7da105e8669e8f7d5a03773642ad82403ba91b80ffcc415deffffffff01e07b9a3b00000000160014b2b2380a1e486aff5ae5ae74c892e902a72c0a4c0400473044022067c9f8ed5c8f0770be1b7d44ade72c4d976a2b0e6c4df39ea70923daff26ea5e02205894350de5304d446343fbf95245cd656876a11c94025554bf878b3ecf90db720147304402204ee76a1814b3eb289e492409bd29ebb77088c9c20645c8a63c75bfe44eac41f70220232bcd35a0cc78e88dfa59dc15331023c3d3bb3a8b63e6b753c8ab4599b7bd290147522103820317ad251bca573c8fda2b8f26ffc9aae9d5ecb15b50ee08d8f9e009def38e210238de8c9eb2842ecaf0cc61ee6ba23fe4e46f1cfd82eac0910e1d8e865bd76df952ae00000000");
            Assert.AreEqual(signedTx.GetHash().ToString(), "66a78d3cda988e4c90611b192ae5bd02e0fa70c08c3219110c02594802a42c01");
        }
    }
}
