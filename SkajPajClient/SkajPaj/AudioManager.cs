using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace SkajPaj
{
    public class AudioManager
    {
        private WaveIn sourceStream;
        private WaveFileWriter waveWriter;
        private MemoryStream memoryStream;
        private byte[] buffer;
        private int packetNumber = 0;
        public ConnectionManager connectionManager;

        public void StartRecording(ConnectionManager connectionManager, string userName)
        {
            this.connectionManager = connectionManager;
            sourceStream = new WaveIn();
            var devicenum = 0;
            for (var i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
            {
                if (NAudio.Wave.WaveIn.GetCapabilities(i).ProductName.Contains("icrophone"))
                    devicenum = i;
            }
            sourceStream.DeviceNumber = devicenum;
            sourceStream.WaveFormat = new WaveFormat(22000, WaveIn.GetCapabilities(devicenum).Channels);
            sourceStream.DataAvailable += sourceStream_DataAvailable;
            
            memoryStream = new MemoryStream();
            waveWriter = new WaveFileWriter(memoryStream, sourceStream.WaveFormat);

            var dataPacket = new DataPacket(buffer, userName, packetNumber);

            connectionManager.SendMessage(dataPacket);

            sourceStream.StartRecording();
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public void StopRecording()
        {
            packetNumber = 0;
            sourceStream.StopRecording();
            var data = ReadFully(memoryStream);
            memoryStream.Close();
        }

        public void EndCall()
        {
            StopRecording();
            waveWriter.Close();
        }

        public void PlayMessage(byte[] message)
        {
            WaveOut waveOut = new WaveOut();
            byte[] bytes = new byte[message.Length];

            IWaveProvider provider = new RawSourceWaveStream(
                new MemoryStream(bytes), new WaveFormat());

            waveOut.Init(provider);
            waveOut.Play();
        }


        private void sourceStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveWriter == null) return;
            waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            waveWriter.Flush();
            buffer = e.Buffer;
            packetNumber++;
        }
    }
}
