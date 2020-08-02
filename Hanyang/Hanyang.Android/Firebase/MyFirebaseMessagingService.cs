#region API 참조
using Hanyang.Droid.Helper;

using Android.App;
using Android.Content;

using Firebase.Messaging;
#endregion

namespace Hanyang.Droid.Firebase
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public MyFirebaseMessagingService()
        {

        }

        [System.Obsolete]
        #pragma warning disable CS0809
        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            new NotificationHelper().CreateNotification(message.GetNotification().Title, message.GetNotification().Body);
        }
    }
}