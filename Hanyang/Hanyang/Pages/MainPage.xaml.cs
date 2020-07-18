#region API 참조
using Hanyang.Controller;
using Hanyang.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using TcpData;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
#endregion

namespace Hanyang
{
    [DesignTimeVisible(false)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        #region 변수
        public static MainPage ins; // Instance
        #endregion

        #region 생성자
        public MainPage()
        {
            #region 변수 초기화
            ins = this;
            #endregion

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            InitializeComponent();

            ServerConnect();
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }
        #endregion

        #region 함수
        #region 서버 접속
        private async void ServerConnect()
        {
            if(Connectivity.NetworkAccess != NetworkAccess.Internet)
                await DisplayAlert("인터넷에 연결되지 않음", 
                    "※ WiFi/LTE/5G 상태를 확인해주세요.\n\n" + 
                    "로컬 저장소에 있는 정보를 가져옵니다.\n\n" + 
                    "! 인터넷에 연결되면 자동으로 최신 정보를 가져옵니다.", "확인");

            var serverThread = new Thread(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    do
                    {
                        Debug.WriteLine("서버 연결 시도");
                        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                        {
                            try
                            {
                                App.Hub = new Hub();
                                App.Hub.Start();
                                break;
                            }
                            catch (Exception e)
                            {
                                await DisplayAlert("서버 연결 오류", e.Message, "확인");
                            }
                        }

                        await Task.Delay(1000); // 1초마다 서버와 연결 시도
                    } while (true);
                });
            });
            serverThread.Start();
        }
        #endregion

        #region 인스턴스 가져오기
        public static MainPage GetInstance()
        {
            return ins;
        }
        #endregion
        #endregion

        #region 이벤트
        #region 인터넷 상태 변경
        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess != NetworkAccess.Internet)
                ServerConnect();
        }
        #endregion
        #endregion
    }
}
