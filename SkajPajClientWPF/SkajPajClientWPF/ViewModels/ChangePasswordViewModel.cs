using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SkajPajClientWPF.ViewModels
{
    public class ChangePasswordViewModel : ObservableObject
    {
        public ICommand ChangePasswordCommand { get { return new RelayCommand(ChangePassword); } }

        private void ChangePassword()
        {
            if (SecurePassword.Length > 0 || SecurePassword2.Length > 0)
            {
                if (RestWebApiRequest.ChangePassword(
                    login, 
                    SHA.GenerateSHA256String(PasswordConvert.convertToUNSecureString(SecurePassword)), 
                    SHA.GenerateSHA256String(PasswordConvert.convertToUNSecureString(SecurePassword2))
                    ))
                {
                    MessageBox.Show("Udało się zmienić hasło.");
                    CloseWindow();
                }
                else
                {
                    MessageBox.Show("Nie udało się zmienić hasła");
                }
            }
            else
            {
                MessageBox.Show("Wypełnij pola.");
            }
        }

        private string login = string.Empty;

        public ChangePasswordViewModel()
        {

        }
        public ChangePasswordViewModel(string l)
        {
            login = l;
        }

        private SecureString _securePassword = new SecureString();
        private SecureString _securePassword2 = new SecureString();

        public SecureString SecurePassword { get => _securePassword; set => _securePassword = value; }
        public SecureString SecurePassword2 { get => _securePassword2; set => _securePassword2 = value; }

        public event EventHandler RequestClose;

        private void CloseWindow()
        {
            EventHandler handler = this.RequestClose;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public RestWebApiRequest RestWebApiRequest { get => _restWebApiRequest; set => _restWebApiRequest = value; }

        private RestWebApiRequest _restWebApiRequest = new RestWebApiRequest();
    }
}
