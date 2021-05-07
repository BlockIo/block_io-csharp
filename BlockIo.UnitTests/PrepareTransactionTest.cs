using NBitcoin;
using System;
using NUnit.Framework;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace BlockIoLib.UnitTests
{
    public class PrepareTransactionTest
    {
        Network network;
        string pin;
        Dictionary<string, Key> userKeys;

        public Network getNetwork(string networkString)
        {
            switch(networkString)
            {
                case "BTC":
                    return Network.Main;
                case "LTC":
                    return Litecoin.Instance.Mainnet;
                case "DOGE":
                    return Dogecoin.Instance.Mainnet;
                case "BTCTEST":
                    return Network.TestNet;
                case "LTCTEST":
                    return Litecoin.Instance.Testnet;
                case "DOGETEST":
                    return Dogecoin.Instance.Testnet;
                default:
                    return Network.Main;
            }
        }
        public object CreateAndSignTransaction(dynamic data, string[] keys = null)
        {
            var status = data["status"];
            dynamic dataObj = data["data"];
            var networkString = dataObj["network"].ToString();
            if (network == null && !object.ReferenceEquals(status, null) &&
                status == "success" && !object.ReferenceEquals(dataObj, null) &&
                !object.ReferenceEquals(networkString, null))
            {
                network = getNetwork(networkString);
            }
            dynamic inputs = dataObj["inputs"];
            dynamic outputs = dataObj["outputs"];
            dynamic inputAddressData = dataObj["input_address_data"];

            Coin[] inputCoins = new Coin[inputs.Count];
            string[] inputAddrs = new string[inputs.Count];

            int ite = 0;
            double InputOutputDifference = 0;
            foreach (dynamic input in inputs)
            {
                var preOutputValue = input["input_value"].ToString();

                InputOutputDifference += Convert.ToDouble(preOutputValue);
                var from_addr = BitcoinAddress.Create(input["spending_address"].ToString(), network);
                var preTxId = input["previous_txid"].ToString();
                var preOutputIndex = Convert.ToUInt32(input["previous_output_index"].ToString());

                var txOut = new TxOut(preOutputValue, from_addr);
                var InputCoin = new Coin(new OutPoint(new uint256(preTxId), preOutputIndex), txOut);

                inputCoins[ite] = InputCoin;
                inputAddrs[ite] = from_addr.ToString();
                ite++;
            }

            Dictionary<string, Script> addrScriptMap = new Dictionary<string, Script>();
            Dictionary<string, PubKey[]> addrPubkeysMap = new Dictionary<string, PubKey[]>();

            foreach (dynamic curAddressData in inputAddressData)
            {
                var addressType = curAddressData["address_type"].ToString();

                if(addressType == "P2WSH-over-P2SH" || addressType == "WITNESS_V0" || addressType == "P2SH")
                {
                    PubKey[] curPubKeys = new PubKey[curAddressData["public_keys"].Count];
                    int pubKeyIte = 0;
                    foreach (dynamic pubkey in curAddressData["public_keys"])
                    {
                        curPubKeys[pubKeyIte] = new PubKey(pubkey.ToString());
                        pubKeyIte++;
                    }
                    int requiredSignatures = Convert.ToUInt16(curAddressData["required_signatures"].ToString());
                    var P2shMultiSig = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(requiredSignatures, curPubKeys);

                    addrScriptMap.Add(curAddressData["address"].ToString(), P2shMultiSig);
                    addrPubkeysMap.Add(curAddressData["address"].ToString(), curPubKeys);
                }
                else if(addressType != "P2WPKH-over-P2SH" && addressType != "P2PKH" && addressType != "P2WPKH")
                {
                    throw new Exception("Unrecognized address type: " + addressType);
                }
            }

            for(int i = 0; i < inputCoins.Length; i++)
            {
                if(addrScriptMap.ContainsKey(inputAddrs[i]))
                {
                    inputCoins[i] = inputCoins[i].ToScriptCoin(addrScriptMap[inputAddrs[i]]);
                }
            }
            string encryptionKey;
            if(pin != null && pin != "")
            {
                encryptionKey = Helper.PinToAesKey(pin);
                if(!object.ReferenceEquals(dataObj["user_key"], null) && !userKeys.ContainsKey(dataObj["user_key"]["public_key"].ToString()))
                {
                    string pubkeyStr = dataObj["user_key"]["public_key"].ToString();

                    Key key = new Key().ExtractKeyFromEncryptedPassphrase(dataObj["user_key"]["encrypted_passphrase"].ToString(), encryptionKey);

                    if(key.PubKey.ToHex() != pubkeyStr)
                    {
                        throw new Exception("Fail: Invalid Secret PIN provided.");
                    }

                    userKeys.Add(pubkeyStr, key);
                }
            }

            if(keys != null)
            {
                foreach(string key in keys)
                {
                    Key userKey = new Key().FromWif(key);
                    userKeys.Add(userKey.PubKey.ToHex(), userKey);
                }
            }

            Key[] userKeysArr = new Key[userKeys.Count];
            userKeys.Values.CopyTo(userKeysArr, 0);

            var txBuilder = network.CreateTransactionBuilder();
            txBuilder.ShuffleRandom = null;
            txBuilder.ShuffleInputs = false;
            txBuilder.ShuffleOutputs = false;

            txBuilder.AddCoins(inputCoins);
            txBuilder.AddKeys(userKeysArr);

            foreach (dynamic output in outputs)
            {
                var to_addr = BitcoinAddress.Create(output["receiving_address"].ToString(), network);
                var value = output["output_value"].ToString();

                InputOutputDifference -= Convert.ToDouble(value);
                if (output["output_category"].ToString() == "change")
                {
                    txBuilder.SetChange(to_addr);
                    txBuilder.Send(to_addr, value);
                }
                else
                {
                    txBuilder.Send(to_addr, value);
                }
            }
            txBuilder.SendFees(InputOutputDifference.ToString());

            var unsignedTx = txBuilder.BuildTransaction(false);

            int inputIte = 0;
            foreach(dynamic input in inputs)
            {
                var curPubKeys = addrPubkeysMap[input["spending_address"].ToString()];

                foreach(var pubkey in curPubKeys)
                {
                    if (userKeys.ContainsKey(pubkey.ToHex()))
                    {
                        Key key = userKeys[pubkey.ToHex()];

                        TransactionSignature signature = unsignedTx.SignInput(key, inputCoins[inputIte]);
                        inputIte++;
                        Console.WriteLine(signature);
                    }
                }
            }


            return new object();
        }
        [SetUp]
        public void Setup()
        {
            network = null;
            pin = "d1650160bd8d2bb32bebd139d0063eb6063ffa2f9e4501ad";
            userKeys = new Dictionary<string, Key>();
        }

        [Test]
        public void prepareTransactionResponse()
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

            var response = CreateAndSignTransaction(prepareTransactionResponse);
            Assert.AreEqual(response, createAndSignTransactionResponse);
        }
    }
}
