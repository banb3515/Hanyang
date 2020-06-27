#region API 참조
using Xamarin.Forms;
#endregion

namespace Hanyang.Others
{
    public class ExtendedViewCell : ViewCell
    {
        #region 변수
        public static readonly BindableProperty SelectedBackgroundColorProperty = BindableProperty.Create("SelectedBackgroundColor", typeof(Color), typeof(ExtendedViewCell), Color.Default);

        public Color SelectedBackgroundColor
        {
            get { return (Color)GetValue(SelectedBackgroundColorProperty); }
            set { SetValue(SelectedBackgroundColorProperty, value); }
        }
        #endregion
    }
}