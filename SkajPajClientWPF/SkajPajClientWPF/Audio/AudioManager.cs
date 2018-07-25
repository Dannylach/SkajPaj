using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace SkajPajClientWPF.Audio
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

        public string callersIpAddress = null;

        /// <summary>
        /// Initializes all vital instances.
        /// </summary>
        /// <param name="userLogin">The user login.</param>
        public void Initialize(string userLogin)
        {
            try
            {
                clientName = userLogin;
                udpClient = new UdpClient(port);

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
                //TODO Signal error
            }
        }


        /// <summary>
        /// Initializes UDP listening async task
        /// </summary>
        public void ListenForMessage()
        {
            try
            {
                UDPListener();
            }
            catch (Exception e)
            {
                //TODO Signal error
            }
        }

        /// <summary>
        /// Begins call to specified IP address
        /// </summary>
        /// <param name="ip">The ip.</param>
        public void StartCall(string ip)
        {

            ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), 40015);
            udpClient.Send(new byte[1], 1, ipEndPoint);
            recieve_thread = new Thread(AudioReceive);
            recieve_thread.Start();

            waveIn.StartRecording();
        }

        /// <summary>
        /// Handles the DataAvailable event of the waveIn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WaveInEventArgs"/> instance containing the event data.</param>
        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            udpClient.Send(e.Buffer, e.Buffer.Length, ipEndPoint);
        }

        /// <summary>
        /// Handles the RecordingStopped event of the waveIn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            waveIn.Dispose();
            waveIn = null;
        }

        /// <summary>
        /// Exits this instance.
        /// </summary>
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

        /// <summary>
        /// Function responsible for listening for audio during call
        /// </summary>
        static void AudioReceive()
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

        /// <summary>
        /// Begins the call by sending HELLO message with callers IP address.
        /// </summary>
        /// <param name="ipToCall">The ip to call.</param>
        public void BeginCall(string ipToCall)
        {
            try
            {
                running = true;
                var message = "HELLO " + GetLocalIPAddress();
                var messageToByte = Encoding.ASCII.GetBytes(message);
                var serverIp = IPAddress.Parse(ipToCall);
                var dataPacket = new DataPacket(messageToByte, clientName, 0);

                ipEndPoint = new IPEndPoint(serverIp, port);
                friendEndPoint = (EndPoint)ipEndPoint;

                SendMessage(dataPacket);
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// Function responsible for sending text messages to called user.
        /// </summary>
        /// <param name="dataPacket">The data packet.</param>
        public void SendMessage(DataPacket dataPacket)
        {
            try
            {
                var dataToSend = dataPacket.PackMessage();

                udpClient.BeginSend(dataToSend, dataToSend.Length, ipEndPoint, null, null);
            }
            catch (Exception ex)
            {
                //TODO Signal error
            }
        }
        
        /// <summary>
        /// Creates async task listening for any UDP traffic at specified port
        /// </summary>
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

        /// <summary>
        /// Processes message sent from network.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private string ReceivedMessage(byte[] message)
        {
            DataPacket dataPacket = new DataPacket();
            dataPacket = dataPacket.UnpackMessage(message);
            var receivedString = Encoding.ASCII.GetString(dataPacket.Message);

            if (receivedString.Contains("HELLO"))
                AnswerHello(HelloResponse(receivedString));
            else if (receivedString.Contains("BYE"))
            {
                Exit();
            }

            return null;
        }

        /// <summary>
        /// Gets callers IP address from HELLO message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        string HelloResponse(string message)
        {
            return message.Remove(0, 6);
        }

        /// <summary>
        /// Answers the HELLO message.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        void AnswerHello(string ipAddress)
        {
            callersIpAddress = ipAddress;
            StartCall(ipAddress);
        }

        /// <summary>
        /// Gets the ip address of the machine in local network.
        /// </summary>
        /// <returns></returns>
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