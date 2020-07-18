#region API 참조
using Hanyang.Interface;
using Microsoft.AspNetCore.SignalR.Client;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using TcpData;
using Xamarin.Forms;
#endregion

namespace Hanyang
{
    public class Hub
    {
        #region 변수
        private HubConnection connection;
        #endregion

        #region 생성자
        public Hub()
        {
            try
            {
                connection = new HubConnectionBuilder().WithUrl(App.ServerUrl).Build();

                var sendPacket = new Packet(PacketType.GetData, "데이터 요청");

                connection.On<Packet>("ReceivePacket", (packet) =>
                {
                    Debug.WriteLine("@HUB@" + packet.Data["TEST"]);
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
        public async void Start()
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
                    await MainPage.GetInstance().DisplayAlert("서버 연결 오류", e.Message, "확인");
                }
            }
        }
        #endregion

        #region 서버 연결 해제
        public async void Stop()
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
                    await MainPage.GetInstance().DisplayAlert("서버 연결 해제 오류", e.Message, "확인");
                }
            }
        }
        #endregion

        #region 서버로부터 요청
        public async void Request(Packet packet)
        {
            try
            {
                if (connection.State == HubConnectionState.Connected)
                    await connection.SendAsync("MainFromServer", packet);
                else
                    DependencyService.Get<IToastMessage>().Longtime("서버에 연결되어 있지 않습니다.");
            }
            catch (Exception e)
            {
                await MainPage.GetInstance().DisplayAlert("서버 요청 오류", e.Message, "확인");
            }
        }
        #endregion
    }
}