using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF.Models
{
    public class LoginModel : ObservableObject
    {
        private string _login = "test2";// string.Empty;
        public String Login
        {
            get { return _login; }
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        private string _password = string.Empty;
        public String Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public RestWebApiRequest RestWebApiRequest { get => _restWebApiRequest; set => _restWebApiRequest = value; }

        private RestWebApiRequest _restWebApiRequest = new RestWebApiRequest();
    }
}
