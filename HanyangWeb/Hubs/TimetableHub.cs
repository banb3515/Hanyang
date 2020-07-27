using Microsoft.AspNetCore.SignalR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HanyangWeb.Hubs
{
    public class TimetableHub : Hub
    {
        public async Task TimetableFromServer(string[] args)
        {
            Console.WriteLine("시간표 요청: " + args[0]);
            await Clients.Caller.SendAsync("ReceiveTimetable", "Hello, Client!");
        }
    }
}
