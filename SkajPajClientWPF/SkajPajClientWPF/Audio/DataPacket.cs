using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkajPaj
{
    public enum DataIdentifier
    {
        Message,
        LogIn,
        LogOut,
        Null
    }

    public class DataPacket
    {
        public DataIdentifier ChatDataIdentifier { get; set; }
        public string SenderName { get; set; }
        public byte[] Message { get; set; }

        public DataPacket()
        {
            ChatDataIdentifier = DataIdentifier.Null;
            Message = null;
            SenderName = null;
        }

        public DataPacket(byte[] dataStream)
        {
            ChatDataIdentifier = (DataIdentifier)BitConverter.ToInt32(dataStream, 0);

            var nameLength = BitConverter.ToInt32(dataStream, 4);
            var msgLength = BitConverter.ToInt32(dataStream, 8);
            SenderName = nameLength > 0 ? Encoding.UTF8.GetString(dataStream, 12, nameLength) : null;
            Message = msgLength > 0 ? Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(dataStream, 12 + nameLength, msgLength)) : null;
        }

        public byte[] PackMessage()
        {
            var dataStream = new List<byte[]>();

            dataStream.Add(BitConverter.GetBytes((int)ChatDataIdentifier));
            dataStream.Add(SenderName != null ? BitConverter.GetBytes(SenderName.Length) : BitConverter.GetBytes(0));
            dataStream.Add(Message != null ? BitConverter.GetBytes(Message.Length) : BitConverter.GetBytes(0));
            if (SenderName != null) dataStream.Add(Encoding.UTF8.GetBytes(SenderName));
            if (Message != null) dataStream.Add(Message);

            return dataStream.SelectMany(a => a).ToArray();
        }
    }
}
