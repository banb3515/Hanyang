#region API 참조
using Hanyang.Interface;
using System;
using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
#endregion

namespace Hanyang
{
    [DesignTimeVisible(false)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        #region 생성자
        public MainPage()
        {
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            InitializeComponent();
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

        #region 버튼 클릭
        #region 학교 홈페이지 바로가기 버튼
        private void HomepageButton_Clicked(object sender, EventArgs e)
        {
            StartAnimation((Xamarin.Forms.ImageButton)sender);
            OpenBrowser("http://hanyang.sen.hs.kr/index.do");
        }
        #endregion

        #region 한양뉴스 바로가기 버튼
        private void NewsButton_Clicked(object sender, EventArgs e)
        {
            StartAnimation((Xamarin.Forms.ImageButton)sender);
            OpenBrowser("http://hanyangnews.com/");
        }
        #endregion

        #region 코로나맵 바로가기 버튼
        private void CoronamapButton_Clicked(object sender, EventArgs e)
        {
            StartAnimation((Xamarin.Forms.ImageButton)sender);
            OpenBrowser("https://coronamap.site/");
        }
        #endregion
        #endregion

        #region 애니메이션
        #region 버튼 클릭
        private async void StartAnimation(Xamarin.Forms.ImageButton b)
        {
            await b.ScaleTo(0.8, 50, Easing.SinOut);
            await b.ScaleTo(1, 50, Easing.SinIn);
        }
        #endregion
        #endregion
    }
}
