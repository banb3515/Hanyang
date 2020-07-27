#region API 참조
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace Hanyang.SubPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewPage : ContentPage
    {
        private bool isLoading;
        private bool task;

        public WebViewPage(string title, string url)
        {
            isLoading = false;
            task = false;

            InitializeComponent();

            Title = title;

            WebView.Source = url;
        }

        #region 애니메이션
        #region 이미지 버튼 클릭
        private async Task ImageButtonAnimation(ImageButton b)
        {
            await b.ScaleTo(0.8, 150, Easing.SinOut);
            await b.ScaleTo(1, 100, Easing.SinIn);
        }
        #endregion
        #endregion

        #region 버튼 클릭
        #region 뒤로가기 버튼
        private async void GoBack_Clicked(object sender, EventArgs e)
        {
            if (!task)
            {
                task = true;
                await ImageButtonAnimation(sender as ImageButton);
                WebView.GoBack();
                task = false;
            }
        }
        #endregion

        #region 앞으로가기 버튼
        private async void GoForward_Clicked(object sender, EventArgs e)
        {
            if (!task)
            {
                task = true;
                await ImageButtonAnimation(sender as ImageButton);
                WebView.GoForward();
                task = false;
            }
        }
        #endregion
        #endregion

        #region 페이지 이동
        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (!isLoading && Title == "자가 진단")
            {
                isLoading = true;

                string birthDate = (DateTime.Now.Year - (App.Grade + 15)).ToString().Substring(2) +
                    App.BirthMonth.ToString().PadLeft(2, '0') + 
                    App.BirthDay.ToString().PadLeft(2, '0');

                string script = "document.getElementById(\"schulNm\").value = \"한양공업고등학교\";" +
                    "document.getElementById(\"schulCode\").value = \"B100000601\";" +
                    $"document.getElementById(\"pName\").value = \"{App.Name}\";" +
                    $"document.getElementById(\"frnoRidno\").value = \"{birthDate}\";" +
                    "document.getElementById(\"btnConfirm\").click();";
                await WebView.EvaluateJavaScriptAsync(script);
            }
        }
        #endregion
    }
}