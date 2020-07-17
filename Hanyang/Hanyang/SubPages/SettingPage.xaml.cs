#region API 참조
using Hanyang.Controller;
using Hanyang.Popup;

using Newtonsoft.Json.Linq;

using Rg.Plugins.Popup.Services;

using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace Hanyang.SubPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        #region 변수
        private bool task; // 다른 작업 중인지 확인
        #endregion

        #region 생성자
        public SettingPage()
        {
            InitializeComponent();

            AnimationSwitch.IsToggled = App.Animation;
        }
        #endregion

        #region 버튼 클릭
        #region 프로필 버튼
        private async void ProfileButton_Clicked(object sender, EventArgs e)
        {
            if (!task)
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
                                { "Name", arg.Name }
                                };
                                controller.Add(dict);
                            }
                            catch (Exception ex)
                            {
                                await DisplayAlert("오류", ex.Message, "확인");
                            }
                        }
                        else
                        {
                            var jsonObj = new JObject(
                                new JProperty("Grade", arg.Grade),
                                new JProperty("Class", arg.Class),
                                new JProperty("Number", arg.Number),
                                new JProperty("Name", arg.Name)
                                );
                            await controller.Write(jsonObj);
                        }

                        TabbedHomePage.ins.MyInfoUpdate(arg.Grade, arg.Class, arg.Number, arg.Name);

                        await DisplayAlert("프로필 설정", "입력된 정보가 저장되었습니다.", "확인");
                    }
                };

                await PopupNavigation.Instance.PushAsync(popup, App.Animation);
                task = false;
            }
        }
        #endregion

        #region 애니메이션 버튼
        private void AnimationButton_Clicked(object sender, EventArgs e)
        {
            AnimationSwitch.IsToggled = !AnimationSwitch.IsToggled;
        }
        #endregion
        #endregion

        #region 스위치 토글
        #region 애니메이션 스위치
        private async void AnimationSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (!task)
            {
                task = true;

                try
                {
                    var controller = new JsonController("setting");

                    try
                    {
                        var dict = new Dictionary<string, object>();
                        dict.Add("Animation", AnimationSwitch.IsToggled);
                        controller.Add(dict);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("오류", ex.Message, "확인");
                    }

                    var read = controller.Read();

                    App.Animation = Convert.ToBoolean(read["Animation"]);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("오류", "MainPage - GetSetting\n" + ex.Message, "확인");
                }
                task = false;
            }
        }
        #endregion
        #endregion
    }
}