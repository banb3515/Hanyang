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
using System.Diagnostics;
using ByteSizeLib;
#endregion

namespace Hanyang.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedHomePage : ContentPage
    {
        #region 변수
        private static TabbedHomePage instance;

        private bool task; // 다른 작업 중인지 확인
        private string view; // 현재 보고있는 레이아웃
        private bool hanyangLogoRotate; // 한양공고 로고 애니메이션 작동중인지 확인
        private bool myInfoSet; // 나의 정보가 설정되어있는지 확인
        #endregion

        #region 생성자
        public TabbedHomePage()
        {
            #region 변수 초기화
            instance = this;

            task = false;
            view = "notice";
            hanyangLogoRotate = false;
            myInfoSet = false;
            #endregion

            InitializeComponent();

            MyInfoUpdate();
        }
        #endregion

        #region 함수
        #region 새 페이지 열기
        private async void NewPage(Page page)
        {
            await Navigation.PushAsync(page);
            task = false;
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

        #region 학교 공지사항 초기화
        public void InitSchoolNotice()
        {
            var noticeList = new List<Article>();
            var notices = App.SchoolNotice;

            foreach (var key in notices.Keys)
            {
                var title = notices[key]["Title"];

                var article = new Article
                {
                    Id = Convert.ToInt32(key),
                    Title = title,
                    Info = notices[key]["Name"] + " | " + notices[key]["Date"]
                };

                noticeList.Add(article);
            }

            NoticeList.ItemsSource = noticeList;
        }
        #endregion

        #region 가정통신문 초기화
        public void InitSchoolNewsletter()
        {
            var newsletterList = new List<Article>();
            var newsletters = App.SchoolNewsletter;

            foreach (var key in newsletters.Keys)
            {
                var title = newsletters[key]["Title"];

                var article = new Article
                {
                    Id = Convert.ToInt32(key),
                    Title = title,
                    Info = newsletters[key]["Name"] + " | " + newsletters[key]["Date"]
                };

                newsletterList.Add(article);
            }

            SNList.ItemsSource = newsletterList;
            SNList.IsVisible = false;
        }
        #endregion

        #region 인스턴스 가져오기
        public static TabbedHomePage GetInstance()
        {
            return instance;
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
            await ImageButtonAnimation(sender as ImageButton);
            NewPage(new WebViewPage("학교 홈페이지", "http://hanyang.sen.hs.kr/index.do"));
        }
        #endregion

        #region 한양 뉴스 바로가기 버튼
        private async void NewsButton_Clicked(object sender, EventArgs e)
        {
            await ImageButtonAnimation(sender as ImageButton);
            NewPage(new WebViewPage("한양 뉴스", "http://hanyangnews.com/"));
        }
        #endregion

        #region 코로나맵 바로가기 버튼
        private async void CoronamapButton_Clicked(object sender, EventArgs e)
        {
            await ImageButtonAnimation(sender as ImageButton);
            NewPage(new WebViewPage("코로나맵", "https://coronamap.site/"));
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
            NewPage(new WebViewPage("자가 진단", "https://eduro.sen.go.kr/stv_cvd_co00_002.do"));
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
            await ImageButtonAnimation(sender as ImageButton);

            if (!task)
            {
                task = true;

                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    var dataInfo = MainPage.GetInstance().GetDataInfo();

                    if (dataInfo == null)
                    {
                        DependencyService.Get<IToastMessage>().Longtime("서버에서 데이터를 받아오지 못했습니다.\n잠시 후 다시 시도해 주시기 바랍니다.");
                        task = false;
                        return;
                    }

                    var schoolNoticeByte = ByteSize.FromBytes(MainPage.GetInstance().GetJsonByteLength(App.SchoolNotice).Result).ToString();
                    var schoolNewsletterByte = ByteSize.FromBytes(MainPage.GetInstance().GetJsonByteLength(App.SchoolNewsletter).Result).ToString();

                    var schoolNotice = dataInfo["SchoolNotice"]["Size"];
                    var schoolNewsletter = dataInfo["SchoolNewsletter"]["Size"];

                    if (schoolNoticeByte == schoolNotice && schoolNewsletterByte == schoolNewsletter)
                    {
                        await DisplayAlert("새로고침", "이미 최신 데이터입니다.", "확인");
                        task = false;
                        return;
                    }

                    var total = ByteSize.Parse(schoolNotice);
                    total = total.Add(ByteSize.Parse(schoolNewsletter));

                    var result = await DisplayAlert("새로고침",
                        "데이터를 새로 다운받습니다.\n" +
                        "LTE/5G를 사용 중인 경우 데이터가 사용됩니다.\n\n" +
                        "※ 다운받는 데이터 크기\n" +
                        "- 공지사항: " + schoolNotice + "\n" +
                        "- 가정통신문: " + schoolNewsletter + "\n\n" +
                        "총 [" + total.ToString() + "] 를 다운받습니다.\n\n" +
                        "* 글에 포함된 사진은 다운되지 않습니다.",
                        "확인", "취소");

                    if (!result)
                    {
                        task = false;
                        return;
                    }
                    MainPage.GetInstance().GetCrawling(refresh: true);
                    task = false;
                }
                else
                {
                    DependencyService.Get<IToastMessage>().Longtime("데이터를 가져올 수 없습니다.\n" +
                        "인터넷 상태를 확인해주세요.");
                    task = false;
                }
            }
        }
        #endregion
        #endregion

        #region ListView 아이템 탭
        #region 공지사항 목록 글 탭
        private void NoticeList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!task)
            {
                task = true;
                var article = e.Item as Article;
                NewPage(new ArticlePage("SchoolNotice", article.Id));
            }
        }
        #endregion

        #region 가정통신문 목록 글 탭
        private void SNList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!task)
            {
                task = true;
                var article = e.Item as Article;
                NewPage(new ArticlePage("SchoolNewsletter", article.Id));
            }
        }
        #endregion

        #region 앱 공지사항 목록 글 탭
        private void AppNoticeList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!task)
            {
                task = true;
                //var article = e.Item as Article;
                //NewPage(new ArticlePage("앱 공지사항", article.Id));
            }
        }
        #endregion

        #endregion

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
                        MainPage.GetInstance().GetTimetable();
                        _ = TabbedSchedulePage.GetInstance().ViewScheduleAnimation();

                        DependencyService.Get<IToastMessage>().Longtime("입력된 정보가 저장되었습니다.");
                    }
                };

                await PopupNavigation.Instance.PushAsync(popup, App.Animation);
                task = false;
            }
        }
        #endregion
    }
}