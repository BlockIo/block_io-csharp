using System;
using System.Reflection;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using NBitcoin;
using System.Globalization;

namespace BlockIoLib
{
    public partial class BlockIo
    {
        Network network;
        Dictionary<string, Key> userKeys;

		CultureInfo DecimalCulture = new CultureInfo("en-US");
		
        private readonly RestClient RestClient;
        private readonly string ApiUrl;

        private Options Opts;
        private string ApiKey { get; set; }
        private int Version { get; set; }
        private string Server { get; set; }
        private string Port { get; set; }
        private string Pin { get; set; }
        private string AesKey { get; set; }

        private int DefaultVersion = 2;
        private string DefaultServer = "";
        private string DefaultPort = "";
        private string Host = "block.io";
        private string UserAgent;

        public BlockIo(string ApiKey, string Pin = null, int Version = 2, Options Opts = null)
        {
            network = null;
            userKeys = new Dictionary<string, Key>();

            Assembly asm = typeof(BlockIo).Assembly;
            string LibVersion = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            UserAgent = string.Join(":", new string[] { "csharp", "block_io", LibVersion });

            this.Opts = Opts != null ? Opts : new Options();
            ApiUrl = this.Opts.ApiUrl;
            this.Pin = Pin;
            this.AesKey = null;

            this.ApiKey = ApiKey;
            if (Version == -1) this.Version = this.DefaultVersion; else this.Version = Version;
            this.Server = this.DefaultServer;
            this.Port = this.DefaultPort;
            if (Pin != null)
            {
                this.Pin = Pin;
                this.AesKey = Helper.PinToAesKey(this.Pin);
            }

            string ServerString = Server != "" ? Server + "." : Server;  // eg: 'dev.'
            string PortString = Port != "" ? ":" + Port : Port;

            ApiUrl = ApiUrl == "" ? "https://" + ServerString + Host + PortString + "/api/v" + Version.ToString() : ApiUrl;

            RestClient = new RestClient(ApiUrl) { Authenticator = new BlockIoAuthenticator(this.ApiKey) };
        }

	private BlockIoResponse<dynamic> _prepare_sweep_transaction(string Method, string Path, dynamic args)
        { // handle extraction of public key from given WIF private key, store the key for later use, and return the response for prepare_sweep_transaction
	    
            Key KeyFromWif = null;
            BlockIoResponse<dynamic> res = null;
            var argsObj = args;

            if(argsObj.GetType().GetProperty("private_key") == null) throw new Exception("Missing mandatory private_key argument.");
            if(argsObj.GetType().GetProperty("to_address") == null) throw new Exception("Missing mandatory to_address argument.");

            string PrivKeyStr = argsObj.GetType().GetProperty("private_key").GetValue(argsObj, null);
            KeyFromWif = new Key().FromWif(PrivKeyStr);
            string to_address = argsObj.GetType().GetProperty("to_address").GetValue(argsObj, null);
            argsObj = new { to_address, public_key = KeyFromWif.PubKey.ToHex() };
            args = argsObj;

	    userKeys.Add(KeyFromWif.PubKey.ToHex(), KeyFromWif);
	    
	    return _request(Method, Path, args).Result;
            
        }
	
        public BlockIoResponse<dynamic> ValidateKey()
        {
            return _request("GET", "get_balance").Result;
        }

        private async Task<BlockIoResponse<dynamic>> _request(string Method, string Path, dynamic args=null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var request = new RestRequest(Path, (Method)Enum.Parse(typeof(Method), Method));

            if (Method == "POST")
            {
                request.AddJsonBody(args);
            }
	    request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("User-Agent", UserAgent);
            var response = Method == "POST" ? await RestClient.ExecutePostAsync(request) : await RestClient.ExecuteGetAsync(request);

            return GetData<BlockIoResponse<dynamic>>(response);
        }
        private T GetData<T>(IRestResponse response)
        {
            var data = JsonConvert.DeserializeObject<T>(response.Content);
            if (data == null) throw new Exception("No response from the API server");

            return data;
        }

        public object SummarizePreparedTransaction(BlockIoResponse<dynamic> data, CultureInfo ResultCulture = null)
        {
            dynamic inputs = data.Data["inputs"];
            dynamic outputs = data.Data["outputs"];

