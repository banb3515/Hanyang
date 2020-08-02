#region API 참조
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

using Rg.Plugins.Popup.Services;

using LabelHtml.Forms.Plugin.Droid;
#endregion

namespace Hanyang.Droid.Activitys
{
    [Activity(Theme = "@style/MainTheme", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private static MainActivity instance;

        #region OnCreate
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            HtmlLabelRenderer.Initialize();
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            instance = this;
        }
        #endregion

        #region OnRequestPermissionsResult
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        #endregion

        #region 디바이스 Back 버튼
        public override async void OnBackPressed()
        {
            if (((App)Xamarin.Forms.Application.Current).PromptToConfirmExit)
            {
                using (var alert = new AlertDialog.Builder(this))
                {
                    alert.SetTitle("한양이");
                    alert.SetMessage("앱을 종료하시겠습니까?");
                    alert.SetPositiveButton("예", (sender, args) => { FinishAffinity(); });
                    alert.SetNegativeButton("아니요", (sender, args) => { });

                    var dialog = alert.Create();
                    dialog.Show();
                }
                return;
            }

            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                await PopupNavigation.Instance.PopAsync();
                return;
            }

            base.OnBackPressed();
        }
        #endregion

        #region 인스턴스 가져오기
        public static MainActivity GetInstance()
        {
            return instance;
        }
        #endregion
    }

    #region Back 버튼 핸들러
    public interface IBackButtonHandler
    {
        bool HandleBackButton();
    }
    #endregion
}