using System;
using System.Collections.Generic;
using System.Text;

namespace BlockIoLib
{
    public partial class BlockIo
    {
        public BlockIoResponse<dynamic> PrepareSweepTransaction(dynamic args = null) { return _prepare_sweep_transaction("POST", "prepare_sweep_transaction", args); }
    }
}
