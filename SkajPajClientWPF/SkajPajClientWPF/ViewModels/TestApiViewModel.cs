using SkajPajClientWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SkajPajClientWPF.ViewModels
{
    public class TestApiViewModel : ObservableObject
    {

        private TestRestApiModel _testApiViewModel;
        public TestRestApiModel TestApiModel
        {
            get { return _testApiViewModel; }
            set
            {
                _testApiViewModel = value;
                OnPropertyChanged();
            }
        }

        public TestApiViewModel()
        {
            TestApiModel = new TestRestApiModel();
            TestApiModel.Request = "link...";
            TestApiModel.Response = "json...";
        }

        public ICommand SendDataCommand { get { return new RelayCommand(SendData);} }

        private void SendData()
        {
            TestApiModel.RestClient = new RestClient();
            TestApiModel.RestClient.endPoint = TestApiModel.Request;

            TestApiModel.Response = TestApiModel.RestClient.makeRequest();
        }

    }


}
