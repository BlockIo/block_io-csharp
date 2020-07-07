using System;
using System.Collections.Generic;
using System.Text;

namespace BlockIoLib
{
    public partial class BlockIo
    {
        public BlockIoResponse<dynamic> Withdraw(string args = "{}") { return _withdraw("POST", "withdraw", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromUser(string args = "{}") { return _withdraw("POST", "withdraw_from_user", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromLabel(string args = "{}") { return _withdraw("POST", "withdraw_from_label", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromAddress(string args = "{}") { return _withdraw("POST", "withdraw_from_address", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromLabels(string args = "{}") { return _withdraw("POST", "withdraw_from_labels", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromAddresses(string args = "{}") { return _withdraw("POST", "withdraw_from_addresses", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromUsers(string args = "{}") { return _withdraw("POST", "withdraw_from_users", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromDtrustAddress(string args = "{}") { return _withdraw("POST", "withdraw_from_dtrust_address", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromDtrustAddresses(string args = "{}") { return _withdraw("POST", "withdraw_from_dtrust_addresses", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromDtrustLabels(string args = "{}") { return _withdraw("POST", "withdraw_from_dtrust_labels", args).Result; }
    }
}