            var inputSum = new decimal(0);
            var blockIoFee = new decimal(0);
            var changeAmount = new decimal(0);
            var outputSum = new decimal(0);

			// user can specify the localization for the resulting decimals
			if (ResultCulture == null)
				ResultCulture = DecimalCulture;
			
            foreach(dynamic input in inputs)
            {
                inputSum += Decimal.Parse(input["input_value"].ToString(), DecimalCulture);
            }

            foreach(dynamic output in outputs)
            {
                if(output["output_category"].ToString() == "blockio-fee")
                {
                    blockIoFee += Decimal.Parse(output["output_value"].ToString(), DecimalCulture);
                } else if (output["output_category"].ToString() == "change")
                {
                    changeAmount += Decimal.Parse(output["output_value"].ToString(), DecimalCulture);
                } else
                {
                    outputSum += Decimal.Parse(output["output_value"].ToString(), DecimalCulture);
                }
            }

            decimal networkFee = inputSum - outputSum - changeAmount - blockIoFee;

            dynamic returnObj = new
            {
                network = data.Data["network"],
				network_fee = networkFee.ToString("F8", ResultCulture),
				blockio_fee = blockIoFee.ToString("F8", ResultCulture),
				total_amount_to_send = outputSum.ToString("F8", ResultCulture)
            };


            return returnObj;
        }

