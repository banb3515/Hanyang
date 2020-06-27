#region API 참조
using Hanyang.Interface;
using Hanyang.Droid.Request;

using System;

using Android.Content;

using Xamarin.Forms;
#endregion

[assembly: Dependency(typeof(Browser))]
namespace Hanyang.Droid.Request
{
    class Browser : IBrowser
    {
        #region 브라우저 열기
        [Obsolete]
        public bool Open(string url)
        {
            var uri = Android.Net.Uri.Parse(url);
            var intent = new Intent(Intent.ActionView, uri);
            Forms.Context.StartActivity(intent);
            return true;
        }
        #endregion
    }
}