using SkajPajClientWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SkajPajClientWPF.Audio;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.ComponentModel;
using System.Net.Sockets;

namespace SkajPajClientWPF.Views
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        int time = 0;
        public CallViewModel cvm;
        //private AudioManager audioManager;

        public CallWindow()
        {

        }

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private bool pleaseClose = false;
        /*
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!pleaseClose)
            {cvm.sck.Close();
                base.OnClosing(e);
                e.Cancel = true;
            }
        }*/

        public CallWindow(string login, string password, string friendAvatar, string friendLogin, string address_ip, string call_id, string state, UdpClient udp)
        {
            InitializeComponent();
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            //audioManager = new AudioManager();
            //audioManager.Initialize(login);
            //audioManager.StartCall(address_ip);

            NotWaitToCallButton.IsEnabled = false;
            SendButton.IsEnabled = false;

            NotReceiveButton.IsEnabled = false;
            StartCallButton.IsEnabled = false;

            cvm = new CallViewModel(login, password, friendAvatar, friendLogin, address_ip, call_id, state,this,udp);
            cvm.RequestClose += new EventHandler(CloseWindow);
            DataContext = cvm;

            if (!cvm.CallModel.IsNotInLocalNewtwork)
            {
                cvm.sendHi();

                System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();
            }
            else
            {
                pleaseClose = true;
                Close();
            }
        }

        public void CloseWindow(Object source, EventArgs args)
        {
            //audioManager.Exit();
            pleaseClose = true;
            Close();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            time++;
            Timer.Text = "Czas: " + timeToString();
            if (cvm.CallModel.CallState == "create" && time==7)
            {
                string tmp = cvm.CallModel.FriendLogin + ": %SYSTEM%PLEASEWAIT";
                if (cvm.CallModel.Chat[0] == tmp)
                {
                    SendButton.IsEnabled = true;
                    NotWaitToCallButton.IsEnabled = true;
                    cvm.sendMessage("%SYSTEM%YESIWAIT");
                    cvm.CallModel.Chat.Clear();
                }
                else
                {
                    MessageBox.Show("Abonent czasowo niedostępny");
                    pleaseClose = true;
                    Close();
                }
            }
        }

        private string timeToString()
        {
            int secunds = time % 60;
            int minutes = time / 60;
            if(minutes < 1)
            {
                return secunds.ToString() + " sekund";
            }
            return minutes.ToString() + " minut " + secunds.ToString() + " sekund";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cvm.sck.Close();
        }

        public void scrollToButtom()
        {
            Chat.SelectedIndex = 0;
            Chat.ScrollIntoView(Chat.SelectedIndex);
        }

        public void ActiveButton()
        {
            StartCallButton.IsEnabled = true;
            NotReceiveButton.IsEnabled = true;
            SendButton.IsEnabled = true;
        }
    }
}
