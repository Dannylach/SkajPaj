using SkajPajClientWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SkajPajClientWPF.ViewModels
{
    public class RegistrationViewModel : ObservableObject
    {
        private RegistrationModel _registrationModel;
        public RegistrationModel RegistrationModel { get => _registrationModel; set { _registrationModel = value; OnPropertyChanged(); } }

        public RegistrationViewModel()
        {
            RegistrationModel = new RegistrationModel();
            RegistrationModel.Avatar = "1";
        }

        public ICommand RegistrationCommand { get { return new RelayCommandWithParams(Registration, param => this.canExecute); } }

        private void Registration(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
            RegistrationModel.Password = password.ToString();
            RegistrationModel.AddressIP = new IPHelpfulFunktions().GetLocalIPAddress();
            //MessageBox.Show(RegistrationModel.Login + "." + RegistrationModel.Password + "." + RegistrationModel.Avatar + "." + RegistrationModel.AddressIP + ".");

            //TO DO 
            //request to rest api
            //show dialog if data are bad
        }

        private bool canExecute = true;

        public ICommand AvatarRadioButton { get { return new RelayCommandWithParams(AvatarRadioButtonClick, param => this.canExecute); } }

        private void AvatarRadioButtonClick(object avatar)
        {

            RegistrationModel.Avatar = avatar.ToString();
        }


    }
}