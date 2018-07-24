using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkajPaj
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
            var dataStream = new List<byte[]>();
            dataStream.Add(SenderName != null ? BitConverter.GetBytes(SenderName.Length) : BitConverter.GetBytes(0));
            dataStream.Add(Message != null ? BitConverter.GetBytes(Message.Length) : BitConverter.GetBytes(0));
            if (SenderName != null) dataStream.Add(Encoding.UTF8.GetBytes(SenderName));
            if (Message != null) dataStream.Add(Message);

            return dataStream.SelectMany(a => a).ToArray();
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
