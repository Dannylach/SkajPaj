using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using NAudio.Wave;
using SkajPaj;

namespace SkajPajClientWPF.Audio
{
    public class CallingManager
    {
        //TODO Sending and receiving data out of local network
        private Socket client;
        private string clientName;
        private EndPoint serverEndPoint;

        public Socket listeningSocket = null;
        public Socket sendingSocket = null;
        public const int BufferSize = 256;
        public byte[] buffer = new byte[BufferSize];
        private byte[] dataStream = new byte[1024];
        private const int port = 11000;
        private DataPacket dataPacket;

        public void BeginCall(IPAddress ipAddress, string username)
        {
            clientName = username;
            Connect(ipAddress);
            var audioManager = new AudioManager();
            audioManager.StartRecording("C:/Projekty/Studia/Audio", client);
        }

        public void Connect(IPAddress ipAddress)
        {
            try
            {
                dataPacket.SenderName = clientName;
                dataPacket.Message = null;
                dataPacket.ChatDataIdentifier = DataIdentifier.LogIn;

                client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //TODO Change IP to send not on local machine
                var server = new IPEndPoint(ipAddress, port);

                serverEndPoint = server;
                var data = dataPacket.PackMessage();

                client.BeginSendTo(data, 0, data.Length, SocketFlags.None, serverEndPoint, SendData, null);
                dataStream = new byte[1024];
                client.BeginReceiveFrom(dataStream, 0, dataStream.Length, SocketFlags.None, ref serverEndPoint, ReceiveData, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void EndCall()
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        
        private void SendData(IAsyncResult ar)
        {
            try
            {
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send Data: " + ex.Message, "UDP Client");
            }
        }

        private void ReceiveData(IAsyncResult ar)
        {
            try
            {
                client.EndReceive(ar);
                DataPacket receivedData = new DataPacket(this.dataStream);

                if (receivedData.Message != null)
                    SaveMessage(receivedData.Message);

                dataStream = new byte[1024];

                client.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None,
                    ref serverEndPoint, new AsyncCallback(this.ReceiveData), null);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine("Receive Data: " + ex.Message, "UDP Client");
            }
        }
        private void SaveMessage(byte[] messge)
        {
            MemoryStream mp3Buffered = new MemoryStream();
            using (var responseStream = new NetworkStream(client))
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
    }
}
