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

            var res = blockIo.SweepFromAddress(new
            {
                to_address = envReader.GetStringValue("TO_ADDRESS"),
                private_key = envReader.GetStringValue("PRIVATE_KEY_FROM_ADDRESS"),
                from_address = envReader.GetStringValue("FROM_ADDRESS")
            });

            if (res.Status == "success")
            {
                Console.WriteLine("Sweep Res: " + res.Data);
                return;
            }

            Console.WriteLine("Error occurred: " + res.Data);
        }
    }
}
