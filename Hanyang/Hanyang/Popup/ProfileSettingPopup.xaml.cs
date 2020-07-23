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
        #region 이벤트 핸들러
        public EventHandler<PopupResult> OnPopupSaved;
        #endregion

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

            for (int i = 1; i <= 12; i++)
                BirthMonth.Items.Add(i + "월");
            BirthMonth.SelectedIndex = 0;

            for (int i = 1; i <= 31; i++)
                BirthDay.Items.Add(i + "일");
            BirthDay.SelectedIndex = 0;

            #endregion
        }
        #endregion

        #region 버튼 클릭
        #region 취소 버튼
        private async void CancleButton_Clicked(object sender, System.EventArgs e)
        {
            OnPopupSaved?.Invoke(this, new PopupResult { Result = false });
            await PopupNavigation.Instance.PopAsync(true);
        }
        #endregion

        #region 저장 버튼
        private async void SaveButton_Clicked(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Name.Text))
            {
                await DisplayAlert("프로필 설정", "이름을 입력해주세요.", "확인");
                Name.Focus();
                return;
            }

            OnPopupSaved?.Invoke(this, new PopupResult
            {
                Result = true,
                Grade = Convert.ToInt32(Grade.SelectedItem.ToString().Replace("학년", "")),
                Class = Convert.ToInt32(Class.SelectedItem.ToString().Replace("반", "")),
                Number = Convert.ToInt32(Number.SelectedItem.ToString().Replace("번", "")),
                BirthMonth = Convert.ToInt32(BirthMonth.SelectedItem.ToString().Replace("월", "")),
                BirthDay = Convert.ToInt32(BirthDay.SelectedItem.ToString().Replace("일", "")),
                Name = Name.Text
            });
            await PopupNavigation.Instance.PopAsync(App.Animation);
        }
        #endregion
        #endregion

        #region Picker 아이템 변경
        private void BirthMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            int dayLength = DateTime.DaysInMonth(DateTime.Now.Year, Convert.ToInt32(BirthMonth.SelectedItem.ToString().Replace("월", "")));

            BirthDay.Items.Clear();

            for (int i = 1; i <= dayLength; i++)
                BirthDay.Items.Add(i + "일");
            BirthDay.SelectedIndex = 0;
        }
        #endregion
    }

    #region 값 전달
    public class PopupResult
    {
        public bool Result { get; set; }

        public int Grade { get; set; }

        public int Class { get; set; }

        public int Number { get; set; }

        public string Name { get; set; }

        public int BirthMonth { get; set; }

        public int BirthDay { get; set; }
    }
    #endregion
}