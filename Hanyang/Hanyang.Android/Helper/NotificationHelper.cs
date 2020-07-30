#region API 참조
using System;
using System.Diagnostics;

using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;

using Hanyang.Droid.Activitys;
using Hanyang.Droid.Helper;
using Hanyang.Interface;

using Xamarin.Forms;
#endregion

[assembly: Dependency(typeof(NotificationHelper))]
namespace Hanyang.Droid.Helper
{
    class NotificationHelper : INotification
    {
        private Context mContext;
        private NotificationManager mNotificationManager;
        private NotificationCompat.Builder mBuilder;
        public static string NOTIFICATION_CHANNEL_ID = "3515";

        public NotificationHelper()
        {
            mContext = Android.App.Application.Context;
        }

        [Obsolete]
        public void CreateNotification(string title, string message)
        {
            try
            {
                var intent = new Intent(mContext, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                intent.PutExtra(title, message);
                var pendingIntent = PendingIntent.GetActivity(mContext, 0, intent, PendingIntentFlags.OneShot);

                var sound = Android.Net.Uri.Parse(ContentResolver.SchemeAndroidResource + "://" + mContext.PackageName + "/" + Resource.Raw.notification);
                
                var alarmAttributes = new AudioAttributes.Builder()
                    .SetContentType(AudioContentType.Sonification)
                    .SetUsage(AudioUsageKind.Notification).Build();

                mBuilder = new NotificationCompat.Builder(mContext);
                mBuilder.SetSmallIcon(Resource.Drawable.icon);
                mBuilder.SetContentTitle(title)
                        .SetSound(sound)
                        .SetAutoCancel(true)
                        .SetContentTitle(title)
                        .SetContentText(message)
                        .SetChannelId(NOTIFICATION_CHANNEL_ID)
                        .SetPriority((int)NotificationPriority.High)
                        .SetVibrate(new long[0])
                        .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate)
                        .SetVisibility((int)NotificationVisibility.Public)
                        .SetSmallIcon(Resource.Drawable.icon)
                        .SetContentIntent(pendingIntent);



                mNotificationManager = mContext.GetSystemService(Context.NotificationService) as NotificationManager;

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    NotificationImportance importance = NotificationImportance.High;

                    NotificationChannel notificationChannel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, title, importance);
                    notificationChannel.EnableLights(true);
                    notificationChannel.EnableVibration(true);
                    notificationChannel.SetSound(sound, alarmAttributes);
                    notificationChannel.SetShowBadge(true);
                    notificationChannel.Importance = NotificationImportance.High;
                    notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });

                    if (mNotificationManager != null)
                    {
                        mBuilder.SetChannelId(NOTIFICATION_CHANNEL_ID);
                        mNotificationManager.CreateNotificationChannel(notificationChannel);
                    }
                }

                mNotificationManager.Notify(0, mBuilder.Build());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}