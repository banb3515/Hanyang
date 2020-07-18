#region API 참조
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
#endregion

namespace TcpData
{
    public class Packet
    {
        public PacketXML xml;

        #region 생성자 - PacketType
        public Packet(PacketType type)
        {
            xml = new PacketXML();
            xml.Data = new Dictionary<string, object>();
            xml.Type = type;
        }
        #endregion

        #region 생성자 - PacketBytes
        public Packet(byte[] packetBytes)
        {
            XmlSerializer xmlS = new XmlSerializer(typeof(PacketXML));
            MemoryStream ms = new MemoryStream(packetBytes);
            XmlTextWriter xmlTW = new XmlTextWriter(ms, Encoding.UTF8);
            xml = (PacketXML)xmlS.Deserialize(ms);
        }
        #endregion

        #region Packet To Bytes
        public byte[] ToBytes()
        {
            MemoryStream ms = new MemoryStream();
            XmlSerializer xmlS = new XmlSerializer(typeof(PacketXML));
            XmlTextWriter xmlTW = new XmlTextWriter(ms, Encoding.UTF8);

            xmlS.Serialize(xmlTW, xml);
            ms = (MemoryStream)xmlTW.BaseStream;

            return ms.ToArray();
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
        Registration,
        GetData
    }
    #endregion
}
