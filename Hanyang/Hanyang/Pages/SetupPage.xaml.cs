#region API 참조
using Hanyang.Controller;
using Hanyang.Interface;
using Hanyang.Popup;

using Newtonsoft.Json.Linq;

using Rg.Plugins.Popup.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace Hanyang.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetupPage : ContentPage
    {
        const int PAGE_MIN = 1;
        const int PAGE_MAX = 2;

        private List<bool> pagesAnimation;

        private int page;
        private bool task;

        public SetupPage()
        {
            pagesAnimation = new List<bool>();

            for (int i = 1; i <= PAGE_MAX; i++)
                pagesAnimation.Add(false);

            page = PAGE_MIN;
            task = false;

            InitializeComponent();

            NavigatePage();
        }

        #region 애니메이션
        #region 타이틀
        private async Task TitleAnimation()
        {
            await ContentTitle.FadeTo(1, 1250, Easing.SpringIn);
        }
        #endregion

        #region 버튼
        private async Task ButtonAnimation()
        {
            await ButtonLayout.FadeTo(1, 1000, Easing.SpringIn);
        }
        #endregion

        #region 페이지 1
        private async void Page1Animation()
        {
            ContentTitle.Text = "환영합니다.";
            Page1.IsVisible = true;
            Back.IsVisible = false;
            Forward.IsVisible = true;

            if (pagesAnimation[page - 1])
                return;

            ContentTitle.Opacity = 0;
            ButtonLayout.Opacity = 0;

            Page1Label1.Opacity = 0;
            Page1Label2.Opacity = 0;
            Page1Label3.Opacity = 0;
            Page1Label4.Opacity = 0;
            Page1Label5.Opacity = 0;
            Page1Label6.Opacity = 0;
            Page1Label7.Opacity = 0;

            pagesAnimation[page - 1] = true;

            await TitleAnimation();
            await Task.Delay(250);
            await Page1Label1.FadeTo(1, 1000, Easing.SpringIn);
            await Task.Delay(250);
            await Page1Label2.FadeTo(1, 1000, Easing.SpringIn);
            await Task.Delay(250);
            await Page1Label3.FadeTo(1, 1000, Easing.SpringIn);
            await Task.Delay(250);
            await Page1Label4.FadeTo(1, 1000, Easing.SpringIn);
            await Task.Delay(250);
            await Page1Label5.FadeTo(1, 1000, Easing.SpringIn);
            await Task.Delay(250);
            await Page1Label6.FadeTo(1, 1000, Easing.SpringIn);
            await Task.Delay(250);
            await Page1Label7.FadeTo(1, 1000, Easing.SpringIn);
            await Task.Delay(250);
            await ButtonAnimation();
        }
        #endregion

        #region 페이지 2
        private async void Page2Animation()
        {
            ContentTitle.Text = "프로필 설정";

            Page1.IsVisible = false;
            Page2.IsVisible = true;

            Back.IsVisible = true;
            if(!Page2Label3.IsVisible)
                Forward.IsVisible = false;

            if (pagesAnimation[page - 1])
                return;

            ContentTitle.Opacity = 0;
            ButtonLayout.Opacity = 0;

            Page2Label1.Opacity = 0;
            Page2Label2.Opacity = 0;

            pagesAnimation[page - 1] = true;

            await TitleAnimation();
            await Task.Delay(250);
            await Page2Label1.FadeTo(1, 1000, Easing.SpringIn);
            await Task.Delay(250);
            await Page2Label2.FadeTo(1, 1000, Easing.SpringIn);
            await Task.Delay(250);
            await ButtonAnimation();
        }
        #endregion
        #endregion

        #region 함수
        #region 페이지 이동
        private void NavigatePage()
        {
            if (page > PAGE_MIN)
                Back.IsVisible = true;

            if (page == PAGE_MAX)
                Forward.Text = "완료";
            else
                Forward.Text = "다음";

            switch (page)
            {
                case 1:
                    Page2.IsVisible = false;
                    Page1Animation();
                    break;
                case 2:
                    Page1.IsVisible = false;
                    Page2Animation();
                    break;
            }
        }
        #endregion
        #endregion

        #region 버튼 클릭
        #region 이전 버튼
        private void Back_Clicked(object sender, System.EventArgs e)
        {
            if(!task && page > PAGE_MIN)
            {
                task = true;
                page--;
                NavigatePage();
                task = false;
            }
        }
        #endregion

        #region 다음 버튼
        private async void Forward_Clicked(object sender, System.EventArgs e)
        {
            if (!task)
            {
                if(page == PAGE_MAX)
                {
                    task = true;

                    Back.IsVisible = false;
                    Forward.IsVisible = false;

                    Page2Label4.IsVisible = true;
                    Page2Label4.Opacity = 0;

                    await Page2Label4.FadeTo(1, 1000, Easing.SpringIn);

                    var controller = new JsonController("setting");

                    try
                    {
                        var dict = new Dictionary<string, object>();
                        dict.Add("Setup", true);
                        controller.Add(dict);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("초기 설정", "초기 설정을 완료하는 도중 오류가 발생했습니다.\n" + ex.Message, "확인");
                    }

                    var read = controller.Read();

                    App.Setup = Convert.ToBoolean(read["Setup"]);

                    Application.Current.MainPage = new MainPage();

                    DependencyService.Get<IToastMessage>().Longtime("이제 한양이 앱을 사용해보세요 !\n설정 탭에서 여러 가지를 설정할 수 있습니다.");

                    task = false;
                }
                else if (page < PAGE_MAX)
                {
                    task = true;
                    page++;
                    NavigatePage();
                    task = false;
                }
            }
        }
        #endregion
        #endregion

        #region 프로필 설정하기 탭
        private async void GoSetting_Clicked(object sender, EventArgs e)
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
                                await DisplayAlert("설정", "설정을 완료하는 도중 오류가 발생했습니다.\n" + ex.Message, "확인");
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

                        DependencyService.Get<IToastMessage>().Longtime("입력된 정보가 저장되었습니다.");

                        Page2Label2.Text = "프로필 다시 설정하기";
                        Forward.IsVisible = true;

                        Page2Label3.IsVisible = true;
                        Page2Label3.Opacity = 0;

                        await Page2Label3.FadeTo(1, 1000, Easing.SpringIn);
                    }
                };

                await PopupNavigation.Instance.PushAsync(popup, App.Animation);

                task = false;
            }
        }
        #endregion
    }
}