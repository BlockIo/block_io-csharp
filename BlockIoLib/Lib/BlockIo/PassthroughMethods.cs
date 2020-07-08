using System;
using System.Collections.Generic;
using System.Text;

namespace BlockIoLib
{
    public partial class BlockIo
    {
        public BlockIoResponse<dynamic> GetNewAddress(string args = "{}") { return _request("POST", "get_new_address", args).Result; }
        public BlockIoResponse<dynamic> GetBalance(string args = "{}") { return _request("GET", "get_balance", args).Result; }
        public BlockIoResponse<dynamic> GetMyAddresses(string args = "{}") { return _request("POST", "get_my_addresses", args).Result; }
        public BlockIoResponse<dynamic> GetAddressReceived(string args = "{}") { return _request("POST", "get_address_received", args).Result; }
        public BlockIoResponse<dynamic> GetAddressByLabel(string args = "{}") { return _request("POST", "get_address_by_label", args).Result; }
        public BlockIoResponse<dynamic> GetAddressBalance(string args = "{}") { return _request("POST", "get_address_balance", args).Result; }
        public BlockIoResponse<dynamic> CreateUser(string args = "{}") { return _request("POST", "create_user", args).Result; }
        public BlockIoResponse<dynamic> GetUsers(string args = "{}") { return _request("POST", "get_users", args).Result; }
        public BlockIoResponse<dynamic> GetUserBalance(string args = "{}") { return _request("POST", "get_user_balance", args).Result; }
        public BlockIoResponse<dynamic> GetUserAddress(string args = "{}") { return _request("POST", "get_user_address", args).Result; }
        public BlockIoResponse<dynamic> GetUserReceived(string args = "{}") { return _request("POST", "get_user_received", args).Result; }
        public BlockIoResponse<dynamic> GetTransactions(string args = "{}") { return _request("POST", "get_transactions", args).Result; }
        public BlockIoResponse<dynamic> SignAndFinalizeWithdrawal(string args = "{}") { return _request("POST", "sign_and_finalize_withdrawal", args).Result; }
        public BlockIoResponse<dynamic> GetMyDtrustAddresses(string args = "{}") { return _request("POST", "get_my_dtrust_addresses", args).Result; }
        public BlockIoResponse<dynamic> GetDtrustAddressByLabel(string args = "{}") { return _request("POST", "get_dtrust_address_by_label", args).Result; }
        public BlockIoResponse<dynamic> GetDtrustTransactions(string args = "{}") { return _request("POST", "get_dtrust_transactions", args).Result; }
        public BlockIoResponse<dynamic> GetDtrustAddressBalance(string args = "{}") { return _request("POST", "get_dtrust_address_balance", args).Result; }
        public BlockIoResponse<dynamic> GetNetworkFeeEstimate(string args = "{}") { return _request("POST", "get_network_fee_estimate", args).Result; }
        public BlockIoResponse<dynamic> ArchiveAddress(string args = "{}") { return _request("POST", "archive_address", args).Result; }
        public BlockIoResponse<dynamic> UnarchiveAddress(string args = "{}") { return _request("POST", "unarchive_address", args).Result; }
        public BlockIoResponse<dynamic> GetMyArchivedAddresses(string args = "{}") { return _request("POST", "get_my_archived_addresses", args).Result; }
        public BlockIoResponse<dynamic> ArchiveDtrustAddress(string args = "{}") { return _request("POST", "archive_dtrust_address", args).Result; }
        public BlockIoResponse<dynamic> UnarchiveDtrustAddress(string args = "{}") { return _request("POST", "unarchive_dtrust_address", args).Result; }
        public BlockIoResponse<dynamic> GetMyArchivedDtrustAddresses(string args = "{}") { return _request("POST", "get_my_archived_dtrust_addresses", args).Result; }
        public BlockIoResponse<dynamic> GetDtrustNetworkFeeEstimate(string args = "{}") { return _request("POST", "get_dtrust_network_fee_estimate", args).Result; }
        public BlockIoResponse<dynamic> CreateNotification(string args = "{}") { return _request("POST", "create_notification", args).Result; }
        public BlockIoResponse<dynamic> DisableNotification(string args = "{}") { return _request("POST", "disable_notification", args).Result; }
        public BlockIoResponse<dynamic> EnableNotification(string args = "{}") { return _request("POST", "enable_notification", args).Result; }
        public BlockIoResponse<dynamic> GetNotifications(string args = "{}") { return _request("POST", "get_notifications", args).Result; }
        public BlockIoResponse<dynamic> GetRecentNotificationEvents(string args = "{}") { return _request("POST", "get_recent_notification_events", args).Result; }
        public BlockIoResponse<dynamic> DeleteNotification(string args = "{}") { return _request("POST", "delete_notification", args).Result; }
        public BlockIoResponse<dynamic> ValidateApiKey(string args = "{}") { return _request("POST", "validate_api_key", args).Result; }
        public BlockIoResponse<dynamic> SignTransation(string args = "{}") { return _request("POST", "sign_transaction", args).Result; }
        public BlockIoResponse<dynamic> FinalizeTransaction(string args = "{}") { return _request("POST", "finalize_transaction", args).Result; }
        public BlockIoResponse<dynamic> GetMyAddressesWithoutBalances(string args = "{}") { return _request("POST", "get_my_addresses_without_balances", args).Result; }
        public BlockIoResponse<dynamic> GetRawTransaction(string args = "{}") { return _request("POST", "get_raw_transaction", args).Result; }
        public BlockIoResponse<dynamic> GetDtrustBalance(string args = "{}") { return _request("POST", "get_dtrust_balance", args).Result; }
        public BlockIoResponse<dynamic> ArchiveAddresses(string args = "{}") { return _request("POST", "archive_addresses", args).Result; }
        public BlockIoResponse<dynamic> UnarchiveAddresses(string args = "{}") { return _request("POST", "unarchive_addresses", args).Result; }
        public BlockIoResponse<dynamic> ArchiveDtrustAddresses(string args = "{}") { return _request("POST", "archive_dtrust_addresses", args).Result; }
        public BlockIoResponse<dynamic> UnarchiveDtrustAddresses(string args = "{}") { return _request("POST", "unarchive_dtrust_addresses", args).Result; }
        public BlockIoResponse<dynamic> IsValidAddress(string args = "{}") { return _request("POST", "is_valid_address", args).Result; }
        public BlockIoResponse<dynamic> GetCurrentPrice(string args = "{}") { return _request("POST", "get_current_price", args).Result; }
        public BlockIoResponse<dynamic> GetAccountInfo(string args = "{}") { return _request("POST", "get_account_info", args).Result; }
    }
}
