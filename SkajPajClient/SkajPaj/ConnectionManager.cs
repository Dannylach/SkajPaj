using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        private string clientName;
        private EndPoint serverEndPoint;
        private IPEndPoint serverIpEndPoint;
        private byte[] dataStream = new byte[1024];

        public void Connect(DataPacket dataPacket)
        {
            try
            {
                //TODO Change naming for login
                clientName = "Testowy";

                dataPacket.SenderName = clientName;
                dataPacket.Message = null;
                dataPacket.ChatDataIdentifier = DataIdentifier.LogIn;

                //TODO Change IP to send to ip from serwer
                var serverIp = IPAddress.Parse("192.168.1.33");
                serverIpEndPoint = new IPEndPoint(serverIp, 3000);
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
                var data = new byte[4];
                data[0] = 1;
                data[2] = 0;
                data[3] = 1;

                Connect(dataPacket);
                
                udpClient.BeginSend(data, data.Length, serverIpEndPoint, null, null);
                dataStream = new byte[1024];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

        //TODO Receive by UpdClient
        private void ReceiveData(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndReceive(ar);
                DataPacket receivedData = new DataPacket(this.dataStream);

                if (receivedData.Message != null)
                    SaveMessage(receivedData.Message);
                
                dataStream = new byte[1024];

                clientSocket.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None,
                    ref serverEndPoint, new AsyncCallback(this.ReceiveData), null);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Receive Data: " + ex.Message, "UDP Client", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
                        ChatDataIdentifier = DataIdentifier.LogOut,
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
    }
}
