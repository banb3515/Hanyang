#region API 참조
using System;
using System.IO;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

using Java.Lang;
#endregion

namespace Hanyang.Droid.Activitys
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

            var progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            int progressBarStatus = 0;

            var intent = new Intent(this, typeof(MainActivity));

            new System.Threading.Thread(new ThreadStart(delegate
            {
                System.Threading.Thread.Sleep(500);

                while (progressBarStatus < 150)
                {
                    progressBarStatus += 1;
                    progressBar.Progress = progressBarStatus;
                    System.Threading.Thread.Sleep(10);
                }

                StartActivity(intent);
                Finish();
            })).Start();
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