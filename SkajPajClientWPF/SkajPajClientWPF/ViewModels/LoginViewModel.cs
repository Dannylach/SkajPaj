using SkajPajClientWPF.Models;
using SkajPajClientWPF.Views;
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
    public class LoginViewModel : ObservableObject
    {
        private LoginModel _loginModel;
        public LoginModel LoginModel { get => _loginModel; set { _loginModel = value; OnPropertyChanged(); } }

        public LoginViewModel()
        {
            _loginModel = new LoginModel();
        }

        public ICommand OpenRegistrationWindowCommand { get { return new RelayCommand(OpenRegistrationWindow); } }

        private void OpenRegistrationWindow()
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.ShowDialog();
        }

        public ICommand SignInCommand { get { return new RelayCommandWithParams(SignIn, param => this.canExecute); } }
        private bool canExecute = true;
        private void SignIn(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
            LoginModel.Password = SHA.GenerateSHA256String(password.ToString());

            if (LoginModel.Login.Length < 3)
            {
                MessageBox.Show("Zbyt krótki login!");
            }
            else if (LoginModel.Password.Length < 8)
            {
                MessageBox.Show("Zbyt krótkie hasło!");
            }
            else
            {
                if (!LoginModel.RestWebApiRequest.SignIn(LoginModel.Login, LoginModel.Password))
                {
                    MessageBox.Show("Nieudało się zalogować. Prawdopodobnie login lub hasło jest niepoprawne.");
                }
                else
                {
                    if (LoginModel.RestWebApiRequest.EditAdressIP(LoginModel.Login, LoginModel.Password))
                    {
                        MainWindow mainWindow = new MainWindow(LoginModel.Login, LoginModel.Password);
                        mainWindow.Show();
                        CloseWindow();
                    }
                }
            }
        }

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