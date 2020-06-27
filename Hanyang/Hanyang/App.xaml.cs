#region API 참조
using Hanyang.Model;

using System.Collections.Generic;

using Xamarin.Forms;
#endregion

namespace Hanyang
{
    public partial class App : Application
    {
        #region 변수
        public const string VERSION = "1.0"; // 앱 버전
        public static Dictionary<int, Article> notices;
        public static Dictionary<int, Article> sns;
        public static Dictionary<int, Article> appNotices;
        #endregion

        #region 생성자
        public App()
        {
            // Syncfusion 라이선스 키
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mjc3NDE2QDMxMzgyZTMxMmUzMGJiT2Jic1N1ZUVzZHh6U2dLU0RMWDkxbW11VUo2VDY0MnU5bG5mOW1MZW89");

            #region 임시 생성
            notices = new Dictionary<int, Article>();
            sns = new Dictionary<int, Article>();
            appNotices = new Dictionary<int, Article>();

            for (int i = 1; i <= 20; i++)
            {
                var atts = new List<Attachment>
                {
                    new Attachment() { Name = "테스트파일 1", Format = "PDF", Size = "90.4KB" },
                    new Attachment() { Name = "테스트파일 2", Format = "HWP", Size = "50.6KB" },
                    new Attachment() { Name = "테스트파일 3", Format = "PPT", Size = "44.8KB" }
                };

                notices.Add(i, new Article
                {
                    Id = i,
                    Title = "공지사항입니다.",
                    Contents = "공지사항 내용입니다.\n테스트입니다.\nㅋ\nㅇㅇ\nㄴㄴㄴㄴ\nㅂㅈㄷㅂ\nㄷㅂㄷ\nㅂㅈ\nㄷ\nㅁㄴ\nㅇㅁㄴ\nㅇㅁㄴㅇ\nㅋㅌ\n" + 
                    "ㅊㅋㅌ\nㅊㅋ\nㅌㅊ\nㅁㄴ\nㅇㅁㄴ\nㅋ\nㅌ\nㅊㅌㅋ\n호ㅓ\nㅗ허\nㅁㄴ\nㅇㅂ\nㅈㄷ\nㅐㅏㅔ\nㅑㅐ\nㅓㅐ\nㅗㅕㅑㅙ\nㅕㅑㅗ\nㅕㅑㅐ\nㅗㅕㅑㅐ\nㅗㅕㅑㅐㅗ",
                    Info = "최정원 | 2020.06.27",
                    Attachments = atts
                });
            }

            for (int i = 1; i <= 20; i++)
            {
                var atts = new List<Attachment>
                {
                    new Attachment() { Name = "테스트파일 1", Format = "PDF", Size = "90.4KB" }
                };

                sns.Add(i, new Article
                {
                    Id = i,
                    Title = "가정통신문입니다.",
                    Contents = "가정통신문 내용입니다.\n테스트입니다.\nㅋ\nㅇㅇ\nㄴㄴㄴㄴ\nㅂㅈㄷㅂ\nㄷㅂㄷ\nㅂㅈ\nㄷ\nㅁㄴ\nㅇㅁㄴ\nㅇㅁㄴㅇ\nㅋㅌ\n" +
                    "ㅊㅋㅌ\nㅊㅋ\nㅌㅊ\nㅁㄴ\nㅇㅁㄴ\nㅋ\nㅌ\nㅊㅌㅋ\n호ㅓ\nㅗ허\nㅁㄴ\nㅇㅂ\nㅈㄷ\nㅐㅏㅔ\nㅑㅐ\nㅓㅐ\nㅗㅕㅑㅙ\nㅕㅑㅗ\nㅕㅑㅐ\nㅗㅕㅑㅐ\nㅗㅕㅑㅐㅗ",
                    Info = "최정원 | 2020.06.27",
                    Attachments = atts
                });
            }

            for (int i = 1; i <= 20; i++)
            {
                var atts = new List<Attachment>();

                appNotices.Add(i, new Article
                {
                    Id = i,
                    Title = "앱 공지사항입니다.",
                    Contents = "앱 공지사항 내용입니다.\n테스트입니다.\nㅋ\nㅇㅇ\nㄴㄴㄴㄴ\nㅂㅈㄷㅂ\nㄷㅂㄷ\nㅂㅈ\nㄷ\nㅁㄴ\nㅇㅁㄴ\nㅇㅁㄴㅇ\nㅋㅌ\n" +
                    "ㅊㅋㅌ\nㅊㅋ\nㅌㅊ\nㅁㄴ\nㅇㅁㄴ\nㅋ\nㅌ\nㅊㅌㅋ\n호ㅓ\nㅗ허\nㅁㄴ\nㅇㅂ\nㅈㄷ\nㅐㅏㅔ\nㅑㅐ\nㅓㅐ\nㅗㅕㅑㅙ\nㅕㅑㅗ\nㅕㅑㅐ\nㅗㅕㅑㅐ\nㅗㅕㅑㅐㅗ",
                    Info = "최정원 | 2020.06.27",
                    Attachments = atts
                });
            }
            #endregion

            InitializeComponent();

            MainPage = new MainPage();
        }
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
