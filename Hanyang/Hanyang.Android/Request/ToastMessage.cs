#region API 참조
using Hanyang.Droid.Request;
using Hanyang.Interface;

using Android.Widget;

using Xamarin.Forms;
#endregion

[assembly: Dependency(typeof(ToastMessage))]
namespace Hanyang.Droid.Request
{
    public class ToastMessage : IToastMessage
    {
        #region 긴 시간동안 표시
        public void Longtime(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }
        #endregion

        #region 짧은 시간동안 표시
        public void Shorttime(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
        }
        #endregion
    }
}