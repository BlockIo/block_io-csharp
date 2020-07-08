using dotenv.net;
using dotenv.net.Utilities;
using System;
using System.IO;
using System.Text;

namespace BlockIoLib.Examples
{
    class Basic
    {
        private BlockIo blockIo;

        public Basic()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..");
            path = Path.GetFullPath(path) + "\\.env";
            DotEnv.Config(true, path);
            DotEnv.Config(true, path, Encoding.Unicode, false);
            var envReader = new EnvReader();

            blockIo = new BlockIo(envReader.GetStringValue("API_KEY"), envReader.GetStringValue("PIN"));
        }

        public void RunBasicExample()
        {
            Console.WriteLine("Get New Address: " + blockIo.GetNewAddress("{label: 'testDest2'}").Data);
            Console.WriteLine("Withdraw from labels: " + blockIo.WithdrawFromLabels("{from_labels: 'default', to_label: 'testDest2', amount: 2.5}").Data);
            Console.WriteLine("Get Address Balance: " + blockIo.GetAddressBalance("{labels: ['default', 'testDest2']}").Data);
            Console.WriteLine("Get Sent Transactions: " + blockIo.GetTransactions("{type: 'sent'}").Data);
            Console.WriteLine("Get Received Transactions: " + blockIo.GetTransactions("{type: 'received'}").Data);
            Console.WriteLine("Get Current Price: " + blockIo.GetCurrentPrice("{base_price: 'BTC'}").Data);
        }
    }
}
