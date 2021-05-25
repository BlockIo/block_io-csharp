using BlockIoLib;
using dotenv.net;
using dotenv.net.Utilities;
using System;
using System.IO;
using System.Text;

namespace Sweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory());
            path = Path.GetFullPath(path) + "/.env";
            DotEnv.Config(true, path);
            DotEnv.Config(true, path, Encoding.Unicode, false);
            EnvReader envReader = new EnvReader();

            BlockIo blockIo = new BlockIo(envReader.GetStringValue("API_KEY"));

	    // prepare the transaction
            dynamic res = blockIo.PrepareSweepTransaction(new
            {
                to_address = envReader.GetStringValue("TO_ADDRESS"),
                private_key = envReader.GetStringValue("PRIVATE_KEY"),
            });

	    // summarize the transaction
	    // inspect it in-depth yourself to ensure everything as you expect
	    Console.WriteLine("Summarized Prepared Sweep Transaction: " + blockIo.SummarizePreparedTransaction(res));

	    // create and sign the transaction
	    res = blockIo.CreateAndSignTransaction(res);

	    // submit the final transaction to broadcast to the peer-to-peer blockchain network
	    res = blockIo.SubmitTransaction(new { transaction_data = res });
	    
            if (res.Status == "success")
            {
                Console.WriteLine("Sweep Res: " + res.Data);
                return;
            }

            Console.WriteLine("Error occurred: " + res.Data);
        }
    }
}
