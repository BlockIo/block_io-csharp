using dotenv.net;
using dotenv.net.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BlockIoLib.Examples
{
    class Proxy
    {
        private BlockIo blockIo;
        public Proxy()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..");
            path = Path.GetFullPath(path) + "\\.env";
            DotEnv.Config(true, path);
            DotEnv.Config(true, path, Encoding.Unicode, false);
            var envReader = new EnvReader();

            blockIo = new BlockIo(envReader.GetStringValue("API_KEY"), envReader.GetStringValue("PIN"), 2,
                                  "{proxy: {" +
                                  "  hostname: '" + envReader.GetStringValue("PROXY_HOST") + 
                                  "', port: '" + envReader.GetStringValue("PROXY_PORT") + 
                                  "', username: '" + envReader.GetStringValue("PROXY_USER") + 
                                  "', password: '" + envReader.GetStringValue("PROXY_PASSWORD") + 
                                  "'}}");
        }

        public void RunProxyExample()
        {
            Console.WriteLine("Get Balance: " + blockIo.GetBalance().Data);

            Console.WriteLine("Get New Address: " + blockIo.GetNewAddress("{label: 'testDest4'}").Data);
            Console.WriteLine("Withdraw from labels: " + blockIo.WithdrawFromLabels("{from_labels: 'default', to_label: 'testDest4', amount: 0.003}").Data);
            Console.WriteLine("Get Address Balance: " + blockIo.GetAddressBalance("{labels: ['default', 'testDest4']}").Data);
            Console.WriteLine("Get Sent Transactions: " + blockIo.GetTransactions("{type: 'sent'}").Data);
            Console.WriteLine("Get Received Transactions: " + blockIo.GetTransactions("{type: 'received'}").Data);
            Console.WriteLine("Get Current Price: " + blockIo.GetCurrentPrice("{base_price: 'BTC'}").Data);
        }
    }
}
