#region API 참조
using Hanyang.Controller;
using Hanyang.Interface;
using Hanyang.Models;

using Models;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Diagnostics;

using Xamarin.Forms;
#endregion

namespace Hanyang
{
    public partial class App : Application
    {
        #region 변수
        public static string Version { get; } = "1.0"; // 앱 버전

        // 한양이 WebServer API 키
        public const string API_KEY = "3tcPgoxHf2XZboJWuoF3mOX2ZV2OXlfbunUpFvjUvBORUeYWZBApTsYh6PbBXyweF4iPO1wZXLoKXOCrykHMVTrBWvwEcWIOzl1a1CzswHEQvGTWp3hMJEMbFZtqxXcI";

        public static string ServerUrl { get; } = "http://hanyang.azurewebsite.com/"; // 서버 URL

        public static string NewestVersion { get; set; } // 최신 버전

        public static bool Animation { get; set; } // 애니메이션 On/Off

        public static int Grade { get; set; } = 0; // 학년

        public static int Class { get; set; } = 0; // 반

        public static int Number { get; set; } = 0; // 출석 번호

        public static string Name { get; set; } = "NONE"; // 이름

        public static int BirthMonth { get; set; } = 0; // 생일 - 월

        public static int BirthDay { get; set; } = 0; // 생일 - 일

        public static Timetable Timetable { get; set; } // 시간표

        public static LunchMenu LunchMenu { get; set; } // 급식 메뉴

        public static Dictionary<string, SchoolSchedule> SchoolSchedule { get; set; } // 학사 일정

        public static Dictionary<string, Dictionary<string, string>> SchoolNotice { get; set; } // 학교 공지사항

        private static Dictionary<int, Article> sns; // 가정통신문 글 목록
        private static Dictionary<int, Article> appNotices; // 앱 공지사항 글 목록
        #endregion

        #region 앱 종료 확인
        public bool PromptToConfirmExit
        {
            get
            {
                bool promptToConfirmExit = false;
                if (MainPage is ContentPage)
                {
                    promptToConfirmExit = true;
                }
                else if (MainPage is Xamarin.Forms.MasterDetailPage masterDetailPage
                    && masterDetailPage.Detail is NavigationPage detailNavigationPage)
                {
                    promptToConfirmExit = detailNavigationPage.Navigation.NavigationStack.Count <= 1;
                }
                else if (MainPage is NavigationPage mainPage)
                {
                    if (mainPage.CurrentPage is TabbedPage tabbedPage
                        && tabbedPage.CurrentPage is NavigationPage navigationPage)
                    {
                        promptToConfirmExit = navigationPage.Navigation.NavigationStack.Count <= 1;
                    }
                    else
                    {
                        promptToConfirmExit = mainPage.Navigation.NavigationStack.Count <= 1;
                    }
                }
                else if (MainPage is TabbedPage tabbedPage
                    && tabbedPage.CurrentPage is NavigationPage navigationPage)
                {
                    promptToConfirmExit = navigationPage.Navigation.NavigationStack.Count <= 1;
                }
                return promptToConfirmExit;
            }
        }
        #endregion

        #region 생성자
        public App()
        {
            // Syncfusion 라이선스 키
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mjg5MzIwQDMxMzgyZTMyMmUzMG1rdm93cVY1UXUxZDlPS0dESmh5dFBrNmlNenBGYS9pU0RUN3VKV3JwOEE9");

            InitSetting();
            GetSetting();

            InitializeComponent();

            MainPage = new MainPage();
        }
        #endregion

        #region 함수
        #region 설정 초기화
        private async void InitSetting()
        {
            try
            {
                var controller = new JsonController("setting");
                var read = controller.Read();

                // 애니메이션이 설정되지 않았을 때
                if (read != null)
                {
                    if (!read.ContainsKey("Animation"))
                    {
                        // 애니메이션 On으로 초기화
                        try
                        {
                            var dict = new Dictionary<string, object>();
                            dict.Add("Animation", true);
                            controller.Add(dict);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }
                }
                else
                    await controller.Write(new JObject(new JProperty("Animation", true)));
                read = controller.Read();

                App.Animation = Convert.ToBoolean(read["Animation"]);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        #endregion

        #region 반 이름 얻기
        public static string GetClassName()
        {
            var grade = App.Grade;
            var _class = App.Class;
            var dep = "";
            var classAlphabet = "";

            if (_class >= 1 && _class <= 2)
                dep = "건설";
            else if (_class >= 3 && _class <= 4)
                dep = "건축";
            else if (_class >= 5 && _class <= 6)
                dep = "기계";
            else if (_class >= 7 && _class <= 8)
                dep = "전자";
            else if (_class >= 9 && _class <= 10)
                dep = "자동차";
            else if (_class >= 11 && _class <= 12)
                dep = "컴넷";

            if (_class % 2 == 0)
                classAlphabet = "B";
            else
                classAlphabet = "A";

            return App.Grade + dep + classAlphabet;
        }
        #endregion

        #region 설정 가져오기
        private void GetSetting()
        {
            var controller = new JsonController("setting");
            var read = controller.Read();
            var resettingMsg = "더 보기 -> 설정에서 다시 설정을 해주세요.\n! 설정을 하지 않을 시 UI표시에 문제가 생길 수 있습니다.";

            try
            {
                if (read != null)
                {
                    if (!read.ContainsKey("Grade") || !read.ContainsKey("Class") || !read.ContainsKey("Number") || !read.ContainsKey("Name"))
                        DependencyService.Get<IToastMessage>().Shorttime("* 프로필이 설정되지 않았습니다.\n" + resettingMsg);
                    else
                    {
                        Grade = Convert.ToInt32(read["Grade"]);
                        Class = Convert.ToInt32(read["Class"]);
                        Number = Convert.ToInt32(read["Number"]);
                        Name = read["Name"].ToString();
                        BirthMonth = Convert.ToInt32(read["BirthMonth"]);
                        BirthDay = Convert.ToInt32(read["BirthDay"]);
                    }
                }
                else
                    DependencyService.Get<IToastMessage>().Shorttime("* 설정 파일을 찾을 수 없습니다.\n" + resettingMsg);
            }
            catch (Exception e)
            {
                DependencyService.Get<IToastMessage>().Shorttime("※ 설정 가져오기 오류\n" + e.Message);
            }
        }
        #endregion
        #endregion

        #region GET
        public static Dictionary<int, Article> GetSchoolNewsletters() { return sns; }
        public static Dictionary<int, Article> GetAppNotices() { return appNotices; }
        #endregion

        #region 앱 시작
        protected override void OnStart()
        {
            // 서버 연결
        }
        #endregion

        #region 앱 중지
        protected override void OnSleep() { }
        #endregion

        #region 앱 다시시작
        protected override void OnResume() { }
        #endregion
    }
}