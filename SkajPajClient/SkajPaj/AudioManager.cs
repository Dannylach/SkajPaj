using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace SkajPaj
{
    public class AudioManager
    {
        static WaveIn waveIn;
        static WaveOut waveOut;
        static WaveFileWriter waveWriter;
        static Thread recieve_thread;
        static UdpClient udpc;
        static IPEndPoint ipEndPoint;
        private MemoryStream memoryStream;
        static byte[] incoming;
        private int packetNumber = 0;

        public void StartCall(string ip)
        {
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

            udpc = new UdpClient(40015);
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), 40015);

            udpc.Send(new byte[1], 1, ipEndPoint);
            recieve_thread = new Thread(recv);
            recieve_thread.Start();

            waveIn.StartRecording();
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            udpc.Send(e.Buffer, e.Buffer.Length, ipEndPoint);
        }

        void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            waveIn.Dispose();
            waveIn = null;
        }

        void Exit()
        {
            if (waveIn == null) return;
            recieve_thread.Abort();
            waveIn.StopRecording();
            waveOut.Dispose();
            waveOut = null;
            udpc = null;
        }

        static void recv()
        {
            BufferedWaveProvider PlayBuffer = new BufferedWaveProvider(waveIn.WaveFormat);
            waveOut.Init(PlayBuffer);
            waveOut.Play();

            while (true)
            {
                incoming = udpc.Receive(ref ipEndPoint);
                PlayBuffer.AddSamples(incoming, 0, incoming.Length);
            }
        }
    }
}
