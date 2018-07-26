using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace SkajPaj
{
    public class AudioManager
    {
        static WaveIn waveIn;
        static WaveOut waveOut;
        static WaveFileWriter waveWriter;
        static Thread recieve_thread;
        static UdpClient udpClient;
        static IPEndPoint ipEndPoint;
        static byte[] incoming;
        static int port = 3000;

        private MemoryStream memoryStream;
        private int packetNumber = 0;
        private bool running = true;
        private string clientName;
        private EndPoint friendEndPoint;
        private IPEndPoint friendIpEndPoint;


        public void Initialize(string userLogin)
        {
            try
            {
                clientName = userLogin;
                friendIpEndPoint = new IPEndPoint(IPAddress.Parse("192.168.43.24"), port);
                udpClient = new UdpClient(port);
                friendEndPoint = (EndPoint)friendIpEndPoint;

                waveIn = new WaveIn();
                waveIn.BufferMilliseconds = 100;
                waveIn.NumberOfBuffers = 10;
                waveOut = new WaveOut();

                waveIn.DeviceNumber = 0;
                waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(waveIn_DataAvailable);

                waveIn.WaveFormat = new WaveFormat(44200, 2);
                waveIn.RecordingStopped += new EventHandler<StoppedEventArgs>(waveIn_RecordingStopped);

                memoryStream = new MemoryStream();
                waveWriter = new WaveFileWriter(memoryStream, waveIn.WaveFormat);

                udpClient = new UdpClient(40015);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void StartCall(string ip)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), 40015);
            udpClient = new UdpClient(40015);
            udpClient.Send(new byte[1], 1, ipEndPoint);
            recieve_thread = new Thread(recv);
            recieve_thread.Start();

            waveIn.StartRecording();
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            udpClient.Send(e.Buffer, e.Buffer.Length, ipEndPoint);
        }

        void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            waveIn.Dispose();
            waveIn = null;
        }

        public void Exit()
        {
            if (waveIn == null) return;
            recieve_thread.Abort();
            waveIn.StopRecording();
            waveOut.Dispose();
            waveOut = null;
            
            udpClient = null;
            running = false;
        }

        static void recv()
        {
            BufferedWaveProvider PlayBuffer = new BufferedWaveProvider(waveIn.WaveFormat);
            waveOut.Init(PlayBuffer);
            waveOut.Play();

            while (true)
            {
                incoming = udpClient.Receive(ref ipEndPoint);
                PlayBuffer.AddSamples(incoming, 0, incoming.Length);
            }
        }

        public void BeginCall(string ipToCall)
        {
            try
            {
                running = true;
                var message = "HELLO " + GetLocalIPAddress();
                var messageToByte = Encoding.ASCII.GetBytes(message);
                var serverIp = IPAddress.Parse(ipToCall);
                var dataPacket = new DataPacket(messageToByte, clientName, 0);

                friendIpEndPoint = new IPEndPoint(serverIp, port);
                friendEndPoint = (EndPoint) friendIpEndPoint;

                SendMessage(dataPacket);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public void SendMessage(DataPacket dataPacket)
        {
            try
            {
                var dataToSend = dataPacket.Message;

                udpClient.BeginSend(dataToSend, dataToSend.Length, friendIpEndPoint, null, null);
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
                    while (running)
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
                Exit();
            }

            return null;
        }

        string HelloResponse(string message)
        {
            return message.Remove(0, 6);
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