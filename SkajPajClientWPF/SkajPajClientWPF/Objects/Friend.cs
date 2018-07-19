using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SkajPajClientWPF.Objects
{
    public class Friend : User
    {
        public Friend(string l, string a, string ip) : base(l, a, ip)
        {
        }

        public Friend(string l, string p, string a, string ip) : base(l, p, a, ip)
        {
        }

        public ICommand DeleteFriendCommand { get { return new RelayCommand(DeleteFriend); } }

        private void DeleteFriend()
        {
            OnClickDelete(new FriendEventArgs(Login));
        }

        public event EventHandler<FriendEventArgs> ClickDelete;

        protected virtual void OnClickDelete(FriendEventArgs e)
        {
            EventHandler<FriendEventArgs> handler = ClickDelete;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public ICommand CallFriendCommand { get { return new RelayCommand(CallFriend); } }

        private void CallFriend()
        {
            MessageBox.Show("Zwonisz do" + Login + " na ip: " + Address_ip);
        }
    }

    public class FriendEventArgs : EventArgs
    {
        public FriendEventArgs(string l)
        {
            Login = l;
        }
        public string Login { get; set; }    
    }
}
