#region API 참조
using System.Globalization;
using System.Reflection;

using Syncfusion.SfCalendar.XForms;

using Xamarin.Forms;
#endregion

namespace Hanyang.Others
{
    public class CalendarBehavior : Behavior<SfCalendar>
    {
        #region 변수
        SfCalendar calendar;
        #endregion

        #region 생성자
        public CalendarBehavior() { }
        #endregion

        #region OnAttachedTo
        protected override void OnAttachedTo(SfCalendar bindable)
        {
            base.OnAttachedTo(bindable);

            calendar = bindable;

            CalendarResourceManager.Manager = new System.Resources.ResourceManager("Hanyang.Resources.Syncfusion.SfCalendar.XForms", GetType().GetTypeInfo().Assembly);
            CultureInfo.CurrentUICulture = new CultureInfo("ko");
            calendar.Locale = new CultureInfo("ko-KR");
        }
        #endregion

        #region OnDetachingFrom
        protected override void OnDetachingFrom(SfCalendar bindable)
        {
            base.OnDetachingFrom(bindable);
        }
        #endregion
    }
}