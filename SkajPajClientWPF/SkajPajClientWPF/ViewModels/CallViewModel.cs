using SkajPajClientWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;
using SkajPajClientWPF.Views;
using SkajPajClientWPF.Audio;

namespace SkajPajClientWPF.ViewModels
{
    public class CallViewModel : ObservableObject
    {
        private CallModel _callModel;
        public CallModel CallModel
        {
            get { return _callModel; }
            set { _callModel = value; }
        }

        public Socket sck;
        private IPEndPoint epLocal;
        private EndPoint epRemote;

        private const string SENDER_PORT = "2049";
        private const string RECEIVED_PORT = "2050";

        private string MY_PORT = string.Empty;
        private string MY_ADDRESS_IP = string.Empty;
        private string FRIEND_PORT = string.Empty;

        public CallWindow callWindow;
        
        public CallViewModel()
        {
            
        }

        public AudioManager AudioManager = new AudioManager();
        private UdpClient udpClient;
        public CallViewModel(string login, string password, string friendAvatar, string friendLogin, string address_ip, string call_id, string state, CallWindow window, UdpClient udp)
        {
            callWindow = window;
            udpClient = udp;
            CallModel = new CallModel(login, password, friendAvatar, friendLogin, address_ip, call_id, state);

            CallModel.Message = "message";
            CallModel.Chat.Add("test");
            CallModel.Chat.Add("test2");
            CallModel.Chat.Add("test3");

            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            if (state == "create")
            {
                MY_PORT = SENDER_PORT;
                FRIEND_PORT = RECEIVED_PORT;
            }
            else
            {
                MY_PORT = RECEIVED_PORT;
                FRIEND_PORT = SENDER_PORT;
            }

            MY_ADDRESS_IP = new IPHelpfulFunktions().GetLocalIPAddress();

            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(MY_ADDRESS_IP), Convert.ToInt32(MY_PORT));
                sck.Bind(epLocal);

                epRemote = new IPEndPoint(IPAddress.Parse(CallModel.FriendAddressIP), Convert.ToInt32(FRIEND_PORT));
                sck.Connect(epRemote);

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
                
            }
            catch (Exception exp)
            {
                //MessageBox.Show(exp.ToString());
                MessageBox.Show(friendLogin + " prawdopodobnie nie jest w sieci lokalnej i połączenie z niem nie jest możliwe.");
                CallModel.IsNotInLocalNewtwork = true;
            }
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);

                if (size > 0)
                {
                    byte[] receivedData = new byte[1464];

                    receivedData = (byte[])aResult.AsyncState;

                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string receivedMessage = eEncoding.GetString(receivedData);
                    System.Windows.Application.Current.Dispatcher.Invoke(
                       (Action)(() => {
                           CallModel.Chat.Insert(0,CallModel.FriendLogin + ": " + clearMessage(receivedMessage));
                           callWindow.scrollToButtom();
                       }
                    ));

                    if(clearMessage(receivedMessage) == "%SYSTEM%YESIWAIT" && CallModel.CallState == "select")
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(
                           (Action)(() => {
                               callWindow.ActiveButton();
                           }
                        ));
                    }

                    if (clearMessage(receivedMessage) == "%SYSTEM%INOTRECEIVECALL" && CallModel.CallState == "create")
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(
                           (Action)(() => {
                               MessageBox.Show(CallModel.FriendLogin + " niechce odebrać połączenia.");
                               CloseWindow();
                           }
                        ));
                    }

                    if (clearMessage(receivedMessage) == "%SYSTEM%IRESIGNEDCALL" && CallModel.CallState == "select")
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(
                           (Action)(() => {
                               MessageBox.Show(CallModel.FriendLogin + " zrezygnował z połączenia.");
                               CloseWindow();
                           }
                        ));
                    }

                    if (clearMessage(receivedMessage) == "%SYSTEM%STARTCALL" && CallModel.CallState == "create")
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(
                           (Action)(() => {
                               StartCall();
                           }
                        ));
                    }

                    if (clearMessage(receivedMessage) == "%SYSTEM%IENDCALL" && CallModel.CallState == "call")
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(
                           (Action)(() => {
                               MessageBox.Show(CallModel.FriendLogin + " zakończył połączenie.");
                               endCall();
                               CloseWindow();
                           }
                        ));
                    }
                }

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception exp)
            {
                //MessageBox.Show(exp.ToString());
            }
        }

        private string clearMessage(string buf)
        {
            string result = string.Empty;
            foreach (char sign in buf)
            {
                if (sign != '\0') result += sign;
            }
            return result;
        }

        public void sendMessage(string buf)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(buf);
                
                System.Windows.Application.Current.Dispatcher.Invoke(
                       (Action)(() => {
                           CallModel.Chat.Insert(0,"Ja: " + buf);
                           callWindow.scrollToButtom();
                       }));
                sck.Send(msg);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
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
        
        public ICommand SendMessageCommand { get { return new RelayCommand(SendMessage); } }

        private void SendMessage()
        {
            sendMessage(CallModel.Message);
        }
        public ICommand EndCallCommand { get { return new RelayCommand(EndCall); } }

        private void EndCall()
        {
            sendMessage("%SYSTEM%IENDCALL");
            endCall();
            CloseWindow();
        }

        public void sendHi()
        {
            if (CallModel.CallState == "create")
            {
                Thread.Sleep(3000);
                //sendMessage("%" + CallModel.Login + " call to you MY DATA" + MY_ADDRESS_IP + " " + MY_PORT);
                //sendMessage("%" + CallModel.Login + " call to you YOUR DATA" + CallModel.FriendAddressIP + " " + FRIEND_PORT);
            }
            else if (CallModel.CallState == "select")
            {
                //sendMessage("%" + CallModel.Login + " hi to you MY DATA" + MY_ADDRESS_IP + " " + MY_PORT);
                //sendMessage("%" + CallModel.Login + " hi to you YOUR DATA" + CallModel.FriendAddressIP + " " + FRIEND_PORT);
                sendMessage("%SYSTEM%PLEASEWAIT");
            }
        }
        
        public ICommand StartCallCommand { get { return new RelayCommand(StartCall); } }

        private string pastState = string.Empty;
        private void StartCall()
        {
            sendMessage("%SYSTEM%STARTCALL");
            pastState = CallModel.CallState;
            startCall();
            
        }

        public ICommand NotReceiveCommand { get { return new RelayCommand(NotReceive); } }

        private void NotReceive()
        {
            sendMessage("%SYSTEM%INOTRECEIVECALL");
            CloseWindow();
        }

        public ICommand EndCallNotWaitCommand { get { return new RelayCommand(EndCallNotWait); } }

        private void EndCallNotWait()
        {
            sendMessage("%SYSTEM%IRESIGNEDCALL");
            CloseWindow();
        }
        
        private void startCall()
        {
            CallModel.changeState("call");
            //AUDIO START...
            AudioManager = new AudioManager();
            AudioManager.Initialize(CallModel.FriendLogin, udpClient);
            AudioManager.StartCall(CallModel.FriendAddressIP);
            AudioManager.ListenForMessage("XD", "XD");

            if (pastState == "create")
            {
                CallModel.RestWebApiRequest.ReceivedCall(CallModel.Login, CallModel.Password, CallModel.CallID, "true");
            }
        }
       
        private void endCall()
        {
            AudioManager.Exit();
            if (pastState == "create")
            {
                CallModel.RestWebApiRequest.EndCall(CallModel.Login, CallModel.Password, CallModel.CallID);
            }
        }
    }
}
