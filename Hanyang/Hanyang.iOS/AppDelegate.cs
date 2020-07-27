#region API 참조
using Foundation;
using LabelHtml.Forms.Plugin.iOS;
using UIKit;
#endregion

namespace Hanyang.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        #region FinishedLaunching
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();

            HtmlLabelRenderer.Initialize();
            global::Xamarin.Forms.Forms.Init();

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
        #endregion
    }
}