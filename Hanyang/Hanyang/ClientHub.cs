#region API 참조
using Hanyang.Interface;
using Microsoft.AspNetCore.SignalR.Client;

using Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
#endregion

namespace Hanyang
{
    public class ClientHub
    {
        #region 변수
        private HubConnection connection;
        #endregion

        #region 생성자
        public ClientHub(string hub)
        {
            try
            {
                connection = new HubConnectionBuilder()
                    .WithUrl($"{App.ServerUrl}/hubs/{hub}")
                    .Build();

                connection.On<string>("ReceiveData", (test) =>
                {
                    DependencyService.Get<IToastMessage>().Longtime("Hub: " + test);
                });

                Debug.WriteLine("@HUB@: Hub 생성 완료");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region 서버 연결
        public async Task Start()
        {
            if (connection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    Debug.WriteLine("@HUB@: 연결 대기");
                    await connection.StartAsync();

                    DependencyService.Get<IToastMessage>().Longtime("서버와 연결되었습니다.");
                }
                catch (Exception e)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await MainPage.GetInstance().DisplayAlert("서버 연결 오류", e.Message, "확인");
                    });
                }
            }
        }
        #endregion

        #region 서버 연결 해제
        public async Task Stop()
        {
            if (connection.State == HubConnectionState.Connected)
            {
                try
                {
                    await connection.StopAsync();

                    DependencyService.Get<IToastMessage>().Longtime("서버와의 연결이 끊어졌습니다.");
                }
                catch (Exception e)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await MainPage.GetInstance().DisplayAlert("서버 연결 해제 오류", e.Message, "확인");
                    });
                }
            }
        }
        #endregion

        #region 서버로부터 요청
        public async Task Request(string packet)
        {
            try
            {
                if (connection.State == HubConnectionState.Connected)
                    await connection.InvokeAsync("DataManager", "Hello, Server!");
                else
                    DependencyService.Get<IToastMessage>().Longtime("서버에 연결되어 있지 않습니다.");
            }
            catch (Exception e)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await MainPage.GetInstance().DisplayAlert("서버 요청 오류", e.Message, "확인");
                });
            }
        }
        #endregion
    }
}