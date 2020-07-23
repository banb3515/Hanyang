#region API 참조
using Hanyang.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            _ = GetData();

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }
        #endregion

        #region 함수
        #region 데이터 가져오기
        public async Task GetData()
        {
            try
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    try
                    {
                        GetTimetable();
                    }
                    catch (Exception e)
                    {
                        await ErrorAlert("데이터 가져오기", "데이터를 가져오는 도중 오류가 발생했습니다.\n" + e.Message);
                    }
                }
                else
                    DependencyService.Get<IToastMessage>().Longtime("데이터를 가져올 수 없습니다.\n" +
                        "인터넷 상태를 확인해주세요.");
            }
            catch (Exception e)
            {
                await ErrorAlert("데이터 가져오기 (인터넷 상태)", "데이터를 가져오는 도중 오류가 발생했습니다.\n" + e.Message);
            }
        }
        #endregion

        #region 시간표 가져오기
        private void GetTimetable()
        {
            var json = WebServer.GetJson("timetable");

            if (json == null)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await ErrorAlert("시간표 가져오기", "시간표를 가져오는 도중 오류가 발생했습니다.\n인터넷 상태를 확인해주세요.", sendError: false);
                });
                return;
            }

            var tempDict = JsonConvert.DeserializeObject<Dictionary<string, Timetable>>(json.Result);

            foreach (var value in tempDict.Values)
            {
                if (value.ResultCode != "000")
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if(value.ResultCode == "999")
                            await ErrorAlert("시간표 가져오기 (" + value.ResultCode + ")", "시간표를 가져오는 도중 알 수 없는 오류가 발생했습니다.\n" + value.ResultMsg);
                        else
                            await ErrorAlert("시간표 가져오기 (" + value.ResultCode + ")", "시간표를 가져오는 도중 오류가 발생했습니다.\n" + value.ResultMsg, sendError: false);
                    });
                    return;
                }
            }

            App.Timetable = tempDict;

            // 시간표 초기화
            TabbedSchedulePage.GetInstance().task = true;
            _ = TabbedSchedulePage.GetInstance().ViewScheduleAnimation();
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

        #region 오류 출력창
        public async Task ErrorAlert(string title, string message, bool sendError = true)
        {
            if (!sendError)
            {
                await DisplayAlert("오류 - " + title, message, "확인");
                return;
            }

            var result = await DisplayAlert("오류 - " + title, message +
                "\n\n※ 개발자에게 이 오류 내용을 전송하시겠습니까?" +
                "\n- 디바이스 정보도 함께 전송됩니다.", "오류 전송", "취소");

            if(result)
            {
                try
                {
                    var body = "⊙ 오류 제목: " + title +
                        "\n\n⊙ 오류 발생 시각: " + DateTime.Now.ToString("yyyy년 M월 d일 H시 m분 s초") +
                        "\n\n⊙ 오류 내용: " + message +
                        "\n\n⊙ 디바이스 모델명: " + "(" + DeviceInfo.Manufacturer+ ") " + DeviceInfo.Model +
                        "\n\n⊙ OS 버전: " + DeviceInfo.Platform + " " + DeviceInfo.VersionString +
                        "\n\n※ 모든 정보들은 오류 해결을 위해서만 쓰입니다.";

                    var email = new EmailMessage
                    {
                        Subject = "[한양이] 오류 발생",
                        Body = body,
                        To = new List<string> { "banb3515@outlook.kr" } // 개발자 이메일
                    };
                    await Email.ComposeAsync(email);
                }
                catch { }
            }
        }
        #endregion
        #endregion

        #region 이벤트
        #region 인터넷 상태 변경
        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                DependencyService.Get<IToastMessage>().Longtime("인터넷에 연결되었습니다.");
                await GetData();
            }
            else
                DependencyService.Get<IToastMessage>().Longtime("인터넷 연결이 해제되었습니다.");
        }
        #endregion
        #endregion
    }
}
