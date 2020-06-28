#region API 참조
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

using System;

using Xamarin.Forms.Xaml;
#endregion

namespace Hanyang.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileSettingPopup : PopupPage
    {
        #region 생성자
        public ProfileSettingPopup()
        {
            InitializeComponent();

            #region UI 설정
            for (int i = 1; i <= 3; i++)
                Grade.Items.Add(i + "학년");
            Grade.SelectedIndex = 0;

            for (int i = 1; i <= 12; i++)
                Class.Items.Add(i + "반");
            Class.SelectedIndex = 0;

            for (int i = 1; i <= 26; i++)
                Number.Items.Add(i + "번");
            Number.SelectedIndex = 0;
            #endregion
        }
        #endregion

        public EventHandler<PopupResult> OnPopupSaved;

        #region 버튼 클릭
        #region 취소 버튼
        private async void CancleButton_Clicked(object sender, System.EventArgs e)
        {
            OnPopupSaved?.Invoke(this, new PopupResult { Result = false });
            await PopupNavigation.Instance.PopAsync(true);
        }
        #endregion

        #endregion

        private async void SaveButton_Clicked(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Name.Text))
            {
                OnPopupSaved?.Invoke(this, new PopupResult
                {
                    Result = true,
                    Grade = Convert.ToInt32(Grade.SelectedItem.ToString().Replace("학년", "")),
                    Class = Convert.ToInt32(Class.SelectedItem.ToString().Replace("반", "")),
                    Number = Convert.ToInt32(Number.SelectedItem.ToString().Replace("번", "")),
                    Name = Name.Text
                });
                await PopupNavigation.Instance.PopAsync(true);
            }
            else
            {
                await DisplayAlert("프로필 설정", "이름을 입력해주세요.", "확인");
                Name.Focus();
            }
        }
    }

    public class PopupResult
    {
        public bool Result { get; set; }
        public int Grade { get; set; }
        public int Class { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
    }
}