        public object CreateAndSignTransaction(BlockIoResponse<dynamic> data, string[] keys = null)
        {
	    var status = data.Status;
	    dynamic dataObj = data.Data;
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
            decimal InputOutputDifference = 0;
            foreach (dynamic input in inputs)
            {
                var preOutputValue = input["input_value"].ToString();

                InputOutputDifference += Decimal.Parse(preOutputValue, DecimalCulture);
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
            Dictionary<string, int> addrRequiredSigs = new Dictionary<string, int>();

            foreach (dynamic curAddressData in inputAddressData)
            {
                var addressType = curAddressData["address_type"].ToString();
                int requiredSigs = Convert.ToUInt16(curAddressData["required_signatures"].ToString());

                if (addressType == "P2WSH-over-P2SH" || addressType == "WITNESS_V0" || addressType == "P2SH")
                {
                    PubKey[] curPubKeys = new PubKey[curAddressData["public_keys"].Count];
                    int pubKeyIte = 0;
                    foreach (dynamic pubkey in curAddressData["public_keys"])
                    {
                        curPubKeys[pubKeyIte] = new PubKey(pubkey.ToString());
                        pubKeyIte++;
                    }
                    
                    var P2shMultiSig = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(requiredSigs, curPubKeys);

                    addrScriptMap.Add(curAddressData["address"].ToString(), P2shMultiSig);
                    addrPubkeysMap.Add(curAddressData["address"].ToString(), curPubKeys);
                    addrRequiredSigs.Add(curAddressData["address"].ToString(), requiredSigs);
                }
                else if (addressType == "P2WPKH-over-P2SH" || addressType == "P2PKH" || addressType == "P2WPKH")
                {
                    PubKey[] curPubKey = new PubKey[1];
                    curPubKey[0] = new PubKey(curAddressData["public_keys"][0].ToString());
                    addrPubkeysMap.Add(curAddressData["address"].ToString(), curPubKey);
                    addrRequiredSigs.Add(curAddressData["address"].ToString(), requiredSigs);

                    if(addressType == "P2WPKH-over-P2SH")
                    {
                        var script = curPubKey[0].GetScriptPubKey(ScriptPubKeyType.Segwit);
                        addrScriptMap.Add(curAddressData["address"].ToString(), script);
                    }
                }
                else
                {
                    throw new Exception("Unrecognized address type: " + addressType);
                }
            }

            for (int i = 0; i < inputCoins.Length; i++)
            {
                if (addrScriptMap.ContainsKey(inputAddrs[i]))
                {
                    inputCoins[i] = inputCoins[i].ToScriptCoin(addrScriptMap[inputAddrs[i]]);
                }
            }

            if (keys != null)
            { // user provided some keys, let's index them
                foreach (string key in keys)
                {
                    Key userKey = new Key().FromHex(key);
                    
                    userKeys.Add(userKey.PubKey.ToHex(), userKey);
                }
            }

			if (!object.ReferenceEquals(dataObj["user_key"], null) && !userKeys.ContainsKey(dataObj["user_key"]["public_key"].ToString()))
			{ // we don't have the key to sign for transaction yet
				
				if (Pin != null && Pin != "")
				{ // use the user_key to extract private key dynamically
					
                    string pubkeyStr = dataObj["user_key"]["public_key"].ToString();
					Key key = new Key().DynamicExtractKey(dataObj["user_key"], Pin);

					if (pubkeyStr != key.PubKey.ToHex())
                        throw new Exception("Fail: Invalid Secret PIN provided.");

					// we have the key, let's save it for later use
                    userKeys.Add(pubkeyStr, key);
					
				} else {
					throw new Exception("Fail: No PIN provided to decrypt private key.");
				}
				
            }

            Key[] userKeysArr = new Key[userKeys.Count];
            userKeys.Values.CopyTo(userKeysArr, 0);

            var txBuilder = network.CreateTransactionBuilder();
            txBuilder.ShuffleInputs = false;
            txBuilder.ShuffleOutputs = false;

            txBuilder.AddCoins(inputCoins);
            txBuilder.AddKeys(userKeysArr);

            foreach (dynamic output in outputs)
            {
                var to_addr = BitcoinAddress.Create(output["receiving_address"].ToString(), network);
                var value = output["output_value"].ToString();
                InputOutputDifference -= Decimal.Parse(value, DecimalCulture);
                if (output["output_category"].ToString() == "change")
                {
                    txBuilder.SetChange(to_addr);
                }
                txBuilder.Send(to_addr, value);
            }

            txBuilder.SendFees(InputOutputDifference.ToString());

            var unsignedTx = txBuilder.BuildTransaction(false);

            int inputIte = 0;
            foreach (var coin in inputCoins)
            {
                // adding input coins to txbuilder shuffles the inputs even though shuffle inputs is set to false
                // this is used to reorder the inputs correctly
                unsignedTx.Inputs[inputIte] = new TxIn(coin.Outpoint);
                inputIte++;
            }

            var expectedUnsignedTxId = dataObj["expected_unsigned_txid"];

            if (!object.ReferenceEquals(expectedUnsignedTxId, null) &&
                expectedUnsignedTxId != unsignedTx.GetHash().ToString())
            {
                throw new Exception("Expected unsigned transaction ID mismatch. Please report this error to support@block.io.");
            }

            bool txFullySigned = true;
            List<dynamic> signatures = new List<dynamic>();
            inputIte = 0;

            foreach (dynamic input in inputs)
            {
                var curPubKeys = addrPubkeysMap[input["spending_address"].ToString()];
                var curSignatures = new Dictionary<PubKey, string>();
                var curAddr = input["spending_address"].ToString();

                foreach (var pubkey in curPubKeys)
                {
                    if (userKeys.ContainsKey(pubkey.ToHex()))
                    {
                        Key key = userKeys[pubkey.ToHex()];
                        //TransactionSignature signature = unsignedTx.SignInput(key, inputCoins[inputIte]);
                        TransactionSignature signature = unsignedTx.Inputs.FindIndexedInput(inputCoins[inputIte].Outpoint).Sign(key, inputCoins[inputIte]);
                        var sigString = signature.ToString();

                        // signature contains the sighash at the end (sighash.ALL => 01)
                        // removing here
                        var sighashRemovedSig = sigString.Remove(sigString.Length - 2, 2);
                        curSignatures.Add(pubkey, sighashRemovedSig);
                    }
                }
                if (curSignatures.Count < addrRequiredSigs[curAddr])
                {
                    txFullySigned = false;
                }

                foreach (KeyValuePair<PubKey, string> entry in curSignatures)
                {
                    signatures.Add(new
                    {
                        input_index = inputIte,
                        public_key = entry.Key.ToHex(),
                        signature = entry.Value

                    });
                }

                inputIte++;
            }

            dynamic createAndSignResponse;

            if (txFullySigned)
            {
                signatures = null;
                unsignedTx = txBuilder.SignTransaction(unsignedTx);
            }
            createAndSignResponse = new
            {
                tx_type = dataObj["tx_type"].ToString(),
                tx_hex = unsignedTx.ToHex(),
                signatures
            };

            userKeys.Clear();
            return createAndSignResponse;
        }

        public Network getNetwork(string networkString)
        {
            switch (networkString)
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
    }
}
