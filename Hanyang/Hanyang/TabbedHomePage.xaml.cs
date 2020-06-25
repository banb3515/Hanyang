#region API 참조
using Hanyang.Animations;
using Hanyang.BindingData;
using Hanyang.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace Hanyang
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedHomePage : ContentPage
    {
        #region 변수
        private bool task;
        private string view;
        private bool homepageButtonRotate;
        #endregion

        #region 생성자
        public TabbedHomePage()
        {
            #region 변수 초기화
            task = false;
            view = "notice";
            homepageButtonRotate = false;
            #endregion

            InitializeComponent();

            #region 글 목록 임시 생성
            List<Article> notices = new List<Article>();
            notices.Add(new Article("테스트 공지", "최정원 | 2020.06.23"));
            notices.Add(new Article("아아아아", "전준혁 | 2020.06.23"));
            notices.Add(new Article("ㅋㅋㅋ", "김정태 | 2020.06.23"));
            notices.Add(new Article("abcd", "ㅇㅇㅇ | 2020.06.23"));
            notices.Add(new Article("뭐지용", "? ? ? | 2020.06.23"));
            notices.Add(new Article("테스트 공지", "최정원 | 2020.06.23"));
            notices.Add(new Article("아아아아", "전준혁 | 2020.06.23"));
            notices.Add(new Article("ㅋㅋㅋ", "김정태 | 2020.06.23"));
            notices.Add(new Article("abcd", "ㅇㅇㅇ | 2020.06.23"));
            notices.Add(new Article("뭐지용", "? ? ? | 2020.06.23"));
            notices.Add(new Article("테스트 공지", "최정원 | 2020.06.23"));
            notices.Add(new Article("아아아아", "전준혁 | 2020.06.23"));
            notices.Add(new Article("ㅋㅋㅋ", "김정태 | 2020.06.23"));
            notices.Add(new Article("abcd", "ㅇㅇㅇ | 2020.06.23"));
            notices.Add(new Article("뭐지용", "? ? ? | 2020.06.23"));
            notices.Add(new Article("테스트 공지", "최정원 | 2020.06.23"));
            notices.Add(new Article("아아아아", "전준혁 | 2020.06.23"));
            notices.Add(new Article("ㅋㅋㅋ", "김정태 | 2020.06.23"));
            notices.Add(new Article("abcd", "ㅇㅇㅇ | 2020.06.23"));
            notices.Add(new Article("뭐지용", "? ? ? | 2020.06.23"));
            notices.Add(new Article("테스트 공지", "최정원 | 2020.06.23"));
            notices.Add(new Article("아아아아", "전준혁 | 2020.06.23"));
            NoticeList.ItemsSource = notices;

            List<Article> sns = new List<Article>();
            sns.Add(new Article("테스트 가정통신문", "최정원 | 2020.06.23"));
            sns.Add(new Article("알립니다요", "김정태 | 2020.06.23"));
            sns.Add(new Article("가정통신문이요", "전준혁 | 2020.06.23"));
            sns.Add(new Article("테스트 가정통신문", "최정원 | 2020.06.23"));
            sns.Add(new Article("알립니다요", "김정태 | 2020.06.23"));
            sns.Add(new Article("가정통신문이요", "전준혁 | 2020.06.23"));
            sns.Add(new Article("테스트 가정통신문", "최정원 | 2020.06.23"));
            sns.Add(new Article("알립니다요", "김정태 | 2020.06.23"));
            sns.Add(new Article("가정통신문이요", "전준혁 | 2020.06.23"));
            sns.Add(new Article("테스트 가정통신문", "최정원 | 2020.06.23"));
            sns.Add(new Article("알립니다요", "김정태 | 2020.06.23"));
            sns.Add(new Article("가정통신문이요", "전준혁 | 2020.06.23"));
            sns.Add(new Article("테스트 가정통신문", "최정원 | 2020.06.23"));
            sns.Add(new Article("알립니다요", "김정태 | 2020.06.23"));
            sns.Add(new Article("가정통신문이요", "전준혁 | 2020.06.23"));
            sns.Add(new Article("테스트 가정통신문", "최정원 | 2020.06.23"));
            sns.Add(new Article("알립니다요", "김정태 | 2020.06.23"));
            sns.Add(new Article("가정통신문이요", "전준혁 | 2020.06.23"));
            sns.Add(new Article("테스트 가정통신문", "최정원 | 2020.06.23"));
            sns.Add(new Article("알립니다요", "김정태 | 2020.06.23"));
            sns.Add(new Article("가정통신문이요", "전준혁 | 2020.06.23"));
            sns.Add(new Article("테스트 가정통신문", "최정원 | 2020.06.23"));
            sns.Add(new Article("알립니다요", "김정태 | 2020.06.23"));
            sns.Add(new Article("가정통신문이요", "전준혁 | 2020.06.23"));
            SNList.ItemsSource = sns;

            List<Article> appNotices = new List<Article>();
            appNotices.Add(new Article("앱 버전 1.0.0", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("앱 개발중", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("테스트테스트", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("앱 버전 1.0.0", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("앱 개발중", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("테스트테스트", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("앱 버전 1.0.0", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("앱 개발중", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("테스트테스트", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("앱 버전 1.0.0", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("앱 개발중", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("테스트테스트", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("앱 버전 1.0.0", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("앱 개발중", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("테스트테스트", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("앱 버전 1.0.0", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("앱 개발중", "관리자 | 2020.06.23"));
            appNotices.Add(new Article("테스트테스트", "관리자 | 2020.06.23"));
            AppNoticeList.ItemsSource = appNotices;
            #endregion
        }
        #endregion

        #region 함수
        #region 브라우저 열기
        private void OpenBrowser(string url)
        {
            var browser = DependencyService.Get<IBrowser>();
            browser.Open(url);
        }
        #endregion
        #endregion

        #region 애니메이션
        #region 웹사이트 바로가기 버튼 클릭
        private async Task ImageButtonAnimation(ImageButton b)
        {
            await b.ScaleTo(0.8, 150, Easing.SinOut);
            await b.ScaleTo(1, 100, Easing.SinIn);
        }
        #endregion

        #region 보기 버튼 클릭
        private async void ViewButtonAnimation(Button b)
        {
            await b.ColorTo(Color.FromRgb(248, 248, 255), Color.FromRgb(78, 78, 78), c => b.BackgroundColor = c, 75);
            await b.ColorTo(Color.FromRgb(43, 43, 43), Color.White, c => b.TextColor = c, 50);
        }
        #endregion

        #region 게시판 글 보이기
        private async Task ViewArticleAnimation(ListView lv)
        {
            await lv.TranslateTo(300, 0, 1, Easing.SpringOut);
            lv.IsVisible = true;
            await lv.TranslateTo(0, 0, 500, Easing.SpringOut);
        }
        #endregion
        #endregion

        #region 이스터에그
        #region 한양 아이콘 탭
        private async void HanyangIcon_Tapped(object sender, EventArgs e)
        {
            if (!homepageButtonRotate)
            {
                homepageButtonRotate = true;
                await HanyangIcon.RotateTo(360, 500, Easing.SinOut);
                await HanyangIcon.RotateTo(0, 400, Easing.SinIn);
                homepageButtonRotate = false;
            }
        }
        #endregion
        #endregion

        #region 버튼 클릭
        #region 학교 홈페이지 바로가기 버튼
        private async void HomepageButton_Clicked(object sender, EventArgs e)
        {
            await ImageButtonAnimation(sender as ImageButton);
            OpenBrowser("http://hanyang.sen.hs.kr/index.do");
        }
        #endregion

        #region 한양뉴스 바로가기 버튼
        private async void NewsButton_Clicked(object sender, EventArgs e)
        {
            await ImageButtonAnimation(sender as ImageButton);
            OpenBrowser("http://hanyangnews.com/");
        }
        #endregion

        #region 코로나맵 바로가기 버튼
        private async void CoronamapButton_Clicked(object sender, EventArgs e)
        {
            await ImageButtonAnimation(sender as ImageButton);
            OpenBrowser("https://coronamap.site/");
        }
        #endregion

        #region 공지사항 보기 버튼
        private async void ViewNoticeButton_Clicked(object sender, EventArgs e)
        {
            if (!task && view != "notice")
            {
                task = true;
                view = "notice";
                SNList.IsVisible = false;
                AppNoticeList.IsVisible = false;

                ViewButtonAnimation(sender as Button);

                ViewSNButton.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewAppNoticeButton.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewSNButton.TextColor = Color.FromRgb(43, 43, 43);
                ViewAppNoticeButton.TextColor = Color.FromRgb(43, 43, 43);

                await ViewArticleAnimation(NoticeList);
                task = false;
            }
        }
        #endregion

        #region 가정통신문 보기 버튼
        private async void ViewSNButton_Clicked(object sender, EventArgs e)
        {
            if (!task && view != "school_newsletter")
            {
                task = true;
                view = "school_newsletter";
                NoticeList.IsVisible = false;
                AppNoticeList.IsVisible = false;

                ViewButtonAnimation(sender as Button);

                ViewNoticeButton.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewAppNoticeButton.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewNoticeButton.TextColor = Color.FromRgb(43, 43, 43);
                ViewAppNoticeButton.TextColor = Color.FromRgb(43, 43, 43);

                await ViewArticleAnimation(SNList);
                task = false;
            }
        }
        #endregion

        #region 한양이(앱 공지사항) 보기 버튼
        private async void ViewAppNoticeButton_Clicked(object sender, EventArgs e)
        {
            if (!task && view != "app_notice")
            {
                task = true;
                view = "app_notice";
                NoticeList.IsVisible = false;
                SNList.IsVisible = false;

                ViewButtonAnimation(sender as Button);

                ViewNoticeButton.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSNButton.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewNoticeButton.TextColor = Color.FromRgb(43, 43, 43);
                ViewSNButton.TextColor = Color.FromRgb(43, 43, 43);

                await ViewArticleAnimation(AppNoticeList);
                task = false;
            }
        }
        #endregion

        #region 새로고침 버튼
        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            await ImageButtonAnimation(sender as ImageButton);
        }
        #endregion
        #endregion

        #region ListView 아이템 탭
        private void NoticeList_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }

        private void SNList_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }

        private void AppNoticeList_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }
        #endregion
    }
}