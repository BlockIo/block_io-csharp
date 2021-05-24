using System;
using System.Collections.Generic;
using System.Text;

namespace BlockIoLib
{
    public partial class BlockIo
    {
        public BlockIoResponse<dynamic> Withdraw(dynamic args = null) { return _withdraw("POST", "withdraw", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromUser(dynamic args = null) { return _withdraw("POST", "withdraw_from_user", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromLabel(dynamic args = null) { return _withdraw("POST", "withdraw_from_label", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromAddress(dynamic args = null) { return _withdraw("POST", "withdraw_from_address", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromLabels(dynamic args = null) { return _withdraw("POST", "withdraw_from_labels", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromAddresses(dynamic args = null) { return _withdraw("POST", "withdraw_from_addresses", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromUsers(dynamic args = null) { return _withdraw("POST", "withdraw_from_users", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromDtrustAddress(dynamic args = null) { return _withdraw("POST", "withdraw_from_dtrust_address", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromDtrustAddresses(dynamic args = null) { return _withdraw("POST", "withdraw_from_dtrust_addresses", args).Result; }
        public BlockIoResponse<dynamic> WithdrawFromDtrustLabels(dynamic args = null) { return _withdraw("POST", "withdraw_from_dtrust_labels", args).Result; }
    }
}
