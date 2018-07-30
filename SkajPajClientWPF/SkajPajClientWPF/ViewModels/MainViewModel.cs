﻿using SkajPajClientWPF.Models;
using SkajPajClientWPF.Objects;
using SkajPajClientWPF.Views;
using System;
using System.Windows;
using System.Windows.Input;
using SkajPajClientWPF.Audio;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;

namespace SkajPajClientWPF.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public AudioManager AudioManager = new AudioManager();

        //detect call
        private BackgroundWorker m_oBackgroundWorker = null;
        private CallDetectWindow cdw;
        private void StartBackgroundWorker()
        {
            cdw = new CallDetectWindow();
            cdw.Show();

            if (null == m_oBackgroundWorker)
            {
                m_oBackgroundWorker = new BackgroundWorker();
                m_oBackgroundWorker.DoWork +=
                    new DoWorkEventHandler(m_oBackgroundWorker_DoWork);
                m_oBackgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(
                    m_oBackgroundWorker_RunWorkerCompleted);
                m_oBackgroundWorker.ProgressChanged +=
                    new ProgressChangedEventHandler(m_oBackgroundWorker_ProgressChanged);
                m_oBackgroundWorker.WorkerReportsProgress = true;
                m_oBackgroundWorker.WorkerSupportsCancellation = true;
            }
            m_oBackgroundWorker.RunWorkerAsync();
        }
        private void StopBackgroundWorker()
        {
            if ((null != m_oBackgroundWorker) && m_oBackgroundWorker.IsBusy)
            {
                m_oBackgroundWorker.CancelAsync();
            }
        }
        private void AppendLog(string sText)
        {
            cdw.StateTextBlock.Text = sText;
            cdw.cdvm.Logs.Add(new Log("testxd", "ble ble ble ..."));
        }
        void m_oBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true) { 
                if (m_oBackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                m_oBackgroundWorker.ReportProgress(1);
                //GET DETECT CALL ME
                

                Thread.Sleep(1000);

                m_oBackgroundWorker.ReportProgress(2);

                Thread.Sleep(1000);
            }
        }
        void m_oBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (true)
            {
                if(e.ProgressPercentage == 1)
                {
                    AppendLog("SDF");
                }
                else if(e.ProgressPercentage == 2)
                {
                    AppendLog("DUPA");
                }
            }
        }
        void m_oBackgroundWorker_RunWorkerCompleted(object sender,
    RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                AppendLog("Przerwano.");
            }
            else
            {
                AppendLog("Zakończono.");
            }
            cdw.Close();
        }
        //end detect call

        public MainViewModel(string login, string password)
        {
            MainModel = new MainModel();
            AudioManager = new AudioManager();
            AudioManager.Initialize(login);
            AudioManager.ListenForMessage(login, password);
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

            StartBackgroundWorker();
        }

        private void UpdateFriendList()
        {
            MainModel.FriendList = MainModel.RestWebApiRequest.ListOfFriends(MainModel.UserData.Login, MainModel.UserData.Password);
            foreach (Friend f in MainModel.FriendList)
            {
                f.ClickDelete += new EventHandler<FriendEventArgs>(DeleteFriend);
                f.ClickCallToFriend += new EventHandler<FriendEventArgs>(CallToFriend);
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

        private void CallToFriend(object sender, FriendEventArgs f)
        {
            CallToLogin(f.Login);
        }

        private void CallToLogin(string login)
        {
            StopBackgroundWorker();
            ReadFriendDataRequest tmp = MainModel.RestWebApiRequest.ReadFriendData(MainModel.UserData.Login, MainModel.UserData.Password, login);
            AudioManager.BeginCall("192.168.43.227");
            if (tmp.read_data)
            {
                string avatar = tmp.avatar;
                string address_ip = "192.168.43.227";
                string call_id = MainModel.RestWebApiRequest.CreateCall(MainModel.UserData.Login, MainModel.UserData.Password, login);

                if (call_id != "0")
                {
                    CallWindow loginnWindow = new CallWindow(MainModel.UserData.Login, MainModel.UserData.Password, avatar, login, address_ip, call_id, "create");
                    loginnWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Nieudało się wywołać połączenia.");
                }
            }
            else
            {
                MessageBox.Show("Użytkownik o podanym loginie prawdopodobnie nie istnieje.");
            }

            UpdateCallList();
            StartBackgroundWorker();
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
            StopBackgroundWorker();
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

        public ICommand CallCommand { get { return new RelayCommand(CallButton); } }

        private void CallButton()
        {
            if (MainModel.UserData.Login != MainModel.CallLogin)
            {
                CallToLogin(MainModel.CallLogin);
            }
        }

        public ICommand ChangePasswordCommand { get { return new RelayCommand(ChangePassword); } }

        private void ChangePassword()
        {
            ChangePasswordWindow loginnWindow = new ChangePasswordWindow(MainModel.UserData.Login);
            loginnWindow.ShowDialog();
            LogOut();
        }
    }
}