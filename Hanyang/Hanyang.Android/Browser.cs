#region API 참조
using System;
using Android.Content;
using Hanyang.Droid;
using Hanyang.Interface;
using Xamarin.Forms;
#endregion

[assembly: Dependency(typeof(Browser))]
namespace Hanyang.Droid
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