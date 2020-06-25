#region API 참조
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace Hanyang
{
    public partial class App : Application
    {
        #region 변수
        public const string VERSION = "1.0"; // 앱 버전
        #endregion

        #region 생성자
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
        #endregion

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
