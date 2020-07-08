using dotenv.net;
using dotenv.net.Utilities;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

// Use litecoin api key

namespace BlockIoLib.Examples
{
    class DTrust
    {
        private BlockIo blockIo;
        string DtrustAddress;
        string DtrustAddressLabel;
        List<string> PublicKeys;
        List<Key> PrivKeys;

        public DTrust()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory());
            path = Path.GetFullPath(path) + "/.env";
            DotEnv.Config(true, path);
            DotEnv.Config(true, path, Encoding.Unicode, false);
            var envReader = new EnvReader();

            DtrustAddress = null;
            DtrustAddressLabel = "dTrust1_witness_v0";
            blockIo = new BlockIo(envReader.GetStringValue("API_KEY"), envReader.GetStringValue("PIN"));
            PrivKeys = new List<Key>() {
                new Key().ExtractKeyFromPassphraseString("verysecretkey1"),
                new Key().ExtractKeyFromPassphraseString("verysecretkey2"),
                new Key().ExtractKeyFromPassphraseString("verysecretkey3"),
                new Key().ExtractKeyFromPassphraseString("verysecretkey4")
            }; 
            PublicKeys = new List<string>() {
                PrivKeys[0].PubKey.ToHex(),
                PrivKeys[1].PubKey.ToHex(),
                PrivKeys[2].PubKey.ToHex(),
                PrivKeys[3].PubKey.ToHex()
            }; 
        }

        public void RunDtrustExample()
        {
            string signers = string.Join(",", PublicKeys);
            var res = blockIo.GetNewDtrustAddress("{label: '" + DtrustAddressLabel + "', public_keys: '" + signers + "', required_signatures: 3, address_type: 'witness_v0'}");
            if(res.Status != "success"){
                Console.WriteLine("Error: " + res.Data);
                // if this failed, we probably created the same label before. let's fetch the address then

                res = blockIo.GetDtrustAddressByLabel("{label: '" + DtrustAddressLabel + "'}");
                DtrustAddress = res.Data.address;
            }
            else{
                DtrustAddress = res.Data.address;
            }
            Console.WriteLine("Our dTrust Address: " + DtrustAddress);

            res = blockIo.WithdrawFromLabels("{from_labels: 'default', to_address: '" + DtrustAddress + "', amounts: 0.001}");
            Console.WriteLine("Withdrawal Response: " + res.Data);

            res = blockIo.GetDtrustAddressBalance("{label: '" + DtrustAddressLabel + "'}");
            Console.WriteLine("Dtrust address label Balance: " + res.Data);

            var normalAddress = blockIo.GetAddressByLabel("{label: 'default'}").Data.address;

            Console.WriteLine("Withdrawing from dtrust_address_label to the 'default' label in normal multisig");

            res = blockIo.WithdrawFromDtrustAddress("{from_labels: '" + DtrustAddressLabel + "', to_address: '" + normalAddress + "', amounts: 0.0009}");

            Console.WriteLine("Withdraw from Dtrust Address response: " + res.Data);

            int keyIte;

            foreach(dynamic input in res.Data.inputs)
            {
                keyIte = 0;
                foreach(dynamic signer in input.signers)
                {
                    signer.signed_data = Helper.SignInputs(PrivKeys[keyIte++], input.data_to_sign.ToString(), signer.signer_public_key.ToString());
                }
            }
            Console.WriteLine("Our Signed Request: " + res.Data);
            Console.WriteLine("Finalize Withdrawal: " );
            Console.WriteLine(blockIo.SignAndFinalizeWithdrawal(res.Data.ToString()).Data);
            Console.WriteLine("Get transactions sent by our dtrust_address_label address: ");
            Console.WriteLine(blockIo.GetDtrustTransactions("{type: 'sent', labels: '" + DtrustAddressLabel + "'}").Data);
            
        }
    }
}
