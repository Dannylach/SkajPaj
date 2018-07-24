using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF.Models
{
    public class CallModel : ObservableObject
    {
        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _friendAddressIP = string.Empty;
        private string _callID = string.Empty;

        private string _callState = string.Empty;
        private string _friendAvatar = string.Empty;
        private string _friendLogin = string.Empty;

        public string Login { get => _login; set => _login = value; }
        public string Password { get => _password; set => _password = value; }
        public string FriendAddressIP { get => _friendAddressIP; set => _friendAddressIP = value; }
        public string CallID { get => _callID; set => _callID = value; }
        public string CallState { get => _callState; set => _callState = value; }
        public string FriendAvatar { get => _friendAvatar; set => _friendAvatar = value; }
        public string FriendLogin { get => _friendLogin; set => _friendLogin = value; }
    }
}
