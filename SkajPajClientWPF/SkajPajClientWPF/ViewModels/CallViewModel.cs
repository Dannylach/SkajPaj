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

        public CallViewModel()
        {
            
        }

        public CallViewModel(string login, string password, string friendAvatar, string friendLogin, string address_ip, string call_id, string state)
        {
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
                MessageBox.Show(exp.ToString());
            }
            
            System.Windows.Application.Current.Dispatcher.Invoke(
                       (Action)(() => { dispatcherTimer_TickXD(); }));
        }

        private void dispatcherTimer_TickXD()
        {
            if (CallModel.CallState == "create")
            {
                sendMessage("%" + CallModel.Login + " call to you MY DATA" + MY_ADDRESS_IP + " " + MY_PORT);
                sendMessage("%" + CallModel.Login + " call to you YOUR DATA" + CallModel.FriendAddressIP + " " + FRIEND_PORT);
            }
            else
            {
                sendMessage("%" + CallModel.Login + " hi to you MY DATA" + MY_ADDRESS_IP + " " + MY_PORT);
                sendMessage("%" + CallModel.Login + " hi to you YOUR DATA" + CallModel.FriendAddressIP + " " + FRIEND_PORT);
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
                       (Action)(() => { CallModel.Chat.Add(CallModel.FriendLogin + ": " + receivedMessage); }));
                }

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private void sendMessage(string buf)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(buf);
                
                System.Windows.Application.Current.Dispatcher.Invoke(
                       (Action)(() => { CallModel.Chat.Add("Ja: " + buf); }));
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
            MessageBox.Show("XZD");
        }
    }
}
