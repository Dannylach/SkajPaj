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
            RegistrationModel = new RegistrationModel
            {
                Avatar = "1"
            };
        }

        public ICommand RegistrationCommand { get { return new RelayCommandWithParams(Registration, param => this.canExecute); } }

        private void Registration(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
            RegistrationModel.Password = SHA.GenerateSHA256String(password.ToString());
            RegistrationModel.AddressIP = new IPHelpfulFunktions().GetLocalIPAddress();


            if (RegistrationModel.Login.Length < 3)
            {
                MessageBox.Show("Zbyt krótki login!");
            }
            else if (RegistrationModel.Password.Length < 8)
            {
                MessageBox.Show("Zbyt krótkie hasło!");
            }
            else if (RegistrationModel.Login == RegistrationModel.Password)
            {
                MessageBox.Show("Hasło identyczne jak login!");
            }
            else
            {
                //MessageBox.Show("|" + RegistrationModel.Login + "|" + RegistrationModel.Password + "|" + RegistrationModel.Avatar + "|" + RegistrationModel.AddressIP + "|");
                if(!RegistrationModel.RestWebApiRequest.CreateAccount(RegistrationModel.Login, RegistrationModel.Password, RegistrationModel.Avatar, RegistrationModel.AddressIP))
                {
                    MessageBox.Show("Nieudało się utworzyć konta. Prawdopodobnie login jest już zajęty.");
                }
                else
                {
                    MessageBox.Show("Udało się utworzyć konto.");
                    CloseWindow();
                }
            }
            
        }

        private bool canExecute = true;

        public ICommand AvatarRadioButton { get { return new RelayCommandWithParams(AvatarRadioButtonClick, param => this.canExecute); } }

        private void AvatarRadioButtonClick(object avatar) => RegistrationModel.Avatar = avatar.ToString();

        public event EventHandler RequestClose;

        private void CloseWindow()
        {
            EventHandler handler = this.RequestClose;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}