using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkajPajClientWPF.Objects;
using System;
using System.Collections.Generic;
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

        public const string SERVER_DOMAIN = SERVER_DOMAIN_LOCAL;

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
    }
}
