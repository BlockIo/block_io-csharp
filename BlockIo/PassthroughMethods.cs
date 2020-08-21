using System;
using System.Collections.Generic;
using System.Text;

namespace BlockIoLib
{
    public partial class BlockIo
    {
        public BlockIoResponse<dynamic> GetNewAddress(dynamic args = null) { return _request("POST", "get_new_address", args).Result; }
        public BlockIoResponse<dynamic> GetBalance(dynamic args = null) { return _request("GET", "get_balance", args).Result; }
        public BlockIoResponse<dynamic> GetMyAddresses(dynamic args = null) { return _request("POST", "get_my_addresses", args).Result; }
        public BlockIoResponse<dynamic> GetAddressReceived(dynamic args = null) { return _request("POST", "get_address_received", args).Result; }
        public BlockIoResponse<dynamic> GetAddressByLabel(dynamic args = null) { return _request("POST", "get_address_by_label", args).Result; }
        public BlockIoResponse<dynamic> GetAddressBalance(dynamic args = null) { return _request("POST", "get_address_balance", args).Result; }
        public BlockIoResponse<dynamic> CreateUser(dynamic args = null) { return _request("POST", "create_user", args).Result; }
        public BlockIoResponse<dynamic> GetUsers(dynamic args = null) { return _request("POST", "get_users", args).Result; }
        public BlockIoResponse<dynamic> GetUserBalance(dynamic args = null) { return _request("POST", "get_user_balance", args).Result; }
        public BlockIoResponse<dynamic> GetUserAddress(dynamic args = null) { return _request("POST", "get_user_address", args).Result; }
        public BlockIoResponse<dynamic> GetUserReceived(dynamic args = null) { return _request("POST", "get_user_received", args).Result; }
        public BlockIoResponse<dynamic> GetTransactions(dynamic args = null) { return _request("POST", "get_transactions", args).Result; }
        public BlockIoResponse<dynamic> SignAndFinalizeWithdrawal(dynamic args = null) { return _request("POST", "sign_and_finalize_withdrawal", args).Result; }
        public BlockIoResponse<dynamic> GetNewDtrustAddress(dynamic args = null) { return _request("POST", "get_new_dtrust_address", args).Result; }
        public BlockIoResponse<dynamic> GetMyDtrustAddresses(dynamic args = null) { return _request("POST", "get_my_dtrust_addresses", args).Result; }
        public BlockIoResponse<dynamic> GetDtrustAddressByLabel(dynamic args = null) { return _request("POST", "get_dtrust_address_by_label", args).Result; }
        public BlockIoResponse<dynamic> GetDtrustTransactions(dynamic args = null) { return _request("POST", "get_dtrust_transactions", args).Result; }
        public BlockIoResponse<dynamic> GetDtrustAddressBalance(dynamic args = null) { return _request("POST", "get_dtrust_address_balance", args).Result; }
        public BlockIoResponse<dynamic> GetNetworkFeeEstimate(dynamic args = null) { return _request("POST", "get_network_fee_estimate", args).Result; }
        public BlockIoResponse<dynamic> ArchiveAddress(dynamic args = null) { return _request("POST", "archive_address", args).Result; }
        public BlockIoResponse<dynamic> UnarchiveAddress(dynamic args = null) { return _request("POST", "unarchive_address", args).Result; }
        public BlockIoResponse<dynamic> GetMyArchivedAddresses(dynamic args = null) { return _request("POST", "get_my_archived_addresses", args).Result; }
        public BlockIoResponse<dynamic> ArchiveDtrustAddress(dynamic args = null) { return _request("POST", "archive_dtrust_address", args).Result; }
        public BlockIoResponse<dynamic> UnarchiveDtrustAddress(dynamic args = null) { return _request("POST", "unarchive_dtrust_address", args).Result; }
        public BlockIoResponse<dynamic> GetMyArchivedDtrustAddresses(dynamic args = null) { return _request("POST", "get_my_archived_dtrust_addresses", args).Result; }
        public BlockIoResponse<dynamic> GetDtrustNetworkFeeEstimate(dynamic args = null) { return _request("POST", "get_dtrust_network_fee_estimate", args).Result; }
        public BlockIoResponse<dynamic> CreateNotification(dynamic args = null) { return _request("POST", "create_notification", args).Result; }
        public BlockIoResponse<dynamic> DisableNotification(dynamic args = null) { return _request("POST", "disable_notification", args).Result; }
        public BlockIoResponse<dynamic> EnableNotification(dynamic args = null) { return _request("POST", "enable_notification", args).Result; }
        public BlockIoResponse<dynamic> GetNotifications(dynamic args = null) { return _request("POST", "get_notifications", args).Result; }
        public BlockIoResponse<dynamic> ListNotifications(dynamic args = null) { return _request("GET", "list_notifications", args).Result; }
        public BlockIoResponse<dynamic> GetRecentNotificationEvents(dynamic args = null) { return _request("POST", "get_recent_notification_events", args).Result; }
        public BlockIoResponse<dynamic> DeleteNotification(dynamic args = null) { return _request("POST", "delete_notification", args).Result; }
        public BlockIoResponse<dynamic> ValidateApiKey(dynamic args = null) { return _request("POST", "validate_api_key", args).Result; }
        public BlockIoResponse<dynamic> SignTransation(dynamic args = null) { return _request("POST", "sign_transaction", args).Result; }
        public BlockIoResponse<dynamic> FinalizeTransaction(dynamic args = null) { return _request("POST", "finalize_transaction", args).Result; }
        public BlockIoResponse<dynamic> GetMyAddressesWithoutBalances(dynamic args = null) { return _request("POST", "get_my_addresses_without_balances", args).Result; }
        public BlockIoResponse<dynamic> GetRawTransaction(dynamic args = null) { return _request("POST", "get_raw_transaction", args).Result; }
        public BlockIoResponse<dynamic> GetDtrustBalance(dynamic args = null) { return _request("POST", "get_dtrust_balance", args).Result; }
        public BlockIoResponse<dynamic> ArchiveAddresses(dynamic args = null) { return _request("POST", "archive_addresses", args).Result; }
        public BlockIoResponse<dynamic> UnarchiveAddresses(dynamic args = null) { return _request("POST", "unarchive_addresses", args).Result; }
        public BlockIoResponse<dynamic> ArchiveDtrustAddresses(dynamic args = null) { return _request("POST", "archive_dtrust_addresses", args).Result; }
        public BlockIoResponse<dynamic> UnarchiveDtrustAddresses(dynamic args = null) { return _request("POST", "unarchive_dtrust_addresses", args).Result; }
        public BlockIoResponse<dynamic> IsValidAddress(dynamic args = null) { return _request("POST", "is_valid_address", args).Result; }
        public BlockIoResponse<dynamic> GetCurrentPrice(dynamic args = null) { return _request("POST", "get_current_price", args).Result; }
        public BlockIoResponse<dynamic> GetAccountInfo(dynamic args = null) { return _request("POST", "get_account_info", args).Result; }
    }
}
