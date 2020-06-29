#region API 참조
using Hanyang.Controller;
using Hanyang.Interface;
using Hanyang.Server;

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
#endregion

namespace Hanyang
{
    [DesignTimeVisible(false)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        #region 변수
        #region Response
        public static bool versionCheckResponse; // 버전 확인
        #endregion

        public static MainPage ins;
        #endregion

        #region 생성자
        public MainPage()
        {
            #region 변수 초기화
            versionCheckResponse = false;

            ins = this;
            #endregion

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            InitializeComponent();

            ServerConnection();
        }
        #endregion

        #region 함수
        #region 서버 접속
        private void ServerConnection()
        {
            Thread clientThread = new Thread(async () =>
            {
                try
                {
                    App.Client = new Client("172.30.1.18", 35155);
                }
                catch (Exception e)
                {
                    await DisplayAlert("서버 연결", e.Message, "확인");
                }
            });

            clientThread.Start();
        }
        #endregion

        #region 인스턴스 가져오기
        public static MainPage GetInstance()
        {
            return ins;
        }
        #endregion
        #endregion
    }
}
