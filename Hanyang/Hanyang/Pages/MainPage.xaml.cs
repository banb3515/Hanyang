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

using Models;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Newtonsoft.Json;
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

            GetData();

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }
        #endregion

        #region 함수
        #region 서버 접속
        private async void GetData()
        {
            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    //await App.Hub.Stop();
                    await DisplayAlert("인터넷에 연결되지 않음",
                        "※ 인터넷 상태를 확인해주세요.\n\n" +
                        "로컬 저장소에 있는 정보를 가져옵니다.\n\n" +
                        "! 인터넷에 연결되면 자동으로 최신 정보를 가져옵니다.", "확인");
                }

                var serverThread = new Thread(() =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        Debug.WriteLine("서버 연결 시도");
                        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                        {
                            try
                            {
                                GetTimetable();
                            }
                            catch (Exception e)
                            {
                                await DisplayAlert("서버 연결 오류", e.Message, "확인");
                            }
                        }
                    });
                });
                serverThread.Start();
            }
            catch (Exception e)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("서버 연결 오류 - 인터넷 상태", e.Message, "확인");
                });
            }
        }
        #endregion

        #region 시간표 가져오기
        private async void GetTimetable()
        {
            var json = WebServer.GetJson("timetable");

            if(json == null)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("시간표 가져오기 오류", "※ 인터넷 상태를 확인해주세요.\n\n시간표를 가져오는 도중 오류가 발생했습니다.", "확인");
                });
                return;
            }

            var tempDict = JsonConvert.DeserializeObject<Dictionary<string, Timetable>>(json);

            foreach(var value in tempDict.Values)
            {
                if (value.ResultCode != "000")
                {
                    await DisplayAlert("시간표 가져오기 오류 (" + value.ResultCode + ")", value.ResultMsg, "확인");
                    return;
                }
            }

            App.Timetable = tempDict;
            await DisplayAlert("테스트", 
                "※ 2컴넷B 월요일 시간표\n" +
                "[날짜: " + App.Timetable["2컴넷B"].Date["Monday"] + "]\n" +
                "1교시: " + App.Timetable["2컴넷B"].Data["Monday"]["1"] + "\n" +
                "2교시: " + App.Timetable["2컴넷B"].Data["Monday"]["2"] + "\n" +
                "3교시: " + App.Timetable["2컴넷B"].Data["Monday"]["3"] + "\n" +
                "4교시: " + App.Timetable["2컴넷B"].Data["Monday"]["4"] + "\n" +
                "5교시: " + App.Timetable["2컴넷B"].Data["Monday"]["5"] + "\n" +
                "6교시: " + App.Timetable["2컴넷B"].Data["Monday"]["6"] + "\n" +
                "7교시: " + App.Timetable["2컴넷B"].Data["Monday"]["7"] + "\n", "확인");
        }
        #endregion

        #region 급식 메뉴 가져오기
        #endregion

        #region 학사 일정 가져오기
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
                GetData();
        }
        #endregion
        #endregion
    }
}
