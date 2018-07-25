using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkajPajClientWPF.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SkajPajClientWPF
{
    public class RestWebApiRequest
    {
        public const string SERVER_DOMAIN_INTERNET = "http://skajpaj.azurewebsites.net/api/";
        public const string SERVER_DOMAIN_LOCAL = "http://localhost/PHP-REST-API-Skajpaj-server/api/";

        public const string SERVER_DOMAIN = SERVER_DOMAIN_INTERNET;

        public const string SIGN_IN = "user/sign_in.php?";
        public const string READ_USER_DATA = "user/read_user_data.php?";
        public const string CREATE_ACCOUNT = "user/create_account.php?";
        public const string CHANGE_PASSWORD = "user/change_password.php?";
        public const string EDIT_ADDRESS_IP = "user/edit_address_ip.php?";
        public const string ADD_FRIEND = "friendship/add_friend.php?";
        public const string LIST_OF_FRIENDS = "friendship/list_of_friends.php?";
        public const string DELETE_FRIEND = "friendship/delete_friend.php?";
        public const string CALL_LIST = "call/call_list.php?";
        public const string CREATE_CALL = "call/create_call.php?";
        public const string RECEIVED_CALL = "call/received_call.php?";
        public const string END_CALL = "call/end_call.php?";
        public const string READ_FRIEND_DATA = "user/read_friend_data.php?";
        private object searchResults;

        public RestClient RestClient { get; private set; }

        private string makeRequest(string request)
        {
            RestClient = new RestClient();
            RestClient.endPoint = request;

            return RestClient.makeRequest();
        }

        public bool CreateAccount(string login, string password, string avatar, string address_ip)
        {
            string request = SERVER_DOMAIN + CREATE_ACCOUNT + "login=" + login + "&password=" + password + "&avatar=" + avatar + "&address_ip=" + address_ip;
            string json = makeRequest(request);

            try
            {
                CreateAccountsRequest createAccountsRequest = JsonConvert.DeserializeObject<CreateAccountsRequest>(json);

                return createAccountsRequest.create_account;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return false;
        }

        public bool SignIn(string login, string password)
        {
            string request = SERVER_DOMAIN + SIGN_IN + "login=" + login + "&password=" + password;
            string json = makeRequest(request);

            try
            {
                SignInRequest signInRequest = JsonConvert.DeserializeObject<SignInRequest>(json);

                return signInRequest.is_sign_in;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return false;
        }

        public User ReadUserData(string login, string password)
        {
            string request = SERVER_DOMAIN + READ_USER_DATA + "login=" + login + "&password=" + password;
            string json = makeRequest(request);

            try
            {
                ReadUserDataRequest readUserDataRequest = JsonConvert.DeserializeObject<ReadUserDataRequest>(json);

                if (readUserDataRequest.read_data)
                {
                    return new User(readUserDataRequest.login, readUserDataRequest.password,
                        readUserDataRequest.avatar, readUserDataRequest.address_ip);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }

        public ObservableCollection<Friend> ListOfFriends(string login, string password)
        {
            string request = SERVER_DOMAIN + LIST_OF_FRIENDS + "login=" + login + "&password=" + password;
            string json = makeRequest(request);

            try
            {
                ListOfFriendsRequest listOfFriendsRequest = JsonConvert.DeserializeObject<ListOfFriendsRequest>(json);

                if (listOfFriendsRequest.friends_number>0)
                {
                    ObservableCollection<Friend> result = new ObservableCollection<Friend>();
                    for(int i=listOfFriendsRequest.records.Count-1; i>=0; i--)
                    {
                        result.Add(new Friend(listOfFriendsRequest.records[i].login, listOfFriendsRequest.records[i].avatar, listOfFriendsRequest.records[i].address_ip));
                    }
                    return result; 
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return new ObservableCollection<Friend>();
        }

        public bool AddFriend(string login, string password, string friend_login)
        {
            string request = SERVER_DOMAIN + ADD_FRIEND + "login=" + login + "&password=" + password + "&friend_login=" + friend_login;
            string json = makeRequest(request);

            try
            {
                AddFriendRequest addFriendRequest = JsonConvert.DeserializeObject<AddFriendRequest>(json);

                return addFriendRequest.add_friend;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return false;
        }

        public bool DeleteFriend(string login, string password, string friend_login)
        {
            string request = SERVER_DOMAIN + DELETE_FRIEND + "login=" + login + "&password=" + password + "&friend_login=" + friend_login;
            string json = makeRequest(request);

            try
            {
                DeleteFriendRequest deleteFriendRequest = JsonConvert.DeserializeObject<DeleteFriendRequest>(json);

                return deleteFriendRequest.delete_friend;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return false;
        }

        public ObservableCollection<Call> CallList(string login, string password)
        {
            string request = SERVER_DOMAIN + CALL_LIST + "login=" + login + "&password=" + password;
            string json = makeRequest(request);

            try
            {
                CallListRequest callListRequest = JsonConvert.DeserializeObject<CallListRequest>(json);

                if (callListRequest.calls_number > 0)
                {
                    ObservableCollection<Call> result = new ObservableCollection<Call>();
                    for (int i = callListRequest.records.Count - 1; i >= 0; i--)
                    {
                        Call tmp = new Call(callListRequest.records[i].login, callListRequest.records[i].avatar, callListRequest.records[i].address_ip, callListRequest.records[i].start_date);
                        if (callListRequest.records[i].is_received == "f") tmp.IsReceived = "Nie odebrane";
                        else tmp.IsReceived = "Odebrane";
                        if (callListRequest.records[i].end_date != null) tmp.EndDate = callListRequest.records[i].end_date;
                        result.Add(tmp);
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return new ObservableCollection<Call>();
        }

        public bool ChangePassword(string login, string oldPassword, string newPassword)
        {
            string request = SERVER_DOMAIN + CHANGE_PASSWORD + "login=" + login + "&old_password=" + oldPassword + "&new_password=" + newPassword;
            string json = makeRequest(request);

            try
            {
                ChangePasswordRequest changePasswordRequest = JsonConvert.DeserializeObject<ChangePasswordRequest>(json);

                return changePasswordRequest.change_password;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return false;
        }

        public bool EditAdressIP(string login, string password) {
            string request = SERVER_DOMAIN + EDIT_ADDRESS_IP + "login=" + login + "&password=" + password + "&address_ip=" + new IPHelpfulFunktions().GetLocalIPAddress();
            string json = makeRequest(request);

            try
            {
                EditIPAddressRequest editIPAddressRequest = JsonConvert.DeserializeObject<EditIPAddressRequest>(json);

                return editIPAddressRequest.edit_address_ip;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return false;
        }

        public string CreateCall(string login, string password, string receiver_login)
        {
            string request = SERVER_DOMAIN + CREATE_CALL + "login=" + login + "&password=" + password + "&receiver_login=" + receiver_login;
            string json = makeRequest(request);

            try
            {
                CreateCallRequest editIPAddressRequest = JsonConvert.DeserializeObject<CreateCallRequest>(json);

                return editIPAddressRequest.call_id;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return "0";
        }

        public ReadFriendDataRequest ReadFriendData(string login, string password, string friend_login)
        {
            string request = SERVER_DOMAIN + READ_FRIEND_DATA + "login=" + login + "&password=" + password + "&friend_login=" + friend_login;
            string json = makeRequest(request);

            try
            {
                ReadFriendDataRequest readFriendDataRequest = JsonConvert.DeserializeObject<ReadFriendDataRequest>(json);

                return readFriendDataRequest;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }

        public bool ReceivedCall(string login, string password, string call_id, string is_received)
        {
            string request = SERVER_DOMAIN + RECEIVED_CALL + "login=" + login + "&password=" + password + "&call_id=" + call_id + "&is_received=" + is_received;
            string json = makeRequest(request);

            try
            {
                ReceivedCallRequest receivedCallRequest = JsonConvert.DeserializeObject<ReceivedCallRequest>(json);

                return receivedCallRequest.received_call;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return false;
        }
    }
}