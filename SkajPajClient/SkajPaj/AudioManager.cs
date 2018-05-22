using System;
using System.Collections.Generic;
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
        private WaveFileWriter waveWriter2;
        //private DataPacket dataPacket;

        public void StartRecording(string path, DataPacket packet)
        {
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
            //TODO Send to stream
            //waveWriter = new WaveFileWriter(dataPacket.Message, sourceStream.WaveFormat);

            waveWriter = new WaveFileWriter(path, sourceStream.WaveFormat);
            //waveWriter2 = new

            sourceStream.StartRecording();
        }

        public void StopRecording()
        {
            sourceStream.StopRecording();
        }

        private void sourceStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveWriter == null) return;
            waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            waveWriter.Flush();
        }
    }
}
