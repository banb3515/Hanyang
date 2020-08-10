#region API 참조
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace Hanyang.SubPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ArticlePage : ContentPage
    {
        #region 변수
        private bool task; // 다른 작업 중인지 확인
        Dictionary<string, string> article;
        int id;
        #endregion

        #region 생성자
        public ArticlePage(string type, int id)
        {
            #region 변수 초기화
            task = false;
            #endregion

            InitializeComponent();

            #region 글 가져오기
            article = null;
            this.id = id;

            var title = "";

            switch (type)
            {
                case "SchoolNotice":
                    article = App.SchoolNotice[id.ToString()];
                    title = "공지사항";
                    break;
                case "SchoolNewsletter":
                    article = App.SchoolNewsletter[id.ToString()];
                    title = "가정통신문";
                    break;
                case "AppNotice":
                    article = App.AppNotice[id.ToString()];
                    title = "앱 공지사항";
                    break;
            }
            #endregion

            #region UI 설정
            Title = title;

            var descText = "⊙ 첨부파일은 웹에서 다운로드해 주시기 바랍니다.";
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                descText += "\n⊙ 사진은 인터넷에 연결된 상태에서만 보입니다.";
            Desc.Text = descText;

            ArticleTitle.Text = article["Title"];
            ArticleWriter.Text = article["Name"];
            ArticleDate.Text = article["Date"];

            var htmlSource = new HtmlWebViewSource();
            htmlSource.Html = article["Content"];
            WebView.Source = htmlSource;

            if(Title == "앱 공지사항")
            {
                ViewWebLine.IsVisible = false;
                ViewWeb.IsVisible = false;
                ViewWebDesc.IsVisible = false;
            }
            #endregion
        }
        #endregion

        #region 버튼 클릭
        #region 맨 위로 이동 버튼
        private async void TopButton_Clicked(object sender, System.EventArgs e)
        {
            await ImageButtonAnimation(sender as ImageButton);

            if (!task)
            {
                task = true;
                await WebView.EvaluateJavaScriptAsync("window.scrollTo(0, 0)");
                task = false;
            }
        }
        #endregion
        #endregion

        #region 이미지 버튼 클릭
        private async Task ImageButtonAnimation(ImageButton b)
        {
            await b.ScaleTo(0.8, 150, Easing.SinOut);
            await b.ScaleTo(1, 100, Easing.SinIn);
        }
        #endregion

        #region 새 페이지 열기
        private async void NewPage(Page page)
        {
            await Navigation.PushAsync(page);
        }
        #endregion

        #region 웹에서 보기 탭
        private void ViewWeb_Tapped(object sender, System.EventArgs e)
        {
            var url = "";

            if (Title == "공지사항")
                url = "http://hanyang.sen.hs.kr/8665/subMenu.do";
            if (Title == "가정통신문")
                url = "http://hanyang.sen.hs.kr/8666/subMenu.do";

            NewPage(new WebViewPage(Title, url));
        }
        #endregion
    }
}