#region API 참조
using System.Threading.Tasks;
using System.Timers;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace Hanyang.SubPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppInfoPage : ContentPage
    {
        #region 생성자
        public AppInfoPage()
        {
            InitializeComponent();

            var timer = new Timer();
            timer.Interval = 100;
            timer.AutoReset = false;
            timer.Elapsed += AppInfoAnimation;
            timer.Start();
        }
        #endregion

        #region 애니메이션
        #region 앱 정보
        private async void AppInfoAnimation(object sender, ElapsedEventArgs e)
        {
            if (App.Animation)
            {
                #region 앱 정보
                await AppIconImage.FadeTo(1, 1500, Easing.SpringIn);

                await Task.Delay(250);

                await AppNameText.FadeTo(1, 1000, Easing.SpringIn);
                Device.BeginInvokeOnMainThread(() =>
                {
                    AppVersionText.Text = "v" + App.VERSION;
                    AppVersionText.TranslationX -= 60;
                    AppVersionText.Opacity = 1;
                });
                await AppVersionText.TranslateTo(1, 0, 1000, Easing.BounceOut);
                #endregion

                await Task.Delay(100);
                await Line1.FadeTo(1, 250, Easing.SpringIn);
                await Task.Delay(100);

                #region 사용 된 프레임워크 텍스트
                await FrameworkText.FadeTo(1, 1000, Easing.SpringIn);
                #endregion

                await Task.Delay(500);

                #region Xamarin 텍스트 로고
                Device.BeginInvokeOnMainThread(() =>
                {
                    TextLogoImage.Opacity = 1;
                    TextLogoImage.TranslationX += 250;
                });
                _ = TextLogoImage.TranslateTo(-50, 0, 1500, Easing.BounceIn);
                _ = TextLogoImage.TranslateTo(1, 0, 500, Easing.BounceOut);
                #endregion

                #region Xamarin 로고
                Device.BeginInvokeOnMainThread(() =>
                {
                    LogoImage.Opacity = 1;
                    LogoImage.TranslationX -= 100;
                });
                _ = LogoImage.TranslateTo(100, 0, 1500, Easing.BounceIn);
                _ = LogoImage.TranslateTo(1, 0, 500, Easing.BounceOut);
                #endregion

                await Task.Delay(1000);
                await Line2.FadeTo(1, 250, Easing.SpringIn);
                await Task.Delay(100);

                #region 개발자 텍스트
                await DevelopmentText.FadeTo(1, 1000, Easing.SpringIn);

                await Task.Delay(250);

                await Developer1LText.FadeTo(1, 250, Easing.SpringIn);
                await Task.Delay(100);
                await Developer1Text.FadeTo(1, 750, Easing.SpringOut);

                await Task.Delay(100);

                await Developer2LText.FadeTo(1, 250, Easing.SpringIn);
                await Task.Delay(100);
                await Developer2Text.FadeTo(1, 750, Easing.SpringOut);

                await Task.Delay(100);

                await Developer3LText.FadeTo(1, 250, Easing.SpringIn);
                await Task.Delay(100);
                await Developer3Text.FadeTo(1, 750, Easing.SpringOut);
                #endregion
            }
            else
            {
                AppIconImage.Opacity = 1;
                AppNameText.Opacity = 1;
                AppVersionText.Text = "v" + App.VERSION;
                AppVersionText.Opacity = 1;
                Line1.Opacity = 1;
                FrameworkText.Opacity = 1;
                TextLogoImage.Opacity = 1;
                LogoImage.Opacity = 1;
                Line2.Opacity = 1;
                DevelopmentText.Opacity = 1;
                Developer1LText.Opacity = 1;
                Developer1Text.Opacity = 1;
                Developer2LText.Opacity = 1;
                Developer2Text.Opacity = 1;
                Developer3LText.Opacity = 1;
                Developer3Text.Opacity = 1;
            }
        }
        #endregion
        #endregion
    }
}