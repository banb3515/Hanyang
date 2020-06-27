#region API 참조
using System.IO;
using System.Timers;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

using Felipecsl.GifImageViewLib;
#endregion

namespace Hanyang.Droid.Activity
{
    [Activity(Label = "한양이", Icon = "@drawable/icon", NoHistory = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar", 
        MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashActivity : AppCompatActivity
    {
        #region OnCreate
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashScreen);
            var gifImageView = FindViewById<GifImageView>(Resource.Id.splash);
            var progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            Stream stream = Assets.Open("splash_screen.gif");
            byte[] bytes = ConvertFileToByteArray(stream);
            gifImageView.SetBytes(bytes);
            gifImageView.StartAnimation();

            Timer timer = new Timer();
            timer.Interval = 3000;
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        #endregion

        #region 이벤트
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
        }
        #endregion

        #region ConvertFileToByteArray
        private byte[] ConvertFileToByteArray(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using(MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }
        #endregion
    }
}