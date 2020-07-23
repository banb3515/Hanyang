#region API 참조
using Hanyang.Controller;
using Hanyang.Interface;
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

                        TabbedHomePage.ins.MyInfoUpdate();
                        _ = TabbedSchedulePage.GetInstance().ViewScheduleAnimation();

                        DependencyService.Get<IToastMessage>().Longtime("입력된 정보가 저장되었습니다.");
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

                var controller = new JsonController("setting");

                try
                {
                    var dict = new Dictionary<string, object>();
                    dict.Add("Animation", AnimationSwitch.IsToggled);
                    controller.Add(dict);
                }
                catch (Exception ex)
                {
                    await MainPage.GetInstance().ErrorAlert("설정 변경", "애니메이션 설정을 변경하는 도중 오류가 발생했습니다.\n" + ex.Message);
                }

                var read = controller.Read();

                App.Animation = Convert.ToBoolean(read["Animation"]);

                task = false;
            }
        }
        #endregion
        #endregion
    }
}