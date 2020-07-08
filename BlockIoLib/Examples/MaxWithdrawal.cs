using dotenv.net;
using dotenv.net.Utilities;
using System;
using System.IO;
using System.Text;

namespace BlockIoLib.Examples
{
    class MaxWithdrawal
    {
        private BlockIo blockIo;
        private EnvReader envReader;

        public MaxWithdrawal()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..");
            path = Path.GetFullPath(path) + "\\.env";
            DotEnv.Config(true, path);
            DotEnv.Config(true, path, Encoding.Unicode, false);
            envReader = new EnvReader();

            blockIo = new BlockIo(envReader.GetStringValue("API_KEY"), envReader.GetStringValue("PIN"));
        }

        public void RunMaxWithdrawalExample()
        {
            var balance = blockIo.GetBalance().Data.available_balance;
            
            Console.WriteLine("Balance: " + balance);

            while (true)
            {
                var res = blockIo.Withdraw("{to_address: '" + envReader.GetStringValue("TO_ADDRESS") + "', amount: " + balance + "}");
                double maxWithdraw = res.Data.max_withdrawal_available;

                Console.WriteLine("Max Withdraw Available: " + maxWithdraw.ToString());

                if (maxWithdraw == 0) break;
                blockIo.Withdraw("{to_address: '" + envReader.GetStringValue("TO_ADDRESS") + "', amount: " + maxWithdraw + "}");
            }

            balance = blockIo.GetBalance().Data.available_balance;

            Console.WriteLine("Final Balance: " + balance);
        }
    }
}
