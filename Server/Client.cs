#region API 참조
using System;
using System.Net.Sockets;
using System.Threading;

using TcpData;
#endregion

namespace Server
{
    class Client
    {
        #region 변수
        public Socket client;
        public Thread thread;
        public string id;
        #endregion

        #region 생성자
        public Client()
        {
            id = Guid.NewGuid().ToString();
            thread = new Thread(Program.server.DataIn);
            thread.Start(client);
            SendRegistrationPacket();
        }
        #endregion

        #region 생성자 - client
        public Client(Socket client)
        {
            this.client = client;
            id = Guid.NewGuid().ToString();
            thread = new Thread(Program.server.DataIn);
            thread.Start(client);
            SendRegistrationPacket();
        }
        #endregion

        #region 패킷 등록
        public void SendRegistrationPacket()
        {
            Packet packet = new Packet(PacketType.Registration, "server");
            packet.Data.Add("ID", id);
            packet.Data.Add("Version", Program.VERSION);
            client.Send(packet.ToBytes());
        }
        #endregion
    }
}