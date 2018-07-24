using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF.Audio
{
    public class DataPacket
    {
        public int PacketNumber { get; set; }
        public string SenderName { get; set; }
        public byte[] Message { get; set; }

        public DataPacket()
        {
            PacketNumber = 0;
            Message = null;
            SenderName = null;
        }

        public DataPacket(byte[] dataStream, string userLogin, int packetNumber)
        {
            PacketNumber = PacketNumber;
            SenderName = userLogin;
            Message = dataStream;
        }

        public byte[] PackMessage()
        {

            var memoryStream = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(DataPacket));

            ser.WriteObject(memoryStream, this);
            var dataToSend = memoryStream.ToArray();
            memoryStream.Close();
            return dataToSend;
        }

        public DataPacket UnpackMessage(byte[] message)
        {
            MemoryStream memoryStream = new MemoryStream(message);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(this.GetType());
            var dataUnpaked = ser.ReadObject(memoryStream) as DataPacket;
            memoryStream.Close();
            return dataUnpaked;
        }

        public override string ToString()
        {
            string stringToReturn = null;
            stringToReturn += SenderName + "\n";
            stringToReturn += Encoding.ASCII.GetString(Message);
            return stringToReturn;
        }
    }
}
