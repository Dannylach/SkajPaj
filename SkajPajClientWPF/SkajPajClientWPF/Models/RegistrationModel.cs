using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF.Models
{
    public class RegistrationModel : ObservableObject
    {
        private string _login = string.Empty;
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

        private string _avatar = string.Empty;
        public String Avatar
        {
            get { return _avatar; }
            set
            {
                _avatar = value;
                OnPropertyChanged();
            }
        }

        private string _addressIP = string.Empty;
        public String AddressIP
        {
            get { return _addressIP; }
            set
            {
                _addressIP = value;
                OnPropertyChanged();
            }
        }

        public RestWebApiRequest RestWebApiRequest { get => _restWebApiRequest; set => _restWebApiRequest = value; }

        private RestWebApiRequest _restWebApiRequest = new RestWebApiRequest();

    }
}
