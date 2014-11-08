using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;

namespace block_io_sharp
{
    public class BlockIO
    {
        private string ApiKey { get; set; }
        private string Uri = "https://block.io/api/v1/";
        
        public BlockIO(string _ApiKey)
        {
            this.ApiKey = _ApiKey;
        }

        public APIResponse apiCall (string Method, Dictionary<string, string> Parameters)
        {
            WebClient Client = new WebClient();
            Uri url = new Uri(Uri+ Method + "/");
            Parameters.Add("api_key", this.ApiKey);

            foreach (KeyValuePair<string, string> entry in Parameters)
            {
                url.AddQuery(entry.Key, entry.Value);
            }

            string JsonString = Client.DownloadString(url);
            DataContractJsonSerializer Ser = new DataContractJsonSerializer(typeof (APIResponse));
            byte[] byteArray = Encoding.UTF8.GetBytes(JsonString);
            MemoryStream stream = new MemoryStream(byteArray);
            APIResponse Response = (APIResponse)Ser.ReadObject(stream);

            if (Response.Status == "Success")
            {
                if (Response.Data.ContainsKey("user_id"))
                {
                    Response.Data["user_id"] = Convert.ToInt32(Response.Data["user_id"]);
                }
            }
            else
            {
                throw new Exception("Block.io API Error: " + Response.Data["error_message"]);
            }
            return Response;
        }

        public APIResponse getNewAddress(string label)
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            if (label != null)
            {
                Params.Add("label", label);
            }
            return apiCall("get_new_address", Params);
        }

        public APIResponse getAddressRecieved (Dictionary<string,string> AddressOrLabel)
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            if (AddressOrLabel.ContainsKey("address"))
            {
                Params.Add("address", AddressOrLabel["address"]);
            }
            else if (AddressOrLabel.ContainsKey("label"))
            {
                Params.Add("label", AddressOrLabel["label"]);
            }
            else
            {
                throw new Exception("Missing value for key: address or label");
            }

            return apiCall("get_address_received", Params);
        }

    }
}
