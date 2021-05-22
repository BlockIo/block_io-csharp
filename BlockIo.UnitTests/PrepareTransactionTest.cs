using NUnit.Framework;
using System.IO;
using Newtonsoft.Json;
using System;

namespace BlockIoLib.UnitTests
{
    public class PrepareTransactionTest
    {
        string pin;
        BlockIo blockIo;
        string[] sweepKey;
        string[] dtrustKeys;


        [SetUp]
        public void Setup()
        {
            pin = "d1650160bd8d2bb32bebd139d0063eb6063ffa2f9e4501ad";
            blockIo = new BlockIo("", pin);

            var sweepKeyFromWif = new Key().FromWif("cTj8Ydq9LhZgttMpxb7YjYSqsZ2ZfmyzVprQgjEzAzQ28frQi4ML");
            sweepKey = new []{ sweepKeyFromWif.ToHex() }; // hex

            dtrustKeys = new[] {
            "b515fd806a662e061b488e78e5d0c2ff46df80083a79818e166300666385c0a2",
            "1584b821c62ecdc554e185222591720d6fe651ed1b820d83f92cdc45c5e21f",
            "2f9090b8aa4ddb32c3b0b8371db1b50e19084c720c30db1d6bb9fcd3a0f78e61",
            "6c1cefdfd9187b36b36c3698c1362642083dcc1941dc76d751481d3aa29ca65"
            };
        }

        [Test]
        public void testCreateAndSignTransaction()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic createAndSignTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_transaction_response.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/create_and_sign_transaction_response.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                createAndSignTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.CreateAndSignTransaction(prepareTransactionResponse);

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, createAndSignTransactionResponse);
        }

        [Test]
        public void testSweepP2WPKH()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic createAndSignTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_sweep_transaction_response_p2wpkh.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/create_and_sign_transaction_response_sweep_p2wpkh.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                createAndSignTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.CreateAndSignTransaction(prepareTransactionResponse, sweepKey);

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, createAndSignTransactionResponse);
        }

        [Test]
        public void testSweepP2WPKHOverP2SH()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic createAndSignTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_sweep_transaction_response_p2wpkh_over_p2sh.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/create_and_sign_transaction_response_sweep_p2wpkh_over_p2sh.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                createAndSignTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.CreateAndSignTransaction(prepareTransactionResponse, sweepKey);

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, createAndSignTransactionResponse);
        }

        [Test]
        public void testSweepP2PKH()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic createAndSignTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_sweep_transaction_response_p2pkh.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/create_and_sign_transaction_response_sweep_p2pkh.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                createAndSignTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.CreateAndSignTransaction(prepareTransactionResponse, sweepKey);

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, createAndSignTransactionResponse);
        }

        [Test]
        public void testDTrustWitnessV04of5Keys()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic createAndSignTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_dtrust_transaction_response_witness_v0.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/create_and_sign_transaction_response_dtrust_witness_v0_4_of_5_keys.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                createAndSignTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.CreateAndSignTransaction(prepareTransactionResponse, dtrustKeys);

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, createAndSignTransactionResponse);
        }

        [Test]
        public void testDTrustWitnessV03of5Keys()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic createAndSignTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_dtrust_transaction_response_witness_v0.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/create_and_sign_transaction_response_dtrust_witness_v0_3_of_5_keys.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                createAndSignTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.CreateAndSignTransaction(prepareTransactionResponse, dtrustKeys[0..3]);

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, createAndSignTransactionResponse);
        }

        [Test]
        public void testDTrustP2WSHOverP2SH4of5Keys()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic createAndSignTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_dtrust_transaction_response_p2wsh_over_p2sh.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/create_and_sign_transaction_response_dtrust_p2wsh_over_p2sh_4_of_5_keys.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                createAndSignTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.CreateAndSignTransaction(prepareTransactionResponse, dtrustKeys);

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, createAndSignTransactionResponse);
        }

        [Test]
        public void testDTrustP2WSHOverP2SH3of5Keys()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic createAndSignTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_dtrust_transaction_response_p2wsh_over_p2sh.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/create_and_sign_transaction_response_dtrust_p2wsh_over_p2sh_3_of_5_keys.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                createAndSignTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.CreateAndSignTransaction(prepareTransactionResponse, dtrustKeys[0..3]);

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, createAndSignTransactionResponse);
        }

        [Test]
        public void testDTrustP2SH4of5Keys()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic createAndSignTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_dtrust_transaction_response_p2sh.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/create_and_sign_transaction_response_dtrust_p2sh_4_of_5_keys.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                createAndSignTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.CreateAndSignTransaction(prepareTransactionResponse, dtrustKeys);

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, createAndSignTransactionResponse);
        }

        [Test]
        public void testDTrustP2SH3of5Keys()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic createAndSignTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_dtrust_transaction_response_p2sh.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/create_and_sign_transaction_response_dtrust_p2sh_3_of_5_keys.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                createAndSignTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.CreateAndSignTransaction(prepareTransactionResponse, dtrustKeys[0..3]);

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, createAndSignTransactionResponse);
        }

        [Test]
        public void testDTrustP2SH3of5UnorderedKeys()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic createAndSignTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_dtrust_transaction_response_p2sh.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/create_and_sign_transaction_response_dtrust_p2sh_3_of_5_keys.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                createAndSignTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.CreateAndSignTransaction(prepareTransactionResponse, new []{ dtrustKeys[1], dtrustKeys[2], dtrustKeys[0] });

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, createAndSignTransactionResponse);
        }

        [Test]
        public void testUseOfExpectedUnsignedTxid()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic createAndSignTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_transaction_response_with_blockio_fee_and_expected_unsigned_txid.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/create_and_sign_transaction_response_with_blockio_fee_and_expected_unsigned_txid.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                createAndSignTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.CreateAndSignTransaction(prepareTransactionResponse);

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, createAndSignTransactionResponse);

            // changing expected id
            prepareTransactionResponse["data"]["expected_unsigned_txid"] = "";

            try
            {
                // this should throw an exception since expected tx id is being changed
                blockIo.CreateAndSignTransaction(prepareTransactionResponse);

            }
            catch (Exception ex)
            {
                Assert.AreEqual("Expected unsigned transaction ID mismatch. Please report this error to support@block.io.", ex.Message);
            }
        }

        [Test]
        public void testSummarizePreparedTransaction()
        {
            dynamic prepareTransactionResponse = new object();
            dynamic summarizedPreparedTransactionResponse = new object();

            using (StreamReader r = new StreamReader("./data/prepare_transaction_response_with_blockio_fee_and_expected_unsigned_txid.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                prepareTransactionResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/summarize_prepared_transaction_response_with_blockio_fee_and_expected_unsigned_txid.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                summarizedPreparedTransactionResponse = JsonConvert.DeserializeObject(json);
            }

            var response = blockIo.SummarizePreparedTransaction(prepareTransactionResponse);

            response = JsonConvert.SerializeObject(response); //convert dynamic object to json string
            response = JsonConvert.DeserializeObject(response); //convert json string back to object

            Assert.AreEqual(response, summarizedPreparedTransactionResponse);
        }
    }
}
