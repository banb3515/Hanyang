using Microsoft.AspNetCore.SignalR.Client;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hanyang
{
    public class SignalR
    {
        HubConnection hubConnection;
        string type;

        public SignalR(string type)
        {
            type = type.ToLower();

            hubConnection = new HubConnectionBuilder().WithUrl(App.ServerUrl + "hub/" + type).Build();
            this.type = type;

            switch(type)
            {
                case "timetable":
                    hubConnection.On<string>("ReceiveTimetable", (text) =>
                    {
                        MainPage.GetInstance().DisplayAlert("TEST", "TEXT: " + text, "확인");
                    });
                    break;
            }
        }

        public async void Start()
        {
            if(hubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await hubConnection.StartAsync();
                }
                catch (Exception)
                {

                }
            }
        }

        public async void Stop()
        {
            if(hubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    await hubConnection.StopAsync();
                }
                catch (Exception)
                {

                }
            }
        }

        public async void Send(string serverMethod, params string[] args)
        {
            if(hubConnection.State == HubConnectionState.Connected)
            {
                await hubConnection.SendAsync(serverMethod, args);
            }
        }
    }
}
