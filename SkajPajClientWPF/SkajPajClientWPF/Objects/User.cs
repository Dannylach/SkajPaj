using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF.Objects
{
    public class User : ObservableObject
    {
        private string _login;
        private string _password;
        private string _avatar;
        private string _address_ip;

        public User(string l, string p, string a, string ip)
        {
            Login = l;
            Password = p;
            Avatar = a;
            Address_ip = ip;
        }

        public User(string l, string a, string ip)
        {
            Login = l;
            Password = String.Empty;
            Avatar = a;
            Address_ip = ip;
        }

        public string Login { get => _login; set => _login = value; }
        public string Password { get => _password; set => _password = value; }
        public string Avatar {
            get
            {
                for(int i = 1; i < 13; i++)
                {
                    if (_avatar==i.ToString())
                    {
                        return "..\\Images\\Avatars\\" + _avatar + ".PNG";
                    }
                }
                return "..\\Images\\Avatars\\DEFAULT.PNG"; ;
            }
            set
            {
                _avatar = value;
            }
        }
        public string Address_ip { get => _address_ip; set => _address_ip = value; }
    }
}
