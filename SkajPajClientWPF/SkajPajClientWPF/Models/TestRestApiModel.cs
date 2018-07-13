using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF.Models
{
    public class TestRestApiModel : ObservableObject
    {
        private RestClient _restClient;
        public RestClient RestClient
        {
            get { return _restClient; }
            set
            {
                _restClient = value;
                OnPropertyChanged();
            }
        }

        private string _request = string.Empty;
        public string Request
        {
            get { return _request; }
            set
            {
                _request = value;
                OnPropertyChanged();
            }
        }

        private string _response = string.Empty;
        public string Response
        {
            get { return _response; }
            set
            {
                _response = value;
                OnPropertyChanged();
            }
        }
    }
}
