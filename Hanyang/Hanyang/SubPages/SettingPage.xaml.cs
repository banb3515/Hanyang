#region API 참조
using Hanyang.Popup;

using Rg.Plugins.Popup.Services;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace Hanyang.SubPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        #region 생성자
        public SettingPage()
        {
            InitializeComponent();
        }
        #endregion

        #region 버튼 클릭
        #region 프로필 버튼
        private async void ProfileButton_Clicked(object sender, EventArgs e)
        {
            var popup = new ProfileSettingPopup();
            
            popup.OnPopupSaved += async (s, arg) =>
            {
                if (arg.Result)
                {
                    // 로컬 저장소에 저장

                    await DisplayAlert("프로필 설정", "입력된 정보가 저장되었습니다.", "확인");

                    TabbedHomePage.ins.MyInfoUpdate(arg.Grade, arg.Class, arg.Number, arg.Name);
                }
            };

            await PopupNavigation.Instance.PushAsync(popup, true);
        }
        #endregion
        #endregion
    }
}