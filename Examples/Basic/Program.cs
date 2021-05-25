using BlockIoLib;
using dotenv.net;
using dotenv.net.Utilities;
using System;
using System.IO;
using System.Text;

namespace Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory());
            path = Path.GetFullPath(path) + "/.env";
            DotEnv.Config(true, path);
            DotEnv.Config(true, path, Encoding.Unicode, false);
            var envReader = new EnvReader();

	    // initiate the BlockIo library with the API Key and Secret PIN
            BlockIo blockIo = new BlockIo(envReader.GetStringValue("API_KEY"), envReader.GetStringValue("PIN"));

	    // get a new address
            Console.WriteLine("Get New Address: " + blockIo.GetNewAddress(new { label = "testDest2" }).Data);

	    // send coins to our new address
	    var preparedTransaction = blockIo.PrepareTransaction(new { to_label = "testDest2", amount = "0.0005" });
	    Console.WriteLine("Prepared Transaction: " + preparedTransaction.Data);

	    // summarize the prepared transaction
	    // inspect this in-depth yourself, we're just showing the summary here
	    Console.WriteLine("Summarized Prepared Transaction: " + blockIo.SummarizePreparedTransaction(preparedTransaction));

	    // create and sign the prepared transaction
	    // transactionData contains the unsigned tx_hex (inspect it yourself), and your signatures to append to the transaction
	    var transactionData = blockIo.CreateAndSignTransaction(preparedTransaction);

	    // submit the transaction
	    // if partially signed, Block.io will add its signature to complete the transaction
	    // and then broadcast to the peer-to-peer blockchain network
	    Console.WriteLine("Submit Transaction: " + blockIo.SubmitTransaction(new { transaction_data = transactionData }).Data);
	    
            Console.WriteLine("Get Address Balance: " + blockIo.GetAddressBalance(new { labels = "testDest2" }).Data);
            Console.WriteLine("Get Sent Transactions: " + blockIo.GetTransactions(new { type = "sent" }).Data);
            Console.WriteLine("Get Received Transactions: " + blockIo.GetTransactions(new { type = "received" }).Data);
            Console.WriteLine("Get Current Price: " + blockIo.GetCurrentPrice(new { base_price = "BTC" }).Data);
        }
    }
}
