#region API 참조
using Hanyang.Interface;
using Hanyang.Controller;

using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using Models;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            try
            {
                // 데이터 정보 파일 읽기
                var controller = new JsonController("data_info");
                var json = controller.ReadString();

                if (json != null)
                    dataInfo = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            }
            catch (Exception e)
            {
                _ = ErrorAlert("알 수 없는 오류 (MainPage)", "알 수 없는 오류가 발생했습니다:\n" + e.Message);
            }

            GetData();
            GetArticle();
            
            var appInfo = GetAppInfo();

            if (appInfo["Version"] != App.Version)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var store = "";

                    switch (Device.RuntimePlatform)
                    {
                        case Device.Android:
                            store = "Google Play 스토어";
                            break;
                        case Device.iOS:
                            store = "앱 스토어";
                            break;
                    }

                    var result = await DisplayAlert("업데이트", "최신 버전(v" + appInfo["Version"] + ")으로 업데이트할 수 있습니다.\n\n" +
                        "※ 업데이트 내용\n" + appInfo["UpdateContent"] + "\n\n" +
                        "[이동] 버튼 클릭 시 " + store + "로 이동합니다.", "이동", "취소");

                    if (result)
                    {
                        switch (Device.RuntimePlatform)
                        {
                            case Device.Android:
                                await Launcher.OpenAsync(new Uri("market://details?id=io.github.banb3515.hanyang"));
                                break;
                            case Device.iOS:
                                await Launcher.OpenAsync(new Uri("itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?id=io.github.banb3515.hanyang&amp;onlyLatestVersion=true&amp;pageNumber=0&amp;sortOrdering=1&amp;type=Purple+Software"));
                                break;
                        }
                    }
                });
            }

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }
        #endregion

        #region 함수
        #region 데이터 가져오기
        public async void GetData(bool refresh = false)
        {
            try
            {
                JsonController controller = null;
                string json = "";

                // 시간표 파일 읽기
                if (App.Class != 0)
                {
                    controller = new JsonController("timetable-" + App.GetClassName());
                    json = controller.ReadString();

                    if (json != null)
                        App.Timetable = JsonConvert.DeserializeObject<Timetable>(json);
                }

                // 급식 메뉴 파일 읽기
                controller = new JsonController("lunch_menu");
                json = controller.ReadString();

                if (json != null)
                    App.LunchMenu = JsonConvert.DeserializeObject<LunchMenu>(json);

                // 학사 일정 파일 읽기
                controller = new JsonController("school_schedule");
                json = controller.ReadString();

                if (json != null)
                    App.SchoolSchedule = JsonConvert.DeserializeObject<Dictionary<string, SchoolSchedule>>(json);

                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    try
                    {
                        if(dataInfo != null)
                        {
                            var serverDataInfo = GetDataInfo();

                            // 서버에 접속할 수 없으므로 로컬 파일 이용
                            if (serverDataInfo == null)
                            {
                                if (App.Timetable != null)
                                {
                                    if (TabbedSchedulePage.GetInstance().view == "schedule")
                                    {
                                        TabbedSchedulePage.GetInstance().task = true;
                                        _ = TabbedSchedulePage.GetInstance().ViewScheduleAnimation();
                                    }
                                }

                                if (App.LunchMenu != null)
                                    TabbedSchedulePage.GetInstance().InitLunchMenu();

                                if (App.SchoolSchedule != null)
                                    TabbedSchedulePage.GetInstance().InitSchoolSchedule();

                                return;
                            }

                            var timetableT = "";
                            var lunchMenuT = "";
                            var schoolScheduleT = "";

                            if (App.Grade != 0 && !dataInfo.ContainsKey("Timetable-" + App.GetClassName()) && !serverDataInfo.ContainsKey("Timetable-" + App.GetClassName()) ||
                                dataInfo["Timetable-" + App.GetClassName()]["Size"] != serverDataInfo["Timetable-" + App.GetClassName()]["Size"])
                                timetableT = GetTimetable();

                            if (!dataInfo.ContainsKey("LunchMenu") && !serverDataInfo.ContainsKey("LunchMenu") || dataInfo["LunchMenu"]["Size"] != serverDataInfo["LunchMenu"]["Size"])
                                lunchMenuT = GetLunchMenu();

                            if (!dataInfo.ContainsKey("SchoolSchedule") && !serverDataInfo.ContainsKey("SchoolSchedule") || dataInfo["SchoolSchedule"]["Size"] != serverDataInfo["SchoolSchedule"]["Size"])
                                schoolScheduleT = GetSchoolSchedule();

                            if(timetableT != "" || lunchMenuT != "" || schoolScheduleT != "")
                            {
                                // 파일로 저장
                                controller = new JsonController("data_info");
                                await controller.Write(JObject.Parse(JsonConvert.SerializeObject(serverDataInfo)));
                            }

                            // 시간표 초기화
                            if (TabbedSchedulePage.GetInstance().view == "schedule")
                            {
                                TabbedSchedulePage.GetInstance().task = true;
                                _ = TabbedSchedulePage.GetInstance().ViewScheduleAnimation();
                            }

                            if (timetableT != "")
                            {
                                // 파일로 저장
                                controller = new JsonController("timetable-" + App.GetClassName());
                                await controller.Write(JObject.Parse(timetableT));
                            }

                            // 급식 메뉴 초기화
                            TabbedSchedulePage.GetInstance().InitLunchMenu();

                            if (lunchMenuT != "")
                            {
                                // 파일로 저장
                                controller = new JsonController("lunch_menu");
                                await controller.Write(JObject.Parse(lunchMenuT));
                            }

                            // 학사 일정 초기화
                            TabbedSchedulePage.GetInstance().InitSchoolSchedule();

                            if (schoolScheduleT != "")
                            {
                                // 파일로 저장
                                controller = new JsonController("school_schedule");
                                await controller.Write(JObject.Parse(schoolScheduleT));
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
                            controller = new JsonController("data_info");
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
                            controller = new JsonController("timetable-" + App.GetClassName());
                            await controller.Write(JObject.Parse(timetable));
                        }

                        if (lunchMenu != "")
                        {
                            // 급식 메뉴 초기화
                            TabbedSchedulePage.GetInstance().InitLunchMenu();

                            // 파일로 저장
                            controller = new JsonController("lunch_menu");
                            await controller.Write(JObject.Parse(lunchMenu));
                        }

                        if (schoolSchedule != "")
                        {
                            // 학사 일정 초기화
                            TabbedSchedulePage.GetInstance().InitSchoolSchedule();

                            // 파일로 저장
                            controller = new JsonController("school_schedule");
                            await controller.Write(JObject.Parse(schoolSchedule));
                        }
                    }
                    catch (Exception e)
                    {
                        await ErrorAlert("데이터 가져오기", "데이터를 가져오는 도중 오류가 발생했습니다.\n" + e.Message);
                    }
                }
                else
                {
                    // 시간표 파일 읽기
                    if (App.Class != 0)
                    {
                        controller = new JsonController("timetable-" + App.GetClassName());
                        json = controller.ReadString();

                        if (json != null)
                        {
                            App.Timetable = JsonConvert.DeserializeObject<Timetable>(json);

                            if (TabbedSchedulePage.GetInstance().view == "schedule")
                            {
                                TabbedSchedulePage.GetInstance().task = true;
                                _ = TabbedSchedulePage.GetInstance().ViewScheduleAnimation();
                            }
                        }
                    }

                    // 급식 메뉴 파일 읽기
                    controller = new JsonController("lunch_menu");
                    json = controller.ReadString();

                    if (json != null)
                    {
                        App.LunchMenu = JsonConvert.DeserializeObject<LunchMenu>(json);

                        TabbedSchedulePage.GetInstance().InitLunchMenu();
                    }

                    // 학사 일정 파일 읽기
                    controller = new JsonController("school_schedule");
                    json = controller.ReadString();

                    if (json != null)
                    {
                        App.SchoolSchedule = JsonConvert.DeserializeObject<Dictionary<string, SchoolSchedule>>(json);

                        TabbedSchedulePage.GetInstance().InitSchoolSchedule();
                    }

                    DependencyService.Get<IToastMessage>().Longtime("인터넷에 연결되어 있지 않아 최신 정보를 확인할 수 없습니다.");
                }
            }
            catch (Exception e)
            {
                await ErrorAlert("데이터 가져오기 (인터넷 상태)", "데이터를 가져오는 도중 오류가 발생했습니다.\n" + e.Message);
            }
            TabbedSchedulePage.GetInstance().task = false;
        }
        #endregion

        #region 게시글 가져오기
        public async void GetArticle(bool refresh = false)
        {
            try
            {
                JsonController controller = null;
                string json = "";

                // 학교 공지사항 파일 읽기
                controller = new JsonController("school_notice");
                json = controller.ReadString();

                if (json != null)
                    App.SchoolNotice = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

                // 가정통신문 파일 읽기
                controller = new JsonController("school_newsletter");
                json = controller.ReadString();

                if (json != null)
                    App.SchoolNewsletter = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

                // 앱 공지사항 파일 읽기
                controller = new JsonController("app_notice");
                json = controller.ReadString();

                if (json != null)
                    App.AppNotice = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    try
                    {
                        if (dataInfo != null)
                        {
                            var serverDataInfo = GetDataInfo();

                            // 서버에 접속할 수 없으므로 로컬 파일 이용
                            if (serverDataInfo == null)
                            {
                                if (App.SchoolNotice != null)
                                    TabbedHomePage.GetInstance().InitSchoolNotice();

                                if (App.SchoolNewsletter != null)
                                    TabbedHomePage.GetInstance().InitSchoolNewsletter();

                                if (App.AppNotice != null)
                                    TabbedHomePage.GetInstance().InitAppNotice();

                                return;
                            }

                            var schoolNoticeT = "";
                            var schoolNewsletterT = "";
                            var appNoticeT = "";

                            if (!dataInfo["SchoolNotice"].ContainsKey("Size") && !serverDataInfo["SchoolNotice"].ContainsKey("Size") || dataInfo["SchoolNotice"]["Size"] != serverDataInfo["SchoolNotice"]["Size"])
                                schoolNoticeT = GetSchoolNotice();

                            if (!dataInfo["SchoolNewsletter"].ContainsKey("Size") && !serverDataInfo["SchoolNewsletter"].ContainsKey("Size") || dataInfo["SchoolNewsletter"]["Size"] != serverDataInfo["SchoolNewsletter"]["Size"])
                                schoolNewsletterT = GetSchoolNewsletter();

                            if (!dataInfo["AppNotice"].ContainsKey("Size") && !serverDataInfo["AppNotice"].ContainsKey("Size") || dataInfo["AppNotice"]["Size"] != serverDataInfo["AppNotice"]["Size"])
                                appNoticeT = GetAppNotice();

                            if (schoolNoticeT != "" || schoolNewsletterT != "" || appNoticeT != "")
                            {
                                // 파일로 저장
                                controller = new JsonController("data_info");
                                await controller.Write(JObject.Parse(JsonConvert.SerializeObject(serverDataInfo)));
                            }

                            // 학교 공지사항 초기화
                            TabbedHomePage.GetInstance().InitSchoolNotice();

                            if (schoolNoticeT != "")
                            {
                                // 파일로 저장
                                controller = new JsonController("school_notice");
                                await controller.Write(JObject.Parse(schoolNoticeT));
                            }

                            // 가정통신문 초기화
                            TabbedHomePage.GetInstance().InitSchoolNewsletter();

                            if (schoolNewsletterT != "")
                            {
                                // 파일로 저장
                                controller = new JsonController("school_newsletter");
                                await controller.Write(JObject.Parse(schoolNewsletterT));
                            }

                            // 앱 공지사항 초기화
                            TabbedHomePage.GetInstance().InitAppNotice();

                            if (appNoticeT != "")
                            {
                                // 파일로 저장
                                controller = new JsonController("app_notice");
                                await controller.Write(JObject.Parse(appNoticeT));
                            }

                            return;
                        }

                        var schoolNotice = "";
                        var schoolNewsletter = "";
                        var appNotice = "";

                        if (refresh || App.SchoolNotice == null)
                            schoolNotice = GetSchoolNotice();

                        if (refresh || App.SchoolNewsletter == null)
                            schoolNewsletter = GetSchoolNewsletter();

                        if (refresh || App.AppNotice == null)
                            appNotice = GetAppNotice();

                        if (schoolNotice != "")
                        {
                            // 학교 공지사항 초기화
                            TabbedHomePage.GetInstance().InitSchoolNotice();

                            // 파일로 저장
                            controller = new JsonController("school_notice");
                            await controller.Write(JObject.Parse(schoolNotice));
                        }

                        if (schoolNewsletter != "")
                        {
                            // 가정통신문 초기화
                            TabbedHomePage.GetInstance().InitSchoolNewsletter();

                            // 파일로 저장
                            controller = new JsonController("school_newsletter");
                            await controller.Write(JObject.Parse(schoolNewsletter));
                        }

                        if (appNotice != "")
                        {
                            // 가정통신문 초기화
                            TabbedHomePage.GetInstance().InitAppNotice();

                            // 파일로 저장
                            controller = new JsonController("app_notice");
                            await controller.Write(JObject.Parse(appNotice));
                        }
                    }
                    catch (Exception e)
                    {
                        await ErrorAlert("게시글 가져오기", "게시글을 가져오는 도중 오류가 발생했습니다.\n" + e.Message);
                    }
                }
                else
                {
                    // 학교 공지사항 파일 읽기
                    controller = new JsonController("school_notice");
                    json = controller.ReadString();

                    if (json != null)
                    {
                        App.SchoolNotice = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

                        TabbedHomePage.GetInstance().InitSchoolNotice();
                    }

                    // 가정통신문 파일 읽기
                    controller = new JsonController("school_newsletter");
                    json = controller.ReadString();

                    if (json != null)
                    {
                        App.SchoolNewsletter = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

                        TabbedHomePage.GetInstance().InitSchoolNewsletter();
                    }

                    // 앱 공지사항 파일 읽기
                    controller = new JsonController("app_notice");
                    json = controller.ReadString();

                    if (json != null)
                    {
                        App.AppNotice = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

                        TabbedHomePage.GetInstance().InitAppNotice();
                    }
                }
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
        #endregion

        #region 앱 정보 가져오기
        public Dictionary<string, string> GetAppInfo()
        {
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
        #endregion

        #region 시간표 가져오기
        public string GetTimetable()
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
        #endregion

        #region 급식 메뉴 가져오기
        public string GetLunchMenu()
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
        #endregion

        #region 학사 일정 가져오기
        public string GetSchoolSchedule()
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
        #endregion

        #region 학교 공지사항 가져오기
        public string GetSchoolNotice()
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
        #endregion

        #region 가정통신문 가져오기
        public string GetSchoolNewsletter()
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
        #endregion

        #region 앱 공지사항 가져오기
        public string GetAppNotice()
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
                GetData();
            }
            else
                DependencyService.Get<IToastMessage>().Longtime("인터넷 연결이 해제되었습니다.");
        }
        #endregion
        #endregion
    }
}
