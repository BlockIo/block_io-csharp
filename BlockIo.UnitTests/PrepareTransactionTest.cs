﻿using NUnit.Framework;
using System.IO;
using Newtonsoft.Json;

namespace BlockIoLib.UnitTests
{
    public class PrepareTransactionTest
    {
        string pin;
        BlockIo blockIo;
        string[] sweepKey;

        
        [SetUp]
        public void Setup()
        {
            pin = "d1650160bd8d2bb32bebd139d0063eb6063ffa2f9e4501ad";
            blockIo = new BlockIo("", pin);
            sweepKey = new []{ "cTj8Ydq9LhZgttMpxb7YjYSqsZ2ZfmyzVprQgjEzAzQ28frQi4ML" };
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
    }
}
