#region API 참조
using System.Collections.Generic;
#endregion

namespace Models
{
    public enum PacketType
    {
        Temp = 0
    }

    public class Packet
    {
        #region 변수
        public Dictionary<string, object> Data { get; set; } // 데이터

        public string Title { get; } // 패킷 제목

        public PacketType Type { get; } // 패킷 타입
        #endregion

        public Packet() { }

        #region 생성자 - PacketType
        public Packet(PacketType type, string title)
        {
            Data = new Dictionary<string, object>();
            Title = title;
            Type = type;
        }
        #endregion
    }
}