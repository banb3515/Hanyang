#region API 참조
using Hanyang.Animations;
using Hanyang.Models;
using Hanyang.Interface;
using Hanyang.SubPages;
using Hanyang.Controller;
using Hanyang.Popup;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Newtonsoft.Json.Linq;

using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
#endregion

namespace Hanyang
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedHomePage : ContentPage
    {
        #region 변수
        private bool task; // 다른 작업 중인지 확인
        private string view; // 현재 보고있는 레이아웃
        private bool hanyangLogoRotate; // 한양공고 로고 애니메이션 작동중인지 확인
        private bool myInfoSet; // 나의 정보가 설정되어있는지 확인
        public static TabbedHomePage ins;
        #endregion

        #region 생성자
        public TabbedHomePage()
        {
            #region 변수 초기화
            task = false;
            view = "notice";
            hanyangLogoRotate = false;
            myInfoSet = false;
            #endregion

            ins = this;

            InitializeComponent();

            MyInfoUpdate();

            #region 글 목록 임시 생성
            var notices = new List<Article>(App.GetNotices().Values);
            notices.Reverse();
            var sns = new List<Article>(App.GetSchoolNewsletters().Values);
            sns.Reverse();
            var appNotices = new List<Article>(App.GetAppNotices().Values);
            appNotices.Reverse();

            NoticeList.ItemsSource = notices;
            SNList.ItemsSource = sns;
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

        #region 새 페이지 열기
        private async void NewPage(Page page)
        {
            await Navigation.PushAsync(page);
        }
        #endregion

        #region 나의 정보 UI 업데이트
        public void MyInfoUpdate()
        {
            var grade = App.Grade;
            var _class = App.Class;
            var number = App.Number;
            var name = App.Name;

            Device.BeginInvokeOnMainThread(() =>
            {
                MyInfoText.Text = grade + "학년 " + _class + "반 " + number + "번, " + name;

                string dep = "NONE";

                if (_class != 0)
                {
                    if (_class >= 1 && _class <= 2)
                        dep = "건설정보";
                    else if (_class >= 3 && _class <= 4)
                        dep = "건축";
                    else if (_class >= 5 && _class <= 6)
                        dep = "자동화기계";
                    else if (_class >= 7 && _class <= 8)
                        dep = "디지털전자";
                    else if (_class >= 9 && _class <= 10)
                        dep = "자동차";
                    else if (_class >= 11 && _class <= 12)
                        dep = "컴퓨터네트워크";
                }

                if (dep != "NONE")
                {
                    MyDepartment.Text = dep + "과";
                    MyDepartment.TextColor = Color.White;
                    MyDepartment.TextDecorations = TextDecorations.None;
                    myInfoSet = true;
                }
            });
        }
        #endregion
        #endregion

        #region 애니메이션
        #region 이미지 버튼 클릭
        private async Task ImageButtonAnimation(ImageButton b)
        {
            await b.ScaleTo(0.8, 150, Easing.SinOut);
            await b.ScaleTo(1, 100, Easing.SinIn);
        }
        #endregion

        #region 보기 버튼 클릭
        private async void ViewButtonAnimation(Button b)
        {
            if (App.Animation)
            {
                await b.ColorTo(Color.FromRgb(248, 248, 255), Color.FromRgb(78, 78, 78), c => b.BackgroundColor = c, 75);
                await b.ColorTo(Color.FromRgb(43, 43, 43), Color.White, c => b.TextColor = c, 50);
            }
            else
            {
                b.BackgroundColor = Color.FromRgb(78, 78, 78);
                b.TextColor = Color.White;
            }
        }
        #endregion

        #region 게시판 글 보이기
        private async Task ViewArticleAnimation(ListView lv)
        {
            if (App.Animation)
            {
                await lv.TranslateTo(300, 0, 1, Easing.SpringOut);
                lv.IsVisible = true;
                await lv.TranslateTo(0, 0, 500, Easing.SpringOut);
            }
            else
                lv.IsVisible = true;
        }
        #endregion
        #endregion

        #region 이스터에그
        #region 한양 아이콘 탭
        private async void HanyangIcon_Tapped(object sender, EventArgs e)
        {
            if (!hanyangLogoRotate)
            {
                hanyangLogoRotate = true;
                await HanyangIcon.RotateTo(360, 500, Easing.SinOut);
                await HanyangIcon.RotateTo(0, 400, Easing.SinIn);
                hanyangLogoRotate = false;
            }
        }
        #endregion
        #endregion

        #region 버튼 클릭
        #region 학교 홈페이지 바로가기 버튼
        private async void HomepageButton_Clicked(object sender, EventArgs e)
        {
            await MainPage.GetInstance().ErrorAlert("테스트", "에러 메시지입니다.");
            //await ImageButtonAnimation(sender as ImageButton);
            //OpenBrowser("http://hanyang.sen.hs.kr/index.do");
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

        #region 자가 진단 바로가기 버튼
        private async void SelfDiagnosisButton_Clicked(object sender, EventArgs e)
        {
            await ImageButtonAnimation(sender as ImageButton);
            if(App.Name == "NONE")
            {
                await DisplayAlert("자가 진단 바로가기", "프로필 설정 후 이용 가능합니다.", "확인");
                return;
            }
            NewPage(new SelfDiagnosisPage());
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

        #region 글 목록 새로고침 버튼
        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            // 글 목록 새로고침
            await ImageButtonAnimation(sender as ImageButton);
        }
        #endregion
        #endregion

        #region ListView 아이템 탭
        #region 공지사항 목록 글 탭
        private void NoticeList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var article = e.Item as Article;
            NewPage(new ArticlePage("공지사항", article.Id));
        }
        #endregion

        #region 가정통신문 목록 글 탭
        private void SNList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var article = e.Item as Article;
            NewPage(new ArticlePage("가정통신문", article.Id));
        }
        #endregion

        #region 앱 공지사항 목록 글 탭
        private void AppNoticeList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var article = e.Item as Article;
            NewPage(new ArticlePage("앱 공지사항", article.Id));
        }
        #endregion

        #endregion

        #region 탭
        #region 프로필 설정하기 탭
        private async void GoSetting_Tapped(object sender, EventArgs e)
        {
            if(!myInfoSet && !task)
            {
                task = true;

                var popup = new ProfileSettingPopup();

                popup.OnPopupSaved += async (s, arg) =>
                {
                    if (arg.Result)
                    {
                        var controller = new JsonController("setting");
                        var read = controller.Read();

                        if (read != null)
                        {
                            try
                            {
                                var dict = new Dictionary<string, object>
                                {
                                    { "Grade", arg.Grade },
                                    { "Class", arg.Class },
                                    { "Number", arg.Number },
                                    { "Name", arg.Name },
                                    { "BirthMonth", arg.BirthMonth },
                                    { "BirthDay", arg.BirthDay }
                                };
                                controller.Add(dict);
                            }
                            catch (Exception ex)
                            {
                                await MainPage.GetInstance().ErrorAlert("설정", "설정을 완료하는 도중 오류가 발생했습니다.\n" + ex.Message);
                            }
                        }
                        else
                        {
                            var jsonObj = new JObject(
                                new JProperty("Grade", arg.Grade),
                                new JProperty("Class", arg.Class),
                                new JProperty("Number", arg.Number),
                                new JProperty("Name", arg.Name),
                                new JProperty("BirthMonth", arg.BirthMonth),
                                new JProperty("BirthDay", arg.BirthDay));
                            await controller.Write(jsonObj);
                        }

                        App.Grade = arg.Grade;
                        App.Class = arg.Class;
                        App.Number = arg.Number;
                        App.Name = arg.Name;
                        App.BirthMonth = arg.BirthMonth;
                        App.BirthDay = arg.BirthDay;

                        MyInfoUpdate();
                        _ = TabbedSchedulePage.GetInstance().ViewScheduleAnimation();

                        DependencyService.Get<IToastMessage>().Longtime("입력된 정보가 저장되었습니다.");
                    }
                };

                await PopupNavigation.Instance.PushAsync(popup, App.Animation);
                task = false;
            }
        }
        #endregion
        #endregion
    }
}