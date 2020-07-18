#region API 참조
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

using TcpData;
#endregion

namespace SignalR.Hubs
{
    public class MainHub : Hub
    {
        public async Task MainFromServer(Packet packet)
        {
            Console.WriteLine("[" + packet.IPAddress + "]: " + packet.Type.ToString() + "/" + packet.Title);

            var serverPacket = new Packet(PacketType.Registration, "테스트");
            serverPacket.Data.Add("TEST", "TestContent");

            await Clients.Caller.SendAsync("ReceivePacket", serverPacket);
        }
    }
}
