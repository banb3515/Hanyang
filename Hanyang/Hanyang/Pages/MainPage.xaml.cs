#region API 참조
using Hanyang.Interface;
using Hanyang.Controller;

using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

using Models;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using AiForms.Dialogs.Abstractions;
using AiForms.Dialogs;
#endregion

namespace Hanyang.Pages
{
    [DesignTimeVisible(false)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        #region 변수
        public static MainPage ins; // Instance

        public Dictionary<string, Dictionary<string, string>> dataInfo;
        #endregion

        #region 생성자
        public MainPage()
        {
            #region 변수 초기화
            ins = this;
            #endregion

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            InitializeComponent();

            LoadingData();

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }
        #endregion

        #region 함수
        #region 데이터 로딩
        public void LoadingData()
        {
            try
            {
                Configurations.LoadingConfig = new LoadingConfig
                {
                    IndicatorColor = Color.White,
                    OverlayColor = Color.Black,
                    Opacity = 0.75,
                    DefaultMessage = "데이터를 가져오는 중 입니다...\n잠시만 기다려주세요.",
                };

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Loading.Instance.StartAsync(async progress =>
                    {
                        try
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                await Task.Delay(10);
                                progress.Report((i + 1) * 0.01d);
                            }

                            try
                            {
                                // 데이터 정보 파일 읽기
                                var controller = new JsonController("data_info");
                                for (int i = 10; i < 15; i++)
                                {
                                    await Task.Delay(10);
                                    progress.Report((i + 1) * 0.01d);
                                }
                                var json = controller.ReadString();
                                for (int i = 15; i < 20; i++)
                                {
                                    await Task.Delay(10);
                                    progress.Report((i + 1) * 0.01d);
                                }

                                if (json != null)
                                    dataInfo = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
                                for (int i = 20; i < 30; i++)
                                {
                                    await Task.Delay(10);
                                    progress.Report((i + 1) * 0.01d);
                                }
                            }
                            catch (Exception e)
                            {
                                _ = ErrorAlert("알 수 없는 오류 (MainPage)", "알 수 없는 오류가 발생했습니다:\n" + e.Message);
                            }

                            InitUI();
                            for (int i = 30; i < 50; i++)
                            {
                                await Task.Delay(10);
                                progress.Report((i + 1) * 0.01d);
                            }

                            await GetData();
                            for (int i = 50; i < 70; i++)
                            {
                                await Task.Delay(10);
                                progress.Report((i + 1) * 0.01d);
                            }
                            await GetArticle();
                            for (int i = 70; i < 90; i++)
                            {
                                await Task.Delay(10);
                                progress.Report((i + 1) * 0.01d);
                            }

                            #region 앱 정보 가져오기
                            var appInfo = GetAppInfo();
                            for (int i = 90; i < 99; i++)
                            {
                                await Task.Delay(10);
                                progress.Report((i + 1) * 0.01d);
                            }

                            if (appInfo != null && appInfo["Version"] != App.Version)
                            {
                                var store = "";
                                var uri = "";

                                switch (Device.RuntimePlatform)
                                {
                                    case Device.Android:
                                        store = "Google Play 스토어";
                                        uri = "market://details?id=io.github.banb3515.hanyang";
                                        break;
                                    case Device.iOS:
                                        store = "앱 스토어";
                                        uri = "itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?id=io.github.banb3515.hanyang&amp;onlyLatestVersion=true&amp;pageNumber=0&amp;sortOrdering=1&amp;type=Purple+Software";
                                        break;
                                }

                                var result = await DisplayAlert("업데이트", "최신 버전(v" + appInfo["Version"] + ")으로 업데이트할 수 있습니다.\n\n" +
                                    "※ 업데이트 내용\n" + appInfo["UpdateContent"] + "\n\n" +
                                    "[이동] 버튼 클릭 시 " + store + "로 이동합니다.", "이동", "취소");

                                if (result)
                                    await Launcher.OpenAsync(new Uri(uri));
                            }
                            #endregion

                            for (int i = 95; i < 100; i++)
                            {
                                await Task.Delay(10);
                                progress.Report((i + 1) * 0.01d);
                            }
                        }
                        catch (Exception) { }
                    });
                });
            }
            catch (Exception) { }
        }
        #endregion

        #region UI 초기화
        public async void InitUI()
        {
            try
            {
                JsonController controller = null;
                string timetableJson = null;
                string lunchMenuJson = null;
                string schoolScheduleJson = null;
                string schoolNoticeJson = null;
                string schoolNewsletterJson = null;
                string appNoticeJson = null;

                if (App.Class != 0)
                {
                    controller = new JsonController("timetable-" + App.GetClassName());
                    timetableJson = controller.ReadString();
                }

                controller = new JsonController("lunch_menu");
                lunchMenuJson = controller.ReadString();

                controller = new JsonController("school_schedule");
                schoolScheduleJson = controller.ReadString();

                controller = new JsonController("school_notice");
                schoolNoticeJson = controller.ReadString();

                controller = new JsonController("school_newsletter");
                schoolNewsletterJson = controller.ReadString();

                controller = new JsonController("app_notice");
                appNoticeJson = controller.ReadString();

                #region 시간표 파일 읽기
                if (timetableJson != null)
                {
                    App.Timetable = JsonConvert.DeserializeObject<Timetable>(timetableJson);

                    TabbedSchedulePage.GetInstance().task = true;
                    _ = TabbedSchedulePage.GetInstance().ViewScheduleAnimation();
                }
                #endregion

                #region 급식 메뉴 파일 읽기
                if (lunchMenuJson != null)
                {
                    App.LunchMenu = JsonConvert.DeserializeObject<LunchMenu>(lunchMenuJson);

                    TabbedSchedulePage.GetInstance().InitLunchMenu();
                }
                #endregion

                #region 학사 일정 파일 읽기
                if (schoolScheduleJson != null)
                {
                    App.SchoolSchedule = JsonConvert.DeserializeObject<Dictionary<string, SchoolSchedule>>(schoolScheduleJson);

                    TabbedSchedulePage.GetInstance().InitSchoolSchedule();
                }
                #endregion

                #region 학교 공지사항 파일 읽기
                if (schoolNoticeJson != null)
                {
                    App.SchoolNotice = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(schoolNoticeJson);

                    TabbedHomePage.GetInstance().InitSchoolNotice();
                }
                #endregion

                #region 가정통신문 파일 읽기
                if (schoolNewsletterJson != null)
                {
                    App.SchoolNewsletter = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(schoolNewsletterJson);

                    TabbedHomePage.GetInstance().InitSchoolNewsletter();
                }
                #endregion

                #region 앱 공지사항 파일 읽기
                if (appNoticeJson != null)
                {
                    App.AppNotice = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(appNoticeJson);

                    TabbedHomePage.GetInstance().InitAppNotice();
                }
                #endregion
            }
            catch (Exception e)
            {
                await ErrorAlert("UI 초기화", "UI를 초기화하는 도중 오류가 발생했습니다.\n" + e.Message);
            }
        }
        #endregion

        #region 데이터 가져오기
        public async Task GetData(bool refresh = false)
        {
            try
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    try
                    {
                        if(dataInfo != null && !refresh)
                        {
                            var serverDataInfo = GetDataInfo();

                            if (serverDataInfo != null)
                            {
                                serverDataInfo.Remove("SchoolNotice");
                                serverDataInfo.Remove("SchoolNewsletter");
                                serverDataInfo.Remove("AppNotice");
                                serverDataInfo.Add("SchoolNotice", dataInfo["SchoolNotice"]);
                                serverDataInfo.Add("SchoolNewsletter", dataInfo["SchoolNewsletter"]);
                                serverDataInfo.Add("AppNotice", dataInfo["AppNotice"]);

                                var timetableChange = false;
                                var lunchMenuChange = false;
                                var schoolScheduleChange = false;

                                if (App.Grade != 0 && dataInfo.ContainsKey("Timetable-" + App.GetClassName()) &&
                                    dataInfo["Timetable-" + App.GetClassName()].ContainsKey("Size") &&
                                    serverDataInfo.ContainsKey("Timetable-" + App.GetClassName()) &&
                                    serverDataInfo["Timetable-" + App.GetClassName()].ContainsKey("Size") &&
                                    dataInfo["Timetable-" + App.GetClassName()]["Size"] != serverDataInfo["Timetable-" + App.GetClassName()]["Size"])
                                    timetableChange = true;

                                if (dataInfo["LunchMenu"].ContainsKey("Size") && serverDataInfo["LunchMenu"].ContainsKey("Size") &&
                                    dataInfo["LunchMenu"]["Size"] != serverDataInfo["LunchMenu"]["Size"])
                                    lunchMenuChange = true;

                                if (dataInfo["SchoolSchedule"].ContainsKey("Size") && serverDataInfo["SchoolSchedule"].ContainsKey("Size") &&
                                    dataInfo["SchoolSchedule"]["Size"] != serverDataInfo["SchoolSchedule"]["Size"])
                                    schoolScheduleChange = true;

                                if(timetableChange || lunchMenuChange || schoolScheduleChange)
                                {
                                    var result = await DisplayAlert("최신 데이터 발견", 
                                        "최신 데이터를 발견하였습니다.\n" +
                                        (timetableChange ? "\n- 시간표 : " + serverDataInfo["Timetable-" + App.GetClassName()]["Size"] : "") +
                                        (lunchMenuChange ? "\n- 급식 메뉴 : " + serverDataInfo["LunchMenu"]["Size"] : "") +
                                        (schoolScheduleChange ? "\n- 학사 일정 : " + serverDataInfo["SchoolSchedule"]["Size"] : ""), "다운로드", "취소");

                                    if (!result)
                                        return;

                                    var timetableT = "";
                                    var lunchMenuT = "";
                                    var schoolScheduleT = "";

                                    if (timetableChange)
                                        timetableT = GetTimetable();

                                    if (lunchMenuChange)
                                        lunchMenuT = GetLunchMenu();

                                    if (schoolScheduleChange)
                                        schoolScheduleT = GetSchoolSchedule();

                                    if (timetableT != "" || lunchMenuT != "" || schoolScheduleT != "")
                                    {
                                        // 파일로 저장
                                        var controller = new JsonController("data_info");
                                        dataInfo = serverDataInfo;
                                        await controller.Write(JObject.Parse(JsonConvert.SerializeObject(serverDataInfo)));
                                    }

                                    #region 시간표 초기화
                                    if (timetableT != "")
                                    {
                                        if (TabbedSchedulePage.GetInstance().view == "schedule")
                                        {
                                            TabbedSchedulePage.GetInstance().task = true;
                                            _ = TabbedSchedulePage.GetInstance().ViewScheduleAnimation();
                                        }

                                        // 파일로 저장
                                        var controller = new JsonController("timetable-" + App.GetClassName());
                                        await controller.Write(JObject.Parse(timetableT));
                                    }
                                    #endregion

                                    #region 급식 메뉴 초기화
                                    if (lunchMenuT != "")
                                    {
                                        TabbedSchedulePage.GetInstance().InitLunchMenu();

                                        // 파일로 저장
                                        var controller = new JsonController("lunch_menu");
                                        await controller.Write(JObject.Parse(lunchMenuT));
                                    }
                                    #endregion

                                    #region 학사 일정 초기화
                                    if (schoolScheduleT != "")
                                    {
                                        TabbedSchedulePage.GetInstance().InitSchoolSchedule();

                                        // 파일로 저장
                                        var controller = new JsonController("school_schedule");
                                        await controller.Write(JObject.Parse(schoolScheduleT));
                                    }
                                    #endregion
                                }
                            }

                            return;
                        }

                        var dataInfoT = GetDataInfo();
                        var timetable = "";
                        var lunchMenu = "";
                        var schoolSchedule = "";

                        if (refresh || App.Timetable == null)
                            timetable = GetTimetable();

                        if (refresh || App.LunchMenu == null)
                            lunchMenu = GetLunchMenu();

                        if (refresh || App.SchoolSchedule == null)
                            schoolSchedule = GetSchoolSchedule();

                        if(dataInfoT != null)
                        {
                            var controller = new JsonController("data_info");
                            await controller.Write(JObject.Parse(JsonConvert.SerializeObject(dataInfoT)));
                        }

                        if (timetable != "")
                        {
                            if (TabbedSchedulePage.GetInstance().view == "schedule")
                            {
                                // 시간표 초기화
                                TabbedSchedulePage.GetInstance().task = true;
                                _ = TabbedSchedulePage.GetInstance().ViewScheduleAnimation();
                            }

                            // 파일로 저장
                            var controller = new JsonController("timetable-" + App.GetClassName());
                            await controller.Write(JObject.Parse(timetable));
                        }

                        if (lunchMenu != "")
                        {
                            // 급식 메뉴 초기화
                            TabbedSchedulePage.GetInstance().InitLunchMenu();

                            // 파일로 저장
                            var controller = new JsonController("lunch_menu");
                            await controller.Write(JObject.Parse(lunchMenu));
                        }

                        if (schoolSchedule != "")
                        {
                            // 학사 일정 초기화
                            TabbedSchedulePage.GetInstance().InitSchoolSchedule();

                            // 파일로 저장
                            var controller = new JsonController("school_schedule");
                            await controller.Write(JObject.Parse(schoolSchedule));
                        }
                    }
                    catch (Exception e)
                    {
                        await ErrorAlert("데이터 가져오기", "데이터를 가져오는 도중 오류가 발생했습니다.\n" + e.Message);
                    }
                }
                else
                    DependencyService.Get<IToastMessage>().Longtime("인터넷에 연결되어 있지 않아 최신 정보를 확인할 수 없습니다.");
            }
            catch (Exception e)
            {
                await ErrorAlert("데이터 가져오기 (인터넷 상태)", "데이터를 가져오는 도중 오류가 발생했습니다.\n" + e.Message);
            }
            TabbedSchedulePage.GetInstance().task = false;
        }
        #endregion

        #region 게시글 가져오기
        public async Task GetArticle(bool refresh = false)
        {
            try
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    try
                    {
                        if (dataInfo != null && !refresh)
                        {
                            var serverDataInfo = GetDataInfo();

                            if (serverDataInfo != null)
                            {
                                serverDataInfo.Remove("Timetable-" + App.GetClassName());
                                serverDataInfo.Remove("LunchMenu");
                                serverDataInfo.Remove("SchoolSchedule");
                                serverDataInfo.Add("Timetable-" + App.GetClassName(), dataInfo["Timetable-" + App.GetClassName()]);
                                serverDataInfo.Add("LunchMenu", dataInfo["LunchMenu"]);
                                serverDataInfo.Add("SchoolSchedule", dataInfo["SchoolSchedule"]);

                                var schoolNoticeChange = false;
                                var schoolNewsletterChange = false;
                                var appNoticeChange = false;

                                if (dataInfo["SchoolNotice"].ContainsKey("Size") && serverDataInfo["SchoolNotice"].ContainsKey("Size") &&
                                    dataInfo["SchoolNotice"]["Size"] != serverDataInfo["SchoolNotice"]["Size"])
                                    schoolNoticeChange = true;

                                if (dataInfo["SchoolNewsletter"].ContainsKey("Size") && serverDataInfo["SchoolNewsletter"].ContainsKey("Size") &&
                                    dataInfo["SchoolNewsletter"]["Size"] != serverDataInfo["SchoolNewsletter"]["Size"])
                                    schoolNewsletterChange = true;

                                if (dataInfo["AppNotice"].ContainsKey("Size") && serverDataInfo["AppNotice"].ContainsKey("Size") &&
                                    dataInfo["AppNotice"]["Size"] != serverDataInfo["AppNotice"]["Size"])
                                    appNoticeChange = true;

                                if (schoolNoticeChange || schoolNewsletterChange || appNoticeChange)
                                {
                                    var result = await DisplayAlert("최신 데이터 발견",
                                        "최신 데이터를 발견하였습니다.\n" +
                                        (schoolNoticeChange ? "\n- 공지사항 : " + serverDataInfo["SchoolNotice"]["Size"] : "") +
                                        (schoolNewsletterChange ? "\n- 가정통신문 : " + serverDataInfo["SchoolNewsletter"]["Size"] : "") +
                                        (appNoticeChange ? "\n- 앱 공지사항 : " + serverDataInfo["AppNotice"]["Size"] : ""), "다운로드", "취소");

                                    if (!result)
                                        return;

                                    var schoolNoticeT = "";
                                    var schoolNewsletterT = "";
                                    var appNoticeT = "";

                                    if (schoolNoticeChange)
                                        schoolNoticeT = GetTimetable();

                                    if (schoolNewsletterChange)
                                        schoolNewsletterT = GetLunchMenu();

                                    if (appNoticeChange)
                                        appNoticeT = GetSchoolSchedule();

                                    if (schoolNoticeT != "" || schoolNewsletterT != "" || appNoticeT != "")
                                    {
                                        // 파일로 저장
                                        var controller = new JsonController("data_info");
                                        dataInfo = serverDataInfo;
                                        await controller.Write(JObject.Parse(JsonConvert.SerializeObject(serverDataInfo)));
                                    }

                                    #region 공지사항 초기화
                                    if (schoolNoticeT != "")
                                    {
                                        TabbedHomePage.GetInstance().InitSchoolNotice();

                                        // 파일로 저장
                                        var controller = new JsonController("school_notice");
                                        await controller.Write(JObject.Parse(schoolNoticeT));
                                    }
                                    #endregion

                                    #region 가정통신문 초기화
                                    if (schoolNewsletterT != "")
                                    {
                                        TabbedHomePage.GetInstance().InitSchoolNewsletter();

                                        // 파일로 저장
                                        var controller = new JsonController("school_newsletter");
                                        await controller.Write(JObject.Parse(schoolNewsletterT));
                                    }
                                    #endregion

                                    #region 앱 공지사항 초기화
                                    if (appNoticeT != "")
                                    {
                                        TabbedHomePage.GetInstance().InitAppNotice();

                                        // 파일로 저장
                                        var controller = new JsonController("app_notice");
                                        await controller.Write(JObject.Parse(appNoticeT));
                                    }
                                    #endregion
                                }
                            }

                            return;
                        }

                        var dataInfoT = GetDataInfo();
                        var schoolNotice = "";
                        var schoolNewsletter = "";
                        var appNotice = "";

                        if (refresh || App.SchoolNotice == null)
                            schoolNotice = GetSchoolNotice();

                        if (refresh || App.SchoolNewsletter == null)
                            schoolNewsletter = GetSchoolNewsletter();

                        if (refresh || App.AppNotice == null)
                            appNotice = GetAppNotice();

                        if (dataInfoT != null)
                        {
                            var controller = new JsonController("data_info");
                            await controller.Write(JObject.Parse(JsonConvert.SerializeObject(dataInfoT)));
                        }

                        if (schoolNotice != "")
                        {
                            // 공지사항 초기화
                            TabbedHomePage.GetInstance().InitSchoolNotice();

                            // 파일로 저장
                            var controller = new JsonController("school_notice");
                            await controller.Write(JObject.Parse(schoolNotice));
                        }

                        if (schoolNewsletter != "")
                        {
                            // 가정통신문 초기화
                            TabbedHomePage.GetInstance().InitSchoolNewsletter();

                            // 파일로 저장
                            var controller = new JsonController("school_newsletter");
                            await controller.Write(JObject.Parse(schoolNewsletter));
                        }

                        if (appNotice != "")
                        {
                            // 앱 공지사항 초기화
                            TabbedHomePage.GetInstance().InitAppNotice();

                            // 파일로 저장
                            var controller = new JsonController("app_notice");
                            await controller.Write(JObject.Parse(appNotice));
                        }
                    }
                    catch (Exception e)
                    {
                        await ErrorAlert("게시글 가져오기", "게시글을 가져오는 도중 오류가 발생했습니다.\n" + e.Message);
                    }
                }
                else
                    DependencyService.Get<IToastMessage>().Longtime("인터넷에 연결되어 있지 않아 최신 정보를 확인할 수 없습니다.");
            }
            catch (Exception e)
            {
                await ErrorAlert("게시글 가져오기 (인터넷 상태)", "게시글을 가져오는 도중 오류가 발생했습니다.\n" + e.Message);
            }
        }
        #endregion

        #region 데이터 정보 가져오기
        public Dictionary<string, Dictionary<string, string>> GetDataInfo()
        {
            try
            {
                var json = WebServer.GetJson("datainfo");

                if (json == null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await ErrorAlert("데이터 정보 가져오기", "데이터 정보를 가져오는 도중 오류가 발생했습니다.\n인터넷 상태를 확인해주세요.", sendError: false);
                    });
                    return null;
                }

                var tempDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json.Result);

                foreach (var value in tempDict.Values)
                {
                    if (value.ContainsKey("ResultCode"))
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            if (value["ResultCode"] == "999")
                                await ErrorAlert("데이터 정보 가져오기 (" + value["ResultCode"] + ")", "데이터 정보를 가져오는 도중 알 수 없는 오류가 발생했습니다.\n" + value["ResultMsg"]);
                            else
                                await ErrorAlert("데이터 정보 가져오기 (" + value["ResultCode"] + ")", "데이터 정보를 가져오는 도중 오류가 발생했습니다.\n" + value["ResultMsg"], sendError: false);
                        });
                        return null;
                    }
                }

                return tempDict;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region 앱 정보 가져오기
        public Dictionary<string, string> GetAppInfo()
        {
            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                    return null;
                
                var json = WebServer.GetJson("appinfo");

                if (json == null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await ErrorAlert("앱 정보 가져오기", "앱 정보를 가져오는 도중 오류가 발생했습니다.\n인터넷 상태를 확인해주세요.", sendError: false);
                    });
                    return null;
                }

                var tempDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json.Result);

                if (tempDict.ContainsKey("ResultCode"))
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (tempDict["ResultCode"] == "999")
                            await ErrorAlert("앱 정보 가져오기 (" + tempDict["ResultCode"] + ")", "앱 정보를 가져오는 도중 알 수 없는 오류가 발생했습니다.\n"
                                + tempDict["ResultMsg"]);
                        else
                            await ErrorAlert("앱 정보 가져오기 (" + tempDict["ResultCode"] + ")", "앱 정보를 가져오는 도중 오류가 발생했습니다.\n" +
                                tempDict["ResultMsg"], sendError: false);
                    });
                    return null;
                }

                return tempDict;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region 시간표 가져오기
        public string GetTimetable()
        {
            try
            {
                if (App.Class == 0)
                {
                    DependencyService.Get<IToastMessage>().Longtime("데이터를 가져올 수 없습니다.\n" +
                                "프로필 설정을 완료해주세요.");
                    return "";
                }

                var json = WebServer.GetJson("timetable", App.GetClassName());

                if (json == null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await ErrorAlert("시간표 가져오기", "시간표를 가져오는 도중 오류가 발생했습니다.\n인터넷 상태를 확인해주세요.", sendError: false);
                    });
                    return "";
                }

                var timetable = JsonConvert.DeserializeObject<Timetable>(json.Result);

                if (timetable.ResultCode != "000")
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (timetable.ResultCode == "999")
                            await ErrorAlert("시간표 가져오기 (" + timetable.ResultCode + ")", "시간표를 가져오는 도중 알 수 없는 오류가 발생했습니다.\n" + timetable.ResultMsg);
                        else
                            await ErrorAlert("시간표 가져오기 (" + timetable.ResultCode + ")", "시간표를 가져오는 도중 오류가 발생했습니다.\n" + timetable.ResultMsg, sendError: false);
                    });
                    return "";
                }

                App.Timetable = timetable;
                return json.Result;
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion

        #region 급식 메뉴 가져오기
        public string GetLunchMenu()
        {
            try
            {
                var json = WebServer.GetJson("lunchmenu");

                if (json == null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await ErrorAlert("급식 메뉴 가져오기", "급식 메뉴를 가져오는 도중 오류가 발생했습니다.\n인터넷 상태를 확인해주세요.", sendError: false);
                    });
                    return "";
                }

                var lunchMenu = JsonConvert.DeserializeObject<LunchMenu>(json.Result);

                if (lunchMenu.ResultCode != "000")
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (lunchMenu.ResultCode == "999")
                            await ErrorAlert("급식 메뉴 가져오기 (" + lunchMenu.ResultCode + ")", "급식 메뉴를 가져오는 도중 알 수 없는 오류가 발생했습니다.\n" + lunchMenu.ResultMsg);
                        else
                            await ErrorAlert("급식 메뉴 가져오기 (" + lunchMenu.ResultCode + ")", "급식 메뉴를 가져오는 도중 오류가 발생했습니다.\n" + lunchMenu.ResultMsg, sendError: false);
                    });
                    return "";
                }

                App.LunchMenu = lunchMenu;
                return json.Result;
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion

        #region 학사 일정 가져오기
        public string GetSchoolSchedule()
        {
            try
            {
                var json = WebServer.GetJson("schoolschedule");

                if (json == null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await ErrorAlert("학사 일정 가져오기", "학사 일정을 가져오는 도중 오류가 발생했습니다.\n인터넷 상태를 확인해주세요.", sendError: false);
                    });
                    return "";
                }

                var schoolSchedule = JsonConvert.DeserializeObject<Dictionary<string, SchoolSchedule>>(json.Result);

                foreach (var value in schoolSchedule.Values)
                {
                    if (value.ResultCode != "000")
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            if (value.ResultCode == "999")
                                await ErrorAlert("학사 일정 가져오기 (" + value.ResultCode + ")", "학사 일정을 가져오는 도중 알 수 없는 오류가 발생했습니다.\n" + value.ResultMsg);
                            else
                                await ErrorAlert("학사 일정 가져오기 (" + value.ResultCode + ")", "학사 일정을 가져오는 도중 오류가 발생했습니다.\n" + value.ResultMsg, sendError: false);
                        });
                        return "";
                    }
                }

                App.SchoolSchedule = schoolSchedule;
                return json.Result;
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion

        #region 학교 공지사항 가져오기
        public string GetSchoolNotice()
        {
            try
            {
                var json = WebServer.GetJson("schoolnotice");

                if (json == null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await ErrorAlert("학교 공지사항 가져오기", "학교 공지사항을 가져오는 도중 오류가 발생했습니다.\n인터넷 상태를 확인해주세요.", sendError: false);
                    });
                    return "";
                }

                var schoolNotice = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json.Result);

                if (schoolNotice.ContainsKey("Error"))
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (schoolNotice["Error"]["ResultCode"] == "999")
                            await ErrorAlert("학교 공지사항 가져오기 (" + schoolNotice["Error"]["ResultCode"] + ")",
                                "학교 공지사항을 가져오는 도중 알 수 없는 오류가 발생했습니다.\n" + schoolNotice["Error"]["ResultMsg"]);
                        else
                            await ErrorAlert("학교 공지사항 가져오기 (" + schoolNotice["Error"]["ResultCode"] + ")",
                                "학교 공지사항을 가져오는 도중 오류가 발생했습니다.\n" + schoolNotice["Error"]["ResultMsg"], sendError: false);
                    });
                    return "";
                }

                App.SchoolNotice = schoolNotice;
                return json.Result;
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion

        #region 가정통신문 가져오기
        public string GetSchoolNewsletter()
        {
            try
            {
                var json = WebServer.GetJson("schoolnewsletter");

                if (json == null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await ErrorAlert("가정통신문 가져오기", "가정통신문을 가져오는 도중 오류가 발생했습니다.\n인터넷 상태를 확인해주세요.", sendError: false);
                    });
                    return "";
                }

                var schoolNewsletter = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json.Result);

                if (schoolNewsletter.ContainsKey("Error"))
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (schoolNewsletter["Error"]["ResultCode"] == "999")
                            await ErrorAlert("가정통신문 가져오기 (" + schoolNewsletter["Error"]["ResultCode"] + ")",
                                "가정통신문을 가져오는 도중 알 수 없는 오류가 발생했습니다.\n" + schoolNewsletter["Error"]["ResultMsg"]);
                        else
                            await ErrorAlert("가정통신문 가져오기 (" + schoolNewsletter["Error"]["ResultCode"] + ")",
                                "가정통신문을 가져오는 도중 오류가 발생했습니다.\n" + schoolNewsletter["Error"]["ResultMsg"], sendError: false);
                    });
                    return "";
                }

                App.SchoolNewsletter = schoolNewsletter;
                return json.Result;
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion

        #region 앱 공지사항 가져오기
        public string GetAppNotice()
        {
            try
            {
                var json = WebServer.GetJson("appnotice");

                if (json == null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await ErrorAlert("앱 공지사항 가져오기", "앱 공지사항을 가져오는 도중 오류가 발생했습니다.\n인터넷 상태를 확인해주세요.", sendError: false);
                    });
                    return "";
                }

                var appNotice = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json.Result);

                if (appNotice.ContainsKey("Error"))
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (appNotice["Error"]["ResultCode"] == "999")
                            await ErrorAlert("앱 공지사항 가져오기 (" + appNotice["Error"]["ResultCode"] + ")",
                                "앱 공지사항을 가져오는 도중 알 수 없는 오류가 발생했습니다.\n" + appNotice["Error"]["ResultMsg"]);
                        else
                            await ErrorAlert("앱 공지사항 가져오기 (" + appNotice["Error"]["ResultCode"] + ")",
                                "앱 공지사항을 가져오는 도중 오류가 발생했습니다.\n" + appNotice["Error"]["ResultMsg"], sendError: false);
                    });
                    return "";
                }

                App.AppNotice = appNotice;
                return json.Result;
            }
            catch (Exception)
            {
                return "";
            }
        }
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

        #region Json 바이트 크기 가져오기
        public async Task<int> GetJsonByteLength(object obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);

                byte[] byteArr;
                byteArr = Encoding.UTF8.GetBytes(json);
                return byteArr.Length;
            }
            catch (Exception e)
            {
                await ErrorAlert("Json 데이터 바이트 크기 가져오기", "Json 데이터 바이트 크기를 가져오는 도중 오류가 발생했습니다.\n" + e.Message);
                return 0;
            }
        }
        #endregion
        #endregion

        #region 이벤트
        #region 인터넷 상태 변경
        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                DependencyService.Get<IToastMessage>().Longtime("인터넷에 연결되었습니다.");
                Thread thread = new Thread(async () =>
                {
                    await GetData();
                    await GetArticle();
                });
            }
            else
                DependencyService.Get<IToastMessage>().Longtime("인터넷 연결이 해제되었습니다.");
        }
        #endregion
        #endregion
    }
}
