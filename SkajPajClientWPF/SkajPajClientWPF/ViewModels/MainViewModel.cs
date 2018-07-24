﻿using SkajPajClientWPF.Models;
using SkajPajClientWPF.Objects;
using SkajPajClientWPF.Views;
using System;
using System.Windows;
using System.Windows.Input;
using SkajPajClientWPF.Audio;

namespace SkajPajClientWPF.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public AudioManager callingManager = new AudioManager();

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

            UpdateFriendList();
            UpdateCallList();
        }

        private void UpdateFriendList()
        {
            MainModel.FriendList = MainModel.RestWebApiRequest.ListOfFriends(MainModel.UserData.Login, MainModel.UserData.Password);
            foreach (Friend f in MainModel.FriendList)
            {
                f.ClickDelete += new EventHandler<FriendEventArgs>(DeleteFriend);
            }
        }

        private void UpdateCallList()
        {
            MainModel.CallList = MainModel.RestWebApiRequest.CallList(MainModel.UserData.Login, MainModel.UserData.Password);
            foreach (Call f in MainModel.CallList)
            {
                //f.ClickDelete += new EventHandler<FriendEventArgs>(DeleteFriend);
            }
        }

        private void DeleteFriend(object sender, FriendEventArgs f)
        {
            MainModel.RestWebApiRequest.DeleteFriend(MainModel.UserData.Login, MainModel.UserData.Password, f.Login);
            UpdateFriendList();
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
                    UpdateFriendList();
                }
                else
                {
                    MessageBox.Show("Nieudało się dodać znajomego do listy kontaktów. Sprawdź czy login jest poprawny.");
                }
            }
        }

        public ICommand ChangePasswordCommand { get { return new RelayCommand(ChangePassword); } }

        private void ChangePassword()
        {
            ChangePasswordWindow loginnWindow = new ChangePasswordWindow(MainModel.UserData.Login);
            loginnWindow.ShowDialog();
        }
    }
}