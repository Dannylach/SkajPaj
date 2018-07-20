using SkajPajClientWPF.Models;
using SkajPajClientWPF.Views;
using System;
using System.Windows;
using System.Windows.Input;
using SkajPajClientWPF.Audio;

namespace SkajPajClientWPF.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public CallingManager callingManager = new CallingManager();

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

            MainModel.FriendList = MainModel.RestWebApiRequest.ListOfFriends(login, password);
            
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

        public ICommand AddFriendCommand { get { return new RelayCommand(AddFriend); } }

        private void AddFriend()
        {
            if (MainModel.UserData.Login != MainModel.LoginAddFriend)
            {
                if (MainModel.RestWebApiRequest.AddFriend(MainModel.UserData.Login, MainModel.UserData.Password, MainModel.LoginAddFriend)){
                    MainModel.FriendList = MainModel.RestWebApiRequest.ListOfFriends(MainModel.UserData.Login, MainModel.UserData.Password);
                }
                else
                {
                    MessageBox.Show("Nieudało się dodać znajomego do listy kontaktów. Sprawdź czy login jest poprawny.");
                }
            }
        }
    }
}