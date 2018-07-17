using SkajPajClientWPF.Models;
using SkajPajClientWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SkajPajClientWPF.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel(string login, string password)
        {
            MainModel = new MainModel();
            try
            {
                MainModel.UserData = MainModel.RestWebApiRequest.ReadUserData(login, password);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public MainViewModel()
        {

        }

        private MainModel _mainModel;
        public MainModel MainModel { get => _mainModel; set => _mainModel = value; }

        public event EventHandler RequestClose;

        private void CloseWindow()
        {
            EventHandler handler = this.RequestClose;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public ICommand LogOutCommand { get { return new RelayCommand(LogOut); } }

        private void LogOut()
        {
            LoginWindow loginnWindow = new LoginWindow();
            loginnWindow.Show();
            CloseWindow();
        }
    }
}
