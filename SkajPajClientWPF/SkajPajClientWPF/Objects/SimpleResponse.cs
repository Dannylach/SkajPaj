using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF.Objects
{
    public class SimpleResponse
    {
        
    }

    public class CreateAccountsRequest
    {
        public string message;
        public bool create_account;
    }

    public class SignInRequest
    {
        public string message;
        public bool is_sign_in;
    }

    public class ReadUserDataRequest
    {
        public string message;
        public bool read_data;
        public string login;
        public string password;
        public string avatar;
        public string address_ip;
    }

    public class ListOfFriendsRequest
    {
        public string message;
        public int friends_number;
        public IList<FriendsRecord> records;
    }

    public class FriendsRecord
    {
        public string login;
        public string avatar;
        public string address_ip;
    }

    public class AddFriendRequest
    {
        public string message;
        public bool add_friend;
    }

    public class DeleteFriendRequest
    {
        public string message;
        public bool delete_friend;
    }

    public class CallListRequest
    {
        public IList<CallRecord> records;
        public string message;
        public int calls_number;
    }

    public class CallRecord
    {
        public string login;
        public string avatar;
        public string address_ip;
        public DateTime start_date;
        public string is_received;
        public DateTime end_date;
    }

    public class ChangePasswordRequest
    {
        public string message;
        public bool change_password;
    }

    public class EditIPAddressRequest
    {
        public string message;
        public bool edit_address_ip;
    }

    public class CreateCallRequest
    {
        public string message;
        public string call_id;
        public bool add_call;
    }

    public class ReadFriendDataRequest
    {
        public string message;
        public bool read_data;
        public string login;
        public string avatar;
        public string address_ip;
    }

    public class ReceivedCallRequest
    {
        public string message;
        public bool received_call;
    }
}
