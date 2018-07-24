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
    class ConnectionManager
    {
        //TODO Sending and receiving data out of local network
        private Socket clientSocket;
        private UdpClient udpClient;
        private EndPoint serverEndPoint;
        private IPEndPoint serverIpEndPoint;
        private string clientName;
        private int port = 3000;
        private bool calling = true;
        private byte[] dataStream = new byte[1024];

        public void Initialize(string userLogin)
        {
            try
            {
                clientName = userLogin;
                serverIpEndPoint = new IPEndPoint(IPAddress.Any, port);
                udpClient = new UdpClient();
                serverEndPoint = (EndPoint)serverIpEndPoint;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveMessage(byte[] messge)
        {
            MemoryStream mp3Buffered = new MemoryStream();
            using (var responseStream = new NetworkStream(clientSocket))
            {
                byte[] buffer = new byte[65536];
                int bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                while (bytesRead > 0)
                {
                    mp3Buffered.Write(buffer, 0, bytesRead);
                    bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                }
            }

            mp3Buffered.Position = 0;
            using (var mp3Stream = new Mp3FileReader(mp3Buffered))
            {
                WaveFileWriter.CreateWaveFile("file.wav", mp3Stream);
            }
        }
        
        public void SendMessage(DataPacket dataPacket)
        {
            try
            {
                dataPacket.SenderName = clientName;
                var message = "HELLO";
                dataPacket.Message = Encoding.ASCII.GetBytes(message);

                MemoryStream memoryStream = new MemoryStream();

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(DataPacket));
                ser.WriteObject(memoryStream, dataPacket);
                byte[] dataToSend = memoryStream.ToArray();
                memoryStream.Close();

                
                udpClient.BeginSend(dataToSend, dataToSend.Length, serverIpEndPoint, null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ListenForMessage()
        {
            //Connect();

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
                using (var udpClient = new UdpClient(port))
                {
                    calling = true;
                    while (calling)
                    {
                        var receivedResults = await udpClient.ReceiveAsync();
                        var receivedByte = receivedResults.Buffer;
                        ReceivedMessage(receivedByte);
                        
                    }
                }
            });
        }

        private void ReceivedMessage(byte[] message)
        {
            DataPacket dataPacket = new DataPacket();
            MemoryStream memoryStream = new MemoryStream(message);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(dataPacket.GetType());
            dataPacket = ser.ReadObject(memoryStream) as DataPacket;
            memoryStream.Close();
            var receivedString = Encoding.ASCII.GetString(dataPacket.Message);
            MessageBox.Show("Receive Data: " + dataPacket.ToString());
            if (receivedString.Contains("HELLO")) HelloResponse(receivedString);
            if (receivedString.Contains("BYE")) calling = false;
            if (receivedString.Contains("BYE")) calling = false;
        }

        void HelloResponse(string message)
        {
            var ipSayingHello = message.Remove(0, 6);
            var serverIp = IPAddress.Parse(ipSayingHello);
            serverIpEndPoint = new IPEndPoint(serverIp, port);
            udpClient = new UdpClient();
            serverEndPoint = (EndPoint)serverIpEndPoint;
        }

        public void BeginCall(string ipToCall)
        {
            try
            {
                Initialize("Daniel");
                var serverIp = IPAddress.Parse(ipToCall);
                serverIpEndPoint = new IPEndPoint(serverIp, port);
                serverEndPoint = (EndPoint)serverIpEndPoint;

                DataPacket dataPacket = new DataPacket();
                dataPacket.SenderName = clientName;
                var message = "HELLO " + GetLocalIPAddress();
                dataPacket.Message = Encoding.ASCII.GetBytes(message);

                MemoryStream memoryStream = new MemoryStream();

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(DataPacket));
                ser.WriteObject(memoryStream, dataPacket);
                byte[] dataToSend = memoryStream.ToArray();
                memoryStream.Close();

                SendMessage(dataPacket);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Exit()
        {
            try
            {
                if (this.clientSocket != null)
                {
                    DataPacket sendData = new DataPacket
                    {
                        SenderName = clientName,
                        Message = null
                    };

                    byte[] byteData = sendData.PackMessage();

                    clientSocket.SendTo(byteData, 0, byteData.Length, SocketFlags.None, serverEndPoint);
                    clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Closing Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static string GetLocalIPAddress()
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
