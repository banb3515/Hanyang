using System;
using System.Collections.Generic;
using System.Text;

namespace TcpData
{
    [Serializable]
    public class PacketXML
    {
        public Dictionary<string, object> Data { get; set; }
        public PacketType Type { get; set; }
    }
}
