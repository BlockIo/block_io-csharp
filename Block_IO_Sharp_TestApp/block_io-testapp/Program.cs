using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace block_io_testapp
{
    class TestApp
    {
        private static string Pin = "PUT PIN HERE!";

        static void Main(string[] args)
        {

            block_io_sharp.BlockIO Api = new block_io_sharp.BlockIO("REPLACE APIKEY!!!");
            //getNewAddress
            try
            {
                block_io_sharp.APIResponse Response = new block_io_sharp.APIResponse();
                Response = Api.getNewAddress();
                Console.WriteLine("getNewAddress: " + Response.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            //GetBalance
            try
            {
                block_io_sharp.APIResponse Response = new block_io_sharp.APIResponse();
                Response = Api.getBalance();
                
                Console.WriteLine("GetBalance: " + Response.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            //getMyAddresses
            try
            {
                block_io_sharp.APIResponse Response = new block_io_sharp.APIResponse();
                Response = Api.getMyAddresses();
                Console.WriteLine("GetMyAddresses: " + Response.Status);
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            Console.WriteLine();
            //GetAddressBalance-Addresses
            try
            {
                block_io_sharp.APIResponse Response = new block_io_sharp.APIResponse();
                List<string> Addresses = new List<string>();
                Addresses.Add("2N187BaUnkQd5ku2HGRru7fRfLZR7aJRvPo");
                Addresses.Add("2N3kfpjeLwDAi27BZf8sq5585Z9GTivVwQr");
                Response = Api.getAddressBalance("addresses", Addresses);
                Console.WriteLine("GetAddressBalance-addresses: " + Response.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            //GetAddressBalance-Labels
            try
            {
                block_io_sharp.APIResponse Response = new block_io_sharp.APIResponse();
                List<string> Labels = new List<string>();
                Labels.Add("default");
                Labels.Add("testwallet");
                Response = Api.getAddressBalance("Labels", Labels);
                Console.WriteLine("GetAddressBalance-Labels: " + Response.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            //GetAddressByLabel
            try
            {
                block_io_sharp.APIResponse Response = new block_io_sharp.APIResponse();
                Response = Api.getAddressByLabel("default");
                Console.WriteLine("GetAddressByLabel: " + Response.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            //withdraw
            try
            {
                block_io_sharp.APIResponse Response = new block_io_sharp.APIResponse();
                List<string> ToAddresses = new List<string>();
                List<string> Amounts = new List<string>();
                Amounts.Add("10");
                ToAddresses.Add("2N3kfpjeLwDAi27BZf8sq5585Z9GTivVwQr");
                Response = Api.withdraw(Amounts, ToAddresses, Pin);
                Console.WriteLine("Witdraw: " + Response.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            //Withdrawfromaddress
            try
            {
                block_io_sharp.APIResponse Response = new block_io_sharp.APIResponse();
                List<string> FromAddresses = new List<string>();
                List<string> ToAddresses = new List<string>();
                List<string> Amounts = new List<string>();
                FromAddresses.Add("2N187BaUnkQd5ku2HGRru7fRfLZR7aJRvPo");
                ToAddresses.Add("2N3kfpjeLwDAi27BZf8sq5585Z9GTivVwQr");
                Amounts.Add("20");
                Response = Api.withdrawFromAddresses(FromAddresses, ToAddresses, Amounts, Pin);
                Console.WriteLine("withdrawFromAddresses: " + Response.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            //WithdrawFromLabels-ToLabels
            try
            {
                block_io_sharp.APIResponse Response = new block_io_sharp.APIResponse();
                List<string> FromLabels = new List<string>();
                List<string> ToLabels = new List<string>();
                List<string> Amounts = new List<string>();
                FromLabels.Add("default");
                ToLabels.Add("testwallet");
                Amounts.Add("20");
                Response = Api.withdrawFromLabels(FromLabels, Amounts, Pin,null,ToLabels);
                Console.WriteLine("withdrawFromLabels-ToLabels: " + Response.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            //WithdrawFromLabels-ToAddresses
            try
            {
                block_io_sharp.APIResponse Response = new block_io_sharp.APIResponse();
                List<string> FromLabels = new List<string>();
                List<string> ToAddresses = new List<string>();
                List<string> Amounts = new List<string>();
                FromLabels.Add("default");
                ToAddresses.Add("2N3kfpjeLwDAi27BZf8sq5585Z9GTivVwQr");
                Amounts.Add("20");
                Response = Api.withdrawFromLabels(FromLabels, Amounts, Pin, ToAddresses, null);
                Console.WriteLine("withdrawFromLabels-ToAddresses: " + Response.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            //GetCurrentPrice
            try
            {
                block_io_sharp.APIResponse Response = new block_io_sharp.APIResponse();
                Response = Api.getCurrentPrices();
                Console.WriteLine("getCurrentPrice: " + Response.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            //IsGreenAddress
            try
            {
                block_io_sharp.APIResponse Response = new block_io_sharp.APIResponse();
                List<string> Addresses = new List<string>();
                Addresses.Add("2N187BaUnkQd5ku2HGRru7fRfLZR7aJRvPo");
                Response = Api.isGreenAddress(Addresses);
                Console.WriteLine("isGreenAddress: " + Response.Status);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadKey();
        }
    }
}
