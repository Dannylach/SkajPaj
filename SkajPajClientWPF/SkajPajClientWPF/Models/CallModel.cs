using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public string FriendLogin { get => _friendLogin; set => _friendLogin = value; }

        public string FriendAvatar
        {
            get
            {
                for (int i = 1; i < 13; i++)
                {
                    if (_friendAvatar == i.ToString())
                    {
                        return "..\\Images\\Avatars\\" + _friendAvatar + ".PNG";
                    }
                }
                return "..\\Images\\Avatars\\DEFAULT.PNG"; ;
            }
            set
            {
                _friendAvatar = value;
            }
        }

        public CallModel(string login, string password, string avatar, string friendLogin, string addressIp, string callId, string State)
        {
            Login = login; Password = password; FriendLogin = friendLogin; FriendAddressIP = addressIp;
            CallID = callId; CallState = State; FriendAvatar = avatar;
            changeState(CallState);
        }

        public void changeState(string s)
        {
            CallState = s;
            OpenCallVisibility = "Hidden";
            DecisionCallVisibility = "Hidden";
            VoiceCallVisibility = "Hidden";
            switch (s)
            {
                case "create":
                    OpenCallVisibility = "Visible";
                    break;
                case "select":
                    DecisionCallVisibility = "Visible";
                    break;
                case "call":
                    VoiceCallVisibility = "Visible";
                    break;
                default:
                    MessageBox.Show("ERROR");
                    break;
            }
        }

        private string _button1Visibity;
        public string OpenCallVisibility
        {
            get { return _button1Visibity; }
            set { _button1Visibity = value;
                OnPropertyChanged();

            }
        }
        
        private string _button2Visibity;
        public string DecisionCallVisibility
        {
            get { return _button2Visibity; }
            set { _button2Visibity = value;
                OnPropertyChanged();
            }
        }

        private string _button4Visibity;
        public string VoiceCallVisibility
        {
            get { return _button4Visibity; }
            set { _button4Visibity = value;
                OnPropertyChanged();
            }
        }


        private string _message = string.Empty;
        public string Message { get => _message; set => _message = value; }

        private ObservableCollection<string> _chat = new ObservableCollection<string>();
        public ObservableCollection<string> Chat { get => _chat; set => _chat = value; }

        private bool _isNotInLocalNewtwork = false;
        public bool IsNotInLocalNewtwork { get => _isNotInLocalNewtwork; set => _isNotInLocalNewtwork = value; }

        private RestWebApiRequest _restWebApiRequest = new RestWebApiRequest();
        public RestWebApiRequest RestWebApiRequest { get => _restWebApiRequest; set => _restWebApiRequest = value; }

    }
}