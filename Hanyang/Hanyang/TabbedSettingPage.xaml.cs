#region API 참조
using Hanyang.SubPages;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace Hanyang
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedSettingPage : ContentPage
    {
        #region 생성자
        public TabbedSettingPage()
        {
            InitializeComponent();
        }
        #endregion

        #region 함수
        private async void NewPage(Page page)
        {
            await Navigation.PushAsync(page);
        }
        #endregion

        #region 버튼 클릭
        #region 앱 정보 버튼
        private void AppInfoButton_Clicked(object sender, EventArgs e)
        {
            NewPage(new SettingInfoPage());
        }
        #endregion

        #region 오픈소스 라이센스 버튼
        private void OSLButton_Clicked(object sender, EventArgs e)
        {

        }
        #endregion
        #endregion
    }
}