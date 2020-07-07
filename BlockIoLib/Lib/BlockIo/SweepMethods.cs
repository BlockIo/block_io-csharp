using System;
using System.Collections.Generic;
using System.Text;

namespace BlockIoLib
{
    public partial class BlockIo
    {
        public BlockIoResponse<dynamic> SweepFromAddress(string args = "{}") { return _sweep("POST", "sweep_from_address", args).Result; }
    }
}
