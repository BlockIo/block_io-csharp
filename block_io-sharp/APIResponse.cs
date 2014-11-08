using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace block_io_sharp
{
    [DataContract]
    public class APIResponse
    {
        [DataMember]
        public string Status;
        [DataMember]
        public Dictionary<string, object> Data;
    }
}
