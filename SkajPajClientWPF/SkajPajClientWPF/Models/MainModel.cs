using SkajPajClientWPF.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF.Models
{
    public class MainModel : ObservableObject
    {
        public MainModel()
        {

        }

        private User _userData;
        public User UserData { get => _userData; set => _userData = value; }

        private ObservableCollection<User> _friendList;
        public ObservableCollection<User> FriendList { get => _friendList; set => _friendList = value; }

        private RestWebApiRequest _restWebApiRequest = new RestWebApiRequest();
        public RestWebApiRequest RestWebApiRequest { get => _restWebApiRequest; set => _restWebApiRequest = value; }

        
    }
}
