using System;
using NBitcoin;
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
                .Send(to_addr, PreOutputValue - Fee)
                .SendFees(Fee)
                .BuildTransaction(false);

            Assert.AreEqual(unsignedTx.ToHex(), "010000000162be60c1cdbd6b44167d8460756f04feb65ba7bd969ef825a174f576970bd84a0100000000ffffffff01f0a29a3b0000000017a914c99a494597ade09b5194f9ec8e02d96607ae64798700000000");

            var sighash0 = unsignedTx.GetSignatureHash(P2shMultiSig, 0, SigHash.All).ToString();

            Assert.AreEqual(sighash0, "93a075651d1b6b79cd9bf128bf5e15001fe65865defea6cedab0a1da438f565e");
        }
    }
}
