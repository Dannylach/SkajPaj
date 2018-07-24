using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace SkajPaj
{
    public class ConnectionManager
    {
        //TODO Sending and receiving data out of local network
        private UdpClient udpClient;
        private EndPoint serverEndPoint;
        private IPEndPoint serverIpEndPoint;
        private string clientName;
        private int port = 3000;
        private bool calling = true;
        private byte[] dataStream = new byte[1024];
        private AudioManager audioManager;

        public void Initialize(string userLogin)
        {
            try
            {
                clientName = userLogin;
                serverIpEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.33"), port);
                udpClient = new UdpClient(port);
                serverEndPoint = (EndPoint)serverIpEndPoint;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        public void BeginCall(string ipToCall)
        {
            try
            {
                calling = true;
                var message = "HELLO " + GetLocalIPAddress();
                var messageToByte = Encoding.ASCII.GetBytes(message);
                var serverIp = IPAddress.Parse(ipToCall);
                var dataPacket = new DataPacket(messageToByte, clientName, 0);

                serverIpEndPoint = new IPEndPoint(serverIp, port);
                serverEndPoint = (EndPoint)serverIpEndPoint;

                SendMessage(dataPacket);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SendMessage(DataPacket dataPacket)
        {
            try
            {
                var dataToSend = dataPacket.PackMessage();
                
                udpClient.BeginSend(dataToSend, dataToSend.Length, serverIpEndPoint, null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ListenForMessage()
        {
            try
            {
                UDPListener();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void UDPListener()
        {
            Task.Run(async () =>
            {
                using (udpClient = new UdpClient(port))
                {
                    while (calling)
                    {
                        var receivedResults = await udpClient.ReceiveAsync();
                        var receivedByte = receivedResults.Buffer;
                        ReceivedMessage(receivedByte);
                        
                    }
                }
            });
        }

        private string ReceivedMessage(byte[] message)
        {
            DataPacket dataPacket = new DataPacket();
            dataPacket = dataPacket.UnpackMessage(message);
            var receivedString = Encoding.ASCII.GetString(dataPacket.Message);
            MessageBox.Show("Receive Data: " + dataPacket.ToString());

            if (receivedString.Contains("HELLO"))
                HelloResponse(receivedString);
            else if (receivedString.Contains("BYE"))
            {
                calling = false;
                Exit();
            }

            return null;
        }

        string HelloResponse(string message)
        {
            return message.Remove(0, 6);
        }

        public void Exit()
        {
            try
            {
                if (this.udpClient != null)
                {
                    DataPacket sendData = new DataPacket
                    {
                        SenderName = clientName,
                        Message = null
                    };

                    byte[] byteData = sendData.PackMessage();

                    udpClient.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Closing Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GetLocalIPAddress()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }
    }
}
