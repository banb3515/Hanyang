#region API 참조
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
#endregion

namespace TcpData
{
    [Serializable]
    public class Packet
    {
        #region 변수
        public Dictionary<string, object> data;
        public string senderID;
        public PacketType packetType;
        #endregion

        #region 생성자 - PacketType
        public Packet(PacketType type, string senderID)
        {
            data = new Dictionary<string, object>();
            this.packetType = type;
            this.senderID = senderID;
        }
        #endregion

        #region 생성자 - PacketBytes
        public Packet(byte[] packetBytes)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(packetBytes);
            formatter.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
            formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            Packet packet = (Packet)formatter.Deserialize(stream);

            stream.Close();

            this.data = packet.data;
            this.senderID = packet.senderID;
            this.packetType = packet.packetType;
        }
        #endregion

        #region Packet To Bytes
        public byte[] ToBytes()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            formatter.Serialize(stream, this);
            byte[] bytes = stream.ToArray();
            stream.Close();

            return bytes;
        }
        #endregion

        #region IP4 주소 가져오기
        public static string GetIP4Address()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ip in ips)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();

            return IPAddress.Loopback.ToString();
        }
        #endregion
    }

    #region PacketType
    public enum PacketType
    {
        Registration, // senderID 할당
        GetData
    }
    #endregion
}
