using System;
namespace BlockIo

{
    public class Options
    {
        public string ApiUrl
        {
            get; set;
        }
        public bool AllowNoPin
        {
            get; set;
        }
        public Options(string apiUrl = "", bool allowNoPin = false)
        {
            ApiUrl = apiUrl; AllowNoPin = allowNoPin;
        }
    }
}
