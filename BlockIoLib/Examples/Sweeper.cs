using dotenv.net;
using dotenv.net.Utilities;
using System;
using System.IO;
using System.Text;

namespace BlockIoLib.Examples
{
    class Sweeper
    {
        private BlockIo blockIo;
        private EnvReader envReader;

        public Sweeper()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..");
            path = Path.GetFullPath(path) + "\\.env";
            DotEnv.Config(true, path);
            DotEnv.Config(true, path, Encoding.Unicode, false);
            envReader = new EnvReader();

            blockIo = new BlockIo(envReader.GetStringValue("API_KEY"));
        }

        public void RunSweeperExample()
        {
            var res = blockIo.SweepFromAddress("{" +
                                  "  to_address: '" + envReader.GetStringValue("TO_ADDRESS") +
                                  "', private_key: '" + envReader.GetStringValue("PRIVATE_KEY_FROM_ADDRESS") +
                                  "', from_addresss: '" + envReader.GetStringValue("FROM_ADDRESS") +
                                  "'}");

            if (res.Status == "success") {
                Console.WriteLine("Sweep Res: " + res.Data);
                return;
            }

            Console.WriteLine("Error occurred: " + res.Data);
        }
    }
}
