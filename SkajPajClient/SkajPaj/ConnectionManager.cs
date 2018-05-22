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
        private string clientName;
        private EndPoint serverEndPoint;
        private byte[] dataStream = new byte[1024];

        public void Connect(DataPacket dataPacket)
        {
            try
            {
                //TODO Change naming
                clientName = "Testowy";

                dataPacket.SenderName = clientName;
                dataPacket.Message = null;
                dataPacket.ChatDataIdentifier = DataIdentifier.LogIn;

                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //TODO Change IP to send not on local machine
                var serverIp = IPAddress.Parse("127.0.0.1");
                var server = new IPEndPoint(serverIp, 30000);

                serverEndPoint = (EndPoint)server;
                var data = dataPacket.PackMessage();

                clientSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, serverEndPoint, SendData, null);
                dataStream = new byte[1024];
                clientSocket.BeginReceiveFrom(dataStream, 0, dataStream.Length, SocketFlags.None, ref serverEndPoint, ReceiveData, null);
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

                byte[] byteData = dataPacket.PackMessage();

                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, serverEndPoint, new AsyncCallback(this.SendData), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendData(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Data: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
