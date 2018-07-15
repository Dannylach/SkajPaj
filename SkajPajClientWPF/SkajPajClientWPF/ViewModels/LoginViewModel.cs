using SkajPajClientWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SkajPajClientWPF.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        public ICommand OpenRegistrationWindowCommand { get { return new RelayCommand(OpenRegistrationWindow); } }

        private void OpenRegistrationWindow()
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.ShowDialog();
        }
    }
}
