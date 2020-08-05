using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace BlockIoLib
{
    /**
     * A note on args
     * 
     * args are in JSON format and need to be passed like this: 
     * "{param1: 'stringValue', param2: intValue, param3: ['this', 'is', 'a', 'list']}"
     * 
     * 
     * A note on lib method usage
     * 
     * BlockIo blockLib = new BlockIo("{ api_key: 'key' }");
     * var res = blockLib.GetBalance().Data;
     * var status = blockLib.GetBalance().Status;
     */

    public partial class BlockIo
    {
        private readonly RestClient RestClient;
        private readonly string ApiUrl;

        private Dictionary<string, string> Options;
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

        public BlockIo(string ApiKey, string Pin = null, int Version = 2, dynamic Options = null)
        {
            this.Options = Options != null ? Options : new Dictionary<string, string>();
            this.Options.Add("allowNoPin", "false");
            this.ApiUrl = "";
            ApiUrl = this.Options.ContainsKey("api_url") ? this.Options["api_url"] : "";
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

        private string JsonToQuery(string jsonQuery)
        {
            jsonQuery = jsonQuery.Replace(":", "=").Replace("{", "").
                        Replace("}", "").Replace(",", "&").
                            Replace("\"", "").Replace(" ", "").Replace("'", "");

            int lastAmpercant = -1;

            for(int i=0; i<jsonQuery.Length; i++)
            {
                // need to handle ',' within a json array
                //'&' will always come after '=' if at all in a query string
                if (jsonQuery[i] == '&')
                {
                    lastAmpercant = i;
                    jsonQuery = jsonQuery.Remove(i, 1).Insert(i, ",");
                }
                if (jsonQuery[i] == '=')
                {
                    if (lastAmpercant != -1)
                    {
                        jsonQuery = jsonQuery.Remove(lastAmpercant, 1).Insert(lastAmpercant, "&");
                    }
                }
            }
            return jsonQuery.Replace("[", "").Replace("]", "");
        }


        private Task<BlockIoResponse<dynamic>> _withdraw(string Method, string Path, string args)
        {
            BlockIoResponse<dynamic> res = null;
            try
            {
                dynamic argsObj = JsonConvert.DeserializeObject(args);
                string pin = argsObj.pin != null ? argsObj.pin : this.Pin;
                argsObj.pin = "";
                Task<BlockIoResponse<dynamic>> RequestTask = _request(Method, Path, args);
                res = RequestTask.Result;
                if (res.Status == "fail" || res.Data.reference_id == null
                || res.Data.encrypted_passphrase == null || res.Data.encrypted_passphrase.passphrase == null)
                    return RequestTask;

                if (pin == null)
                {
                    if ((string)this.Options["allowNoPin"] == "true")
                    {
                        return RequestTask;
                    }
                    throw new Exception("Public key mismatch. Invalid Secret PIN detected.");
                }

                string enrypted_passphrase = res.Data.encrypted_passphrase.passphrase;
                string aesKey = this.AesKey != null ? this.AesKey : Helper.PinToAesKey(pin);
                Key privKey = new Key().ExtractKeyFromEncryptedPassphrase(enrypted_passphrase, aesKey);
                string pubKey = privKey.PubKey.ToHex();
                if (pubKey != res.Data.encrypted_passphrase.signer_public_key.ToString())
                    throw new Exception("Public key mismatch. Invalid Secret PIN detected.");

                foreach (dynamic input in res.Data.inputs)
                {
                    foreach (dynamic signer in input.signers)
                    {
                        signer.signed_data = Helper.SignInputs(privKey, input.data_to_sign.ToString(), pubKey);
                    }
                }

                aesKey = "";
                privKey = null;

                dynamic signAndFinalizeRequestJson = new { res.Data.reference_id, res.Data.inputs };

                return _request(Method, "sign_and_finalize_withdrawal", JsonConvert.SerializeObject(signAndFinalizeRequestJson));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private Task<BlockIoResponse<dynamic>> _sweep(string Method, string Path, string args)
        {
            Key KeyFromWif = null;
            BlockIoResponse<dynamic> res = null;
            try
            {
                dynamic argsObj = JsonConvert.DeserializeObject(args);

                if(argsObj.to_address == null)
                {
                    throw new Exception("Missing mandatory private_key argument.");
                }

                string PrivKeyStr = argsObj.private_key.ToString();
                KeyFromWif = new Key().FromWif(PrivKeyStr);
                argsObj.public_key = KeyFromWif.PubKey.ToHex();
                argsObj.private_key = "";
                args = JsonConvert.SerializeObject(argsObj);

                Task<BlockIoResponse<dynamic>> RequestTask = _request(Method, Path, args);
                res = RequestTask.Result;
                if (res.Data.reference_id == null)
                    return RequestTask;
                foreach (dynamic input in res.Data.inputs)
                {
                    foreach (dynamic signer in input.signers)
                    {
                        signer.signed_data = Helper.SignInputs(KeyFromWif, input.data_to_sign.ToString(), argsObj.public_key.ToString());
                    }
                }
                KeyFromWif = null;

                dynamic signAndFinalizeRequestJson = new { res.Data.reference_id, res.Data.inputs };

                return _request(Method, "sign_and_finalize_sweep", JsonConvert.SerializeObject(signAndFinalizeRequestJson));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public BlockIoResponse<dynamic> ValidateKey()
        {
            return _request("GET", "get_balance").Result;
        }

        private async Task<BlockIoResponse<dynamic>> _request(string Method, string Path, string args="{}")
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var request = new RestRequest(Path, (Method)Enum.Parse(typeof(Method), Method));

            if (Method == "POST" && !Path.Contains("sign_and_finalize"))
            {
                string queryString = JsonToQuery(args);
                if (args != "{}" )
                {
                    string[] querySegments = queryString.Split('&');
                    foreach (string segment in querySegments)
                    {
                        string[] parts = segment.Split('=');
                        if (parts.Length > 0)
                        {
                            string key = parts[0].Trim(new char[] { '?', ' ' });
                            string val = parts[1].Trim();
                            request.AddParameter(key, val, ParameterType.QueryString);
                        }
                    }
                }
            }
            else
            {
                request.AddJsonBody(new { 
                    signature_data = args
                });
            }
            var response = !Path.Contains("sign_and_finalize") ? await RestClient.ExecuteGetAsync(request) : await RestClient.ExecutePostAsync(request);

            return GetData<BlockIoResponse<dynamic>>(response);
        }
        private T GetData<T>(IRestResponse response)
        {
            var data = JsonConvert.DeserializeObject<T>(response.Content);
            return data;
        }
    }
}
