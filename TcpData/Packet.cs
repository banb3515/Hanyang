#region API 참조
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

using ProtoBuf;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
#endregion

namespace TcpData
{
    public class Packet
    {
        #region 변수
        public Dictionary<string, object> Data { get; set; } // 데이터
        #endregion

        public Packet() { }

        #region 생성자 - PacketType
        public Packet(PacketType type, string senderID)
        {
            Data = new Dictionary<string, object>();
            Data.Add("Type", type);
            Data.Add("SenderID", senderID);
        }
        #endregion

        #region Bytes to Packet
        public Packet(byte[] bytes)
        {
            var str = Encoding.Unicode.GetString(bytes);
            Data = JsonConvert.DeserializeObject<Dictionary<string, object>>(str);
        }
        #endregion

        #region Packet To Bytes
        public byte[] ToBytes()
        {
            return Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(Data, Formatting.Indented));
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
}