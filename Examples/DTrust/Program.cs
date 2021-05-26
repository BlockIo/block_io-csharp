using dotenv.net;
using dotenv.net.Utilities;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using BlockIoLib;

// Use litecoin api key

namespace DTrust
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

            string DtrustAddress = null;
            string DtrustAddressLabel = "dTrust1_witness_v0";
            BlockIo blockIo = new BlockIo(envReader.GetStringValue("API_KEY"), envReader.GetStringValue("PIN"));

	    // WARNING: THIS IS JUST A DEMO
	    // private keys must always be generated using secure random number generators
	    // for instance, by using new Key(true)
	    // ofcourse, you will store the private keys yourself before using them anywhere
            List<Key> PrivKeys = new List<Key>() {
                new Key().ExtractKeyFromPassphraseString("verysecretkey1"),
                new Key().ExtractKeyFromPassphraseString("verysecretkey2"),
                new Key().ExtractKeyFromPassphraseString("verysecretkey3"),
                new Key().ExtractKeyFromPassphraseString("verysecretkey4")
            };
	    
	    // the public keys for our private keys
            List<string> PublicKeys = new List<string>() {
                PrivKeys[0].PubKey.ToHex(),
                PrivKeys[1].PubKey.ToHex(),
                PrivKeys[2].PubKey.ToHex(),
                PrivKeys[3].PubKey.ToHex()
            };

	    // get the new dTrust address with 3 of our 4 keys as required signers
            string signers = string.Join(",", PublicKeys);
            dynamic res = blockIo.GetNewDtrustAddress(new { label = DtrustAddressLabel, public_keys = signers, required_signatures = "3", address_type = "witness_v0" });

            if (res.Status != "success")
            {
                Console.WriteLine("Error: " + res.Data);
                // if this failed, we probably created the same label before. let's fetch the address then

                res = blockIo.GetDtrustAddressByLabel(new { label = DtrustAddressLabel });
                DtrustAddress = res.Data.address;
            }
            else
            {
                DtrustAddress = res.Data.address;
            }
            Console.WriteLine("Our dTrust Address: " + DtrustAddress);

	    // send coins from our basic wallet to the new dTrust address
	    // below is just a quick demo: you will always inspect data from responses yourself to ensure everything's as you expect it
	    // prepare the transaction
	    res = blockIo.PrepareTransaction(new { from_labels = "default", to_address = DtrustAddress, amounts = "0.0003" });
	    Console.WriteLine("Summarized Prepared Transaction: " + blockIo.SummarizePreparedTransaction(res));
	    
	    // create and sign the transaction
	    res = blockIo.CreateAndSignTransaction(res);
	    
	    // submit the transaction to Block.io for its signature and to broadcast to the peer-to-peer network
	    res = blockIo.SubmitTransaction(new { transaction_data = res });
	    Console.WriteLine("Withdrawal Response: " + res.Data);

            res = blockIo.GetDtrustAddressBalance(new { label = DtrustAddressLabel });
            Console.WriteLine("Dtrust address label Balance: " + res.Data);

            var normalAddress = blockIo.GetAddressByLabel(new { label = "default" }).Data.address.ToString();

            Console.WriteLine("Withdrawing from dtrust_address_label to the 'default' label in normal multisig");

	    // prepare the dTrust transaction
	    res = blockIo.PrepareDtrustTransaction(new { from_labels = DtrustAddressLabel, to_address = normalAddress, amounts = "0.0002" });
	    Console.WriteLine("Summarized Prepared dTrust Transaction: " + blockIo.SummarizePreparedTransaction(res));
	    
	    // create and sign the transaction using just three keys (you can use all 4 keys to create the final transaction for broadcasting as well)
	    res = blockIo.CreateAndSignTransaction(res, PrivKeys.Select(privkey => privkey.ToHex()).ToArray()[0..3]);
	    
	    // submit the transaction
	    res = blockIo.SubmitTransaction(new { transaction_data = res });
            Console.WriteLine("Withdraw from Dtrust Address response: " + res.Data);

            Console.WriteLine("Get transactions sent by our dtrust_address_label address: ");
            Console.WriteLine(blockIo.GetDtrustTransactions(new { type = "sent", labels = DtrustAddressLabel }).Data);
        }
    }
}
